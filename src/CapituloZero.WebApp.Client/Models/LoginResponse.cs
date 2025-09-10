namespace CapituloZero.WebApp.Client.Models;

public sealed record LoginResponse(bool success, bool requiresTwoFactor, bool lockedOut, string? message);