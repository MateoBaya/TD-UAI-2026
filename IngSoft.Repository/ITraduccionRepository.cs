using System;
using IngSoft.Domain.Multidioma;

namespace IngSoft.Repository
{
    public interface ITraduccionRepository
    {
        void CrearTraduccion(Traduccion traduccion);
        void ActualizarTraduccion(Traduccion traduccion);
        Traduccion ObtenerTraduccionPorIdiomaYControlIdioma(Guid idIdioma, Guid idControlIdioma);
    }
}
