using System;

namespace IngSoft.Domain
{
    public class PedidoResumen : Entity
    {
        public int      NroCarrito         { get; set; }
        public string   Tipo               { get; set; }
        public string   Estado             { get; set; }
        public DateTime FechaInsert        { get; set; }
        public string   FechaEntregaTexto  { get; set; }
    }
}
