using System.Collections.Generic;
using IngSoft.Abstractions.Multidioma;

namespace IngSoft.Domain.Multidioma
{
    public class Idioma : Entity, IIdioma
    {
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public bool isDefault { get; set; } = false;

        private readonly List<IObserver> _observers = new List<IObserver>();

        public void Desuscribir(IObserver observer)
        {
            _observers.Remove(observer);
        }

        public void NotificarObservers()
        {
            _observers.ForEach(o => o.Actualizar());
        }

        public void Suscribir(IObserver observer)
        {
            _observers.Add(observer);
        }
    }
}
