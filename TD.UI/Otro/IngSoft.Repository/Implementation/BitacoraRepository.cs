using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using IngSoft.DBConnection;
using IngSoft.DBConnection.Factory;
using IngSoft.DBConnection.Models;
using IngSoft.Domain;
using IngSoft.Domain.Enums;
using IngSoft.Repository.Factory;

namespace IngSoft.Repository.Implementation
{
    public class BitacoraRepository : IBitacoraRepository
    {
        private readonly IConnection _connection;
        private IDigitoVerificadorRepository _digitoVerificadorRepository;

        internal BitacoraRepository(IConnection connection)
        {
            _connection = connection ?? ConnectionFactory.CreateSqlServerConnection();
        }
        private IDigitoVerificadorRepository DigitoVerificadorRepository
        {
            get
            {
                if (_digitoVerificadorRepository == null)
                {
                    _digitoVerificadorRepository = FactoryRepository.CreateDigitoVerificadorRepository();
                }
                return _digitoVerificadorRepository;
            }
        }

        public void GuardarBitacora(Bitacora bitacora)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["IngSoftConnection"].ConnectionString;
            _connection.NuevaConexion(connectionString);
            bitacora.Dvh = DigitoVerificadorRepository.CrearDVH(bitacora);
            
            try
            {

                var parametros = new Dictionary<string, object>
                {
                    {"@Id", bitacora.Id},
                    {"@IdUsuario", bitacora.Usuario.IdUsuario },
                    {"@Fecha", bitacora.Fecha },
                    {"@Descripcion", bitacora.Descripcion },
                    {"@Origen", bitacora.Origen },
                    {"@TipoEvento", bitacora.TipoEvento },
                    {"@DVH", bitacora.Dvh }
                };

                _connection.EjecutarSinResultado("GuardarBitacora", parametros);
                var dvvActualizado = DigitoVerificadorRepository.CrearDVV(nameof(Bitacora));
                DigitoVerificadorRepository.ActualizarDVV(nameof(Bitacora), dvvActualizado);
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
        public List<Bitacora> ObtenerBitacoras()
        {
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["IngSoftConnection"].ConnectionString;

                _connection.NuevaConexion(connectionString);

                var resultado = _connection.EjecutarDataSet<BitacoraQuerySql>("ObtenerBitacoras", new Dictionary<string, object>());

                var bitacoras = resultado.Select( b => new Bitacora
                {
                    Id = b.Id,
                    Descripcion = b.Descripcion,
                    Fecha = b.Fecha,
                    Origen = b.Origen,
                    TipoEvento = (TipoEvento)b.TipoEvento,
                    Dvh = b.Dvh,
                    Usuario = new Usuario
                    {
                        IdBitacora = b.UsuarioId,
                        Nombre = b.Nombre,
                        Apellido = b.Apellido,
                        Email = b.Email,
                        Contrasena = b.Contrasena,
                        UserName = b.UserName
                    }
                }).ToList();

                _connection.FinalizarConexion();

                return bitacoras;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Bitacora> ObtenerBitacorasFiltradas(string filtro)
        {
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["IngSoftConnection"].ConnectionString;

                _connection.NuevaConexion(connectionString);

                var parametros = new Dictionary<string, object>
                {
                    { "@Filtro", filtro }
                };

                var resultado = _connection.EjecutarDataSet<BitacoraQuerySql>("ObtenerBitacoraFiltradas", parametros);

                var bitacoras = resultado.Select(b => new Bitacora
                {
                    Id = b.Id,
                    Descripcion = b.Descripcion,
                    Fecha = b.Fecha,
                    Origen = b.Origen,
                    TipoEvento = (TipoEvento)b.TipoEvento,
                    Usuario = new Usuario
                    {
                        IdBitacora = b.UsuarioId,
                        Nombre = b.Nombre,
                        Apellido = b.Apellido,
                        Email = b.Email,
                        Contrasena = b.Contrasena,
                        UserName = b.UserName
                    }
                }).ToList();

                _connection.FinalizarConexion();

                return bitacoras;

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
