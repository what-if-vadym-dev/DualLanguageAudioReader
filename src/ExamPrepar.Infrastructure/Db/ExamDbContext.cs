using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ExamPrepar.Domain.Entities;

namespace ExamPrepar.Infrastructure.Db;

public class ExamDbContext : DbContext
{
    public ExamDbContext(DbContextOptions<ExamDbContext> options) : base(options) { }

    public DbSet<Exam> Exams => Set<Exam>();
    public DbSet<Transcript> Transcripts => Set<Transcript>();
    public DbSet<TranscriptSegment> Segments => Set<TranscriptSegment>();
    public DbSet<ReadingPreference> Preferences => Set<ReadingPreference>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var questionsConverter = new ValueConverter<List<string>, string>(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
        );

        var questionsComparer = new ValueComparer<List<string>>(
            (a, b) => (a ?? new List<string>()).SequenceEqual(b ?? new List<string>()),
            v => (v ?? new List<string>()).Aggregate(0, (hash, item) => HashCode.Combine(hash, (item == null ? 0 : item.GetHashCode()))),
            v => v == null ? new List<string>() : v.ToList()
        );

        modelBuilder.Entity<Exam>(b =>
        {
            b.HasKey(e => e.Id);
            b.Property(e => e.Title).IsRequired().HasMaxLength(200);
            var prop = b.Property(e => e.Questions);
            prop.HasConversion(questionsConverter);
            prop.Metadata.SetValueComparer(questionsComparer);
        });

        modelBuilder.Entity<Transcript>(b =>
        {
            b.HasKey(t => t.Id);
            b.Property(t => t.Title).IsRequired().HasMaxLength(200);
            b.Property(t => t.TargetLanguage).HasMaxLength(16);
            b.Property(t => t.NativeLanguage).HasMaxLength(16);
            b.Property(t => t.Level).HasMaxLength(3);
            b.HasMany(t => t.Segments)
             .WithOne(s => s.Transcript)
             .HasForeignKey(s => s.TranscriptId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TranscriptSegment>(b =>
        {
            b.HasKey(s => s.Id);
            b.Property(s => s.Start).IsRequired();
            b.Property(s => s.End).IsRequired();
            b.Property(s => s.Target).IsRequired();
            b.Property(s => s.Native).IsRequired();
        });

        modelBuilder.Entity<ReadingPreference>(b =>
        {
            b.HasKey(p => p.Id);
            b.Property(p => p.TargetLanguage).IsRequired().HasMaxLength(16);
            b.Property(p => p.NativeLanguage).IsRequired().HasMaxLength(16);
            b.Property(p => p.Level).IsRequired().HasMaxLength(3);
            b.Property(p => p.CreatedAt).IsRequired();
        });
    }
}
