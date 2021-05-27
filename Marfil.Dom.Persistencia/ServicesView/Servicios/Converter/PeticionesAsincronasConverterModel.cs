using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Inf.Genericos;
using System.Data.Entity;
using System.Collections;
using Marfil.Dom.Persistencia.Model.CRM;
using Marfil.Inf.Genericos.Helper;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Helpers;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class PeticionesAsincronasConverterModel : BaseConverterModel<PeticionesAsincronasModel, PeticionesAsincronas>
    {

        #region CTR

        public PeticionesAsincronasConverterModel(IContextService context, MarfilEntities db) : base(context, db)
        {

        }

        #endregion

        public override IEnumerable<IModelView> GetAll()
        {
            var result = _db.Set<PeticionesAsincronas>().Where(f => f.empresa == Empresa).ToList().Select(f => GetModelView(f) as PeticionesAsincronasModel).ToList();
            return result;
        }

        public override bool Exists(string id)
        {
            var intid = Funciones.Qint(id);
            return _db.PeticionesAsincronas.Any(f => f.empresa == Context.Empresa && f.id == intid);
        }

        public override IModelView CreateView(string id)
        {
            var identificador = Funciones.Qint(id);
            var obj = _db.Set<PeticionesAsincronas>().Where(f => f.empresa == Empresa && f.id == identificador).Single();
            var result = GetModelView(obj) as PeticionesAsincronasModel;

            var s = result.Configuracion.Split('-');
            if (result.Tipo == (int)TipoPeticion.Importacion && Funciones.Qint(s[0]) == (int)TipoImportacion.ImportarMovs)
            {
                result.Configuracion = TipoImportacion.ImportarMovs + s[1];
            }   
            else if(result.Tipo == (int)TipoPeticion.Importacion && Funciones.Qint(s[0]) == (int)TipoImportacion.ImportarCuentas)
            {
                result.Configuracion = TipoImportacion.ImportarCuentas + s[1];
            }

            return result;
        }

        public override PeticionesAsincronas CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as PeticionesAsincronasModel;
            var result = _db.Set<PeticionesAsincronas>().Create();

            foreach (var item in result.GetType().GetProperties())
            {
                if ((obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false) &&
                    (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.GetGenericTypeDefinition() !=
                    typeof(ICollection<>)))
                {
                    item.SetValue(result, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
                else if (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsEnum ?? false)
                {
                    item.SetValue(result, (int)obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
                else if (!obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false)
                {
                    item.SetValue(result, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
            }

            return result;
        }

        public override PeticionesAsincronas EditPersitance(IModelView obj)
        {
            var viewmodel = obj as PeticionesAsincronasModel;
            var result = _db.PeticionesAsincronas.Where(f => f.empresa == viewmodel.Empresa && f.id == viewmodel.Id).Single();
            
            foreach (var item in result.GetType().GetProperties())
            {
                if ((obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false) &&
                    (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.GetGenericTypeDefinition() !=
                    typeof(ICollection<>)))
                {
                    item.SetValue(result, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
                else if (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsEnum ?? false)
                {
                    item.SetValue(result, (int)obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
                else if (!obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false)
                {
                    item.SetValue(result, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
            }

            return result;
        }

        public override IModelView GetModelView(PeticionesAsincronas obj)
        {
            var result = base.GetModelView(obj) as PeticionesAsincronasModel;
            return result;
        }
            
    }
}
