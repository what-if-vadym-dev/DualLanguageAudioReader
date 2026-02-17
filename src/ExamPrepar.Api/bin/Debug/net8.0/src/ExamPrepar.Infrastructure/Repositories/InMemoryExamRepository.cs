using ExamPrepar.Application.Abstractions;
using ExamPrepar.Domain.Entities;

namespace ExamPrepar.Infrastructure.Repositories;

public class InMemoryExamRepository : IExamRepository
{
    private readonly Dictionary<Guid, Exam> _store = new();

    public Task<Exam?> GetAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(_store.TryGetValue(id, out var exam) ? exam : null);

    public Task<IReadOnlyList<Exam>> ListAsync(CancellationToken ct = default)
        => Task.FromResult((IReadOnlyList<Exam>)_store.Values.ToList());

    public Task<Exam> AddAsync(Exam exam, CancellationToken ct = default)
    {
        _store[exam.Id] = exam;
        return Task.FromResult(exam);
    }
}
