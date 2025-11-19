namespace IngSoft.UI
{
    partial class FrmBackUp
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
            this.lblBackUp = new System.Windows.Forms.Label();
            this.dgvBackup = new System.Windows.Forms.DataGridView();
            this.btnCrearBackup = new System.Windows.Forms.Button();
            this.btnRestaurar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBackup)).BeginInit();
            this.SuspendLayout();
            // 
            // lblBackUp
            // 
            this.lblBackUp.AutoSize = true;
            this.lblBackUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBackUp.Location = new System.Drawing.Point(324, 43);
            this.lblBackUp.Name = "lblBackUp";
            this.lblBackUp.Size = new System.Drawing.Size(219, 29);
            this.lblBackUp.TabIndex = 0;
            this.lblBackUp.Text = "Gestion De Backup";
            // 
            // dgvBackup
            // 
            this.dgvBackup.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBackup.Location = new System.Drawing.Point(49, 114);
            this.dgvBackup.Name = "dgvBackup";
            this.dgvBackup.RowHeadersWidth = 51;
            this.dgvBackup.RowTemplate.Height = 24;
            this.dgvBackup.Size = new System.Drawing.Size(683, 390);
            this.dgvBackup.TabIndex = 1;
            // 
            // btnCrearBackup
            // 
            this.btnCrearBackup.Location = new System.Drawing.Point(761, 114);
            this.btnCrearBackup.Name = "btnCrearBackup";
            this.btnCrearBackup.Size = new System.Drawing.Size(123, 40);
            this.btnCrearBackup.TabIndex = 2;
            this.btnCrearBackup.Text = "Crear Backup";
            this.btnCrearBackup.UseVisualStyleBackColor = true;
            this.btnCrearBackup.Click += new System.EventHandler(this.btnCrearBackup_Click);
            // 
            // btnRestaurar
            // 
            this.btnRestaurar.Location = new System.Drawing.Point(761, 182);
            this.btnRestaurar.Name = "btnRestaurar";
            this.btnRestaurar.Size = new System.Drawing.Size(123, 40);
            this.btnRestaurar.TabIndex = 3;
            this.btnRestaurar.Text = "Restaurar";
            this.btnRestaurar.UseVisualStyleBackColor = true;
            this.btnRestaurar.Click += new System.EventHandler(this.btnRestaurar_Click);
            // 
            // FrmBackUp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(918, 530);
            this.Controls.Add(this.btnRestaurar);
            this.Controls.Add(this.btnCrearBackup);
            this.Controls.Add(this.dgvBackup);
            this.Controls.Add(this.lblBackUp);
            this.Name = "FrmBackUp";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmBackUp";
            this.Load += new System.EventHandler(this.FrmBackUp_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBackup)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblBackUp;
        private System.Windows.Forms.DataGridView dgvBackup;
        private System.Windows.Forms.Button btnCrearBackup;
        private System.Windows.Forms.Button btnRestaurar;
    }
}