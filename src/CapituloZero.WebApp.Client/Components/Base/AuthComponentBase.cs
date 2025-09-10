using Microsoft.AspNetCore.Components;

namespace CapituloZero.WebApp.Client.Components.Base;

public abstract class AuthComponentBase : ComponentBase
{
    protected bool IsSubmitting { get; private set; }
    protected string? ErrorMessage { get; set; }

    protected async Task ExecuteSubmitAsync(Func<Task> action)
    {
        if (IsSubmitting) return;
        ErrorMessage = null;
        IsSubmitting = true;
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsSubmitting = false;
            StateHasChanged();
        }
    }
}
