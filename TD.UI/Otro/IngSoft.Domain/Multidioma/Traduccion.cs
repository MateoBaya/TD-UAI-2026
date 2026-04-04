using System;
using IngSoft.Abstractions.Multidioma;

namespace IngSoft.Domain.Multidioma
{
    public class Traduccion : Entity, ITraduccion
    {
        public Guid IdIdioma { get; set; }
        public Guid IdControlIdioma { get; set; }
        public string TextoTraducido { get; set; }
    }
}
