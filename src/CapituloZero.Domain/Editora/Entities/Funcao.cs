using CapituloZero.SharedKernel;

namespace CapituloZero.Domain.Editora.Entities;

public class Funcao : Entity
{
    public string Descricao { get; set; } = string.Empty;
    public ICollection<Terceiro> Terceiros { get; set; }
}
