using CapituloZero.Application.Abstractions.Authentication;
using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.SharedKernel;

namespace CapituloZero.Application.Users.AddTipos;

public sealed class AddTiposCommand(Guid userId, IEnumerable<string> tipos, Guid currentUserId) : ICommand
{
    public Guid UserId { get; } = userId;
    public IEnumerable<string> Tipos { get; } = tipos;
    public Guid CurrentUserId { get; } = currentUserId;
}

internal sealed class AddTiposCommandHandler(IIdentityService identityService) : ICommandHandler<AddTiposCommand>
{
    public async Task<Result> Handle(AddTiposCommand request, CancellationToken cancellationToken)
    {
        // Autorização feita no endpoint via política de permissões (apenas Admin)
        return await identityService.AddUserTypesAsync(request.UserId, request.Tipos, cancellationToken).ConfigureAwait(false);
    }
}
