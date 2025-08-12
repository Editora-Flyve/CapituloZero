using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.Domain.Todos.Enums;

namespace CapituloZero.Application.Todos.Create;

public sealed class CreateTodoCommand : ICommand<Guid>
{
    public Guid UserId { get; set; }
    public required string Description { get; set; }
    public DateTime? DueDate { get; set; }
    public IReadOnlyCollection<string> Labels { get; set; } = [];
    public Priority Priority { get; set; }
}
