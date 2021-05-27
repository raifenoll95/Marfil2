namespace Marfil.App.TestGenerator
{
    partial class Form1
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
            this.BtnClientes = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // BtnClientes
            // 
            this.BtnClientes.Location = new System.Drawing.Point(22, 31);
            this.BtnClientes.Name = "BtnClientes";
            this.BtnClientes.Size = new System.Drawing.Size(75, 23);
            this.BtnClientes.TabIndex = 0;
            this.BtnClientes.Text = "Test clientes";
            this.BtnClientes.UseVisualStyleBackColor = true;
            this.BtnClientes.Click += new System.EventHandler(this.BtnClientes_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(616, 299);
            this.Controls.Add(this.BtnClientes);
            this.Name = "Form1";
            this.Text = "Generar test";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BtnClientes;
    }
}

