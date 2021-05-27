using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Marfil.Dom.ControlsUI.Ayuda;
using Marfil.Inf.Genericos;
using static System.Configuration.ConfigurationSettings;

namespace Marfil.App.ConfiguracionAyuda.Service
{
    class PublicadorAyudaService
    {
        private readonly AyudaModel _model;

        public PublicadorAyudaService(AyudaModel model)
        {
            _model = model;
        }

        public void Actualizar(string original, string nueva)
        {
            if(string.IsNullOrEmpty(original))
                throw new Exception("El campo Original está vacío");

            if (string.IsNullOrEmpty(nueva))
                throw new Exception("El campo Reemplazo está vacío");

            foreach (var item in _model.List)
            {
                item.Url = item.Url.Replace(original, nueva);
            }
        }

        public void Publicar()
        {
            var serializer=new Serializer<AyudaModel>();
            var salida = serializer.GetXml(_model);
            var ficheroreal= AppSettings["real"];
            var ficherodesarrollo = AppSettings["desarrollo"];
            File.WriteAllText(ficheroreal,salida);
            File.WriteAllText(ficherodesarrollo, salida);
        }

        public void Load()
        {
            var ficherodesarrollo = AppSettings["desarrollo"];
            if (File.Exists(ficherodesarrollo))
            {
                var serializer = new Serializer<AyudaModel>();
                var aux = serializer.SetXml(File.ReadAllText(ficherodesarrollo));
                if (aux != null)
                    _model.List = aux.List;
            }
            
        }

        public void Backup(string fileName)
        {
            var serializer = new Serializer<AyudaModel>();
            var salida = serializer.GetXml(_model);
            File.WriteAllText(fileName, salida);
        }
    }
}
