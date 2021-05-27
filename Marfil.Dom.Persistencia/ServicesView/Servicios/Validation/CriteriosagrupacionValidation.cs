using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Inf.Genericos;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RCriteriosagrupacion = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Criteriosagrupacion;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class CriteriosagrupacionValidation : BaseValidation<Criteriosagrupacion>
    {
        public CriteriosagrupacionValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override bool ValidarGrabar(Criteriosagrupacion model)
        {
            if (string.IsNullOrEmpty(model.nombre))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RCriteriosagrupacion.Nombre));

            //verificar lineas obligatorias
            if (model.CriteriosagrupacionLin.Any())
            {
                if (model.CriteriosagrupacionLin.All(f => f.campo != (int)CamposAgrupacionAlbaran.Fkarticulos))
                    throw new ValidationException(string.Format(RCriteriosagrupacion.ErrorCriterioObligatorio, Funciones.GetEnumByStringValueAttribute(CamposAgrupacionAlbaran.Fkarticulos)));
                if (model.CriteriosagrupacionLin.All(f => f.campo != (int)CamposAgrupacionAlbaran.Precio))
                    throw new ValidationException(string.Format(RCriteriosagrupacion.ErrorCriterioObligatorio, Funciones.GetEnumByStringValueAttribute(CamposAgrupacionAlbaran.Precio)));
                if (model.CriteriosagrupacionLin.All(f => f.campo != (int)CamposAgrupacionAlbaran.Porcentajedescuento))
                    throw new ValidationException(string.Format(RCriteriosagrupacion.ErrorCriterioObligatorio, Funciones.GetEnumByStringValueAttribute(CamposAgrupacionAlbaran.Porcentajedescuento)));
            }
            

            return base.ValidarGrabar(model);
        }
    }
}
