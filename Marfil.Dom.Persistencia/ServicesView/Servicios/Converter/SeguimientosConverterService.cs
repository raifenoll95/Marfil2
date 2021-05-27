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
using Marfil.Dom.Persistencia.Model.Configuracion;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class SeguimientosConverterService : BaseConverterModel<SeguimientosModel, Seguimientos>
    {

        #region CTR

        public SeguimientosConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {

        }

        public override IEnumerable<IModelView> GetAll()
        {
            var result = _db.Set<Seguimientos>().Where(f => f.empresa == Empresa).ToList().Select(f => GetModelView(f) as SeguimientosModel).ToList();
            return result;
        }

        public override bool Exists(string id)
        {
            var intid = Funciones.Qint(id);
            return _db.Seguimientos.Any(f => f.empresa == Context.Empresa && f.id == intid);
        }

        public override IModelView CreateView(string id)
        {
            var identificador = Funciones.Qint(id);
            var obj = _db.Set<Seguimientos>().Where(f => f.empresa == Empresa && f.id == identificador).Single();
            var result = GetModelView(obj) as SeguimientosModel;

            if (result.Tipo == (int)DocumentoEstado.Oportunidades)
            {
                result.idPadre = _db.Oportunidades.Where(f => f.empresa == Empresa && f.referencia == result.Origen).Select(f => f.id).SingleOrDefault();
            }
            else if (result.Tipo == (int)DocumentoEstado.Proyectos)
            {
                result.idPadre = _db.Proyectos.Where(f => f.empresa == Empresa && f.referencia == result.Origen).Select(f => f.id).SingleOrDefault();
            }
            else if (result.Tipo == (int)DocumentoEstado.Campañas)
            {
                result.idPadre = _db.Campañas.Where(f => f.empresa == Empresa && f.referencia == result.Origen).Select(f => f.id).SingleOrDefault();
            }
            else if (result.Tipo == (int)DocumentoEstado.Incidencias)
            {
                result.idPadre = _db.IncidenciasCRM.Where(f => f.empresa == Empresa && f.referencia == result.Origen).Select(f => f.id).SingleOrDefault();
            }
            
            var serviceSeguimientos = FService.Instance.GetService(typeof(SeguimientosModel), Context) as SeguimientosService;

            if (!string.IsNullOrEmpty(result.Fkreferenciadocumentorelacionado))
                result.Idrelacionado = serviceSeguimientos.BuscarIdDocumentoRelaciondo(result.Fkreferenciadocumentorelacionado).SingleOrDefault();

            return result;
        }

        public override Seguimientos CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as SeguimientosModel;
            var result = _db.Set<Seguimientos>().Create();

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

        public override Seguimientos EditPersitance(IModelView obj)
        {
            var viewmodel = obj as SeguimientosModel;
            var result = _db.Seguimientos.Where(f => f.empresa == viewmodel.Empresa && f.id == viewmodel.Id).Single();
            
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

        public override IModelView GetModelView(Seguimientos obj)
        {
            var result = base.GetModelView(obj) as SeguimientosModel;
            return result;
        }

            #endregion
    }
}
