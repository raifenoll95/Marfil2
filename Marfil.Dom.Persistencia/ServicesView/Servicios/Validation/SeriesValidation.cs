using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Resources;
using RSeries=Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Series;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class SeriesValidation : BaseValidation<Series>
    {
        public SeriesValidation(IContextService context, MarfilEntities db) : base(context,db)
        {
        }

        public override bool ValidarGrabar(Series model)
        {
            if (model.bloqueada.HasValue && model.bloqueada.Value)
                throw new ValidationException(General.ErrorModificarRegistroBloqueado);

            if (string.IsNullOrEmpty(model.descripcion))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RSeries.Descripcion));

            if (string.IsNullOrEmpty(model.fkcontadores))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RSeries.Fkcontadores));

            var objcontador = _db.Contadores.Single(f => f.empresa == model.empresa && f.id == model.fkcontadores);
            if (objcontador.tipocontador == (int)TipoContador.Privado)
            {
                if (_db.Series.Count(f => f.empresa == model.empresa && f.fkcontadores == model.fkcontadores && model.id!=f.id) > 0)
                    throw new ValidationException(RSeries.ErrorContadorPridado);
            }

            return base.ValidarGrabar(model);
        }

        public override bool ValidarBorrar(Series model)
        {
            //todo check if it used in a serie
            return base.ValidarBorrar(model);
        }
    }
}
