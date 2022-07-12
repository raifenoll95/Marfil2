using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Documentos.CobrosYPagos;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Startup
{
    internal class CircuitosTesoreriaStartup : IStartup
    {
        private IContextService context;
        //private MarfilEntities db;
        private readonly CircuitosTesoreriaCobrosService _circuitosService;

        public CircuitosTesoreriaStartup(IContextService context, MarfilEntities db)
        {
            this.context = context;
            //this.db = db;
            _circuitosService = new CircuitosTesoreriaCobrosService(context, db);
        }

        //JUL22 - Se pasa a la creación de nueva empresa
        public void CrearDatos(string fichero)
        {
            var csvFile = context.ServerMapPath(fichero);
            using (var reader = new StreamReader(csvFile, Encoding.Default, true))
            {
                var contenido = reader.ReadToEnd();
                CrearCircuitos(contenido);
            }
        }

        private void CrearCircuitos(string xml)
        {
            var lineas = xml.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var item in lineas)
            {
                if (!string.IsNullOrEmpty(item))
                    CrearCircuito(item);
            }

        }

        private void CrearCircuito(string linea)
        {
            var vector = linea.Split(';');
            var model = new CircuitoTesoreriaCobrosModel()
            {
                Empresa = vector[0],
                Id = Int32.Parse(vector[1]),
                Descripcion = vector[2],
                Situacioninicial = vector[3],
                Situacionfinal = vector[4],
                //Datos = !String.IsNullOrEmpty(vector[5]) ? Int32.Parse(vector[5]) : null,
                Asientocontable = string.Equals(vector[6], '0') ? false : true,
                Fecharemesa = string.Equals(vector[7], '0') ? false : true,
                Fechapago = string.Equals(vector[8], '0') ? false : true,
                Liquidariva = string.Equals(vector[9], '0') ? false : true,
                Conciliacion = string.Equals(vector[10], '0') ? false : true,
                Datosdocumento = string.Equals(vector[11], '0') ? false : true,
                Cuentacargo1 = !string.Equals(vector[12], "NULL") ? vector[12] : null,
                Cuentacargo2 = !string.Equals(vector[13], "NULL") ? vector[13] : null,
                Cuentacargorel = !string.Equals(vector[14], "NULL") ? vector[14] : null,
                Cuentaabono1 = !string.Equals(vector[15], "NULL") ? vector[15] : null,
                Cuentaabono2 = !string.Equals(vector[16], "NULL") ? vector[16] : null,
                Cuentaabonorel = !string.Equals(vector[17], "NULL") ? vector[17] : null,
                Importecuentacargo1 = (TipoImporte)Int32.Parse(vector[18]),
                Importecuentacargo2 = (TipoImporte)Int32.Parse(vector[19]),
                Importecuentaabono1 = (TipoImporte)Int32.Parse(vector[20]),
                Importecuentaabono2 = (TipoImporte)Int32.Parse(vector[21]),
                Importecuentacargorel = (TipoImporte)Int32.Parse(vector[22]),
                Importecuentaabonorel = (TipoImporte)Int32.Parse(vector[23]),
                Desccuentacargo1 = !string.Equals(vector[24], "NULL") ? vector[24] : null,
                Desccuentacargo2 = !string.Equals(vector[25], "NULL") ? vector[25] : null,
                Desccuentacargorel = !string.Equals(vector[26], "NULL") ? vector[26] : null,
                Desccuentaabono1 = !string.Equals(vector[27], "NULL") ? vector[27] : null,
                Desccuentaabono2 = !string.Equals(vector[28], "NULL") ? vector[28] : null,
                Desccuentaabonorel = !string.Equals(vector[29], "NULL") ? vector[29] : null,
                Tipocircuito = (TipoCircuito)Int32.Parse(vector[30]),
                Codigodescripcionasiento = vector[31],
                Documentocartera = string.Equals(vector[32], '0') ? false : true,
                Actualizarcobrador = string.Equals(vector[33], '0') ? false : true,
                Anularremesa = string.Equals(vector[34], '0') ? false : true,
                Desvalorizacioncartera = string.Equals(vector[35], '0') ? false : true,
                Fkmodopagopreferido = vector[36]
            };

            if(!string.Equals(vector[5],"NULL"))
            {
                model.Datos = Int32.Parse(vector[5]);
            }
            else
            {
                model.Datos = null;
            }



            _circuitosService.create(model);
        }

        public void Dispose()
        {
            _circuitosService?.Dispose();
        }
    }
}