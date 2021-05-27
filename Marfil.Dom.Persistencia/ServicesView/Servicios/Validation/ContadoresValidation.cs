using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Resources;
using RContadores=Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Contadores;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class ContadoresValidation : BaseValidation<Contadores>
    {
        public ContadoresValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override bool ValidarGrabar(Contadores model)
        {
            if(string.IsNullOrEmpty(model.descripcion))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RContadores.Descripcion));

            if (!model.tipoinicio.HasValue)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RContadores.Tipoinicio));

            if (!model.primerdocumento.HasValue)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RContadores.Primerdocumento));

            if (!model.ContadoresLin.Any())
                throw new ValidationException(RContadores.ErrorLineaObligatoria);

            //if(_db.Series.Any(f=>f.empresa==model.empresa && f.fkcontadores==model.id))
            //    throw new ValidationException(RContadores.ErrorContadorSerieUtilizada);

            ValidarLineas(model);

            return base.ValidarGrabar(model);
        }

        private void ValidarLineas(Contadores model)
        {
            var total = model.ContadoresLin.Sum(f => f.longitud);
            if(total>12)
                throw new ValidationException(RContadores.ErrorLongitudMaximaLineas);

            if(!model.ContadoresLin.Any(f=>f.tiposegmento==(int?)TiposSegmentos.Secuencia))
                throw new ValidationException(RContadores.ErrorSegmentoObligatorio);

            if(model.tipoinicio!=(int?)TipoInicio.Sinreinicio)
            {
                if (model.tipoinicio == (int?)TipoInicio.Mensual && !model.ContadoresLin.Any(f => f.tiposegmento == (int?)TiposSegmentos.Mes))
                    throw new ValidationException(RContadores.ErrorSegmentoMesObligatorio);
                else if (model.tipoinicio == (int?)TipoInicio.Anual && !model.ContadoresLin.Any(f => f.tiposegmento == (int?)TiposSegmentos.Año))
                    throw new ValidationException(RContadores.ErrorSegmentoAñoObligatorio);
            }
        }

        public override bool ValidarBorrar(Contadores model)
        {
            //todo check if it used in a serie
            return base.ValidarBorrar(model);
        }
    }
}
