using System.Collections.Generic;

namespace IngSoft.Repository.Dto
{
    public class ResultadoIntegridad
    {
        public bool EsValida { get; set; }
        public string Mensaje { get; set; }
        public List<MessageErrorIntegridad> Errores { get; set; }
    }
}
