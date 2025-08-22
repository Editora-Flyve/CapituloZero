namespace CapituloZero.Infrastructure.Usuarios;

public static class UserTypes
{
    public const string Default = "Default";
    public const string Autor = "Autor";
    public const string Parceiro = "Parceiro";
    public const string Admin = "Admin";

    public static readonly IReadOnlySet<string> Allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        Default, Autor, Parceiro, Admin
    };
}

public static class UserTypeIds
{
    public static readonly Guid Default = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid Autor = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid Parceiro = Guid.Parse("33333333-3333-3333-3333-333333333333");
    public static readonly Guid Admin = Guid.Parse("44444444-4444-4444-4444-444444444444");
}
