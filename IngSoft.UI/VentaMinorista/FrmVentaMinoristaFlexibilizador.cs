using IngSoft.ApplicationServices;
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
    internal class FrmVentaMinoristaFlexibilizador
    {
        private readonly FrmPrincipal form = SingleInstancesManager.Instance.ObtenerInstancia<FrmPrincipal>();
        private readonly ICarritoServices _carritoServices = ServicesFactory.CreateCarritoMinoristaServices();
        private readonly IProductoServices _productoServices = ServicesFactory.CreateProductoServices();
        private List<Producto> _productos;

        public List<Producto> Productos { get => _productos; set => _productos = value; }

        internal DataGridView DataGridViewProductosCreator(List<Producto> productos, Point position, Size size)
        {
            var columnDefinitions = new System.Collections.Generic.Dictionary<string, Type>
            {
                { "Id", typeof(Guid) },
                { "Nombre", typeof(string) },
                { "Marca", typeof(string) },
                { "Modelo", typeof(string) },
                { "PrecioActual", typeof(float) },
                { "Stock", typeof(int) }
            };

            var dgv = FlexibilizadorFormularios.CreateDataGridView<Producto>(form.GetPanelMain, "dataGridViewProductosMinorista", position, size, columnDefinitions, productos);
            if (dgv.Columns["Id"] != null)
                dgv.Columns["Id"].Visible = false;
            return dgv;
        }

        internal DataGridView DataGridViewProductosConSeleccion(List<Producto> productos, Point position, Size size)
        {
            var dgv = DataGridViewProductosCreator(productos, position, size);
            dgv.SelectionChanged += DataGridViewProductos_SelectionChanged;
            if (dgv.Rows.Count > 0)
                dgv.Rows[0].Selected = false;
            return dgv;
        }

        private void DataGridViewProductos_SelectionChanged(object sender, EventArgs e)
        {
            if (sender is DataGridView dgv && dgv.CurrentRow?.DataBoundItem is System.Data.DataRowView drv)
            {
                var row = drv.Row;
                foreach (System.Data.DataColumn col in row.Table.Columns)
                {
                    string txtName = $"txt{col.ColumnName}";
                    var controls = form.GetPanelMain.Controls.Find(txtName, true);
                    if (controls.Length > 0 && controls[0] is TextBox tb)
                        tb.Text = row[col].ToString();
                }
            }
        }

        internal void TextBoxCantidadCreator(Point position)
        {
            FlexibilizadorFormularios.CreateTextBox(form.GetPanelMain, "Cantidad", position);
        }

        internal void AgregarItemButtonCreator(Point position)
        {
            FlexibilizadorFormularios.CreateButton(form.GetPanelMain, "btnAgregarItem", position, new Size(200, 30), "Agregar Item", btnAgregarItem_Click);
        }

        internal void FinalizarCarritoButtonCreator(Point position)
        {
            FlexibilizadorFormularios.CreateButton(form.GetPanelMain, "btnFinalizarCarrito", position, new Size(200, 30), "Finalizar Carrito", btnFinalizarCarrito_Click);
        }

        private void btnAgregarItem_Click(object sender, EventArgs e)
        {
            try
            {
                var productoId = ObtenerGuid("txtId");
                var cantidad = ObtenerInt("txtCantidad");

                if (productoId == Guid.Empty || cantidad <= 0)
                    throw new ArgumentException("Datos inválidos: seleccione un producto e ingrese una cantidad mayor a 0.");

                var item = new CarritoItem
                {
                    Producto = new Producto { Id = productoId },
                    Cantidad = cantidad
                };

                _carritoServices.AgregarCantidadesItem(item);
                MessageBox.Show("Item agregado al carrito correctamente.");
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"No puede agregar este producto: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar item: {ex.Message}");
            }
        }

        private void btnFinalizarCarrito_Click(object sender, EventArgs e)
        {
            try
            {
                _carritoServices.FinalizarCarrito();
                MessageBox.Show("Carro finalizado correctamente.");
                EliminarControlesAdicionalesVentaMinorista();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"No se pudo finalizar el carro: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al finalizar el carrito: {ex.Message}");
            }
        }

        internal void EliminarControlesAdicionalesVentaMinorista()
        {
            FlexibilizadorFormularios.EliminarControlesAdicionalesForm(form.GetPanelMain, form.ControlesSalvar());
        }

        private Guid ObtenerGuid(string nombreControl)
        {
            var control = form.GetPanelMain.Controls.Find(nombreControl, true).FirstOrDefault() as TextBox;
            return Guid.TryParse(control?.Text?.Trim(), out Guid valor) ? valor : Guid.Empty;
        }

        private int ObtenerInt(string nombreControl)
        {
            var control = form.GetPanelMain.Controls.Find(nombreControl, true).FirstOrDefault() as TextBox;
            return int.TryParse(control?.Text?.Trim(), out int valor) ? valor : 0;
        }
    }
}
