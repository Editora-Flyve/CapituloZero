using System;
using CapituloZero.SharedKernel;
using Shouldly;
using Xunit;

namespace CapituloZero.UnitTests.SharedKernel;

public class ResultTests
{
    [Fact]
    public void Success_without_value_creates_success_result()
    {
        var r = Result.Success();
        r.IsSuccess.ShouldBeTrue();
        r.IsFailure.ShouldBeFalse();
        r.Error.ShouldBe(Error.None);
    }

    [Fact]
    public void Success_with_value_creates_success_result_and_value_is_accessible()
    {
        var r = Result.Success(42);
        r.IsSuccess.ShouldBeTrue();
        r.Error.ShouldBe(Error.None);
        r.ShouldBeOfType<Result<int>>();
        ((Result<int>)r).Value.ShouldBe(42);
    }

    [Fact]
    public void Failure_creates_failure_result_and_value_throws()
    {
        var r = Result.Failure<int>(Error.Failure("Test.Error", "failed"));
        r.IsFailure.ShouldBeTrue();
        r.Error.Code.ShouldBe("Test.Error");
        Should.Throw<InvalidOperationException>(() => r.Value);
    }

    [Fact]
    public void Implicit_from_value_maps_null_to_failure_and_non_null_to_success()
    {
        Result<string> r1 = "abc";
        r1.IsSuccess.ShouldBeTrue();
        r1.Value.ShouldBe("abc");

        string? s = null;
        Result<string> r2 = s;
        r2.IsFailure.ShouldBeTrue();
        r2.Error.ShouldBe(Error.NullValue);
    }

    [Fact]
    public void Invalid_constructor_combinations_throw()
    {
        Should.Throw<ArgumentException>(() => new Result(true, Error.Failure("x","y")));
        Should.Throw<ArgumentException>(() => new Result(false, Error.None));
    }
}

