using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Resources;
using RSeguimientos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Seguimientos;
using Marfil.Dom.Persistencia.Model.Configuracion;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class SeguimientosValidation : BaseValidation<Seguimientos>
    {
        public bool CambiarEstado { get; set; }

        public SeguimientosValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override bool ValidarGrabar(Seguimientos model)
        {
            if (!CambiarEstado)
            {
                ValidarEstado(model);
                ValidarEstadoPadre(model);
            }

            return base.ValidarGrabar(model);
        }

        private void ValidarEstado(Seguimientos model)
        {
            string message;
            if (!_appService.ValidarEstado(model.fketapa, _db, out message))
                throw new ValidationException(message);

        }

        private void ValidarEstadoPadre(Seguimientos model)
        {
            var cerrado = false;

            if (model.tipo == (int)DocumentoEstado.Oportunidades)
            {
                cerrado = _db.Oportunidades.Where(f => f.empresa == model.empresa && f.referencia == model.origen).Select(f => f.cerrado).SingleOrDefault() ?? false;
            }
            else if (model.tipo == (int)DocumentoEstado.Proyectos)
            {
                cerrado = _db.Oportunidades.Where(f => f.empresa == model.empresa && f.referencia == model.origen).Select(f => f.cerrado).SingleOrDefault() ?? false;
            }
            else if (model.tipo == (int)DocumentoEstado.Campañas)
            {
                cerrado = _db.Oportunidades.Where(f => f.empresa == model.empresa && f.referencia == model.origen).Select(f => f.cerrado).SingleOrDefault() ?? false;
            }
            else if (model.tipo == (int)DocumentoEstado.Incidencias)
            {
                cerrado = _db.Oportunidades.Where(f => f.empresa == model.empresa && f.referencia == model.origen).Select(f => f.cerrado).SingleOrDefault() ?? false;
            }

            if (cerrado)
                throw new ValidationException("El documneto padre está cerrado");
        }

    }
}
