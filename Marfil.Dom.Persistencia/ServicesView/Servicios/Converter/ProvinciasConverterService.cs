using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    class ProvinciasConverterService : BaseConverterModel<ProvinciasModel, Provincias>
    {
        public ProvinciasConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override IEnumerable<IModelView> GetAll()
        {
            var result = base.GetAll();

            using (var service = new TablasVariasService(Context,_db))
            {
                var listPaises = service.GetListPaises();
                return result.Select(c => { ((ProvinciasModel)c).DescripcionPais = listPaises.Single(f => f.Valor == ((ProvinciasModel)c).Codigopais).Descripcion;
                                       return c;
                });
               
            }

            
        }

        public override IModelView CreateView(string id)
        {
            var fmodel = new FModel();
            var objExt = fmodel.GetModel<ProvinciasModel>(Context) as IModelViewExtension;
            var vector = objExt.generateId(id) as string[];
            var codpais = vector[0];
            var codprovincia = vector[1];
            var obj = _db.Provincias.Single(f => f.codigopais == codpais && f.id == codprovincia);


            var result = GetModelView(obj) as ProvinciasModel;

            var service = new TablasVariasService(Context, _db);
            result.DescripcionPais = service.GetListPaises().Single(f => f.Valor == codpais).Descripcion;
            

            return result;
        }

        public override Provincias EditPersitance(IModelView obj)
        {
            var model = obj as ProvinciasModel;
            var result = _db.Set<Provincias>().Single(f => f.codigopais == model.Codigopais && f.id == model.Id); 
            foreach (var item in result.GetType().GetProperties())
            {
                item.SetValue(result, obj.get(item.Name));
            }
           

            return result;
        }


    }
}

