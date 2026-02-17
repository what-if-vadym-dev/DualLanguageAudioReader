using ExamPrepar.Application.Abstractions;
using ExamPrepar.Domain.Entities;

namespace ExamPrepar.Application.Services;

public class ExamService
{
    private readonly IExamRepository _repo;

    public ExamService(IExamRepository repo) => _repo = repo;

    public Task<IReadOnlyList<Exam>> ListAsync(CancellationToken ct = default) => _repo.ListAsync(ct);

    public async Task<Exam> CreateAsync(string title, IEnumerable<string> questions, CancellationToken ct = default)
    {
        var exam = new Exam { Title = title, Questions = questions.ToList() };
        return await _repo.AddAsync(exam, ct);
    }

    public Task<Exam?> GetAsync(Guid id, CancellationToken ct = default) => _repo.GetAsync(id, ct);
}
