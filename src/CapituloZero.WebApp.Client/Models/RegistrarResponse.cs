namespace CapituloZero.WebApp.Client.Models;

public sealed record RegistrarResponse(bool success, bool requiresConfirmation, string[]? errors);