using CapituloZero.SharedKernel;

namespace CapituloZero.SharedKernel.Tests;

public class EntityAndValueObjectTests
{
    private sealed class SampleEntity : Entity { public string Name { get; set; } = string.Empty; }

    private sealed class Money : ValueObject
    {
        public decimal Amount { get; }
        public string Currency { get; }
        public Money(decimal amount, string currency) { Amount = amount; Currency = currency; }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }
    }

    [Fact]
    public void Entity_Equality_By_Id()
    {
        var e1 = new SampleEntity();
        var e2 = new SampleEntity { Id = e1.Id };
        Assert.True(e1 == e2);
        Assert.Equal(e1, e2);
    }

    [Fact]
    public void ValueObject_Equality_By_Components()
    {
        var m1 = new Money(10, "USD");
        var m2 = new Money(10, "USD");
        var m3 = new Money(11, "USD");

        Assert.True(m1 == m2);
        Assert.False(m1 == m3);
        Assert.Equal(m1.GetHashCode(), m2.GetHashCode());
    }
}
