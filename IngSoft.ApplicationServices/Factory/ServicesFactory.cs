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
            return new UsuarioServices(usuarioRepository);
        }
        public static IPermisoServices CreatePermisoServices()
        {
            IPermisoRepository permisoRepository = FactoryRepository.CreatePermisoRepository();
            return new PermisoServices(permisoRepository);
        }
    }
}
