using System;
using IngSoft.Abstractions.Multidioma;

namespace IngSoft.Domain.Multidioma
{
    public class ControlIdioma : Entity, IControlIdioma
    {
        public string NombreControl { get; set; }
        public Guid IdIdioma { get; set; }
        public string TextoTraducido { get; set; }
    }
}
