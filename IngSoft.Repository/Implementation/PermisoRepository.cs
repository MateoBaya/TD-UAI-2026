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
            _connection.NuevaConexion(connectionString);
            try
            {
                var parametros = new Dictionary<string, object>
                {
                    {"@PermisoNombre", permiso.Nombre },
                    {"@Padre", permiso.ParentName }
                };
                _connection.EjecutarSinResultado("GuardarPermiso", parametros);
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
        public void GuardarPermisoEnUsuario(PermisoComponent permiso, string userName)
        {
            _connection.NuevaConexion(connectionString);
            try
            {
                var parametros = new Dictionary<string, object>
                {
                    {"@PermisoNombre", permiso.Nombre },
                    { "@Padre", permiso.ParentName },
                    {"@UserName", userName }
                };
                _connection.EjecutarSinResultado("AsignarPermisoAUsuario", parametros);
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
        public void EliminarPermiso(string permisoNombre)
        {
            _connection.NuevaConexion(connectionString);
            try
            {
                var parametros = new Dictionary<string, object>
                {
                    {"@PermisoNombre", permisoNombre },
                };
                _connection.EjecutarSinResultado("EliminarPermiso", parametros);
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
        public void ModificarPermiso(string permisoNombre, string nuevoNombre)
        {
            _connection.NuevaConexion(connectionString);
            try
            {
                var parametros = new Dictionary<string, object>
                {
                    {"@PermisoNombre", permisoNombre },
                    {"@NuevoNombre", nuevoNombre }
                };
                _connection.EjecutarSinResultado("ModificarPermiso", parametros);
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
                    permisosSimples = _connection.EjecutarDataTable<PermisoQuerySql>("ObtenerPermisosSimples", new Dictionary<string, object>());
                    permisosAgrupados = _connection.EjecutarDataTable<PermisoQuerySql>("ObtenerPermisosAgrupados", new Dictionary<string, object>());
                }
                else
                {
                    var parametros = new Dictionary<string, object>
                    {
                        {"@UserName", userName }
                    };
                    permisosSimples = _connection.EjecutarDataTable<PermisoQuerySql>("ObtenerPermisosSimplesPorUsuario", parametros);
                    permisosAgrupados = _connection.EjecutarDataTable<PermisoQuerySql>("ObtenerPermisosAgrupadosPorUsuario", parametros);
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

                    var subAgrupamientos = permisosAgrupados.Where(pa => pa.Padre == permisoAgrupamiento.Nombre).ToList();
                    foreach (var subAgrupamiento in subAgrupamientos)
                    {
                        var permisoSubAgrupamiento = permisosAgrupamiento.FirstOrDefault(p => p.Nombre == subAgrupamiento.Nombre);
                        if (permisoSubAgrupamiento != null)
                        {
                            permisoAgrupamiento.Add(permisoSubAgrupamiento);
                        }
                    }
                }

                // crear root y agregar el agrupamiento administrador si existe
                PermisoAgrupamiento newRoot = new PermisoAgrupamiento
                {
                    Nombre = "Root"
                };
                string nombreRoot = permisosAgrupados.FirstOrDefault(p => p.Padre == null)?.Nombre;
                PermisoAgrupamiento permisoRoot = permisosAgrupamiento.FirstOrDefault(p => p.Nombre == nombreRoot);
                if (permisoRoot != null)
                {
                    newRoot.Add(permisoRoot);
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
