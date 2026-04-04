using System.Collections.Generic;
using IngSoft.Domain;
using IngSoft.Domain.Enums;
using IngSoft.Repository;
using IngSoft.Repository.Factory;
using IngSoft.Services;

namespace IngSoft.ApplicationServices.Implementation
{
    public class UsuarioHistoricoServices : IUsuarioHistoricoServices
    {
        private readonly IUsuarioHistoricoRepository _usuarioHistoricoRepository;
        private readonly IBitacoraRepository _bitacoraRepository;

        internal UsuarioHistoricoServices(IUsuarioHistoricoRepository usuarioHistoricoRepository, IBitacoraRepository bitacoraRepository)
        {
            _usuarioHistoricoRepository = usuarioHistoricoRepository ?? FactoryRepository.CreateUsuarioHistoricoRepository();
            _bitacoraRepository = bitacoraRepository ?? FactoryRepository.CreateBitacoraRepository();
        }

        public void GuardarUsuarioHistorico(UsuarioHistorico usuarioHistorico)
        {
            _usuarioHistoricoRepository.GuardarUsuarioHistorico(usuarioHistorico);
            var usuarioId = SessionManager.GetUsuario() == null ? usuarioHistorico.IdUsuario : SessionManager.GetUsuario().IdUsuario;

            _bitacoraRepository.GuardarBitacora(new Bitacora
            {
                Id = System.Guid.NewGuid(),
                Usuario = new Usuario { IdUsuario = usuarioId },
                Fecha = System.DateTime.Now,
                Descripcion = $"Se guardó el historial del usuario {usuarioHistorico.UserName}",
                Origen = "GuardarUsuarioHistorico",
                TipoEvento = TipoEvento.Message
            });
        }

        public List<UsuarioHistorico> ObtenerUsuarioHistorico(string username)
        {
            var resultado = _usuarioHistoricoRepository.ObtenerUsuarioHistorico(username);
            return resultado;
        }
    }
}
