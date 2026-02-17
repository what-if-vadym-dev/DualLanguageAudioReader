using ExamPrepar.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace ExamPrepar.Api.Endpoints;

public static class PreferencesEndpoints
{
    public record PreferenceCreateRequest(string targetLanguage, string nativeLanguage, string level);

    public static void MapPreferences(this WebApplication app)
    {
        app.MapPost("/api/v1/preferences", async (PreferenceCreateRequest req, ExamDbContext db) =>
        {
            var pref = new ExamPrepar.Domain.Entities.ReadingPreference
            {
                Id = Guid.NewGuid(),
                TargetLanguage = req.targetLanguage,
                NativeLanguage = req.nativeLanguage,
                Level = req.level,
                CreatedAt = DateTime.UtcNow
            };
            db.Preferences.Add(pref);
            await db.SaveChangesAsync();
            return Results.Created($"/api/v1/preferences/{pref.Id}", new { id = pref.Id });
        });

        app.MapGet("/api/v1/preferences/{id}", async (Guid id, ExamDbContext db) =>
        {
            var pref = await db.Preferences.FindAsync(id);
            return pref is null ? Results.NotFound() : Results.Ok(new
            {
                id = pref.Id,
                targetLanguage = pref.TargetLanguage,
                nativeLanguage = pref.NativeLanguage,
                level = pref.Level,
                createdAt = pref.CreatedAt
            });
        });
    }
}
