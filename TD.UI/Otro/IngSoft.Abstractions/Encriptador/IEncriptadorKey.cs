using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngSoft.Abstractions
{
    public interface IEncriptadorKey : IEncriptador
    {
        string Desencriptar(string textoEncriptado);
    }
}
