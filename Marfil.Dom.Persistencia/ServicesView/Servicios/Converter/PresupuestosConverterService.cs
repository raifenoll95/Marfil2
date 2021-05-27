using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Documentos.Presupuestos;
using Marfil.Dom.Persistencia.Model.Ficheros;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.GaleriaImagenes;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    class PresupuestosConverterService : BaseConverterModel<PresupuestosModel, Presupuestos>
    {


        public PresupuestosConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override IEnumerable<IModelView> GetAll()
        {
           return _db.Set<Presupuestos>().Where(f => f.empresa == Empresa).ToList().Select(f=>GetModelView(f) as PresupuestosModel);
        }

        public override IModelView CreateView(string id)
        {
            
            var cp = Funciones.Qint(id);
            var obj = _db.Set<Presupuestos>().Where(f => f.empresa ==Empresa && f.id == cp).Include(f => f.PresupuestosLin).Include(f=>f.PresupuestosTotales).Include(f => f.PresupuestosComponentesLin).Single();
            var monedasObj = _db.Monedas.Single(f => f.id == obj.fkmonedas);
            
           
            var result = GetModelView(obj) as PresupuestosModel;
            
            result.Decimalesmonedas = monedasObj.decimales??2;
            //Lineas
            result.Lineas = obj.PresupuestosLin.ToList().Select(f => new PresupuestosLinModel()
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
                , EnPedido = _db.PedidosLin.Any(j => j.empresa == Empresa && j.fkpresupuestos == result.Id )
                , Fkpedidoreferencia = _db.Pedidos.Include("PedidosLin").Where(j=> j.empresa == Empresa && j.PedidosLin.Any(h=> h.empresa == Empresa && h.fkpresupuestos == result.Id)).ToList().Select(h=> new StDocumentoReferencia() { CampoId= string.Format("{0}",h.id) , Referencia = h.referencia}).ToList()
                , Orden = f.orden?? f.id
                , Integridadreferenciaflag = f.integridadreferenciaflag
                , Intaux = f.intaux
            }).ToList();

            //Totales
            result.Totales = obj.PresupuestosTotales.ToList().Select(f => new PresupuestosTotalesModel()
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

            //Componentes
            result.Componentes = obj.PresupuestosComponentesLin.ToList().OrderBy(f => f.idlineaarticulo).Select(f => new PresupuestosComponentesLinModel()
            {
                Fkpresupuestos = f.fkpresupuestos,
                Id = f.id,
                IdComponente = f.idcomponente,
                Integridadreferenciaflag = f.integridadreferenciaflag,
                Descripcioncomponente = f.descripcioncomponente,
                Piezas = f.piezas,
                Largo = f.largo,
                Ancho = f.ancho,
                Grueso = f.grueso,
                Merma = f.merma,
                Precio = f.precio,
                PrecioInicial = f.precioinicial,
                Idlineaarticulo = f.idlineaarticulo
            }).ToList();

            return result;
        }

        public override Presupuestos CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as PresupuestosModel;
            var result = _db.Set<Presupuestos>().Create();
           
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
            result.clientepais = viewmodel.Clientepais;
            result.fechaalta = DateTime.Now;
            result.fechamodificacion = result.fechaalta;
            result.fkusuarioalta = Context.Id;
            result.fkusuariomodificacion = Context.Id;
            result.fkpuertosfkpaises = viewmodel.Fkpuertos.Fkpaises;
            result.fkpuertosid = viewmodel.Fkpuertos.Id;

            result.empresa = Empresa;
           
            foreach (var item in viewmodel.Lineas)
            {
                var newItem= _db.Set<PresupuestosLin>().Create();
                newItem.empresa = Empresa;
                newItem.fkpresupuestos = result.id;
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
                newItem.integridadreferenciaflag = item.Integridadreferenciaflag;
                newItem.intaux = item.Intaux;
                result.PresupuestosLin.Add(newItem);
            }


            foreach (var item in viewmodel.Totales)
            {
                var newItem = _db.Set<PresupuestosTotales>().Create();
                newItem.empresa = Empresa;
                newItem.fkpresupuestos = result.id;
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
                
                result.PresupuestosTotales.Add(newItem);
            }

            foreach (var item in viewmodel.Componentes)
            {
                var newItem = _db.Set<PresupuestosComponentesLin>().Create();
                newItem.empresa = Empresa;
                newItem.fkpresupuestos = result.id;
                newItem.id = item.Id;
                newItem.idcomponente = item.IdComponente;
                newItem.descripcioncomponente = item.Descripcioncomponente;                
                newItem.piezas = item.Piezas;
                newItem.largo = item.Largo;
                newItem.ancho = item.Ancho;
                newItem.grueso = item.Grueso;
                newItem.merma = item.Merma;
                newItem.precio = item.Precio;
                newItem.precioinicial = item.PrecioInicial;
                newItem.idlineaarticulo = item.Idlineaarticulo.Value;

                result.PresupuestosComponentesLin.Add(newItem);
            }

            return result;
        }

        public override Presupuestos EditPersitance(IModelView obj)
        {
            var viewmodel = obj as PresupuestosModel;
            var result = _db.Set<Presupuestos>().Where(f => f.empresa == Empresa && f.id == viewmodel.Id).Include(f => f.PresupuestosLin).Include(f => f.PresupuestosTotales).Single();
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

            result.PresupuestosLin.Clear();
            foreach (var item in viewmodel.Lineas)
            {
                var newItem = _db.Set<PresupuestosLin>().Create();
                newItem.empresa = result.empresa;
                newItem.fkpresupuestos = result.id;
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
                newItem.integridadreferenciaflag = item.Integridadreferenciaflag;
                newItem.intaux = item.Intaux;
                result.PresupuestosLin.Add(newItem);
            }

            result.PresupuestosTotales.Clear();
            foreach (var item in viewmodel.Totales)
            {
                var newItem = _db.Set<PresupuestosTotales>().Create();
                newItem.empresa = result.empresa;
                newItem.fkpresupuestos = result.id;
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
                result.PresupuestosTotales.Add(newItem);
            }

            result.PresupuestosComponentesLin.Clear();
            foreach (var item in viewmodel.Componentes)
            {
                var newItem = _db.Set<PresupuestosComponentesLin>().Create();
                newItem.empresa = Empresa;
                newItem.fkpresupuestos = result.id;
                newItem.id = item.Id;
                newItem.idcomponente = item.IdComponente;
                newItem.integridadreferenciaflag = item.Integridadreferenciaflag;
                newItem.descripcioncomponente = item.Descripcioncomponente; newItem.piezas = item.Piezas;
                newItem.piezas = item.Piezas;
                newItem.largo = item.Largo;
                newItem.ancho = item.Ancho;
                newItem.grueso = item.Grueso;
                newItem.merma = item.Merma;
                newItem.precio = item.Precio;
                newItem.precioinicial = item.PrecioInicial;
                newItem.idlineaarticulo = item.Idlineaarticulo.Value;
                result.PresupuestosComponentesLin.Add(newItem);
            }
            return result;
        }

        public override IModelView GetModelView(Presupuestos obj)
        {
            var result= base.GetModelView(obj) as PresupuestosModel;

            result.Fkpuertos.Fkpaises = obj.fkpuertosfkpaises;
            result.Fkpuertos.Id = obj.fkpuertosid;

            return result;
        }
    }
}
