using CapituloZero.SharedKernel;

namespace CapituloZero.Domain.Editora.Entities;

public class Autor : Entity
{
    public required string Nome { get; set; }
    public required string Email { get; set; }
        private readonly List<Livro> _livros = new();
        public IReadOnlyCollection<Livro> Livros => _livros.AsReadOnly();

        public void AddLivro(Livro livro)
        {
            _livros.Add(livro);
        }
}
