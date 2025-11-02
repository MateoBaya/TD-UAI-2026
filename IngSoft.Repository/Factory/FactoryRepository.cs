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
    }
}
