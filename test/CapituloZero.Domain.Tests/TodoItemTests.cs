using CapituloZero.Domain.Todos.Entities;
using CapituloZero.Domain.Todos.Enums;

namespace CapituloZero.Domain.Tests;

public class TodoItemTests
{
    [Fact]
    public void AddLabel_Adds_Single_Label()
    {
        var todo = new TodoItem
        {
            UserId = Guid.NewGuid(),
            Description = "desc",
            Priority = Priority.Low,
            CreatedAt = DateTime.UtcNow
        };

        todo.AddLabel("work");

        Assert.Contains("work", todo.Labels);
        Assert.Single(todo.Labels);
    }

    [Fact]
    public void AddLabels_Adds_Multiple_Labels()
    {
        var todo = new TodoItem
        {
            UserId = Guid.NewGuid(),
            Description = "desc",
            Priority = Priority.Low,
            CreatedAt = DateTime.UtcNow
        };

        todo.AddLabels(new []{"work","home"});

        Assert.Equal(2, todo.Labels.Count);
        Assert.Contains("home", todo.Labels);
    }
}
