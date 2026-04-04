using System.Collections.Generic;
using IngSoft.Domain;

namespace IngSoft.Repository
{
    public interface IUsuarioHistoricoRepository
    {
        void GuardarUsuarioHistorico(UsuarioHistorico usuarioHistorico);
        List<UsuarioHistorico> ObtenerUsuarioHistorico(string username);
    }
}
