using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
   /*
    *TODO EL : Eliminar esta clase public static class FTercerosService
    {
        public static IGestionService CreateService(TiposCuentas tipo,string empresa,MarfilEntities db=null)
        {
            var fservice=FService.Instance;
            switch (tipo)
            {
                case TiposCuentas.Aseguradoras:
                    return fservice.GetService(typeof (AseguradorasModel), empresa, db);

                default:
                    return null;
            }
        }
    }*/
}
