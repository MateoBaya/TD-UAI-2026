using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using IngSoft.Abstractions.Multidioma;
using IngSoft.ApplicationServices;
using IngSoft.Domain.Multidioma;
using IngSoft.Services;
using IngSoft.UI.DTOs;

namespace IngSoft.UI.Multidioma
{
    internal static class FrmMultidiomasFlexibilizador
    {
        public static void CrearPantallaCrearIdioma(
            FrmMultidiomas form,
            IMultidiomaServices svc,
            Point ptNombre,
            Point ptCodigo,
            Point ptBtn)
        {
            FlexibilizadorFormularios.CreateTextBox(form, "IdiomaNombre", ptNombre);
            FlexibilizadorFormularios.CreateTextBox(form, "IdiomaCodigo", ptCodigo);

            FlexibilizadorFormularios.CreateButton(form, "btnCrearIdioma", ptBtn, new Size(120, 35), "Crear Idioma", (s, e) =>
            {
                var txtNombre = form.Controls.Find("txtIdiomaNombre", true)[0] as TextBox;
                var txtCodigo = form.Controls.Find("txtIdiomaCodigo", true)[0] as TextBox;

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
                    svc.CrearIdioma(nuevoIdioma);
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

        public static void CrearPantallaModificarIdioma(
            FrmMultidiomas form,
            IMultidiomaServices svc,
            Point ptCombo,
            Point ptDgv,
            Size szDgv,
            Point ptBtn)
        {
            var lblCombo = new Label
            {
                Name = "lblIdiomas",
                Text = "Idioma",
                AutoSize = true,
                Location = new Point(ptCombo.X, ptCombo.Y - 20)
            };
            form.Controls.Add(lblCombo);

            var cbo = new ComboBox
            {
                Name = "cboIdiomas",
                Location = ptCombo,
                Size = new Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = "Nombre"
            };

            var idiomas = svc.ObtenerIdiomas().Cast<IIdioma>().ToList();
            var idiomasCacheados = MultidiomaManager.ObtenerIdiomasCache(idiomas);
            cbo.DataSource = idiomasCacheados;

            form.Controls.Add(cbo);

            var dgv = FlexibilizadorFormularios.CreateDataGridView(form, "dgvTraducciones", ptDgv, szDgv);
            dgv.ReadOnly = false;

            cbo.SelectedIndexChanged += (s, e) =>
            {
                var idiomaSeleccionado = cbo.SelectedItem as IIdioma;
                if (idiomaSeleccionado != null)
                    CargarTraducciones(cbo, dgv, svc);
            };

            if (cbo.Items.Count > 0)
                CargarTraducciones(cbo, dgv, svc);

            FlexibilizadorFormularios.CreateButton(form, "btnGuardarTraducciones", ptBtn, new Size(120, 35), "Guardar", (s, e) =>
            {
                var idiomaSeleccionado = cbo.SelectedItem as IIdioma;
                var traduccionesActuales = dgv.DataSource as List<MultidiomaGridDto>;
                if (idiomaSeleccionado == null || traduccionesActuales == null) return;

                var controlIdioma = svc.ObtenerControlesPorIdioma(idiomaSeleccionado.Id);

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
                        svc.ActualizarTraduccion(traduccion);
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
                        svc.CrearTraduccion(traduccion);
                    }
                }

                MessageBox.Show("Traducciones guardadas correctamente.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarTraducciones(cbo, dgv, svc);
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
