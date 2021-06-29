using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Resources;
using RCuadernos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.CuadernosBancarios;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    class CuadernosBancariosValidation : BaseValidation<CuadernosBancarios>
    {
        public CuadernosBancariosValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override bool ValidarGrabar(CuadernosBancarios model)
        {
            if (string.IsNullOrEmpty(model.descripcion))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RCuadernos.Descripcion));

            //ValidarLineas(model);

            return base.ValidarGrabar(model);
        }

        private void ValidarLineas(CuadernosBancarios model)
        {
            if (model.CuadernosBancariosLin.Any(f => f.etiquetaIni != "" && f.etiquetaFin == "") 
                || model.CuadernosBancariosLin.Any(f => f.etiquetaIni == "" && f.etiquetaFin != ""))
                throw new ValidationException(RCuadernos.ErrorFaltaEtiqueta);
        }

        public override bool ValidarBorrar(CuadernosBancarios model)
        {
            //todo check if it used in a serie
            return base.ValidarBorrar(model);
        }
    }
}
