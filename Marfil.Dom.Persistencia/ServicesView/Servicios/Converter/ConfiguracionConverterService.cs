using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class ConfiguracionConverterService : BaseConverterModel<ConfiguracionModel, Configuracion>
    {
        public ConfiguracionConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override Configuracion CreatePersitance(IModelView obj)
        {
            var model = obj as ConfiguracionModel;
            var serializer = new Serializer<InternalConfiguracionModel>();
            var configuracion = _db.Set<Configuracion>().Create();
            configuracion.id = ConfiguracionService.Id;
            configuracion.xml = serializer.GetXml(model.Model);
            return configuracion;
        }

        public override Configuracion EditPersitance(IModelView obj)
        {
            var model = obj as ConfiguracionModel;
            var serializer = new Serializer<InternalConfiguracionModel>();
            var configuracion = _db.Set<Configuracion>().Find(model.Id);
            configuracion.xml = serializer.GetXml(model.Model);
            return configuracion;
        }

        public override IModelView GetModelView(Configuracion obj)
        {
            var serializer = new Serializer<InternalConfiguracionModel>();
            return new ConfiguracionModel(Context)
            {
                Id=obj.id,
                Model = serializer.SetXml(obj.xml)
            };
        
        }
    }

    
}
