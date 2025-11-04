using System;
using System.Collections.Generic;

namespace IngSoft.Services
{
    public class SingleInstancesManager
    {
        private static SingleInstancesManager _instance;

        // Listas estática de Object
        private static readonly List<object> _objetos = new List<object>();
        private static readonly object _lock = new object();

        private SingleInstancesManager() {}

        // Propiedad pública para obtener la instancia única
        public static SingleInstancesManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SingleInstancesManager();
                }
                return _instance;
            }
        }

        // Agrega un objeto a la lista si su tipo aún no fue agregado. Devuelve true si se agregó, false si ya existe el tipo.
        public bool AgregarObjeto(object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            lock (_lock)
            {
                foreach (object o in _objetos)
                {
                    if (o.GetType() == obj.GetType())
                    {
                        return false; // Ya existe un objeto de este tipo
                    }
                }

                _objetos.Add(obj);
                return true;
            }
        }

        // Obtiene la instancia de la clase solicitada (genérico). Devuelve null si no existe.
        public T ObtenerInstancia<T>() where T : class
        {
            lock (_lock)
            {
                foreach (var obj in _objetos)
                {
                    if (obj is T t) { return t; }
                }
            }
            return null;
        }

    }
}
