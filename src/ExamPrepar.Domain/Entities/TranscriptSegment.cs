using System;

namespace ExamPrepar.Domain.Entities;

public class TranscriptSegment
{
    public int Id { get; set; }
    public double Start { get; set; }
    public double End { get; set; }
    public string Target { get; set; } = string.Empty;
    public string Native { get; set; } = string.Empty;
    public int TranscriptId { get; set; }
    public Transcript? Transcript { get; set; }
}
