using System.Linq;
using CapituloZero.SharedKernel;
using Shouldly;
using Xunit;

namespace CapituloZero.UnitTests.SharedKernel;

public class ValidationErrorTests
{
    [Fact]
    public void FromResults_aggregates_failure_errors()
    {
        var r1 = Result.Success();
        var r2 = Result.Failure(Error.Problem("E.1", "bad1"));
        var r3 = Result.Failure(Error.NotFound("E.2", "bad2"));

        var ve = ValidationError.FromResults(new[] { r1, r2, r3 });

        ve.Type.ShouldBe(ErrorType.Validation);
        ve.Code.ShouldBe("Validation.General");
        ve.Errors.Select(e => e.Code).ShouldBe(new[] { "E.1", "E.2" });
    }
}

