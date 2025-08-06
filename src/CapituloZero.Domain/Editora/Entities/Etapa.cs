using CapituloZero.Domain.Editora.Enums;
using CapituloZero.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapituloZero.Domain.Editora.Entities
{
    public class Etapa : Entity
    {
        public Guid ProximaEtapa { get; set; }
        public string Observacao { get; set; } = string.Empty;
        public DateTime DataLimite { get; set; } = DateTime.Now.AddDays(1);
        public EStatusEtapa Status { get; set; } = EStatusEtapa.Pendente;
    }
}
