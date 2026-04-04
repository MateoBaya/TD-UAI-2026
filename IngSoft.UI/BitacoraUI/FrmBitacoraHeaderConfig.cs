using System.Windows.Forms;
using IngSoft.Domain;
using IngSoft.Services;

namespace IngSoft.UI.BitacoraUI
{
    /// <summary>
    /// Configures the main MenuStrip for the Bitacora module and injects the
    /// Bitacora UI into pnlMain via FrmBitacoraFlexibilizador.
    ///
    /// Event wiring summary:
    ///   pnlMain_Resize  →  LastAction = ResizeControls   (dragged resize)
    ///   FrmPrincipal.Resize  →  _onFormResize = ResizeControls  (maximize / restore)
    ///   Both are subscribed inside PantallaBitacoraCreator and removed in
    ///   EliminarControlesAdicionales to avoid ghost handlers.
    /// </summary>
    internal class FrmBitacoraHeaderConfig
    {
        private readonly FrmPrincipal _formulario;
        private readonly FrmBitacoraFlexibilizador _flexibilizador;

        public FrmBitacoraHeaderConfig()
        {
            _formulario     = Services.SingleInstancesManager.Instance.ObtenerInstancia<FrmPrincipal>();
            _flexibilizador = new FrmBitacoraFlexibilizador();

            MenuStrip header = _formulario.MainMenuStrip;
            if (header != null)
            {
                FrmPrincipalFlexibilizador.HeaderClearer(header);
                _formulario.AplicarIdiomaActual();
                FlexibilizadorFormularios.MenuStripHider(
                    header,
                    SessionManager.GetPermisos() as PermisoComponent);
            }

            IniciarPantallaBitacora();
        }

        private void IniciarPantallaBitacora()
        {
            _flexibilizador.EliminarControlesAdicionales();
            _flexibilizador.PantallaBitacoraCreator();

            // LastAction handles dragged resize (fires from pnlMain_Resize).
            // Maximize is handled by _form.Resize subscribed inside PantallaBitacoraCreator.
            _formulario.LastAction = (s, e) => _flexibilizador.ResizeControls();
        }
    }
}
