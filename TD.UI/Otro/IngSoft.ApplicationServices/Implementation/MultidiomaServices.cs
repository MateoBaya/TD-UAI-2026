using System;
using System.Collections.Generic;
using IngSoft.Domain;
using IngSoft.Domain.Enums;
using IngSoft.Domain.Multidioma;
using IngSoft.Repository;
using IngSoft.Repository.Factory;
using IngSoft.Services;

namespace IngSoft.ApplicationServices.Implementation
{
    public class MultidiomaServices : IMultidiomaServices
    {
        private readonly IIdiomaRepository _idiomaRepository;
        private readonly IControlIdiomaRepository _controlIdiomaRepository;
        private readonly IBitacoraRepository _bitacoraRepository;
        private readonly ITraduccionRepository _traduccionRepository;

        public MultidiomaServices(IIdiomaRepository idiomaRepository, IControlIdiomaRepository controlIdiomaRepository, IBitacoraRepository bitacoraRepository, ITraduccionRepository traduccionRepository)
        {
            _idiomaRepository = idiomaRepository ?? FactoryRepository.CreateIdiomaRepository();
            _controlIdiomaRepository = controlIdiomaRepository ?? FactoryRepository.CreateControlIdiomaRepository();
            _bitacoraRepository = bitacoraRepository ?? FactoryRepository.CreateBitacoraRepository();
            _traduccionRepository = traduccionRepository ?? FactoryRepository.CreateTraduccionRepository();
        }

        public void ActualizarTraduccion(Traduccion traduccion)
        {
            _traduccionRepository.ActualizarTraduccion(traduccion);
        }

        public void CrearIdioma(Idioma idioma)
        {
            try
            {
                _idiomaRepository.CrearIdioma(idioma);
                GuardarBitacora($"Se creó el idioma: {idioma.Nombre}", TipoEvento.Message);
            }
            catch (Exception)
            {
                GuardarBitacora($"Error al crear el idioma: {idioma.Nombre}", TipoEvento.Error);
                throw;
            }
        }

        public void CrearTraduccion(Traduccion traduccion)
        {
            _traduccionRepository.CrearTraduccion(traduccion);
        }

        public List<ControlIdioma> ObtenerControlesPorIdioma(Guid idiomaId)
        {
            var resultado = _controlIdiomaRepository.ObtenerControlesPorIdioma(idiomaId);
            return resultado;
        }

        public Idioma ObtenerIdiomaPorDefecto()
        {
            var resultado = _idiomaRepository.ObtenerIdiomaPorDefecto();
            return resultado;
        }

        public List<Idioma> ObtenerIdiomas()
        {
            var resultado = _idiomaRepository.ObtenerIdiomas();
            return resultado;
        }

        public Traduccion ObtenerTraduccionPorIdiomaYControlIdioma(Guid idIdioma, Guid idControlIdioma)
        {
            return _traduccionRepository.ObtenerTraduccionPorIdiomaYControlIdioma(idIdioma, idControlIdioma);
        }

        private void GuardarBitacora(string descripcion, TipoEvento tipoEvento)
        {
            var bitacora = new Bitacora
            {
                Id = Guid.NewGuid(),
                Usuario = new Usuario { IdUsuario = SessionManager.GetUsuario().IdUsuario },
                Fecha = DateTime.Now,
                Descripcion = descripcion,
                TipoEvento = tipoEvento,
                Origen = "Multidioma"
            };
            _bitacoraRepository.GuardarBitacora(bitacora);            
        }
    }
}
