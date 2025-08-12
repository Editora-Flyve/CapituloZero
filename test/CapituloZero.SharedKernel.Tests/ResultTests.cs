using CapituloZero.SharedKernel;

namespace CapituloZero.SharedKernel.Tests;

public class ResultTests
{
    [Fact]
    public void SuccessResultHasIsSuccessTrueAndNoError()
    {
        var r = Result.Success();
        Assert.True(r.IsSuccess);
        Assert.False(r.IsFailure);
        Assert.Equal(Error.None, r.Error);
    }

    [Fact]
    public void FailureResultHasIsSuccessFalseAndError()
    {
        var err = Error.Failure("Test", "message");
        var r = Result.Failure(err);
        Assert.False(r.IsSuccess);
        Assert.True(r.IsFailure);
        Assert.Equal(err, r.Error);
    }

    [Fact]
    public void GenericSuccessCarriesValue()
    {
        Result<int> r = Result.Success(42);
        Assert.True(r.IsSuccess);
        Assert.Equal(42, r.Value);
    }

    [Fact]
    public void GenericFailureThrowsOnValueAccess()
    {
        Result<int> r = Result.Failure<int>(Error.Failure("X","Y"));
        Assert.True(r.IsFailure);
        Assert.Throws<InvalidOperationException>(() => _ = r.Value);
    }

    [Fact]
    public void ImplicitConversionFromNonNullIsSuccess()
    {
        Result<string> r = "ok";
        Assert.True(r.IsSuccess);
        Assert.Equal("ok", r.Value);
    }

    [Fact]
    public void ImplicitConversionFromNullIsFailureNullValue()
    {
        string? s = null;
        Result<string> r = s;
        Assert.True(r.IsFailure);
        Assert.Equal(Error.NullValue, r.Error);
    }
}
