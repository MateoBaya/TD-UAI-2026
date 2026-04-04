using System;
using System.Collections.Generic;
using IngSoft.Domain.Multidioma;

namespace IngSoft.ApplicationServices
{
    public interface IMultidiomaServices
    {
        List<Idioma> ObtenerIdiomas();
        Idioma ObtenerIdiomaPorDefecto();
        void CrearIdioma(Idioma idioma);
        List<ControlIdioma> ObtenerControlesPorIdioma(Guid idiomaId);
        void CrearTraduccion(Traduccion traduccion);
        void ActualizarTraduccion(Traduccion traduccion);
        Traduccion ObtenerTraduccionPorIdiomaYControlIdioma(Guid idIdioma, Guid idControlIdioma);
    }
}
