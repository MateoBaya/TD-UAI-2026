using IngSoft.ApplicationServices.Factory;
using IngSoft.Domain;
using IngSoft.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace IngSoft.UI
{
    internal class FrmVentaMinoristaHeaderConfig
    {
        private readonly FrmPrincipal formulario;
        private readonly FrmVentaMinoristaFlexibilizador flexibilizador;
        private readonly EventHandler iniciarVentaOnClick;

        public FrmVentaMinoristaHeaderConfig()
        {
            formulario = SingleInstancesManager.Instance.ObtenerInstancia<FrmPrincipal>();
            flexibilizador = new FrmVentaMinoristaFlexibilizador();

            iniciarVentaOnClick = IniciarVentaEventHandler;

            MenuStrip header = formulario.MainMenuStrip;
            if (header == null)
                return;

            var iniciarVentaMenu = new ToolStripMenuItem("Venta Minorista")
            {
                Name = "iniciarVentaMinoristaToolStripMenuItem"
            };
            iniciarVentaMenu.Click += iniciarVentaOnClick;

            FrmPrincipalFlexibilizador.HeaderClearer(header);
            header.Items.Add(iniciarVentaMenu);

            FlexibilizadorFormularios.MenuStripHider(header, SessionManager.GetPermisos() as PermisoComponent);
            formulario.AplicarIdiomaActual();
        }

        private void IniciarVentaEventHandler(object sender, EventArgs e)
        {
            flexibilizador.EliminarControlesAdicionalesVentaMinorista();

            List<Producto> productos;
            try
            {
                var filtro = new Producto { AceptaMinorista = true };
                productos = ServicesFactory.CreateProductoServices().BuscarProductosValidos(filtro);
            }
            catch (Exception)
            {
                productos = new List<Producto>();
            }

            if (productos == null || productos.Count == 0)
            {
                MessageBox.Show("Ningún producto disponible para venta minorista.");
                return;
            }

            flexibilizador.Productos = productos;

            var dgvPosition = new Point(formulario.GetPanelMain.Width / 16, formulario.GetPanelMain.Height / 8);
            var dgvSize = new Size(
                formulario.GetPanelMain.Width / 2 - formulario.GetPanelMain.Width / 16,
                formulario.GetPanelMain.Height / 2 + formulario.GetPanelMain.Height / 4);

            flexibilizador.DataGridViewProductosConSeleccion(productos, dgvPosition, dgvSize);

            var rightX = formulario.GetPanelMain.Width / 2 + 20;
            var baseY = formulario.GetPanelMain.Height / 8;
            var offset = formulario.GetPanelMain.Height / 10;

            var txtId = FlexibilizadorFormularios.CreateTextBox(formulario.GetPanelMain, "Id", new Point(rightX, baseY));
            txtId.Visible = false;
            var lblId = formulario.GetPanelMain.Controls.Find("lblId", true);
            if (lblId.Length > 0) lblId[0].Visible = false;

            flexibilizador.TextBoxCantidadCreator(new Point(rightX, baseY + offset));
            flexibilizador.AgregarItemButtonCreator(new Point(rightX, baseY + offset * 2));
            flexibilizador.FinalizarCarritoButtonCreator(new Point(rightX, baseY + offset * 3));

            formulario.AplicarIdiomaActual();
            formulario.LastAction = IniciarVentaEventHandler;
        }
    }
}
