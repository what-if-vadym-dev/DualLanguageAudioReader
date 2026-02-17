using ExamPrepar.Application.Services;
using ExamPrepar.Infrastructure.Repositories;

namespace ExamPrepar.Api.Endpoints;

public static class ExamsEndpoints
{
    public static void MapExams(this WebApplication app)
    {
        app.MapGet("/exams", async (ExamService service, CancellationToken ct) =>
        {
            var items = await service.ListAsync(ct);
            return Results.Ok(items);
        });

        app.MapGet("/exams/{id}", async (Guid id, ExamService service, CancellationToken ct) =>
        {
            var item = await service.GetAsync(id, ct);
            return item is null ? Results.NotFound() : Results.Ok(item);
        });

        app.MapPost("/exams", async (ExamCreateRequest req, ExamService service, CancellationToken ct) =>
        {
            var created = await service.CreateAsync(req.Title, req.Questions, ct);
            return Results.Created($"/exams/{created.Id}", created);
        });
    }
}

public record ExamCreateRequest(string Title, List<string> Questions);
