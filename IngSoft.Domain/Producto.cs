using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngSoft.Domain
{
    public class Producto : Entity
    {
        string nombre;
        float precioActual;
        string marca;
        string modelo;
        bool aceptaMayorista;
        bool aceptaMinorista;
        int stock;

        public string Nombre { get => nombre; set => nombre = value; }
        public float PrecioActual { get => precioActual; set => precioActual = value; }
        public string Marca { get => marca; set => marca = value; }
        public string Modelo { get => modelo; set => modelo = value; }
        public bool AceptaMayorista { get => aceptaMayorista; set => aceptaMayorista = value; }
        public bool AceptaMinorista { get => aceptaMinorista; set => aceptaMinorista = value; }
        public int Stock { get => stock; set => stock = value; }
    }
}
