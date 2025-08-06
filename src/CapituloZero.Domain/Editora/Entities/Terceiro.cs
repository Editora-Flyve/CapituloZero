using CapituloZero.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapituloZero.Domain.Editora.Entities;

public class Terceiro : Entity
{
    public string Nome { get; set; }
    public string Documento { get; set; }
    public Funcao Funcao { get; set; }
    public string Email { get; set; }
}
