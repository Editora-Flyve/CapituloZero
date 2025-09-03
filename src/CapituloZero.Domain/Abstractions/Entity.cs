namespace CapituloZero.Domain.Abstractions
{
    public abstract class Entity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; private  set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get;private set; }
        public bool IsDeleted { get; private set; } = false;
    
        protected Entity()
        {
            Id = Guid.CreateVersion7();
        }
    
        public void MarkAsUpdated()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    
        public void MarkAsDeleted()
        {
            IsDeleted = true;
            MarkAsUpdated();
        }

        public override bool Equals(object obj)
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
    }
}