using ExamPrepar.Domain.Entities;

namespace ExamPrepar.Application.Abstractions;

public interface IExamRepository
{
    Task<Exam?> GetAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Exam>> ListAsync(CancellationToken ct = default);
    Task<Exam> AddAsync(Exam exam, CancellationToken ct = default);
}
