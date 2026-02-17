using ExamPrepar.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace ExamPrepar.Api.Endpoints;

public static class TranscriptEndpoints
{
    public static void MapTranscript(this WebApplication app)
    {
        app.MapGet("/api/v1/transcript", async (int? transcriptId, ExamDbContext db) =>
        {
            var id = transcriptId ?? await db.Transcripts.Select(t => t.Id).OrderBy(i => i).FirstOrDefaultAsync();
            var segments = await db.Segments.Where(s => s.TranscriptId == id).OrderBy(s => s.Start).ToListAsync();
            return Results.Ok(segments.Select(s => new { s.Start, s.End, s.Target, s.Native }));
        });

        app.MapGet("/api/v1/transcripts", async (string? target, string? native, string? level, ExamDbContext db) =>
        {
            var q = db.Transcripts.AsQueryable();
            if (!string.IsNullOrWhiteSpace(target)) q = q.Where(t => t.TargetLanguage == target);
            if (!string.IsNullOrWhiteSpace(native)) q = q.Where(t => t.NativeLanguage == native);
            if (!string.IsNullOrWhiteSpace(level)) q = q.Where(t => t.Level == level);
            var list = await q.Select(t => new { t.Id, t.Title, t.TargetLanguage, t.NativeLanguage, t.Level }).ToListAsync();
            return Results.Ok(list);
        });
    }
}
