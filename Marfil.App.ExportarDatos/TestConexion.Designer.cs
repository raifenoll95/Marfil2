namespace Marfil.App.ExportarDatos
{
    partial class TestConexion
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
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.TxtSalida = new System.Windows.Forms.TextBox();
            this.TxtParams = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(32, 81);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "test";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(32, 29);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(228, 20);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = "http://localhost/Marfil/Api/MobileLoginApi";
            // 
            // TxtSalida
            // 
            this.TxtSalida.Location = new System.Drawing.Point(32, 110);
            this.TxtSalida.Multiline = true;
            this.TxtSalida.Name = "TxtSalida";
            this.TxtSalida.Size = new System.Drawing.Size(228, 130);
            this.TxtSalida.TabIndex = 2;
            // 
            // TxtParams
            // 
            this.TxtParams.Location = new System.Drawing.Point(32, 55);
            this.TxtParams.Name = "TxtParams";
            this.TxtParams.Size = new System.Drawing.Size(228, 20);
            this.TxtParams.TabIndex = 3;
            this.TxtParams.Text = "Fkalmacen=0010&Referencialote=TB-170045001";
            // 
            // TestConexion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.TxtParams);
            this.Controls.Add(this.TxtSalida);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Name = "TestConexion";
            this.Text = "TestConexion";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox TxtSalida;
        private System.Windows.Forms.TextBox TxtParams;
    }
}