using Microsoft.EntityFrameworkCore;
using ExamPrepar.Application.Abstractions;
using ExamPrepar.Domain.Entities;
using ExamPrepar.Infrastructure.Db;

namespace ExamPrepar.Infrastructure.Repositories;

public class EfExamRepository : IExamRepository
{
    private readonly ExamDbContext _db;
    public EfExamRepository(ExamDbContext db) => _db = db;

    public async Task<Exam?> GetAsync(Guid id, CancellationToken ct = default)
        => await _db.Exams.FindAsync([id], ct);

    public async Task<IReadOnlyList<Exam>> ListAsync(CancellationToken ct = default)
        => await _db.Exams.AsNoTracking().ToListAsync(ct);

    public async Task<Exam> AddAsync(Exam exam, CancellationToken ct = default)
    {
        _db.Exams.Add(exam);
        await _db.SaveChangesAsync(ct);
        return exam;
    }
}
