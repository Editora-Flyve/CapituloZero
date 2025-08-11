namespace CapituloZero.Application.Todos.GetById;

public sealed class TodoResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
        public required string Description { get; set; }
        public DateTime? DueDate { get; set; }
        public required IReadOnlyCollection<string> Labels { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
