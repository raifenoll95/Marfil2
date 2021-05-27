using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.ControlsUI.Bloqueo;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos;
using REjercicios =Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Ejercicios;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class EjerciciosConverterService : BaseConverterModel<EjerciciosModel, Ejercicios>
    {
        public EjerciciosConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override bool Exists(string id)
        {
            var idInt = int.Parse(id);
            return _db.Set<Ejercicios>().Any(f => f.id == idInt && f.empresa == Empresa);
        }

        public override IEnumerable<IModelView> GetAll()
        {
            var result = new List<EjerciciosModel>();
            var list = _db.Ejercicios.Where(f => f.empresa == Empresa);

            foreach(var ejercicio in list)
            {
                var ejercicioModel = GetModelView(ejercicio) as EjerciciosModel;
                result.Add(ejercicioModel);
            }
            return result;
        }

        public override IModelView CreateView(string id)
        {
            var idInt = int.Parse(id);
            var obj = _db.Set<Ejercicios>().Single(f => f.id == idInt && f.empresa == Empresa);
            return GetModelView(obj) as EjerciciosModel;
        }

        public override Ejercicios EditPersitance(IModelView obj)
        {
            var model = obj as EjerciciosModel;
            var efobj = _db.Set<Ejercicios>().Single(f => f.id == model.Id && f.empresa == Empresa);
            foreach (var item in efobj.GetType().GetProperties())
            {
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

            return efobj;
        }

        //Create p
        //public override Ejercicios CreatePersitance(IModelView obj)
        //{
        //    var objmodel = obj as EjerciciosModel;
        //    var result = _db.Set<Ejercicios>().Create();

        //    foreach (var item in result.GetType().GetProperties())
        //    {
        //        if ((obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false) &&
        //            (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.GetGenericTypeDefinition() !=
        //            typeof(ICollection<>)))
        //        {
        //            item.SetValue(result, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
        //        }
        //        else if (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsEnum ?? false)
        //        {
        //            item.SetValue(result, (int)obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
        //        }
        //        else if (!obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false)
        //        {
        //            item.SetValue(result, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
        //        }
        //    }

        //    return result;
        //}

        public override IModelView GetModelView(Ejercicios viewmodel)
        {
            var result = new EjerciciosModel
            {
                Empresa = viewmodel.empresa,
                Id = viewmodel.id,
                Descripcion = viewmodel.descripcion,
                Descripcioncorta = viewmodel.descripcioncorta,
                Desde = viewmodel.desde,
                Hasta = viewmodel.hasta,
                FkseriescontablesAST = viewmodel.fkseriescontablesAST,
                FkseriescontablesIVS = viewmodel.fkseriescontablesIVS,
                FkseriescontablesIVP = viewmodel.fkseriescontablesIVP,
                FkseriescontablesPRC = viewmodel.fkseriescontablesPRC,
                FkseriescontablesPRP = viewmodel.fkseriescontablesPRP,
                FkseriescontablesCRC = viewmodel.fkseriescontablesCRC,
                FkseriescontablesCRP = viewmodel.fkseriescontablesCRP,
                FkseriescontablesREM = viewmodel.fkseriescontablesREM,
                FkseriescontablesINM = viewmodel.fkseriescontablesINM,
                Estado = (EstadoEjercicio)viewmodel.estado,
                Contabilidadcerradahasta = viewmodel.contabilidadcerradahasta,
                Registroivacerradohasta = viewmodel.registroivacerradohasta,
                Criterioiva = viewmodel.criterioiva,
                Fkejercicios = viewmodel.fkejercicios
            };

            return result;
        }
    }
}
