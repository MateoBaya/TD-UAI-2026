using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using IngSoft.Abstractions.Multidioma;
using IngSoft.ApplicationServices;
using IngSoft.ApplicationServices.Factory;
using IngSoft.Domain;
using IngSoft.Domain.Multidioma;
using IngSoft.Services;

namespace IngSoft.UI
{
    public partial class FrmBackUp : Form , IObserver
    {
        private readonly IBackupServices _backupServices;
        private readonly IMultidiomaServices _multidiomaServices;
        private List<Backups> _backups;

        public FrmBackUp()
        {
            InitializeComponent();
            _backupServices = ServicesFactory.CreateBackupServices();
            _multidiomaServices = ServicesFactory.CreateMultidiomaServices();
        }

        private void FrmBackUp_Load(object sender, EventArgs e)
        {
            ConfigurarDataGridView();
            CargarHistorialBackups();
            SuscribirseAIdiomaActual();
            AplicarIdiomaActual();
        }

        private void btnCrearBackup_Click(object sender, EventArgs e)
        {
            try
            {
                var resultado = MessageBox.Show(
                    "¿Está seguro de crear un backup?\n\n" +
                    "Se guardará en la carpeta de backups de SQL Server.",
                    "Confirmar Backup",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (resultado == DialogResult.Yes)
                {
                    Cursor = Cursors.WaitCursor;
                    btnCrearBackup.Enabled = false;

                    _backupServices.CrearBackup();

                    MessageBox.Show("Backup creado exitosamente.",
                        "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    CargarHistorialBackups();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear el backup: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
                btnCrearBackup.Enabled = true;
            }
        }

        private void btnRestaurar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvBackup.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Por favor, seleccione un backup del historial para restaurar.",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var backupSeleccionado = dgvBackup.SelectedRows[0].DataBoundItem as Backups;
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
                    Cursor = Cursors.WaitCursor;
                    btnRestaurar.Enabled = false;

                    _backupServices.RestaurarBackup(backupSeleccionado.RutaCompleta);

                    MessageBox.Show(
                        "Backup restaurado exitosamente.\n\n",
                        "Éxito",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al restaurar el backup: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
                btnRestaurar.Enabled = true;
            }
        }

        private void ConfigurarDataGridView()
        {
            dgvBackup.AutoGenerateColumns = false;
            dgvBackup.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBackup.MultiSelect = false;
            dgvBackup.ReadOnly = true;

            dgvBackup.Columns.Clear();
            dgvBackup.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "NombreArchivo",
                HeaderText = "Nombre del Archivo",
                Width = 250
            });
            dgvBackup.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "FechaCreacion",
                HeaderText = "Fecha de Creación",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm:ss" }
            });
            dgvBackup.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TamanoFormateado",
                HeaderText = "Tamaño",
                Width = 100
            });
            dgvBackup.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "UsuarioCreador",
                HeaderText = "Usuario",
                Width = 150
            });
        }

        private void CargarHistorialBackups()
        {
            try
            {
                _backups = _backupServices.ObtenerHistorialBackups();
                dgvBackup.DataSource = _backups.OrderByDescending(b => b.FechaCreacion).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el historial de backups: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Actualizar()
        {
            if (MultidiomaManager.GetIdioma() != null)
            {
                var controles = _multidiomaServices.ObtenerControlesPorIdioma(MultidiomaManager.GetIdioma().Id);
                MultidiomaManager.CambiarIdiomaControles(this, controles.Cast<IControlIdioma>().ToList());
            }
        }

        private void SuscribirseAIdiomaActual()
        {
            var idioma = MultidiomaManager.GetIdioma();

            if (idioma != null)
            {
                // Asegurarse de que sea la instancia cacheada
                if (idioma is Idioma idiomaConcreto)
                {
                    var idiomaCacheado = MultidiomaManager.ObtenerIdiomaCache(idiomaConcreto);
                    idiomaCacheado.Suscribir(this);
                }
                else
                {
                    idioma.Suscribir(this);
                }
            }
        }
        private void AplicarIdiomaActual()
        {
            // Aplicar el idioma actual al formulario
            if (MultidiomaManager.GetIdioma() != null)
            {
                var controles = _multidiomaServices.ObtenerControlesPorIdioma(MultidiomaManager.GetIdioma().Id)
                    .Cast<IControlIdioma>().ToList();
                MultidiomaManager.CambiarIdiomaControles(this, controles);
            }
        }
    }
}
