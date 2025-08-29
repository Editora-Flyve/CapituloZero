using CapituloZero.SharedKernel;
using Shouldly;
using Xunit;

namespace CapituloZero.UnitTests.SharedKernel;

file sealed class TestEntity : Entity { public string Name { get; set; } = string.Empty; }

public class EntityTests
{
    [Fact]
    public void EqualityBasedOnId()
    {
        var id = Guid.NewGuid();
        var a = new TestEntity { Id = id, Name = "A" };
        var b = new TestEntity { Id = id, Name = "B" };
        var c = new TestEntity { Id = Guid.NewGuid(), Name = "C" };

        (a == b).ShouldBeTrue();
        (a != c).ShouldBeTrue();
        a.Equals(b).ShouldBeTrue();
        a.GetHashCode().ShouldBe(b.GetHashCode());
        a.ToString().ShouldContain(id.ToString());
    }

    [Fact]
    public void RaiseAndClearDomainEvents()
    {
        var e = new TestEntity { Id = Guid.NewGuid(), Name = "X" };
        e.DomainEvents.Count.ShouldBe(0);

        var de1 = new DummyDomainEvent(Guid.NewGuid());
        var de2 = new DummyDomainEvent(Guid.NewGuid());
        e.Raise(de1);
        e.Raise(de2);

        e.DomainEvents.Count.ShouldBe(2);
        e.DomainEvents.First().ShouldBe(de1);

        e.ClearDomainEvents();
        e.DomainEvents.Count.ShouldBe(0);
    }
}

file sealed record DummyDomainEvent(Guid Id) : IDomainEvent;

