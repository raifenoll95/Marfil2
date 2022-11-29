using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Resources;
using REstados= Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Estados;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class EstadosValidation : BaseValidation<Estados>
    {
        public EstadosValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override bool ValidarGrabar(Estados model)
        {

            if(!model.tipoestado.HasValue)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, REstados.Tipoestado));

            if (string.IsNullOrEmpty(model.descripcion))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, REstados.Descripcion));

            if (ExisteEnAlgunDocumentos(model))
                throw new ValidationException(General.ErrorBorradoEstadosUsados);

            return base.ValidarGrabar(model);
        }

        public override bool ValidarBorrar(Estados model)
        {
            if (ExisteEnAlgunDocumentos(model))
                throw new ValidationException(General.ErrorBorradoEstadosUsados);

            return base.ValidarBorrar(model);
        }

        private bool ExisteEnAlgunDocumentos(Estados model)
        {


            return _db.Presupuestos.Any(f => f.fkestados == (model.documento + "-" + model.id)) ||
                   _db.Pedidos.Any(f => f.fkestados == (model.documento + "-" + model.id)) ||
                   _db.Albaranes.Any(f => f.fkestados == (model.documento + "-" + model.id)) ||
                   _db.Facturas.Any(f => f.fkestados == (model.documento + "-" + model.id)) ||
                   _db.PresupuestosCompras.Any(f => f.fkestados == (model.documento + "-" + model.id)) ||
                   _db.PedidosCompras.Any(f => f.fkestados == (model.documento + "-" + model.id)) ||
                   _db.AlbaranesCompras.Any(f => f.fkestados == (model.documento + "-" + model.id)) ||
                   _db.FacturasCompras.Any(f => f.fkestados == (model.documento + "-" + model.id));



        }
    }
}
