using System.Collections.Generic;
using IngSoft.Domain;

namespace IngSoft.Repository
{
    public interface IUsuarioRepository
    {
        void ModificarUsuarioPrueba(Usuario usuario);
        void GuardarUsuario(Usuario usuario);
        List<Usuario> ObtenerUsuarios();
        List<Usuario> ObtenerUsuariosFiltrados(string filtro);
        Usuario AumentarIntentosFallidos(Usuario usuario);
        void ResetearIntentosFallidos(Usuario usuario);
        Usuario ObtenerUsuario(string username);
        Usuario CrearUsuario(Usuario usuario);
        Usuario ModificarUsuario(Usuario usuario);
        Usuario EliminarUsuario(Usuario usuario);
    }
}
