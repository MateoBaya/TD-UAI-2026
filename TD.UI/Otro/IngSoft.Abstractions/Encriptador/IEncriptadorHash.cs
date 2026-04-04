using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace IngSoft.Abstractions
{
    public interface IEncriptadorHash : IEncriptador
    {
        bool VerificarHash(string textoPlano, string hashGenerado);
    }
}
