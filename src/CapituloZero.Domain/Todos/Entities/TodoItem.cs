using CapituloZero.Domain.Todos.Enums;
using CapituloZero.SharedKernel;

namespace CapituloZero.Domain.Todos.Entities;

public sealed class TodoItem : Entity
{
    public Guid UserId { get; set; }
    public required string Description { get; set; }
    public DateTime? DueDate { get; set; }
        private readonly List<string> _labels = new();
        public IReadOnlyCollection<string> Labels => _labels.AsReadOnly();

        public void AddLabel(string label)
        {
            _labels.Add(label);
        }
        public void AddLabels(IEnumerable<string> labels)
        {
            _labels.AddRange(labels);
        }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public Priority Priority { get; set; }
}
