using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using IngSoft.Abstractions.Multidioma;
using IngSoft.ApplicationServices;
using IngSoft.ApplicationServices.Factory;
using IngSoft.Domain.Multidioma;
using IngSoft.Services;
using IngSoft.UI.DTOs;

namespace IngSoft.UI.Multidioma
{
    internal static class FrmMultidiomasFlexibilizador
    {
        internal static void CrearPantallaCrearIdioma(
            Point ptNombre,
            Point ptCodigo,
            Point ptBtn)
        {
            var parent = FrmMultidiomas.ActiveForm;
            IMultidiomaServices multidiomaServices = ServicesFactory.CreateMultidiomaServices();

            FlexibilizadorFormularios.CreateTextBox(parent, "IdiomaNombre", ptNombre);
            FlexibilizadorFormularios.CreateTextBox(parent, "IdiomaCodigo", ptCodigo);

            FlexibilizadorFormularios.CreateButton(parent, "btnCrearIdioma", ptBtn, new Size(120, 35), "Crear Idioma", (s, e) =>
            {
                var form = FrmMultidiomas.ActiveForm;
                var txtNombre = form.Controls.Find("txtIdiomaNombre", true).FirstOrDefault() as TextBox;
                var txtCodigo = form.Controls.Find("txtIdiomaCodigo", true).FirstOrDefault() as TextBox;

                if (string.IsNullOrWhiteSpace(txtNombre?.Text))
                {
                    MessageBox.Show("El nombre del idioma no puede estar vacío.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtCodigo?.Text))
                {
                    MessageBox.Show("El código del idioma no puede estar vacío.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var nuevoIdioma = new Idioma
                {
                    Id = Guid.NewGuid(),
                    Nombre = txtNombre.Text.Trim(),
                    Codigo = txtCodigo.Text.Trim()
                };

                try
                {
                    multidiomaServices.CrearIdioma(nuevoIdioma);
                    MessageBox.Show("Idioma creado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtNombre.Text = string.Empty;
                    txtCodigo.Text = string.Empty;
                }
                catch (Exception)
                {
                    MessageBox.Show("Se quiere crear un Idioma que ya existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
        }

        internal static void CrearPantallaModificarIdioma(
            Point ptCombo,
            Point ptDgv,
            Size szDgv,
            Point ptBtn)
        {
            var parent = FrmMultidiomas.ActiveForm;
            IMultidiomaServices multidiomaServices = ServicesFactory.CreateMultidiomaServices();

            var lblCombo = new Label
            {
                Name = "lblIdiomas",
                Text = "Idioma",
                AutoSize = true,
                Location = new Point(ptCombo.X, ptCombo.Y - 20)
            };
            parent.Controls.Add(lblCombo);

            var cbo = new ComboBox
            {
                Name = "cboIdiomas",
                Location = ptCombo,
                Size = new Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = "Nombre"
            };

            var idiomas = multidiomaServices.ObtenerIdiomas().Cast<IIdioma>().ToList();
            var idiomasCacheados = MultidiomaManager.ObtenerIdiomasCache(idiomas);
            cbo.DataSource = idiomasCacheados;

            parent.Controls.Add(cbo);

            var dgv = FlexibilizadorFormularios.CreateDataGridView(parent, "dgvTraducciones", ptDgv, szDgv);
            dgv.ReadOnly = false;

            cbo.SelectedIndexChanged += (s, e) =>
            {
                var idiomaSeleccionado = cbo.SelectedItem as IIdioma;
                if (idiomaSeleccionado != null)
                    CargarTraducciones(cbo, dgv, multidiomaServices);
            };

            if (cbo.Items.Count > 0)
                CargarTraducciones(cbo, dgv, multidiomaServices);

            FlexibilizadorFormularios.CreateButton(parent, "btnGuardarTraducciones", ptBtn, new Size(120, 35), "Guardar", (s, e) =>
            {
                var form = FrmMultidiomas.ActiveForm;
                var cboCtrl = form.Controls.Find("cboIdiomas", true).FirstOrDefault() as ComboBox;
                var dgvCtrl = form.Controls.Find("dgvTraducciones", true).FirstOrDefault() as DataGridView;

                var idiomaSeleccionado = cboCtrl?.SelectedItem as IIdioma;
                var traduccionesActuales = dgvCtrl?.DataSource as List<MultidiomaGridDto>;
                if (idiomaSeleccionado == null || traduccionesActuales == null) return;

                var controlIdioma = multidiomaServices.ObtenerControlesPorIdioma(idiomaSeleccionado.Id);

                foreach (var traduccionDto in traduccionesActuales)
                {
                    if (controlIdioma.Count() > 0)
                    {
                        var traduccion = new Traduccion
                        {
                            IdIdioma = idiomaSeleccionado.Id,
                            IdControlIdioma = traduccionDto.IdControlIdioma,
                            TextoTraducido = traduccionDto.TextoTraducido
                        };
                        multidiomaServices.ActualizarTraduccion(traduccion);
                    }
                    else
                    {
                        var traduccion = new Traduccion
                        {
                            Id = Guid.NewGuid(),
                            IdIdioma = idiomaSeleccionado.Id,
                            IdControlIdioma = traduccionDto.IdControlIdioma,
                            TextoTraducido = traduccionDto.TextoTraducido
                        };
                        multidiomaServices.CrearTraduccion(traduccion);
                    }
                }

                MessageBox.Show("Traducciones guardadas correctamente.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarTraducciones(cboCtrl, dgvCtrl, multidiomaServices);
            });
        }

        private static void CargarTraducciones(ComboBox cbo, DataGridView dgv, IMultidiomaServices svc)
        {
            var idiomaSeleccionado = cbo.SelectedItem as IIdioma;
            if (idiomaSeleccionado == null) return;

            var idiomaPorDefecto = svc.ObtenerIdiomaPorDefecto();
            var traduccionesPorDefecto = svc.ObtenerControlesPorIdioma(idiomaPorDefecto.Id);
            var traducciones = svc.ObtenerControlesPorIdioma(idiomaSeleccionado.Id);

            var listaMultidioma = new List<MultidiomaGridDto>();
            foreach (var traduccionDefecto in traduccionesPorDefecto)
            {
                var traduccionExistente = traducciones.FirstOrDefault(t => t.NombreControl == traduccionDefecto.NombreControl);
                var dto = new MultidiomaGridDto
                {
                    NombreControl = traduccionDefecto.NombreControl,
                    TextoPorDefecto = traduccionDefecto.TextoTraducido,
                    IdControlIdioma = traduccionDefecto.Id,
                    TextoTraducido = traduccionExistente != null
                        ? traduccionExistente.TextoTraducido
                        : "[" + traduccionDefecto.TextoTraducido + "]"
                };
                listaMultidioma.Add(dto);
            }

            dgv.DataSource = listaMultidioma;

            if (dgv.Columns.Count > 0)
            {
                if (dgv.Columns["NombreControl"] != null) { dgv.Columns["NombreControl"].HeaderText = "Control"; dgv.Columns["NombreControl"].ReadOnly = true; dgv.Columns["NombreControl"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; }
                if (dgv.Columns["TextoPorDefecto"] != null) { dgv.Columns["TextoPorDefecto"].HeaderText = "Texto Original"; dgv.Columns["TextoPorDefecto"].ReadOnly = true; dgv.Columns["TextoPorDefecto"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; }
                if (dgv.Columns["TextoTraducido"] != null) { dgv.Columns["TextoTraducido"].HeaderText = "Traducción"; dgv.Columns["TextoTraducido"].ReadOnly = false; dgv.Columns["TextoTraducido"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; }
                if (dgv.Columns["IdControlIdioma"] != null) dgv.Columns["IdControlIdioma"].Visible = false;
            }
        }
    }
}
