using System;
using System.Collections.Generic;
using IngSoft.Domain;
using IngSoft.Services;

namespace IngSoft.ApplicationServices
{
    public interface IUsuarioServices
    {
        void ModificarUsuario(Usuario usuario);
        void GuardarUsuario(Usuario usuario);
        void SetRegistradoBitacora(Action<Usuario, string, string, Domain.Enums.TipoEvento> registrarEnBitacora);
        List<Usuario> ObtenerUsuarios();
        List<Usuario> ObtenerUsuarioFiltrados(string filtro);
        Usuario ObtenerUsuario(string username);
        SessionManager LoginUser(Usuario usuario);
        void LogOutUser();
        void CrearUsuario(Usuario usuario);
    }
}