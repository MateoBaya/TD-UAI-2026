using IngSoft.ApplicationServices;
using IngSoft.ApplicationServices.Dto;
using IngSoft.ApplicationServices.Factory;
using IngSoft.Services;
using IngSoft.UI.Pagination;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace IngSoft.UI
{
    /// <summary>
    /// Injects the Integridad DB UI controls directly into FrmPrincipal's pnlMain.
    ///
    /// Resize strategy (mirrors FrmBitacoraFlexibilizador):
    ///   pnlMain_Resize  →  LastAction = ResizeControls   ← handles dragged resize
    ///   FrmPrincipal.Resize  →  _onFormResize = ResizeControls  ← handles maximize/restore
    ///   ResizeControls() only repositions controls — it never calls the service again.
    ///
    /// Data strategy:
    ///   EjecutarVerificacion() is called ONCE in IntegridadDBCreator().
    ///   The Recalcular button and the "Ver Integridad" menu item both call
    ///   RecalcularIntegridad(), which refreshes the data source without
    ///   rebuilding any controls.
    /// </summary>
    internal class FrmIntegridadDBFlexibilizador
    {
        // ── Services ─────────────────────────────────────────────────────────────
        private readonly FrmPrincipal _form;
        private readonly IDigitoVerificadorServices _integridadServices;

        // ── Last fetched result (kept so Recalcular can replace without rebuild) ─
        private ResultadoIntegridad _resultado;

        // ── Controls kept as fields (ResizeControls moves them directly) ─────────
        private Button _btnRecalcular;
        private DataGridViewPaginationDecorator _paginatedGrid;

        // ── Form-level resize handler (kept so we can unsubscribe cleanly) ────────
        private EventHandler _onFormResize;

        // ── Layout constants (px) ─────────────────────────────────────────────────
        private const int BtnTopPad  = 12;
        private const int BtnHeight  = 41;
        private const int HMargin    = 12;
        // Top of the grid = below the button row
        private const int GridTopPad = BtnTopPad + BtnHeight + 8;

        internal FrmIntegridadDBFlexibilizador()
        {
            _form               = SingleInstancesManager.Instance.ObtenerInstancia<FrmPrincipal>();
            _integridadServices = ServicesFactory.CreateDigitoVerificadorServices();
        }

        // ── UI construction ───────────────────────────────────────────────────────

        /// <summary>
        /// Builds all controls, subscribes to Form.Resize, and fetches data once.
        /// Called by FrmIntegridadDBHeaderConfig — mirrors PantallaBitacoraCreator.
        /// </summary>
        internal void IntegridadDBCreator()
        {
            var parent = _form.GetPanelMain;
            if (parent == null) return;

            var (btnPos, btnSize) = CalculateBtnGeometry(parent.Width);
            var (gridPos, gridSize) = CalculateGridGeometry(parent.Width, parent.Height);

            RecalcularButtonCreator(btnPos, btnSize);
            DataGridViewIntegridadCreator(gridPos, gridSize);
        }

        // ── Resize ───────────────────────────────────────────────────────────────

        /// <summary>
        /// Repositions all existing controls to match the current pnlMain size.
        /// Called from LastAction (pnlMain_Resize) and Form.Resize.
        /// Never calls the service — avoids triggering a full DB verification on resize.
        /// </summary>
        internal void ResizeControls()
        {
            var parent = _form.GetPanelMain;
            // Guard: if controls haven't been built yet (or were already cleared) do nothing.
            if (parent == null || _btnRecalcular == null) return;

            var (btnPos, btnSize)   = CalculateBtnGeometry(parent.Width);
            _btnRecalcular.Location = btnPos;
            _btnRecalcular.Size     = btnSize;

            var (gridPos, gridSize) = CalculateGridGeometry(parent.Width, parent.Height);
            _paginatedGrid?.Resize(gridPos, gridSize);
        }

        // ── Public refresh (menu item + Recalcular button) ────────────────────────

        /// <summary>
        /// Re-runs the integrity check and refreshes the grid without rebuilding
        /// any controls. Used by the "Ver Integridad" menu item and the Recalcular button.
        /// </summary>
        internal void RecalcularIntegridad()
        {
            CargarIntegridad();
        }

        // ── Layout helpers ────────────────────────────────────────────────────────

        private static (Point position, Size size) CalculateBtnGeometry(int panelWidth)
        {
            int w = 187;
            return (new Point(HMargin, BtnTopPad), new Size(w, BtnHeight));
        }

        private static (Point position, Size size) CalculateGridGeometry(int panelWidth, int panelHeight)
        {
            int gridWidth  = Math.Max(200, panelWidth  - 2 * HMargin);
            int gridHeight = Math.Max(60,  panelHeight - GridTopPad - 8);
            return (new Point(HMargin, GridTopPad), new Size(gridWidth, gridHeight));
        }

        /// <summary>
        /// Creates and stores the Recalcular button.
        /// Position/size come from the caller (HeaderConfig) so layout is centralized there.
        /// </summary>
        internal void RecalcularButtonCreator(Point position, Size size)
        {
            _btnRecalcular = FlexibilizadorFormularios.CreateButton(
                _form.GetPanelMain, "btnRecalcularIntegridad",
                position, size,
                "Recalcular", BtnRecalcular_Click);
        }

        /// <summary>
        /// Creates the paginated DataGridView, subscribes Form.Resize, and loads data once.
        /// Must be called AFTER RecalcularButtonCreator so the button field is already set
        /// and ResizeControls() has both references available from the first resize.
        /// </summary>
        internal void DataGridViewIntegridadCreator(Point position, Size size)
        {
            _paginatedGrid = FlexibilizadorFormularios.CreatePaginatedDataGridView(
                _form.GetPanelMain, "dgvIntegridadDB",
                position, size,
                pageSize: 15);

            // Subscribe to Form.Resize to catch maximize / restore.
            // pnlMain_Resize (via LastAction) handles dragged resize.
            _onFormResize = (s, e) => ResizeControls();
            _form.Resize += _onFormResize;

            CargarIntegridad();
            _form.AplicarIdiomaActual();
        }

        // ── Data helpers ──────────────────────────────────────────────────────────

        private void CargarIntegridad()
        {
            _resultado = EjecutarVerificacion();
            var items  = _resultado?.Errores ?? new List<MessageErrorIntegridad>();
            _paginatedGrid?.SetDataSource(items);
        }

        private ResultadoIntegridad EjecutarVerificacion()
        {
            try
            {
                return _integridadServices.ValidarIntegridad();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al verificar integridad: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        // ── Event handlers ────────────────────────────────────────────────────────

        private void BtnRecalcular_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            try
            {
                if (btn != null) btn.Enabled = false;
                _form.Cursor = Cursors.WaitCursor;
                CargarIntegridad();
            }
            finally
            {
                _form.Cursor = Cursors.Default;
                if (btn != null) btn.Enabled = true;
            }
        }

        // ── Cleanup ───────────────────────────────────────────────────────────────

        /// <summary>
        /// Unsubscribes from Form.Resize, clears controls, and nulls all field
        /// references so ResizeControls() is a safe no-op if called afterwards.
        /// </summary>
        internal void EliminarControlesAdicionales()
        {
            // Must unsubscribe before clearing controls to avoid ghost handlers
            if (_onFormResize != null)
            {
                _form.Resize -= _onFormResize;
                _onFormResize = null;
            }

            FlexibilizadorFormularios.EliminarControlesAdicionalesForm(
                _form.GetPanelMain, _form.ControlesSalvar());

            // Null refs so ResizeControls guard fires cleanly
            _btnRecalcular = null;
            _paginatedGrid = null;
            _resultado     = null;
        }
    }
}
