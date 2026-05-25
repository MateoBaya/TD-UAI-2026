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
    internal class FrmMisPedidosFlexibilizador
    {
        private readonly FrmPrincipal _form = SingleInstancesManager.Instance.ObtenerInstancia<FrmPrincipal>();
        private readonly ICarritoServices _carritoServices = ServicesFactory.CreateCarritoMinoristaServices();

        private List<PedidoResumen> _pedidos = new List<PedidoResumen>();
        private List<PedidoItem>    _detalle = new List<PedidoItem>();
        private PedidoResumen       _pedidoSeleccionado;

        // ── Paneles ──────────────────────────────────────────────────────────────
        private Panel _pnlLeft;
        private Panel _pnlRight;

        // ── Controles panel izquierdo ────────────────────────────────────────────
        private DataGridView _dgvPedidos;

        // ── Controles panel derecho ──────────────────────────────────────────────
        private Label        _lblNroCarrito;
        private Label        _lblTipo;
        private Label        _lblEstado;
        private Label        _lblFechaPedido;
        private Label        _lblFechaEntrega;
        private DataGridView _dgvDetalle;
        private Label        _lblTotal;

        // ── Resize ───────────────────────────────────────────────────────────────
        private EventHandler _onFormResize;

        // ── Constantes de layout ──────────────────────────────────────────────────
        private const int CardH        = 120;
        private const int TopPad       = 8;
        private const int ItemsDgvRelY = TopPad + 28 + CardH + 28;  // 184
        private const int TotalFromBot = 42;

        // ── Geometría ────────────────────────────────────────────────────────────
        private (int topY, int panelH, int leftW, int rightW, int rightX) Geometry()
        {
            var pnl    = _form.GetPanelMain;
            int menuH  = _form.MainMenuStrip?.Bottom ?? 28;
            int topY   = menuH + 6;
            int margin = 12;
            int totalW = pnl.Width - margin * 2;
            int panelH = pnl.Height - topY - 8;
            int leftW  = (int)(totalW * 0.45);
            int rightW = totalW - leftW - margin;
            int rightX = margin + leftW + margin;
            return (topY, panelH, leftW, rightW, rightX);
        }

        // ── Construcción inicial ─────────────────────────────────────────────────
        internal void ConstruirLayout()
        {
            _pedidoSeleccionado = null;
            _detalle.Clear();

            CargarPedidos();

            var pnlMain = _form.GetPanelMain;
            var (topY, panelH, leftW, rightW, rightX) = Geometry();
            int margin = 12;

            // ── Panel izquierdo ───────────────────────────────────────────────────
            _pnlLeft = FlexibilizadorFormularios.CreatePanel(pnlMain, "pnlMisPedidosLeft",
                new Point(margin, topY), new Size(leftW, panelH), Color.White);
            _pnlLeft.BorderStyle = BorderStyle.FixedSingle;

            FlexibilizadorFormularios.CreateDisplayLabel(_pnlLeft, "lblPedidosTitle",
                new Point(8, 10), new Size(leftW - 16, 22), "Mis Pedidos",
                new Font("Arial", 10, FontStyle.Bold));

            _dgvPedidos = CrearDGVPedidos(_pnlLeft,
                new Point(8, 40), new Size(leftW - 18, panelH - 54));
            ColorearFilasPorEstado();

            // ── Panel derecho ─────────────────────────────────────────────────────
            _pnlRight = FlexibilizadorFormularios.CreatePanel(pnlMain, "pnlMisPedidosRight",
                new Point(rightX, topY), new Size(rightW, panelH), Color.White);
            _pnlRight.BorderStyle = BorderStyle.FixedSingle;

            FlexibilizadorFormularios.CreateDisplayLabel(_pnlRight, "lblMisPedDetalleTitle",
                new Point(8, 10), new Size(rightW - 16, 22), "Detalle del pedido",
                new Font("Arial", 10, FontStyle.Bold));

            var pnlCard = FlexibilizadorFormularios.CreatePanel(_pnlRight, "pnlPedidoCard",
                new Point(8, TopPad + 28), new Size(rightW - 18, CardH),
                Color.FromArgb(245, 248, 252));
            pnlCard.BorderStyle = BorderStyle.FixedSingle;
            ConstruirFilasCard(pnlCard, rightW - 18);

            FlexibilizadorFormularios.CreateDisplayLabel(_pnlRight, "lblMisPedItemsTitle",
                new Point(8, TopPad + 28 + CardH + 8), new Size(rightW - 16, 20),
                "Items:", new Font("Arial", 9, FontStyle.Bold));

            int detDgvH = panelH - TotalFromBot - 4 - ItemsDgvRelY;
            _dgvDetalle = CrearDGVDetalleVacio(_pnlRight,
                new Point(8, ItemsDgvRelY), new Size(rightW - 18, detDgvH));

            _lblTotal = FlexibilizadorFormularios.CreateDisplayLabel(_pnlRight, "lblMisPedidosTotal",
                new Point(8, panelH - TotalFromBot), new Size(rightW - 18, 22),
                "Total:  $ 0,00",
                new Font("Arial", 10, FontStyle.Bold), ContentAlignment.MiddleRight);

            _dgvPedidos.SelectionChanged += DGVPedidos_SelectionChanged;
            _dgvPedidos.ClearSelection();

            _onFormResize = (s, e) => ResizeControls();
            _form.Resize += _onFormResize;
        }

        // ── Resize ───────────────────────────────────────────────────────────────
        internal void ResizeControls()
        {
            if (_pnlLeft == null) return;

            var (topY, panelH, leftW, rightW, rightX) = Geometry();
            int margin = 12;

            _pnlLeft.Location = new Point(margin, topY);
            _pnlLeft.Size     = new Size(leftW, panelH);

            var lblTitLeft = _pnlLeft.Controls.Find("lblPedidosTitle", false).FirstOrDefault() as Label;
            if (lblTitLeft != null) lblTitLeft.Size = new Size(leftW - 16, 22);

            if (_dgvPedidos != null) _dgvPedidos.Size = new Size(leftW - 18, panelH - 54);

            _pnlRight.Location = new Point(rightX, topY);
            _pnlRight.Size     = new Size(rightW, panelH);

            var pnlCard = _pnlRight.Controls.Find("pnlPedidoCard", false).FirstOrDefault() as Panel;
            if (pnlCard != null)
            {
                pnlCard.Size = new Size(rightW - 18, CardH);
                ActualizarAnchoCard(pnlCard, rightW - 18);
            }

            if (_dgvDetalle != null)
                _dgvDetalle.Size = new Size(rightW - 18, panelH - TotalFromBot - 4 - ItemsDgvRelY);

            if (_lblTotal != null)
            {
                _lblTotal.Location = new Point(8, panelH - TotalFromBot);
                _lblTotal.Width    = rightW - 18;
            }
        }

        // ── Ficha del pedido ─────────────────────────────────────────────────────
        private void ConstruirFilasCard(Panel card, int cardW)
        {
            int valX = 110;
            int valW = cardW - valX - 10;

            CrearFilaCard(card, "lblNroPedidoTitle",  "lblNroPedido",    8,  "Nro. Pedido:",  valX, valW);
            CrearFilaCard(card, "lblTipoTitle",        "lblTipo",         28, "Tipo:",          valX, valW);
            CrearFilaCard(card, "lblEstadoTitle",      "lblEstado",       48, "Estado:",        valX, valW);
            CrearFilaCard(card, "lblFechaPTitle",      "lblFechaPedido",  68, "Fecha Pedido:",  valX, valW);
            CrearFilaCard(card, "lblFechaETitle",      "lblFechaEntrega", 88, "Fecha Entrega:", valX, valW);

            _lblNroCarrito   = (Label)card.Controls.Find("lblNroPedido",    false)[0];
            _lblTipo         = (Label)card.Controls.Find("lblTipo",         false)[0];
            _lblEstado       = (Label)card.Controls.Find("lblEstado",       false)[0];
            _lblFechaPedido  = (Label)card.Controls.Find("lblFechaPedido",  false)[0];
            _lblFechaEntrega = (Label)card.Controls.Find("lblFechaEntrega", false)[0];
        }

        private static void CrearFilaCard(Panel card, string nameTitle, string nameValue,
            int y, string titulo, int valX, int valW)
        {
            FlexibilizadorFormularios.CreateDisplayLabel(card, nameTitle,
                new Point(8, y), new Size(100, 20), titulo,
                new Font("Arial", 8, FontStyle.Bold));
            FlexibilizadorFormularios.CreateDisplayLabel(card, nameValue,
                new Point(valX, y), new Size(valW, 20), "—", new Font("Arial", 9));
        }

        private static void ActualizarAnchoCard(Panel card, int cardW)
        {
            int valX = 110;
            int valW = cardW - valX - 10;
            foreach (string name in new[] { "lblNroPedido", "lblTipo", "lblEstado", "lblFechaPedido", "lblFechaEntrega" })
            {
                var lbl = card.Controls.Find(name, false).FirstOrDefault() as Label;
                if (lbl != null) lbl.Width = valW;
            }
        }

        // ── Grillas ──────────────────────────────────────────────────────────────
        private DataGridView CrearDGVPedidos(Panel parent, Point position, Size size)
        {
            var dgv = FlexibilizadorFormularios.CreateDataGridView(
                parent, "dgvMisPedidos", position, size,
                BuildPedidosDataTable(_pedidos));
            if (dgv.Columns["Id"] != null)
                dgv.Columns["Id"].Visible = false;
            return dgv;
        }

        private DataGridView CrearDGVDetalleVacio(Panel parent, Point position, Size size)
        {
            var dgv = FlexibilizadorFormularios.CreateDataGridView(
                parent, "dgvDetallePedido", position, size,
                BuildItemsDataTable(new List<PedidoItem>()));
            dgv.RowHeadersVisible = false;
            return dgv;
        }

        // ── DataTables ───────────────────────────────────────────────────────────
        private static DataTable BuildPedidosDataTable(List<PedidoResumen> pedidos)
        {
            var dt = new DataTable();
            dt.Columns.Add("Id",            typeof(Guid));
            dt.Columns.Add("Nro.",          typeof(int));
            dt.Columns.Add("Tipo",          typeof(string));
            dt.Columns.Add("Estado",        typeof(string));
            dt.Columns.Add("Fecha Pedido",  typeof(DateTime));
            dt.Columns.Add("Fecha Entrega", typeof(string));
            foreach (var p in pedidos)
                dt.Rows.Add(p.Id, p.NroCarrito, p.Tipo, p.Estado, p.FechaInsert, p.FechaEntregaTexto);
            return dt;
        }

        private static DataTable BuildItemsDataTable(List<PedidoItem> items)
        {
            var dt = new DataTable();
            dt.Columns.Add("Producto", typeof(string));
            dt.Columns.Add("Cantidad", typeof(int));
            dt.Columns.Add("Precio",   typeof(float));
            dt.Columns.Add("Subtotal", typeof(float));
            foreach (var item in items)
                dt.Rows.Add(item.NombreProducto, item.Cantidad, item.Precio, item.Subtotal);
            return dt;
        }

        // ── Color por Estado ─────────────────────────────────────────────────────
        private void ColorearFilasPorEstado()
        {
            if (_dgvPedidos == null) return;
            foreach (DataGridViewRow row in _dgvPedidos.Rows)
            {
                if (row.IsNewRow) continue;
                var estado = row.Cells["Estado"]?.Value?.ToString();
                switch (estado)
                {
                    case "Aceptado":
                        row.DefaultCellStyle.BackColor = Color.FromArgb(212, 237, 218);
                        row.DefaultCellStyle.ForeColor = Color.FromArgb(21,  87,  36);
                        break;
                    case "Rechazado":
                        row.DefaultCellStyle.BackColor = Color.FromArgb(248, 215, 218);
                        row.DefaultCellStyle.ForeColor = Color.FromArgb(114, 28,  36);
                        break;
                    default: // Pendiente
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 248, 220);
                        row.DefaultCellStyle.ForeColor = Color.FromArgb(133, 100,  4);
                        break;
                }
            }
        }

        // ── Carga de datos ───────────────────────────────────────────────────────
        private void CargarPedidos()
        {
            try
            {
                _pedidos = _carritoServices.ObtenerMisPedidos() ?? new List<PedidoResumen>();
            }
            catch (Exception)
            {
                _pedidos = new List<PedidoResumen>();
            }
        }

        private void CargarDetalle()
        {
            try
            {
                _detalle = _carritoServices.ObtenerDetallePedido(_pedidoSeleccionado.Id)
                           ?? new List<PedidoItem>();
            }
            catch (Exception)
            {
                _detalle = new List<PedidoItem>();
            }

            _dgvDetalle.DataSource = BuildItemsDataTable(_detalle);
            float total = _detalle.Sum(i => i.Subtotal);
            _lblTotal.Text = $"Total:  $ {total:F2}";
        }

        private void ResetDetalle()
        {
            _detalle.Clear();
            if (_dgvDetalle      != null) _dgvDetalle.DataSource = BuildItemsDataTable(_detalle);
            if (_lblTotal        != null) _lblTotal.Text = "Total:  $ 0,00";
            if (_lblNroCarrito   != null) _lblNroCarrito.Text   = "—";
            if (_lblTipo         != null) _lblTipo.Text         = "—";
            if (_lblEstado       != null) _lblEstado.Text       = "—";
            if (_lblFechaPedido  != null) _lblFechaPedido.Text  = "—";
            if (_lblFechaEntrega != null) _lblFechaEntrega.Text = "—";
        }

        // ── Seleccion ────────────────────────────────────────────────────────────
        private void DGVPedidos_SelectionChanged(object sender, EventArgs e)
        {
            if (_dgvPedidos.SelectedRows.Count == 0)
            {
                _pedidoSeleccionado = null;
                ResetDetalle();
                return;
            }

            var row = _dgvPedidos.SelectedRows[0];
            if (!(row.DataBoundItem is DataRowView drv)) return;
            if (!(drv.Row["Id"] is Guid id)) return;

            _pedidoSeleccionado = _pedidos.FirstOrDefault(p => p.Id == id);
            if (_pedidoSeleccionado == null) return;

            if (_lblNroCarrito   != null) _lblNroCarrito.Text   = _pedidoSeleccionado.NroCarrito.ToString();
            if (_lblTipo         != null) _lblTipo.Text         = _pedidoSeleccionado.Tipo;
            if (_lblEstado       != null) _lblEstado.Text       = _pedidoSeleccionado.Estado;
            if (_lblFechaPedido  != null) _lblFechaPedido.Text  = _pedidoSeleccionado.FechaInsert.ToString("dd/MM/yyyy HH:mm");
            if (_lblFechaEntrega != null) _lblFechaEntrega.Text = _pedidoSeleccionado.FechaEntregaTexto;

            CargarDetalle();
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
    }
}
