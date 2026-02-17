using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExamPrepar.Application.Abstractions;
using ExamPrepar.Application.Services;
using ExamPrepar.Domain.Entities;
using Xunit;

namespace ExamPrepar.UnitTests;

public class ExamServiceTests
{
    private class FakeRepo : IExamRepository
    {
        private readonly Dictionary<Guid, Exam> _store = new();
        public Task<Exam?> GetAsync(Guid id, CancellationToken ct = default) => Task.FromResult(_store.TryGetValue(id, out var e) ? e : null);
        public Task<IReadOnlyList<Exam>> ListAsync(CancellationToken ct = default) => Task.FromResult((IReadOnlyList<Exam>)_store.Values.ToList());
        public Task<Exam> AddAsync(Exam exam, CancellationToken ct = default) { _store[exam.Id] = exam; return Task.FromResult(exam); }
    }

    [Fact]
    public async Task Create_And_Get_Exam()
    {
        var repo = new FakeRepo();
        var service = new ExamService(repo);

        var created = await service.CreateAsync("Math", new[] { "Q1", "Q2" });
        Assert.Equal("Math", created.Title);
        Assert.Equal(2, created.Questions.Count);

        var fetched = await service.GetAsync(created.Id);
        Assert.NotNull(fetched);
        Assert.Equal(created.Id, fetched!.Id);
    }
}
