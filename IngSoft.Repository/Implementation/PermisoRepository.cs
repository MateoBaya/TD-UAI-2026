using IngSoft.DBConnection;
using IngSoft.DBConnection.Factory;
using IngSoft.DBConnection.Models;
using IngSoft.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace IngSoft.Repository.Implementation
{
    public class PermisoRepository : IPermisoRepository
    {
        private readonly IConnection _connection;
        private static readonly string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["IngSoftConnection"].ConnectionString;

        internal PermisoRepository(IConnection connection)
        {
            _connection = connection ?? ConnectionFactory.CreateSqlServerConnection();
        }
        public void GuardarPermiso(PermisoComponent permiso)
        {
            Dictionary<string, object> parametros;
            _connection.NuevaConexion(connectionString);
            if(string.IsNullOrEmpty(permiso.ParentName))
            {
                parametros = new Dictionary<string, object>
                {
                    { "@IdPermiso", permiso.Id },
                    {"@NombrePermiso", permiso.Nombre }
                };
            }
            else
            {
                parametros = new Dictionary<string, object>
                {
                    { "@IdPermiso", permiso.Id },
                    {"@NombrePermiso", permiso.Nombre },
                    {"@PermisoParent", permiso.ParentName }
                };
            }
            try
            {

                _connection.EjecutarSinResultado("AltaPermiso", parametros);
            }
            catch (Exception)
            {
                _connection.CancelarTransaccion();
                throw;
            }
            finally
            {
                _connection.FinalizarConexion();
            }
        }

        // Nuevo método: guarda todo un composite en una sola transacción usando PermisoComponent.Ejecutar
        public void AsignarPermisoEnUsuario(PermisoComponent permisoComposite,string pUserName)
        {
            if (permisoComposite == null) throw new ArgumentNullException(nameof(permisoComposite));

            _connection.NuevaConexion(connectionString);
            try
            {
                _connection.IniciarTransaccion();

                // Acción que se pasará a Ejecutar: guarda cada permiso usando el mismo connection/transaction
                Action<PermisoComponent> action = permiso =>
                {
                    var parametros = new Dictionary<string, object>
                    {
                        {"@NombrePermiso", permiso.Nombre },
                        {"@UserName", pUserName }
                    };
                    _connection.EjecutarSinResultado("AsignarPermisoUsuario", parametros);
                };

                // Ejecutar el plan (recursivo en los agrupamientos)
                permisoComposite.Ejecutar(action);
                //if (permisoComposite is PermisoAgrupamiento)
                //    foreach (var child in (permisoComposite as PermisoAgrupamiento))
                //    {
                //        child.Ejecutar(action);
                //    }

                // Si todo sale bien aceptar transacción
                _connection.AceptarTransaccion();
            }
            catch (Exception)
            {
                // En caso de error revertir la transacción y propagar
                _connection.CancelarTransaccion();
        
                throw;
            }
            finally
            {
                _connection.FinalizarConexion();
            }
        }

        // Nuevo método: elimina en una sola transacción los permisos del composite para el usuario usando PermisoComponent.Ejecutar
        public void EliminarPermisosDeUsuario(PermisoComponent permisoComposite, string pUserName)
        {
            if (permisoComposite == null) throw new ArgumentNullException(nameof(permisoComposite));

            _connection.NuevaConexion(connectionString);
            try
            {
                _connection.IniciarTransaccion();

                Action<PermisoComponent> action = permiso =>
                {
                    var parametros = new Dictionary<string, object>
                    {
                        {"@NombrePermiso", permiso.Nombre },
                        {"@UserName", pUserName }
                    };
                    _connection.EjecutarSinResultado("EliminarPermisoUsuario", parametros);
                };

                permisoComposite.Ejecutar(action);

                _connection.AceptarTransaccion();
            }
            catch (Exception)
            {
                _connection.CancelarTransaccion();
                throw;
            }
            finally
            {
                _connection.FinalizarConexion();
            }
        }

        public void EliminarPermiso(string permisoNombre)
        {
            _connection.NuevaConexion(connectionString);
            try
            {
                var parametros = new Dictionary<string, object>
                {
                    {"@NombrePermiso", permisoNombre },
                };
                _connection.EjecutarSinResultado("EliminarPermiso", parametros);
            }
            catch (Exception)
            {
                _connection.CancelarTransaccion();
                throw;
            }
            finally
            {
                _connection.FinalizarConexion();
            }
        }
        public void ModificarPermiso(string permisoNombre, string nuevoNombre, string permisoPadre=null)
        {
            _connection.NuevaConexion(connectionString);
            try
            {
                Dictionary<string,object> parametros;
                if (string.IsNullOrEmpty(permisoPadre))
                {
                    parametros = new Dictionary<string, object>
                    {
                        {"@NombrePermiso", permisoNombre },
                        {"@NombreNuevo", nuevoNombre }
                    };
                }
                else
                {
                    parametros = new Dictionary<string, object>
                    {
                        {"@NombrePermiso", permisoNombre },
                        {"@NombreNuevo", nuevoNombre },
                        { "@PermisoParent", permisoPadre }
                    };
                }
                _connection.EjecutarSinResultado("ModificarPermiso", parametros);
            }
            catch (Exception)
            {
                _connection.CancelarTransaccion();
                throw;
            }
            finally
            {
                _connection.FinalizarConexion();
            }
        }

        public PermisoComponent ObtenerTodosLosPermisos()
        {
            return ObtenerTodosLosPermisos(null);
        }

        // Si userName es null se obtienen todos los permisos,
        // si no, se obtienen solo los permisos asignados al usuario.
        internal PermisoComponent ObtenerTodosLosPermisos(string userName)
        {
            _connection.NuevaConexion(connectionString);
            try
            {
                // Obtener las listas de permisos (simples y agrupados). Si se proporciona userName se usan los procedimientos por usuario.
                List<PermisoQuerySql> permisosSimples;
                List<PermisoQuerySql> permisosAgrupados;

                if (string.IsNullOrEmpty(userName))
                {
                    permisosSimples = _connection.EjecutarDataSet<PermisoQuerySql>("ObtenerPermisosSimples", new Dictionary<string, object>());
                    permisosAgrupados = _connection.EjecutarDataSet<PermisoQuerySql>("ObtenerPermisosAgrupados", new Dictionary<string, object>());
                }
                else
                {
                    var parametros = new Dictionary<string, object>
                    {
                        {"@UserName", userName }
                    };
                    permisosSimples = _connection.EjecutarDataSet<PermisoQuerySql>("ObtenerPermisosSimplesPorUsuario", parametros);
                    permisosAgrupados = _connection.EjecutarDataSet<PermisoQuerySql>("ObtenerPermisosAgrupadosPorUsuario", parametros);
                }

                // construir instancias de permisos a partir de las filas obtenidas
                List<PermisoAtomico> permisos = permisosSimples.Select(p => new PermisoAtomico
                {
                    Nombre = p.Nombre
                }).ToList();

                List<PermisoAgrupamiento> permisosAgrupamiento = permisosAgrupados.Select(p => new PermisoAgrupamiento
                {
                    Nombre = p.Nombre
                }).ToList();

                foreach(var permisoAgrupamiento in permisosAgrupamiento)
                {
                    var hijos = permisosSimples.Where(ps => ps.Padre == permisoAgrupamiento.Nombre).ToList();
                    foreach (var hijo in hijos)
                    {
                        var permisoHijo = permisos.FirstOrDefault(p => p.Nombre == hijo.Nombre);
                        if (permisoHijo != null)
                        {
                            permisoAgrupamiento.Add(permisoHijo);
                        }
                    }

                    // Asignar sub-agrupamientos: buscar filas agrupadas donde Padre == agrup.Nombre
                    var subGrupo = permisosAgrupados.Where(pa => pa.Padre == permisoAgrupamiento.Nombre).ToList();

                    foreach (var grupoHijo in subGrupo)
                    {
                        // encontrar la instancia del sub-agrupamiento usando el tempRoot
                        var permisoSubAgrupamiento = permisosAgrupamiento.FirstOrDefault(p => p.Nombre == grupoHijo.Nombre);

                        if (permisoSubAgrupamiento != null)
                        {
                            permisoAgrupamiento.Add(permisoSubAgrupamiento);
                        }
                    }
                }

                // crear root final y agregar agrupamientos que no tienen padre conocido
                PermisoAgrupamiento newRoot = new PermisoAgrupamiento
                {
                    Nombre = "Root"
                };


                string nombreRoot = permisosAgrupados.FirstOrDefault(p => p.Padre == null)?.Nombre;
                PermisoAgrupamiento permisoRoot = permisosAgrupamiento.FirstOrDefault(p => p.Nombre == nombreRoot);

                // permisos sin padres conocidos (excepto el root elegido)
                if (permisoRoot != null)
                    newRoot.Add(permisoRoot);

                List<PermisoAgrupamiento> permisosAgrupadosSinPadresConocidos = permisosAgrupamiento
                    .Where(p => p.ParentName == null && p.Nombre != nombreRoot)
                    .ToList();
                List<PermisoAtomico> permisosSimplesSinPadresConocidos = permisos
                    .Where(p => p.ParentName == null && p.Nombre != nombreRoot)
                    .ToList();

                foreach (PermisoAgrupamiento permiso in permisosAgrupadosSinPadresConocidos)
                {
                    newRoot.Add(permiso);
                }
                foreach (PermisoAtomico permiso in permisosSimplesSinPadresConocidos) 
                {
                    newRoot.Add(permiso);
                }

                return newRoot;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _connection.FinalizarConexion();
            }
        }

        public PermisoComponent ObtenerPermisosPorUsuario(string userName)
        {
            // Delegar a ObtenerTodosLosPermisos pasando el userName
            return ObtenerTodosLosPermisos(userName);
        }
    }
}
