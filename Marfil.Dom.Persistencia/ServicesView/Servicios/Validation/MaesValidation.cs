using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Documentos.Albaranes;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RAlbaranes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Albaranes;
using RAlbaranesCompras = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.AlbaranesCompras;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
   
    internal class MaesValidation : BaseValidation<Maes>
    {
        public string EjercicioId { get; set; }

        public MaesValidation(IContextService context, MarfilEntities db) : base(context, db)
        {

        }

        #region Validar grabar

        //public override bool ValidarGrabar(Maes model)
        //{
        //    //Todo EL: verificar los albaranes de compra si se pueden modificar si está en alguna factura de compra
        //    //if (CambiarEstado)
        //    //    if (_db.FacturasLin.Any(f => f.fkalbaranesreferencia == model.referencia && f.empresa == model.empresa))
        //    //    throw new ValidationException(RAlbaranesCompras.ErrorAlbaranFacturado);

        //    if (!CambiarEstado)
        //    {
        //        if (!ModificarCostes)
        //            ValidarEstado(model);
        //        ValidarCabecera(model);
        //        ValidarLineas(model);
        //        CalcularTotales(model);
        //        CalcularTotalesCabecera(model);
        //        CalcularCostesadicionales(model);
        //    }
            
        //    return base.ValidarGrabar(model);
        //}

       

 

        #endregion

        #region Eliminar

        //public override bool ValidarBorrar(AlbaranesCompras model)
        //{
        //    if (_db.FacturasLin.Any(f => f.fkalbaranesreferencia == model.referencia && f.empresa == model.empresa))
        //        throw new ValidationException(RAlbaranesCompras.ErrorAlbaranFacturado);

        //    return base.ValidarBorrar(model);
        //}

        #endregion

       
    }
}
