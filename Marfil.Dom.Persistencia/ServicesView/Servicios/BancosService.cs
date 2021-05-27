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
    public interface IBancosService
    {

    }

    public class BancosService : GestionService<BancosModel, Bancos>, IBancosService
    {
        #region CTR

        public BancosService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion



    }
}
