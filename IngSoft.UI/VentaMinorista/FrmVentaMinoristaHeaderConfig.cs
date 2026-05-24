using IngSoft.ApplicationServices.Factory;
using IngSoft.Domain;
using IngSoft.Services;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace IngSoft.UI
{
    /// <summary>
    /// Configura el header para el módulo Minorista e inyecta la UI en pnlMain.
    /// Sigue el mismo patrón que FrmBitacoraHeaderConfig:
    ///   - No agrega ítems al header (el módulo no los necesita)
    ///   - Llama IniciarVenta() directamente desde el constructor
    ///   - LastAction = ResizeControls cubre resize de sidebar y ventana
    /// </summary>
    internal class FrmVentaMinoristaHeaderConfig
    {
        private readonly FrmPrincipal _formulario;
        private readonly FrmVentaMinoristaFlexibilizador _flex;

        public FrmVentaMinoristaHeaderConfig()
        {
            _formulario = SingleInstancesManager.Instance.ObtenerInstancia<FrmPrincipal>();
            _flex       = new FrmVentaMinoristaFlexibilizador();

            var header = _formulario.MainMenuStrip;
            if (header != null)
            {
                FrmPrincipalFlexibilizador.HeaderClearer(header);
                FlexibilizadorFormularios.MenuStripHider(
                    header, SessionManager.GetPermisos() as PermisoComponent);
                _formulario.AplicarIdiomaActual();
            }

            IniciarVenta();
        }

        private void IniciarVenta()
        {
            _flex.EliminarControlesAdicionalesVentaMinorista();

            List<Producto> productos;
            try
            {
                productos = ServicesFactory.CreateProductoServices()
                    .BuscarProductosValidos(new Producto { AceptaMinorista = true });
            }
            catch (Exception)
            {
                productos = new List<Producto>();
            }

            if (productos == null || productos.Count == 0)
            {
                MessageBox.Show("No hay productos disponibles.");
                return;
            }

            _flex.ConstruirLayout(productos);

            _formulario.LastAction = (s, e) => _flex.ResizeControls();
            _formulario.AplicarIdiomaActual();
        }
    }
}
