using CapituloZero.SharedKernel;

namespace CapituloZero.Domain.Editora.Entities;

public class Funcao : Entity
{
    public string Descricao { get; set; }
    public ICollection<Terceiro> Terceiros { get; set; }
}
