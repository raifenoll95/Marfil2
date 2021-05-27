using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RTarifas= Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Tarifas;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class TarifasValidation : BaseValidation<Tarifas>
    {
        public TarifasValidation(IContextService context, MarfilEntities db) : base(context,db)
        {
        }

        public override bool ValidarGrabar(Tarifas model)
        {
            if(!model.tipotarifa.HasValue)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio,RTarifas.Tipotarifa));

            var tipotarifa = (TipoTarifa) model.tipotarifa.Value;

            if(string.IsNullOrEmpty(model.descripcion))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RTarifas.Descripcion));
            if (tipotarifa==TipoTarifa.Sistema)
                ValidarTarifaSistema(model);
        

            return base.ValidarGrabar(model);
        }

        private void ValidarTarifaSistema(Tarifas model)
        {
            model.asignartarifaalcreararticulos = true;
            model.validodesde = null;
            model.validohasta = null;
            model.fkcuentas = string.Empty;
            model.fkmonedas = Funciones.Qint(_appService.GetCurrentEmpresa(model.empresa).FkMonedaBase);
        }

        

        public override bool ValidarBorrar(Tarifas model)
        {
            if (model.TarifasLin.Any())
                throw new ValidationException(RTarifas.ErrorLineasExistentes);

            return base.ValidarBorrar(model);
        }
    }
}
