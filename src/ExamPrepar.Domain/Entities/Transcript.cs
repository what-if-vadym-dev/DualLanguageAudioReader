using System;
using System.Collections.Generic;

namespace ExamPrepar.Domain.Entities;

public class Transcript
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string TargetLanguage { get; set; } = "";
    public string NativeLanguage { get; set; } = "";
    public string? Level { get; set; } // CEFR A1..C1
    public List<TranscriptSegment> Segments { get; set; } = new();
}
