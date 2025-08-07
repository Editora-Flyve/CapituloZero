using CapituloZero.SharedKernel;

namespace CapituloZero.Domain.Editora.Entities;

public class Autor : Entity
{
    public string Nome { get; set; }
    public string Email { get; set; }
    public ICollection<Livro> Livros { get; set; }
}
