using CapituloZero.SharedKernel;

namespace CapituloZero.Domain.Editora.Entities;

public class Terceiro : Entity
{
    public required string Nome { get; set; }
    public required string Documento { get; set; }
    public required Funcao Funcao { get; set; }
    public required string Email { get; set; }
    // Optional link to an authenticated User to drive kanban filtering/permissions
    public Guid? UserId { get; set; }
}
