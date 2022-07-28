using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Iva;
using Resources;
using RPedidos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Pedidos;
using RProspectos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Prospectos;
using RAlbaranes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Albaranes;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
   
    internal class PedidosValidation : BaseValidation<Pedidos>
    {
        public string EjercicioId { get; set; }
        public bool CambiarEstado { get; set; }
        public bool ModificarCostes { get; set; }
        public bool FlagActualizarCantidadesPedidas { get; set; }
        public PedidosValidation(IContextService context, MarfilEntities db) : base(context,db)
        {

        }

        #region Validar grabar

        public override bool ValidarGrabar(Pedidos model)
        {
            if (!CambiarEstado)
            {
                if (!ModificarCostes)
                    ValidarEstado(model);                
                ValidarCabecera(model);
                ValidarLineas(model);
                CalcularTotales(model);
                CalcularTotalesCabecera(model);
            }
            
            return base.ValidarGrabar(model);
        }

        private void ValidarEstado(Pedidos model)
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
                if (!string.IsNullOrEmpty(configuracionModel.Estadototal) && estadoactualObj.Tipoestado <= TipoEstado.Curso && model.PedidosLin.Any() && model.PedidosLin.All(f => (f.cantidad ?? 0) != 0 && (f.cantidad ?? 0) - (f.cantidadpedida ?? 0) <= 0))
                {

                    model.fkestados = configuracionModel.Estadopedidosventastotal;
                }
                else if (!string.IsNullOrEmpty(configuracionModel.Estadoparcial) && estadoactualObj.Tipoestado <= TipoEstado.Curso &&
                         model.PedidosLin.Any(f => (f.cantidadpedida ?? 0) > 0))
                {
                    model.fkestados = configuracionModel.Estadopedidosventasparcial;
                }
                else if (!string.IsNullOrEmpty(configuracionModel.Estadoinicial) && estadoactualObj.Tipoestado <= TipoEstado.Curso &&
                         model.PedidosLin.Any(f => (f.cantidadpedida ?? 0) == 0))
                {
                    model.fkestados = configuracionModel.Estadopedidosventasinicial;
                }
            }
            
        }

        private bool ValidaRangoEjercicio(Pedidos model)
        {
            var result = true;
            var ejercicio = model.fkejercicio;
            var ejercicioobj = _db.Ejercicios.Single(f => f.empresa == model.empresa && f.id == ejercicio);
            var fechadocumento = model.fechadocumento.Value;
            return fechadocumento >= ejercicioobj.desde.Value && fechadocumento <= ejercicioobj.hasta.Value;
        }

        private void ValidarCabecera(Pedidos model)
        {
            if (model.fechadocumento == null)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RPedidos.Fechadocumento));
            if (model.fkclientes == null)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RPedidos.Fkclientes));
            if (!model.fkmonedas.HasValue)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RPedidos.Fkmonedas));
            if (!model.fkformaspago.HasValue)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RPedidos.Fkformaspago));

            if(_db.Prospectos.Any(f=>f.empresa==model.empresa && f.fkcuentas==model.fkclientes))
                throw new ValidationException(RProspectos.ErrorCrearPedidoProspecto);

            var clienteObj = _db.Clientes.Single(f => f.empresa == model.empresa && f.fkcuentas == model.fkclientes);
            var serieObj =
                _db.Series.Single(f => f.empresa == model.empresa && f.tipodocumento == "PED" && f.id == model.fkseries);

            if(serieObj.fkmonedas.HasValue && serieObj.fkmonedas!=clienteObj.fkmonedas)
                throw new ValidationException(RPedidos.ErrorMonedaClienteSerie);

            if (!FlagActualizarCantidadesPedidas && !ValidaRangoEjercicio(model))
                throw new ValidationException(RPedidos.ErrorFechaEjercicio);
        }

        private void ValidarLineas(Pedidos model)
        {
            var familiasProductosService = FService.Instance.GetService(typeof(FamiliasproductosModel), Context, _db) as FamiliasproductosService;
            foreach (var item in model.PedidosLin)
            {
                if (string.IsNullOrEmpty(item.fkarticulos))
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RPedidos.Fkarticulos));

                if (!item.cantidad.HasValue)
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RPedidos.Cantidad));

                if (!item.metros.HasValue)
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RPedidos.Metros));

                if (!item.precio.HasValue)
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RPedidos.Precio));

                if (string.IsNullOrEmpty(item.fktiposiva))
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RPedidos.Fktiposiva));

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
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RPedidos.Importe));
                              
                familiasProductosService.ValidarDimensiones(familiacodigo, item.largo, item.ancho, item.grueso, art.id);                                    
            }

            var vector = model.PedidosLin.OrderBy(f => f.orden).ToList();
            for (var i = 0; i < vector.Count(); i++)
                vector[i].orden = (i + 1) * ApplicationHelper.EspacioOrdenLineas;
        }

        private void CalcularTotales(Pedidos model)
        {
            model.PedidosTotales.Clear();
            var vector = model.PedidosLin.GroupBy(f => f.fktiposiva);
            var decimales = _db.Monedas.Single(f => f.id == model.fkmonedas).decimales;
            foreach (var item in vector)
            {
                var newItem = _db.PedidosTotales.Create();
                var objIva = _db.TiposIva.Single(f => f.empresa == model.empresa && f.id == item.Key);
                newItem.empresa = model.empresa;
                newItem.fkpedidos = model.id;
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

                model.PedidosTotales.Add(newItem);
            }
        }

        private void CalcularTotalesCabecera(Pedidos model)
        {
            var decimales = _db.Monedas.Single(f => f.id == model.fkmonedas).decimales ?? 0;

            model.importebruto =Math.Round((double) model.PedidosTotales.Sum(f => f.brutototal),decimales);
            model.importedescuentoprontopago = Math.Round((double)model.PedidosTotales.Sum(f => f.importedescuentoprontopago),decimales);
            model.importedescuentocomercial = Math.Round((double)model.PedidosTotales.Sum(f => f.importedescuentocomercial), decimales);
            model.importebaseimponible = Math.Round((double)model.PedidosTotales.Sum(f => f.basetotal), decimales);
            model.importetotaldoc = Math.Round((double)model.PedidosTotales.Sum(f => f.subtotal), decimales);
            
            //todo revisar esto y recalcular el importe total
            model.importetotalmonedabase = model.PedidosTotales.Sum(f => f.subtotal*(model.cambioadicional??1.0));
        }

        #endregion

        #region Eliminar

        public override bool ValidarBorrar(Pedidos model)
        {
            if (_db.AlbaranesLin.Any(f => f.empresa == model.empresa && f.fkpedidos == model.id))
                throw new ValidationException(string.Format(General.ErrorIntegridadReferencial, RPedidos.TituloEntidadSingular, RAlbaranes.TituloEntidadSingular));

            return base.ValidarBorrar(model);
        }

        #endregion

       
    }
}
