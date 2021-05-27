using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Documentos.PedidosCompras;
using Marfil.Dom.Persistencia.Model.Documentos.Presupuestos;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.GaleriaImagenes;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    class PedidosComprasConverterService : BaseConverterModel<PedidosComprasModel, PedidosCompras>
    {
        public PedidosComprasConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override IEnumerable<IModelView> GetAll()
        {
            return _db.Set<PedidosCompras>().Where(f=>f.empresa==Empresa).ToList().Select(f=>GetModelView(f) as PedidosComprasModel);
        }

        public override IModelView CreateView(string id)
        {
            var identificador = Funciones.Qint(id);
            var obj = _db.Set<PedidosCompras>().Where(f => f.empresa == Empresa && identificador== f.id).Include(f => f.PedidosComprasLin).Include(f=>f.PedidosComprasTotales).Single();
            var monedasObj = _db.Monedas.Single(f => f.id == obj.fkmonedas);
            
           
            var result = GetModelView(obj) as PedidosComprasModel;

            result.Decimalesmonedas = monedasObj.decimales??2;
            //Lineas
            result.Lineas = obj.PedidosComprasLin.ToList().Select(f => new PedidosComprasLinModel()
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
                , Fkpresupuestos = f.fkpresupuestos
                , Fkpresupuestosid = f.fkpresupuestosid
                , Fkpresupuestosreferencia = f.fkpresupuestosreferencia
                , EnAlbaran = _db.AlbaranesComprasLin.Any(j => j.empresa == Empresa && j.fkpedidos == obj.id && j.fkpedidosid == f.id)
                ,Fkalbaranreferencia = GetReferenciasDocumentos(obj, f)
                , Orden = f.orden ?? f.id
                , Fkpedidosventas = f.fkpedidosventas
                , Fkpedidosventasreferencia = f.fkpedidosventasreferencia
                
            }).ToList();

            //Totales
            result.Totales = obj.PedidosComprasTotales.ToList().Select(f => new PedidosComprasTotalesModel()
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

        private List<StDocumentoReferencia> GetReferenciasDocumentos(PedidosCompras obj, PedidosComprasLin f)
        {
            var result =
                _db.AlbaranesCompras.Include("AlbaranesComprasLin")
                    .Where(
                        j =>
                            j.empresa == Empresa &&
                            j.AlbaranesComprasLin.Any(
                                h => h.empresa == Empresa && h.fkpedidos == obj.id && h.fkpedidosid == f.id))
                    .ToList()
                    .Select(
                        h =>
                            new StDocumentoReferencia()
                            {
                                CampoId = string.Format("{0}", h.id),
                                Referencia = h.referencia
                            })
                    .ToList();

            result.AddRange(_db.AlbaranesCompras.Where(d => d.empresa == obj.empresa && d.fkpedidoscompras == obj.referencia).ToList().Select(h => new StDocumentoReferencia()
            {
                CampoId = string.Format("{0}", h.id),
                Referencia = h.referencia
            }));

            return result;
        }

        public override PedidosCompras CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as PedidosComprasModel;
            var result = _db.Set<PedidosCompras>().Create();
           
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

            result.fechaalta = DateTime.Now;
            result.fechamodificacion = result.fechaalta;
            result.fkusuarioalta = Context.Id;
            result.fkusuariomodificacion = Context.Id;
            result.fkpuertosfkpaises = viewmodel.Fkpuertos.Fkpaises;
            result.fkpuertosid = viewmodel.Fkpuertos.Id;

            result.empresa = Empresa;
            foreach (var item in viewmodel.Lineas)
            {
                var newItem= _db.Set<PedidosComprasLin>().Create();
                newItem.empresa = Empresa;
                newItem.fkpedidoscompras = result.id;
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
                newItem.fkpresupuestos = item.Fkpresupuestos;
                newItem.fkpresupuestosid = item.Fkpresupuestosid;
                newItem.fkpresupuestosreferencia = item.Fkpresupuestosreferencia;
                newItem.orden = item.Orden;
                newItem.fkpedidosventas = item.Fkpedidosventas;
                newItem.fkpedidosventasreferencia = item.Fkpedidosventasreferencia;
                result.PedidosComprasLin.Add(newItem);
            }


            foreach (var item in viewmodel.Totales)
            {
                var newItem = _db.Set<PedidosComprasTotales>().Create();
                newItem.empresa = Empresa;
                newItem.fkpedidoscompras = result.id;
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
                
                result.PedidosComprasTotales.Add(newItem);
            }

            return result;
        }

        public override PedidosCompras EditPersitance(IModelView obj)
        {
            var viewmodel = obj as PedidosComprasModel;
            var result = _db.PedidosCompras.Where(f =>f.empresa==viewmodel.Empresa && f.id == viewmodel.Id).Include(b => b.PedidosComprasLin).Include(b => b.PedidosComprasTotales).ToList().Single();
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

            result.PedidosComprasLin.Clear();
            foreach (var item in viewmodel.Lineas)
            {
                var newItem = _db.Set<PedidosComprasLin>().Create();
                newItem.empresa = result.empresa;
                newItem.fkpedidoscompras = result.id;
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
                newItem.fkpresupuestos = item.Fkpresupuestos;
                newItem.fkpresupuestosid = item.Fkpresupuestosid;
                newItem.fkpresupuestosreferencia = item.Fkpresupuestosreferencia;
                newItem.orden = item.Orden;
                newItem.fkpedidosventas = item.Fkpedidosventas;
                newItem.fkpedidosventasreferencia = item.Fkpedidosventasreferencia;
                result.PedidosComprasLin.Add(newItem);
            }

            result.PedidosComprasTotales.Clear();
            foreach (var item in viewmodel.Totales)
            {
                var newItem = _db.Set<PedidosComprasTotales>().Create();
                newItem.empresa = result.empresa;
                newItem.fkpedidoscompras = result.id;
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
                result.PedidosComprasTotales.Add(newItem);
            }
            return result;
        }

        public override IModelView GetModelView(PedidosCompras obj)
        {
            var result= base.GetModelView(obj) as PedidosComprasModel;

            result.Fkpuertos.Fkpaises = obj.fkpuertosfkpaises;
            result.Fkpuertos.Id = obj.fkpuertosid;

            return result;
        }
    }
}
