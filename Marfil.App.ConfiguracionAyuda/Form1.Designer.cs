namespace Marfil.App.ConfiguracionAyuda
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
            this.btnPublicar = new System.Windows.Forms.Button();
            this.dgVista = new System.Windows.Forms.DataGridView();
            this.controller = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.action = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.url = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.btnBackup = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TxtReemplazo = new System.Windows.Forms.TextBox();
            this.TxtOriginal = new System.Windows.Forms.TextBox();
            this.BtnActualizar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgVista)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnPublicar
            // 
            this.btnPublicar.Location = new System.Drawing.Point(12, 460);
            this.btnPublicar.Name = "btnPublicar";
            this.btnPublicar.Size = new System.Drawing.Size(75, 23);
            this.btnPublicar.TabIndex = 0;
            this.btnPublicar.Text = "Publicar";
            this.btnPublicar.UseVisualStyleBackColor = true;
            this.btnPublicar.Click += new System.EventHandler(this.btnPublicar_Click);
            // 
            // dgVista
            // 
            this.dgVista.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgVista.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.controller,
            this.action,
            this.url});
            this.dgVista.Location = new System.Drawing.Point(12, 120);
            this.dgVista.Name = "dgVista";
            this.dgVista.Size = new System.Drawing.Size(809, 331);
            this.dgVista.TabIndex = 1;
            // 
            // controller
            // 
            this.controller.DataPropertyName = "Controller";
            this.controller.HeaderText = "Entidad";
            this.controller.Name = "controller";
            this.controller.Width = 200;
            // 
            // action
            // 
            this.action.DataPropertyName = "Action";
            this.action.HeaderText = "Acción";
            this.action.Name = "action";
            this.action.Width = 150;
            // 
            // url
            // 
            this.url.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.url.DataPropertyName = "Url";
            this.url.HeaderText = "Url";
            this.url.Name = "url";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(297, 24);
            this.label1.TabIndex = 2;
            this.label1.Text = "Configurador de la ayuda en Marfil";
            // 
            // btnBackup
            // 
            this.btnBackup.Location = new System.Drawing.Point(93, 460);
            this.btnBackup.Name = "btnBackup";
            this.btnBackup.Size = new System.Drawing.Size(117, 23);
            this.btnBackup.TabIndex = 3;
            this.btnBackup.Text = "Generar Backup";
            this.btnBackup.UseVisualStyleBackColor = true;
            this.btnBackup.Click += new System.EventHandler(this.btnBackup_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.BtnActualizar);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.TxtReemplazo);
            this.groupBox1.Controls.Add(this.TxtOriginal);
            this.groupBox1.Location = new System.Drawing.Point(12, 47);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(809, 67);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Raíz URL";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(231, 44);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(19, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "-->";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(262, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Texto reemplazo";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Texto original";
            // 
            // TxtReemplazo
            // 
            this.TxtReemplazo.Location = new System.Drawing.Point(265, 41);
            this.TxtReemplazo.Name = "TxtReemplazo";
            this.TxtReemplazo.Size = new System.Drawing.Size(205, 20);
            this.TxtReemplazo.TabIndex = 1;
            // 
            // TxtOriginal
            // 
            this.TxtOriginal.Location = new System.Drawing.Point(10, 41);
            this.TxtOriginal.Name = "TxtOriginal";
            this.TxtOriginal.Size = new System.Drawing.Size(205, 20);
            this.TxtOriginal.TabIndex = 0;
            // 
            // BtnActualizar
            // 
            this.BtnActualizar.Location = new System.Drawing.Point(488, 39);
            this.BtnActualizar.Name = "BtnActualizar";
            this.BtnActualizar.Size = new System.Drawing.Size(75, 23);
            this.BtnActualizar.TabIndex = 5;
            this.BtnActualizar.Text = "Actualizar URLs";
            this.BtnActualizar.UseVisualStyleBackColor = true;
            this.BtnActualizar.Click += new System.EventHandler(this.BtnActualizar_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(833, 495);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnBackup);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgVista);
            this.Controls.Add(this.btnPublicar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configurar ayuda";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgVista)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnPublicar;
        private System.Windows.Forms.DataGridView dgVista;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn controller;
        private System.Windows.Forms.DataGridViewTextBoxColumn action;
        private System.Windows.Forms.DataGridViewTextBoxColumn url;
        private System.Windows.Forms.Button btnBackup;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox TxtOriginal;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TxtReemplazo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button BtnActualizar;
    }
}

