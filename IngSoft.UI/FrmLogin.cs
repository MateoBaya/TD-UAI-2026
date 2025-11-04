using System;
using System.Linq;
using System.Security.Authentication;
using System.Windows.Forms;
using IngSoft.Abstractions.Multidioma;
using IngSoft.ApplicationServices;
using IngSoft.ApplicationServices.Factory;
using IngSoft.Domain;
using IngSoft.Domain.Enums;
using IngSoft.Domain.Multidioma;
using IngSoft.Services;

namespace IngSoft.UI
{
    public partial class FrmLogin : Form, IObserver
    {
        private readonly IUsuarioServices usuarioServices;
        private readonly IBitacoraServices bitacoraServices;
        private readonly IDigitoVerificadorServices digitoVerificadorServices;
        private readonly IMultidiomaServices _multidiomaServices;
        //private readonly IUsuarioServices usuarioServices = SingleInstancesManager.Instance.ObtenerInstancia<IUsuarioServices>();
        //private readonly IBitacoraServices bitacoraServices = SingleInstancesManager.Instance.ObtenerInstancia<IBitacoraServices>();
        public FrmLogin()
        {
            InitializeComponent();
            usuarioServices = ServicesFactory.CreateUsuarioServices();
            bitacoraServices = ServicesFactory.CreateBitacoraServices();
            digitoVerificadorServices = ServicesFactory.CreateDigitoVerificadorServices();
            _multidiomaServices = ServicesFactory.CreateMultidiomaServices();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            usuarioServices.SetRegistradoBitacora(RegistrarEnBitacora);

            try
            {
                var mUsuarioActual = new Usuario
                {
                    UserName = txtUsuario.Text,
                    Contrasena = txtContrasena.Text
                };

                usuarioServices.LoginUser(mUsuarioActual);
                var integridadDB = digitoVerificadorServices.ValidarIntegridad();
                
                MessageBox.Show($"Iniciado sesion: {SessionManager.GetUsuario().UserName}");

                if (!integridadDB.EsValida)
                {
                    var frmIntegridadDB = new FrmIntegridadDB(integridadDB);
                    frmIntegridadDB.ShowDialog();
                }

                this.DialogResult = DialogResult.OK;
                //new FrmUsuario().EliminarControlesAdicionalesUsuario();
                this.Close();
            }
            catch(InvalidCredentialException)
            {
                MessageBox.Show($"Credenciales Incorrectas");
                this.DialogResult = DialogResult.None;
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show($"Su cuenta está bloqueada");
                this.DialogResult = DialogResult.None;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al iniciar sesión: {ex.Message}");
                this.DialogResult = DialogResult.None;
            }
        }

        private void RegistrarEnBitacora(Usuario usuario, string descripcion, string origen, TipoEvento tipoEvento)
        {            
            var bitacora = new Bitacora
            {
                Id = Guid.NewGuid(),
                Usuario = usuario,
                Fecha = DateTime.Now,
                Descripcion = descripcion,
                Origen = origen,
                TipoEvento = tipoEvento
            };
            bitacoraServices.GuardarBitacora(bitacora);
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {
            SuscribirseAIdiomaActual();
            AplicarIdiomaActual();
        }

        private void SuscribirseAIdiomaActual()
        {
            var idioma = MultidiomaManager.GetIdioma();

            if (idioma != null)
            {
                // Asegurarse de que sea la instancia cacheada
                if (idioma is Idioma idiomaConcreto)
                {
                    var idiomaCacheado = MultidiomaManager.ObtenerIdiomaCache(idiomaConcreto);
                    idiomaCacheado.Suscribir(this);
                }
                else
                {
                    idioma.Suscribir(this);
                }
            }
        }
        private void AplicarIdiomaActual()
        {
            // Aplicar el idioma actual al formulario
            if (MultidiomaManager.GetIdioma() != null)
            {
                var controles = _multidiomaServices.ObtenerControlesPorIdioma(MultidiomaManager.GetIdioma().Id)
                    .Cast<IControlIdioma>().ToList();
                MultidiomaManager.CambiarIdiomaControles(this, controles);
            }
        }

        public void Actualizar()
        {
            if (MultidiomaManager.GetIdioma() != null)
            {
                var controles = _multidiomaServices.ObtenerControlesPorIdioma(MultidiomaManager.GetIdioma().Id);
                MultidiomaManager.CambiarIdiomaControles(this, controles.Cast<IControlIdioma>().ToList());
            }
        }
    }
}
