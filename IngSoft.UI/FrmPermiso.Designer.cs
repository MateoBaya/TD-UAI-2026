namespace IngSoft.UI
{
    partial class FrmPermiso
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
            this.agregarPermisoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modificarPermisoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eliminarPermisoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.asignarPermisoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.agregarPermisoToolStripMenuItem,
            this.modificarPermisoToolStripMenuItem,
            this.eliminarPermisoToolStripMenuItem,
            this.asignarPermisoToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // agregarPermisoToolStripMenuItem
            // 
            this.agregarPermisoToolStripMenuItem.Name = "agregarPermisoToolStripMenuItem";
            this.agregarPermisoToolStripMenuItem.Size = new System.Drawing.Size(133, 24);
            this.agregarPermisoToolStripMenuItem.Text = "Agregar Permiso";
            this.agregarPermisoToolStripMenuItem.Click += new System.EventHandler(this.verTodosToolStripMenuItem_Click);
            // 
            // modificarPermisoToolStripMenuItem
            // 
            this.modificarPermisoToolStripMenuItem.Name = "modificarPermisoToolStripMenuItem";
            this.modificarPermisoToolStripMenuItem.Size = new System.Drawing.Size(143, 24);
            this.modificarPermisoToolStripMenuItem.Text = "Modificar Permiso";
            this.modificarPermisoToolStripMenuItem.Click += new System.EventHandler(this.modificarPermisoToolStripMenuItem_Click);
            // 
            // eliminarPermisoToolStripMenuItem
            // 
            this.eliminarPermisoToolStripMenuItem.Name = "eliminarPermisoToolStripMenuItem";
            this.eliminarPermisoToolStripMenuItem.Size = new System.Drawing.Size(133, 24);
            this.eliminarPermisoToolStripMenuItem.Text = "Eliminar Permiso";
            this.eliminarPermisoToolStripMenuItem.Click += new System.EventHandler(this.eliminarPermisoToolStripMenuItem_Click);
            // 
            // asignarPermisoToolStripMenuItem
            // 
            this.asignarPermisoToolStripMenuItem.Name = "asignarPermisoToolStripMenuItem";
            this.asignarPermisoToolStripMenuItem.Size = new System.Drawing.Size(129, 24);
            this.asignarPermisoToolStripMenuItem.Text = "Asignar Permiso";
            this.asignarPermisoToolStripMenuItem.Click += new System.EventHandler(this.asignarPermisoToolStripMenuItem_Click);
            // 
            // FrmPermiso
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FrmPermiso";
            this.Text = "FrmPermisos";
            this.Shown += new System.EventHandler(this.FrmPermiso_Shown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem agregarPermisoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modificarPermisoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem eliminarPermisoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem asignarPermisoToolStripMenuItem;
    }
}