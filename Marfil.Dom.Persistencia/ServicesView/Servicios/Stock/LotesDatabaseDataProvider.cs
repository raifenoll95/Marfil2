using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DevExpress.Web;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Stock;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Stock
{
    public class LotesDatabaseDataProvider
    {
        const string LargeDatabaseDataContextKey = "DXLargeDatabaseDataContext";

        public static MarfilEntities DB(IContextService context)
        {
            
                
                if (context.GetItem(LargeDatabaseDataContextKey) == null)
                context.SetItem(LargeDatabaseDataContextKey, MarfilEntities.ConnectToSqlServer(context.BaseDatos));
                return (MarfilEntities)context.GetItem(LargeDatabaseDataContextKey);
            
        }

        

        public static IQueryable<Lotes> GetMovimientos(ListadoLotesModel model)
        {
            //var list = DB(model.Context).Database.SqlQuery<Lotes>("spLotes").AsQueryable();
            var list = DB(model.Context).Lotes.Where(f => model.Context.Empresa == f.empresa);
            list = list.Where(f => f.empresa == model.Context.Empresa);
            list = string.IsNullOrEmpty(model.LoteDesde) ? list : list.Where(f => f.lote.CompareTo(model.LoteDesde) >= 0);
            list = string.IsNullOrEmpty(model.LoteHasta) ? list : list.Where(f => f.lote.CompareTo(model.LoteHasta) <= 0);
            list = string.IsNullOrEmpty(model.FkarticulosDesde) ? list : list.Where(f => f.Codarticulo.CompareTo(model.FkarticulosDesde) >= 0);
            list = string.IsNullOrEmpty(model.FkarticulosHasta) ? list : list.Where(f => f.Codarticulo.CompareTo(model.FkarticulosHasta) <= 0);
            list = string.IsNullOrEmpty(model.FkfamiliasDesde) ? list : list.Where(f => f.Codarticulo.Substring(0, 2).CompareTo(model.FkfamiliasDesde) >= 0);
            list = string.IsNullOrEmpty(model.FkfamiliasHasta) ? list : list.Where(f => f.Codarticulo.Substring(0, 2).CompareTo(model.FkfamiliasHasta) <= 0);
            list = string.IsNullOrEmpty(model.FkmaterialesDesde) ? list : list.Where(f => f.Codarticulo.Substring(2, 3).CompareTo(model.FkmaterialesDesde) >= 0);
            list = string.IsNullOrEmpty(model.FkmaterialesHasta) ? list : list.Where(f => f.Codarticulo.Substring(2, 3).CompareTo(model.FkmaterialesHasta) <= 0);
            list = string.IsNullOrEmpty(model.FkcaracteristicasDesde) ? list : list.Where(f => f.Codarticulo.Substring(5, 2).CompareTo(model.FkcaracteristicasDesde) >= 0);
            list = string.IsNullOrEmpty(model.FkcaracteristicasHasta) ? list : list.Where(f => f.Codarticulo.Substring(5, 2).CompareTo(model.FkcaracteristicasHasta) <= 0);
            list = string.IsNullOrEmpty(model.FkgrosoresDesde) ? list : list.Where(f => f.Codarticulo.Substring(7, 2).CompareTo(model.FkgrosoresDesde) >= 0);
            list = string.IsNullOrEmpty(model.FkgrosoresHasta) ? list : list.Where(f => f.Codarticulo.Substring(7, 2).CompareTo(model.FkgrosoresHasta) <= 0);
            list = string.IsNullOrEmpty(model.FkacabadosDesde) ? list : list.Where(f => f.Codarticulo.Substring(9, 2).CompareTo(model.FkacabadosDesde) >= 0);
            list = string.IsNullOrEmpty(model.FkacabadosHasta) ? list : list.Where(f => f.Codarticulo.Substring(9, 2).CompareTo(model.FkacabadosHasta) <= 0);
            list = model.EnStock == false ? list : list.Where(f => f.EnStock > 0);
            return list;
        }
    }
}
