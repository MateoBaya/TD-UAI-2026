namespace IngSoft.Domain
{
    public class CarritoItem : Entity
    {
        private Producto producto;
        private int cantidad;
        private float precioUnitario;

        public Producto Producto { get => producto; set => producto = value; }
        public int Cantidad { get => cantidad; set => cantidad = value; }
        public float PrecioUnitario { get => precioUnitario; set => precioUnitario = value; }
        public float Subtotal => cantidad * precioUnitario;
    }
}
