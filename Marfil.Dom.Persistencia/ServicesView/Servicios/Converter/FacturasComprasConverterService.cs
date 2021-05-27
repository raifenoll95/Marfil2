using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Documentos.FacturasCompras;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.GaleriaImagenes;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    class FacturasComprasConverterService : BaseConverterModel<FacturasComprasModel, FacturasCompras>
    {
        public FacturasComprasConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override IEnumerable<IModelView> GetAll()
        {
            return _db.Set<FacturasCompras>().Where(f=>f.empresa==Empresa).ToList().Select(f=> GetModelView(f) as FacturasComprasModel);
            
        }

        public override IModelView CreateView(string id)
        {

          
            var identificador = Funciones.Qint(id);
            var obj = _db.Set<FacturasCompras>().Where(f => f.empresa==Empresa && f.id == identificador).Include(f => f.FacturasComprasLin).Include(f=>f.FacturasComprasTotales).Single();
            var monedasObj = _db.Monedas.Single(f => f.id == obj.fkmonedas);
            
           
            var result = GetModelView(obj) as FacturasComprasModel;

            result.Decimalesmonedas = monedasObj.decimales??2;
            //Lineas
            result.Lineas = obj.FacturasComprasLin.ToList().Select(f => new FacturasComprasLinModel()
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
                , Importenetolinea = f.importenetolinea
                , Notas = f.notas
                , Canal=f.canal
                , Precioanterior = f.precioanterior
                , Revision = f.revision
                , Decimalesmonedas = f.decimalesmonedas
                , Decimalesmedidas = f.decimalesmedidas
                , Fkalbaranes = f.fkalbaranes
                , Fkalbaranesreferencia = f.fkalbaranesreferencia
                , Bundle = f.bundle
                , Tblnum = f.tblnum
                , Contenedor = f.contenedor
                , Sello = f.sello
                , Caja = f.caja
                , Pesoneto = f.pesoneto
                , Pesobruto = f.pesobruto
                , Seccion = f.seccion
                , Costeadicionalvariable = f.costeacicionalvariable
                , Costeadicionalportes = f.costeadicionalportes
                , Costeadicionalmaterial = f.costeadicionalmaterial
                , Costeadicionalotro = f.costeadicionalotro
                , Orden = f.orden ?? f.id
        }).ToList();

            //Totales
            result.Totales = obj.FacturasComprasTotales.ToList().Select(f => new FacturasComprasTotalesModel()
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
                , Baseretencion = f.baseretencion
                , Porcentajeretencion = f.porcentajeretencion
                , Importeretencion = f.importeretencion
                , Subtotal = f.subtotal
                , Decimalesmonedas = f.decimalesmonedas
                , Baseconredondeo = result.Valorredondeo.HasValue && result.Valorredondeo!=0
            }).ToList();


            //vencimientos
            result.Vencimientos = obj.FacturasComprasVencimientos.ToList().Select(f=> new FacturasComprasVencimientosModel()
            {
               Id= f.id,
               Diasvencimiento = f.diasvencimiento,
               Fechavencimiento = f.fechavencimiento,
               Importevencimiento = f.importevencimiento,
               Decimalesmonedas = result.Decimalesmonedas

            }).ToList();

            

            return result;
        }

        public override FacturasCompras CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as FacturasComprasModel;
            var result = _db.Set<FacturasCompras>().Create();
           
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
            result.dua = viewmodel.Dua;
            
            viewmodel.Fkseries = result.fkseries;

            foreach (var item in viewmodel.Lineas)
            {
                var newItem= _db.Set<FacturasComprasLin>().Create();
                newItem.empresa = Empresa;
                newItem.fkfacturascompras = result.id;
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
                newItem.porcentajerecargoequivalencia = item.Porcentajerecargoequivalencia;
                newItem.cuotarecargoequivalencia = item.Cuotarecargoequivalencia;
                newItem.importe = item.Importe;
                newItem.notas = item.Notas;
                newItem.canal = item.Canal;
                newItem.precioanterior = item.Precioanterior;
                newItem.revision = item.Revision;
                newItem.decimalesmonedas = item.Decimalesmonedas;
                newItem.decimalesmedidas = item.Decimalesmedidas;
                newItem.fkalbaranes = item.Fkalbaranes;
                newItem.fkalbaranesfecha = item.Fkalbaranesfecha;
                newItem.fkalbaranesreferencia = item.Fkalbaranesreferencia;
                newItem.bundle = item.Bundle?.ToUpper();
                newItem.tblnum = item.Tblnum;
                newItem.contenedor = item.Contenedor;
                newItem.sello = item.Sello;
                newItem.caja = item.Caja;
                newItem.pesoneto = item.Pesoneto;
                newItem.pesobruto = item.Pesobruto;
                newItem.seccion = item.Seccion;
                newItem.costeadicionalportes = item.Costeadicionalportes;
                newItem.costeacicionalvariable = item.Costeadicionalvariable;
                newItem.costeadicionalmaterial = item.Costeadicionalmaterial;
                newItem.costeadicionalotro = item.Costeadicionalotro;
                
                result.FacturasComprasLin.Add(newItem);
            }


            foreach (var item in viewmodel.Totales)
            {
                var newItem = _db.Set<FacturasComprasTotales>().Create();
                newItem.empresa = Empresa;
                newItem.fkfacturascompras = result.id;
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
                newItem.baseretencion = item.Baseretencion;
                newItem.porcentajeretencion = item.Porcentajeretencion;
                newItem.importeretencion = item.Importeretencion;
                newItem.subtotal = item.Subtotal;
                newItem.decimalesmonedas = item.Decimalesmonedas;
                
                result.FacturasComprasTotales.Add(newItem);
            }

            foreach (var item in viewmodel.Vencimientos)
            {
                var newItem = _db.Set<FacturasComprasVencimientos>().Create();
                newItem.empresa = Empresa;
                newItem.fkfacturascompras = result.id;
                newItem.id = item.Id;
                newItem.diasvencimiento = item.Diasvencimiento;
                newItem.fechavencimiento = item.Fechavencimiento;
                newItem.importevencimiento = item.Importevencimiento;
                result.FacturasComprasVencimientos.Add(newItem);
            }

            return result;
        }

        public override FacturasCompras EditPersitance(IModelView obj)
        {
            var viewmodel = obj as FacturasComprasModel;
            var result = _db.FacturasCompras.Where(f => f.empresa == viewmodel.Empresa && f.id == viewmodel.Id).Include(b => b.FacturasComprasLin).Include(b => b.FacturasComprasTotales).ToList().Single();
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
            result.fkusuariomodificacion  = Context.Id;
            result.fkpuertosfkpaises = viewmodel.Fkpuertos.Fkpaises;
            result.fkpuertosid = viewmodel.Fkpuertos.Id;
            result.dua = viewmodel.Dua;

            result.FacturasComprasLin.Clear();
            foreach (var item in viewmodel.Lineas)
            {
                var newItem = _db.Set<FacturasComprasLin>().Create();
                newItem.empresa = result.empresa;
                newItem.fkfacturascompras = result.id;
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
                newItem.porcentajerecargoequivalencia = item.Porcentajerecargoequivalencia;
                newItem.cuotarecargoequivalencia = item.Cuotarecargoequivalencia;
                newItem.importe = item.Importe;
                newItem.notas = item.Notas;
                newItem.canal = item.Canal;
                newItem.precioanterior = item.Precioanterior;
                newItem.revision = item.Revision;
                newItem.decimalesmonedas = item.Decimalesmonedas;
                newItem.decimalesmedidas = item.Decimalesmedidas;
                newItem.fkalbaranes = item.Fkalbaranes;
                newItem.fkalbaranesfecha = item.Fkalbaranesfecha;
                newItem.fkalbaranesreferencia = item.Fkalbaranesreferencia;
                newItem.bundle = item.Bundle?.ToUpper();
                newItem.tblnum = item.Tblnum;
                newItem.contenedor = item.Contenedor;
                newItem.sello = item.Sello;
                newItem.caja = item.Caja;
                newItem.pesoneto = item.Pesoneto;
                newItem.pesobruto = item.Pesobruto;
                newItem.seccion = item.Seccion;
                newItem.costeadicionalportes = item.Costeadicionalportes;
                newItem.costeacicionalvariable = item.Costeadicionalvariable;
                newItem.costeadicionalmaterial = item.Costeadicionalmaterial;
                newItem.costeadicionalotro = item.Costeadicionalotro;
                newItem.orden = item.Orden;
                result.FacturasComprasLin.Add(newItem);
            }

            result.FacturasComprasTotales.Clear();
            foreach (var item in viewmodel.Totales)
            {
                var newItem = _db.Set<FacturasComprasTotales>().Create();
                newItem.empresa = result.empresa;
                newItem.fkfacturascompras = result.id;
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
                newItem.baseretencion = item.Baseretencion;
                newItem.porcentajeretencion = item.Porcentajeretencion;
                newItem.importeretencion = item.Importeretencion;
                newItem.subtotal = item.Subtotal;
                newItem.decimalesmonedas = item.Decimalesmonedas;
                result.FacturasComprasTotales.Add(newItem);
            }

            result.FacturasComprasVencimientos.Clear();
            foreach (var item in viewmodel.Vencimientos)
            {
                var newItem = _db.Set<FacturasComprasVencimientos>().Create();
                newItem.empresa = Empresa;
                newItem.fkfacturascompras = result.id;
                newItem.id = item.Id;
                newItem.diasvencimiento = item.Diasvencimiento;
                newItem.fechavencimiento = item.Fechavencimiento;
                newItem.importevencimiento = item.Importevencimiento;
                result.FacturasComprasVencimientos.Add(newItem);
            }
            return result;
        }

        public override IModelView GetModelView(FacturasCompras obj)
        {
            var result= base.GetModelView(obj) as FacturasComprasModel;

            result.Fkpuertos.Fkpaises = obj.fkpuertosfkpaises;
            result.Fkpuertos.Id = obj.fkpuertosid;

            return result;
        }
    }
}
