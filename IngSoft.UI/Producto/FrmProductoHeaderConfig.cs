using IngSoft.ApplicationServices.Factory;
using IngSoft.Domain;
using IngSoft.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace IngSoft.UI
{
    internal class FrmProductoHeaderConfig
    {
        private readonly FrmPrincipal formulario;
        private readonly FrmProductoFlexibilizador flexibilizador;
        private readonly EventHandler agregarProductoOnClick;
        private readonly EventHandler verProductosOnClick;
        private readonly EventHandler eliminarProductoOnClick;
        private readonly EventHandler modificarProductoOnClick;

        public FrmProductoHeaderConfig()
        {
            formulario = SingleInstancesManager.Instance.ObtenerInstancia<FrmPrincipal>();
            flexibilizador = new FrmProductoFlexibilizador();

            agregarProductoOnClick = AgregarProductoEventHandler;
            modificarProductoOnClick = ModificarProductoEventHandler;
            verProductosOnClick = VerTodosProductosEventHandler;
            eliminarProductoOnClick = EliminarProductoEventHandler;

            MenuStrip header = formulario.MainMenuStrip;
            if (header == null)
                return;

            var agregarProductoMenu = new ToolStripMenuItem("Agregar Producto")
            {
                Name = "agregarProductoToolStripMenuItem"
            };
            agregarProductoMenu.Click += agregarProductoOnClick;

            var modificarProductoMenu = new ToolStripMenuItem("Modificar Producto")
            {
                Name = "modificarProductoToolStripMenuItem"
            };
            modificarProductoMenu.Click += modificarProductoOnClick;

            var verProductosMenu = new ToolStripMenuItem("Ver Todos")
            {
                Name = "verProductosToolStripMenuItem"
            };
            verProductosMenu.Click += verProductosOnClick;

            var eliminarProductoMenu = new ToolStripMenuItem("Eliminar Producto")
            {
                Name = "eliminarProductoToolStripMenuItem"
            };
            eliminarProductoMenu.Click += eliminarProductoOnClick;

            FrmPrincipalFlexibilizador.HeaderClearer(header);
            header.Items.Add(agregarProductoMenu);
            header.Items.Add(modificarProductoMenu);
            header.Items.Add(verProductosMenu);
            header.Items.Add(eliminarProductoMenu);

            FlexibilizadorFormularios.MenuStripHider(header, Services.SessionManager.GetPermisos() as PermisoComponent);
            formulario.AplicarIdiomaActual();
        }

        private void AgregarProductoEventHandler(object sender, EventArgs e)
        {
            flexibilizador.EliminarControlesAdicionalesProducto();

            var baseX = formulario.GetPanelMain.Width / 4;
            var baseY = formulario.GetPanelMain.Height / 8;
            var offset = formulario.GetPanelMain.Height / 10;

            flexibilizador.TextBoxCreator("Nombre", new Point(baseX, baseY));
            flexibilizador.TextBoxCreator("Marca", new Point(baseX, baseY + offset));
            flexibilizador.TextBoxCreator("Modelo", new Point(baseX, baseY + offset * 2));
            flexibilizador.TextBoxCreator("PrecioActual", new Point(baseX, baseY + offset * 3));
            flexibilizador.TextBoxCreator("Stock", new Point(baseX, baseY + offset * 4));
            flexibilizador.CheckboxCreator("AceptaMayorista", new Point(baseX, baseY + offset * 5));
            flexibilizador.CheckboxCreator("AceptaMinorista", new Point(baseX, baseY + offset * 6));
            flexibilizador.GuardarProductoButtonCreator();
            formulario.AplicarIdiomaActual();
            formulario.LastAction = AgregarProductoEventHandler;
        }

        private void ModificarProductoEventHandler(object sender, EventArgs e)
        {
            flexibilizador.EliminarControlesAdicionalesProducto();

            var dgvPosition = new Point(formulario.GetPanelMain.Width / 16, formulario.GetPanelMain.Height / 8);
            var listSize = new Size(formulario.GetPanelMain.Width / 2 - formulario.GetPanelMain.Width / 16, formulario.GetPanelMain.Height / 2 + formulario.GetPanelMain.Height / 4);
            flexibilizador.dataGridViewWithSelectionChanged(GetProductos(e), dgvPosition, listSize);

            var pointInicial = new Point(formulario.GetPanelMain.Width / 2 + 20, formulario.GetPanelMain.Height / 16);
            var offset = formulario.GetPanelMain.Height / 10;

            var txtId = FlexibilizadorFormularios.CreateTextBox(formulario.GetPanelMain, "Id", new Point(pointInicial.X, pointInicial.Y));
            txtId.Visible = false;
            var lblId = formulario.GetPanelMain.Controls.Find("lblId", true).FirstOrDefault();
            if (lblId != null)
                lblId.Visible = false;

            flexibilizador.TextBoxCreator("Nombre", new Point(pointInicial.X, pointInicial.Y + offset));
            flexibilizador.TextBoxCreator("Marca", new Point(pointInicial.X, pointInicial.Y + offset * 2));
            flexibilizador.TextBoxCreator("Modelo", new Point(pointInicial.X, pointInicial.Y + offset * 3));
            flexibilizador.TextBoxCreator("PrecioActual", new Point(pointInicial.X, pointInicial.Y + offset * 4));
            flexibilizador.TextBoxCreator("Stock", new Point(pointInicial.X, pointInicial.Y + offset * 5));
            flexibilizador.CheckboxCreator("AceptaMayorista", new Point(pointInicial.X, pointInicial.Y + offset * 6));
            flexibilizador.CheckboxCreator("AceptaMinorista", new Point(pointInicial.X, pointInicial.Y + offset * 7));
            flexibilizador.ModificarProductoButtonCreator(new Point(pointInicial.X, pointInicial.Y + offset * 8));

            formulario.AplicarIdiomaActual();
            formulario.LastAction = ModificarProductoEventHandler;
        }

        private void EliminarProductoEventHandler(object sender, EventArgs e)
        {
            flexibilizador.EliminarControlesAdicionalesProducto();

            var dgvPosition = new Point(formulario.GetPanelMain.Width / 16, formulario.GetPanelMain.Height / 16);
            var listSize = new Size(formulario.GetPanelMain.Width / 2 - formulario.GetPanelMain.Width / 16, formulario.GetPanelMain.Height - formulario.GetPanelMain.Height / 6);
            var txtPosition = new Point(formulario.GetPanelMain.Width / 2 + formulario.GetPanelMain.Width / 6, formulario.GetPanelMain.Height / 4);
            var buttonPosition = new Point(txtPosition.X, txtPosition.Y + 50);

            flexibilizador.dataGridViewWithSelectionChanged(GetProductos(e), dgvPosition, listSize);
            flexibilizador.TextBoxCreator("Nombre", txtPosition);

            var txtId = FlexibilizadorFormularios.CreateTextBox(formulario.GetPanelMain, "Id", new Point(txtPosition.X, txtPosition.Y - 60));
            txtId.Visible = false;
            var lblId = formulario.GetPanelMain.Controls.Find("lblId", true).FirstOrDefault();
            if (lblId != null)
                lblId.Visible = false;

            flexibilizador.EliminarProductoButtonCreator(dgvPosition, listSize, txtPosition, buttonPosition);
            formulario.AplicarIdiomaActual();
            formulario.LastAction = EliminarProductoEventHandler;
        }

        private void VerTodosProductosEventHandler(object sender, EventArgs e)
        {
            flexibilizador.EliminarControlesAdicionalesProducto();

            var position = new Point(formulario.GetPanelMain.Width / 8, formulario.GetPanelMain.Height / 8);
            var size = new Size(formulario.GetPanelMain.Width / 2 + formulario.GetPanelMain.Width / 4, formulario.GetPanelMain.Height / 2 + formulario.GetPanelMain.Height / 4);
            flexibilizador.DataGridViewProductoCreator(GetProductos(e), position, size);
            formulario.AplicarIdiomaActual();
            formulario.LastAction = VerTodosProductosEventHandler;
        }

        private List<Producto> GetProductos(EventArgs e)
        {
            if (!formulario.IsResizing(e))
            {
                try
                {
                    flexibilizador.Productos = ServicesFactory.CreateProductoServices().BuscarProductos(null);
                }
                catch (Exception)
                {
                    flexibilizador.Productos = new List<Producto>();
                }
            }
            return (flexibilizador.Productos != null && flexibilizador.Productos.Count > 0) ? flexibilizador.Productos : new List<Producto>();
        }
    }
}
