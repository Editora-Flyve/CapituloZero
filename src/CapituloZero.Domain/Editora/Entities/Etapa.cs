using CapituloZero.Domain.Editora.Enums;
using CapituloZero.SharedKernel;

namespace CapituloZero.Domain.Editora.Entities;

public class Etapa : Entity
{
    public string Observacao { get; set; } = string.Empty;
    public DateTime DataLimite { get; set; } = DateTime.Now.AddDays(1);
    public EStatusEtapa Status { get; set; } = EStatusEtapa.Pendente;
    public required Terceiro Responsavel { get; set; }
}
