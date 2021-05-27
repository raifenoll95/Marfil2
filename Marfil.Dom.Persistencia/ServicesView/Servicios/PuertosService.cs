using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IPuertosService
    {

    }

    public class PuertosService : GestionService<PuertosModel, Puertos>, IPuertosService
    {
        #region CTR

        public PuertosService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            model.ExcludedColumns = new[] { "CustomId", "Fkpaises","Toolbar" };
            model.PrimaryColumnns = new[] { "CustomId" };
            model.ColumnasCombo.Add("DescripcionPais", _appService.GetListPaises().Select(f => new Tuple<string, string>(f.Descripcion, f.Descripcion)));
            return model;
        }

        public override string GetSelectPrincipal()
        {
            return "select p.*,pa.descripcion as DescripcionPais, concat(p.fkpaises,'-',p.id) as CustomId from puertos as p " +
                   " inner join paises as pa on pa.valor=p.Fkpaises " +
                   " order by p.descripcion asc";
        }
    }
}
