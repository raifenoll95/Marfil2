using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.ControlsUI.Bloqueo;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos;
using RUsuarios =Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Usuarios;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class UsuariosConverterService : BaseConverterModel<UsuariosModel, Usuarios>
    {
        public UsuariosConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override bool Exists(string id)
        {
            var idInt = new Guid(id);
            return _db.Set<Usuarios>().Any(f => f.id == idInt);
        }

        public override IEnumerable<IModelView> GetAll()
        {
            return base.GetAll().ToList().Where(f=>((UsuariosModel)f).Id!=Guid.Empty);
        }

        public override Usuarios CreatePersitance(IModelView obj)
        {
            var model = obj as UsuariosModel;
            var efobj = _db.Set<Usuarios>().Create();
            foreach (var item in efobj.GetType().GetProperties())
            {
                if (item.Name.FirstToUpper() == "Firma"|| item.Name.FirstToUpper() == "Copiaremitente")
                    continue;

                if ((obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false) &&
                    obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.GetGenericTypeDefinition() !=
                    typeof(ICollection<>))
                {
                    item.SetValue(efobj, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
                else if (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsEnum ?? false)
                {
                    item.SetValue(efobj, (int)obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
                else if (!obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false)
                {
                    item.SetValue(efobj, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
            }

            if (!string.IsNullOrEmpty(model.Contenidofirma))
                efobj.firma = model.Contenidofirma;
            efobj.id = Guid.NewGuid();
            efobj.ssl = model.Ssl;
            efobj.copiaremitente = (int?)model.Copiaremitente;
            return efobj;
        }

        public override Usuarios EditPersitance(IModelView obj)
        {
            var model = obj as UsuariosModel;
            var efobj = _db.Set<Usuarios>().Single(f => f.id == model.Id);
            foreach (var item in efobj.GetType().GetProperties())
            {
                if(item.Name.FirstToUpper()=="Firma" || item.Name.FirstToUpper() == "Copiaremitente")
                    continue;
                
                if ((obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false) &&
                    obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.GetGenericTypeDefinition() !=
                    typeof(ICollection<>))
                {
                    item.SetValue(efobj, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
                else if (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsEnum ?? false)
                {
                    item.SetValue(efobj, (int)obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
                else if (!obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false)
                {
                    item.SetValue(efobj, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
            }

            if (!string.IsNullOrEmpty(model.Contenidofirma))
                efobj.firma = model.Contenidofirma;
            efobj.ssl = model.Ssl;
            efobj.copiaremitente = (int?)model.Copiaremitente;

            return efobj;
        }

        public override IModelView GetModelView(Usuarios obj)
        {
            var fmodel = new FModel();
            var result = fmodel.GetModel<UsuariosModel>(Context);
            var objProperties = obj.GetType().GetProperties();
            foreach (var item in objProperties)
            {
                if ( item.Name.FirstToUpper() == "Copiaremitente")
                    continue;
                if (obj.GetType().GetProperty(item.Name).PropertyType.IsGenericType &&
                    obj.GetType().GetProperty(item.Name).PropertyType.GetGenericTypeDefinition() !=
                    typeof(ICollection<>))
                {
                    result.set(item.Name.FirstToUpper(), obj.GetType().GetProperty(item.Name)?.GetValue(obj, null));
                }
                else if (obj.GetType().GetProperty(item.Name).PropertyType.IsEnum)
                {
                    result.set(item.Name.FirstToUpper(), (int)obj.GetType().GetProperty(item.Name)?.GetValue(obj, null));
                }
                else if (!obj.GetType().GetProperty(item.Name).PropertyType.IsGenericType)
                {
                    result.set(item.Name.FirstToUpper(), obj.GetType().GetProperty(item.Name)?.GetValue(obj, null));
                }

            }

            
            result.Ssl = obj.ssl??false;
            result.Copiaremitente = (TipoCopiaRemitente?)obj.copiaremitente;
            return result;
        }
    }
}
