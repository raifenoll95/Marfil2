using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Documentos.FacturasCompras;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RFacturasCompras = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.FacturasCompras;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{

    internal class FacturasComprasValidation : BaseValidation<FacturasCompras>
    {
        public string EjercicioId { get; set; }
        public bool CambiarEstado { get; set; }
        public double? Margenfactura { get; set; }
        public bool FlagActualizarCantidadesServidas { get; set; }


        public FacturasComprasValidation(IContextService context, MarfilEntities db) : base(context, db)
        {

        }

        #region Validar grabar

        public override bool ValidarGrabar(FacturasCompras model)
        {
            if (!CambiarEstado)
            {
                ValidarEstado(model);
                ValidarCabecera(model);
                ValidarLineas(model);
                CalcularTotales(model);
                CalcularTotalesCabecera(model);
                AplicarRedondeo(model);
                ValidarVencimientos(model);
            }
            else ValidarCambiarEstado(model);

            return base.ValidarGrabar(model);
        }

        #region Validar cambio de estado

        private void ValidarEstado(FacturasCompras model)
        {
            string message;
            
            if (!_appService.ValidarEstado(model.fkestados, _db, out message))
                    throw new ValidationException(message);

            var estadosService = new EstadosService(Context, _db);
            var configuracionService = new ConfiguracionService(Context, _db);
            var configuracionModel = configuracionService.GetModel();
            var estadoactualObj = estadosService.get(model.fkestados) as EstadosModel;
            if (!string.IsNullOrEmpty(configuracionModel.Estadototal) && estadoactualObj.Tipoestado <= TipoEstado.Curso &&
                _db.Movs.Where(m => m.traza == model.referencia).Count() > 0)
            {
                model.fkestados = configuracionModel.Estadofacturascomprastotal;
            }
        }

        private void ValidarCambiarEstado(FacturasCompras model)
        {
            if (_db.Estados.Any(f => f.documento + "-" + f.id == model.fkestados))
            {
                var estado = _db.Estados.Single(f => f.documento + "-" + f.id ==  model.fkestados);
                if (estado.tipoestado == (int) TipoEstado.Finalizado)
                {
                    ValidarCondicionesContabilizar(model);
                }
            }
            else
            {
                throw new Exception("El estado no existe");
            }

        }

        private void ValidarCondicionesContabilizar(FacturasCompras model)
        {
            if (!model.fechadocumentoproveedor.HasValue)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RFacturasCompras.Fechadocumentoproveedor));
            if (string.IsNullOrEmpty(model.numerodocumentoproveedor))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RFacturasCompras.Numerodocumentoproveedor));
            if (!model.importefacturaproveedor.HasValue)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RFacturasCompras.Importefacturaproveedor));
            var importefacturaactual = CalculoTotalFacturaAjusta(model);
            if (model.importefacturaproveedor != importefacturaactual)
            {
                var diferencia = model.importefacturaproveedor - importefacturaactual;
                if (Math.Abs((model.importefacturaproveedor??0) - (importefacturaactual??0)) > (Margenfactura ?? 0))
                {
                    throw new ValidationException(string.Format("El importe de la factura del proveedor, {0}, no coincide con el de la factura actual, {1}.", model.importefacturaproveedor, importefacturaactual));
                }
            }
                

        }

        private void AplicarRedondeo(FacturasCompras model)
        {
            var diferencia = (model.FacturasComprasVencimientos.Sum(f=>f.importevencimiento)??0) - model.importefacturaproveedor;
            model.valorredondeo = CalcularValorRedondeo(model);
            CalcularTotalesCabecera(model);
            if (model.importetotaldoc != model.importefacturaproveedor)
            {
                AjusteIvaParaRedondeo(model);
            }
            RecalcularVencimientos(model, diferencia);
        }

        private void AjusteIvaParaRedondeo(FacturasCompras model)
        {
            var total1 = model.FacturasComprasTotales.First();
            var diferencia = Math.Round((model.importetotaldoc??0) - (model.importefacturaproveedor??0), total1.decimalesmonedas.Value);
           
            total1.cuotaiva = Math.Round((double)((total1.basetotal) * (total1.porcentajeiva / 100.0)), total1.decimalesmonedas.Value);
            total1.cuotaiva -= diferencia;
            total1.subtotal = total1.basetotal + total1.cuotaiva + total1.importerecargoequivalencia - total1.importeretencion;
            CalcularTotalesCabecera(model);
        }

        private void RecalcularVencimientos(FacturasCompras model,double? diferencia)
        {
            if (model.FacturasComprasVencimientos.Any())
            {
                var item = model.FacturasComprasVencimientos.First();
                item.importevencimiento -= diferencia;
            }
            
        }
        private double? CalcularValorRedondeo(FacturasCompras model)
        {
            double? result;
            var vector = model.FacturasComprasLin.GroupBy(f => f.fktiposiva);
            var decimales = _db.Monedas.Single(f => f.id == model.fkmonedas).decimales;
            var i = 0;

            double? baseprimeraacum = 0;
            double? porcentajeiva = 0;
            double? porcentajere = 0;
            double? porcentajeretencion = 0;

            double? basetotalacum=0;
            double? cuotaivaacum = 0;
            double? importerecargoequivalenciaacum = 0;
            double? importeretencionacum = 0;
            double? subtotalacum = 0;
            
            foreach (var item in vector)
            {
                var newItem = _db.FacturasComprasTotales.Create();
                var objIva = _db.TiposIva.Single(f => f.empresa == model.empresa && f.id == item.Key);
                newItem.empresa = model.empresa;
                newItem.fkfacturascompras = model.id;
                newItem.fktiposiva = item.Key;
                newItem.porcentajeiva = objIva.porcentajeiva;
                var importe = Math.Round(item.Sum(f => f.importe) ?? 0.0, decimales ?? 2);
                var importedescuento = Math.Round(item.Sum(f => f.importedescuento) ?? 0.0, decimales ?? 2);
                newItem.brutototal = Math.Round(importe - importedescuento, decimales.Value);
                newItem.porcentajerecargoequivalencia = objIva.porcentajerecargoequivalente;
                newItem.porcentajedescuentoprontopago = model.porcentajedescuentoprontopago ?? 0;
                newItem.porcentajedescuentocomercial = model.porcentajedescuentocomercial ?? 0;
                newItem.importedescuentocomercial = Math.Round((double)(newItem.brutototal * (model.porcentajedescuentocomercial ?? 0) / 100.0), decimales.Value);
                var basepp = newItem.brutototal - newItem.importedescuentocomercial;
                newItem.importedescuentoprontopago = Math.Round((double)((double)(basepp) * ((model.porcentajedescuentoprontopago ?? 0) / 100.0)), decimales.Value);
                newItem.basetotal = newItem.brutototal - (newItem.importedescuentoprontopago + newItem.importedescuentocomercial);
                newItem.cuotaiva = Math.Round((double)((newItem.basetotal) * (objIva.porcentajeiva / 100.0)), decimales.Value);
                newItem.importerecargoequivalencia = Math.Round((double)((newItem.basetotal) * (objIva.porcentajerecargoequivalente / 100.0)), decimales.Value);
                newItem.baseretencion = newItem.basetotal;
                newItem.porcentajeretencion = model.porcentajeretencion ?? 0;
                newItem.importeretencion = Math.Round((double)((newItem.baseretencion) * (newItem.porcentajeretencion / 100.0)), decimales.Value);
                newItem.subtotal = newItem.basetotal + newItem.cuotaiva + newItem.importerecargoequivalencia - newItem.importeretencion;
                newItem.decimalesmonedas = decimales;
                if (i != 0)
                {
                    

                    basetotalacum += newItem.basetotal;
                    cuotaivaacum += newItem.cuotaiva;
                    importerecargoequivalenciaacum += newItem.importerecargoequivalencia ?? 0;
                    importeretencionacum = newItem.importeretencion ?? 0;
                    subtotalacum += newItem.subtotal;
                }
                else
                {
                    baseprimeraacum = newItem.basetotal;
                    porcentajeiva = objIva.porcentajeiva??0;
                    porcentajere = objIva.porcentajerecargoequivalente ??0;
                    porcentajeretencion = newItem.porcentajeretencion ?? 0;
                }
                i++;
            }

            //operear con primera linea
            
            result = (model.importefacturaproveedor - (subtotalacum + baseprimeraacum + ((baseprimeraacum) * (porcentajeiva / 100.0)) + ((baseprimeraacum) * (porcentajere / 100.0)) - ((baseprimeraacum) * (porcentajeretencion /100.0)) )) / (1 + (porcentajeiva / 100.0) + (porcentajere / 100.0) - (porcentajeretencion /100.0));                            
            result = Math.Round(result??0, decimales.Value);

            var totalItem = model.FacturasComprasTotales.First();
            var totalIva = _db.TiposIva.Single(f => f.empresa == model.empresa && f.id == totalItem.fktiposiva);
            totalItem.basetotal = baseprimeraacum + result;
            //totalItem.baseretencion = totalItem.basetotal;

            totalItem.cuotaiva = Math.Round((double)((totalItem.basetotal) * (totalItem.porcentajeiva / 100.0)), decimales.Value);
            totalItem.importerecargoequivalencia = Math.Round((double)((totalItem.basetotal) * (totalIva.porcentajerecargoequivalente / 100.0)), decimales.Value);

            totalItem.importeretencion = Math.Round((double)((totalItem.baseretencion) * (totalItem.porcentajeretencion / 100.0)), decimales.Value);

            totalItem.subtotal = totalItem.basetotal + totalItem.cuotaiva + totalItem.importerecargoequivalencia- totalItem.importeretencion;

            return result;
        }

        private double? CalculoTotalFacturaAjusta(FacturasCompras model)
        {
            double? result = 0;
            if (model.FacturasComprasTotales.Any())
            {
               
                CalcularTotales(model,model.valorredondeo??0);
                CalcularTotalesCabecera(model);
                result = model.importetotaldoc;

                CalcularTotales(model, -1*(model.valorredondeo ?? 0));
                CalcularTotalesCabecera(model);
            }
            
            return result;
        }

        #endregion

        private bool ValidaRangoEjercicio(FacturasCompras model)
        {
            var result = true;
            var ejercicio = model.fkejercicio;
            var ejercicioobj = _db.Ejercicios.Single(f => f.empresa == model.empresa && f.id == ejercicio);
            var fechadocumento = model.fechadocumento.Value;
            return fechadocumento >= ejercicioobj.desde.Value && fechadocumento <= ejercicioobj.hasta.Value;
        }

        private void ValidarCabecera(FacturasCompras model)
        {
            if (model.fechadocumento == null)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RFacturasCompras.Fechadocumento));
            if (model.fkproveedores == null)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RFacturasCompras.Fkproveedores));
            if (!model.fkmonedas.HasValue)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RFacturasCompras.Fkmonedas));
            if (!model.fkformaspago.HasValue)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RFacturasCompras.Fkformaspago));
            if (!model.fechadocumentoproveedor.HasValue)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RFacturasCompras.Fechadocumentoproveedor));
           // if (string.IsNullOrEmpty(model.numerodocumentoproveedor))
           //     throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RFacturasCompras.Numerodocumentoproveedor));
            if (!model.importefacturaproveedor.HasValue)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RFacturasCompras.Importefacturaproveedor));
            var serieObj =
                _db.Series.Single(f => f.empresa == model.empresa && f.tipodocumento == "FRC" && f.id == model.fkseries);

            var clienteObj = _db.Proveedores.SingleOrDefault(f => f.empresa == model.empresa && f.fkcuentas == model.fkproveedores);
            Acreedores prospectoObj = null;
            if (clienteObj == null)
                prospectoObj = _db.Acreedores.Single(f => f.empresa == model.empresa && f.fkcuentas == model.fkproveedores);
            var fkmonedacliente = clienteObj?.fkmonedas ?? Funciones.Qint(prospectoObj.fkmonedas);

            if (serieObj.fkmonedas.HasValue && serieObj.fkmonedas != fkmonedacliente)
                throw new ValidationException(RFacturasCompras.ErrorMonedaClienteSerie);

            if (!ValidaRangoEjercicio(model))
                throw new ValidationException(RFacturasCompras.ErrorFechaEjercicio);

            var formapagoobj = _db.FormasPago.Single(f => f.id == model.fkformaspago);
            if (formapagoobj.mandato.HasValue && formapagoobj.mandato.Value && string.IsNullOrEmpty(model.fkbancosmandatos))
            {
                var mandato = _db.BancosMandatos.SingleOrDefault(
                    f => f.fkcuentas == model.fkproveedores && f.defecto == true && !string.IsNullOrEmpty(f.idmandato));
                var vector = model.fkestados.Split('-');
                var tipoestado = Funciones.Qint(vector[0]);
                var idestado = vector[1];
                var estadoObj = _db.Estados.Single(f => f.documento == tipoestado && f.id == idestado);
                //if (mandato == null && estadoObj.tipoestado == (int)TipoEstado.Finalizado)
                //{
                //    throw new ValidationException(RFacturasCompras.ErrorFormaPagoMandatoRequerido);
                //}
                //else
                //{
                //    WarningList.Add(RFacturasCompras.WarningFormaPagoMandatoRequerido);
                //}
                if(mandato != null && estadoObj.tipoestado == (int)TipoEstado.Finalizado)
                    WarningList.Add(RFacturasCompras.WarningFormaPagoMandatoRequerido);

                model.fkbancosmandatos = mandato?.id;
            }


            CalcularComisiones(model);
        }

        private void CalcularComisiones(FacturasCompras model)
        {
            var comision = Calculobrutocomision(model);
            var cuotaDescuentoComercial = 0.0;
            var cuotaDescuentoProntoPago = 0.0;
            var cuotaDescuentoRecargoFinanciero = 0.0;
            var netocomision = 0.0;
            var brutocomisionoriginal = comision;
            var decimales = _db.Monedas.Single(f => f.id == model.fkmonedas).decimales ?? 2;

            if (model.comisiondescontardescuentocomercial.HasValue && model.comisiondescontardescuentocomercial.Value)
            {
                cuotaDescuentoComercial = comision*(model.porcentajedescuentocomercial ?? 0.0)/100.0;
                comision -= cuotaDescuentoComercial;
            }

            if (model.comsiondescontardescuentoprontopago.HasValue && model.comsiondescontardescuentoprontopago.Value)
            {
                cuotaDescuentoProntoPago = comision * (model.porcentajedescuentoprontopago ?? 0.0) / 100.0;
                comision -= cuotaDescuentoProntoPago;
            }


            if (model.comisiondescontarrecargofinancieroformapago.HasValue && model.comisiondescontarrecargofinancieroformapago.Value && model.fkformaspago.HasValue)
            {
                var formapagoObj = _db.FormasPago.SingleOrDefault(f => f.id == model.fkformaspago.Value);
                if (formapagoObj != null)
                {
                    cuotaDescuentoRecargoFinanciero = comision * (formapagoObj.recargofinanciero ?? 0.0) / 100.0;
                    comision -= cuotaDescuentoRecargoFinanciero;
                }
            }

            netocomision = comision;

            model.brutocomision = Math.Round(brutocomisionoriginal, decimales);
            model.cuotadescuentocomercialcomision = Math.Round(cuotaDescuentoComercial, decimales);
            model.cuotadescuentoprontopagocomision = Math.Round(cuotaDescuentoProntoPago, decimales);
            model.cuotadescuentorecargofinancieroformapagocomision = Math.Round(cuotaDescuentoRecargoFinanciero, decimales);
            model.netobasecomision = Math.Round(netocomision, decimales);

            model.importecomisionagente = Math.Round(netocomision * (model.comisionagente ?? 0.0)/100.0, decimales);
            model.importecomisioncomercial = Math.Round(netocomision * (model.comisioncomercial ?? 0.0) / 100.0, decimales);
        }

        private double Calculobrutocomision(FacturasCompras model)
        {
            var lineas = model.FacturasComprasLin.Join(_db.Articulos, f => f.fkarticulos, j => j.id, (factura, articulo) => new {factura, articulo}).Where(f=>(f.articulo.excluircomisiones??false)==false);

            return lineas.Sum(item => item.factura.importe ?? 0);
        }

        private void ValidarLineas(FacturasCompras model)
        {
            var familiasProductosService = FService.Instance.GetService(typeof(FamiliasproductosModel), Context, _db) as FamiliasproductosService;
            var decimales = _db.Monedas.Single(f => f.id == model.fkmonedas).decimales;
            foreach (var item in model.FacturasComprasLin)
            {
                if (string.IsNullOrEmpty(item.fkarticulos))
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RFacturasCompras.Fkarticulos));

                if (!item.cantidad.HasValue)
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RFacturasCompras.Cantidad));

                if (!item.metros.HasValue)
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RFacturasCompras.Metros));

                if (!item.precio.HasValue)
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RFacturasCompras.Precio));

                if (string.IsNullOrEmpty(item.fktiposiva))
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RFacturasCompras.Fktiposiva));

               

                if (item.porcentajedescuento.HasValue)
                    item.importedescuento = Math.Round((double)(item.precio * item.metros * item.porcentajedescuento) / 100.0, 2);

                

                var art = _db.Articulos.Single(f => f.empresa == model.empresa && f.id == item.fkarticulos);
                var codGrupo = art.fkgruposiva;
                
                if (!art.tipoivavariable)
                {
                    var tiposivaService = FService.Instance.GetService(typeof(TiposIvaModel), Context, _db) as TiposivaService;
                    item.fktiposiva = tiposivaService.GetTipoIva(codGrupo, model.fkregimeniva).Id;
                }

                double cantidad = item.metros ?? 0;
                double precio = item.precio ?? 0;
                double importedescuento = item.importedescuento ?? 0;

                var baseimponible = cantidad * precio - importedescuento;

                item.importe = baseimponible;
                item.decimalesmonedas = decimales;
                if (!item.importe.HasValue)
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RFacturasCompras.Importe));

                //ang calcular importe neto
                item.importenetolinea = Math.Round((baseimponible * (1 - ((model.porcentajedescuentocomercial ?? 0) / 100)) * (1 - ((model.porcentajedescuentoprontopago ?? 0) / 100))), (item.decimalesmonedas ?? 2));

                var familiacodigo = ArticulosService.GetCodigoFamilia(item.fkarticulos);
                familiasProductosService.ValidarDimensiones(familiacodigo, item.largo, item.ancho, item.grueso);
            }
            var vector = model.FacturasComprasLin.OrderBy(f => f.orden).ToList();
            for (var i = 0; i < vector.Count(); i++)
                vector[i].orden = (i + 1) * ApplicationHelper.EspacioOrdenLineas;
        }

        private void CalcularTotales(FacturasCompras model,double redondeo=0)
        {
            model.FacturasComprasTotales.Clear();
            var vector = model.FacturasComprasLin.GroupBy(f => f.fktiposiva);
            var decimales = _db.Monedas.Single(f => f.id == model.fkmonedas).decimales;
            
            foreach (var item in vector)
            {
                var newItem = _db.FacturasComprasTotales.Create();
                var objIva = _db.TiposIva.Single(f => f.empresa == model.empresa && f.id == item.Key);
                newItem.empresa = model.empresa;
                newItem.fkfacturascompras = model.id;
                newItem.fktiposiva = item.Key;
                newItem.porcentajeiva = objIva.porcentajeiva;
                var importe = Math.Round(item.Sum(f => f.importe)??0.0, decimales??2);
                var importedescuento = Math.Round(item.Sum(f => f.importedescuento) ?? 0.0, decimales ?? 2);
                //newItem.brutototal = Math.Round(importe - importedescuento, decimales.Value);
                newItem.brutototal = Math.Round(importe, decimales.Value); //Cambio 26/03/2021
                newItem.porcentajerecargoequivalencia = objIva.porcentajerecargoequivalente;
                newItem.porcentajedescuentoprontopago = model.porcentajedescuentoprontopago ?? 0;
                newItem.porcentajedescuentocomercial = model.porcentajedescuentocomercial ?? 0;
                newItem.importedescuentocomercial = Math.Round((double)(newItem.brutototal * (model.porcentajedescuentocomercial ?? 0) / 100.0), decimales.Value);
                var basepp = newItem.brutototal - newItem.importedescuentocomercial;
                newItem.importedescuentoprontopago = Math.Round((double)((double)(basepp) * ((model.porcentajedescuentoprontopago ?? 0) / 100.0)), decimales.Value);
                newItem.basetotal = newItem.brutototal - (newItem.importedescuentoprontopago + newItem.importedescuentocomercial);
                newItem.basetotal += redondeo;
                newItem.cuotaiva = Math.Round((double)((newItem.basetotal) * (objIva.porcentajeiva / 100.0)), decimales.Value);
                newItem.importerecargoequivalencia = Math.Round((double)((newItem.basetotal) * (objIva.porcentajerecargoequivalente / 100.0)), decimales.Value);
                newItem.baseretencion = newItem.basetotal;
                newItem.porcentajeretencion = model.porcentajeretencion ?? 0;
                newItem.importeretencion = Math.Round((double)((newItem.basetotal) * (newItem.porcentajeretencion / 100.0)), decimales.Value);

                newItem.subtotal = newItem.basetotal + newItem.cuotaiva + newItem.importerecargoequivalencia - newItem.importeretencion;
                newItem.decimalesmonedas = decimales;

                redondeo = 0;
                model.FacturasComprasTotales.Add(newItem);
            }
        }

        private void CalcularTotalesCabecera(FacturasCompras model)
        {
            var decimales = _db.Monedas.Single(f => f.id == model.fkmonedas).decimales ?? 0;

            model.importebruto = Math.Round((double)model.FacturasComprasTotales.Sum(f => f.brutototal), decimales);
            model.importedescuentoprontopago = Math.Round((double)model.FacturasComprasTotales.Sum(f => f.importedescuentoprontopago), decimales);
            model.importedescuentocomercial = Math.Round((double)model.FacturasComprasTotales.Sum(f => f.importedescuentocomercial), decimales);
            model.importebaseimponible = Math.Round((double)model.FacturasComprasTotales.Sum(f => f.basetotal), decimales);
            model.importetotaldoc = Math.Round((double)model.FacturasComprasTotales.Sum(f => f.subtotal), decimales);

            //todo revisar esto y recalcular el importe total
            model.importetotalmonedabase = model.FacturasComprasTotales.Sum(f => f.subtotal * (model.cambioadicional ?? 1.0));

            //ang - comprobar total neto lineas
            if (model.importebaseimponible != model.FacturasComprasLin.Sum(l => l.importenetolinea))
            {
                model.FacturasComprasLin.Last().importenetolinea += model.importebaseimponible - model.FacturasComprasLin.Sum(l => l.importenetolinea);
            }
        }

        private void ValidarVencimientos(FacturasCompras model)
        {
            var decimales = _db.Monedas.Single(f => f.id == model.fkmonedas).decimales ?? 0;
            if (model.FacturasComprasVencimientos.Any())
            {
                var total = Math.Round(model.FacturasComprasVencimientos.Sum(f => f.importevencimiento) ??0.0, decimales);
                if(total!=model.importetotaldoc)
                    throw new ValidationException(RFacturasCompras.ErrorImporteVencimientosTotal);
            }
        }

        #endregion

        #region Eliminar

        public override bool ValidarBorrar(FacturasCompras model)
        {
            if (_db.Movs.Any(f => f.traza == model.referencia && f.empresa == model.empresa))
                throw new ValidationException("No se puede borrar una factura contabilizada");

            return base.ValidarBorrar(model);
        }

        #endregion


    }
}
