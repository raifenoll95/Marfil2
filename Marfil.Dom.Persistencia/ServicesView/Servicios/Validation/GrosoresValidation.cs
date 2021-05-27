using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Inf.Genericos;
using Resources;
using RGrosores = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Grosores;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class GrosoresValidation : BaseValidation<Grosores>
    {
        //todo: coger los espesores de la configuración de la aplicación
        private readonly double _espesorfleje=0;
        private readonly double _espesordisco=0;

        public GrosoresValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
            var configuracion = db.Configuracion.FirstOrDefault();
            if (configuracion != null)
            {
                var serializer=new Serializer<InternalConfiguracionModel>();
                var model = serializer.SetXml(configuracion.xml);
                _espesorfleje =  model.Espesorfleje;
                _espesordisco = model.Espesordisco;
            }
        }

        public override bool ValidarGrabar(Grosores model)
        {
            if (string.IsNullOrEmpty(model.descripcion))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RGrosores.Descripcion));

            if (!model.grosor.HasValue)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RGrosores.Grosor));

            RecalculateCoeficients(model);

            return base.ValidarGrabar(model);
        }

        private void RecalculateCoeficients(Grosores model)
        {
            model.coeficientetelares = (model.grosor*100 + _espesorfleje) /(2+ _espesorfleje);
            model.coficientecortabloques = (model.grosor * 100 + _espesordisco) / (2 + _espesordisco);
        }
    }
}
