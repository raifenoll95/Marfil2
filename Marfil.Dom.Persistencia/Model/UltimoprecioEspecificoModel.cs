using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.ControlsUI.Busquedas;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;

namespace Marfil.Dom.Persistencia.Model
{
    public class UltimospreciosModel
    {
        public ResultBusquedas<UltimoprecioEspecificoModel> Especificos { get; set; }
        public ResultBusquedas<UltimopreciosistemaModel> SistemaVenta { get; set; }
        public ResultBusquedas<UltimopreciosistemaModel> SistemaCompra { get; set; }
        public ResultBusquedas<UltimopreciosistemaModel> SistemaEspecial { get; set; }
    }

    public class UltimoprecioEspecificoModel
    {
        public string Referenciadocumento { get; set; }
        public string Fecha { get; set; }
        public double? DtoCial { get; set; }
        public double? DtoPP { get; set; }
        public double? Cantidad { get; set; }
        public double? Metros { get; set; }
        public string Moneda { get; set; }
        public double? Precio { get; set; }
        public double? DtoLin { get; set; }
    }

    public class UltimopreciosistemaModel
    {
        public string Tarifa { get; set; }
        public double? Precio { get; set; }
    }
}
