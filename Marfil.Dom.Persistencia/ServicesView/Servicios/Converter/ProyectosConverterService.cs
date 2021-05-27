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
    internal class ProyectosConverterService : BaseConverterModel<ProyectosModel, Proyectos>
    {

        #region CTR

        public ProyectosConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {

        }

        #endregion

        public override IEnumerable<IModelView> GetAll()
        {
            var result = _db.Set<Proyectos>().Where(f => f.empresa == Empresa).ToList().Select(f => GetModelView(f) as ProyectosModel).ToList();
            return result;
        }

        public override bool Exists(string id)
        {
            var intid = Funciones.Qint(id);
            return _db.Proyectos.Any(f => f.empresa == Context.Empresa && f.id == intid);
        }

        public override IModelView CreateView(string id)
        {
            var identificador = Funciones.Qint(id);
            var obj = _db.Set<Proyectos>().Where(f => f.empresa == Empresa && f.id == identificador).Single();
            var result = GetModelView(obj) as ProyectosModel;                        

            var seguimientos = _db.Seguimientos.Where(f => f.empresa == result.Empresa && f.origen == result.Referencia);

            result.Seguimientos = seguimientos.Select(f => new SeguimientosModel()
            {
                Id = f.id,
                Fechadocumento = f.fechadocumento,
                Usuario = f.usuario,
                Fketapa = f.fketapa,               
                Fkaccion = f.fkaccion,
                Notas = f.notas,
                Fkdocumentorelacionado = f.fkdocumentorelacionado,
                Fkreferenciadocumentorelacionado = f.fkreferenciadocumentorelacionado
            }).ToList();


            var appService = new ApplicationHelper(Context);
            var serviceSeguimientos = FService.Instance.GetService(typeof(SeguimientosModel), Context) as SeguimientosService;            

            foreach (var item in result.Seguimientos)
            {
                item.Fketapa = _db.Estados.Where(f => (f.documento + "-" + f.id) == item.Fketapa).Select(f => f.descripcion).SingleOrDefault();
                item.Fkaccion = appService.GetListAcciones().Where(f => f.Valor == item.Fkaccion).Select(f => f.Descripcion).SingleOrDefault();
                if(!string.IsNullOrEmpty(item.Fkreferenciadocumentorelacionado))
                    item.Idrelacionado = serviceSeguimientos.BuscarIdDocumentoRelaciondo(item.Fkreferenciadocumentorelacionado).SingleOrDefault(); 
            }            

            return result;
        }

        public override Proyectos CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as ProyectosModel;
            var result = _db.Set<Proyectos>().Create();

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

        public override Proyectos EditPersitance(IModelView obj)
        {
            var viewmodel = obj as ProyectosModel;
            var result = _db.Proyectos.Where(f => f.empresa == viewmodel.Empresa && f.id == viewmodel.Id && f.referencia == viewmodel.Referencia).Single();
            
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

        public override IModelView GetModelView(Proyectos obj)
        {
            var result = base.GetModelView(obj) as ProyectosModel;
            return result;
        }
 
    }
}
