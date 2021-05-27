using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Resources;
using RFamiliasproductos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Familiasproductos;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    class FamiliasproductosValidation : BaseValidation<Familiasproductos>
    {
        public FamiliasproductosValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override bool ValidarGrabar(Familiasproductos model)
        {
            VerificarTipoFamilia(model);

            if (string.IsNullOrEmpty(model.fkunidadesmedida))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RFamiliasproductos.Fkunidadesmedida));

            if ((!(model.gestionstock??false) || model.tipogestionlotes==(int?)Tipogestionlotes.Singestion) && !string.IsNullOrEmpty(model.fkcontador))
                throw new ValidationException(RFamiliasproductos.ErrorContadorGestionStock);

            return base.ValidarGrabar(model);
        }

        private void VerificarTipoFamilia(Familiasproductos model)
        {
            
            if ((TipoFamilia)(model.tipofamilia ?? 0) != TipoFamilia.General && (TipoFamilia)(model.tipofamilia ?? 0) != TipoFamilia.Libre)
            {
                model.tipogestionlotes = (int)Tipogestionlotes.Loteobligatorio;
                model.gestionstock = true;
            }

            if ((TipoFamilia) (model.tipofamilia ?? 0) == TipoFamilia.Libre)
            {
                model.validargrosor = false;
                model.validaracabado = false;
            }
            else
            {
                model.validarmaterial = true;
                model.validarcaracteristica = true;
                model.validargrosor = true;
                model.validaracabado = true;
            }
        }

        public override bool ValidarBorrar(Familiasproductos model)
        {
            if(_db.Articulos.Any(f=>f.empresa == model.empresa && f.id.StartsWith(model.id)))
                throw new ValidationException("No se puede borrar la familia {0} porque existen articulos relacionados");

            if(_db.Caracteristicas.Any(f=>f.empresa==model.empresa && f.id== model.id))
                throw new ValidationException("No se puede borrar la familia {0} porque existen caracteristicas relacionadas");

            return base.ValidarBorrar(model);


        }
    }
}

