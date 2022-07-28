using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RPedidosCompras = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.PedidosCompras;
using RProspectos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Prospectos;
using RAlbaranes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Albaranes;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
   
    internal class PedidosComprasValidation : BaseValidation<PedidosCompras>
    {
        public string EjercicioId { get; set; }
        public bool CambiarEstado { get; set; }
        public bool FlagActualizarCantidadesPedidas { get; set; }
        public PedidosComprasValidation(IContextService context, MarfilEntities db) : base(context,db)
        {

        }

        #region Validar grabar

        public override bool ValidarGrabar(PedidosCompras model)
        {
            if (!CambiarEstado)
            {
                ValidarEstado(model);
                ValidarCabecera(model);
                ValidarLineas(model);
                CalcularTotales(model);
                CalcularTotalesCabecera(model);
            }
            
            return base.ValidarGrabar(model);
        }

        private void ValidarEstado(PedidosCompras model)
        {
            string message;
            if (!FlagActualizarCantidadesPedidas)
                if (!_appService.ValidarEstado(model.fkestados, _db, out message))
                throw new ValidationException(message);

            var estadosService = new EstadosService(Context, _db);
            var configuracionService = new ConfiguracionService(Context, _db);
            var configuracionModel = configuracionService.GetModel();
            var estadoactualObj = estadosService.get(model.fkestados) as EstadosModel;
            if (!(estadoactualObj.Tipomovimiento == Model.Configuracion.TipoMovimiento.Manual))
            {
                if (!string.IsNullOrEmpty(configuracionModel.Estadototal) && estadoactualObj.Tipoestado <= TipoEstado.Curso && model.PedidosComprasLin.Any() && model.PedidosComprasLin.All(f => (f.cantidad ?? 0) != 0 && (f.cantidad ?? 0) - (f.cantidadpedida ?? 0) <= 0))
                {

                    model.fkestados = configuracionModel.Estadopedidosventastotal;
                }
                else if (!string.IsNullOrEmpty(configuracionModel.Estadoparcial) && estadoactualObj.Tipoestado <= TipoEstado.Curso &&
                         model.PedidosComprasLin.Any(f => (f.cantidadpedida ?? 0) > 0))
                {
                    model.fkestados = configuracionModel.Estadopedidosventasparcial;
                }
                else if (!string.IsNullOrEmpty(configuracionModel.Estadoinicial) && estadoactualObj.Tipoestado <= TipoEstado.Curso &&
                         model.PedidosComprasLin.Any(f => (f.cantidadpedida ?? 0) == 0))
                {
                    model.fkestados = configuracionModel.Estadopedidosventasinicial;
                }
            }
        }

        private bool ValidaRangoEjercicio(PedidosCompras model)
        {
            var result = true;
            var ejercicio = model.fkejercicio;
            var ejercicioobj = _db.Ejercicios.Single(f => f.empresa == model.empresa && f.id == ejercicio);
            var fechadocumento = model.fechadocumento.Value;
            return fechadocumento >= ejercicioobj.desde.Value && fechadocumento <= ejercicioobj.hasta.Value;
        }

        private void ValidarCabecera(PedidosCompras model)
        {
            if (model.fechadocumento == null)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RPedidosCompras.Fechadocumento));
            if (model.fkproveedores == null)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RPedidosCompras.Fkclientes));
            if (!model.fkmonedas.HasValue)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RPedidosCompras.Fkmonedas));
            if (!model.fkformaspago.HasValue)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RPedidosCompras.Fkformaspago));
            
            
            var serieObj =
                _db.Series.Single(f => f.empresa == model.empresa && f.tipodocumento == "PEC" && f.id == model.fkseries);

            var clienteObj = _db.Proveedores.SingleOrDefault(f => f.empresa == model.empresa && f.fkcuentas == model.fkproveedores);
            Acreedores prospectoObj = null;
            if (clienteObj == null)
                prospectoObj = _db.Acreedores.Single(f => f.empresa == model.empresa && f.fkcuentas == model.fkproveedores);
            var fkmonedacliente = clienteObj?.fkmonedas ?? Funciones.Qint(prospectoObj.fkmonedas);
            if (serieObj.fkmonedas.HasValue && serieObj.fkmonedas!= fkmonedacliente)
                throw new ValidationException(RPedidosCompras.ErrorMonedaClienteSerie);

            if (!FlagActualizarCantidadesPedidas && !ValidaRangoEjercicio(model))
                throw new ValidationException(RPedidosCompras.ErrorFechaEjercicio);
        }

        private void ValidarLineas(PedidosCompras model)
        {
            var decimales = _db.Monedas.Single(f => f.id == model.fkmonedas).decimales;
            var familiasProductosService = FService.Instance.GetService(typeof(FamiliasproductosModel), Context, _db) as FamiliasproductosService;
            foreach (var item in model.PedidosComprasLin)
            {
                if (string.IsNullOrEmpty(item.fkarticulos))
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RPedidosCompras.Fkarticulos));

                if (!item.cantidad.HasValue)
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RPedidosCompras.Cantidad));

                if (!item.metros.HasValue)
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RPedidosCompras.Metros));

                if (!item.precio.HasValue)
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RPedidosCompras.Precio));

                if (string.IsNullOrEmpty(item.fktiposiva))
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RPedidosCompras.Fktiposiva));

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
                item.decimalesmonedas = decimales;
                item.importe = baseimponible;

                if (!item.importe.HasValue)
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RPedidosCompras.Importe));

                familiasProductosService.ValidarDimensiones(familiacodigo, item.largo, item.ancho, item.grueso);
            }

            var vector = model.PedidosComprasLin.OrderBy(f => f.orden).ToList();
            for (var i = 0; i < vector.Count(); i++)
                vector[i].orden = (i + 1) * ApplicationHelper.EspacioOrdenLineas;
        }

        private void CalcularTotales(PedidosCompras model)
        {
            model.PedidosComprasTotales.Clear();
            var vector = model.PedidosComprasLin.GroupBy(f => f.fktiposiva);
            var decimales = _db.Monedas.Single(f => f.id == model.fkmonedas).decimales;
            foreach (var item in vector)
            {
                var newItem = _db.PedidosComprasTotales.Create();
                var objIva = _db.TiposIva.Single(f => f.empresa == model.empresa && f.id == item.Key);
                newItem.empresa = model.empresa;
                newItem.fkpedidoscompras = model.id;
                newItem.fktiposiva = item.Key;
                newItem.porcentajeiva = objIva.porcentajeiva;
                
                newItem.brutototal = Math.Round((double)(item.Sum(f => f.importe) - item.Sum(f => f.importedescuento)), decimales.Value);
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
                newItem.decimalesmonedas = decimales;
                model.PedidosComprasTotales.Add(newItem);
            }
        }

        private void CalcularTotalesCabecera(PedidosCompras model)
        {
            var decimales = _db.Monedas.Single(f => f.id == model.fkmonedas).decimales ?? 0;

            model.importebruto =Math.Round((double) model.PedidosComprasTotales.Sum(f => f.brutototal),decimales);
            model.importedescuentoprontopago = Math.Round((double)model.PedidosComprasTotales.Sum(f => f.importedescuentoprontopago),decimales);
            model.importedescuentocomercial = Math.Round((double)model.PedidosComprasTotales.Sum(f => f.importedescuentocomercial), decimales);
            model.importebaseimponible = Math.Round((double)model.PedidosComprasTotales.Sum(f => f.basetotal), decimales);
            model.importetotaldoc = Math.Round((double)model.PedidosComprasTotales.Sum(f => f.subtotal), decimales);
            
            //todo revisar esto y recalcular el importe total
            model.importetotalmonedabase = model.PedidosComprasTotales.Sum(f => f.subtotal*(model.cambioadicional??1.0));
        }

        #endregion

        #region Eliminar

        public override bool ValidarBorrar(PedidosCompras model)
        {
            if (_db.AlbaranesComprasLin.Any(f => f.empresa == model.empresa && f.fkpedidos == model.id))
                throw new ValidationException(string.Format(General.ErrorIntegridadReferencial, RPedidosCompras.TituloEntidadSingular, RAlbaranes.TituloEntidadSingular));

            return base.ValidarBorrar(model);
        }

        #endregion

       
    }
}
