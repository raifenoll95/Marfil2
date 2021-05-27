using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Documentos.Reservasstock;
using Marfil.Dom.Persistencia.Model.Documentos.Presupuestos;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.GaleriaImagenes;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Inf.Genericos;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    class ReservasstockConverterService : BaseConverterModel<ReservasstockModel, Reservasstock>
    {
        public string Ejercicio { get; set; }

        public ReservasstockConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override IEnumerable<IModelView> GetAll()
        {
            var result = _db.Set<Reservasstock>().Where(f => f.empresa == Empresa).ToList().Select(f => GetModelView(f) as ReservasstockModel).ToList();
            using (var serviceCriterios = FService.Instance.GetService(typeof(CriteriosagrupacionModel), Context))
            {
                var criterioslist = serviceCriterios.getAll().Select(f => (CriteriosagrupacionModel)f);
                foreach (var item in result)
                {
                    item.Descripcioncriterioagrupacion = criterioslist.SingleOrDefault(f => f.Id == item.Fkcriteriosagrupacion)?.Nombre;

                }
            }

            return result;
        }

        public override IModelView CreateView(string id)
        {

            var identificador = Funciones.Qint(id);
            var obj = _db.Set<Reservasstock>().Where(f => f.empresa == Empresa && f.id == identificador).Include(f => f.ReservasstockLin).Include(f => f.ReservasstockTotales).Single();
            var monedasObj = _db.Monedas.Single(f => f.id == obj.fkmonedas);

            var result = GetModelView(obj) as ReservasstockModel;

            

           
            result.Tipodeportes = (Tipoportes?)obj.tipoportes;
            result.Decimalesmonedas = monedasObj.decimales ?? 2;
            //Lineas
            result.Lineas = obj.ReservasstockLin.ToList().Select(f => new ReservasstockLinModel()
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
                Fkpedidosreferencia = f.fkpedidosreferencia,
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
                Orden = f.orden ?? f.id,
                EnFactura = _db.AlbaranesLin.Any(j => j.empresa == Empresa && j.fkpedidos == result.Id),
                Fkfacturasreferencia = _db.Albaranes.Include("AlbaranesLin").Where(j => j.empresa == Empresa && j.AlbaranesLin.Any(h => h.empresa == Empresa && h.fkpedidos == result.Id)).ToList().Select(h => new StDocumentoReferencia() { CampoId = string.Format("{0}", h.id), Referencia = h.referencia }).ToList(),
                Flagidentifier = f.flagidentifier

            }).ToList();

            //Totales
            result.Totales = obj.ReservasstockTotales.ToList().Select(f => new ReservasstockTotalesModel()
            {
                Fktiposiva = f.fktiposiva
                ,
                Porcentajeiva = f.porcentajeiva
                ,
                Baseimponible = f.basetotal
                ,
                Brutototal = f.brutototal
                ,
                Cuotaiva = f.cuotaiva
                ,
                Porcentajerecargoequivalencia = f.porcentajerecargoequivalencia
                ,
                Importerecargoequivalencia = f.importerecargoequivalencia
                ,
                Porcentajedescuentoprontopago = f.porcentajedescuentoprontopago
                ,
                Importedescuentoprontopago = f.importedescuentoprontopago
                ,
                Porcentajedescuentocomercial = f.porcentajedescuentocomercial
                ,
                Importedescuentocomercial = f.importedescuentocomercial
                ,
                Subtotal = f.subtotal
                ,
                Decimalesmonedas = f.decimalesmonedas
            }).ToList();

           

            return result;
        }

        public override Reservasstock CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as ReservasstockModel;
            var result = _db.Set<Reservasstock>().Create();

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
            result.tipoportes = (int?)viewmodel.Tipodeportes;
            result.empresa = Empresa;
           
            foreach (var item in viewmodel.Lineas)
            {
                var newItem = _db.Set<ReservasstockLin>().Create();
                newItem.empresa = Empresa;
                newItem.fkreservasstock = result.id;
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
                newItem.fkpedidos = item.Fkpedidos;
                newItem.fkpedidosid = item.Fkpedidosid;
                newItem.fkpedidosreferencia = item.Fkpedidosreferencia;
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
                newItem.flagidentifier = Guid.NewGuid();
                result.ReservasstockLin.Add(newItem);
            }


            foreach (var item in viewmodel.Totales)
            {
                var newItem = _db.Set<ReservasstockTotales>().Create();
                newItem.empresa = Empresa;
                newItem.fkreservasstock = result.id;
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

                result.ReservasstockTotales.Add(newItem);
            }

            return result;
        }

        public override Reservasstock EditPersitance(IModelView obj)
        {
            var viewmodel = obj as ReservasstockModel;
            var result = _db.Reservasstock.Where(f => f.empresa == viewmodel.Empresa && f.id == viewmodel.Id).Include(b => b.ReservasstockLin).Include(b => b.ReservasstockTotales).ToList().Single();
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
            result.tipoportes = (int?)viewmodel.Tipodeportes;
            result.ReservasstockLin.Clear();
            foreach (var item in viewmodel.Lineas)
            {
                var newItem = _db.Set<ReservasstockLin>().Create();
                newItem.empresa = result.empresa;
                newItem.fkreservasstock = result.id;
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
                newItem.fkpedidos = item.Fkpedidos;
                newItem.fkpedidosid = item.Fkpedidosid;
                newItem.fkpedidosreferencia = item.Fkpedidosreferencia;
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
                newItem.flagidentifier = item.Flagidentifier;
                result.ReservasstockLin.Add(newItem);
            }

            result.ReservasstockTotales.Clear();
            foreach (var item in viewmodel.Totales)
            {
                var newItem = _db.Set<ReservasstockTotales>().Create();
                newItem.empresa = result.empresa;
                newItem.fkreservasstock = result.id;
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
                result.ReservasstockTotales.Add(newItem);
            }
            return result;
        }

        public override IModelView GetModelView(Reservasstock obj)
        {
            var result = base.GetModelView(obj) as ReservasstockModel;

            result.Fkpuertos.Fkpaises = obj.fkpuertosfkpaises;
            result.Fkpuertos.Id = obj.fkpuertosid;

            return result;
        }
    }
}
