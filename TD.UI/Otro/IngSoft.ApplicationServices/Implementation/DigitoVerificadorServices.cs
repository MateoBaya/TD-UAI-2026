using System;
using System.Linq;
using IngSoft.ApplicationServices.Dto;
using IngSoft.Domain;
using IngSoft.Domain.Enums;
using IngSoft.Repository;
using IngSoft.Repository.Factory;
using IngSoft.Services;

namespace IngSoft.ApplicationServices.Implementation
{
    public class DigitoVerificadorServices : IDigitoVerificadorServices
    {
        private readonly IDigitoVerificadorRepository _digitoVerificadorRepository;
        private readonly IBitacoraRepository _bitacoraRepository;
        
        internal DigitoVerificadorServices(IDigitoVerificadorRepository digitoVerificadorRepository, IBitacoraRepository bitacoraRepository)
        {
            _digitoVerificadorRepository = digitoVerificadorRepository ?? FactoryRepository.CreateDigitoVerificadorRepository();
            _bitacoraRepository = bitacoraRepository ?? FactoryRepository.CreateBitacoraRepository();
        }

        public void RecaulcularDigitosVerificadores()
        {
            try
            {
                _digitoVerificadorRepository.RecalcularDigitosVerificadores();
                GuardarBitacora("Dígitos verificadores recalculados correctamente", TipoEvento.Message);
            }
            catch (Exception ex)
            {
                GuardarBitacora("Error al recalcular dígitos verificadores", TipoEvento.Error);
                throw;
            }
        }

        public ResultadoIntegridad ValidarIntegridad()
        {
            var resultado =  _digitoVerificadorRepository.ValidarIntegridad();
            var descripcion = resultado.EsValida ? "Integridad de la DB validada correctamente" : $"Errores de integridad encontrados: {resultado.Errores.Count}";
            var tipoEvento = resultado.EsValida ? TipoEvento.Message : TipoEvento.Warning;
            
            var resultadoFinal = new ResultadoIntegridad
            {
                EsValida = resultado.EsValida,
                Mensaje = resultado.Mensaje,
                Errores = resultado.Errores.Select(e => new MessageErrorIntegridad
                {
                    Tabla = e.Tabla,
                    Id = e.Id,
                    DVEsperado = e.DVEsperado,
                    DVCalculado = e.DVCalculado,
                    TipoDV = e.TipoDV
                }).ToList()
            };

            GuardarBitacora(descripcion, tipoEvento);

            return resultadoFinal;
        }

        private void GuardarBitacora(string descripcion, TipoEvento tipoEvento)
        {
            var bitacora = new Bitacora
            {
                Id = System.Guid.NewGuid(),
                Usuario = new Usuario { IdUsuario = SessionManager.GetUsuario().IdUsuario },
                Fecha = System.DateTime.Now,
                Descripcion = descripcion,
                Origen = "ValidarIntegridadDB",
                TipoEvento = tipoEvento
            };
            _bitacoraRepository.GuardarBitacora(bitacora);
        }
    }
}
