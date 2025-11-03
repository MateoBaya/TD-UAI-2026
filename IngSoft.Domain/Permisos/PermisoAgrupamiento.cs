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
            if (item == null) throw new ArgumentNullException(nameof(item));
            AddCompositable(item);
        }

        public override ICompositable AddCompositable(ICompositable compositable)
        {
            if (compositable == null) throw new ArgumentNullException(nameof(compositable));
            if(EncontrarRoot().Operacion(compositable.Nombre))
            {
                throw new InvalidOperationException("No se puede agregar un permiso como hijo de sí mismo o crear una referencia circular.");
            }

            if (!_children.Contains(compositable))
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

        // Nuevo: Ejecuta el action recursivamente en cada hoja del agrupamiento
        public override void Ejecutar(Action<PermisoComponent> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            action(this);

            foreach (var child in _children)
            {
                if (child is PermisoAgrupamiento agrupamiento)
                {
                    agrupamiento.Ejecutar(action);
                }
                else if (child is PermisoComponent permiso)
                {
                    action(permiso);
                }
            }
        }

        // Nuevo: devuelve todos los PermisoAgrupamiento del árbol (incluye this)
        public List<PermisoAgrupamiento> ObtenerTodosLosAgrupamientos()
        {
            var result = new List<PermisoAgrupamiento>();
            Ejecutar(c =>
            {
                if (c is PermisoAgrupamiento a)
                    result.Add(a);
            });
            return result;
        }

        // Nuevo: devuelve los agrupamientos cuyo Parent.Nombre coincide con padreNombre
        public List<PermisoAgrupamiento> ObtenerAgrupamientosPorPadre(string padreNombre)
        {
            var result = new List<PermisoAgrupamiento>();
            if (padreNombre == null) return result;

            Ejecutar(c =>
            {
                if (c is PermisoAgrupamiento a)
                {
                    var parent = (a as PermisoComponent).Parent;
                    var parentName = parent != null ? parent.Nombre : null;
                    if (parentName == padreNombre)
                        result.Add(a);
                }
            });

            return result;
        }

        // Nuevo: busca un agrupamiento por nombre dentro del árbol
        public PermisoAgrupamiento EncontrarAgrupamientoPorNombre(string nombre)
        {
            if (string.IsNullOrEmpty(nombre)) return null;
            PermisoAgrupamiento found = null;
            Ejecutar(c =>
            {
                if (found == null && c is PermisoAgrupamiento a && a.Nombre == nombre)
                {
                    found = a;
                }
            });
            return found;
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
                Parent.RemoveCompositable(this);
            }

            var item = BuscarRecursivo(compositable);
            
            if(item!= null&& this._children.Remove(item))
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
