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
    internal class ContadoresLotesValidation : BaseValidation<ContadoresLotes>
    {
        public ContadoresLotesValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override bool ValidarGrabar(ContadoresLotes model)
        {
            if (_db.Movimientosstock.Any(f => f.fkcontadorlote == model.id && f.empresa==model.empresa))
                throw new ValidationException("No se puede modificar un contador de lote que ya ha sido usado");

            if (string.IsNullOrEmpty(model.descripcion))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RContadores.Descripcion));

            if (!model.tipoinicio.HasValue)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RContadores.Tipoinicio));

            if (!model.primerdocumento.HasValue)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RContadores.Primerdocumento));

            if (!model.ContadoresLotesLin.Any())
                throw new ValidationException(RContadores.ErrorLineaObligatoria);

            ValidarLineas(model);

            return base.ValidarGrabar(model);
        }

        private void ValidarLineas(ContadoresLotes model)
        {
            var total = model.ContadoresLotesLin.Sum(f => f.longitud);
            if(total>12)
                throw new ValidationException(RContadores.ErrorLongitudMaximaLineas);

            if(!model.ContadoresLotesLin.Any(f=>f.tiposegmento==(int?)TiposLoteSegmentos.Secuencia))
                throw new ValidationException(RContadores.ErrorSegmentoObligatorio);

            if(model.tipoinicio!=(int?)TipoInicio.Sinreinicio)
            {
                if (model.tipoinicio == (int?)TipoLoteInicio.Mensual && !model.ContadoresLotesLin.Any(f => f.tiposegmento == (int?)TiposLoteSegmentos.Mes))
                    throw new ValidationException(RContadores.ErrorSegmentoMesObligatorio);
                else if (model.tipoinicio == (int?)TipoLoteInicio.Anual && !model.ContadoresLotesLin.Any(f => f.tiposegmento == (int?)TiposLoteSegmentos.Año))
                    throw new ValidationException(RContadores.ErrorSegmentoAñoObligatorio);
            }

            var vector = model.ContadoresLotesLin.Where(f => f.tiposegmento == (int?) TiposLoteSegmentos.Constante);
            foreach (var item in vector)
                item.valor = item.valor?.ToUpper()??string.Empty;
        }

        public override bool ValidarBorrar(ContadoresLotes model)
        {
            //todo check if it used in a serie
            return base.ValidarBorrar(model);
        }
    }
}
