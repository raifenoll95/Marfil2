using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Iva;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface ITiposRetencionesService
    {

    }

    public class TiposRetencionesService : GestionService<TiposRetencionesModel, Tiposretenciones>, ITiposRetencionesService
    {
        #region CTR

        public TiposRetencionesService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        #region List Index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            
            model.ExcludedColumns = new[] { "Empresa","Fkcuentarecargo", "Fkcuentaabono", "Tiporendimiento","Toolbar" };

            
            return model;
        }

        public override string GetSelectPrincipal()
        {
            return string.Format("select tr.*, cc.descripcion as CuentaRecargoDescripcion, ca.descripcion as CuentaAbonoDescripcion from tiposretenciones as tr " +
                                 " left join cuentas as cc on cc.empresa=tr.empresa and cc.id= tr.fkcuentascargo  " +
                                 " left join cuentas as ca on ca.empresa=tr.empresa and ca.id=tr.fkcuentasabono " +
                   " where tr.empresa='{0}'",Empresa);
        }

        #endregion
    }
}
