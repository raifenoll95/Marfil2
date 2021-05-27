

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Documentos.Albaranes;
using Marfil.Dom.Persistencia.Model.Contabilidad.Movs;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RMovs = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Movs;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{

    internal class MovsValidation : BaseValidation<Movs>
    {
        public string EjercicioId { get; set; }

        public MovsValidation(IContextService context, MarfilEntities db) : base(context, db)
        {

        }

        #region Validar grabar

        //        public override bool ValidarGrabar(Movs model)
        //        {

        ////            if (!CambiarEstado)
        ////            {
        //                ValidarCabecera(model);
        //  //              ValidarLineas(model);
        //  //              CalcularTotales(model);
        //   //             CalcularTotalesCabecera(model);

        //            }

        //            return base.ValidarGrabar(model);
        //        }
        public override bool ValidarGrabar(Movs model)
        {
            //if (string.IsNullOrEmpty(model.nombre))
            //    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RFormasPago.Nombre));

            if (model.bloqueado.HasValue && model.bloqueado.Value)
            {
                throw new ValidationException(General.ErrorModificarRegistroBloqueado);
            }

            var totaldebe = model.MovsLin.Where(f => f.esdebe == 1).Sum(f => f.importe);
            var totalhaber = model.MovsLin.Where(f => f.esdebe == -1).Sum(f => f.importe);

            model.debe = totaldebe;
            model.haber = totalhaber;

            if (totaldebe - totalhaber != 0)
                throw new ValidationException(RMovs.ErrorDescuadreDebeHaber);
            
            ValidarCabecera(model);

            return base.ValidarGrabar(model);
        }

        private bool ValidaRangoEjercicio(Movs model)
        {
            var result = true;
            var ejercicio = model.fkejercicio;
            var ejercicioobj = _db.Ejercicios.Single(f => f.empresa == model.empresa && f.id == ejercicio);
            var fecha = model.fecha.Value;
            return fecha >= ejercicioobj.desde.Value && fecha <= ejercicioobj.hasta.Value;
        }

        private void ValidarCabecera(Movs model)
        {
            if (model.fecha == null)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RMovs.Fecha));
            if (model.fkseriescontables == null)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RMovs.Fkseriescontables));

            //    var monedas = _db.Proveedores.Any(f => f.empresa == model.empresa && f.fkcuentas == model.fkproveedores) ? _db.Proveedores.SingleOrDefault(f => f.empresa == model.empresa && f.fkcuentas == model.fkproveedores).fkmonedas : _db.Acreedores.Single(f => f.empresa == model.empresa && f.fkcuentas == model.fkproveedores).fkmonedas;

            if (!ValidaRangoEjercicio(model))
                throw new ValidationException(RMovs.ErrorFechaEjercicio);

            model.integridadreferencial = Guid.NewGuid();

        }

        //private void ValidarLineas(Movs model)
        //{

        //    if (model.MovsCostesadicionales.Any() && !model.MovsLin.Any())
        //        throw new ValidationException(General.ErrorLineasObligatoriasCostes);
        //    var familiasProductosService = FService.Instance.GetService(typeof(FamiliasproductosModel), Context, _db);
        //    foreach (var item in model.MovsLin)
        //    {
        //        if (string.IsNullOrEmpty(item.fkarticulos))
        //            throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RMovs.Fkarticulos));

        //        if (!item.cantidad.HasValue)
        //            throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RMovs.Cantidad));

        //        if (!item.metros.HasValue)
        //            throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RMovs.Metros));

        //        if (!item.precio.HasValue)
        //            throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RMovs.Precio));

        //        if (string.IsNullOrEmpty(item.fktiposiva))
        //            throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RMovs.Fktiposiva));

        //        if (item.porcentajedescuento.HasValue)
        //            item.importedescuento = Math.Round((double)(item.precio * item.metros * item.porcentajedescuento) / 100.0, 2);

        //        var familiacodigo = ArticulosService.GetCodigoFamilia(item.fkarticulos);
        //        var familiaModel = familiasProductosService.get(familiacodigo) as FamiliasproductosModel;
        //        item.fkunidades = _db.Unidades.Single(f => f.empresa == model.empresa && f.id == familiaModel.Fkunidadesmedida).id;

        //        var art = _db.Articulos.Single(f => f.empresa == model.empresa && f.id == item.fkarticulos);
        //        var codGrupo = art.fkgruposiva;
        //        if (!art.tipoivavariable)
        //        {
        //            var tiposivaService = FService.Instance.GetService(typeof(TiposIvaModel), Context, _db) as TiposivaService;
        //            item.fktiposiva = tiposivaService.GetTipoIva(codGrupo, model.fkregimeniva).Id;
        //        }


        //        double cantidad = item.metros ?? 0;
        //        double precio = item.precio ?? 0;
        //        double importedescuento = item.importedescuento ?? 0;

        //        var baseimponible = cantidad * precio - importedescuento;

        //        item.importe = baseimponible;

        //        if (!item.importe.HasValue)
        //            throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RMovs.Importe));
        //    }

        //    var vector = model.MovsLin.OrderBy(f => f.orden).ToList();
        //    for (var i = 0; i < vector.Count(); i++)
        //        vector[i].orden = (i + 1) * ApplicationHelper.EspacioOrdenLineas;

        //}

        //private void CalcularTotales(Movs model)
        //{
        //    model.MovsTotales.Clear();
        //    var vector = model.MovsLin.GroupBy(f => f.esdebe);
        //    var decimales = _db.Monedas.Single(f => f.id == model.fkmonedas).decimales;
        //    foreach (var item in vector)
        //    {
        //        var newItem = _db.MovsTotales.Create();
        //        //var objIva = _db.TiposIva.Single(f => f.empresa == model.empresa && f.id == item.Key);
        //        newItem.empresa = model.empresa;
        //        newItem.fkmovs = model.id;
        //        newItem.esdebe = item.Key;


        //        //newItem.porcentajeiva = objIva.porcentajeiva;

        //        // newItem.importe = Math.Round((decimal)(item.Sum(f => f.importe)), decimales.Value);
        //        newItem.importe = (decimal)(item.Sum(f => f.importe));

        //        model.MovsTotales.Add(newItem);
        //    }
        //}

        //private void CalcularTotalesCabecera(Movs model)
        //{
        //    var decimales = _db.Monedas.Single(f => f.id == model.fkmonedas).decimales ?? 0;

        //    model.importebruto = Math.Round((double)model.MovsTotales.Sum(f => f.brutototal), decimales);
        //    model.importedescuentoprontopago = Math.Round((double)model.MovsTotales.Sum(f => f.importedescuentoprontopago), decimales);
        //    model.importedescuentocomercial = Math.Round((double)model.MovsTotales.Sum(f => f.importedescuentocomercial), decimales);
        //    model.importebaseimponible = Math.Round((double)model.MovsTotales.Sum(f => f.basetotal), decimales);
        //    model.importetotaldoc = Math.Round((double)model.MovsTotales.Sum(f => f.subtotal), decimales);

        //    //todo revisar esto y recalcular el importe total
        //    model.importetotalmonedabase = model.MovsTotales.Sum(f => f.subtotal * (model.cambioadicional ?? 1.0));
        //}



        #endregion

        #region Eliminar

        //public override bool ValidarBorrar(Movs model)
        //{
        ////if (_db.FacturasLin.Any(f => f.fkalbaranesreferencia == model.referencia && f.empresa == model.empresa))
        ////    throw new ValidationException(RMovs.ErrorAlbaranFacturado);

        ////return base.ValidarBorrar(model);

        //}

        #endregion


    }
}
