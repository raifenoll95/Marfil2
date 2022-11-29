using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Documentos.Reservasstock;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RReservasstock = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Reservas;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
   
    internal class ReservasstockValidation : BaseValidation<Reservasstock>
    {
        public string EjercicioId { get; set; }
        public bool CambiarEstado { get; set; }
        public bool FlagActualizarCantidadesServidas { get; set; }
        public bool FlagActualizarCantidadesFacturadas { get; set; }

        public ReservasstockValidation(IContextService context, MarfilEntities db) : base(context,db)
        {

        }

        #region Validar grabar

        public override bool ValidarGrabar(Reservasstock model)
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

        private void ValidarEstado(Reservasstock model)
        {
            string message;
            if (!FlagActualizarCantidadesFacturadas)
            {
                if (!_appService.ValidarEstado(model.fkestados, _db, out message))
                    throw new ValidationException(message);
            }

            var estadosService = new EstadosService(Context,_db);
            var configuracionService = new ConfiguracionService(Context,_db);
            var configuracionModel = configuracionService.GetModel();
            var estadoactualObj = estadosService.get(model.fkestados) as EstadosModel;
            if (!(estadoactualObj.Tipomovimiento == Model.Configuracion.TipoMovimiento.Manual))
            {
                if (!string.IsNullOrEmpty(configuracionModel.Estadototal) && estadoactualObj.Tipoestado <= TipoEstado.Curso && model.ReservasstockLin.Any() && model.ReservasstockLin.All(f => (f.cantidad ?? 0) != 0 && (f.cantidad ?? 0) - (f.cantidadpedida ?? 0) <= 0))
                {
                    model.fkestados = configuracionModel.Estadoreservastotal;
                }
                else if (!string.IsNullOrEmpty(configuracionModel.Estadoparcial) && estadoactualObj.Tipoestado <= TipoEstado.Curso &&
                         model.ReservasstockLin.Any(f => (f.cantidadpedida ?? 0) > 0))
                {
                    model.fkestados = configuracionModel.Estadoparcial;
                }
                else if (!string.IsNullOrEmpty(configuracionModel.Estadoinicial) && estadoactualObj.Tipoestado <= TipoEstado.Curso &&
                         model.ReservasstockLin.Any(f => (f.cantidadpedida ?? 0) == 0))
                {
                    model.fkestados = configuracionModel.Estadoreservasinicial;
                }
            }
        }

        private bool ValidaRangoEjercicio(Reservasstock model)
        {
            var result = true;
            var ejercicio = model.fkejercicio;
            var ejercicioobj = _db.Ejercicios.Single(f => f.empresa == model.empresa && f.id == ejercicio);
            var fechadocumento = model.fechadocumento.Value;
            return fechadocumento >= ejercicioobj.desde.Value && fechadocumento <= ejercicioobj.hasta.Value;
        }

        private void ValidarCabecera(Reservasstock model)
        {
            if (model.fechadocumento == null)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RReservasstock.Fechadocumento));
            if (model.fkclientes == null)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RReservasstock.Fkclientes));
            if (!model.fkmonedas.HasValue)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RReservasstock.Fkmonedas));
            if (!model.fkformaspago.HasValue)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RReservasstock.Fkformaspago));

            var clienteObj = _db.Clientes.Single(f => f.empresa == model.empresa && f.fkcuentas == model.fkclientes);
            var serieObj =
                _db.Series.Single(f => f.empresa == model.empresa && f.tipodocumento == "RES" && f.id == model.fkseries);

            if(serieObj.fkmonedas.HasValue && serieObj.fkmonedas!=clienteObj.fkmonedas)
                throw new ValidationException(RReservasstock.ErrorMonedaClienteSerie);

            if (!FlagActualizarCantidadesFacturadas && !ValidaRangoEjercicio(model))
                throw new ValidationException(RReservasstock.ErrorFechaEjercicio);
            
            model.integridadreferencial = Guid.NewGuid();
        }

        private void ValidarLineas(Reservasstock model)
        {
            var familiasProductosService = FService.Instance.GetService(typeof(FamiliasproductosModel), Context, _db) as FamiliasproductosService;
            foreach (var item in model.ReservasstockLin)
            {
                if (string.IsNullOrEmpty(item.fkarticulos))
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RReservasstock.Fkarticulos));

                if (!item.cantidad.HasValue)
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RReservasstock.Cantidad));

                if (!item.metros.HasValue)
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RReservasstock.Metros));

                if (!item.precio.HasValue)
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RReservasstock.Precio));

                if (string.IsNullOrEmpty(item.fktiposiva))
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RReservasstock.Fktiposiva));

                if (item.porcentajedescuento.HasValue)
                    item.importedescuento = Math.Round((double)(item.precio*item.metros*item.porcentajedescuento)/100.0,2);

                var familiacodigo = ArticulosService.GetCodigoFamilia(item.fkarticulos);
                var familiaModel = familiasProductosService.get(familiacodigo) as FamiliasproductosModel;
                item.fkunidades = _db.Unidades.Single(f => f.id == familiaModel.Fkunidadesmedida).id;

                var art = _db.Articulos.Single(f => f.empresa == model.empresa && f.id == item.fkarticulos);
                var codGrupo = art.fkgruposiva;
                var tiposivaService= FService.Instance.GetService(typeof (TiposIvaModel), Context, _db) as TiposivaService;
                item.fktiposiva = tiposivaService.GetTipoIva(codGrupo, model.fkregimeniva).Id;

                double cantidad = item.metros??0;
                double precio = item.precio ?? 0;
                double importedescuento = item.importedescuento ?? 0;

                var baseimponible = cantidad * precio - importedescuento;

                item.importe = baseimponible;

                if (!item.importe.HasValue)
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RReservasstock.Importe));

                familiasProductosService.ValidarDimensiones(familiacodigo, item.largo, item.ancho, item.grueso, art.id);

                if (!FlagActualizarCantidadesFacturadas)
                    VerificarStockLinea(model, item);
            }
            var vector = model.ReservasstockLin.OrderBy(f => f.orden).ToList();
            for (var i = 0; i < vector.Count(); i++)
                vector[i].orden = (i + 1) * ApplicationHelper.EspacioOrdenLineas;
        }

        #region Validar restricciones stock

        private void VerificarStockLinea(Reservasstock model, ReservasstockLin linea)
        {
            if (!string.IsNullOrEmpty(linea.lote))
            {
                VerificarDisponibilidad(model, linea);
                VerificarPertenenciaKit(model, linea);

            }
        }

        private void VerificarDisponibilidad(Reservasstock model, ReservasstockLin linea)
        {
            var loteid = linea.tabla?.ToString()?? string.Empty;
            if (
                !_db.Stockactual.Any(
                    f =>
                        f.empresa == model.empresa && f.fkalmacenes == model.fkalmacen &&
                        f.fkarticulos == linea.fkarticulos && f.lote == linea.lote && f.loteid == loteid && f.cantidaddisponible >= linea.cantidad) &&
                        !_db.ReservasstockLin.Any(f=>f.empresa ==model.empresa && f.fkreservasstock ==model.id && 
                        f.fkarticulos == linea.fkarticulos && f.lote == linea.lote && f.tabla == linea.tabla))
            {
                throw new ValidationException(string.Format("El Lote {0}{1} no dispone de {2} pieza ",linea.lote,Funciones.RellenaCod(loteid,3),linea.cantidad));
            }
        }

        private void VerificarPertenenciaKit(Reservasstock model, ReservasstockLin linea)
        {
            var loteid = linea.tabla?.ToString() ?? string.Empty;
            if (_db.KitLin.Any(f => f.empresa == model.empresa && f.lote == linea.lote && f.loteid == loteid))
            {
                var kitobj =
                    _db.Kit.Include("KitLin")
                        .Single(
                            f =>
                                f.empresa == model.empresa &&
                                f.KitLin.Any(j => j.lote == linea.lote && j.loteid == loteid));
                if (!kitobj.KitLin.All(f => model.ReservasstockLin.Any(j => j.lote == f.lote && j.tabla.ToString() == f.loteid)))
                {
                    throw new ValidationException(string.Format("El Lote {0}{1} pertenece al Kit {2} que no está completo. Todas los registros del Kit deben añadirse al albarán.", linea.lote, Funciones.RellenaCod(loteid, 3), kitobj.referencia));
                }

                kitobj.estado = (int)EstadoKit.Vendido;
                _db.Kit.AddOrUpdate(kitobj);
            }

            if (_db.BundleLin.Any(f => f.empresa == model.empresa && f.lote == linea.lote && f.loteid == loteid))
            {
                var bundleobj =
                    _db.Bundle.Include("BundleLin")
                        .Single(
                            f =>
                                f.empresa == model.empresa &&
                                f.BundleLin.Any(j => j.lote == linea.lote && j.loteid == loteid));
                if (!bundleobj.BundleLin.All(f => model.ReservasstockLin.Any(j => j.lote == f.lote && j.tabla.ToString() == f.loteid)))
                {
                    throw new ValidationException(string.Format("El Lote {0}{1} pertenece al Bundle {0}{2} que no está completo. Todas los registros del Bundle deben añadirse al albarán.", linea.lote, Funciones.RellenaCod(loteid, 3), bundleobj.id));
                }

                bundleobj.estado = (int)EstadoKit.Vendido;
                _db.Bundle.AddOrUpdate(bundleobj);
            }
        }

        #endregion

        private void CalcularTotales(Reservasstock model)
        {
            model.ReservasstockTotales.Clear();
            var vector = model.ReservasstockLin.GroupBy(f => f.fktiposiva);
            var decimales = _db.Monedas.Single(f => f.id == model.fkmonedas).decimales;
            foreach (var item in vector)
            {
                var newItem = _db.ReservasstockTotales.Create();
                var objIva = _db.TiposIva.Single(f => f.empresa == model.empresa && f.id == item.Key);
                newItem.empresa = model.empresa;
                newItem.fkreservasstock = model.id;
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

                model.ReservasstockTotales.Add(newItem);
            }
        }

        private void CalcularTotalesCabecera(Reservasstock model)
        {
            var decimales = _db.Monedas.Single(f => f.id == model.fkmonedas).decimales ?? 0;

            model.importebruto =Math.Round((double) model.ReservasstockTotales.Sum(f => f.brutototal),decimales);
            model.importedescuentoprontopago = Math.Round((double)model.ReservasstockTotales.Sum(f => f.importedescuentoprontopago),decimales);
            model.importedescuentocomercial = Math.Round((double)model.ReservasstockTotales.Sum(f => f.importedescuentocomercial), decimales);
            model.importebaseimponible = Math.Round((double)model.ReservasstockTotales.Sum(f => f.basetotal), decimales);
            model.importetotaldoc = Math.Round((double)model.ReservasstockTotales.Sum(f => f.subtotal), decimales);
            
            //todo revisar esto y recalcular el importe total
            model.importetotalmonedabase = model.ReservasstockTotales.Sum(f => f.subtotal*(model.cambioadicional??1.0));
        }

        #endregion

        #region Eliminar

        public override bool ValidarBorrar(Reservasstock model)
        {
            ValidarEstadoKit(model);

            return base.ValidarBorrar(model);
        }

        private void ValidarEstadoKit(Reservasstock model)
        {
            foreach (var linea in model.ReservasstockLin)
            {
                var loteid = linea.tabla?.ToString() ?? string.Empty;
                if (_db.KitLin.Any(f => f.empresa == model.empresa && f.lote == linea.lote && f.loteid == loteid))
                {
                    var kitobj =
                        _db.Kit.Include("KitLin")
                            .Single(
                                f =>
                                    f.empresa == model.empresa &&
                                    f.KitLin.Any(j => j.lote == linea.lote && j.loteid == loteid));

                    kitobj.estado = (int)EstadoKit.Montado;
                    _db.Kit.AddOrUpdate(kitobj);
                }
            }
            
        }

        #endregion

       
    }
}
