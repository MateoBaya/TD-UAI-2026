using System;
using System.Linq;
using System.Drawing;
using System.Security.Authentication;
using System.Windows.Forms;
using IngSoft.Abstractions;
using IngSoft.Abstractions.Multidioma;
using IngSoft.ApplicationServices;
using IngSoft.ApplicationServices.Factory;
using IngSoft.Domain;
using IngSoft.Domain.Enums;
using IngSoft.Domain.Multidioma;
using IngSoft.Services;
using IngSoft.UI.Multidioma;


namespace IngSoft.UI
{
    /// <summary>
    /// Injects the login UI controls directly into FrmPrincipal's pnlMain,
    /// following the same pattern as FrmPermisosFlexibilizador.
    /// Also implements IObserver to react to language changes.
    /// </summary>
    internal class FrmLoginFlexibilizador
    {
        private readonly FrmPrincipal _form;
        private readonly IUsuarioServices _usuarioServices;
        private readonly IBitacoraServices _bitacoraServices;
        private readonly IDigitoVerificadorServices _digitoVerificadorServices;
        private readonly IMultidiomaServices _multidiomaServices;
        private readonly IPermisoServices _permisoServices;

        // Kept as fields so the post-login callback can read them
        private TextBox _txtUsuario;
        private TextBox _txtContrasena;

        // Callback invoked by the header config button after a successful login
        internal Action OnLoginSuccess;

        internal FrmLoginFlexibilizador()
        {
            _form = SingleInstancesManager.Instance.ObtenerInstancia<FrmPrincipal>();
            _usuarioServices = SingleInstancesManager.Instance.ObtenerInstancia<IUsuarioServices>();
            _bitacoraServices = SingleInstancesManager.Instance.ObtenerInstancia<IBitacoraServices>();
            _digitoVerificadorServices = ServicesFactory.CreateDigitoVerificadorServices();
            _multidiomaServices = ServicesFactory.CreateMultidiomaServices();
            _permisoServices = ServicesFactory.CreatePermisoServices();
        }

        // ── UI construction ──────────────────────────────────────────────────────

        /// <summary>
        /// Builds the login form controls inside pnlMain at the given positions.
        /// Call this from FrmLoginHeaderConfig after clearing other controls.
        /// </summary>
        internal void PantallaLoginCreator(Point pointUsuario, Point pointContrasena, Point pointButton)
        {
            var parent = _form.GetPanelMain;
            if (parent == null) return;

            _txtUsuario = FlexibilizadorFormularios.CreateTextBox(
                parent, "Usuario", pointUsuario);

            _txtContrasena = FlexibilizadorFormularios.CreatePasswordTextBox(
                parent, "Contrasena", pointContrasena);

            FlexibilizadorFormularios.CreateButton(
                parent,
                "btnLogin",
                pointButton,
                new System.Drawing.Size(200, 30),
                "Iniciar Sesión",
                LoginClickHandler);

            _form.AplicarIdiomaActual();
        }

        // ── Login logic ──────────────────────────────────────────────────────────

        private void LoginClickHandler(object sender, EventArgs e)
        {
            _usuarioServices.SetRegistradoBitacora(RegistrarEnBitacora);

            try
            {
                var usuarioActual = new Usuario
                {
                    UserName = _txtUsuario?.Text ?? string.Empty,
                    Contrasena = _txtContrasena?.Text ?? string.Empty
                };

                _usuarioServices.LoginUser(usuarioActual);
                var integridadDB = _digitoVerificadorServices.ValidarIntegridad();

                MessageBox.Show($"Iniciado sesión: {SessionManager.GetUsuario().UserName}");

                var permisos = _permisoServices.ObtenerPermisosPorUsuario(SessionManager.GetUsuario().UserName);
                var isAdmin = permisos.BuscarRecursivo(new PermisoAtomico { Nombre = "Admin" });

                if (!integridadDB.EsValida && isAdmin != null)
                {
                    var frmIntegridadDB = new FrmIntegridadDB(integridadDB);
                    frmIntegridadDB.ShowDialog();
                }

                // Notify the header config that login succeeded
                OnLoginSuccess?.Invoke();
            }
            catch (InvalidCredentialException)
            {
                MessageBox.Show("Credenciales Incorrectas");
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Su cuenta está bloqueada");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al iniciar sesión: {ex.Message}");
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
            _bitacoraServices.GuardarBitacora(bitacora);
        }

        // ── Cleanup ──────────────────────────────────────────────────────────────

        internal void EliminarControlesAdicionales()
        {
            FlexibilizadorFormularios.EliminarControlesAdicionalesForm(
                _form.GetPanelMain,
                _form.ControlesSalvar());
        }
    }
}
