using CapituloZero.SharedKernel;
using Shouldly;
using Xunit;

namespace CapituloZero.UnitTests.SharedKernel;

public class ValidationErrorInternalTests
{
    [Fact]
    public void FromResultsAggregatesFailureErrors()
    {
        var r1 = Result.Success();
        var r2 = Result.Failure(ErrorInternal.Problem("E.1", "bad1"));
        var r3 = Result.Failure(ErrorInternal.NotFound("E.2", "bad2"));

        var ve = ValidationErrorInternal.FromResults(new[] { r1, r2, r3 });

        ve.Type.ShouldBe(ErrorType.Validation);
        ve.Code.ShouldBe("Validation.General");
        ve.Errors.Select(e => e.Code).ShouldBe(new[] { "E.1", "E.2" });
    }
}

