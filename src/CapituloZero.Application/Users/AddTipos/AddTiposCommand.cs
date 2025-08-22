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
        // Authorization rule: only the same user can self-assign for now (admin permission to be added later)
        if (request.UserId != request.CurrentUserId)
        {
            return Result.Failure(Error.Problem("Usuarios.Unauthorized", "You cannot assign tipos for another user."));
        }

        return await identityService.AddUserTypesAsync(request.UserId, request.Tipos, cancellationToken).ConfigureAwait(false);
    }
}
