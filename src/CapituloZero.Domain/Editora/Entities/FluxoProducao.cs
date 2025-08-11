using CapituloZero.SharedKernel;

namespace CapituloZero.Domain.Editora.Entities;

public class FluxoProducao : Entity
{
    public required string Nome { get; set; }
    public string Descricao { get; set; } = string.Empty;
    private readonly List<EtapaTemplate> _etapas = [];
    public IReadOnlyCollection<EtapaTemplate> Etapas => _etapas;

    public void AddEtapa(EtapaTemplate etapa)
    {
        _etapas.Add(etapa);
    }
}
