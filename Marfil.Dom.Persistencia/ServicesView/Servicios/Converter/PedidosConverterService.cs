using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Documentos.Pedidos;
using Marfil.Dom.Persistencia.Model.Documentos.Presupuestos;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.GaleriaImagenes;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos;
using Marfil.Inf.Genericos.Helper;
using Marfil.Dom.Persistencia.Model.Documentos.Albaranes;
using DOMAlbaranesComprasModel = Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    class PedidosConverterService : BaseConverterModel<PedidosModel, Pedidos>
    {
        public PedidosConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override IEnumerable<IModelView> GetAll()
        {
            return _db.Set<Pedidos>().Where(f=>f.empresa==Empresa).ToList().Select(f=>GetModelView(f) as PedidosModel);
        }

        public override IModelView CreateView(string id)
        {
            var identificador = Funciones.Qint(id);
            var obj = _db.Set<Pedidos>().Where(f => f.empresa == Empresa && identificador== f.id).Include(f => f.PedidosLin).Include(f=>f.PedidosTotales).Include(f => f.PedidosCostesFabricacion).Single();            
            var monedasObj = _db.Monedas.Single(f => f.id == obj.fkmonedas);
            
           
            var result = GetModelView(obj) as PedidosModel;

            result.Decimalesmonedas = monedasObj.decimales??2;
            //Lineas
            result.Lineas = obj.PedidosLin.ToList().Select(f => new PedidosLinModel()
            {
                Id = f.id
                , Fkarticulos = f.fkarticulos
                , Descripcion = f.descripcion
                , Lote = f.lote
                , Tabla = f.tabla
                , Cantidad = f.cantidad
                , Cantidadpedida = f.cantidadpedida
                , Largo = f.largo
                , Ancho = f.ancho
                , Grueso = f.grueso
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
                , Canal = f.canal
                , Precioanterior = f.precioanterior
                , Revision = f.revision
                , Decimalesmonedas = f.decimalesmonedas
                , Decimalesmedidas = f.decimalesmedidas
                , Fkpresupuestos = f.fkpresupuestos
                , Fkpresupuestosid = f.fkpresupuestosid
                , Fkpresupuestosreferencia = f.fkpresupuestosreferencia
                , EnAlbaran = _db.AlbaranesLin.Any(j => j.empresa == Empresa && j.fkpedidos == obj.id && j.fkpedidosid == f.id) || _db.Albaranes.Any(j => j.empresa == obj.empresa && j.fkpedidos == result.Referencia)
                , Fkalbaranreferencia = GetReferenciasDocumentos(obj, f)
                , idAlbaranSalidasVarias = _db.Series.Where(g => g.empresa == Empresa && g.salidasvarias == true).Select(g => g.id).SingleOrDefault()
                , Orden = f.orden ?? f.id
            }).ToList();

            //Totales
            result.Totales = obj.PedidosTotales.ToList().Select(f => new PedidosTotalesModel()
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

            //CostesFabricación
            result.CostesFabricacion = obj.PedidosCostesFabricacion.ToList().Select(f => new PedidosCostesFabricacionModel()
            {
                Id = f.id
                , Fecha = f.fecha
                , Fkoperario = f.fkoperario
                , DescripcionOperario = _db.Cuentas.Where(j => j.empresa == Empresa && j.id == f.fkoperario).Select(j => j.descripcion).SingleOrDefault()
                , Fktarea = f.fktarea
                , Descripcion = f.descripcion
                , Cantidad = (float)f.cantidad
                , Precio = (float)f.precio
                , Total = (float)f.total
            }).ToList();

            var materiales = _db.Albaranes.Where(f => f.empresa == result.Empresa && f.fkpedidos == result.Referencia && f.tipoalbaran == (int)TipoAlbaran.ImputacionMateriales).Include(f => f.AlbaranesLin).Include(f => f.AlbaranesTotales);

            result.ImputacionMateriales = materiales.SelectMany(f => f.AlbaranesLin).Select(f => new AlbaranesLinModel()
            {
                Id = f.id
                ,
                Fkarticulos = f.fkarticulos
                ,
                Descripcion = f.descripcion
                ,
                Lote = f.lote
                ,
                Tabla = f.tabla
                ,
                Cantidad = f.cantidad
                ,
                Cantidadpedida = f.cantidadpedida
                ,
                Largo = f.largo
                ,
                Ancho = f.ancho
                ,
                Grueso = f.grueso
                ,
                Fkunidades = f.fkunidades
                ,
                Metros = f.metros
                ,
                Precio = f.precio
                ,
                Porcentajedescuento = f.porcentajedescuento
                ,
                Importedescuento = f.importedescuento
                ,
                Fkregimeniva = result.Fkregimeniva
                ,
                Fktiposiva = f.fktiposiva
                ,
                Porcentajeiva = f.porcentajeiva
                ,
                Cuotaiva = f.cuotaiva
                ,
                Porcentajerecargoequivalencia = f.porcentajerecargoequivalencia
                ,
                Cuotarecargoequivalencia = f.cuotarecargoequivalencia
                ,
                Importe = f.importe
                ,
                Notas = f.notas
                ,
                Canal = f.canal
                ,
                Precioanterior = f.precioanterior
                ,
                Revision = f.revision
                ,
                Decimalesmonedas = f.decimalesmonedas
                ,
                Decimalesmedidas = f.decimalesmedidas
                ,
                Fkpedidos = f.fkpedidos
                ,
                Fkpedidosid = f.fkpedidosid
                ,
                Fkpedidosreferencia = f.fkpedidosreferencia
                ,
                Bundle = f.bundle
                ,
                Tblnum = f.tblnum
                ,
                Contenedor = f.contenedor
                ,
                Sello = f.sello
                ,
                Caja = f.caja
                ,
                Pesoneto = f.pesoneto
                ,
                Pesobruto = f.pesobruto
                ,
                Seccion = f.seccion
                ,
                Costeadicionalvariable = f.costeacicionalvariable
                ,
                Costeadicionalportes = f.costeadicionalportes
                ,
                Costeadicionalmaterial = f.costeadicionalmaterial
                ,
                Costeadicionalotro = f.costeadicionalotro
                ,
                Orden = f.orden ?? f.id
                ,
                Fkalbaranes = f.fkalbaranes
            }).ToList();

            result.ImputacionMaterialesTotales = materiales.SelectMany(f => f.AlbaranesTotales).GroupBy(f => f.fktiposiva).Select(f => new AlbaranesTotalesModel()
            {
                Fktiposiva = f.Key
                ,
                Porcentajeiva = f.Min(g => g.porcentajeiva)
                ,
                Baseimponible = f.Sum(g => g.basetotal)
                ,
                Brutototal = f.Sum(g => g.brutototal)
                ,
                Cuotaiva = f.Sum(g => g.cuotaiva)
                ,
                Porcentajerecargoequivalencia = f.Average(g => g.porcentajerecargoequivalencia)
                ,
                Importerecargoequivalencia = f.Sum(g => g.importerecargoequivalencia)
                ,
                Porcentajedescuentoprontopago = f.Average(g => g.porcentajedescuentoprontopago)
                ,
                Importedescuentoprontopago = f.Sum(g => g.importedescuentoprontopago)
                ,
                Porcentajedescuentocomercial = f.Average(g => g.porcentajedescuentocomercial)
                ,
                Importedescuentocomercial = f.Sum(g => g.importedescuentocomercial)
                ,
                Subtotal = f.Sum(g => g.subtotal)
                ,
                Decimalesmonedas = f.Max(g => g.decimalesmonedas) ?? 2
            }).ToList();


            return result;
        }

        private List<StDocumentoReferencia> GetReferenciasDocumentos(Pedidos obj,PedidosLin f)
        {
            var result =
                _db.Albaranes.Include("AlbaranesLin")
                    .Where(
                        j =>
                            j.empresa == Empresa &&
                            j.AlbaranesLin.Any(
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

            result.AddRange(_db.Albaranes.Where(d=>d.empresa==obj.empresa && d.fkpedidos==obj.referencia).ToList().Select(h=> new StDocumentoReferencia()
            {
                CampoId = string.Format("{0}", h.id),
                Referencia = h.referencia
            }));

            return result;
        }

        public override Pedidos CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as PedidosModel;
            var result = _db.Set<Pedidos>().Create();
           
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
                var newItem= _db.Set<PedidosLin>().Create();
                newItem.empresa = Empresa;
                newItem.fkpedidos = result.id;
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
                result.PedidosLin.Add(newItem);
            }


            foreach (var item in viewmodel.Totales)
            {
                var newItem = _db.Set<PedidosTotales>().Create();
                newItem.empresa = Empresa;
                newItem.fkpedidos = result.id;
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
                
                result.PedidosTotales.Add(newItem);
            }

            foreach (var item in viewmodel.CostesFabricacion) {
                var newItem = _db.Set<PedidosCostesFabricacion>().Create();
                newItem.empresa = item.Empresa;
                newItem.fkpedido = item.Fkpedido;
                newItem.id = item.Id;
                newItem.fecha = item.Fecha;
                newItem.fkoperario = item.Fkoperario;
                newItem.fktarea = item.Fktarea;
                newItem.descripcion = item.Descripcion;
                newItem.cantidad = item.Cantidad;
                newItem.precio = item.Precio;
                newItem.total = item.Total;

                result.PedidosCostesFabricacion.Add(newItem);             
            }

            return result;
        }

        public override Pedidos EditPersitance(IModelView obj)
        {
            var viewmodel = obj as PedidosModel;
            var result = _db.Pedidos.Where(f =>f.empresa==viewmodel.Empresa && f.id == viewmodel.Id).Include(b => b.PedidosLin).Include(b => b.PedidosTotales).Include(b => b.PedidosCostesFabricacion).ToList().Single();
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

            result.PedidosLin.Clear();
            foreach (var item in viewmodel.Lineas)
            {
                var newItem = _db.Set<PedidosLin>().Create();
                newItem.empresa = result.empresa;
                newItem.fkpedidos = result.id;
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
                result.PedidosLin.Add(newItem);
            }

            result.PedidosTotales.Clear();
            foreach (var item in viewmodel.Totales)
            {
                var newItem = _db.Set<PedidosTotales>().Create();
                newItem.empresa = result.empresa;
                newItem.fkpedidos = result.id;
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
                result.PedidosTotales.Add(newItem);
            }

            result.PedidosCostesFabricacion.Clear();
            foreach (var item in viewmodel.CostesFabricacion)
            {
                var newItem = _db.Set<PedidosCostesFabricacion>().Create();
                newItem.empresa = item.Empresa;
                newItem.fkpedido = item.Fkpedido;
                newItem.id = item.Id;
                newItem.fecha = item.Fecha;
                newItem.fkoperario = item.Fkoperario;
                newItem.fktarea = item.Fktarea;
                newItem.descripcion = item.Descripcion;
                newItem.cantidad = item.Cantidad;
                newItem.precio = item.Precio;
                newItem.total = item.Total;

                result.PedidosCostesFabricacion.Add(newItem);
            }
            return result;           
        }

        public override IModelView GetModelView(Pedidos obj)
        {
            var result= base.GetModelView(obj) as PedidosModel;

            result.Fkpuertos.Fkpaises = obj.fkpuertosfkpaises;
            result.Fkpuertos.Id = obj.fkpuertosid;

            return result;
        }
    }
}
