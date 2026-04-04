using System.Collections.Generic;
using IngSoft.Domain;

namespace IngSoft.ApplicationServices
{
    public interface IUsuarioHistoricoServices
    {
        void GuardarUsuarioHistorico(UsuarioHistorico usuarioHistorico);
        List<UsuarioHistorico> ObtenerUsuarioHistorico(string username);
    }
}
