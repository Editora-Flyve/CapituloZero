namespace CapituloZero.Web.Api.Endpoints.Todos;

internal sealed class CreateTodoRequest
{
    public Guid UserId { get; set; }
    public required string Description { get; set; }
    public DateTime? DueDate { get; set; }
    public IReadOnlyCollection<string> Labels { get; init; } = Array.Empty<string>();
    public int Priority { get; set; }
}
