using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IUnidadesService
    {

    }

    public class UnidadesService : GestionService<UnidadesModel, Unidades>, IUnidadesService
    {
        #region CTR

        public UnidadesService(IContextService context, MarfilEntities db = null) : base(context, db)
        {
            
        }

        #endregion

        #region Index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model= base.GetListIndexModel(t, canEliminar, canModificar, controller);

            model.ExcludedColumns = model.Properties.Where(f => f.property.Name != "Id" && f.property.Name != "Codigounidad" && f.property.Name != "Descripcion").Select(f => f.property.Name).ToArray();

            return model;
        }

        #endregion

        public static double CalculaResultado(UnidadesModel model,double cantidad, double largo, double ancho, double grueso,double metros)
        {
            if (model.Formula == TipoStockFormulas.Cantidad)
                return cantidad;
            if (model.Formula == TipoStockFormulas.Linear)
                return cantidad*largo;
            if (model.Formula==TipoStockFormulas.Superficie)
                return cantidad * largo*ancho;
            if (model.Formula == TipoStockFormulas.Volumen)
                return cantidad * largo * ancho * grueso;
            if (model.Formula == TipoStockFormulas.Total)
                return metros;

            throw new  NotImplementedException();
        }
    }
}
