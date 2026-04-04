using System;
using IngSoft.Domain.Enums;
using IngSoft.Abstractions;

namespace IngSoft.Domain
{
    public class Bitacora : Entity
    {
        public Usuario Usuario { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; }
        public string Origen { get; set; }
        public TipoEvento TipoEvento { get; set; }
        public string Dvh { get; set; }

    }
}

