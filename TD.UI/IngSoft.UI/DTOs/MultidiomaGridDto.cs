using System;

namespace IngSoft.UI.DTOs
{
    public class MultidiomaGridDto
    {
        public string NombreControl { get; set; }
        public string TextoPorDefecto { get; set; }
        public string TextoTraducido { get; set; }
        public Guid IdControlIdioma { get; set; }
    }
}
