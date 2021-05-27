using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Inf.Genericos.Helper;
using System.ComponentModel.DataAnnotations;


namespace Marfil.Dom.Persistencia.Model.Documentos
{
    public class DocumentosBusqueda
    {
        public string Referencia { get; set; }

        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Fechadocumento { get; set; }

        public string Fecha
        {
            get { return Fechadocumento?.ToShortDateString()??string.Empty; }
        }

        public string Fkclientes { get; set; }

        public string Nombrecliente { get; set; }

        public string Estado { get; set; }

        public int? Decimalesmonedas { get; set; }

        public double? Importebaseimponible { get; set; }

        public string Basecadena
        {
            get { return Importebaseimponible?.ToString("N" + Decimalesmonedas); }
        }
    }
}
