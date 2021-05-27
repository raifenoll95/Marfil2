using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Login;
using Marfil.Inf.Genericos;

namespace Marfil.App.ExportarDatos
{
    public partial class Form1 : Form
    {
        private readonly Dictionary<Type,string> _entidadesList;
        public Form1()
        {
            _entidadesList=new Dictionary<Type,string>();
            _entidadesList.Add(typeof(TiposIvaModel),"Tipos IVA");
            _entidadesList.Add(typeof(GruposIvaModel), "Grupos IVA");
            _entidadesList.Add(typeof(RegimenIvaModel), "Regimen IVA");
            _entidadesList.Add(typeof(UnidadesModel), "Unidades");
            _entidadesList.Add(typeof(GuiascontablesModel), "Guías contables");
            _entidadesList.Add(typeof(CriteriosagrupacionModel), "Criterios agrupación");


            InitializeComponent();

            TxtEntidades.DataSource = new BindingSource(_entidadesList, null);
            TxtEntidades.ValueMember = "Key";
            TxtEntidades.DisplayMember = "Value";
        }

        private void BtnRefrescar_Click(object sender, EventArgs e)
        {
            try
            {
                var tipo = TxtEntidades.SelectedValue as Type;
                var context = new LoginContextService(TxtEmpresa.Text, TxtCatalogo.Text);
                var service = FService.Instance.GetService(tipo, context, MarfilEntities.ConnectToSqlServer(TxtCatalogo.Text, TxtHost.Text, TxtUsuario.Text, TxtPassword.Text));

                var list = service.getAll();

                DgData.DataSource = list;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void BtnExportar_Click(object sender, EventArgs e)
        {
            try
            {
                using (var fd = new FolderBrowserDialog())
                {
                    if(fd.ShowDialog() == DialogResult.OK)
                    {
                        
                        var d1 = typeof(Serializer<>);
                        var tipo = TxtEntidades.SelectedValue as Type;
                        var makeme = d1.MakeGenericType(tipo);
                        var o = Activator.CreateInstance(makeme);
                        var methodinfo = makeme.GetMethod("GetXml");
                       
                        var i = 1;
                        foreach (DataGridViewRow item in DgData.SelectedRows)
                        {
                            var salida = methodinfo.Invoke(o, new[] { item.DataBoundItem });
                            
                            File.WriteAllText(Path.Combine(fd.SelectedPath,string.Format("{0}{1}.xml",TxtEntidades.SelectedText,i++)), salida.ToString());
                        }

                        MessageBox.Show("Exportación realizada");
                    }
                }
                    

                
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
    }
}
