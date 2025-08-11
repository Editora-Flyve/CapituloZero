using CapituloZero.SharedKernel;

namespace CapituloZero.Domain.Editora.Entities;

public class EtapaTemplate : Entity
{
    public required string Nome { get; set; }
    public required Funcao Funcao { get; set; }
    public int Ordem { get; set; }
    public int PrazoDias { get; set; } // Deadline offset from start or previous completion in days
    public string ObservacaoPadrao { get; set; } = string.Empty;
}
