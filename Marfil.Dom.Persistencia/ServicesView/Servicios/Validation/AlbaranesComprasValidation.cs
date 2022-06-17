using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Documentos.Albaranes;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RAlbaranes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Albaranes;
using RAlbaranesCompras = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.AlbaranesCompras;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
   
    internal class AlbaranesComprasValidation : BaseValidation<AlbaranesCompras>
    {
        public string EjercicioId { get; set; }
        public bool ModificarCostes { get; set; }
        public bool CambiarEstado { get; set; }
        public bool FlagActualizarCantidadesServidas { get; set; }
        public bool FlagActualizarCantidadesFacturadas { get; set; }

        public AlbaranesComprasValidation(IContextService context, MarfilEntities db) : base(context, db)
        {

        }

        #region Validar grabar

        public override bool ValidarGrabar(AlbaranesCompras model)
        {
            //Todo EL: verificar los albaranes de compra si se pueden modificar si está en alguna factura de compra
            //if (CambiarEstado)
            //    if (_db.FacturasLin.Any(f => f.fkalbaranesreferencia == model.referencia && f.empresa == model.empresa))
            //    throw new ValidationException(RAlbaranesCompras.ErrorAlbaranFacturado);

            if (!CambiarEstado)
            {
                if (!ModificarCostes)
                    ValidarEstado(model);
                ValidarCabecera(model);
                ValidarLineas(model);
                CalcularTotales(model);
                CalcularTotalesCabecera(model);
                CalcularCostesadicionales(model);
                ActualizarPrecios(model);
            }
            
            return base.ValidarGrabar(model);
        }

        

        private void ValidarEstado(AlbaranesCompras model)
        {
            string message;
            if (!FlagActualizarCantidadesFacturadas)
            {
                if (!_appService.ValidarEstado(model.fkestados, _db, out message))
                    throw new ValidationException(message);
            }

            var estadosService = new EstadosService(Context, _db);
            var configuracionService = new ConfiguracionService(Context, _db);
            var configuracionModel = configuracionService.GetModel();
            var estadoactualObj = estadosService.get(model.fkestados) as EstadosModel;
            if (model.tipoalbaran != (int)TipoAlbaran.Devolucion && !string.IsNullOrEmpty(configuracionModel.Estadoalbaranescomprastotal) && estadoactualObj.Tipoestado <= TipoEstado.Curso && model.AlbaranesComprasLin.Any() && model.AlbaranesComprasLin.All(f => (f.cantidad?? 0) != 0 && (f.cantidad ?? 0) - (f.cantidadpedida ?? 0) <= 0))
            {
                model.fkestados = configuracionModel.Estadoalbaranescomprastotal;
            }
            else if (model.tipoalbaran == (int)TipoAlbaran.Devolucion && !string.IsNullOrEmpty(configuracionModel.Estadoalbaranescomprastotal) && estadoactualObj.Tipoestado <= TipoEstado.Curso && _db.FacturasComprasLin.Any(f => f.empresa == Context.Empresa && f.fkalbaranes == model.id))
            {
                model.fkestados = configuracionModel.Estadoalbaranescomprastotal;
            }
            else if (!string.IsNullOrEmpty(configuracionModel.Estadoparcial) && estadoactualObj.Tipoestado <= TipoEstado.Curso &&
                     model.AlbaranesComprasLin.Any(f => (f.cantidadpedida ?? 0) > 0))
            {
                model.fkestados = configuracionModel.Estadoparcial;
            }
            /*
            else if (!string.IsNullOrEmpty(configuracionModel.Estadoinicial) && estadoactualObj.Tipoestado <= TipoEstado.Curso &&
                     model.AlbaranesComprasLin.Any(f => (f.cantidadpedida ?? 0) == 0))
            {
                //model.fkestados = configuracionModel.Estadoalbaranesventasinicial;
                model.fkestados = model.fkestados;
            }
            */
        }

        private bool ValidaRangoEjercicio(AlbaranesCompras model)
        {
            var result = true;
            var ejercicio = model.fkejercicio;
            var ejercicioobj = _db.Ejercicios.Single(f => f.empresa == model.empresa && f.id == ejercicio);
            var fechadocumento = model.fechadocumento.Value;
            return fechadocumento >= ejercicioobj.desde.Value && fechadocumento <= ejercicioobj.hasta.Value;
        }

        private void ValidarCabecera(AlbaranesCompras model)
        {
            if (model.fechadocumento == null)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Fechadocumento));
            if (model.fkproveedores == null)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Fkproveedores));
            if (!model.fkmonedas.HasValue)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Fkmonedas));
            if (!model.fkformaspago.HasValue)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Fkformaspago));
            if (string.IsNullOrEmpty(model.fkalmacen))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Fkalmacen));

            if (!string.IsNullOrEmpty(model.fkzonas))
            {
                var idInt = Funciones.Qint(model.fkzonas) ?? 0;
                if (!_db.AlmacenesZona.Any(
                    f => f.empresa == model.empresa && f.fkalmacenes == model.fkalmacen && f.id == idInt))
                    throw new ValidationException(string.Format(RAlbaranes.ErrorZonaAlmacen,model.fkzonas,model.fkalmacen));
                
            }
            

            var monedas = _db.Proveedores.Any(f => f.empresa == model.empresa && f.fkcuentas == model.fkproveedores) ?_db.Proveedores.SingleOrDefault(f => f.empresa == model.empresa && f.fkcuentas == model.fkproveedores).fkmonedas: _db.Acreedores.Single(f => f.empresa == model.empresa && f.fkcuentas == model.fkproveedores).fkmonedas;

            string tipodocumento;
            tipodocumento = (model.tipoalbaran == (int)TipoAlbaran.VariosAlmacen) ? "ENV" : "ALC";
            var serieObj =
                    _db.Series.Single(f => f.empresa == model.empresa && f.tipodocumento == tipodocumento && f.id == model.fkseries);

            if(serieObj.fkmonedas.HasValue && serieObj.fkmonedas!= monedas)
                throw new ValidationException(RAlbaranesCompras.ErrorMonedaClienteSerie);

            if (!FlagActualizarCantidadesFacturadas &&!ModificarCostes && !ValidaRangoEjercicio(model))
                throw new ValidationException(RAlbaranesCompras.ErrorFechaEjercicio);

            if((model.tipoalbaran==(int)TipoAlbaran.Devolucion && string.IsNullOrEmpty(model.fkmotivosdevolucion)))
            {
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Fkmotivosdevolucion));
            }

            model.integridadreferenciaflag = Guid.NewGuid();

        }

        private void ValidarLineas(AlbaranesCompras model)
        {

            if (model.AlbaranesComprasCostesadicionales.Any() && !model.AlbaranesComprasLin.Any())
                throw new ValidationException(General.ErrorLineasObligatoriasCostes);
            var familiasProductosService = FService.Instance.GetService(typeof(FamiliasproductosModel), Context, _db) as FamiliasproductosService;
            foreach (var item in model.AlbaranesComprasLin)
            {
                if (string.IsNullOrEmpty(item.fkarticulos))
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Fkarticulos));

                if (!item.cantidad.HasValue)
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Cantidad));

                if (!item.metros.HasValue)
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Metros));

                if (!item.precio.HasValue)
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Precio));

                if (string.IsNullOrEmpty(item.fktiposiva))
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Fktiposiva));

                if (item.porcentajedescuento.HasValue)
                    item.importedescuento = Math.Round((double)(item.precio*item.metros*item.porcentajedescuento)/100.0,2);

                var familiacodigo = ArticulosService.GetCodigoFamilia(item.fkarticulos);
                var familiaModel = familiasProductosService.get(familiacodigo) as FamiliasproductosModel;
                item.fkunidades = _db.Unidades.Single(f => f.id == familiaModel.Fkunidadesmedida).id;

                var art = _db.Articulos.Single(f => f.empresa == model.empresa && f.id == item.fkarticulos);
                var codGrupo = art.fkgruposiva;
                if (!art.tipoivavariable)
                {
                    var tiposivaService = FService.Instance.GetService(typeof(TiposIvaModel), Context, _db) as TiposivaService;
                    item.fktiposiva = tiposivaService.GetTipoIva(codGrupo, model.fkregimeniva).Id;
                }
                

                double cantidad = item.metros??0;
                double precio = item.precio ?? 0;
                double importedescuento = item.importedescuento ?? 0;

                var baseimponible = cantidad * precio - importedescuento;

                item.importe = baseimponible;

                if (!item.importe.HasValue)
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Importe));

                familiasProductosService.ValidarDimensiones(familiacodigo, item.largo, item.ancho, item.grueso);

                //ang calcular importe neto
                item.importenetolinea = Math.Round((baseimponible * (1 - ((model.porcentajedescuentocomercial ?? 0) / 100)) * (1 - ((model.porcentajedescuentoprontopago ?? 0) / 100))), (item.decimalesmonedas ?? 2));
            }

            var vector = model.AlbaranesComprasLin.OrderBy(f => f.orden).ToList();
            for (var i = 0; i < vector.Count(); i++)
                vector[i].orden = (i + 1) * ApplicationHelper.EspacioOrdenLineas;

        }

        private void CalcularTotales(AlbaranesCompras model)
        {
            model.AlbaranesComprasTotales.Clear();
            var vector = model.AlbaranesComprasLin.GroupBy(f => f.fktiposiva);
            var decimales = _db.Monedas.Single(f => f.id == model.fkmonedas).decimales;
            foreach (var item in vector)
            {
                var newItem = _db.AlbaranesComprasTotales.Create();
                var objIva = _db.TiposIva.Single(f => f.empresa == model.empresa && f.id == item.Key);
                newItem.empresa = model.empresa;
                newItem.fkalbaranes = model.id;
                newItem.fktiposiva = item.Key;
                newItem.porcentajeiva = objIva.porcentajeiva;
                
                newItem.brutototal = Math.Round((double)(item.Sum(f => (f.metros) * (f.precio)) - item.Sum(f => f.importedescuento)), decimales.Value);
                newItem.porcentajerecargoequivalencia = objIva.porcentajerecargoequivalente;
                newItem.porcentajedescuentoprontopago = model.porcentajedescuentoprontopago??0;
                newItem.porcentajedescuentocomercial = model.porcentajedescuentocomercial ?? 0;
                newItem.importedescuentocomercial = Math.Round((double) (newItem.brutototal * (model.porcentajedescuentocomercial ?? 0) / 100.0), decimales.Value);
                var basepp = newItem.brutototal - newItem.importedescuentocomercial;
                newItem.importedescuentoprontopago = Math.Round((double) ((double)(basepp) *((model.porcentajedescuentoprontopago ?? 0 )/ 100.0)), decimales.Value);
                newItem.basetotal = newItem.brutototal - (newItem.importedescuentoprontopago+ newItem.importedescuentocomercial);
                newItem.cuotaiva =  Math.Round((double)((newItem.basetotal) *(objIva.porcentajeiva/100.0)), decimales.Value);
                newItem.importerecargoequivalencia =  Math.Round((double)((newItem.basetotal) * (objIva.porcentajerecargoequivalente / 100.0)), decimales.Value);
                newItem.subtotal = newItem.basetotal + newItem.cuotaiva + newItem.importerecargoequivalencia;

                model.AlbaranesComprasTotales.Add(newItem);
            }
        }

        private void CalcularTotalesCabecera(AlbaranesCompras model)
        {
            var decimales = _db.Monedas.Single(f => f.id == model.fkmonedas).decimales ?? 0;

            model.importebruto =Math.Round((double) model.AlbaranesComprasTotales.Sum(f => f.brutototal),decimales);
            model.importedescuentoprontopago = Math.Round((double)model.AlbaranesComprasTotales.Sum(f => f.importedescuentoprontopago),decimales);
            model.importedescuentocomercial = Math.Round((double)model.AlbaranesComprasTotales.Sum(f => f.importedescuentocomercial), decimales);
            model.importebaseimponible = Math.Round((double)model.AlbaranesComprasTotales.Sum(f => f.basetotal), decimales);
            model.importetotaldoc = Math.Round((double)model.AlbaranesComprasTotales.Sum(f => f.subtotal), decimales);
            
            //todo revisar esto y recalcular el importe total
            model.importetotalmonedabase = model.AlbaranesComprasTotales.Sum(f => f.subtotal*(model.cambioadicional??1.0));
        }

        private void CalcularCostesadicionales(AlbaranesCompras model)
        {
            var decimales = _db.Monedas.Single(f => f.id == model.fkmonedas).decimales ?? 0;

            foreach (var item in model.AlbaranesComprasCostesadicionales)
            {
                if (!item.importe.HasValue)
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Importe));

                if (!item.porcentaje.HasValue)
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Porcentaje));

                if (item.tipodocumento == (int)TipoCosteAdicional.Importefijo)
                { 
                    item.referenciadocumento = string.Empty;
                    item.total = Math.Round((double)(item.importe * (item.porcentaje / 100.0)), decimales);
                }
            }
        }

        public void ActualizarPrecios(AlbaranesCompras model)
        {

            if (!String.IsNullOrWhiteSpace(model.fkpedidoscompras))
            {
                foreach(var lineaAlbaranCompra in model.AlbaranesComprasLin)
                {
                    if(lineaAlbaranCompra.precio == 0) {

                        var idPedidoCompra = _db.PedidosCompras.Where(f => f.empresa == model.empresa && f.referencia == model.fkpedidoscompras).Select(f => f.id).SingleOrDefault();
                        var lineasPedidosCompra = _db.PedidosComprasLin.Where(f => f.empresa == model.empresa && f.fkpedidoscompras == idPedidoCompra);

                        foreach (var lineaPedido in lineasPedidosCompra) {

                            if (lineaAlbaranCompra.fkarticulos == lineaPedido.fkarticulos) {

                                lineaAlbaranCompra.precio = lineaPedido.precio;
                                lineaAlbaranCompra.importe = Math.Round((double)((lineaAlbaranCompra.metros * lineaAlbaranCompra.precio) - (lineaAlbaranCompra.metros * lineaAlbaranCompra.precio
                                    * (lineaAlbaranCompra.porcentajedescuento / 100))), 2);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Eliminar

        public override bool ValidarBorrar(AlbaranesCompras model)
        {
            if (_db.FacturasLin.Any(f => f.fkalbaranesreferencia == model.referencia && f.empresa == model.empresa))
                throw new ValidationException(RAlbaranesCompras.ErrorAlbaranFacturado);

            return base.ValidarBorrar(model);
        }

        #endregion

       
    }
}
