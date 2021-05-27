using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Inf.ResourcesGlobalization.Textos.Entidades;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{

    public interface ITareasService
    {

    }


    public class TareasService : GestionService<TareasModel, Tareas>, ITareasService
    {
        #region CTR

        public TareasService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion


        #region List Index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var result = base.GetListIndexModel(t, canEliminar, canModificar, controller);

            result.ExcludedColumns = new[] { "Empresa", "SeccionProduccion", "Toolbar", "Context", "Lineas", "NumeroLineasHistorico" };
            return result;
        }

        //public override string GetSelectPrincipal()
        //{
        //    var result = new StringBuilder();

        //    result.Append(
        //        "select t.*,aini.descripcion as Acabadoinicialdescripcion,afin.descripcion as Acabadofinaldescripcion from trabajos as t ");
        //    result.Append(" left join acabados as aini on aini.id=t.fkacabadoinicial ");
        //    result.Append(" left join acabados as afin on afin.id=t.fkacabadofinal ");
        //    result.AppendFormat(" where t.empresa ='{0}' ", _context.Empresa);
        //    return result.ToString();
        //}

        #endregion

        //public int NextId()
        //{
        //    return _db.Tareas.Any() ? _db.Tareas.Max(f => f.id) + 1 : 1;
        //}


        public IEnumerable<TareasModel> getAllTareas()
        {
            return _db.Database.SqlQuery<TareasModel>(string.Format("SELECT * FROM Tareas"));
        }
        

        public TareasModel getTarea(string id)
        {
            return _db.Database.SqlQuery<TareasModel>(string.Format("SELECT * FROM Tareas WHERE id='"+id+"';")).SingleOrDefault();
        }


        public IEnumerable<double> getPrecio(string id, string año)
        {
            return _db.Database.SqlQuery<double>(string.Format("SELECT precio FROM TareasLin WHERE fktareas='"+id+"' AND año="+año+";"));
        }

    }

}
