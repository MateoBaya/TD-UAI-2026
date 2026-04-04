namespace IngSoft.UI.Multidioma
{
    partial class FrmMultidiomas
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.crearIdiomaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modificarIdiomaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.crearIdiomaToolStripMenuItem,
            this.modificarIdiomaToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(932, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // crearIdiomaToolStripMenuItem
            // 
            this.crearIdiomaToolStripMenuItem.Name = "crearIdiomaToolStripMenuItem";
            this.crearIdiomaToolStripMenuItem.Size = new System.Drawing.Size(109, 24);
            this.crearIdiomaToolStripMenuItem.Text = "Crear Idioma";
            this.crearIdiomaToolStripMenuItem.Click += new System.EventHandler(this.crearIdiomaToolStripMenuItem_Click);
            // 
            // modificarIdiomaToolStripMenuItem
            // 
            this.modificarIdiomaToolStripMenuItem.Name = "modificarIdiomaToolStripMenuItem";
            this.modificarIdiomaToolStripMenuItem.Size = new System.Drawing.Size(138, 24);
            this.modificarIdiomaToolStripMenuItem.Text = "Modificar Idioma";
            this.modificarIdiomaToolStripMenuItem.Click += new System.EventHandler(this.modificarIdiomaToolStripMenuItem_Click);
            // 
            // FrmMultidiomas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(932, 546);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FrmMultidiomas";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmMultidiomas";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmMultidiomas_FormClosed);
            this.Load += new System.EventHandler(this.FrmMultidiomas_Load);
            this.Shown += new System.EventHandler(this.FrmMultidiomas_Shown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem crearIdiomaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modificarIdiomaToolStripMenuItem;
    }
}