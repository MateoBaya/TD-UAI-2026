namespace IngSoft.UI
{
    partial class FrmIntegridadDB
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblIntegridadDBTitulo = new System.Windows.Forms.Label();
            this.dgvIntegridad = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvIntegridad)).BeginInit();
            this.SuspendLayout();
            // 
            // lblIntegridadDBTitulo
            // 
            this.lblIntegridadDBTitulo.AutoSize = true;
            this.lblIntegridadDBTitulo.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIntegridadDBTitulo.Location = new System.Drawing.Point(329, 31);
            this.lblIntegridadDBTitulo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblIntegridadDBTitulo.Name = "lblIntegridadDBTitulo";
            this.lblIntegridadDBTitulo.Size = new System.Drawing.Size(356, 29);
            this.lblIntegridadDBTitulo.TabIndex = 0;
            this.lblIntegridadDBTitulo.Text = "INTEGRIDAD BASE DE DATOS";
            // 
            // dgvIntegridad
            // 
            this.dgvIntegridad.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvIntegridad.Location = new System.Drawing.Point(64, 101);
            this.dgvIntegridad.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dgvIntegridad.Name = "dgvIntegridad";
            this.dgvIntegridad.RowHeadersWidth = 51;
            this.dgvIntegridad.Size = new System.Drawing.Size(919, 394);
            this.dgvIntegridad.TabIndex = 1;
            // 
            // FrmIntegridadDB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1067, 554);
            this.Controls.Add(this.dgvIntegridad);
            this.Controls.Add(this.lblIntegridadDBTitulo);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FrmIntegridadDB";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmIntegridadDB";
            this.Load += new System.EventHandler(this.FrmIntegridadDB_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvIntegridad)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblIntegridadDBTitulo;
        private System.Windows.Forms.DataGridView dgvIntegridad;
    }
}