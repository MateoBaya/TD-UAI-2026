using IngSoft.DBConnection;
using IngSoft.DBConnection.Factory;
using IngSoft.Repository.Implementation;

namespace IngSoft.Repository.Factory
{
    public abstract class FactoryRepository
    {
        public static IBitacoraRepository CreateBitacoraRepository()
        {
            var connection = ConnectionFactory.CreateSqlServerConnection();
            return new BitacoraRepository(connection);
        }
        public static IUsuarioRepository CreateUsuarioRepository()
        {
            IConnection connection = ConnectionFactory.CreateSqlServerConnection();
            return new UsuarioRepository(connection);
        }
        public static IDigitoVerificadorRepository CreateDigitoVerificadorRepository()
        {
            IConnection connection = ConnectionFactory.CreateSqlServerConnection();
            IBitacoraRepository bitacoraRepository = CreateBitacoraRepository();
            IUsuarioRepository usuarioRepository = CreateUsuarioRepository();
            return new DigitoVerificadorRepository(bitacoraRepository, usuarioRepository, connection);
        }
        public static IUsuarioHistoricoRepository CreateUsuarioHistoricoRepository()
        {
            IConnection connection = ConnectionFactory.CreateSqlServerConnection();
            return new UsuarioHistoricoRepository(connection);
        }
        public static IIdiomaRepository CreateIdiomaRepository()
        {
            IConnection connection = ConnectionFactory.CreateSqlServerConnection();
            return new IdiomaRepository(connection);
        }
        public static IControlIdiomaRepository CreateControlIdiomaRepository()
        {
            IConnection connection = ConnectionFactory.CreateSqlServerConnection();
            return new ControlIdiomaRepository(connection);
        }
        public static ITraduccionRepository CreateTraduccionRepository()
        {
            IConnection connection = ConnectionFactory.CreateSqlServerConnection();
            return new TraduccionRepository(connection);
        }
        public static IPermisoRepository CreatePermisoRepository()
        {
            IConnection connection = ConnectionFactory.CreateSqlServerConnection();
            return new PermisoRepository(connection);
        }
        public static IBackupRepository CreateBackupRepository()
        {
            IConnection connection = ConnectionFactory.CreateSqlServerConnection();
            return new BackupRepository(connection);
        }
        public static IProductoRepository CreateProductoRepository()
        {
            IConnection connection = ConnectionFactory.CreateSqlServerConnection();
            return new ProductoRepository(connection);
        }
        public static ICarritoRepository CreateCarritoRepository()
        {
            IConnection connection = ConnectionFactory.CreateSqlServerConnection();
            return new CarritoRepository(connection);
        }
        public static ICarritoMinoristaRepository CreateCarritoMinoristaRepository()
        {
            IConnection connection = ConnectionFactory.CreateSqlServerConnection();
            return new CarritoMinoristaRepository(connection);
        }
        public static ICarritoMayoristaRepository CreateCarritoMayoristaRepository()
        {
            IConnection connection = ConnectionFactory.CreateSqlServerConnection();
            return new CarritoMayoristaRepository(connection);
        }
        public static IVentaRepository CreateVentaRepository()
        {
            IConnection connection = ConnectionFactory.CreateSqlServerConnection();
            return new VentaRepository(connection);
        }
    }
}
