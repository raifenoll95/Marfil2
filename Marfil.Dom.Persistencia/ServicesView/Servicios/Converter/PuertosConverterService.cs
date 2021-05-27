using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    class PuertosConverterService : BaseConverterModel<PuertosModel, Puertos>
    {
        public PuertosConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override IEnumerable<IModelView> GetAll()
        {
            var result = base.GetAll();

            using (var service = new TablasVariasService(Context, _db))
            {
                var listPaises = service.GetListPaises();
                return result.Select(c => {
                    ((PuertosModel)c).DescripcionPais = listPaises.Single(f => f.Valor == ((PuertosModel)c).Fkpaises).Descripcion;
                    return c;
                });
            }
        }

        public override IModelView CreateView(string id)
        {
            var fmodel = new FModel();
            var objExt = fmodel.GetModel<PuertosModel>(Context) as IModelViewExtension;
           
            var vector = objExt.generateId(id) as string[];
            var codpais = vector[0];
            var codprovincia = vector[1];
            var obj = _db.Puertos.Single(f => f.fkpaises == codpais && f.id == codprovincia);

            var result = GetModelView(obj) as PuertosModel;

            var service = new TablasVariasService(Context, _db);
            
                result.DescripcionPais = service.GetListPaises().Single(f => f.Valor == codpais).Descripcion;
            

            return result;
        }

        public override Puertos EditPersitance(IModelView obj)
        {
            var model = obj as PuertosModel;
            var result = _db.Set<Puertos>().Single(f => f.fkpaises == model.Fkpaises && f.id == model.Id);
            foreach (var item in result.GetType().GetProperties())
            {
                item.SetValue(result, obj.get(item.Name));
            }


            return result;
        }

        public override bool Exists(string id)
        {
            var fmodel = new FModel();
            var objExt = fmodel.GetModel<PuertosModel>(Context) as IModelViewExtension;
            var vector = objExt.generateId(id) as string[];
            var codpais = vector[0];
            var codprovincia = vector[1];
            return _db.Set<Puertos>().Any(f=>  f.fkpaises == codpais && f.id == codprovincia);
        }
    }
}
