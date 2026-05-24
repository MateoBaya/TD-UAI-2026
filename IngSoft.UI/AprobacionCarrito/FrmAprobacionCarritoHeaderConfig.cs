using IngSoft.Domain;
using IngSoft.Services;
using System.Windows.Forms;

namespace IngSoft.UI
{
    internal class FrmAprobacionCarritoHeaderConfig
    {
        private readonly FrmPrincipal _formulario;
        private readonly FrmAprobacionCarritoFlexibilizador _flex;

        public FrmAprobacionCarritoHeaderConfig()
        {
            _formulario = SingleInstancesManager.Instance.ObtenerInstancia<FrmPrincipal>();
            _flex       = new FrmAprobacionCarritoFlexibilizador();

            var header = _formulario.MainMenuStrip;
            if (header != null)
            {
                FrmPrincipalFlexibilizador.HeaderClearer(header);
                FlexibilizadorFormularios.MenuStripHider(
                    header, SessionManager.GetPermisos() as PermisoComponent);
                _formulario.AplicarIdiomaActual();
            }

            _flex.EliminarControlesAdicionales();
            _flex.ConstruirLayout();

            _formulario.LastAction = (s, e) => _flex.ResizeControls();
            _formulario.AplicarIdiomaActual();
        }
    }
}
