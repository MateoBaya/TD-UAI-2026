using IngSoft.Domain;
using IngSoft.Services;
using System.Windows.Forms;

namespace IngSoft.UI
{
    internal class FrmMisPedidosHeaderConfig
    {
        private readonly FrmPrincipal _formulario;
        private readonly FrmMisPedidosFlexibilizador _flex;

        public FrmMisPedidosHeaderConfig()
        {
            _formulario = SingleInstancesManager.Instance.ObtenerInstancia<FrmPrincipal>();
            _flex       = new FrmMisPedidosFlexibilizador();

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
