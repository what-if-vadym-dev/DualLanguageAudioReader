using ExamPrepar.Application;
using Xunit;

namespace ExamPrepar.UnitTests;

public class SanityTests
{
    [Fact]
    public void CanReferenceApplicationAssembly()
    {
        var c = new Class1();
        Assert.NotNull(c);
    }
}
