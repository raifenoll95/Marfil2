using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Iva;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Inf.Genericos;
using System.Data.Entity;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    class RegistroIvaRepercutidoConverterService : BaseConverterModel<RegistroIvaRepercutidoModel, Persistencia.RegistroIVARepercutido>
    {
        public RegistroIvaRepercutidoConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override IEnumerable<IModelView> GetAll()
        {
            return _db.Set<Persistencia.RegistroIVARepercutido>().Select(GetModelView);
        }

        public override IModelView CreateView(string id)
        {
            var idnum = Int32.Parse(id);
            var obj = _db.Set<Persistencia.RegistroIVARepercutido>().Where(f => f.empresa == Empresa && f.id == idnum).Include(f => f.RegistroIVARepercutidoTotales).Single();
            var result = GetModelView(obj) as RegistroIvaRepercutidoModel;

            //Lineas
            result.Totales = obj.RegistroIVARepercutidoTotales.ToList().Select(f => new RegistroIvaRepercutidoTotalesModel()
            {
                Id = f.id,
                Fktiposiva = f.fktiposiva,
                Porcentajerecargoequivalencia = f.porcentajerecargoequivalencia,
                Importerecargoequivalencia = f.importerecargoequivalencia,
                Porcentajeiva = f.porcentajeiva,
                Cuotaiva = f.cuotaiva,
                Subtotal = f.subtotal,
                Decimalesmonedas = f.decimalesmonedas,
                Baseimponible = f.basetotal
            }).ToList();

            return result;
        }

        public override RegistroIVARepercutido CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as RegistroIvaRepercutidoModel;
            var result = _db.RegistroIVARepercutido.Create();

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

            result.RegistroIVARepercutidoTotales.Clear();
            foreach (var item in viewmodel.Totales)
            {
                var newItem = _db.Set<RegistroIVARepercutidoTotales>().Create();
                newItem.empresa = Empresa;
                newItem.fkregistros = result.id;
                newItem.id = item.Id;
                newItem.fktiposiva = item.Fktiposiva;
                newItem.porcentajeiva = item.Porcentajeiva;
                newItem.cuotaiva = item.Cuotaiva;
                newItem.porcentajerecargoequivalencia = item.Porcentajerecargoequivalencia ?? 0;              
                newItem.importerecargoequivalencia = item.Importerecargoequivalencia;
                newItem.subtotal = item.Subtotal;
                newItem.decimalesmonedas = item.Decimalesmonedas;
                newItem.basetotal = item.Baseimponible;
                result.RegistroIVARepercutidoTotales.Add(newItem);
            }

            return result;
        }

        public override RegistroIVARepercutido EditPersitance(IModelView obj)
        {
            var viewmodel = obj as RegistroIvaRepercutidoModel;
            var result = _db.RegistroIVARepercutido.Where(f => f.empresa == viewmodel.Empresa && f.id == viewmodel.Id).Include(b => b.RegistroIVARepercutidoTotales).ToList().Single();

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

            result.RegistroIVARepercutidoTotales.Clear();
            foreach (var item in viewmodel.Totales)
            {
                var newItem = _db.Set<RegistroIVARepercutidoTotales>().Create();
                newItem.empresa = Empresa;
                newItem.fkregistros = result.id;
                newItem.id = item.Id;
                newItem.fktiposiva = item.Fktiposiva;
                newItem.porcentajeiva = item.Porcentajeiva;
                newItem.cuotaiva = item.Cuotaiva;
                newItem.porcentajerecargoequivalencia = item.Porcentajerecargoequivalencia ?? 0;
                newItem.importerecargoequivalencia = item.Importerecargoequivalencia;
                newItem.subtotal = item.Subtotal;
                newItem.decimalesmonedas = item.Decimalesmonedas;
                newItem.basetotal = item.Baseimponible;
                result.RegistroIVARepercutidoTotales.Add(newItem);
            }

            return result;
        }

        public override IModelView GetModelView(RegistroIVARepercutido viewmodel)
        {
            var result = base.GetModelView(viewmodel) as RegistroIvaRepercutidoModel;

            return result;
        }
    }
}
