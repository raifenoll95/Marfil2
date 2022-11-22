using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Iva;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    class TiposRetencionesConverterService : BaseConverterModel<TiposRetencionesModel, Tiposretenciones>
    {
       

        public TiposRetencionesConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
            
        }

        public override IEnumerable<IModelView> GetAll()
        {
            var list = _db.Set<Tiposretenciones>().Where(f => f.empresa == Empresa).ToList();

            var result = new List<TiposRetencionesModel>();
            foreach (var item in list)
            {
                result.Add(GetModelView(item) as TiposRetencionesModel);
            }

            return result;
        }

        public override bool Exists(string id)
        {
            return _db.Set<Tiposretenciones>().Any(f => f.id == id && f.empresa == Empresa);
        }

        public override IModelView CreateView(string id)
        {
            var obj = _db.Set<Tiposretenciones>().Single(f => f.id == id && f.empresa == Empresa);
            return GetModelView(obj);
        }

        public override Tiposretenciones CreatePersitance(IModelView obj)
        {
            var objRetencion = obj as TiposRetencionesModel;
            var item = _db.Set<Tiposretenciones>().Create();
            item.empresa = objRetencion.Empresa;
            item.id = objRetencion.Id;
            item.descripcion = objRetencion.Descripcion;
            item.fkcuentasabono = objRetencion.Fkcuentaabono;
            item.fkcuentascargo = objRetencion.Fkcuentarecargo;
            item.porcentajeretencion = objRetencion.Porcentajeretencion;
            item.tiporendimiento = (int?)objRetencion.Tiporendimiento;
            item.retencionsobre = (int?)objRetencion.RetencionSobre;
            item.inmueble = objRetencion.Inmueble;
            return item;
        }

        public override Tiposretenciones EditPersitance(IModelView obj)
        {
            var objRetencion = obj as TiposRetencionesModel;
            var item = _db.Set<Tiposretenciones>().Single(f => f.id == objRetencion.Id && f.empresa == Empresa);
            item.descripcion = objRetencion.Descripcion;
            item.fkcuentasabono = objRetencion.Fkcuentaabono;
            item.fkcuentascargo = objRetencion.Fkcuentarecargo;
            item.porcentajeretencion = objRetencion.Porcentajeretencion;
            item.tiporendimiento = (int?)objRetencion.Tiporendimiento;
            item.retencionsobre = (int?)objRetencion.RetencionSobre;
            item.inmueble = objRetencion.Inmueble;
            return item;
        }

        public override IModelView GetModelView(Tiposretenciones obj)
        {
            return new TiposRetencionesModel()
            {
                Empresa= Empresa,
                Id= obj.id,
                Descripcion = obj.descripcion,
                Fkcuentaabono = obj.fkcuentasabono,
                Fkcuentarecargo = obj.fkcuentascargo,
                Porcentajeretencion = obj.porcentajeretencion??0,
                Tiporendimiento = (TipoRendimiento?)obj.tiporendimiento,
                RetencionSobre = (TipoRetencionSobre)obj.retencionsobre,
                Inmueble = obj.inmueble.Value
            };
        }
    }
}
