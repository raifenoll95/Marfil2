using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.Empresa;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using RFamilias = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Familiasproductos;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IFamiliasproductosService
    {

    }

    public class FamiliasproductosService : GestionService<FamiliasproductosModel, Familiasproductos>, IFamiliasproductosService
    {
        #region CTR

        public FamiliasproductosService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion


        public override void create(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as FamiliasproductosModel;
                DocumentosHelpers.GenerarCarpetaAsociada(model, TipoDocumentos.Familias, _context, _db);
                base.create(obj);
                tran.Complete();
            }
        }

        public override void edit(IModelView obj)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                var editado = obj as FamiliasproductosModel;
                base.edit(editado);
                _db.SaveChanges();
                tran.Complete();
            }
        }

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var propiedadesVisibles = new[] { "Id", "Descripcion", "Tipofamilia", "Gestionstock", "Articulonegocio" };
            var propiedades = Helpers.Helper.getProperties<FamiliasproductosModel>();
            model.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();

            return model;
        }

        public override string GetSelectPrincipal()
        {
            return string.Format("select id,descripcion,isnull(tipofamilia,0) as Tipofamilia,gestionstock,articulonegocio from familiasproductos where empresa='{0}'", Empresa);
        }

        public void ValidarDimensiones(string codfamilia, double? largo, double? ancho, double? grueso, string art = "")
        {
            var error = string.Empty;
            var familia = _db.Familiasproductos.Where(f => f.empresa == Empresa && f.id == codfamilia).SingleOrDefault();

            if (familia.validardimensiones == true)
            {
                if ((familia.minlargo > largo) || (largo > familia.maxlargo))
                    error = string.Format(RFamilias.ErrorLargo, art, familia.minlargo, familia.maxlargo);
                if ((familia.minancho > ancho) || (ancho > familia.maxancho))
                {
                    error += string.IsNullOrWhiteSpace(error) ? "" : Environment.NewLine;
                    error += string.Format(RFamilias.ErrorAncho, art, familia.minancho, familia.maxancho);
                }
                if ((familia.mingrueso > grueso) || (grueso > familia.maxgrueso))
                {
                    error += string.IsNullOrWhiteSpace(error) ? "" : Environment.NewLine;
                    error += string.Format(RFamilias.ErrorGrueso, art, familia.mingrueso, familia.maxgrueso);
                }
                if (!string.IsNullOrWhiteSpace(error))
                    throw new ValidationException(error);
            }
        }

        public void ValidarDimensiones(string articulo, double? largo, double? ancho, double? grueso)
        {
            var familiacodigo = ArticulosService.GetCodigoFamilia(articulo);
            ValidarDimensiones(familiacodigo, largo, ancho, grueso, articulo);
        }

        internal IEnumerable<FamiliasproductosModel> GetListFamiliaArticulos()
        {
            return _db.Familiasproductos.Where(f => f.empresa == Empresa).ToList().Select(f => _converterModel.GetModelView(f) as FamiliasproductosModel).ToList();
        }
    }
}
