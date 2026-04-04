using System;
using System.Drawing;
using System.Windows.Forms;
using IngSoft.ApplicationServices.Dto;
using IngSoft.ApplicationServices.Factory;

namespace IngSoft.UI
{
    internal static class FrmIntegridadDBFlexibilizador
    {
        internal static void CrearVistaIntegridad(Point ptDgv, Size szDgv)
        {
            var parent = FrmIntegridadDB.ActiveForm;
            var integridadDB = ((FrmIntegridadDB)parent).IntegridadDB;

            var lbl = new Label
            {
                Name = "lblIntegridadDBTitulo",
                Text = "INTEGRIDAD BASE DE DATOS",
                Font = new Font("Microsoft Sans Serif", 14.25F),
                AutoSize = true,
                Location = new Point(parent.Width / 2 - 180, 20)
            };
            parent.Controls.Add(lbl);

            var dgv = FlexibilizadorFormularios.CreateDataGridView(parent, "dgvIntegridad", ptDgv, szDgv);
            dgv.DataSource = integridadDB.Errores;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        internal static void CrearBotonRecalcular(Point pt, Size sz)
        {
            var parent = FrmIntegridadDB.ActiveForm;
            var digitoVerificadorServices = ServicesFactory.CreateDigitoVerificadorServices();

            FlexibilizadorFormularios.CreateButton(parent, "btnRecalcular", pt, sz, "Recalcular DV", (s, e) =>
            {
                digitoVerificadorServices.RecaulcularDigitosVerificadores();
                MessageBox.Show("Dígitos verificadores recalculados correctamente.");
                parent.Close();
            });
        }
    }
}
