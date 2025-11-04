using System;
using System.Collections.Generic;
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
    public partial class Modificar_Idioma : Form, IObserver
    {
        private readonly IMultidiomaServices _multidiomaServices;
        public Modificar_Idioma()
        {
            InitializeComponent();
            _multidiomaServices = ServicesFactory.CreateMultidiomaServices();
        }

        private void Modificar_Idioma_Load(object sender, EventArgs e)
        {
            CargarIdiomas();
            SuscribirseAIdiomaActual();
            AplicarIdiomaActual();
        }
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            var idiomaSeleccionado = cboIdiomas.SelectedItem as IIdioma;
            var traduccionesActuales = dgvTraducciones.DataSource as List<MultidiomaGridDto>;
            var controlIdioma = _multidiomaServices.ObtenerControlesPorIdioma(idiomaSeleccionado.Id);

            foreach (var traduccionDto in traduccionesActuales)
            {
                if (controlIdioma.Count() > 0)
                {
                    //var traduccionExistente = _multidiomaServices.ObtenerTraduccionPorIdiomaYControlIdioma(idiomaSeleccionado.Id, traduccionDto.IdControlIdioma);
                    //traduccionExistente.TextoTraducido = traduccionDto.TextoTraducido;
                    var traduccion = new Traduccion
                    {
                        IdIdioma = idiomaSeleccionado.Id,
                        IdControlIdioma = traduccionDto.IdControlIdioma,
                        TextoTraducido = traduccionDto.TextoTraducido
                    };

                    _multidiomaServices.ActualizarTraduccion(traduccion);
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
                    _multidiomaServices.CrearTraduccion(traduccion);
                }
            }

            MessageBox.Show("Traducciones guardadas correctamente.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            cboIdiomas_SelectedIndexChanged(sender, e);
        }
        public void Actualizar()
        {
            if (MultidiomaManager.GetIdioma() != null)
            {
                var controles = _multidiomaServices.ObtenerControlesPorIdioma(MultidiomaManager.GetIdioma().Id);
                MultidiomaManager.CambiarIdiomaControles(this, controles.Cast<IControlIdioma>().ToList());
            }
        }
        private void CargarIdiomas()
        {
            var idiomas = _multidiomaServices.ObtenerIdiomas()
                .Cast<IIdioma>().ToList();

            var idiomasCacheados = MultidiomaManager.ObtenerIdiomasCache(idiomas);

            cboIdiomas.DataSource = idiomasCacheados;
            cboIdiomas.DisplayMember = "Nombre";
            cboIdiomas.ValueMember = null;
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

        private void cboIdiomas_SelectedIndexChanged(object sender, EventArgs e)
        {
            var idiomaSeleccionado = cboIdiomas.SelectedItem as IIdioma;

            var idiomaPorDefecto = _multidiomaServices.ObtenerIdiomaPorDefecto();
            var traduccionesPorDefecto = _multidiomaServices.ObtenerControlesPorIdioma(idiomaPorDefecto.Id);
            var traducciones = _multidiomaServices.ObtenerControlesPorIdioma(idiomaSeleccionado.Id);

            var listaMultidioma = new List<MultidiomaGridDto>();

            foreach (var traduccionDefecto in traduccionesPorDefecto)
            {
                var traduccionExistente = traducciones.FirstOrDefault(t => t.NombreControl == traduccionDefecto.NombreControl);

                var dto = new MultidiomaGridDto
                {
                    NombreControl = traduccionDefecto.NombreControl,
                    TextoPorDefecto = traduccionDefecto.TextoTraducido,
                    IdControlIdioma = traduccionDefecto.Id
                };

                if (traduccionExistente != null)
                {
                    dto.TextoTraducido = traduccionExistente.TextoTraducido;
                }
                else
                {
                    dto.TextoTraducido = "[" + traduccionDefecto.TextoTraducido + "]";
                }

                listaMultidioma.Add(dto);
            }

            dgvTraducciones.DataSource = listaMultidioma;
            ConfigurarGridview();
        }
        private void ConfigurarGridview()
        {
            if (dgvTraducciones.Columns.Count > 0)
            {
                // Configurar encabezados
                dgvTraducciones.Columns["NombreControl"].HeaderText = "Control";
                dgvTraducciones.Columns["TextoPorDefecto"].HeaderText = "Texto Original";
                dgvTraducciones.Columns["TextoTraducido"].HeaderText = "Traducción";

                dgvTraducciones.Columns["IdControlIdioma"].Visible = false;

                // Configurar columnas de solo lectura
                dgvTraducciones.Columns["NombreControl"].ReadOnly = true;
                dgvTraducciones.Columns["TextoPorDefecto"].ReadOnly = true;
                dgvTraducciones.Columns["TextoTraducido"].ReadOnly = false; // Esta columna es editable

                // Ajustar el ancho de las columnas
                dgvTraducciones.Columns["NombreControl"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dgvTraducciones.Columns["TextoPorDefecto"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvTraducciones.Columns["TextoTraducido"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            }
        }

        private void dgvTraducciones_DataBindingComplete_1(object sender, DataGridViewBindingCompleteEventArgs e)
        {

        }
    }
}
