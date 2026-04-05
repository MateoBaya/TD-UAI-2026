using IngSoft.Abstractions.Multidioma;
using IngSoft.ApplicationServices;
using IngSoft.ApplicationServices.Factory;
using IngSoft.Domain;
using IngSoft.Domain.Multidioma;
using IngSoft.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace IngSoft.UI.Multidioma
{
    /// <summary>
    /// Instance-based Flexibilizador for Multidiomas. Creates all UI controls
    /// dynamically inside FrmPrincipal.GetPanelMain, following the same pattern
    /// as FrmUsuarioFlexiblizador. Replaces the former static class of the same name.
    /// </summary>
    internal class FrmMultidiomasFlexibilizador
    {
        private readonly FrmPrincipal _form = SingleInstancesManager.Instance.ObtenerInstancia<FrmPrincipal>();
        private readonly IMultidiomaServices _multidiomaServices = ServicesFactory.CreateMultidiomaServices();

        public IMultidiomaServices MultidiomaServices => _multidiomaServices;

        internal void EliminarControlesAdicionales()
        {
            FlexibilizadorFormularios.EliminarControlesAdicionalesForm(
                _form.GetPanelMain, _form.ControlesSalvar());
        }

        // ── Crear Idioma ────────────────────────────────────────────────────────

        internal void TextBoxCreator(string param, Point position)
        {
            FlexibilizadorFormularios.CreateTextBox(_form.GetPanelMain, param, position);
        }

        internal void CrearIdiomaButtonCreator(Point position)
        {
            FlexibilizadorFormularios.CreateButton(
                _form.GetPanelMain, "btnCrearIdioma", position,
                new Size(200, 30), "Crear Idioma", BtnCrearIdioma_Click);
        }

        private void BtnCrearIdioma_Click(object sender, EventArgs e)
        {
            try
            {
                string nombre = GetTextBoxValue("txtNombre");
                string codigo = GetTextBoxValue("txtCodigo");

                // NOTE: assumes IMultidiomaServices exposes CrearIdioma(Idioma)
                _multidiomaServices.CrearIdioma(new Idioma { Nombre = nombre, Codigo = codigo });
                MessageBox.Show("Idioma creado con éxito.");
                EliminarControlesAdicionales();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear idioma: {ex.Message}");
            }
        }

        // ── Modificar Idioma ────────────────────────────────────────────────────

        /// <summary>
        /// Adds a ComboBox populated with all idiomas. Selecting one auto-fills the DGV.
        /// </summary>
        internal ComboBox ComboBoxIdiomaCreator(Point position)
        {
            // NOTE: assumes IMultidiomaServices exposes ObtenerIdiomas()
            var idiomas = _multidiomaServices.ObtenerIdiomas().ToList();
            var cmb = new ComboBox
            {
                Name          = "cmbIdioma",
                Location      = position,
                Size          = new Size(220, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                DataSource    = idiomas,
                DisplayMember = "Nombre",
                ValueMember   = "Id"
            };
            cmb.SelectedIndexChanged += OnIdiomaSeleccionado;
            _form.GetPanelMain.Controls.Add(cmb);
            return cmb;
        }

        private void OnIdiomaSeleccionado(object sender, EventArgs e)
        {
            if (!(sender is ComboBox cmb) || !(cmb.SelectedItem is Idioma idioma))
                return;

            var controles = _multidiomaServices
                .ObtenerControlesPorIdioma(idioma.Id)
                .Cast<IControlIdioma>()
                .ToList();

            var dgv = _form.GetPanelMain.Controls
                          .Find("dgvControlesIdioma", true)
                          .FirstOrDefault() as DataGridView;

            if (dgv != null)
                dgv.DataSource = controles;
        }

        internal DataGridView DataGridViewControlesCreator(Point position, Size size)
        {
            var cols = new Dictionary<string, Type>
            {
                { "Nombre", typeof(string) },
                { "Valor",  typeof(string) }
            };
            // Empty list: selection in the ComboBox fills it via OnIdiomaSeleccionado
            return FlexibilizadorFormularios.CreateDataGridView<IControlIdioma>(
                _form.GetPanelMain, "dgvControlesIdioma", position, size, cols,
                new List<IControlIdioma>());
        }

        internal void GuardarIdiomaButtonCreator(Point position)
        {
            FlexibilizadorFormularios.CreateButton(
                _form.GetPanelMain, "btnGuardarIdioma", position,
                new Size(200, 30), "Guardar Cambios", BtnGuardarIdioma_Click);
        }

        private void BtnGuardarIdioma_Click(object sender, EventArgs e)
        {
            try
            {
                var dgv = _form.GetPanelMain.Controls
                               .Find("dgvControlesIdioma", true)
                               .FirstOrDefault() as DataGridView;

                if (dgv?.DataSource is List<IControlIdioma> controles)
                {
                    // Persist each translation: create or update Traduccion for each ControlIdioma
                    foreach (var ctrl in controles)
                    {
                        try
                        {
                            var existing = _multidiomaServices.ObtenerTraduccionPorIdiomaYControlIdioma(ctrl.IdIdioma, ctrl.Id);
                            if (existing == null)
                            {
                                var nueva = new Traduccion
                                {
                                    IdIdioma = ctrl.IdIdioma,
                                    IdControlIdioma = ctrl.Id,
                                    TextoTraducido = ctrl.TextoTraducido
                                };
                                _multidiomaServices.CrearTraduccion(nueva);
                            }
                            else
                            {
                                existing.TextoTraducido = ctrl.TextoTraducido;
                                _multidiomaServices.ActualizarTraduccion(existing);
                            }
                        }
                        catch (Exception)
                        {
                            // ignore individual failures and continue saving others
                        }
                    }
                }

                MessageBox.Show("Cambios guardados con éxito.");
                EliminarControlesAdicionales();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar cambios: {ex.Message}");
            }
        }

        // ── Helpers ─────────────────────────────────────────────────────────────

        private string GetTextBoxValue(string name) =>
            _form.GetPanelMain.Controls.Find(name, true).FirstOrDefault() is TextBox tb
                ? tb.Text
                : string.Empty;
    }
}
