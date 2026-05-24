using IngSoft.ApplicationServices;
using IngSoft.ApplicationServices.Factory;
using IngSoft.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using IngSoft.Services;

namespace IngSoft.UI
{
    internal class FrmAprobacionCarritoFlexibilizador
    {
        private readonly FrmPrincipal _form = SingleInstancesManager.Instance.ObtenerInstancia<FrmPrincipal>();
        private readonly ICarritoServices _carritoServices = ServicesFactory.CreateCarritoMinoristaServices();

        private List<Carrito> _carritos = new List<Carrito>();
        private List<CarritoItem> _detalle = new List<CarritoItem>();
        private Carrito _carritoSeleccionado;

        // ── Paneles ──────────────────────────────────────────────────────────────
        private Panel _pnlLeft;
        private Panel _pnlRight;

        // ── Controles panel izquierdo ────────────────────────────────────────────
        private DataGridView _dgvCarritos;

        // ── Controles panel derecho ──────────────────────────────────────────────
        private Label _lblInfoNroCarrito;
        private Label _lblInfoFecha;
        private DataGridView _dgvDetalle;
        private Label _lblTotal;
        private Label _lblFechaEntrega;
        private DateTimePicker _dtpFechaEntrega;
        private Button _btnAceptar;

        // ── Resize ───────────────────────────────────────────────────────────────
        private EventHandler _onFormResize;

        // ── Constantes de layout ──────────────────────────────────────────────────
        private const int CardH           = 56;
        private const int TopPad          = 8;
        private const int CartDgvRelY     = TopPad + CardH + 32;
        private const int TotalFromBot    = 126;   // lblTotal
        private const int FechaLblFromBot = 100;   // lblFechaEntrega
        private const int FechaFromBot    = 76;    // dtpFechaEntrega
        private const int BtnFromBot      = 42;    // btnAceptar

        // ── Geometría ────────────────────────────────────────────────────────────

        private (int topY, int panelH, int leftW, int rightW, int rightX) Geometry()
        {
            var pnl   = _form.GetPanelMain;
            int menuH = _form.MainMenuStrip?.Bottom ?? 28;
            int topY  = menuH + 6;
            int margin = 12;
            int totalW = pnl.Width - margin * 2;
            int panelH = pnl.Height - topY - 8;
            int leftW  = (int)(totalW * 0.50);
            int rightW = totalW - leftW - margin;
            int rightX = margin + leftW + margin;
            return (topY, panelH, leftW, rightW, rightX);
        }

        // ── Construcción inicial ─────────────────────────────────────────────────

        internal void ConstruirLayout()
        {
            _carritoSeleccionado = null;
            _detalle.Clear();

            CargarCarritos();

            var pnlMain = _form.GetPanelMain;
            var (topY, panelH, leftW, rightW, rightX) = Geometry();
            int margin = 12;

            // ── Panel izquierdo ───────────────────────────────────────────────────
            _pnlLeft = FlexibilizadorFormularios.CreatePanel(pnlMain, "pnlAprobLeft",
                new Point(margin, topY), new Size(leftW, panelH), Color.White);
            _pnlLeft.BorderStyle = BorderStyle.FixedSingle;

            FlexibilizadorFormularios.CreateDisplayLabel(_pnlLeft, "lblCarritosTitle",
                new Point(8, 10), new Size(leftW - 16, 22), "Carritos pendientes de aprobacion",
                new Font("Arial", 10, FontStyle.Bold));

            _dgvCarritos = CrearDGVCarritos(_pnlLeft,
                new Point(8, 40), new Size(leftW - 18, panelH - 54));
            _dgvCarritos.SelectionChanged += DGVCarritos_SelectionChanged;
            _dgvCarritos.ClearSelection();

            // ── Panel derecho ─────────────────────────────────────────────────────
            _pnlRight = FlexibilizadorFormularios.CreatePanel(pnlMain, "pnlAprobRight",
                new Point(rightX, topY), new Size(rightW, panelH), Color.White);
            _pnlRight.BorderStyle = BorderStyle.FixedSingle;

            FlexibilizadorFormularios.CreateDisplayLabel(_pnlRight, "lblDetalleTitle",
                new Point(8, 10), new Size(rightW - 16, 22), "Detalle del carrito",
                new Font("Arial", 10, FontStyle.Bold));

            // Ficha del carrito seleccionado
            var pnlCard = FlexibilizadorFormularios.CreatePanel(_pnlRight, "pnlCarritoCard",
                new Point(8, TopPad + 28), new Size(rightW - 18, CardH),
                Color.FromArgb(245, 248, 252));
            pnlCard.BorderStyle = BorderStyle.FixedSingle;
            ConstruirFilasCard(pnlCard, rightW - 18);

            FlexibilizadorFormularios.CreateDisplayLabel(_pnlRight, "lblItemsTitle",
                new Point(8, TopPad + 28 + CardH + 8), new Size(rightW - 16, 20),
                "Items:", new Font("Arial", 9, FontStyle.Bold));

            int detDgvH = panelH - TotalFromBot - 4 - CartDgvRelY;
            _dgvDetalle = CrearDGVDetalleVacio(_pnlRight,
                new Point(8, CartDgvRelY), new Size(rightW - 18, detDgvH));

            _lblTotal = FlexibilizadorFormularios.CreateDisplayLabel(_pnlRight, "lblAprobTotal",
                new Point(8, panelH - TotalFromBot), new Size(rightW - 18, 22),
                "Total:  $ 0,00",
                new Font("Arial", 10, FontStyle.Bold), ContentAlignment.MiddleRight);

            _lblFechaEntrega = FlexibilizadorFormularios.CreateDisplayLabel(_pnlRight, "lblFechaEntrega",
                new Point(8, panelH - FechaLblFromBot), new Size(rightW - 18, 20),
                "Fecha de entrega:", new Font("Arial", 9, FontStyle.Bold));

            _dtpFechaEntrega = new DateTimePicker
            {
                Name     = "dtpFechaEntrega",
                Location = new Point(8, panelH - FechaFromBot),
                Size     = new Size(rightW - 18, 28),
                Format   = DateTimePickerFormat.Short,
                Value    = DateTime.Today.AddDays(7),
                MinDate  = DateTime.Today,
                Enabled  = false
            };
            _pnlRight.Controls.Add(_dtpFechaEntrega);

            _btnAceptar = FlexibilizadorFormularios.CreateButton(_pnlRight, "btnAceptarCarrito",
                new Point(8, panelH - BtnFromBot), new Size(rightW - 18, 34),
                "Aceptar carrito",
                BtnAceptarCarrito_Click);
            EstilizarBoton(_btnAceptar, Color.FromArgb(39, 174, 96));
            _btnAceptar.Enabled = false;

            _onFormResize = (s, e) => ResizeControls();
            _form.Resize += _onFormResize;
        }

        // ── Resize ───────────────────────────────────────────────────────────────

        internal void ResizeControls()
        {
            if (_pnlLeft == null) return;

            var (topY, panelH, leftW, rightW, rightX) = Geometry();
            int margin = 12;

            _pnlLeft.Location  = new Point(margin, topY);
            _pnlLeft.Size      = new Size(leftW, panelH);

            var lblTitLeft = _pnlLeft.Controls.Find("lblCarritosTitle", false).FirstOrDefault() as Label;
            if (lblTitLeft != null) lblTitLeft.Size = new Size(leftW - 16, 22);

            _dgvCarritos.Size = new Size(leftW - 18, panelH - 54);

            _pnlRight.Location = new Point(rightX, topY);
            _pnlRight.Size     = new Size(rightW, panelH);

            var pnlCard = _pnlRight.Controls.Find("pnlCarritoCard", false).FirstOrDefault() as Panel;
            if (pnlCard != null)
            {
                pnlCard.Size = new Size(rightW - 18, CardH);
                if (_lblInfoNroCarrito != null) _lblInfoNroCarrito.Width = rightW - 18 - 90 - 10;
                if (_lblInfoFecha     != null) _lblInfoFecha.Width     = rightW - 18 - 90 - 10;
            }

            if (_dgvDetalle != null)
                _dgvDetalle.Size = new Size(rightW - 18, panelH - TotalFromBot - 4 - CartDgvRelY);

            if (_lblTotal != null)
            {
                _lblTotal.Location = new Point(8, panelH - TotalFromBot);
                _lblTotal.Width    = rightW - 18;
            }

            if (_lblFechaEntrega != null)
            {
                _lblFechaEntrega.Location = new Point(8, panelH - FechaLblFromBot);
                _lblFechaEntrega.Width    = rightW - 18;
            }

            if (_dtpFechaEntrega != null)
            {
                _dtpFechaEntrega.Location = new Point(8, panelH - FechaFromBot);
                _dtpFechaEntrega.Width    = rightW - 18;
            }

            if (_btnAceptar != null)
            {
                _btnAceptar.Location = new Point(8, panelH - BtnFromBot);
                _btnAceptar.Size     = new Size(rightW - 18, 34);
            }
        }

        // ── Ficha del carrito ────────────────────────────────────────────────────

        private void ConstruirFilasCard(Panel card, int cardW)
        {
            int valX = 90;
            int valW = cardW - valX - 10;

            CrearFilaCard(card, "lblInfoNroCarritoTitle", "lblInfoNroCarrito",  8, "Nro. Carrito:", valX, valW);
            CrearFilaCard(card, "lblInfoFechaTitle",      "lblInfoFecha",      30, "Fecha:",        valX, valW);

            _lblInfoNroCarrito = (Label)card.Controls.Find("lblInfoNroCarrito", false)[0];
            _lblInfoFecha      = (Label)card.Controls.Find("lblInfoFecha",      false)[0];
        }

        private static void CrearFilaCard(Panel card, string nameTitle, string nameValue,
            int y, string titulo, int valX, int valW)
        {
            FlexibilizadorFormularios.CreateDisplayLabel(card, nameTitle,
                new Point(8, y), new Size(80, 20), titulo,
                new Font("Arial", 8, FontStyle.Bold));
            FlexibilizadorFormularios.CreateDisplayLabel(card, nameValue,
                new Point(valX, y), new Size(valW, 20), "—", new Font("Arial", 9));
        }

        // ── Grillas ──────────────────────────────────────────────────────────────

        private DataGridView CrearDGVCarritos(Panel parent, Point position, Size size)
        {
            var dgv = FlexibilizadorFormularios.CreateDataGridView(
                parent, "dgvCarritosAprobacion", position, size,
                BuildCarritosDataTable(_carritos));
            if (dgv.Columns["Id"] != null)
                dgv.Columns["Id"].Visible = false;
            return dgv;
        }

        private DataGridView CrearDGVDetalleVacio(Panel parent, Point position, Size size)
        {
            var dgv = FlexibilizadorFormularios.CreateDataGridView(
                parent, "dgvDetalleAprobacion", position, size,
                BuildDetalleDataTable(new List<CarritoItem>()));
            dgv.RowHeadersVisible = false;
            return dgv;
        }

        // ── DataTables ───────────────────────────────────────────────────────────

        private static DataTable BuildCarritosDataTable(List<Carrito> carritos)
        {
            var dt = new DataTable();
            dt.Columns.Add("Id",          typeof(Guid));
            dt.Columns.Add("Nro.",        typeof(int));
            dt.Columns.Add("Fecha",       typeof(DateTime));
            foreach (var c in carritos)
                dt.Rows.Add(c.Id, c.NroCarrito, c.FechaInsert);
            return dt;
        }

        private static DataTable BuildDetalleDataTable(List<CarritoItem> items)
        {
            var dt = new DataTable();
            dt.Columns.Add("Cantidad", typeof(int));
            dt.Columns.Add("Precio",   typeof(float));
            dt.Columns.Add("Subtotal", typeof(float));
            foreach (var item in items)
                dt.Rows.Add(item.Cantidad, item.Precio, item.Cantidad * item.Precio);
            return dt;
        }

        // ── Carga de datos ───────────────────────────────────────────────────────

        private void CargarCarritos()
        {
            try
            {
                _carritos = _carritoServices.ObtenerCarritosPendientes() ?? new List<Carrito>();
            }
            catch (Exception)
            {
                _carritos = new List<Carrito>();
            }
        }

        private void CargarDetalle()
        {
            try
            {
                _detalle = _carritoServices.MostrarDetalleCarrito(_carritoSeleccionado.Id) ?? new List<CarritoItem>();
            }
            catch (Exception)
            {
                _detalle = new List<CarritoItem>();
            }

            _dgvDetalle.DataSource = BuildDetalleDataTable(_detalle);
            float total = _detalle.Sum(i => i.Cantidad * i.Precio);
            _lblTotal.Text = $"Total:  $ {total:F2}";
        }

        private void ResetDetalle()
        {
            _detalle.Clear();
            _dgvDetalle.DataSource = BuildDetalleDataTable(_detalle);
            _lblTotal.Text = "Total:  $ 0,00";
            if (_lblInfoNroCarrito != null) _lblInfoNroCarrito.Text = "—";
            if (_lblInfoFecha     != null) _lblInfoFecha.Text     = "—";
            if (_dtpFechaEntrega  != null) { _dtpFechaEntrega.Value = DateTime.Today.AddDays(7); _dtpFechaEntrega.Enabled = false; }
            _btnAceptar.Enabled = false;
        }

        // ── Seleccion ────────────────────────────────────────────────────────────

        private void DGVCarritos_SelectionChanged(object sender, EventArgs e)
        {
            if (_dgvCarritos.SelectedRows.Count == 0)
            {
                _carritoSeleccionado = null;
                ResetDetalle();
                return;
            }

            var row = _dgvCarritos.SelectedRows[0];
            if (!(row.DataBoundItem is DataRowView drv)) return;
            if (!(drv.Row["Id"] is Guid id)) return;

            _carritoSeleccionado = _carritos.FirstOrDefault(c => c.Id == id);
            if (_carritoSeleccionado == null) return;

            if (_lblInfoNroCarrito != null) _lblInfoNroCarrito.Text = _carritoSeleccionado.NroCarrito.ToString();
            if (_lblInfoFecha     != null) _lblInfoFecha.Text      = _carritoSeleccionado.FechaInsert.ToString("dd/MM/yyyy HH:mm");

            CargarDetalle();
            if (_dtpFechaEntrega != null) _dtpFechaEntrega.Enabled = true;
            _btnAceptar.Enabled = true;
        }

        // ── Aceptar ──────────────────────────────────────────────────────────────

        private void BtnAceptarCarrito_Click(object sender, EventArgs e)
        {
            if (_carritoSeleccionado == null)
            {
                MessageBox.Show("Seleccione un carrito de la lista.", "Atencion",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var fechaEntrega = _dtpFechaEntrega?.Value ?? DateTime.Today.AddDays(7);

            var confirm = MessageBox.Show(
                $"¿Aceptar el carrito Nro. {_carritoSeleccionado.NroCarrito}? Esta accion confirma que el pago fue recibido.\nFecha de entrega: {fechaEntrega:dd/MM/yyyy}",
                "Confirmar aceptacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                var resultado = _carritoServices.AprobarCarrito(_carritoSeleccionado.Id, fechaEntrega);
                if (resultado)
                {
                    MessageBox.Show("Carrito aceptado correctamente.", "Aprobacion",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _carritoSeleccionado = null;
                    CargarCarritos();
                    _dgvCarritos.DataSource = BuildCarritosDataTable(_carritos);
                    if (_dgvCarritos.Columns["Id"] != null)
                        _dgvCarritos.Columns["Id"].Visible = false;
                    _dgvCarritos.ClearSelection();
                    ResetDetalle();
                }
                else
                {
                    MessageBox.Show("No se pudo aceptar el carrito.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al aceptar el carrito: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Limpieza ─────────────────────────────────────────────────────────────

        internal void EliminarControlesAdicionales()
        {
            if (_onFormResize != null)
            {
                _form.Resize -= _onFormResize;
                _onFormResize = null;
            }

            FlexibilizadorFormularios.EliminarControlesAdicionalesForm(
                _form.GetPanelMain, _form.ControlesSalvar());

            _pnlLeft  = null;
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
