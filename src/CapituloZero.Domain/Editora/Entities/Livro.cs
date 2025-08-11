using CapituloZero.SharedKernel;
using CapituloZero.Domain.Editora.Events;

namespace CapituloZero.Domain.Editora.Entities;

public class Livro : Entity
{
    public required string Titulo { get; set; }
    public string Subtitulo { get; set; } = string.Empty;
    private readonly List<Etapa> _etapas = [];
    public IReadOnlyCollection<Etapa> Etapas => _etapas;
    public required Autor Autor { get; set; }

    // Workflow instance state
    public Guid? FluxoProducaoId { get; set; }
    public int CurrentEtapaIndex { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataConclusao { get; set; }

    public void AddEtapa(Etapa etapa) => _etapas.Add(etapa);

    public void InitializeFromTemplate(FluxoProducao fluxo)
    {
        ArgumentNullException.ThrowIfNull(fluxo);
        FluxoProducaoId = fluxo.Id;
        DataInicio = DateTime.UtcNow;
        _etapas.Clear();
        foreach (var t in fluxo.Etapas.OrderBy(e => e.Ordem))
        {
            var etapa = new Etapa
            {
                LivroId = this.Id,
                Nome = t.Nome,
                Funcao = t.Funcao,
                Observacao = t.ObservacaoPadrao,
                DataLimite = DateTime.UtcNow.AddDays(t.PrazoDias),
                Status = Enums.EStatusEtapa.Pendente
            };
            _etapas.Add(etapa);
        }

        Raise(new LivroCriadoDomainEvent(this.Id));
    }

    public void Advance()
    {
        if (CurrentEtapaIndex < _etapas.Count - 1)
        {
            CurrentEtapaIndex++;
            Raise(new LivroAvancouDomainEvent(this.Id, CurrentEtapaIndex));
        }
        else if (_etapas.All(e => e.Status == Enums.EStatusEtapa.Concluido))
        {
            DataConclusao = DateTime.UtcNow;
            Raise(new LivroConcluidoDomainEvent(this.Id));
        }
    }
}
