using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Documentos.PresupuestosCompras;
using Marfil.Dom.Persistencia.Model.Ficheros;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.GaleriaImagenes;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    class PresupuestosComprasConverterService : BaseConverterModel<PresupuestosComprasModel, PresupuestosCompras>
    {


        public PresupuestosComprasConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override IEnumerable<IModelView> GetAll()
        {
           return _db.Set<PresupuestosCompras>().Where(f => f.empresa == Empresa).ToList().Select(f=>GetModelView(f) as PresupuestosComprasModel);
        }

        public override IModelView CreateView(string id)
        {
            
            var cp = Funciones.Qint(id);
            var obj = _db.Set<PresupuestosCompras>().Where(f => f.empresa ==Empresa && f.id == cp).Include(f => f.PresupuestosComprasLin).Include(f=>f.PresupuestosComprasTotales).Single();
            var monedasObj = _db.Monedas.Single(f => f.id == obj.fkmonedas);
            
           
            var result = GetModelView(obj) as PresupuestosComprasModel;
            
            result.Decimalesmonedas = monedasObj.decimales??2;
            //Lineas
            result.Lineas = obj.PresupuestosComprasLin.ToList().Select(f => new PresupuestosComprasLinModel()
            {
                Id=f.id
                , Fkarticulos = f.fkarticulos
                , Descripcion=f.descripcion
                , Lote= f.lote
                , Tabla= f.tabla
                , Cantidad = f.cantidad
                , Cantidadpedida = f.cantidadpedida
                , Largo = f.largo
                , Ancho = f.ancho
                , Grueso =  f.grueso
                , Fkunidades = f.fkunidades
                , Metros = f.metros
                , Precio = f.precio
                , Porcentajedescuento = f.porcentajedescuento
                , Importedescuento = f.importedescuento
                , Fkregimeniva = result.Fkregimeniva
                , Fktiposiva = f.fktiposiva
                , Porcentajeiva = f.porcentajeiva
                , Cuotaiva = f.cuotaiva
                , Porcentajerecargoequivalencia = f.porcentajerecargoequivalencia
                , Cuotarecargoequivalencia = f.cuotarecargoequivalencia
                , Importe = f.importe
                , Notas = f.notas
                , Canal=f.canal
                , Precioanterior = f.precioanterior
                , Revision = f.revision
                , Decimalesmonedas = f.decimalesmonedas
                , Decimalesmedidas = f.decimalesmedidas
                , EnPedido = _db.PedidosComprasLin.Any(j => j.empresa == Empresa && j.fkpresupuestos == result.Id )
                , Fkpedidoreferencia = _db.PedidosCompras.Include("PedidosComprasLin").Where(j=> j.empresa == Empresa && j.PedidosComprasLin.Any(h=> h.empresa == Empresa && h.fkpresupuestos == result.Id)).ToList().Select(h=> new StDocumentoReferencia() { CampoId= string.Format("{0}",h.id) , Referencia = h.referencia}).ToList()
                , Orden = f.orden?? f.id
            }).ToList();

            //Totales
            result.Totales = obj.PresupuestosComprasTotales.ToList().Select(f => new PresupuestosComprasTotalesModel()
            {
                Fktiposiva= f.fktiposiva
                , Porcentajeiva = f.porcentajeiva
                , Baseimponible = f.basetotal
                , Brutototal = f.brutototal
                , Cuotaiva = f.cuotaiva
                , Porcentajerecargoequivalencia = f.porcentajerecargoequivalencia
                , Importerecargoequivalencia = f.importerecargoequivalencia
                , Porcentajedescuentoprontopago = f.porcentajedescuentoprontopago
                , Importedescuentoprontopago = f.importedescuentoprontopago
                , Porcentajedescuentocomercial = f.porcentajedescuentocomercial
                , Importedescuentocomercial = f.importedescuentocomercial
                , Subtotal = f.subtotal
                , Decimalesmonedas = f.decimalesmonedas
            }).ToList();

           


            

            return result;
        }

        public override PresupuestosCompras CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as PresupuestosComprasModel;
            var result = _db.Set<PresupuestosCompras>().Create();
           
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
            result.id = -1;
            result.fechaalta = DateTime.Now;
            result.fechamodificacion = result.fechaalta;
            result.fkusuarioalta = Context.Id;
            result.fkusuariomodificacion = Context.Id;
            result.fkpuertosfkpaises = viewmodel.Fkpuertos.Fkpaises;
            result.fkpuertosid = viewmodel.Fkpuertos.Id;

            result.empresa = Empresa;
           
            foreach (var item in viewmodel.Lineas)
            {
                var newItem= _db.Set<PresupuestosComprasLin>().Create();
                newItem.empresa = Empresa;
                newItem.fkpresupuestoscompras = result.id;
                newItem.id = item.Id;
                newItem.fkarticulos = item.Fkarticulos;
                newItem.descripcion = item.Descripcion;
                newItem.lote = item.Lote;
                newItem.tabla = item.Tabla;
                newItem.cantidad = item.Cantidad;
                newItem.cantidadpedida = item.Cantidadpedida;
                newItem.largo = item.Largo;
                newItem.ancho = item.Ancho;
                newItem.grueso = item.Grueso;
                newItem.fkunidades = item.Fkunidades;
                newItem.metros = item.Metros;
                newItem.precio = item.Precio;
                newItem.porcentajedescuento = item.Porcentajedescuento;
                newItem.importedescuento = item.Importedescuento;
                newItem.fktiposiva = item.Fktiposiva;
                newItem.porcentajeiva = item.Porcentajeiva;
                newItem.cuotaiva = item.Cuotaiva;
                newItem.porcentajerecargoequivalencia = item.Porcentajerecargoequivalencia ??0;
                newItem.cuotarecargoequivalencia = item.Cuotarecargoequivalencia;
                newItem.importe = item.Importe;
                newItem.notas = item.Notas;
                newItem.canal = item.Canal;
                newItem.precioanterior = item.Precioanterior;
                newItem.revision = item.Revision;
                newItem.decimalesmonedas = item.Decimalesmonedas;
                newItem.decimalesmedidas = item.Decimalesmedidas;
                newItem.orden = item.Orden;
                result.PresupuestosComprasLin.Add(newItem);
            }


            foreach (var item in viewmodel.Totales)
            {
                var newItem = _db.Set<PresupuestosComprasTotales>().Create();
                newItem.empresa = Empresa;
                newItem.fkpresupuestoscompras = result.id;
                newItem.fktiposiva = item.Fktiposiva;
                newItem.porcentajeiva = item.Porcentajeiva;
                newItem.basetotal = item.Baseimponible;
                newItem.brutototal = item.Brutototal;
                newItem.cuotaiva = item.Cuotaiva;
                newItem.porcentajerecargoequivalencia = item.Porcentajerecargoequivalencia;
                newItem.importerecargoequivalencia = item.Importerecargoequivalencia;
                newItem.porcentajedescuentoprontopago = item.Porcentajedescuentoprontopago;
                newItem.importedescuentoprontopago = item.Importedescuentoprontopago;
                newItem.porcentajedescuentocomercial = item.Porcentajedescuentocomercial;
                newItem.importedescuentocomercial = item.Importedescuentocomercial;
                newItem.subtotal = item.Subtotal;
                newItem.decimalesmonedas = item.Decimalesmonedas;
                
                result.PresupuestosComprasTotales.Add(newItem);
            }




            return result;
        }

        public override PresupuestosCompras EditPersitance(IModelView obj)
        {
            var viewmodel = obj as PresupuestosComprasModel;
            var result = _db.Set<PresupuestosCompras>().Where(f => f.empresa == Empresa && f.id == viewmodel.Id).Include(f => f.PresupuestosComprasLin).Include(f => f.PresupuestosComprasTotales).Single();
            //todo asignar
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
            //todo asignar contador y referencia
            result.fechamodificacion = DateTime.Now;
            result.fkusuariomodificacion = Context.Id;
            result.fkpuertosfkpaises = viewmodel.Fkpuertos.Fkpaises;
            result.fkpuertosid = viewmodel.Fkpuertos.Id;

            result.PresupuestosComprasLin.Clear();
            foreach (var item in viewmodel.Lineas)
            {
                var newItem = _db.Set<PresupuestosComprasLin>().Create();
                newItem.empresa = result.empresa;
                newItem.fkpresupuestoscompras = result.id;
                newItem.id = item.Id;
                newItem.fkarticulos = item.Fkarticulos;
                newItem.descripcion = item.Descripcion;
                newItem.lote = item.Lote;
                newItem.tabla = item.Tabla;
                newItem.cantidad = item.Cantidad;
                newItem.cantidadpedida = item.Cantidadpedida;
                newItem.largo = item.Largo;
                newItem.ancho = item.Ancho;
                newItem.grueso = item.Grueso;
                newItem.fkunidades = item.Fkunidades;
                newItem.metros = item.Metros;
                newItem.precio = item.Precio;
                newItem.porcentajedescuento = item.Porcentajedescuento;
                newItem.importedescuento = item.Importedescuento;
                newItem.fktiposiva = item.Fktiposiva;
                newItem.porcentajeiva = item.Porcentajeiva;
                newItem.cuotaiva = item.Cuotaiva;
                newItem.porcentajerecargoequivalencia = item.Porcentajerecargoequivalencia ?? 0;
                newItem.cuotarecargoequivalencia = item.Cuotarecargoequivalencia;
                newItem.importe = item.Importe;
                newItem.notas = item.Notas;
                newItem.canal = item.Canal;
                newItem.precioanterior = item.Precioanterior;
                newItem.revision = item.Revision;
                newItem.decimalesmonedas = item.Decimalesmonedas;
                newItem.decimalesmedidas = item.Decimalesmedidas;
                newItem.orden = item.Orden;
                result.PresupuestosComprasLin.Add(newItem);
            }

            result.PresupuestosComprasTotales.Clear();
            foreach (var item in viewmodel.Totales)
            {
                var newItem = _db.Set<PresupuestosComprasTotales>().Create();
                newItem.empresa = result.empresa;
                newItem.fkpresupuestoscompras = result.id;
                newItem.fktiposiva = item.Fktiposiva;
                newItem.porcentajeiva = item.Porcentajeiva;
                newItem.basetotal = item.Baseimponible;
                newItem.brutototal = item.Brutototal;
                newItem.cuotaiva = item.Cuotaiva;
                newItem.porcentajerecargoequivalencia = item.Porcentajerecargoequivalencia;
                newItem.importerecargoequivalencia = item.Importerecargoequivalencia;
                newItem.porcentajedescuentoprontopago = item.Porcentajedescuentoprontopago;
                newItem.importedescuentoprontopago = item.Importedescuentoprontopago;
                newItem.porcentajedescuentocomercial = item.Porcentajedescuentocomercial;
                newItem.importedescuentocomercial = item.Importedescuentocomercial;
                newItem.subtotal = item.Subtotal;
                newItem.decimalesmonedas = item.Decimalesmonedas;
                result.PresupuestosComprasTotales.Add(newItem);
            }
            return result;
        }

        public override IModelView GetModelView(PresupuestosCompras obj)
        {
            var result= base.GetModelView(obj) as PresupuestosComprasModel;

            result.Fkpuertos.Fkpaises = obj.fkpuertosfkpaises;
            result.Fkpuertos.Id = obj.fkpuertosid;

            return result;
        }
    }
}
