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
        private MarfilEntities db;
        private readonly CircuitosTesoreriaCobrosService _tablasVariasService;

        public CircuitosTesoreriaStartup(IContextService context, MarfilEntities db)
        {
            this.context = context;
            this.db = db;
        }

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
                Cuentacargo1 = vector[12],
                Cuentacargo2 = vector[13],
                Cuentacargorel = vector[14],
                Cuentaabono1 = vector[15],
                Cuentaabono2 = vector[16],
                Cuentaabonorel = vector[17],
                Importecuentacargo1 = (TipoImporte)Int32.Parse(vector[18]),
                Importecuentacargo2 = (TipoImporte)Int32.Parse(vector[19]),
                Importecuentaabono1 = (TipoImporte)Int32.Parse(vector[20]),
                Importecuentaabono2 = (TipoImporte)Int32.Parse(vector[21]),
                Importecuentacargorel = (TipoImporte)Int32.Parse(vector[22]),
                Importecuentaabonorel = (TipoImporte)Int32.Parse(vector[23]),
                Desccuentacargo1 = vector[24],
                Desccuentacargo2 = vector[25],
                Desccuentacargorel = vector[26],
                Desccuentaabono1 = vector[27],
                Desccuentaabono2 = vector[28],
                Desccuentaabonorel = vector[29],
                Tipocircuito = (TipoCircuito)Int32.Parse(vector[30]),
                Codigodescripcionasiento = vector[31],
                Documentocartera = string.Equals(vector[32], '0') ? false : true,
                Actualizarcobrador = string.Equals(vector[33], '0') ? false : true
            };

            if(!string.Equals(vector[5],"NULL"))
            {
                model.Datos = Int32.Parse(vector[5]);
            }

            else
            {
                model.Datos = null;
            }

            _tablasVariasService.create(model);
        }

        public void Dispose()
        {
            _tablasVariasService?.Dispose();
        }
    }
}