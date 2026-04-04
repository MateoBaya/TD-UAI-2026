using System;

namespace IngSoft.Abstractions.Multidioma
{
    public interface ITraduccion : IEntity
    {
        Guid IdIdioma { get; set; }
        Guid IdControlIdioma { get; set; }
        string TextoTraducido { get; set; }
    }
}
