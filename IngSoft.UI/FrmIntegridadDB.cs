using System;
using System.Windows.Forms;
using IngSoft.ApplicationServices.Dto;

namespace IngSoft.UI
{
    public partial class FrmIntegridadDB : Form
    {
        private ResultadoIntegridad _integridadDB;
        public FrmIntegridadDB(ResultadoIntegridad integridadDB)
        {
            InitializeComponent();
            _integridadDB = integridadDB;
        }

        private void FrmIntegridadDB_Load(object sender, EventArgs e)
        {
            var errores = _integridadDB.Errores;
            dgvIntegridad.DataSource = errores;
            dgvIntegridad.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        }
    }
}
