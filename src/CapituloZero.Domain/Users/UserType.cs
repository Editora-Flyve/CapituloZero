namespace CapituloZero.Domain.Users;

[System.Flags]
public enum UserType
{
    None = 0,
    Cliente = 1 << 0,
    Admin = 1 << 1,
    Terceiro = 1 << 2,
    Autor = 1 << 3
}
