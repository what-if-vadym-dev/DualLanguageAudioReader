using ExamPrepar.Domain.Entities;
using Xunit;

namespace ExamPrepar.UnitTests;

public class DomainSanityTests
{
    [Fact]
    public void CanUseDomainType()
    {
        var e = new Exam { Title = "T" };
        Assert.Equal("T", e.Title);
    }
}
