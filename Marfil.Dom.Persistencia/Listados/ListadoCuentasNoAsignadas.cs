using Marfil.Dom.Persistencia.Listados.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using System.Data.SqlClient;
using System.Data;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Helpers;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Marfil.Dom.Persistencia.Listados
{
    public class ListadoCuentasNoAsignadas : ListadosModel
    {
        public override string TituloListado => "Listado Cuentas no asignadas";

        public override string IdListado => FListadosModel.CuentasNoAsignadas;

        internal override string GenerarSelect()
        {
            var sb = new StringBuilder();
            sb.Append("select asi.fkcuentas as Cuenta, c.descripcion as [Descripción], asi.debe as Debe, asi.haber as Haber, asi.saldo as Saldo from CuentasNoAsignadas as asi, Cuentas as c");

            return sb.ToString();
        }

        internal override string GenerarFiltrosColumnas()
        {
            var sb = new StringBuilder();
            sb.Append(" asi.fkcuentas = c.id and asi.empresa = c.empresa and asi.procesado = 0 and c.nivel = 4");

            return sb.ToString();
        }

        public ListadoCuentasNoAsignadas()
        {

        }

        public ListadoCuentasNoAsignadas(IContextService context) : base(context)
        {

        }
    }
}
