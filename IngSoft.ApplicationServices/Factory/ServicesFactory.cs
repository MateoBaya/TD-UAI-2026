using IngSoft.ApplicationServices.Implementation;
using IngSoft.Repository;
using IngSoft.Repository.Factory;

namespace IngSoft.ApplicationServices.Factory
{
    public abstract class ServicesFactory
    {
        public static IBitacoraServices CreateBitacoraServices()
        {
            var bitacoraRepository = FactoryRepository.CreateBitacoraRepository();
            return new BitacoraServices(bitacoraRepository);
        }
        public static IUsuarioServices CreateUsuarioServices()
        {
            IUsuarioRepository usuarioRepository = FactoryRepository.CreateUsuarioRepository();
            IUsuarioHistoricoServices usuarioHistoricoServices = CreateUsuarioHistoricoServices();
            return new UsuarioServices(usuarioRepository, usuarioHistoricoServices);
        }
        public static IDigitoVerificadorServices CreateDigitoVerificadorServices()
        {
            var digitoVerificadorRepository = FactoryRepository.CreateDigitoVerificadorRepository();
            var bitacoraRepository = FactoryRepository.CreateBitacoraRepository();
            return new DigitoVerificadorServices(digitoVerificadorRepository, bitacoraRepository);
        }
        public static IUsuarioHistoricoServices CreateUsuarioHistoricoServices()
        {
            var usuarioHistoricoRepository = FactoryRepository.CreateUsuarioHistoricoRepository();
            var bitacoraRepository = FactoryRepository.CreateBitacoraRepository();
            return new UsuarioHistoricoServices(usuarioHistoricoRepository, bitacoraRepository);
        }
        public static IMultidiomaServices CreateMultidiomaServices()
        {
            var idiomaRepository = FactoryRepository.CreateIdiomaRepository();
            var controlIdiomaRepository = FactoryRepository.CreateControlIdiomaRepository();
            var bitacoraRepository = FactoryRepository.CreateBitacoraRepository();
            var traduccionRepository = FactoryRepository.CreateTraduccionRepository();
            return new MultidiomaServices(idiomaRepository, controlIdiomaRepository, bitacoraRepository, traduccionRepository);
        }
        public static IPermisoServices CreatePermisoServices()
        {
            IPermisoRepository permisoRepository = FactoryRepository.CreatePermisoRepository();
            return new PermisoServices(permisoRepository);
        }
    }
}
