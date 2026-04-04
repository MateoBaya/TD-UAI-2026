using System;

namespace IngSoft.Abstractions.Multidioma
{
    public interface IControlIdioma : IEntity
    {
        string NombreControl { get; set; }
        Guid IdIdioma { get; set; }
        string TextoTraducido { get; set; }
    }
}
