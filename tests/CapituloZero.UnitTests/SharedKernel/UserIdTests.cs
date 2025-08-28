using System;
using CapituloZero.SharedKernel;
using Shouldly;
using Xunit;

namespace CapituloZero.UnitTests.SharedKernel;

public class UserIdTests
{
    [Fact]
    public void Cannot_create_empty_userid()
    {
        Should.Throw<ArgumentException>(() => new UserId(Guid.Empty));
    }

    [Fact]
    public void Equality_and_implicit_conversions_work()
    {
        var g = Guid.NewGuid();
        UserId a = new(g);
        UserId b = g; // implicit from Guid
        Guid g2 = a; // implicit to Guid

        (a == b).ShouldBeTrue();
        a.Equals(b).ShouldBeTrue();
        g2.ShouldBe(g);
        a.ToString().ShouldBe(g.ToString());
        a.GetHashCode().ShouldBe(b.GetHashCode());
    }
}

