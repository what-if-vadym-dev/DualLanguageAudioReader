using ExamPrepar.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace ExamPrepar.Api.Endpoints;

public static class GenerationEndpoints
{
    public record GenerateRequest(string targetLanguage, string nativeLanguage, string level, string topic);

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
    }

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
}
