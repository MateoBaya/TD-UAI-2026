using IngSoft.ApplicationServices.Factory;
using IngSoft.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using IngSoft.ApplicationServices;
using IngSoft.Services;

namespace IngSoft.UI
{
    internal class FrmProductoFlexibilizador
    {
        private readonly FrmPrincipal form = SingleInstancesManager.Instance.ObtenerInstancia<FrmPrincipal>();
        private readonly IProductoServices _productoServices = ServicesFactory.CreateProductoServices();
        private List<Producto> _productos;

        public List<Producto> Productos { get => _productos; set => _productos = value; }

        internal void TextBoxCreator(string param, Point position)
        {
            FlexibilizadorFormularios.CreateTextBox(form.GetPanelMain, param, position);
        }

        internal void CheckboxCreator(string param, Point position)
        {
            FlexibilizadorFormularios.CreateCheckBox(form.GetPanelMain, param, position);
        }

        internal void GuardarProductoButtonCreator()
        {
            var position = new Point((form.GetPanelMain.Width / 2) - 100, (form.GetPanelMain.Height / 2 + form.GetPanelMain.Height / 4));
            FlexibilizadorFormularios.CreateButton(form.GetPanelMain, "btnGuardarProducto", position, new Size(200, 30), "Guardar Producto", btnGuardar_Click);
        }

        internal void ModificarProductoButtonCreator(Point position)
        {
            FlexibilizadorFormularios.CreateButton(form.GetPanelMain, "btnModificarProducto", position, new Size(200, 30), "Modificar Producto", btnModificar_Click);
        }

        internal void EliminarProductoButtonCreator(Point listPos, Size listSize, Point txtPos, Point btnPos)
        {
            FlexibilizadorFormularios.CreateButton(form.GetPanelMain, "btnEliminarProducto", btnPos, new Size(200, 30), "Eliminar Producto", btnEliminar_Click);
        }

        internal DataGridView DataGridViewProductoCreator(List<Producto> productos, Point position, Size size)
        {
            Dictionary<string, Type> columnDefinitions = new Dictionary<string, Type>
            {
                { "Id", typeof(Guid) },
                { "Nombre", typeof(string) },
                { "Marca", typeof(string) },
                { "Modelo", typeof(string) },
                { "PrecioActual", typeof(float) },
                { "Stock", typeof(int) },
                { "AceptaMayorista", typeof(bool) },
                { "AceptaMinorista", typeof(bool) }
            };

            DataGridView dgv = FlexibilizadorFormularios.CreateDataGridView<Producto>(form.GetPanelMain, "dataGridViewProductos", position, size, columnDefinitions, productos);
            if (dgv.Columns["Id"] != null)
                dgv.Columns["Id"].Visible = false;
            return dgv;
        }

        public DataGridView dataGridViewWithSelectionChanged(List<Producto> productos, Point position, Size size)
        {
            DataGridView dgv = DataGridViewProductoCreator(productos, position, size);
            dgv.SelectionChanged += DataGridViewProducto_SelectionChanged;
            if (dgv.Rows.Count > 0)
                dgv.Rows[0].Selected = false;
            return dgv;
        }

        private void DataGridViewProducto_SelectionChanged(object sender, EventArgs e)
        {
            if (sender is DataGridView dgv)
            {
                FillControlsFromDataRowSelectedEvent(dgv, "txt");
            }
        }

        private void FillControlsFromDataRowSelectedEvent(DataGridView dgv, string textBoxPrefix)
        {
            if (dgv.CurrentRow == null)
                return;

            if (dgv.CurrentRow.DataBoundItem is DataRowView dataRowView)
            {
                DataRow row = dataRowView.Row;
                foreach (DataColumn column in row.Table.Columns)
                {
                    if (column.DataType == typeof(bool))
                    {
                        string checkBoxName = $"chk{column.ColumnName}";
                        Control[] controls = form.GetPanelMain.Controls.Find(checkBoxName, true);
                        if (controls.Length > 0 && controls[0] is CheckBox checkBox)
                        {
                            checkBox.Checked = Convert.ToBoolean(row[column]);
                        }
                    }
                    else
                    {
                        string textBoxName = $"{textBoxPrefix}{column.ColumnName}";
                        Control[] controls = form.GetPanelMain.Controls.Find(textBoxName, true);
                        if (controls.Length > 0 && controls[0] is TextBox textBox)
                        {
                            textBox.Text = row[column].ToString();
                        }
                    }
                }
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                var producto = new Producto
                {
                    Nombre = ObtenerTexto("txtNombre"),
                    Marca = ObtenerTexto("txtMarca"),
                    Modelo = ObtenerTexto("txtModelo"),
                    PrecioActual = ObtenerFloat("txtPrecioActual"),
                    Stock = ObtenerInt("txtStock"),
                    AceptaMayorista = ObtenerCheck("chkAceptaMayorista"),
                    AceptaMinorista = ObtenerCheck("chkAceptaMinorista")
                };

                _productoServices.CrearProducto(producto);
                MessageBox.Show("Producto guardado con éxito.");
                EliminarControlesAdicionalesProducto();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el producto: {ex.Message}");
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            try
            {
                var id = ObtenerGuid("txtId");
                var producto = new Producto
                {
                    Id = id,
                    Nombre = ObtenerTexto("txtNombre"),
                    Marca = ObtenerTexto("txtMarca"),
                    Modelo = ObtenerTexto("txtModelo"),
                    PrecioActual = ObtenerFloat("txtPrecioActual"),
                    Stock = ObtenerInt("txtStock"),
                    AceptaMayorista = ObtenerCheck("chkAceptaMayorista"),
                    AceptaMinorista = ObtenerCheck("chkAceptaMinorista")
                };

                _productoServices.ModificarProducto(producto);
                MessageBox.Show("Producto modificado con éxito.");
                EliminarControlesAdicionalesProducto();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al modificar el producto: {ex.Message}");
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                var producto = new Producto
                {
                    Id = ObtenerGuid("txtId"),
                    Nombre = ObtenerTexto("txtNombre")
                };

                _productoServices.EliminarProducto(producto);
                MessageBox.Show("Producto eliminado con éxito.");
                EliminarControlesAdicionalesProducto();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar el producto: {ex.Message}");
            }
        }

        internal void EliminarControlesAdicionalesProducto()
        {
            FlexibilizadorFormularios.EliminarControlesAdicionalesForm(form.GetPanelMain, form.ControlesSalvar());
        }

        private string ObtenerTexto(string nombreControl)
        {
            var control = form.GetPanelMain.Controls.Find(nombreControl, true).FirstOrDefault() as TextBox;
            return control?.Text?.Trim() ?? string.Empty;
        }

        private bool ObtenerCheck(string nombreControl)
        {
            var control = form.GetPanelMain.Controls.Find(nombreControl, true).FirstOrDefault() as CheckBox;
            return control != null && control.Checked;
        }

        private float ObtenerFloat(string nombreControl)
        {
            var texto = ObtenerTexto(nombreControl);
            return float.TryParse(texto, out float valor) ? valor : 0f;
        }

        private int ObtenerInt(string nombreControl)
        {
            var texto = ObtenerTexto(nombreControl);
            return int.TryParse(texto, out int valor) ? valor : 0;
        }

        private Guid ObtenerGuid(string nombreControl)
        {
            var texto = ObtenerTexto(nombreControl);
            return Guid.TryParse(texto, out Guid valor) ? valor : Guid.Empty;
        }
    }
}
