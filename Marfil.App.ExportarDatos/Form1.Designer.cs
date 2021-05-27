namespace Marfil.App.ExportarDatos
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
            this.BtnRefrescar = new System.Windows.Forms.Button();
            this.TxtEntidades = new System.Windows.Forms.ComboBox();
            this.DgData = new System.Windows.Forms.DataGridView();
            this.BtnExportar = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.TxtCatalogo = new System.Windows.Forms.TextBox();
            this.TxtHost = new System.Windows.Forms.TextBox();
            this.TxtPassword = new System.Windows.Forms.TextBox();
            this.TxtUsuario = new System.Windows.Forms.TextBox();
            this.TxtEmpresa = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.DgData)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnRefrescar
            // 
            this.BtnRefrescar.Location = new System.Drawing.Point(809, 108);
            this.BtnRefrescar.Name = "BtnRefrescar";
            this.BtnRefrescar.Size = new System.Drawing.Size(75, 23);
            this.BtnRefrescar.TabIndex = 0;
            this.BtnRefrescar.Text = "Refrescar";
            this.BtnRefrescar.UseVisualStyleBackColor = true;
            this.BtnRefrescar.Click += new System.EventHandler(this.BtnRefrescar_Click);
            // 
            // TxtEntidades
            // 
            this.TxtEntidades.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TxtEntidades.FormattingEnabled = true;
            this.TxtEntidades.Location = new System.Drawing.Point(12, 108);
            this.TxtEntidades.Name = "TxtEntidades";
            this.TxtEntidades.Size = new System.Drawing.Size(791, 21);
            this.TxtEntidades.TabIndex = 1;
            // 
            // DgData
            // 
            this.DgData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgData.Location = new System.Drawing.Point(12, 135);
            this.DgData.Name = "DgData";
            this.DgData.ReadOnly = true;
            this.DgData.Size = new System.Drawing.Size(872, 297);
            this.DgData.TabIndex = 2;
            // 
            // BtnExportar
            // 
            this.BtnExportar.Location = new System.Drawing.Point(809, 438);
            this.BtnExportar.Name = "BtnExportar";
            this.BtnExportar.Size = new System.Drawing.Size(75, 23);
            this.BtnExportar.TabIndex = 3;
            this.BtnExportar.Text = "Exportar";
            this.BtnExportar.UseVisualStyleBackColor = true;
            this.BtnExportar.Click += new System.EventHandler(this.BtnExportar_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.TxtEmpresa);
            this.groupBox1.Controls.Add(this.TxtPassword);
            this.groupBox1.Controls.Add(this.TxtUsuario);
            this.groupBox1.Controls.Add(this.TxtHost);
            this.groupBox1.Controls.Add(this.TxtCatalogo);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(871, 89);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Configuración de conexión";
            // 
            // TxtCatalogo
            // 
            this.TxtCatalogo.Location = new System.Drawing.Point(45, 30);
            this.TxtCatalogo.Name = "TxtCatalogo";
            this.TxtCatalogo.Size = new System.Drawing.Size(199, 20);
            this.TxtCatalogo.TabIndex = 0;
            this.TxtCatalogo.Text = "marfilestable";
            // 
            // TxtHost
            // 
            this.TxtHost.Location = new System.Drawing.Point(45, 56);
            this.TxtHost.Name = "TxtHost";
            this.TxtHost.Size = new System.Drawing.Size(199, 20);
            this.TxtHost.TabIndex = 1;
            this.TxtHost.Text = "192.168.223.210";
            // 
            // TxtPassword
            // 
            this.TxtPassword.Location = new System.Drawing.Point(329, 56);
            this.TxtPassword.Name = "TxtPassword";
            this.TxtPassword.Size = new System.Drawing.Size(199, 20);
            this.TxtPassword.TabIndex = 3;
            this.TxtPassword.Text = "Tot.2020";
            // 
            // TxtUsuario
            // 
            this.TxtUsuario.Location = new System.Drawing.Point(329, 30);
            this.TxtUsuario.Name = "TxtUsuario";
            this.TxtUsuario.Size = new System.Drawing.Size(199, 20);
            this.TxtUsuario.TabIndex = 2;
            this.TxtUsuario.Text = "sa";
            // 
            // TxtEmpresa
            // 
            this.TxtEmpresa.Location = new System.Drawing.Point(652, 30);
            this.TxtEmpresa.Name = "TxtEmpresa";
            this.TxtEmpresa.Size = new System.Drawing.Size(199, 20);
            this.TxtEmpresa.TabIndex = 4;
            this.TxtEmpresa.Text = "0001";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "BD:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "IP:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(290, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Pass:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(290, 33);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "User:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(567, 33);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Cod. Empresa :";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(898, 469);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.BtnExportar);
            this.Controls.Add(this.DgData);
            this.Controls.Add(this.TxtEntidades);
            this.Controls.Add(this.BtnRefrescar);
            this.Name = "Form1";
            this.Text = "Exportador";
            ((System.ComponentModel.ISupportInitialize)(this.DgData)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BtnRefrescar;
        private System.Windows.Forms.ComboBox TxtEntidades;
        private System.Windows.Forms.DataGridView DgData;
        private System.Windows.Forms.Button BtnExportar;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox TxtCatalogo;
        private System.Windows.Forms.TextBox TxtHost;
        private System.Windows.Forms.TextBox TxtPassword;
        private System.Windows.Forms.TextBox TxtUsuario;
        private System.Windows.Forms.TextBox TxtEmpresa;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}

