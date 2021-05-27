using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Marfil.App.TestGenerator.Model;
using Marfil.App.TestGenerator.Services;
using Marfil.Dom.ControlsUI.NifCif;

using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Inf.Genericos.Helper;
using CClientes = Marfil.App.TestGenerator.Model.Clientes;

namespace Marfil.App.TestGenerator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
        }

        private void BtnClientes_Click(object sender, EventArgs e)
        {
            try
            {
                using (var sd = new SaveFileDialog())
                {
                    if (sd.ShowDialog() == DialogResult.OK)
                    {

                        var inicio = 200;
                        for (var i = 0; i < 1000; i++)
                        {
                            var cadena = FGenerarTest.Instance.Generar<CClientes>().GenerarTest(new CClientes()
                            {
                                Empresa = "0001",
                                Cuentas = new CuentasModel()
                                {
                                    Empresa="0001",
                                    Id="4300" + Funciones.RellenaCod((i+ inicio).ToString(),4),
                                    Descripcion = string.Format("cuenta descripcion {0}", (i + inicio)),
                                    Descripcion2 = string.Format("cuenta razon social {0}", (i + inicio)),
                                    FkPais = "070",
                                    Nif = new NifCifModel()
                                    {
                                        Nif= "28476105N",
                                        TipoNif = "001"
                                    }
                                },
                                Fkpuertos = new PuertoscontrolModel()
                                {
                                    Fkpaises = "070"
                                },
                                 Fkidiomas = "CAS",
                                 Fkmonedas = 978,
                                 Fkregimeniva = "NORMA",
                                 Criterioiva = CriterioIVA.Caja,
                                 Fkformaspago = 1,
                                 Descuentoprontopago = 0,
                                 Descuentocomercial = 0,
                                 Diafijopago1 = 0,
                                 Diafijopago2 = 0,
                                 Numerocopiasfactura = 0,
                                 Fktarifas = "PVALM",
                                 Porcentajeriesgocomercial = 0,
                                 Porcentajeriesgopolitico = 0,
                                 Riesgoaseguradora = 0,
                                 Riesgoconcedidoempresa = 0,
                                 Riesgosolicitado = 0,
                                 Diascondecidos = 0
                                
                            });

                            File.AppendAllText(sd.FileName, cadena);
                        }
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
