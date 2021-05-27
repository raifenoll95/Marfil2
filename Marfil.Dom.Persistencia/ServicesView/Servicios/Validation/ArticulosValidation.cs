using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Resources;
using RArticulos= Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Articulos;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class ArticulosValidation : BaseValidation<Articulos>
    {
        public ArticulosValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override bool ValidarGrabar(Articulos model)
        {
            if(string.IsNullOrEmpty(model.fkgruposiva))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RArticulos.Fkgruposiva));

            if(model.piezascaja.HasValue && (model.piezascaja<0 || model.piezascaja>9999))
                throw new ValidationException(string.Format(General.ErrorRangoEntre, RArticulos.Piezascaja,"0","9999"));

            var tipo = (Tipogestionlotes) (model.tipogestionlotes ?? 0);
            if (tipo != Tipogestionlotes.Singestion)
                model.stocknegativoautorizado = false;

            // Validar dimensiones
            //var familiasProductosService = FService.Instance.GetService(typeof(FamiliasproductosModel), Context, _db) as FamiliasproductosService;
            //var familiacodigo = ArticulosService.GetCodigoFamilia(model.id);
            //familiasProductosService.ValidarDimensiones(familiacodigo, model.largo, model.ancho, model.grueso, "");

            VerificarTipoFamilia(model);

            return base.ValidarGrabar(model);
        }

        private void VerificarTipoFamilia(Articulos model)
        {

            if (model.id.Length != 11)
                throw new ValidationException("La longitud del código del artículo debe ser de 11");

            var familia = ArticulosService.GetCodigoFamilia(model.id);
            var familiaobj = _db.Familiasproductos.Single(f => f.empresa == model.empresa && f.id == familia);

            if ((TipoFamilia)(familiaobj.tipofamilia ?? 0 ) != TipoFamilia.General && (TipoFamilia)(familiaobj.tipofamilia ?? 0) != TipoFamilia.Libre)
            {
                model.tipogestionlotes = (int)Tipogestionlotes.Loteobligatorio;
                model.gestionstock = true;
            }

            if (familiaobj.validarmaterial??false)
            {
                var codmaterial = ArticulosService.GetCodigoMaterial(model.id);
                if(!_db.Materiales.Any(f=>f.empresa==model.empresa && f.id==codmaterial))
                    throw new ValidationException("No existe el material del articulo");
            }

            if (familiaobj.validarcaracteristica ?? false)
            {
                var codcaracteristica = ArticulosService.GetCodigoCaracteristica(model.id);
                if (!_db.CaracteristicasLin.Any(f => f.empresa == model.empresa && f.fkcaracteristicas== familia && f.id == codcaracteristica))
                    throw new ValidationException("No existe la caracteristica del articulo");
            }

            if (familiaobj.validargrosor ?? false)
            {
                var codgrosor = ArticulosService.GetCodigoGrosor(model.id);
                if (!_db.Grosores.Any(f => f.empresa == model.empresa && f.id == codgrosor))
                    throw new ValidationException("No existe la grosor del articulo");
            }

            if (familiaobj.validaracabado ?? false)
            {
                var codacabado = ArticulosService.GetCodigoAcabado(model.id);
                if (!_db.Acabados.Any(f => f.empresa == model.empresa && f.id == codacabado))
                    throw new ValidationException("No existe la acabado del articulo");
            }
        }

        public override bool ValidarBorrar(Articulos model)
        {
            if(ExisteArticuloEnAlgunDocumentos(model))
                throw new ValidationException(General.ErrorBorradoArticulosUsados);
            return base.ValidarBorrar(model);
        }

        private bool ExisteArticuloEnAlgunDocumentos(Articulos model)
        {
            

            return _db.PresupuestosLin.Any(f => f.fkarticulos == model.id) ||
                     _db.PedidosLin.Any(f => f.fkarticulos == model.id);


            
        }
    }
}
