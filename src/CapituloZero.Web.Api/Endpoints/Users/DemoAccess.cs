using CapituloZero.Web.Api.Extensions;

namespace CapituloZero.Web.Api.Endpoints.Users;

internal sealed class DemoAccess : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        // Rota default: qualquer usuário autenticado (todos têm users:access por padrão)
        app.MapGet("demo/default", () => Results.Ok(new { message = "acesso default" }))
            .HasPermission(Permissions.UsersAccess)
            .WithTags(Tags.Users);

        // Rota Autor: requer autor:access (admin também passa)
        app.MapGet("demo/autor", () => Results.Ok(new { message = "acesso autor" }))
            .HasPermission(Permissions.AutorAccess)
            .WithTags(Tags.Users);

        // Rota Parceiro: requer parceiro:access (admin também passa pelo handler)
        app.MapGet("demo/parceiro", () => Results.Ok(new { message = "acesso parceiro" }))
            .HasPermission(Permissions.ParceiroAccess)
            .WithTags(Tags.Users);

        // Rota admin: apenas admin (users:admin)
        app.MapGet("demo/admin", () => Results.Ok(new { message = "acesso admin" }))
            .HasPermission(Permissions.UsersAdmin)
            .WithTags(Tags.Users);
    }
}
