using IngSoft.ApplicationServices;
using IngSoft.ApplicationServices.Factory;
using IngSoft.Domain;
using IngSoft.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace IngSoft.UI
{
    internal class FrmVentaMinoristaFlexibilizador
    {
        private readonly FrmPrincipal _form = SingleInstancesManager.Instance.ObtenerInstancia<FrmPrincipal>();
        private readonly ICarritoServices _carritoServices = ServicesFactory.CreateCarritoMinoristaServices();

        private List<Producto> _productos;
        private Producto _productoSeleccionado;
        private readonly List<CarritoItem> _carritoLocal = new List<CarritoItem>();

        // ── Paneles principales ──────────────────────────────────────────────────
        private Panel _pnlLeft;
        private Panel _pnlRight;
        private Panel _pnlCard;

        // ── Controles del panel izquierdo ────────────────────────────────────────
        private TextBox _txtSearch;
        private DataGridView _dgvProductos;

        // ── Ficha del producto ───────────────────────────────────────────────────
        private Label _lblInfoNombre;
        private Label _lblInfoMarca;
        private Label _lblInfoModelo;
        private Label _lblInfoPrecio;
        private Label _lblInfoStock;

        // ── Panel derecho – controles de acción ──────────────────────────────────
        private Label _lblNudCantidad;
        private NumericUpDown _nudCantidad;
        private Button _btnAgregar;
        private Label _lblCarritoTitle;
        private DataGridView _dgvCarrito;
        private Label _lblTotal;
        private Button _btnFinalizar;

        // ── Resize ───────────────────────────────────────────────────────────────
        private EventHandler _onFormResize;

        // ── Constantes de layout (px, relativas al top del panel derecho) ─────────
        private const int CardH        = 136;
        private const int TopPad       = 8;
        private const int NudLabelRelY = TopPad + CardH + 12;          // 156
        private const int NudRelY      = NudLabelRelY + 22;             // 178
        private const int BtnAgrRelY   = NudRelY + 28 + 10;            // 216
        private const int CartLblRelY  = BtnAgrRelY + 36 + 10;         // 262
        private const int CartDgvRelY  = CartLblRelY + 24;             // 286
        private const int TotalFromBot = 68;  // lblTotal a panelH - TotalFromBot
        private const int BtnFinFromBot = 42; // btnFinalizar a panelH - BtnFinFromBot

        // ── Geometría (calcula desde el tamaño actual de pnlMain) ────────────────

        private (int topY, int panelH, int leftW, int rightW, int rightX) Geometry()
        {
            var pnl    = _form.GetPanelMain;
            int menuH  = _form.MainMenuStrip?.Bottom ?? 28;
            int topY   = menuH + 6;
            int margin = 12;
            int totalW = pnl.Width  - margin * 2;
            int panelH = pnl.Height - topY - 8;
            int leftW  = (int)(totalW * 0.57);
            int rightW = totalW - leftW - margin;
            int rightX = margin + leftW + margin;
            return (topY, panelH, leftW, rightW, rightX);
        }

        // ── Construcción inicial ─────────────────────────────────────────────────

        internal void ConstruirLayout(List<Producto> productos)
        {
            _productos = productos;
            _carritoLocal.Clear();
            _productoSeleccionado = null;

            var pnlMain = _form.GetPanelMain;
            var (topY, panelH, leftW, rightW, rightX) = Geometry();
            int margin = 12;

            // ── Panel izquierdo ───────────────────────────────────────────────────
            _pnlLeft = FlexibilizadorFormularios.CreatePanel(pnlMain, "pnlVentaLeft",
                new Point(margin, topY), new Size(leftW, panelH), Color.White);
            _pnlLeft.BorderStyle = BorderStyle.FixedSingle;

            FlexibilizadorFormularios.CreateDisplayLabel(_pnlLeft, "lblBuscarTitle",
                new Point(8, 10), new Size(62, 22), "Buscar:",
                new Font("Arial", 9, FontStyle.Bold));

            _txtSearch = new TextBox
            {
                Name     = "txtBuscarProducto",
                Location = new Point(74, 9),
                Size     = new Size(leftW - 90, 24),
                Font     = new Font("Arial", 10)
            };
            _pnlLeft.Controls.Add(_txtSearch);
            _txtSearch.TextChanged += (s, e) => FiltrarProductos(_txtSearch.Text);

            _dgvProductos = CrearDGVProductos(_pnlLeft,
                new Point(8, 42), new Size(leftW - 18, panelH - 56));
            _dgvProductos.SelectionChanged += DGVProductos_SelectionChanged;
            _dgvProductos.ClearSelection(); // Fix 1: no auto-seleccionar primer row

            // ── Panel derecho ─────────────────────────────────────────────────────
            _pnlRight = FlexibilizadorFormularios.CreatePanel(pnlMain, "pnlVentaRight",
                new Point(rightX, topY), new Size(rightW, panelH), Color.White);
            _pnlRight.BorderStyle = BorderStyle.FixedSingle;

            // Ficha del producto
            _pnlCard = FlexibilizadorFormularios.CreatePanel(_pnlRight, "pnlProductoCard",
                new Point(8, TopPad), new Size(rightW - 18, CardH),
                Color.FromArgb(245, 248, 252));
            _pnlCard.BorderStyle = BorderStyle.FixedSingle;
            ConstruirFilasCard(rightW - 18);

            // NumericUpDown (label en NudLabelRelY, control en NudRelY)
            _nudCantidad = FlexibilizadorFormularios.CreateNumericUpDown(_pnlRight, "Cantidad",
                new Point(8, NudRelY), new Size(rightW - 18, 28));
            _lblNudCantidad = _pnlRight.Controls.Find("lblCantidad", false).FirstOrDefault() as Label;

            // Botón Agregar
            _btnAgregar = FlexibilizadorFormularios.CreateButton(_pnlRight, "btnAgregarItem",
                new Point(8, BtnAgrRelY), new Size(rightW - 18, 36),
                "Agregar", BtnAgregarItem_Click);
            EstilizarBoton(_btnAgregar, Color.FromArgb(41, 128, 185));

            // Título del carrito
            _lblCarritoTitle = FlexibilizadorFormularios.CreateDisplayLabel(_pnlRight, "lblCarritoTitle",
                new Point(8, CartLblRelY), new Size(rightW - 18, 20),
                "Mi carrito", new Font("Arial", 9, FontStyle.Bold));

            // Grilla del carrito
            int cartDgvH = panelH - TotalFromBot - 4 - CartDgvRelY;
            _dgvCarrito = CrearDGVCarritoVacio(_pnlRight,
                new Point(8, CartDgvRelY), new Size(rightW - 18, cartDgvH));

            // Total
            _lblTotal = FlexibilizadorFormularios.CreateDisplayLabel(_pnlRight, "lblTotal",
                new Point(8, panelH - TotalFromBot), new Size(rightW - 18, 22),
                "Total:  $ 0,00",
                new Font("Arial", 10, FontStyle.Bold), ContentAlignment.MiddleRight);

            // Botón Finalizar
            _btnFinalizar = FlexibilizadorFormularios.CreateButton(_pnlRight, "btnFinalizarCarrito",
                new Point(8, panelH - BtnFinFromBot), new Size(rightW - 18, 34),
                "Confirmar compra", BtnFinalizarCarrito_Click);
            EstilizarBoton(_btnFinalizar, Color.FromArgb(39, 174, 96));

            // Fix 2: suscribir a Form.Resize (captura maximize/restore)
            _onFormResize = (s, e) => ResizeControls();
            _form.Resize += _onFormResize;
        }

        // ── Resize (Fix 2) ───────────────────────────────────────────────────────

        internal void ResizeControls()
        {
            if (_pnlLeft == null) return;

            var (topY, panelH, leftW, rightW, rightX) = Geometry();
            int margin = 12;

            // Panel izquierdo
            _pnlLeft.Location  = new Point(margin, topY);
            _pnlLeft.Size      = new Size(leftW, panelH);
            _txtSearch.Size    = new Size(leftW - 90, 24);
            _dgvProductos.Size = new Size(leftW - 18, panelH - 56);

            // Panel derecho
            _pnlRight.Location = new Point(rightX, topY);
            _pnlRight.Size     = new Size(rightW, panelH);

            // Ficha del producto
            int valW = rightW - 18 - 72 - 10;
            _pnlCard.Size        = new Size(rightW - 18, CardH);
            _lblInfoNombre.Width = valW;
            _lblInfoMarca.Width  = valW;
            _lblInfoModelo.Width = valW;
            _lblInfoPrecio.Width = valW;
            _lblInfoStock.Width  = valW;

            // NUD
            if (_lblNudCantidad != null) _lblNudCantidad.Width = rightW - 18;
            _nudCantidad.Width = rightW - 18;

            // Botones y controles del panel derecho
            _btnAgregar.Size      = new Size(rightW - 18, 36);
            _lblCarritoTitle.Width = rightW - 18;
            _dgvCarrito.Size      = new Size(rightW - 18, panelH - TotalFromBot - 4 - CartDgvRelY);
            _lblTotal.Location    = new Point(8, panelH - TotalFromBot);
            _lblTotal.Width       = rightW - 18;
            _btnFinalizar.Location = new Point(8, panelH - BtnFinFromBot);
            _btnFinalizar.Size    = new Size(rightW - 18, 34);
        }

        // ── Ficha del producto ───────────────────────────────────────────────────

        private void ConstruirFilasCard(int cardW)
        {
            int valX = 72;
            int valW = cardW - valX - 10;

            CrearFilaCard(_pnlCard, "lblInfoNombreTitle", "lblInfoNombre",   8, "Nombre:", valX, valW);
            CrearFilaCard(_pnlCard, "lblInfoMarcaTitle",  "lblInfoMarca",   34, "Marca:",  valX, valW);
            CrearFilaCard(_pnlCard, "lblInfoModeloTitle", "lblInfoModelo",  60, "Modelo:", valX, valW);
            CrearFilaCard(_pnlCard, "lblInfoPrecioTitle", "lblInfoPrecio",  86, "Precio:", valX, valW);
            CrearFilaCard(_pnlCard, "lblInfoStockTitle",  "lblInfoStock",  112, "Stock:",  valX, valW);

            _lblInfoNombre = (Label)_pnlCard.Controls.Find("lblInfoNombre", false)[0];
            _lblInfoMarca  = (Label)_pnlCard.Controls.Find("lblInfoMarca",  false)[0];
            _lblInfoModelo = (Label)_pnlCard.Controls.Find("lblInfoModelo", false)[0];
            _lblInfoPrecio = (Label)_pnlCard.Controls.Find("lblInfoPrecio", false)[0];
            _lblInfoStock  = (Label)_pnlCard.Controls.Find("lblInfoStock",  false)[0];

            _lblInfoPrecio.Font      = new Font("Arial", 9, FontStyle.Bold);
            _lblInfoPrecio.ForeColor = Color.FromArgb(41, 128, 185);
        }

        private static void CrearFilaCard(Panel card, string nameTitle, string nameValue,
            int y, string titulo, int valX, int valW)
        {
            FlexibilizadorFormularios.CreateDisplayLabel(card, nameTitle,
                new Point(8, y), new Size(62, 20), titulo,
                new Font("Arial", 8, FontStyle.Bold));
            FlexibilizadorFormularios.CreateDisplayLabel(card, nameValue,
                new Point(valX, y), new Size(valW, 20), "—", new Font("Arial", 9));
        }

        // ── Grillas ──────────────────────────────────────────────────────────────

        private DataGridView CrearDGVProductos(Panel parent, Point position, Size size)
        {
            var cols = new Dictionary<string, Type>
            {
                { "Id",           typeof(Guid)   },
                { "Nombre",       typeof(string) },
                { "Marca",        typeof(string) },
                { "Modelo",       typeof(string) },
                { "PrecioActual", typeof(float)  },
                { "Stock",        typeof(int)    }
            };
            var dgv = FlexibilizadorFormularios.CreateDataGridView<Producto>(
                parent, "dataGridViewProductosMinorista", position, size, cols, _productos);
            if (dgv.Columns["Id"] != null)
                dgv.Columns["Id"].Visible = false;
            return dgv;
        }

        private DataGridView CrearDGVCarritoVacio(Panel parent, Point position, Size size)
        {
            var dgv = FlexibilizadorFormularios.CreateDataGridView(
                parent, "dataGridViewCarritoMinorista", position, size,
                BuildCarritoDataTable(new List<CarritoItem>()));
            dgv.RowHeadersVisible = false;
            return dgv;
        }

        // ── DataTables ───────────────────────────────────────────────────────────

        private DataTable BuildProductosDataTable(List<Producto> productos)
        {
            var dt = new DataTable();
            dt.Columns.Add("Id",           typeof(Guid));
            dt.Columns.Add("Nombre",       typeof(string));
            dt.Columns.Add("Marca",        typeof(string));
            dt.Columns.Add("Modelo",       typeof(string));
            dt.Columns.Add("PrecioActual", typeof(float));
            dt.Columns.Add("Stock",        typeof(int));
            foreach (var p in productos)
                dt.Rows.Add(p.Id, p.Nombre, p.Marca, p.Modelo, p.PrecioActual, p.Stock);
            return dt;
        }

        private static DataTable BuildCarritoDataTable(List<CarritoItem> items)
        {
            var dt = new DataTable();
            dt.Columns.Add("Producto",  typeof(string));
            dt.Columns.Add("Cant.",     typeof(int));
            dt.Columns.Add("Precio",    typeof(float));
            dt.Columns.Add("Subtotal",  typeof(float));
            foreach (var item in items)
                dt.Rows.Add(item.Producto?.Nombre, item.Cantidad,
                            item.Precio, item.Cantidad * item.Precio);
            return dt;
        }

        // ── Filtro ───────────────────────────────────────────────────────────────

        private void FiltrarProductos(string filtro)
        {
            var lista = string.IsNullOrWhiteSpace(filtro)
                ? _productos
                : _productos.Where(p =>
                    p.Nombre?.IndexOf(filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    p.Marca?.IndexOf(filtro, StringComparison.OrdinalIgnoreCase)  >= 0 ||
                    p.Modelo?.IndexOf(filtro, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

            _dgvProductos.DataSource = BuildProductosDataTable(lista);
            if (_dgvProductos.Columns["Id"] != null)
                _dgvProductos.Columns["Id"].Visible = false;
            _dgvProductos.ClearSelection(); // Fix 1: no auto-seleccionar al filtrar
        }

        // ── Selección ────────────────────────────────────────────────────────────

        private void DGVProductos_SelectionChanged(object sender, EventArgs e)
        {
            // Fix 1: si se limpió la selección, resetear la ficha
            if (_dgvProductos.SelectedRows.Count == 0)
            {
                ResetCard();
                return;
            }

            var row = _dgvProductos.SelectedRows[0];
            if (!(row.DataBoundItem is DataRowView drv)) return;
            if (!(drv.Row["Id"] is Guid id)) return;

            _productoSeleccionado = _productos?.FirstOrDefault(p => p.Id == id);
            if (_productoSeleccionado == null) return;

            _lblInfoNombre.Text = _productoSeleccionado.Nombre ?? "—";
            _lblInfoMarca.Text  = _productoSeleccionado.Marca  ?? "—";
            _lblInfoModelo.Text = _productoSeleccionado.Modelo ?? "—";
            _lblInfoPrecio.Text = $"$ {_productoSeleccionado.PrecioActual:F2}";
            _lblInfoStock.Text  = _productoSeleccionado.Stock.ToString();
        }

        private void ResetCard()
        {
            _productoSeleccionado = null;
            if (_lblInfoNombre != null) _lblInfoNombre.Text = "—";
            if (_lblInfoMarca  != null) _lblInfoMarca.Text  = "—";
            if (_lblInfoModelo != null) _lblInfoModelo.Text = "—";
            if (_lblInfoPrecio != null) _lblInfoPrecio.Text = "—";
            if (_lblInfoStock  != null) _lblInfoStock.Text  = "—";
        }

        // ── Acciones ─────────────────────────────────────────────────────────────

        private void BtnAgregarItem_Click(object sender, EventArgs e)
        {
            if (_productoSeleccionado == null)
            {
                MessageBox.Show("Seleccione un producto de la lista.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int cantidad = (int)_nudCantidad.Value;
            var item = new CarritoItem
            {
                Producto = _productoSeleccionado,
                Cantidad = cantidad,
                Precio   = _productoSeleccionado.PrecioActual
            };

            try
            {
                _carritoServices.AgregarCantidadesItem(item);

                var existente = _carritoLocal
                    .FirstOrDefault(i => i.Producto?.Id == _productoSeleccionado.Id);
                if (existente != null)
                    existente.Cantidad += cantidad;
                else
                    _carritoLocal.Add(new CarritoItem
                    {
                        Producto = _productoSeleccionado,
                        Cantidad = cantidad,
                        Precio   = _productoSeleccionado.PrecioActual
                    });

                ActualizarCarritoDGV();
                _nudCantidad.Value = 1;
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"No se puede agregar el producto: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar item: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ActualizarCarritoDGV()
        {
            _dgvCarrito.DataSource = BuildCarritoDataTable(_carritoLocal);
            float total = _carritoLocal.Sum(i => i.Cantidad * i.Precio);
            _lblTotal.Text = $"Total:  $ {total:F2}";
        }

        private void BtnFinalizarCarrito_Click(object sender, EventArgs e)
        {
            if (_carritoLocal.Count == 0)
            {
                MessageBox.Show("El carrito está vacío.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            float total   = _carritoLocal.Sum(i => i.Cantidad * i.Precio);
            var confirm   = MessageBox.Show(
                $"¿Confirmar compra por $ {total:F2}?",
                "Confirmar compra", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                _carritoServices.FinalizarCarrito();
                MessageBox.Show("Compra realizada correctamente.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                EliminarControlesAdicionalesVentaMinorista();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"No se pudo procesar la compra: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al procesar la compra: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Limpieza ─────────────────────────────────────────────────────────────

        internal void EliminarControlesAdicionalesVentaMinorista()
        {
            // Fix 2: desuscribir antes de destruir controles (evita handlers fantasma)
            if (_onFormResize != null)
            {
                _form.Resize -= _onFormResize;
                _onFormResize = null;
            }

            FlexibilizadorFormularios.EliminarControlesAdicionalesForm(
                _form.GetPanelMain, _form.ControlesSalvar());

            _pnlLeft = null;
            _pnlRight = null;
        }

        // ── Helpers UI ───────────────────────────────────────────────────────────

        private static void EstilizarBoton(Button btn, Color color)
        {
            btn.BackColor = color;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font      = new Font("Arial", 10, FontStyle.Bold);
        }
    }
}
