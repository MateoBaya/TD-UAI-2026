namespace IngSoft.Domain
{
    public class CarritoItem : Entity
    {
        public Producto Producto { get; set; }
        public int Cantidad { get; set; }
        public float Precio { get; set; }
    }
}
