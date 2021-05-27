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
using Marfil.Dom.Persistencia.Model.Documentos.Albaranes;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Dom.Persistencia.Model.Documentos.Presupuestos;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.GaleriaImagenes;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Inf.Genericos;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    class AlbaranesComprasConverterService : BaseConverterModel<AlbaranesComprasModel, AlbaranesCompras>
    {
        public string Ejercicio { get; set; }

        public AlbaranesComprasConverterService(IContextService context, MarfilEntities db) : base(context,db)
        {
        }

        public override IEnumerable<IModelView> GetAll()
        {
            var result = _db.Set<AlbaranesCompras>().Where(f => f.empresa == Empresa).ToList().Select(f => GetModelView(f) as AlbaranesComprasModel).ToList();
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

        public override bool Exists(string id)
        {
            var intid = Funciones.Qint(id);
            return _db.AlbaranesCompras.Any(f => f.empresa == Context.Empresa && f.id == intid);
        }

        public override IModelView CreateView(string id)
        {

            var identificador = Funciones.Qint(id);
            var obj = _db.Set<AlbaranesCompras>().Where(f => f.empresa == Empresa && f.id == identificador).Include(f => f.AlbaranesComprasLin).Include(f => f.AlbaranesComprasTotales).Single();
            var monedasObj = _db.Monedas.Single(f => f.id == obj.fkmonedas);

            var result = GetModelView(obj) as AlbaranesComprasModel;
            result.IsFacturado =
                _db.FacturasComprasLin.Any(f => f.empresa == Empresa && f.fkalbaranes == result.Id);
            if (!result.IsFacturado)
            {
                var serieService = FService.Instance.GetService(typeof(SeriesModel), Context, _db);
                 
                var serieObj = serieService.get(SeriesService.GetSerieCodigo( TipoDocumento.AlbaranesCompras) + "-" + result.Fkseries) as SeriesModel;

                if (!string.IsNullOrEmpty(serieObj.Fkseriesasociada))
                {
                    var serieAsociadaObj = serieService.get(SeriesService.GetSerieCodigo(TipoDocumento.FacturasCompras) + "-" + serieObj.Fkseriesasociada) as SeriesModel;
                    var ejercicioActual = !string.IsNullOrEmpty(serieAsociadaObj.Fkejercicios)
                        ? serieAsociadaObj.Fkejercicios
                        : !string.IsNullOrEmpty(Context.Ejercicio) ? Context.Ejercicio : string.Empty;


                    if (!string.IsNullOrEmpty(ejercicioActual) && ejercicioActual == Context.Ejercicio)
                    {
                        result.Fkseriefactura = serieObj.Fkseriesasociada;
                        var intId = Funciones.Qint(ejercicioActual);
                        var ejercicioObj = _db.Ejercicios.Single(f => f.empresa == Empresa && f.id == intId);
                        if (DateTime.Now > ejercicioObj.hasta)
                        {
                            result.Fechafactura =
                                ejercicioObj.hasta.Value.ToShortDateString().ToString(CultureInfo.InvariantCulture);
                        }
                    }
                }


            }

            result.Tipoalbaran = obj.tipoalbaran;
            result.Tipodeportes = (Tipoportes?)obj.tipoportes;
            result.Tipodealmacenlote = (TipoAlmacenlote?)obj.tipoalmacenlote;
            result.Decimalesmonedas = monedasObj.decimales ?? 2;
            //Lineas
            result.Lineas = obj.AlbaranesComprasLin.ToList().Select(f => new AlbaranesComprasLinModel()
            {
                
                Id = f.id,
                Fkarticulos = f.fkarticulos,
                Descripcion = f.descripcion,
                Lote = f.lote,
                Tabla = f.tabla,
                Cantidad = f.cantidad,
                Cantidadpedida = f.cantidadpedida,
                Largo = f.largo,
                Ancho = f.ancho,
                Grueso = f.grueso,
                Fkunidades = f.fkunidades,
                Metros = f.metros,
                Precio = f.precio,
                Porcentajedescuento = f.porcentajedescuento,
                Importedescuento = f.importedescuento,
                Fkregimeniva = result.Fkregimeniva,
                Fktiposiva = f.fktiposiva,
                Porcentajeiva = f.porcentajeiva,
                Cuotaiva = f.cuotaiva,
                Porcentajerecargoequivalencia = f.porcentajerecargoequivalencia,
                Cuotarecargoequivalencia = f.cuotarecargoequivalencia,
                Importe = f.importe,
                Notas = f.notas,
                Canal = f.canal,
                Precioanterior = f.precioanterior,
                Revision = f.revision,
                Decimalesmonedas = f.decimalesmonedas,
                Decimalesmedidas = f.decimalesmedidas,
                Fkpedidos = f.fkpedidos,
                Fkpedidosid = f.fkpedidosid,
                Fkpedidosreferencia = f.fkpedidosreferencia,
                Bundle = f.bundle,
                Tblnum = f.tblnum,
                Contenedor = f.contenedor,
                Sello = f.sello,
                Caja = f.caja,
                Pesoneto = f.pesoneto,
                Pesobruto = f.pesobruto,
                Seccion = f.seccion,
                Costeadicionalvariable = f.costeacicionalvariable,
                Costeadicionalportes = f.costeadicionalportes,
                Costeadicionalmaterial = f.costeadicionalmaterial,
                Costeadicionalotro = f.costeadicionalotro,            
                Orden = f.orden ?? f.id,
                EnFactura = _db.FacturasComprasLin.Any(j => j.empresa == Empresa && j.fkalbaranes == result.Id),
                Fkfacturasreferencia = _db.FacturasCompras.Include("FacturasComprasLin").Where(j => j.empresa == Empresa && j.FacturasComprasLin.Any(h => h.empresa == Empresa && h.fkalbaranes == result.Id)).ToList().Select(h => new StDocumentoReferencia() { CampoId = string.Format("{0}", h.id), Referencia = h.referencia }).ToList(),
                Fkcontadoreslotes = f.fkcontadoreslotes,
                Flagidentifier = f.flagidentifier,
                Tipodealmacenlote = (TipoAlmacenlote?)f.tipoalmacenlote,
                Fkreclamado = f.fkreclamado,
                Fkreclamadoreferencia = f.fkreclamadoreferencia
            }).ToList();

            //Totales
            result.Totales = obj.AlbaranesComprasTotales.ToList().Select(f => new AlbaranesComprasTotalesModel()
            {
                Fktiposiva = f.fktiposiva,
                Porcentajeiva = f.porcentajeiva,
                Baseimponible = f.basetotal,
                Brutototal = f.brutototal,
                Cuotaiva = f.cuotaiva,
                Porcentajerecargoequivalencia = f.porcentajerecargoequivalencia,
                Importerecargoequivalencia = f.importerecargoequivalencia,
                Porcentajedescuentoprontopago = f.porcentajedescuentoprontopago,
                Importedescuentoprontopago = f.importedescuentoprontopago,
                Porcentajedescuentocomercial = f.porcentajedescuentocomercial,
                Importedescuentocomercial = f.importedescuentocomercial,
                Subtotal = f.subtotal,
                Decimalesmonedas = f.decimalesmonedas
            }).ToList();

            result.Costes =obj.AlbaranesComprasCostesadicionales.ToList().Select(f => new AlbaranesComprasCostesadicionalesModel()
                {
                    Id= f.id,
                    Tipodocumento = (TipoCosteAdicional)f.tipodocumento,
                    Referenciadocumento = f.referenciadocumento,
                    Importe=f.importe,
                    Porcentaje = f.porcentaje,
                    Total =f.total,
                    Tipocoste =(TipoCoste)f.tipocoste,
                    Tiporeparto = (TipoReparto)f.tiporeparto,
                    Notas = f.notas
            }).ToList();

           

            return result;
        }

        public override AlbaranesCompras CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as AlbaranesComprasModel;
            var result = _db.Set<AlbaranesCompras>().Create();

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
            result.tipoalbaran = viewmodel.Tipoalbaran;
            result.fechaalta = DateTime.Now;
            result.fechamodificacion = result.fechaalta;
            result.fkusuarioalta = Context.Id;
            result.fkusuariomodificacion = Context.Id;
            result.fkpuertosfkpaises = viewmodel.Fkpuertos.Fkpaises;
            result.fkpuertosid = viewmodel.Fkpuertos.Id;
            result.tipoportes = (int?)viewmodel.Tipodeportes;
            result.tipoalmacenlote = (int?)viewmodel.Tipodealmacenlote;

            result.empresa = Empresa;
            
            foreach (var item in viewmodel.Lineas)
            {
                var newItem = _db.Set<AlbaranesComprasLin>().Create();
                newItem.empresa = Empresa;
                newItem.fkalbaranes = result.id;
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
                newItem.fkcontadoreslotes = item.Fkcontadoreslotes;
                newItem.flagidentifier= Guid.NewGuid();
                newItem.tipoalmacenlote = result.tipoalmacenlote;
                newItem.fkreclamado = item.Fkreclamado;
                newItem.fkreclamadoreferencia = item.Fkreclamadoreferencia;
                result.AlbaranesComprasLin.Add(newItem);
            }


            foreach (var item in viewmodel.Totales)
            {
                var newItem = _db.Set<AlbaranesComprasTotales>().Create();
                newItem.empresa = Empresa;
                newItem.fkalbaranes = result.id;
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

                result.AlbaranesComprasTotales.Add(newItem);
            }

            foreach (var item in viewmodel.Costes)
            {
                var newItem = _db.Set<AlbaranesComprasCostesadicionales>().Create();
                newItem.empresa = Empresa;
                newItem.fkalbaranescompras = result.id;
                newItem.id = item.Id;
                newItem.tipodocumento = (int) item.Tipodocumento;
                newItem.referenciadocumento = item.Referenciadocumento;
                newItem.importe = item.Importe;
                newItem.porcentaje = item.Porcentaje;
                newItem.total = item.Total;
                newItem.tipocoste = (int)item.Tipocoste;
                newItem.tiporeparto = (int) item.Tiporeparto;
                newItem.notas = item.Notas;
                result.AlbaranesComprasCostesadicionales.Add(newItem);
            }

            return result;
        }

        public override AlbaranesCompras EditPersitance(IModelView obj)
        {
            var viewmodel = obj as AlbaranesComprasModel;
            var result = _db.AlbaranesCompras.Where(f => f.empresa == viewmodel.Empresa && f.id == viewmodel.Id).Include(b => b.AlbaranesComprasLin).Include(b => b.AlbaranesComprasTotales).ToList().Single();
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
            result.tipoalbaran = viewmodel.Tipoalbaran;
            result.fechamodificacion = DateTime.Now;
            result.fkusuariomodificacion = Context.Id;
            result.fkpuertosfkpaises = viewmodel.Fkpuertos.Fkpaises;
            result.fkpuertosid = viewmodel.Fkpuertos.Id;
            result.tipoportes = (int?)viewmodel.Tipodeportes;
            result.tipoalmacenlote = (int?)viewmodel.Tipodealmacenlote;

            result.AlbaranesComprasLin.Clear();

            foreach (var item in viewmodel.Lineas)
            {
                var newItem = _db.Set<AlbaranesComprasLin>().Create();
                newItem.empresa = result.empresa;
                newItem.fkalbaranes = result.id;
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
                newItem.fkcontadoreslotes = item.Fkcontadoreslotes;
                newItem.flagidentifier = item.Flagidentifier;
                newItem.tipoalmacenlote = (int?)item.Tipodealmacenlote;
                newItem.fkreclamado = item.Fkreclamado;
                newItem.fkreclamadoreferencia = item.Fkreclamadoreferencia;

                result.AlbaranesComprasLin.Add(newItem);
            }

            result.AlbaranesComprasTotales.Clear();
            foreach (var item in viewmodel.Totales)
            {
                var newItem = _db.Set<AlbaranesComprasTotales>().Create();
                newItem.empresa = result.empresa;
                newItem.fkalbaranes = result.id;
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
                result.AlbaranesComprasTotales.Add(newItem);
            }

            result.AlbaranesComprasCostesadicionales.Clear();
            foreach (var item in viewmodel.Costes)
            {
                var newItem = _db.Set<AlbaranesComprasCostesadicionales>().Create();
                newItem.empresa = Empresa;
                newItem.fkalbaranescompras = result.id;
                newItem.id = item.Id;
                newItem.tipodocumento = (int)item.Tipodocumento;
                newItem.referenciadocumento = item.Referenciadocumento;
                newItem.importe = item.Importe;
                newItem.porcentaje = item.Porcentaje;
                newItem.total = item.Total;
                newItem.tipocoste = (int)item.Tipocoste;
                newItem.tiporeparto = (int)item.Tiporeparto;
                newItem.notas = item.Notas;
                result.AlbaranesComprasCostesadicionales.Add(newItem);
            }
            return result;
        }

        public override IModelView GetModelView(AlbaranesCompras obj)
        {
            var result = base.GetModelView(obj) as AlbaranesComprasModel;

            result.Fkpuertos.Fkpaises = obj.fkpuertosfkpaises;
            result.Fkpuertos.Id = obj.fkpuertosid;
            result.Integridadreferencialflag = obj.integridadreferenciaflag;
            result.Modo = (ModoAlbaran)obj.modo;
            return result;
        }
    }
}
