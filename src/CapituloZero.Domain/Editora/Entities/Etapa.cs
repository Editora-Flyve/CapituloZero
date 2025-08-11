using CapituloZero.Domain.Editora.Enums;
using CapituloZero.SharedKernel;

namespace CapituloZero.Domain.Editora.Entities;

public class Etapa : Entity
{
    public required Guid LivroId { get; set; }
    public required string Nome { get; set; }
    public string Observacao { get; set; } = string.Empty;
    public DateTime DataLimite { get; set; }
    public EStatusEtapa Status { get; set; } = EStatusEtapa.Pendente;
    public Guid? ResponsavelId { get; set; }
    public required Funcao Funcao { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    private readonly List<Artefato> _artefatos = [];
    public IReadOnlyCollection<Artefato> Artefatos => _artefatos;

    public void AssignResponsavel(Guid terceiroId)
    {
        ResponsavelId = terceiroId;
    }

    public void Start()
    {
        if (Status == EStatusEtapa.Pendente)
        {
            Status = EStatusEtapa.EmAndamento;
            StartedAt = DateTime.UtcNow;
        }
    }

    public void SubmitArtefato(Artefato artefato)
    {
        _artefatos.Add(artefato);
    }

    public void Complete()
    {
        if (Status == EStatusEtapa.EmAndamento || Status == EStatusEtapa.Pendente)
        {
            Status = EStatusEtapa.Concluido;
            CompletedAt = DateTime.UtcNow;
        }
    }
}
