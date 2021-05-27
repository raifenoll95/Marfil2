using System;
using System.Linq;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RTransformaciones = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Transformaciones;
using RAlbaranes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Albaranes;
using RAlbaranesCompras = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.AlbaranesCompras;
using Marfil.Dom.Persistencia.Model.Configuracion;
using System.Collections.Generic;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{

    internal class TransformacionesValidation : BaseValidation<Transformaciones>
    {
        public string EjercicioId { get; set; }
        public bool ModificarCostes { get; set; }
        public bool CambiarEstado { get; set; }

        public TransformacionesValidation(IContextService context, MarfilEntities db) : base(context,db)
        {

        }

        #region Validar grabar

        public override bool ValidarGrabar(Transformaciones model)
        {
            if (!CambiarEstado)
            {
                if (!ModificarCostes)
                    ValidarEstado(model);
                ValidarCabecera(model);
                ValidarLineas(model);
                CalcularCostesadicionales(model);
                return base.ValidarGrabar(model);
            }
            else
            {
                ValidarCabecera(model);
                ValidarLineas(model);
                CalcularCostesadicionales(model);

            }

            return true;

        }

        private void ValidarEstado(Transformaciones model)
        {
            string message;
            if (!_appService.ValidarEstado(model.fkestados, _db, out message))
                throw new ValidationException(message);
        }

        private void ValidarCabecera(Transformaciones model)
        {
            if (model.fechadocumento == null)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Fechadocumento));

            if (model.fkproveedores == null)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Fkproveedores));
           
            if (string.IsNullOrEmpty(model.fkalmacen))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Fkalmacen));

            if (!string.IsNullOrEmpty(model.fkzonas))
            {
                var idInt = Funciones.Qint(model.fkzonas) ?? 0;
                if (!_db.AlmacenesZona.Any(
                    f => f.empresa == model.empresa && f.fkalmacenes == model.fkalmacen && f.id == idInt))
                    throw new ValidationException(string.Format(RAlbaranes.ErrorZonaAlmacen, model.fkzonas, model.fkalmacen));

            }

            var cuentasService = FService.Instance.GetService(typeof (CuentasModel), Context, _db);
            var cuentasObj = cuentasService.get(model.fkproveedores) as CuentasModel;
            model.nombreproveedor = cuentasObj.Descripcion;

            model.integridadreferencialflag = Guid.NewGuid();
        }

        private void ValidarLineas(Transformaciones model)
        {


            if (!model.Transformacionessalidalin.Any())
                throw new ValidationException(string.Format(RTransformaciones.ErrorLineasObligatorias,RTransformaciones.Salida));

            
            var estado = _db.Estados.Single(f => f.documento + "-" + f.id == model.fkestados);
            if (!model.Transformacionesentradalin.Any() && estado.tipoestado==(int)TipoEstado.Finalizado)
                throw new ValidationException(string.Format(RTransformaciones.ErrorLineasObligatorias, RTransformaciones.Entrada));

            String excepcionesGeneradas = "";

            //VERIFICAMOS LAS LINEAS DE LAS TRANSFORMACIONES DE ENTRADAS SON CORRECTAS
            foreach (var item in model.Transformacionesentradalin)
            {

                bool hayEnStock = _db.Stockactual.Any(f => f.empresa == model.empresa && f.lote == item.lote && f.loteid == item.id.ToString());
                bool hayEnStockHistorico = _db.Stockhistorico.Any(f => f.empresa == model.empresa && f.lote == item.lote && f.loteid == item.id.ToString());

                if (hayEnStock)
                    excepcionesGeneradas += string.Format("El Lote: {0}.{1} ya existe en el Stock<br />", item.lote, item.id);

                if (!hayEnStock && hayEnStockHistorico)
                    excepcionesGeneradas += string.Format("El Lote: {0}.{1} ya ha sido Vendido<br />", item.lote, item.id);

                if (string.IsNullOrEmpty(item.fkarticulos))
                    excepcionesGeneradas += string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Fkarticulos);

                if (!item.cantidad.HasValue)
                    excepcionesGeneradas += string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Cantidad);

                if (!item.metros.HasValue)
                    excepcionesGeneradas += string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Metros);
            }

            if (!String.IsNullOrEmpty(excepcionesGeneradas))
            {
                throw new ValidationException(excepcionesGeneradas);
            }

            foreach (var item in model.Transformacionessalidalin)
            {
                if (string.IsNullOrEmpty(item.fkarticulos))
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Fkarticulos));

                if (!item.cantidad.HasValue)
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Cantidad));

                if (!item.metros.HasValue)
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Metros));
            }

            var vector = model.Transformacionesentradalin.OrderBy(f => f.orden).ToList();
            for (var i = 0; i < vector.Count(); i++)
                vector[i].orden = (i + 1) * ApplicationHelper.EspacioOrdenLineas;

            var vectorsalida = model.Transformacionessalidalin.OrderBy(f => f.orden).ToList();
            for (var i = 0; i < vectorsalida.Count(); i++)
                vectorsalida[i].orden = (i + 1) * ApplicationHelper.EspacioOrdenLineas;
        }

        private void CalcularCostesadicionales(Transformaciones model)
        {
            var decimales = 2;//_db.Monedas.Single(f => f.id == model.fkmonedas).decimales ?? 0;

            foreach (var item in model.Transformacionescostesadicionales)
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

        #endregion

        #region Validar borrar

        public override bool ValidarBorrar(Transformaciones model)
        {
            var estadosServicios = FService.Instance.GetService(typeof(EstadosModel), Context, _db);
            var estadoObj = estadosServicios.get(model.fkestados) as EstadosModel;
            if (estadoObj.Tipoestado >= TipoEstado.Finalizado)
                throw new ValidationException("Sólo se pueden eliminar transformaciones en estado inicial o en curso");
            return base.ValidarBorrar(model);

        }

        #endregion
    }
}
