using CapituloZero.SharedKernel;

namespace CapituloZero.Domain.Editora.Entities;

public class Livro : Entity
{
    public required string Titulo { get; set; }
    public string Subtitulo { get; set; } = string.Empty;
    public ICollection<Etapa> Etapas { get; set; }
    public Autor Autor { get; set; }
}
