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
                Idtipofactura = f.idtipofactura,
                Cuentaventas = f.cuentaventas,
                Fktiposiva = f.fktiposiva,
                Porcentajerecargoequivalencia = f.porcentajerecargoequivalencia,
                Importerecargoequivalencia = f.importerecargoequivalencia,
                Porcentajeiva = f.porcentajeiva,
                Cuotaiva = f.cuotaiva,
                Subtotal = f.subtotal,
                Decimalesmonedas = f.decimalesmonedas,
                Baseimponible = f.basetotal,
                Siioperacion = f.siioperacion
            }).ToList();

            //Rectificadas        //Se arrastran los datos de la pestaña Factura, ya no es necesario
            /*result.Rectificadas = obj.RegistroIVARepercutidoRectificadas.ToList().Select(f => new RegistroIvaRepercutidoRectificadasModel()
            {
                Id = f.id,
                Facturaemisor = f.facturaemisor,
                Fechaexpedemisor = (DateTime)f.fechaexpedemisor
            }).ToList();*/

            //Suma totales
            /*result.Sumatotales = obj.RegistroIVARepercutidoSumaTotales.ToList().Select(f => new RegistroIvaRepercutidoSumaTotalesModel()
            {
                Id = f.id,
                Baseretencion = (double)f.baseretencion,
                Porcentajeretencion = (double)f.porcentajeretencion,
                Importeretencion = (double)f.importeretencion,
                Totalfactura = (double)f.totalfactura,
                Operacionesexluidasbi = (double)f.operacionesexluidasbi,
                Decimalesmonedas = f.decimalesmonedas
            }).SingleOrDefault();*/


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
                newItem.idtipofactura = item.Idtipofactura;
                newItem.cuentaventas = item.Cuentaventas;
                newItem.fktiposiva = item.Fktiposiva;
                newItem.porcentajeiva = item.Porcentajeiva;
                newItem.cuotaiva = item.Cuotaiva;
                newItem.porcentajerecargoequivalencia = item.Porcentajerecargoequivalencia ?? 0;              
                newItem.importerecargoequivalencia = item.Importerecargoequivalencia;
                newItem.subtotal = item.Subtotal;
                newItem.decimalesmonedas = item.Decimalesmonedas;
                newItem.basetotal = item.Baseimponible;
                newItem.siioperacion = item.Siioperacion;
                result.RegistroIVARepercutidoTotales.Add(newItem);
            }

            //Se arrastran los datos de la pestaña Factura, ya no es necesario
            /*result.RegistroIVARepercutidoRectificadas.Clear();
            foreach (var item in viewmodel.Rectificadas)
            {
                var newItem = _db.Set<RegistroIVARepercutidoRectificadas>().Create();
                newItem.empresa = Empresa;
                newItem.fkregistros = result.id;
                newItem.id = item.Id;
                newItem.facturaemisor = item.Facturaemisor;
                newItem.fechaexpedemisor = item.Fechaexpedemisor;
                result.RegistroIVARepercutidoRectificadas.Add(newItem);
            }*/

            /*result.RegistroIVARepercutidoSumaTotales.Clear();
            var newreg = _db.Set<RegistroIVARepercutidoSumaTotales>().Create();
            newreg.empresa = Empresa;
            newreg.fkregistros = result.id;
            newreg.id = viewmodel.Sumatotales.Id;
            newreg.baseretencion = viewmodel.Sumatotales.Baseretencion;
            newreg.porcentajeretencion = viewmodel.Sumatotales.Porcentajeretencion;
            newreg.importeretencion = viewmodel.Sumatotales.Importeretencion;
            newreg.operacionesexluidasbi = viewmodel.Sumatotales.Operacionesexluidasbi;
            newreg.totalfactura = viewmodel.Sumatotales.Totalfactura;
            newreg.decimalesmonedas = viewmodel.Sumatotales.Decimalesmonedas;
            result.RegistroIVARepercutidoSumaTotales.Add(newreg);*/

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
                newItem.idtipofactura = item.Idtipofactura;
                newItem.cuentaventas = item.Cuentaventas;
                newItem.fktiposiva = item.Fktiposiva;
                newItem.porcentajeiva = item.Porcentajeiva;
                newItem.cuotaiva = item.Cuotaiva;
                newItem.porcentajerecargoequivalencia = item.Porcentajerecargoequivalencia ?? 0;
                newItem.importerecargoequivalencia = item.Importerecargoequivalencia;
                newItem.subtotal = item.Subtotal;
                newItem.decimalesmonedas = item.Decimalesmonedas;
                newItem.basetotal = item.Baseimponible;
                newItem.siioperacion = item.Siioperacion;
                result.RegistroIVARepercutidoTotales.Add(newItem);
            }

            //Se arrastran los datos de la pestaña Factura, ya no es necesario
            /*result.RegistroIVARepercutidoRectificadas.Clear();
            foreach (var item in viewmodel.Rectificadas)
            {
                var newItem = _db.Set<RegistroIVARepercutidoRectificadas>().Create();
                newItem.empresa = Empresa;
                newItem.fkregistros = result.id;
                newItem.id = item.Id;
                newItem.facturaemisor = item.Facturaemisor;
                newItem.fechaexpedemisor = item.Fechaexpedemisor;
                result.RegistroIVARepercutidoRectificadas.Add(newItem);
            }*/

            /*result.RegistroIVARepercutidoSumaTotales.Clear();
            var newreg = _db.Set<RegistroIVARepercutidoSumaTotales>().Create();
            newreg.empresa = Empresa;
            newreg.fkregistros = result.id;
            newreg.id = viewmodel.Sumatotales.Id;
            newreg.baseretencion = viewmodel.Sumatotales.Baseretencion;
            newreg.porcentajeretencion = viewmodel.Sumatotales.Porcentajeretencion;
            newreg.importeretencion = viewmodel.Sumatotales.Importeretencion;
            newreg.operacionesexluidasbi = viewmodel.Sumatotales.Operacionesexluidasbi;
            newreg.totalfactura = viewmodel.Sumatotales.Totalfactura;
            newreg.decimalesmonedas = viewmodel.Sumatotales.Decimalesmonedas;
            result.RegistroIVARepercutidoSumaTotales.Add(newreg);*/

            return result;
        }

        public override IModelView GetModelView(RegistroIVARepercutido viewmodel)
        {
            var result = base.GetModelView(viewmodel) as RegistroIvaRepercutidoModel;

            return result;
        }
    }
}
