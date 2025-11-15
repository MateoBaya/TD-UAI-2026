using System;
using System.Linq;
using System.Windows.Forms;
using IngSoft.Abstractions.Multidioma;
using IngSoft.ApplicationServices;
using IngSoft.ApplicationServices.Factory;
using IngSoft.Domain;
using IngSoft.Domain.Enums;
using IngSoft.Domain.Multidioma;
using IngSoft.Services;

namespace IngSoft.UI
{
    public partial class FrmControlDeCambios : Form, IObserver
    {
        private readonly IUsuarioHistoricoServices _usuarioHistoricoServices;
        private readonly IUsuarioServices _usuarioServices;
        private readonly IBitacoraServices _bitacoraServices;
        private readonly IMultidiomaServices _multidiomaServices;
        public FrmControlDeCambios()
        {
            InitializeComponent();
            _usuarioHistoricoServices = ServicesFactory.CreateUsuarioHistoricoServices();
            _usuarioServices = ServicesFactory.CreateUsuarioServices();
            _bitacoraServices = ServicesFactory.CreateBitacoraServices();
            _multidiomaServices = ServicesFactory.CreateMultidiomaServices();

            dgvControlCambios.CellFormatting += DgvControlCambios_CellFormatting;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            if (cboEntidades.SelectedItem is null)
            {
                MessageBox.Show("Seleccione una entidad para buscar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (txtUserName.Text == "")
            {
                MessageBox.Show("Ingrese un nombre de usuario para buscar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ActualizarGridView();
        }

        private void ActualizarGridView()
        {
            var historico = _usuarioHistoricoServices.ObtenerUsuarioHistorico(txtUserName.Text);
            dgvControlCambios.DataSource = historico;
            dgvControlCambios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvControlCambios.Columns["IdUsuario"].Visible = false;
            dgvControlCambios.Columns["Id"].Visible = false;
            dgvControlCambios.Columns["TipoOperacion"].Visible = false;

            var columnaBloqueado = dgvControlCambios.Columns["Bloqueado"];
            if (columnaBloqueado is DataGridViewCheckBoxColumn)
            {
                int displayIndex = columnaBloqueado.DisplayIndex;
                dgvControlCambios.Columns.Remove(columnaBloqueado);

                var nuevaColumna = new DataGridViewTextBoxColumn
                {
                    Name = "Bloqueado",
                    HeaderText = "Bloqueado",
                    DataPropertyName = "Bloqueado",
                    DisplayIndex = displayIndex
                };
                dgvControlCambios.Columns.Add(nuevaColumna);
            }
            else
            {
                dgvControlCambios.Columns["Bloqueado"].HeaderText = "Bloqueado";
            }

            dgvControlCambios.Columns["CantidadIntentos"].HeaderText = "Intentos";
            dgvControlCambios.Columns["UserName"].HeaderText = "Usuario";
            dgvControlCambios.Columns["FechaModificacion"].HeaderText = "Fecha";
            dgvControlCambios.Columns["UsuarioModificador"].HeaderText = "Modificado Por";
        }

        private void DgvControlCambios_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Verificar si es la columna Bloqueado
            if (dgvControlCambios.Columns[e.ColumnIndex].Name == "Bloqueado" && e.Value != null)
            {
                if (e.Value is bool bloqueado)
                {
                    e.Value = bloqueado ? "Sí" : "No";
                    e.FormattingApplied = true;
                }
            }
        }

        private void dgvControlCambios_SelectionChanged(object sender, EventArgs e)
        {
            btnRestaurar.Enabled = dgvControlCambios.SelectedRows.Count > 0;


        }
        private void btnRestaurar_Click(object sender, EventArgs e)
        {
            _usuarioServices.SetRegistradoBitacora(RegistrarEnBitacora);

            var usuarioHistorico = ObtenerUsuarioHistoricoSeleccionado();

            if (usuarioHistorico == null)
            {
                MessageBox.Show("Seleccione un registro para restaurar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            var usuarioARevertir = new Usuario
            {
                IdUsuario = usuarioHistorico.IdUsuario,
                Nombre = usuarioHistorico.Nombre,
                Apellido = usuarioHistorico.Apellido,
                Email = usuarioHistorico.Email,
                UserName = usuarioHistorico.UserName,
                Bloqueado = usuarioHistorico.Bloqueado,
                CantidadIntentos = usuarioHistorico.CantidadIntentos,
                FechaEliminado = usuarioHistorico.FechaEliminado
            };
            _usuarioServices.ModificarUsuario(usuarioARevertir);
            ActualizarGridView();
        }
        private UsuarioHistorico ObtenerUsuarioHistoricoSeleccionado()
        {
            if (dgvControlCambios.SelectedRows.Count > 0)
            {
                var filaSeleccionada = dgvControlCambios.SelectedRows[0];
                return filaSeleccionada.DataBoundItem as UsuarioHistorico;
            }
            return null;
        }

        private void RegistrarEnBitacora(Usuario usuario, string descripcion, string origen, TipoEvento tipoEvento)
        {
            var bitacora = new Bitacora
            {
                Id = Guid.NewGuid(),
                Usuario = usuario,
                Fecha = DateTime.Now,
                Descripcion = descripcion,
                Origen = origen,
                TipoEvento = tipoEvento
            };
            _bitacoraServices.GuardarBitacora(bitacora);
        }

        private void FrmControlDeCambios_Load(object sender, EventArgs e)
        {
            AplicarIdiomaActual();
            SuscribirseAIdiomaActual();
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

        public void Actualizar()
        {
            if (MultidiomaManager.GetIdioma() != null)
            {
                var controles = _multidiomaServices.ObtenerControlesPorIdioma(MultidiomaManager.GetIdioma().Id);
                MultidiomaManager.CambiarIdiomaControles(this, controles.Cast<IControlIdioma>().ToList());
            }
        }
    }
}
