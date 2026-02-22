using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;

namespace ExamPrepar.Api.Endpoints;

public static class GenerationEndpoints
{
    public record GenerateRequest(string targetLanguage, string nativeLanguage, string level, string topic);
    public record TopicsRequest(string targetLanguage, string level);
    public record TopicsResponse(string prompt, List<string> topics);
    public record TopicTextRequest(string targetLanguage, string level);
    public record TopicTextItem(string topic, string text);
    public record TopicTextsResponse(string promptTemplate, List<TopicTextItem> items);

    public static void MapGeneration(this WebApplication app)
    {
        app.MapPost("/api/v1/generate", (GenerateRequest req) =>
        {
            // Prompt template (for future AI integration)
            var topicLabel = TopicLabel(req.topic);
            var prompt = $"Learning {req.targetLanguage}, I need to prepare to {req.level} speaking exam. Please write down full text for that topic both in {req.targetLanguage} and {req.nativeLanguage}, Topic: {topicLabel}.";

            // TODO: Integrate Azure OpenAI or OpenAI here. For now, return a deterministic stub.
            var targetText = $@"{topicLabel} – {req.level}

Dette er en øvingspassasje skrevet for nivå {req.level} på språket {req.targetLanguage}. Tema: {topicLabel}. Teksten er tilpasset muntlig eksamen og fokuserer på klare setninger og dagligtale.

Jeg vil beskrive egne erfaringer, men også gi eksempler fra hverdagen i Norge. Målet er å vise ordforråd, uttalevennlige fraser og enkel sammenheng mellom setninger.";

            var nativeText = $@"{topicLabel} – {req.level}

This is a practice passage written for level {req.level} in {req.nativeLanguage}. Topic: {topicLabel}. The text targets the speaking exam with clear sentences and everyday language.

I will describe my own experiences and give examples from daily life in Norway. The goal is to demonstrate vocabulary, easy-to-pronounce phrases, and basic sentence flow.";

            return Results.Ok(new { prompt, targetText, nativeText });
        });

        app.MapPost("/api/v1/topics", async (TopicsRequest req, IMemoryCache cache, IHttpClientFactory httpClientFactory, IConfiguration config, CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(req.targetLanguage) || string.IsNullOrWhiteSpace(req.level))
            {
                return Results.BadRequest(new { error = "targetLanguage and level are required." });
            }

            var cacheKey = BuildTopicsCacheKey(req.targetLanguage, req.level);
            if (cache.TryGetValue(cacheKey, out TopicsResponse? response))
            {
                return Results.Ok(response);
            }

            var prompt = $"Learning {req.targetLanguage} language, I need to prepare to {req.level} speaking exam. Please write down all possible topics";
            var topics = await GenerateTopicsFromLlmAsync(prompt, httpClientFactory, config, ct);
            response = new TopicsResponse(prompt, topics);

            cache.Set(cacheKey, response, TimeSpan.FromHours(12));
            return Results.Ok(response);
        });


        app.MapPost("/api/v1/topic-texts", async (TopicTextRequest req, IMemoryCache cache, IHttpClientFactory httpClientFactory, IConfiguration config, CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(req.targetLanguage) || string.IsNullOrWhiteSpace(req.level))
            {
                return Results.BadRequest(new { error = "targetLanguage and level are required." });
            }

            var cacheKey = BuildTopicTextsCacheKey(req.targetLanguage, req.level);
            if (cache.TryGetValue(cacheKey, out TopicTextsResponse? cached))
            {
                return Results.Ok(cached);
            }

            var topicsCacheKey = BuildTopicsCacheKey(req.targetLanguage, req.level);
            if (!cache.TryGetValue(topicsCacheKey, out TopicsResponse? topicsResponse))
            {
                var topicsPrompt = $"Learning {req.targetLanguage} language, I need to prepare to {req.level} speaking exam. Please write down all possible topics";
                var topics = await GenerateTopicsFromLlmAsync(topicsPrompt, httpClientFactory, config, ct);
                topicsResponse = new TopicsResponse(topicsPrompt, topics);
                cache.Set(topicsCacheKey, topicsResponse, TimeSpan.FromHours(12));
            }

            var promptTemplate = "Learning [learning] language, I need to prepare to [exam level] speaking exam. Please write down full text for that topic [topic from cache] in Norwegian";
            var items = new List<TopicTextItem>();
            foreach (var topic in topicsResponse.topics)
            {
                var prompt = $"Learning {req.targetLanguage} language, I need to prepare to {req.level} speaking exam. Please write down full text for that topic {topic} in Norwegian";
                var text = await GenerateTopicTextFromLlmAsync(prompt, httpClientFactory, config, ct);
                items.Add(new TopicTextItem(topic, text));
            }

            var response = new TopicTextsResponse(promptTemplate, items);
            cache.Set(cacheKey, response, TimeSpan.FromHours(12));
            return Results.Ok(response);
        });

        app.MapGet("/api/v1/topics", (string targetLanguage, string level, IMemoryCache cache) =>
        {
            if (string.IsNullOrWhiteSpace(targetLanguage) || string.IsNullOrWhiteSpace(level))
            {
                return Results.BadRequest(new { error = "targetLanguage and level are required." });
            }

            var cacheKey = BuildTopicsCacheKey(targetLanguage, level);
            return cache.TryGetValue(cacheKey, out TopicsResponse? response)
                ? Results.Ok(response)
                : Results.NotFound(new { error = "No cached topics found for the requested language and level." });
        });
    }

    private static string BuildTopicsCacheKey(string targetLanguage, string level) =>
        $"topics:{targetLanguage}:{level}".ToLowerInvariant();

    private static string BuildTopicTextsCacheKey(string targetLanguage, string level) =>
        $"topictexts:{targetLanguage}:{level}".ToLowerInvariant();

    private static async Task<List<string>> GenerateTopicsFromLlmAsync(string prompt, IHttpClientFactory httpClientFactory, IConfiguration config, CancellationToken ct)
    {
        var endpoint = config["Llm:Endpoint"] ?? config["AZURE_OPENAI_ENDPOINT"];
        var deployment = config["Llm:Deployment"] ?? config["AZURE_OPENAI_DEPLOYMENT"];
        var apiVersion = config["Llm:ApiVersion"] ?? config["AZURE_OPENAI_API_VERSION"] ?? "2024-02-15-preview";
        var apiKey = config["Llm:ApiKey"] ?? config["AZURE_OPENAI_API_KEY"];

        if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(deployment) || string.IsNullOrWhiteSpace(apiKey))
        {
            return GenerateTopicsStub();
        }

        var client = httpClientFactory.CreateClient();
        var baseUri = endpoint.TrimEnd('/') + "/openai/deployments/" + deployment + "/chat/completions?api-version=" + apiVersion;

        var payload = new
        {
            temperature = 0.2,
            messages = new[]
            {
                new { role = "user", content = prompt }
            }
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, baseUri)
        {
            Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
        };
        request.Headers.Add("api-key", apiKey);

        using var response = await client.SendAsync(request, ct);
        if (!response.IsSuccessStatusCode)
        {
            return GenerateTopicsStub();
        }

        var body = await response.Content.ReadAsStringAsync(ct);
        var parsed = JsonSerializer.Deserialize<OpenAiChatResponse>(body);
        var content = parsed?.Choices?.FirstOrDefault()?.Message?.Content;
        var topics = ParseTopics(content);
        return topics.Count > 0 ? topics : GenerateTopicsStub();
    }

    private static List<string> ParseTopics(string? content)
    {
        if (string.IsNullOrWhiteSpace(content)) return new List<string>();

        var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        var results = new List<string>();

        foreach (var line in lines)
        {
            var cleaned = line.Trim();
            cleaned = cleaned.TrimStart('-', '*', '•');
            cleaned = cleaned.Trim();
            cleaned = StripLeadingNumber(cleaned);
            if (!string.IsNullOrWhiteSpace(cleaned))
            {
                results.Add(cleaned);
            }
        }

        return results;
    }

    private static string StripLeadingNumber(string value)
    {
        var i = 0;
        while (i < value.Length && char.IsDigit(value[i])) i++;
        if (i == 0) return value;
        if (i < value.Length && (value[i] == '.' || value[i] == ')')) i++;
        return value[i..].Trim();
    }

    private static List<string> GenerateTopicsStub() => new()
    {
        "Introductions and personal background",
        "Daily routines and lifestyle",
        "Family, relationships, and friends",
        "Work, education, and career goals",
        "Health, fitness, and wellbeing",
        "Travel, transportation, and directions",
        "Housing, neighborhood, and local services",
        "Food, cooking, and shopping",
        "Culture, traditions, and holidays",
        "Media, technology, and social life",
        "Environment and sustainability",
        "Opinions on current social topics"
    };

    private static async Task<string> GenerateTopicTextFromLlmAsync(string prompt, IHttpClientFactory httpClientFactory, IConfiguration config, CancellationToken ct)
    {
        var endpoint = config["Llm:Endpoint"] ?? config["AZURE_OPENAI_ENDPOINT"];
        var deployment = config["Llm:Deployment"] ?? config["AZURE_OPENAI_DEPLOYMENT"];
        var apiVersion = config["Llm:ApiVersion"] ?? config["AZURE_OPENAI_API_VERSION"] ?? "2024-02-15-preview";
        var apiKey = config["Llm:ApiKey"] ?? config["AZURE_OPENAI_API_KEY"];

        if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(deployment) || string.IsNullOrWhiteSpace(apiKey))
        {
            return GenerateTopicTextStub(prompt);
        }

        var client = httpClientFactory.CreateClient();
        var baseUri = endpoint.TrimEnd('/') + "/openai/deployments/" + deployment + "/chat/completions?api-version=" + apiVersion;

        var payload = new
        {
            temperature = 0.2,
            messages = new[]
            {
                new { role = "user", content = prompt }
            }
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, baseUri)
        {
            Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
        };
        request.Headers.Add("api-key", apiKey);

        using var response = await client.SendAsync(request, ct);
        if (!response.IsSuccessStatusCode)
        {
            return GenerateTopicTextStub(prompt);
        }

        var body = await response.Content.ReadAsStringAsync(ct);
        var parsed = JsonSerializer.Deserialize<OpenAiChatResponse>(body);
        var content = parsed?.Choices?.FirstOrDefault()?.Message?.Content;
        return string.IsNullOrWhiteSpace(content) ? GenerateTopicTextStub(prompt) : content.Trim();
    }

    private static string GenerateTopicTextStub(string prompt) =>
        $"(Stub) {prompt}";

    private static string TopicLabel(string topic) => topic switch
    {
        "introductions" => "Introductions – Present yourself, your background, family.",
        "daily-routines" => "Daily routines – What you do on a typical day.",
        "hobbies-interests" => "Hobbies and interests – Sports, music, reading, etc.",
        "travel-experiences" => "Travel experiences – Places you’ve visited, dream destinations.",
        "food-cooking" => "Food and cooking – Favorite dishes, Norwegian food, healthy eating.",
        "shopping" => "Shopping – Clothes, groceries, online shopping.",
        "housing" => "Housing – Describe your home, renting vs owning, moving.",
        "transportation" => "Transportation – How you commute, public transport vs car.",
        "weather-seasons" => "Weather and seasons – Favorite season, climate differences.",
        "daily-technology" => "Technology in daily life – Mobile phones, social media, internet use.",
        "job-career" => "Job and career – Your work, responsibilities, future plans.",
        "education" => "Education – School experiences, learning Norwegian, lifelong learning.",
        "work-life-balance" => "Work-life balance – Stress, free time, productivity.",
        "future-goals" => "Future goals – Career ambitions, personal development.",
        "health-fitness" => "Health and fitness – Exercise habits, healthy living.",
        "illness-healthcare" => "Illness and healthcare – Visiting a doctor, Norwegian health system.",
        "mental-health" => "Mental health – Stress management, relaxation techniques.",
        "norwegian-traditions" => "Norwegian traditions – Holidays, celebrations, national days.",
        "culture-comparison" => "Your culture vs Norwegian culture – Differences and similarities.",
        "music-art-lit" => "Music, art, and literature – Favorite artists, concerts.",
        "media-entertainment" => "Media and entertainment – TV shows, movies, books.",
        "nature-in-norway" => "Nature in Norway – Fjords, mountains, outdoor activities.",
        "environmental-issues" => "Environmental issues – Recycling, climate change.",
        "sustainable-living" => "Sustainable living – Eco-friendly habits.",
        "friendship-relationships" => "Friendship and relationships – Making friends in Norway.",
        "social-media" => "Social media – Pros and cons, privacy.",
        "volunteering-community" => "Volunteering and community – Helping others, charity work.",
        "technology-future" => "Technology and the future – AI, automation.",
        "education-system" => "Education system – Pros and cons.",
        "immigration-integration" => "Immigration and integration – Experiences, challenges.",
        "equality-diversity" => "Equality and diversity – Gender roles, inclusion.",
        "work-culture-norway" => "Work culture in Norway – Flat hierarchy, teamwork.",
        _ => topic
    };

    private sealed class OpenAiChatResponse
    {
        public List<OpenAiChoice>? Choices { get; set; }
    }

    private sealed class OpenAiChoice
    {
        public OpenAiMessage? Message { get; set; }
    }

    private sealed class OpenAiMessage
    {
        public string? Content { get; set; }
    }
}







