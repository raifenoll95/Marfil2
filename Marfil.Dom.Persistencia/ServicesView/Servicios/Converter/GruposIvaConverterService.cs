using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Iva;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class GruposIvaConverterService : BaseConverterModel<GruposIvaModel, GruposIva>
    {
        
        public GruposIvaConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
            
        }

        public override IEnumerable<IModelView> GetAll()
        {
            var list = _db.Set<GruposIva>().Where(f => f.empresa == Empresa).ToList();

            var result = new List<GruposIvaModel>();
            foreach (var item in list)
            {
                result.Add(GetModelView(item) as GruposIvaModel);
            }

            return result;
        }

        public override bool Exists(string id)
        {
            return _db.Set<GruposIva>().Any(f => f.id == id && f.empresa == Empresa);
        }

        public override IModelView CreateView(string id)
        {
            var obj = _db.Set<GruposIva>().Single(f => f.id == id && f.empresa == Empresa);
            return GetModelView(obj);
        }

        public override GruposIva CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as GruposIvaModel;
            var result = _db.Set<GruposIva>().Create();
            result.empresa = Empresa;
            result.id = viewmodel.Id;
            result.descripcion = viewmodel.Descripcion;
            foreach (var item in viewmodel.Lineas)
            {
                var newItem = _db.Set<GruposIvaLin>().Create();
                newItem.empresa = Empresa;
                newItem.fkgruposiva = viewmodel.Id;
                newItem.desde = item.Desde;
                newItem.fktiposivaconrecargo = item.FkTiposIvaConRecargo;
                newItem.fktiposivasinrecargo = item.FkTiposIvaSinRecargo;
                newItem.fktiposivaexentoiva = item.FkTiposIvaExentoIva;
                result.GruposIvaLin.Add(newItem);
            }


            return result;
        }

        public override GruposIva EditPersitance(IModelView obj)
        {
            var viewmodel = obj as GruposIvaModel;
            var result = _db.Set<GruposIva>().Single(f => f.id == viewmodel.Id && f.empresa == viewmodel.Empresa);
            result.empresa = Empresa;
            result.id = viewmodel.Id;
            result.descripcion = viewmodel.Descripcion;

            result.GruposIvaLin.Clear();
            foreach (var item in viewmodel.Lineas)
            {
                var newItem = _db.Set<GruposIvaLin>().Create();
                newItem.empresa = Empresa;
                newItem.fkgruposiva = viewmodel.Id;
                newItem.desde = item.Desde;
                newItem.fktiposivaconrecargo = item.FkTiposIvaConRecargo;
                newItem.fktiposivasinrecargo = item.FkTiposIvaSinRecargo;
                newItem.fktiposivaexentoiva = item.FkTiposIvaExentoIva;
                result.GruposIvaLin.Add(newItem);
            }
            return result;
        }

        public override IModelView GetModelView(GruposIva obj)
        {
            var result = new GruposIvaModel
            {
                Empresa = Empresa,
                Id = obj.id,
                Descripcion =  obj.descripcion,
                Lineas = obj.GruposIvaLin.Select(f => new GruposIvaLinModel() { Empresa = f.empresa,FkGruposIva  = f.fkgruposiva, Id= f.id,Desde=f.desde, FkTiposIvaSinRecargo = f.fktiposivasinrecargo,FkTiposIvaConRecargo = f.fktiposivaconrecargo, FkTiposIvaExentoIva = f.fktiposivaexentoiva}).ToList()
            };

            return result;
        }
    }
}
