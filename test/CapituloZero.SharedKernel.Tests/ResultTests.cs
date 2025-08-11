using CapituloZero.SharedKernel;

namespace CapituloZero.SharedKernel.Tests;

public class ResultTests
{
    [Fact]
    public void Success_Result_Has_IsSuccess_True_And_No_Error()
    {
        var r = Result.Success();
        Assert.True(r.IsSuccess);
        Assert.False(r.IsFailure);
        Assert.Equal(Error.None, r.Error);
    }

    [Fact]
    public void Failure_Result_Has_IsSuccess_False_And_Error()
    {
        var err = Error.Failure("Test", "message");
        var r = Result.Failure(err);
        Assert.False(r.IsSuccess);
        Assert.True(r.IsFailure);
        Assert.Equal(err, r.Error);
    }

    [Fact]
    public void Generic_Success_Carries_Value()
    {
        Result<int> r = Result.Success(42);
        Assert.True(r.IsSuccess);
        Assert.Equal(42, r.Value);
    }

    [Fact]
    public void Generic_Failure_Throws_On_Value_Access()
    {
        Result<int> r = Result.Failure<int>(Error.Failure("X","Y"));
        Assert.True(r.IsFailure);
        Assert.Throws<InvalidOperationException>(() => _ = r.Value);
    }

    [Fact]
    public void Implicit_Conversion_From_NonNull_Is_Success()
    {
        Result<string> r = "ok";
        Assert.True(r.IsSuccess);
        Assert.Equal("ok", r.Value);
    }

    [Fact]
    public void Implicit_Conversion_From_Null_Is_Failure_NullValue()
    {
        string? s = null;
        Result<string> r = s;
        Assert.True(r.IsFailure);
        Assert.Equal(Error.NullValue, r.Error);
    }
}
