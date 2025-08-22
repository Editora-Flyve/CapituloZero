using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.Domain.Todos;
using CapituloZero.SharedKernel;

namespace CapituloZero.Application.Todos.Create;

public sealed class CreateTodoCommand : ICommand<Guid>
{
    public UserId UserId { get; set; }
    public string Description { get; set; }
    public DateTime? DueDate { get; set; }
    public List<string> Labels { get; set; } = [];
    public Priority Priority { get; set; }
}
