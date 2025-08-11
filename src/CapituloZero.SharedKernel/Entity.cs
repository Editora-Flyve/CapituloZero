using System.Diagnostics.CodeAnalysis;

namespace CapituloZero.SharedKernel;

public abstract class Entity : IEqualityComparer<Entity>, IEquatable<Entity>
{
    protected Entity() { }

    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    public Guid Id { get; set; } = Guid.CreateVersion7();

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public void Raise(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public override bool Equals(object? obj)
    {
        var compareTo = obj as Entity;

        if (ReferenceEquals(this, compareTo)) return true;
        if (ReferenceEquals(null, compareTo)) return false;

        return Id.Equals(compareTo.Id);
    }

    public static bool operator ==(Entity a, Entity b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(Entity a, Entity b)
    {
        return !(a == b);
    }

    public override int GetHashCode()
    {
        return (GetType().GetHashCode() ^ 93) + Id.GetHashCode();
    }

    public override string ToString()
    {
        return $"{GetType().Name} [Id={Id}]";
    }

    public bool Equals(Entity? x, Entity? y)
    {
        if (x is null && y is null)
            return true;
        if (x is null || y is null)
            return false;
        return x.Equals(y);
    }

    public int GetHashCode([DisallowNull] Entity obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return obj.GetHashCode();
    }

    public bool Equals(Entity? other)
    {
        if (other is null)
            return false;

        return this.Equals(other as object);
    }
}
