using CapituloZero.SharedKernel;

namespace CapituloZero.Domain.Todos;

public static class TodoItemErrors
{
    public static ErrorInternal AlreadyCompleted(Guid todoItemId) => ErrorInternal.Problem(
        "TodoItems.AlreadyCompleted",
        $"The todo item with Id = '{todoItemId}' is already completed.");

    public static ErrorInternal NotFound(Guid todoItemId) => ErrorInternal.NotFound(
        "TodoItems.NotFound",
        $"The to-do item with the Id = '{todoItemId}' was not found");
}
