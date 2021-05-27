using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Stock;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface ITrabajosService
    {
        
    }

    public class TrabajosService : GestionService<TrabajosModel, Trabajos>, ITrabajosService
    {
        #region CTR

        public TrabajosService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        #region List Index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var result= base.GetListIndexModel(t, canEliminar, canModificar, controller);

            result.ExcludedColumns = new[] {"Empresa", "Fkarticulofacturable","Tipoimputacion","Context","Toolbar", "Fkacabadoinicial", "Fkacabadofinal","Lineas", "NumeroLineasHistorico" };
            return result;
        }

        public override string GetSelectPrincipal()
        {
            var result=new StringBuilder();

            //result.Append(
            //    "select t.*,aini.descripcion as Acabadoinicialdescripcion,afin.descripcion as Acabadofinaldescripcion from trabajos as t ");
            //result.Append(" left join acabados as aini on aini.id=t.fkacabadoinicial ");
            //result.Append(" left join acabados as afin on afin.id=t.fkacabadofinal ");
            //result.AppendFormat(" where t.empresa ='{0}' ", _context.Empresa);

            result.Append("select t.*, aini.descripcion as Acabadoinicialdescripcion, afin.descripcion as Acabadofinaldescripcion from trabajos as t ");
            result.Append(" left join acabados as aini on aini.id = t.fkacabadoinicial ");
            result.Append(" left join acabados as afin on afin.id = t.fkacabadofinal ");
            result.AppendFormat(" where t.empresa ='{0}' ", _context.Empresa);
            result.Append(" group by t.empresa, t.id, t.descripcion, t.tipotrabajo, t.tipoimputacion, t.fkacabadoinicial, t.fkacabadofinal, ");
            result.Append(" t.fkarticulofacturable, t.precio, aini.descripcion, afin.descripcion ");
            return result.ToString();
        }

        #endregion

        //public IEnumerable<TrabajosModel> getAllTareas()
        //{
        //    return _db.Database.SqlQuery<TareasModel>(string.Format("SELECT * FROM Tareas"));
        //}


        //public TareasModel getTarea(string id)
        //{
        //    return _db.Database.SqlQuery<TareasModel>(string.Format("SELECT * FROM Tareas WHERE id='" + id + "';")).SingleOrDefault();
        //}


        public IEnumerable<double> getPrecio(string id, string año)
        {
            return _db.Database.SqlQuery<double>(string.Format("SELECT precio FROM TrabajosLin WHERE fktrabajos='" + id + "' AND año=" + año + ";"));
        }
    }
}
