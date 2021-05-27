using System;
using System.Linq;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RTransformacioneslotes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Transformaciones;
using RAlbaranes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Albaranes;
using RAlbaranesCompras = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.AlbaranesCompras;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{

    internal class TransformacioneslotesValidation : BaseValidation<Transformacioneslotes>
    {
        public string EjercicioId { get; set; }
        public bool ModificarCostes { get; set; }
        public bool CambiarEstado { get; set; }


        public TransformacioneslotesValidation(IContextService context, MarfilEntities db) : base(context,db)
        {

        }

        #region Validar grabar

        public override bool ValidarGrabar(Transformacioneslotes model)
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
            return true;
        }

        private void ValidarEstado(Transformacioneslotes model)
        {
            string message;
            if (!_appService.ValidarEstado(model.fkestados, _db, out message))
                throw new ValidationException(message);
        }

        private void ValidarCabecera(Transformacioneslotes model)
        {
            
           

            if (model.fechadocumento == null)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Fechadocumento));

            if (model.fkproveedores == null)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Fkproveedores));
           
            if (string.IsNullOrEmpty(model.fkalmacen))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Fkalmacen));

            if (!model.Transformacionesloteslin.Any())
                throw new ValidationException(string.Format(RTransformacioneslotes.ErrorLineasObligatorias, RTransformacioneslotes.Salida));

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

        private void ValidarLineas(Transformacioneslotes model)
        {

            foreach (var item in model.Transformacionesloteslin)
            {
                if (string.IsNullOrEmpty(item.fkarticulos))
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Fkarticulos));

                if (!item.cantidad.HasValue)
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Cantidad));

                if (!item.metros.HasValue)
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Metros));
            }

            
            var vector = model.Transformacionesloteslin.OrderBy(f => f.orden).ToList();
            for (var i = 0; i < vector.Count(); i++)
                vector[i].orden = (i + 1) * ApplicationHelper.EspacioOrdenLineas;
        }

        private void CalcularCostesadicionales(Transformacioneslotes model)
        {
            var decimales = 2;//_db.Monedas.Single(f => f.id == model.fkmonedas).decimales ?? 0;

            foreach (var item in model.Transformacioneslotescostesadicionales)
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

        public override bool ValidarBorrar(Transformacioneslotes model)
        {
            var estadosServicios = FService.Instance.GetService(typeof (EstadosModel), Context, _db);
            var estadoObj = estadosServicios.get(model.fkestados) as EstadosModel;
            if(estadoObj.Tipoestado>=TipoEstado.Finalizado)
                throw new ValidationException("Sólo se pueden eliminar transformaciones en estado inicial o en curso");
            return base.ValidarBorrar(model);

        }

        #endregion
    }
}
