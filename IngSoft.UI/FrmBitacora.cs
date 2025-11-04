using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using IngSoft.Abstractions.Multidioma;
using IngSoft.ApplicationServices;
using IngSoft.ApplicationServices.Factory;
using IngSoft.Domain;
using IngSoft.Domain.Multidioma;
using IngSoft.Services;
using IngSoft.UI.DTOs;
using IngSoft.Services;

namespace IngSoft.UI
{
    public partial class FrmBitacora : Form, IObserver
    {
        private readonly IBitacoraServices _bitacoraServices;
        private readonly IMultidiomaServices _multidiomaServices;
        //private readonly IBitacoraServices _bitacoraServices = SingleInstancesManager.Instance.ObtenerInstancia<IBitacoraServices>();
        //private readonly IMultidiomaServices _multidiomaServices = SingleInstancesManager.Instance.ObtenerInstancia<IMultidiomaServices>();
        private List<Bitacora> _bitacoras;
        public FrmBitacora()
        {
            InitializeComponent();
            _bitacoraServices = ServicesFactory.CreateBitacoraServices();
            _multidiomaServices = ServicesFactory.CreateMultidiomaServices();
        }

        private void FrmBitacora_Load(object sender, EventArgs e)
        {
            CargarBitacora();
            AplicarIdiomaActual();
            SuscribirseAIdiomaActual();
        }

        private void txtBusquedaBitacora_TextChanged(object sender, EventArgs e)
        {
            var filtro = txtBusquedaBitacora.Text.Trim();

            if (filtro.Length > 3)
            {
                CargarBitacora(filtro);
            }
            else 
            {
                CargarBitacora();
            }
        }

        private void cboTipoEvento_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedTipoEvento = cboTipoEvento.SelectedItem.ToString();

            if(selectedTipoEvento != "Todos") 
            {
                var filteredBitacoras = _bitacoras.Where(b => b.TipoEvento.ToString() == selectedTipoEvento).ToList();

                gridBitacora.DataSource = BuildBitacorasDataGridView(filteredBitacoras);
            }
            else 
            {
                CargarBitacora();
            }            
        }

        private void dtpFecha_ValueChanged(object sender, EventArgs e)
        {
            dtpFechaHasta.Enabled = true;

            var fechaDesde = dtpFechaDesde.Value.Date;
            var fechaHasta = dtpFechaHasta.Value.Date;

            if (fechaHasta < fechaDesde)
            {
                MessageBox.Show("La fecha hasta no puede ser menor a la fecha desde.", "Error de fecha", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dtpFechaHasta.Value = fechaDesde;
                CargarBitacora();
            }

            var filteredBitacoras = _bitacoras.Where(b => b.Fecha.Date >= fechaDesde && b.Fecha.Date <= fechaHasta).ToList();
            gridBitacora.DataSource = BuildBitacorasDataGridView(filteredBitacoras);
        }

        private void dtpFechaHasta_ValueChanged(object sender, EventArgs e)
        {
            var fechaDesde = dtpFechaDesde.Value.Date;
            var fechaHasta = dtpFechaHasta.Value.Date;

            if(fechaHasta < fechaDesde)
            {
                MessageBox.Show("La fecha hasta no puede ser menor a la fecha desde.", "Error de fecha", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dtpFechaHasta.Value = fechaDesde;
                CargarBitacora();
            }

            var filteredBitacoras = _bitacoras.Where(b => b.Fecha.Date >= fechaDesde && b.Fecha.Date <= fechaHasta).ToList();
            gridBitacora.DataSource = BuildBitacorasDataGridView(filteredBitacoras);
        }

        private void chkFiltroDate_CheckedChanged(object sender, EventArgs e)
        {
            if (chkFiltroDate.Checked)
            {
                dtpFechaDesde.Enabled = true;
            }
            else
            {
                dtpFechaDesde.Enabled = false;
                CargarBitacora();
            }
        }

        private void CargarBitacora(string filtro = null)
        {
            if (filtro != null)
            {
                _bitacoras = _bitacoraServices.ObtenerBitacorasFiltradas(filtro);
            }
            else
            {
                _bitacoras = _bitacoraServices.ObtenerBitacoras();
            }

            gridBitacora.DataSource = BuildBitacorasDataGridView(_bitacoras);
        }

        private List<BitacoraGridDto> BuildBitacorasDataGridView(List<Bitacora> bitacoras) 
        {
            return bitacoras.Select(b => new BitacoraGridDto
            {
                Fecha = b.Fecha,
                Descripcion = b.Descripcion,
                Origen = b.Origen,
                TipoEvento = b.TipoEvento.ToString(),
                Usuario = b.Usuario.UserName
            })
            .OrderByDescending(b => b.Fecha)
            .ToList();
        }

        private void gridBitacora_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            gridBitacora.Columns["Descripcion"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }
        public void Actualizar()
        {
            if(MultidiomaManager.GetIdioma() != null) 
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
