namespace CapituloZero.WebApp.Client.Models;

public sealed record LoginRequest(string Email, string Senha, bool Lembrar);