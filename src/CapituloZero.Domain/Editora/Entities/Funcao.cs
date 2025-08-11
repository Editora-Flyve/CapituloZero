using CapituloZero.SharedKernel;

namespace CapituloZero.Domain.Editora.Entities;

public class Funcao : Entity
{
    public string Descricao { get; set; } = string.Empty;
        public IReadOnlyCollection<Terceiro> Terceiros { get; private set; } = new List<Terceiro>();
}
