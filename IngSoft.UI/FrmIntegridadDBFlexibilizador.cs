using IngSoft.ApplicationServices;
using IngSoft.ApplicationServices.Dto;
using IngSoft.ApplicationServices.Factory;
using IngSoft.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace IngSoft.UI
{
    /// <summary>
    /// Instance-based Flexibilizador for Integridad DB. Replaces the static class
    /// of the same name. Loads ResultadoIntegridad via IIntegridadDBServices,
    /// removing the constructor-injection dependency that existed in FrmIntegridadDB.
    ///
    /// NOTE: depends on IIntegridadDBServices from ServicesFactory.
    ///       Required surface: VerificarIntegridad() → ResultadoIntegridad
    /// </summary>
    internal class FrmIntegridadDBFlexibilizador
    {
        private readonly FrmPrincipal _form = SingleInstancesManager.Instance.ObtenerInstancia<FrmPrincipal>();
        private readonly IDigitoVerificadorServices _integridadServices = ServicesFactory.CreateDigitoVerificadorServices();

        private ResultadoIntegridad _resultado;

        internal void EliminarControlesAdicionales()
        {
            FlexibilizadorFormularios.EliminarControlesAdicionalesForm(
                _form.GetPanelMain, _form.ControlesSalvar());
        }

        // ── Control creators ────────────────────────────────────────────────────

        /// <summary>
        /// Creates the integrity results DataGridView. Fetches fresh data from the
        /// service on every call (and on Recalcular).
        /// </summary>
        internal DataGridView DataGridViewIntegridadCreator(Point position, Size size)
        {
            _resultado = EjecutarVerificacion();

            var cols = new Dictionary<string, Type>
            {
                { "Tabla",       typeof(string) },
                { "Id",          typeof(string) },
                { "DVEsperado",  typeof(string) },
                { "DVCalculado", typeof(string) },
                { "TipoDV",      typeof(string) }
            };

            List<MessageErrorIntegridad> items = _resultado?.Errores
                                                  ?? new List<MessageErrorIntegridad>();

            DataGridView dgv = FlexibilizadorFormularios.CreateDataGridView<MessageErrorIntegridad>(
                _form.GetPanelMain, "dgvIntegridadDB", position, size, cols, items);

            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.ReadOnly      = true;

            return dgv;
        }

        /// <summary>Creates the Recalcular button. Matches FrmIntegridadDB.CrearBotonRecalcular.</summary>
        internal void RecalcularButtonCreator(Point position, Size size)
        {
            FlexibilizadorFormularios.CreateButton(
                _form.GetPanelMain, "btnRecalcularIntegridad", position, size,
                "Recalcular", BtnRecalcular_Click);
        }

        // ── Event handlers ───────────────────────────────────────────────────────

        private void BtnRecalcular_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            try
            {
                if (btn != null) btn.Enabled = false;
                _form.Cursor = Cursors.WaitCursor;

                _resultado = EjecutarVerificacion();

                var dgv = _form.GetPanelMain.Controls
                               .Find("dgvIntegridadDB", true)
                               .FirstOrDefault() as DataGridView;

                if (dgv != null)
                    dgv.DataSource = _resultado?.Errores ?? new List<MessageErrorIntegridad>();
            }
            finally
            {
                _form.Cursor = Cursors.Default;
                if (btn != null) btn.Enabled = true;
            }
        }

        // ── Helpers ─────────────────────────────────────────────────────────────

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
    }
}
