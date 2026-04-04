using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using IngSoft.Abstractions.Multidioma;
using IngSoft.ApplicationServices;
using IngSoft.ApplicationServices.Factory;
using IngSoft.Domain;
using IngSoft.Domain.Enums;
using IngSoft.Domain.Multidioma;
using IngSoft.Services;
using IngSoft.UI.DTOs;
using IngSoft.UI.Pagination;

namespace IngSoft.UI.BitacoraUI
{
    /// <summary>
    /// Injects the Bitacora UI controls directly into FrmPrincipal's pnlMain.
    ///
    /// Resize strategy:
    ///   pnlMain_Resize  →  LastAction (ResizeControls)   ← handles dragged resize
    ///   FrmPrincipal.Resize  →  OnFormResize (ResizeControls)  ← handles maximize/restore
    ///   Both paths read pnlMain.Width/Height AFTER the form has settled,
    ///   so controls are always placed at the correct final size.
    ///
    /// Label strategy:
    ///   Each label is stored as a field alongside its sibling control.
    ///   ResizeControls moves them directly — no Controls.Find needed.
    /// </summary>
    internal class FrmBitacoraFlexibilizador
    {
        // ── Services ─────────────────────────────────────────────────────────────
        private readonly FrmPrincipal _form;
        private readonly IBitacoraServices _bitacoraServices;

        // ── Filter state ─────────────────────────────────────────────────────────
        private List<Bitacora> _bitacoras;

        // ── Filter controls ───────────────────────────────────────────────────────
        private TextBox        _txtBusqueda;
        private Label          _lblBusqueda;
        private ComboBox       _cboTipoEvento;
        private Label          _lblTipoEvento;
        private CheckBox       _chkFiltroFecha;
        private DateTimePicker _dtpFechaDesde;
        private Label          _lblFechaDesde;
        private DateTimePicker _dtpFechaHasta;
        private Label          _lblFechaHasta;

        // ── Paginated grid ────────────────────────────────────────────────────────
        private DataGridViewPaginationDecorator _paginatedGrid;

        // ── Form-level resize handler (kept so we can unsubscribe cleanly) ────────
        private EventHandler _onFormResize;

        // ── Layout constants (px) ─────────────────────────────────────────────────
        private const int FilterRowTopPad = 30;   // vertical space reserved for labels
        private const int ControlHeight   = 24;   // height of every filter control
        private const int FilterRowHeight = FilterRowTopPad + ControlHeight + 8;
        private const int HMargin         = 12;

        internal FrmBitacoraFlexibilizador()
        {
            _form               = SingleInstancesManager.Instance.ObtenerInstancia<FrmPrincipal>();
            _bitacoraServices   = ServicesFactory.CreateBitacoraServices();
        }

        // ── UI construction ───────────────────────────────────────────────────────

        internal void PantallaBitacoraCreator()
        {
            var parent = _form.GetPanelMain;
            if (parent == null) return;

            int controlY = FilterRowTopPad;
            var (txtX, txtW, cboX, cboW, chkX, chkW, dtpDesdeX, dtpW, dtpHastaX) =
                CalculateFilterPositions(parent.Width);

            // ── Search textbox ────────────────────────────────────────────────────
            _txtBusqueda = FlexibilizadorFormularios.CreateTextBox(
                parent, "BusquedaBitacora",
                new Point(txtX, controlY),
                new Size(txtW, ControlHeight));
            _txtBusqueda.TextChanged += TxtBusqueda_TextChanged;
            _lblBusqueda = parent.Controls.Find("lblBusquedaBitacora", false).FirstOrDefault() as Label;

            // ── TipoEvento combobox ───────────────────────────────────────────────
            _cboTipoEvento = FlexibilizadorFormularios.CreateComboBox(
                parent, "TipoEvento",
                new Point(cboX, controlY),
                new Size(cboW, ControlHeight),
                BuildTipoEventoItems(),
                CboTipoEvento_SelectedIndexChanged);
            _lblTipoEvento = parent.Controls.Find("lblTipoEvento", false).FirstOrDefault() as Label;

            // ── Date-filter checkbox ──────────────────────────────────────────────
            _chkFiltroFecha = FlexibilizadorFormularios.CreateCheckBox(
                parent, "FiltroFecha",
                new Point(chkX, controlY),
                new Size(chkW, ControlHeight),
                isChecked: false,
                onCheckedChanged: ChkFiltroFecha_CheckedChanged);

            // ── DateTimePicker Desde ──────────────────────────────────────────────
            _dtpFechaDesde = FlexibilizadorFormularios.CreateDateTimePicker(
                parent, "FechaDesde",
                new Point(dtpDesdeX, controlY),
                new Size(dtpW, ControlHeight),
                enabled: false,
                onValueChanged: DtpFechaDesde_ValueChanged);
            _lblFechaDesde = parent.Controls.Find("lblFechaDesde", false).FirstOrDefault() as Label;

            // ── DateTimePicker Hasta ──────────────────────────────────────────────
            _dtpFechaHasta = FlexibilizadorFormularios.CreateDateTimePicker(
                parent, "FechaHasta",
                new Point(dtpHastaX, controlY),
                new Size(dtpW, ControlHeight),
                enabled: false,
                onValueChanged: DtpFechaHasta_ValueChanged);
            _lblFechaHasta = parent.Controls.Find("lblFechaHasta", false).FirstOrDefault() as Label;

            // ── Paginated DataGridView ────────────────────────────────────────────
            var (gridPos, gridSize) = CalculateGridGeometry(parent.Width, parent.Height);
            _paginatedGrid = FlexibilizadorFormularios.CreatePaginatedDataGridView(
                parent, "gridBitacora",
                gridPos, gridSize,
                pageSize: 15,
                onDataBindingComplete: Grid_DataBindingComplete);

            // ── Subscribe to Form.Resize to catch maximize / restore ──────────────
            // pnlMain_Resize (via LastAction) handles dragged resize.
            // Form.Resize fires AFTER the OS has finished laying out the window,
            // so pnlMain.Width/Height are already at their final values — this is
            // the event that was missing for maximize.
            _onFormResize = (s, e) => ResizeControls();
            _form.Resize += _onFormResize;

            // ── Data + idioma ─────────────────────────────────────────────────────
            CargarBitacora();
            _form.AplicarIdiomaActual();
        }

        // ── Resize ───────────────────────────────────────────────────────────────

        /// <summary>
        /// Repositions all existing controls to match the current pnlMain size.
        /// Called both from LastAction (pnlMain_Resize) and from Form.Resize.
        /// Never recreates controls — avoids flicker and data loss.
        /// </summary>
        internal void ResizeControls()
        {
            var parent = _form.GetPanelMain;
            if (parent == null || _txtBusqueda == null) return;

            int controlY = FilterRowTopPad;
            var (txtX, txtW, cboX, cboW, chkX, chkW, dtpDesdeX, dtpW, dtpHastaX) =
                CalculateFilterPositions(parent.Width);

            MoveControl(_txtBusqueda,    _lblBusqueda,   txtX,      controlY, txtW,  ControlHeight);
            MoveControl(_cboTipoEvento,  _lblTipoEvento, cboX,      controlY, cboW,  ControlHeight);
            MoveControl(_chkFiltroFecha, null,           chkX,      controlY, chkW,  ControlHeight);
            MoveControl(_dtpFechaDesde,  _lblFechaDesde, dtpDesdeX, controlY, dtpW,  ControlHeight);
            MoveControl(_dtpFechaHasta,  _lblFechaHasta, dtpHastaX, controlY, dtpW,  ControlHeight);

            var (gridPos, gridSize) = CalculateGridGeometry(parent.Width, parent.Height);
            _paginatedGrid?.Resize(gridPos, gridSize);
        }

        // ── Layout helpers ────────────────────────────────────────────────────────

        private static (int txtX, int txtW, int cboX, int cboW,
                        int chkX, int chkW, int dtpDesdeX, int dtpW, int dtpHastaX)
            CalculateFilterPositions(int panelWidth)
        {
            int m    = HMargin;
            int txtW = Math.Max(120, panelWidth / 4);
            int cboW = Math.Max(100, panelWidth / 7);
            int chkW = Math.Max(100, panelWidth / 8);
            int dtpW = Math.Max(100, panelWidth / 8);

            int txtX      = m;
            int cboX      = txtX + txtW + m;
            int chkX      = cboX + cboW + m;
            int dtpDesdeX = chkX + chkW + (m / 2);
            int dtpHastaX = dtpDesdeX + dtpW + (m / 2);

            return (txtX, txtW, cboX, cboW, chkX, chkW, dtpDesdeX, dtpW, dtpHastaX);
        }

        private static (Point position, Size size) CalculateGridGeometry(int panelWidth, int panelHeight)
        {
            int gridY      = FilterRowHeight + 4;
            int gridHeight = Math.Max(60, panelHeight - gridY - 8);
            int gridWidth  = Math.Max(200, panelWidth - 2 * HMargin);
            return (new Point(HMargin, gridY), new Size(gridWidth, gridHeight));
        }

        /// <summary>
        /// Moves a control and (optionally) its label to the new position/size.
        /// Works directly on stored references — no Controls.Find required.
        /// </summary>
        private static void MoveControl(Control ctrl, Label lbl,
            int x, int y, int w, int h)
        {
            if (ctrl == null) return;
            ctrl.Location = new Point(x, y);
            ctrl.Size     = new Size(w, h);

            if (lbl != null)
            {
                lbl.Location = new Point(x, y - 20);
                lbl.Width    = w;
            }
        }

        // ── Data helpers ──────────────────────────────────────────────────────────

        private void CargarBitacora(string filtro = null)
        {
            _bitacoras = filtro != null
                ? _bitacoraServices.ObtenerBitacorasFiltradas(filtro)
                : _bitacoraServices.ObtenerBitacoras();
            _paginatedGrid?.SetDataSource(BuildBitacorasDataGridView(_bitacoras));
        }

        private List<BitacoraGridDto> BuildBitacorasDataGridView(List<Bitacora> bitacoras)
        {
            return bitacoras
                .Select(b => new BitacoraGridDto
                {
                    Fecha       = b.Fecha,
                    Descripcion = b.Descripcion,
                    Origen      = b.Origen,
                    TipoEvento  = b.TipoEvento.ToString(),
                    Usuario     = b.Usuario.UserName
                })
                .OrderByDescending(b => b.Fecha)
                .ToList();
        }

        private static IEnumerable<string> BuildTipoEventoItems()
        {
            var items = new List<string> { "Todos" };
            items.AddRange(Enum.GetValues(typeof(TipoEvento))
                              .Cast<TipoEvento>()
                              .Select(t => t.ToString()));
            return items;
        }

        // ── Filter event handlers ─────────────────────────────────────────────────

        private void TxtBusqueda_TextChanged(object sender, EventArgs e)
        {
            var filtro = (sender as TextBox)?.Text.Trim() ?? string.Empty;
            CargarBitacora(filtro.Length > 3 ? filtro : null);
        }

        private void CboTipoEvento_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cboTipoEvento?.SelectedItem == null || _bitacoras == null) return;
            var selected = _cboTipoEvento.SelectedItem.ToString();

            if (selected == "Todos")
            {
                CargarBitacora();
            }
            else
            {
                var filtered = _bitacoras
                    .Where(b => b.TipoEvento.ToString() == selected)
                    .ToList();
                _paginatedGrid?.SetDataSource(BuildBitacorasDataGridView(filtered));
            }
        }

        private void ChkFiltroFecha_CheckedChanged(object sender, EventArgs e)
        {
            bool activo = (sender as CheckBox)?.Checked ?? false;
            if (_dtpFechaDesde != null) _dtpFechaDesde.Enabled = activo;
            if (!activo)
            {
                if (_dtpFechaHasta != null) _dtpFechaHasta.Enabled = false;
                CargarBitacora();
            }
        }

        private void DtpFechaDesde_ValueChanged(object sender, EventArgs e)
        {
            if (_dtpFechaHasta != null) _dtpFechaHasta.Enabled = true;
            AplicarFiltroPorFecha();
        }

        private void DtpFechaHasta_ValueChanged(object sender, EventArgs e)
        {
            AplicarFiltroPorFecha();
        }

        private void AplicarFiltroPorFecha()
        {
            if (_dtpFechaDesde == null || _dtpFechaHasta == null || _bitacoras == null) return;

            var desde = _dtpFechaDesde.Value.Date;
            var hasta = _dtpFechaHasta.Value.Date;

            if (hasta < desde)
            {
                MessageBox.Show(
                    "La fecha hasta no puede ser menor a la fecha desde.",
                    "Error de fecha",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                _dtpFechaHasta.Value = desde;
                CargarBitacora();
                return;
            }

            var filtered = _bitacoras
                .Where(b => b.Fecha.Date >= desde && b.Fecha.Date <= hasta)
                .ToList();
            _paginatedGrid?.SetDataSource(BuildBitacorasDataGridView(filtered));
        }

        private void Grid_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            var dgv = sender as DataGridView;
            if (dgv?.Columns["Descripcion"] != null)
                dgv.Columns["Descripcion"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        // ── Cleanup ───────────────────────────────────────────────────────────────

        internal void EliminarControlesAdicionales()
        {
            // Unsubscribe from Form.Resize before clearing controls
            if (_onFormResize != null)
            {
                _form.Resize -= _onFormResize;
                _onFormResize = null;
            }

            FlexibilizadorFormularios.EliminarControlesAdicionalesForm(
                _form.GetPanelMain,
                _form.ControlesSalvar());

            // Null refs so ResizeControls is a safe no-op before next build
            _txtBusqueda    = null; _lblBusqueda   = null;
            _cboTipoEvento  = null; _lblTipoEvento = null;
            _chkFiltroFecha = null;
            _dtpFechaDesde  = null; _lblFechaDesde = null;
            _dtpFechaHasta  = null; _lblFechaHasta = null;
            _paginatedGrid  = null;
        }
    }
}
