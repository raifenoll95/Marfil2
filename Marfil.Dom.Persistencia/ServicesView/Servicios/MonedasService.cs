using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IMonedasService
    {

    }

    public class MonedasService : GestionService<MonedasModel, Monedas>, IMonedasService
    {
        #region CTR

        public MonedasService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            model.AnchoColumnas.Add("Id",120);
            model.ExcludedColumns=new [] {"Decimales", "CambioMonedaBase", "CambioMonedaAdicional", "Log","FechaModificacion", "Usuario" ,"UsuarioId","Toolbar"};
            return model;
        }

        public int NextId()
        {
            return _db.Monedas.Any() ? _db.Monedas.Max(f => f.id) + 1 : 1;
        }
    }
}
