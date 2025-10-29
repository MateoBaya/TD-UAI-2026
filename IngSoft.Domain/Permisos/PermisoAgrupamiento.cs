using IngSoft.Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngSoft.Domain
{
    public class PermisoAgrupamiento : PermisoComponent, ICollection<PermisoComponent>
    {
        private readonly List<ICompositable> _children = new List<ICompositable>();

        public int Count => _children.Count;

        public bool IsReadOnly => false;

        public void Add(PermisoComponent item)
        {
            AddCompositable(item as ICompositable);
        }

        public override ICompositable AddCompositable(ICompositable compositable)
        {
            if (compositable == null) throw new ArgumentNullException(nameof(compositable));

            if (!(EncontrarRoot().BuscarRecursivo(compositable) is null))
            {
                _children.Add(compositable);
                compositable.RaisePermisoAsignado(this);

            }
            return this;
        }

        public void Clear()
        {
            // Llamar al evento de eliminación en cada hijo para que limpien su Parent
            foreach (var child in _children.ToList())
            {
                child.RaisePermisoEliminado();
            }
            _children.Clear();
        }

        public override ICompositable ClearCompositable()
        {
            Clear();
            return this;
        }

        // Busca recursivamente el target entre los hijos y sus descendientes.
        // Devuelve la instancia encontrada o null si no existe.
        public override ICompositable BuscarRecursivo(ICompositable target)
        {
            if (target == null) return null;

            foreach (var child in _children)
            {
                if (target.Nombre.Equals(child.Nombre))
                    return child;

                if (child is PermisoAgrupamiento agrupamiento)
                {
                    var found = agrupamiento.BuscarRecursivo(target);
                    if (found != null)
                        return found;
                }
            }

            return null;
        }

        public bool Contains(PermisoComponent item)
        {
            if (item == null) return false;
            return BuscarRecursivo(item) != null;
        }

        public void CopyTo(PermisoComponent[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0 || arrayIndex > array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < _children.Count) throw new ArgumentException("El tamaño del arreglo es insuficiente.");

            for (int i = 0; i < _children.Count; i++)
            {
                array[arrayIndex + i] = _children[i] as PermisoComponent;
            }
        }

        public IEnumerator<PermisoComponent> GetEnumerator()
        {
            foreach (var child in _children)
            {
                yield return child as PermisoComponent;
            }
        }

        public override ICollection<ICompositable> GetListCompositable()
        {
            return _children.AsReadOnly();
        }

        public override bool Operacion(string userAction)
        {
            PermisoComponent permiso = new PermisoAgrupamiento();
            permiso.Nombre = userAction;
            return Contains(permiso);
        }

        public bool Remove(PermisoComponent item)
        {
            if (item == null) return false;
            if(RemoveCompositable(item) != null)
                return true;
            else
                return false;
        }

        public override ICompositable RemoveCompositable(ICompositable compositable)
        {
            if (compositable == null)
            {
                throw new ArgumentNullException(nameof(compositable));
            }

            var item = BuscarRecursivo(compositable);
            
            if(item!= null&& ((PermisoAgrupamiento)item)._children.Remove(item))
            {
                item.RaisePermisoEliminado();
            }

            return item;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
