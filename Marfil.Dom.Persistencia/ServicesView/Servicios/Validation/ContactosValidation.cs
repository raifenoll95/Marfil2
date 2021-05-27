using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Resources;
using RContactos= Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Contactos;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class ContactosValidation : BaseValidation<Contactos>
    {
        #region CTR

        public ContactosValidation(IContextService context, MarfilEntities db) : base(context, db)
        {

        }

        #endregion

        public override bool ValidarGrabar(Contactos model)
        {
            if (string.IsNullOrEmpty(model.nombre))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio,RContactos.Nombre));
            
            if (!VerficarNifCif(model))
                throw new ValidationException(RContactos.ErrorNifObligatorioCargoEmpresa);

            return base.ValidarGrabar(model);
        }

        private bool VerficarNifCif(Contactos model)
        {
            var service= new TablasVariasService(Context, _db);
            var cargos = service.GetTablasVariasByCode(2050);
            return
                !(string.IsNullOrEmpty(model.nifcif) &&
                  cargos.Lineas.Any(f => f.Valor == model.fkcargoempresa && f.NifObligatorio));

        }
    }
}
