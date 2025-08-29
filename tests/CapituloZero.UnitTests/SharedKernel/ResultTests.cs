using CapituloZero.SharedKernel;
using Shouldly;
using Xunit;

namespace CapituloZero.UnitTests.SharedKernel;

public class ResultTests
{
    [Fact]
    public void SuccessWithoutValueCreatesSuccessResult()
    {
        var r = Result.Success();
        r.IsSuccess.ShouldBeTrue();
        r.IsFailure.ShouldBeFalse();
        r.ErrorInternal.ShouldBe(ErrorInternal.None);
    }

    [Fact]
    public void SuccessWithValueCreatesSuccessResultAndValueIsAccessible()
    {
        var r = Result.Success(42);
        r.IsSuccess.ShouldBeTrue();
        r.ErrorInternal.ShouldBe(ErrorInternal.None);
        r.ShouldBeOfType<Result<int>>();
        ((Result<int>)r).Value.ShouldBe(42);
    }

    [Fact]
    public void FailureCreatesFailureResultAndValueThrows()
    {
        var r = Result.Failure<int>(ErrorInternal.Failure("Test.ErrorInternal", "failed"));
        r.IsFailure.ShouldBeTrue();
        r.ErrorInternal.Code.ShouldBe("Test.ErrorInternal");
        Should.Throw<InvalidOperationException>(() => r.Value);
    }

    [Fact]
    public void ImplicitFromValueMapsNullToFailureAndNonNullToSuccess()
    {
        Result<string> r1 = "abc";
        r1.IsSuccess.ShouldBeTrue();
        r1.Value.ShouldBe("abc");

        string? s = null;
        Result<string> r2 = s;
        r2.IsFailure.ShouldBeTrue();
        r2.ErrorInternal.ShouldBe(ErrorInternal.NullValue);
    }

    [Fact]
    public void InvalidConstructorCombinationsThrow()
    {
        Should.Throw<ArgumentException>(() => new Result(true, ErrorInternal.Failure("x","y")));
        Should.Throw<ArgumentException>(() => new Result(false, ErrorInternal.None));
    }
}

