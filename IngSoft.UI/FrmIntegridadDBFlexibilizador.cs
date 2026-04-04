using System;
using System.Drawing;
using System.Windows.Forms;
using IngSoft.ApplicationServices.Dto;
using IngSoft.Services;

namespace IngSoft.UI
{
    internal static class FrmIntegridadDBFlexibilizador
    {
        public static void CrearVistaIntegridad(FrmIntegridadDB form, ResultadoIntegridad resultado, Point ptDgv, Size szDgv)
        {
            var lbl = new Label
            {
                Name = "lblIntegridadDBTitulo",
                Text = "INTEGRIDAD BASE DE DATOS",
                Font = new Font("Microsoft Sans Serif", 14.25F),
                AutoSize = true,
                Location = new Point(form.Width / 2 - 180, 20)
            };
            form.Controls.Add(lbl);

            var dgv = FlexibilizadorFormularios.CreateDataGridView(form, "dgvIntegridad", ptDgv, szDgv);
            dgv.DataSource = resultado.Errores;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        public static void CrearBotonRecalcular(FrmIntegridadDB form, IDigitoVerificadorServices svc, Point pt, Size sz)
        {
            FlexibilizadorFormularios.CreateButton(form, "btnRecalcular", pt, sz, "Recalcular DV", (s, e) =>
            {
                svc.RecaulcularDigitosVerificadores();
                MessageBox.Show("Dígitos verificadores recalculados correctamente.");
                form.Close();
            });
        }
    }
}
