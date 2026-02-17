using System;

namespace ExamPrepar.Domain.Entities;

public class ReadingPreference
{
    public Guid Id { get; set; }
    public string TargetLanguage { get; set; } = string.Empty;
    public string NativeLanguage { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty; // CEFR: A1..C1
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
