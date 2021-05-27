using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DevExpress.Web;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Stock;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.DiarioStock
{
    public class DiarioStockDatabaseDataProvider
    {
        const string LargeDatabaseDataContextKey = "DXLargeDatabaseDataContext";

        public static MarfilEntities DB(IContextService context)
        {
           
               
                if (context.GetItem(LargeDatabaseDataContextKey) == null)
                context.SetItem(LargeDatabaseDataContextKey, MarfilEntities.ConnectToSqlServer(context.BaseDatos));
                return (MarfilEntities)context.GetItem(LargeDatabaseDataContextKey);
            
        }

        

        public static IQueryable<Diariostock> GetMovimientos(DiarioStockModel model)
        {

            var list= DB(model.Context).Diariostock.Where(f => model.Context.Empresa == f.empresa );
            list = string.IsNullOrEmpty(model.Fkarticulos) ? list : list.Where(f => f.fkarticulos == model.Fkarticulos);
            list = string.IsNullOrEmpty(model.Lote) ? list : list.Where(f => (f.lote) == model.Lote);
            list = (model.Categoria == CategoriaMovimientos.Principal) ? list.Where(f => f.categoriamovimiento == (int)CategoriaMovimientos.Principal) : list;
                
            list = !model.FechaDesde.HasValue ? list : list.Where(f => f.fecha >= model.FechaDesde.Value);

            if (model.FechaHasta.HasValue)
            {
                var fechahasta = model.FechaHasta.Value.AddDays(1);
                list = list.Where(f => f.fecha <= fechahasta);
            }


            if (model.Tipodealmacenlote.HasValue)
            {
                list = list.Where(f => f.tipoalmacenlote == (int?)model.Tipodealmacenlote);
            }
            
            return list;
        }
    }
}
