using IngSoft.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngSoft.Repository
{
    public interface IUsuarioRepository
    {
        void GuardarUsuario(Usuario usuario);
        List<Usuario> ObtenerUsuarios();
        List<Usuario> ObtenerUsuariosFiltrados(string filtro);
        void AumentarIntentosFallidos(Usuario usuario);
        void ResetearIntentosFallidos(Usuario usuario);
        Usuario ObtenerUsuario(string username);
    }
}
