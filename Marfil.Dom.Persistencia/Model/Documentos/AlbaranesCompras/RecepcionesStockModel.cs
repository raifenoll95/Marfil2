using System.Collections.Generic;
using System.Linq;
using Marfil.Dom.Persistencia.Model.Documentos.Albaranes;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos.Helper;
using RRecepcionstock = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.RecepcionStock;
namespace Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras
{
    public class RecepcionesStockModel:AlbaranesComprasModel
    {
        public RecepcionesStockModel()
        {
            Modo= ModoAlbaran.Constock;
        }

        public RecepcionesStockModel(IContextService context):base(context)
        {
            Modo = ModoAlbaran.Constock;
        }

        public RecepcionesStockModel(AlbaranesComprasModel model):base(model.Context)
        {

            var properties = model.GetType().GetProperties();
            foreach (var p in properties)
            {
                if (p.CanWrite && p.CanRead)
                    p.SetValue(this, p.GetValue(model));
            }

            Lineas = model.Lineas;
            Costes = model.Costes;
            Totales = model.Totales;
            Modo = ModoAlbaran.Constock;
        }

       

        public override string DisplayName => RRecepcionstock.TituloEntidad;
    }
}
