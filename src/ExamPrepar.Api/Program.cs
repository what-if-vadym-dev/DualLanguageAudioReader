using Microsoft.EntityFrameworkCore;
using ExamPrepar.Infrastructure.Db;
using ExamPrepar.Application.Abstractions;
using ExamPrepar.Application.Services;
using ExamPrepar.Infrastructure.Repositories;
using ExamPrepar.Api.Endpoints;
using ExamPrepar.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var conn = builder.Configuration.GetConnectionString("Default") ?? "Data Source=exam.db";
builder.Services.AddDbContext<ExamDbContext>(opts => opts.UseSqlite(conn));
builder.Services.AddScoped<IExamRepository, EfExamRepository>();
builder.Services.AddScoped<ExamService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

// Apply migrations and seed a default transcript if empty
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ExamDbContext>();
    db.Database.Migrate();

    if (!db.Transcripts.Any())
    {
        var t = new Transcript
        {
            Title = "Helsevesen – Norsk/Engelsk",
            TargetLanguage = "nb-NO",
            NativeLanguage = "en-US",
            Level = "B1",
            Segments = new List<TranscriptSegment>
            {
                new() { Start = 0.0, End = 9.0, Target = "I Norge fungerer helsevesenet ganske godt, og det er relativt enkelt å få hjelp når man blir syk.", Native = "In Norway, the healthcare system works quite well, and it's relatively easy to get help when you get sick." },
                new() { Start = 9.0, End = 18.5, Target = "Alle som bor i Norge har en fastlege, altså en personlig lege som man kan kontakte når man trenger medisinsk hjelp.", Native = "Everyone living in Norway has a GP, a personal doctor you can contact when you need medical help." },
                new() { Start = 18.5, End = 27.0, Target = "Fastlegen kjenner pasientens situasjon, helsetilstand og behov.", Native = "The GP knows the patient's situation, health condition, and needs." },
                new() { Start = 27.0, End = 40.0, Target = "Hvis man ønsker, kan man bytte fastlege to ganger i året.", Native = "If you wish, you can change GP twice a year." },
            }
        };
        db.Transcripts.Add(t);
        db.SaveChanges();
    }
}

app.MapHealthChecks("/health");
app.MapExams();
app.MapTranscript();
app.MapPreferences();
app.MapGeneration();

app.Run();

public partial class Program {}

