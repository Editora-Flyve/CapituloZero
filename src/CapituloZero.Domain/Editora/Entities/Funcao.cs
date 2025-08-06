using CapituloZero.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapituloZero.Domain.Editora.Entities;

public class Funcao : Entity
{
    public string Descricao { get; set; }
    public ICollection<Terceiro> Terceiros { get; set; }
}
