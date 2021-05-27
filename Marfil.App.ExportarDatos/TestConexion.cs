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

namespace Marfil.App.ExportarDatos
{
    public partial class TestConexion : Form
    {
        public TestConexion()
        {
            InitializeComponent();
        }

        private string Test(string url,string param)
        {
            
            var request = WebRequest.Create(url);
            request.ContentType = "application/x-www-form-urlencoded";
            request.Credentials = CredentialCache.DefaultNetworkCredentials;
            request.Method = "POST";
            request.Headers.Add("Usuario", "admin");
            request.Headers.Add("Password", "12345");
            request.Headers.Add("Basedatos", "marfilestable");
            var data = System.Text.Encoding.ASCII.GetBytes(param);
             request.ContentLength = data.Length;
            using (var inputstream = request.GetRequestStream())
            {
                inputstream.Write(data, 0, data.Length);
            }
            // Send the request to the server and wait for the response:
            using (var response = request.GetResponse())
            {

                // Get a stream representation of the HTTP web response:
                using (var stream = response.GetResponseStream())
                {

                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        return reader.ReadToEnd();
                    }

                        
                }
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                TxtSalida.Text = "";
                TxtSalida.Text = Test(textBox1.Text,TxtParams.Text);
            }
            catch (Exception ex)
            {
                TxtSalida.Text = ex.Message;
            }
        }
    }
}
