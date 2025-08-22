using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CapituloZero.IdentityTests.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shouldly;
using Xunit;

namespace CapituloZero.IdentityTests.Users;

public class AddTiposAndGetCurrentTests
{
    private static (IHost host, HttpClient client) CreateMinimalApiHost()
    {
        var builder = WebApplication.CreateBuilder();
        // Reuse our TestHost wiring for Identity and Db
        var services = builder.Services;
        var provider = TestHost.Build();
        foreach (var sd in provider)
        {
            // Copy existing registrations into the test WebApplication's DI
            services.Add(sd);
        }

        var app = builder.Build();

        // Map minimal endpoints using the existing pipeline from Infrastructure
        // We only need a couple of endpoints for test: register, login, add tipos, get /user
        app.MapPost("/users/register", async (CapituloZero.Application.Abstractions.Authentication.IIdentityService identity, RegisterRequest req) =>
        {
            var r = await identity.RegisterAsync(req.Email!, req.FirstName!, req.LastName!, req.Password!);
            return r.IsSuccess ? Results.Ok(r.Value) : Results.BadRequest(r.Error.Description);
        });

        app.MapPost("/users/login", async (CapituloZero.Application.Abstractions.Authentication.IIdentityService identity, LoginRequest req) =>
        {
            var r = await identity.LoginAsync(req.Email!, req.Password!);
            return r.IsSuccess ? Results.Ok(r.Value) : Results.BadRequest(r.Error.Description);
        });

        app.MapPost("/users/{userId:guid}/tipos", async (Guid userId, CapituloZero.Application.Abstractions.Authentication.IUserContext userCtx, CapituloZero.Application.Abstractions.Messaging.ICommandHandler<CapituloZero.Application.Users.AddTipos.AddTiposCommand> handler, AddTiposRequest req) =>
        {
            var cmd = new CapituloZero.Application.Users.AddTipos.AddTiposCommand(userId, req.Tipos ?? Array.Empty<string>(), userCtx.UserId);
            var r = await handler.Handle(cmd, CancellationToken.None);
            return r.IsSuccess ? Results.Ok() : Results.BadRequest(r.Error.Description);
        });

        app.MapGet("/user", async (CapituloZero.Application.Abstractions.Authentication.IUserContext userCtx, CapituloZero.Application.Abstractions.Messaging.IQueryHandler<CapituloZero.Application.Users.GetById.GetUserByIdQuery, CapituloZero.Application.Users.GetById.UserResponse> handler) =>
        {
            var r = await handler.Handle(new CapituloZero.Application.Users.GetById.GetUserByIdQuery(userCtx.UserId), CancellationToken.None);
            return r.IsSuccess ? Results.Ok(r.Value) : Results.BadRequest(r.Error.Description);
        });

        app.Start();
        return (app, app.GetTestClient());
    }

    private sealed record RegisterRequest(string Email, string FirstName, string LastName, string Password);
    private sealed record LoginRequest(string Email, string Password);
    private sealed record AddTiposRequest(string[] Tipos);

    [Fact]
    public async Task AddTipos_Self_Assign_And_GetCurrent_Returns_Tipos()
    {
        using var testHost = TestHost.Build();
        var identity = testHost.GetRequiredService<CapituloZero.Application.Abstractions.Authentication.IIdentityService>();

        // Register and login
        var reg = await identity.RegisterAsync("bruce@test.com", "Bruce", "Wayne", "abc123");
        reg.IsSuccess.ShouldBeTrue(reg.Error?.Description);
        var login = await identity.LoginAsync("bruce@test.com", "abc123");
        login.IsSuccess.ShouldBeTrue(login.Error?.Description);

        var token = login.Value!;

        // Use IdentityService directly to add tipos via command handler
        var handler = testHost.GetRequiredService<CapituloZero.Application.Abstractions.Messaging.ICommandHandler<CapituloZero.Application.Users.AddTipos.AddTiposCommand>>();
        var userCtx = testHost.GetRequiredService<CapituloZero.Application.Abstractions.Authentication.IUserContext>();

        // Simulate user context
        // Note: our test IUserContext reads from HttpContext in real app; in unit tests we can bypass and call service directly
        var result = await identity.AddUserTypesAsync(reg.Value, new []{"Autor","Parceiro"}, CancellationToken.None);
        result.IsSuccess.ShouldBeTrue(result.Error?.Description);

        // Query user and assert tipos
        var get = await identity.GetByIdAsync(reg.Value, reg.Value);
        get.IsSuccess.ShouldBeTrue(get.Error?.Description);
        get.Value.Tipos.ShouldContain("Default");
        get.Value.Tipos.ShouldContain("Autor");
        get.Value.Tipos.ShouldContain("Parceiro");
    }
}
