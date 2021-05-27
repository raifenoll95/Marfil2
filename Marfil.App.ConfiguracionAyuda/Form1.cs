using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Marfil.App.ConfiguracionAyuda.Service;
using Marfil.Dom.ControlsUI.Ayuda;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.App.ConfiguracionAyuda
{
    public partial class Form1 : Form
    {
        #region Member

        private readonly PublicadorAyudaService _service;
        private AyudaModel _datasource=new AyudaModel();

        #endregion

        #region CTR

        public Form1()
        {
            InitializeComponent();
            
            _service=new PublicadorAyudaService(_datasource);
           
        }

        

        #endregion

        #region Publicar

        private void btnPublicar_Click(object sender, EventArgs e)
        {
            try
            {
                _service.Publicar();
                MessageBox.Show("publicacion realizada correctamente");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                _service.Load();
                var list = new BindingList<AyudaItemModel>(_datasource.List);
                dgVista.DataSource = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        private void btnBackup_Click(object sender, EventArgs e)
        {
            try
            {
                using (var fd = new SaveFileDialog())
                {
                    fd.Filter = "XML|*.xml";
                    if (fd.ShowDialog() == DialogResult.OK)
                    {
                        _service.Backup(fd.FileName);
                        MessageBox.Show("Backup realizado correctamente. Menos mal!! :D");

                    }
                }
                    
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        private void BtnActualizar_Click(object sender, EventArgs e)
        {
            try
            {
                _service.Actualizar(TxtOriginal.Text,TxtReemplazo.Text);
                dgVista.Refresh();
                MessageBox.Show("Actualización realizada correctamente");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
