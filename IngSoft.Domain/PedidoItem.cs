namespace IngSoft.Domain
{
    public class PedidoItem : Entity
    {
        public string NombreProducto { get; set; }
        public int    Cantidad       { get; set; }
        public float  Precio        { get; set; }
        public float  Subtotal      { get; set; }
    }
}
