using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using IngSoft.ApplicationServices;
using IngSoft.ApplicationServices.Factory;
using IngSoft.Domain;

namespace IngSoft.UI
{
    internal static class FrmBackUpFlexibilizador
    {
        internal static void CrearVistaBackup(
            Point ptTitle,
            Point ptDgv,
            Size szDgv,
            Point ptBtnCrear,
            Point ptBtnRestaurar,
            Size szBtn)
        {
            var parent = FrmBackUp.ActiveForm;
            IBackupServices backupServices = ServicesFactory.CreateBackupServices();

            var lbl = new Label
            {
                Name = "lblBackUp",
                Text = "Gestion De Backup",
                Font = new Font("Microsoft Sans Serif", 13.8F),
                AutoSize = true,
                Location = ptTitle
            };
            parent.Controls.Add(lbl);

            var dgv = FlexibilizadorFormularios.CreateDataGridView(parent, "dgvBackup", ptDgv, szDgv);
            dgv.AutoGenerateColumns = false;
            dgv.Columns.Clear();
            dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NombreArchivo", HeaderText = "Nombre del Archivo", Width = 250 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "FechaCreacion", HeaderText = "Fecha de Creación", Width = 150, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm:ss" } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TamanoFormateado", HeaderText = "Tamaño", Width = 100 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "UsuarioCreador", HeaderText = "Usuario", Width = 150 });

            CargarHistorialBackups(dgv, backupServices);

            FlexibilizadorFormularios.CreateButton(parent, "btnCrearBackup", ptBtnCrear, szBtn, "Crear Backup", (s, e) =>
            {
                try
                {
                    var resultado = MessageBox.Show(
                        "¿Está seguro de crear un backup?\n\nSe guardará en la carpeta de backups de SQL Server.",
                        "Confirmar Backup",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (resultado == DialogResult.Yes)
                    {
                        var form = FrmBackUp.ActiveForm;
                        form.Cursor = Cursors.WaitCursor;
                        var btn = form.Controls.Find("btnCrearBackup", true).FirstOrDefault() as Button;
                        if (btn != null) btn.Enabled = false;

                        backupServices.CrearBackup();

                        MessageBox.Show("Backup creado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        var dgvActual = form.Controls.Find("dgvBackup", true).FirstOrDefault() as DataGridView;
                        if (dgvActual != null) CargarHistorialBackups(dgvActual, backupServices);

                        form.Cursor = Cursors.Default;
                        if (btn != null) btn.Enabled = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al crear el backup: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    var form = FrmBackUp.ActiveForm;
                    form.Cursor = Cursors.Default;
                    var btn = form.Controls.Find("btnCrearBackup", true).FirstOrDefault() as Button;
                    if (btn != null) btn.Enabled = true;
                }
            });

            FlexibilizadorFormularios.CreateButton(parent, "btnRestaurar", ptBtnRestaurar, szBtn, "Restaurar", (s, e) =>
            {
                try
                {
                    var form = FrmBackUp.ActiveForm;
                    var dgvActual = form.Controls.Find("dgvBackup", true).FirstOrDefault() as DataGridView;
                    if (dgvActual == null || dgvActual.SelectedRows.Count == 0)
                    {
                        MessageBox.Show("Por favor, seleccione un backup del historial para restaurar.",
                            "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    var backupSeleccionado = dgvActual.SelectedRows[0].DataBoundItem as Backups;
                    if (backupSeleccionado == null) return;

                    var resultado = MessageBox.Show(
                        $"ADVERTENCIA: Esta operación sobrescribirá la base de datos actual.\n\n" +
                        $"Archivo: {backupSeleccionado.NombreArchivo}\n" +
                        $"Fecha: {backupSeleccionado.FechaCreacion:dd/MM/yyyy HH:mm:ss}\n" +
                        $"Tamaño: {backupSeleccionado.TamanoFormateado}\n\n" +
                        $"¿Está seguro de continuar?",
                        "Confirmar Restauración",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (resultado == DialogResult.Yes)
                    {
                        form.Cursor = Cursors.WaitCursor;
                        var btn = form.Controls.Find("btnRestaurar", true).FirstOrDefault() as Button;
                        if (btn != null) btn.Enabled = false;

                        svc.RestaurarBackup(backupSeleccionado.RutaCompleta);
                        
                        MessageBox.Show("Backup restaurado exitosamente.\n\n", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        form.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al restaurar el backup: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    var form = FrmBackUp.ActiveForm;
                    form.Cursor = Cursors.Default;
                    var btn = form.Controls.Find("btnRestaurar", true).FirstOrDefault() as Button;
                    if (btn != null) btn.Enabled = true;
                }
            });
        }

        private static void CargarHistorialBackups(DataGridView dgv, IBackupServices svc)
        {
            try
            {
                var backups = svc.ObtenerHistorialBackups();
                dgv.DataSource = backups.OrderByDescending(b => b.FechaCreacion).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el historial de backups: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
