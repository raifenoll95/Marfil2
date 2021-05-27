using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RKit = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Kit;
using RBundle = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Bundle;
using RAlbaran = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Albaranes;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class BundleValidation : BaseValidation<Bundle>
    {
        public BundleValidation(IContextService context, MarfilEntities db) : base(context,db)
        {
        }

        public override bool ValidarGrabar(Bundle model)
        {
            if (string.IsNullOrEmpty(model.lote))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RAlbaran.Lote));
            if (string.IsNullOrEmpty(model.id))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RKit.Id));

            if (string.IsNullOrEmpty(model.fkalmacen))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RKit.Fkalmacen));

            if (string.IsNullOrEmpty(model.descripcion))
            {
                CalcularDescripcion(model);
            }

            ValidarLineas(model);

            return base.ValidarGrabar(model);
        }

        private void CalcularDescripcion(Bundle model)
        {
            if (model.BundleLin.Any() && model.BundleLin.GroupBy(f => f.fkarticulos).Count() == 1)
            {
                model.descripcion = model.BundleLin.First().descripcion;
            }
        }

        private void ValidarLineas(Bundle model)
        {
            if(model.BundleLin.GroupBy(f=>f.lote).Count()>1)
                throw new ValidationException(RBundle.ErrorBundleLoteUnico);
            model.pesoneto = 0;
            foreach (var item in model.BundleLin)
            {
                if (item.lote != model.lote)
                {
                    throw new ValidationException(string.Format("El lote {0} de no coincide con el lote del bundle: {1}",item.fkbundlelote,model.lote));
                }

                if (
                    _db.KitLin.Any(
                        f =>
                            f.empresa == model.empresa && f.lote == item.lote && f.loteid == item.loteid &&
                            f.fkarticulos == item.fkarticulos && f.fkalmacenes == item.fkalmacenes))
                {
                    throw new ValidationException(string.Format(RBundle.ErrorKitLoteUtilizado, item.lote + Funciones.RellenaCod(item.loteid,3)));
                }

                if (
                   _db.BundleLin.Any(
                       f =>
                           f.empresa == model.empresa && f.lote == item.lote && f.loteid == item.loteid &&
                           f.fkarticulos == item.fkarticulos && f.fkalmacenes == item.fkalmacenes && f.fkbundle!=model.id))
                {
                    throw new ValidationException(string.Format(RBundle.ErrorLoteUtilizado, item.lote + Funciones.RellenaCod(item.loteid, 3)));
                }


                var stock =
                    _db.Stockactual.SingleOrDefault(
                        f =>
                            f.empresa == model.empresa && f.lote == item.lote && f.loteid == item.loteid &&
                            f.fkalmacenes == model.fkalmacen && f.fkarticulos == item.fkarticulos &&
                            f.fkalmaceneszona != model.fkzonaalmacen);
                if (stock != null)
                {
                    model.pesoneto += stock.pesonetolote ?? 0;
                    stock.fkalmaceneszona = model.fkzonaalmacen;
                    _db.Stockactual.AddOrUpdate(stock);
                }

                var historicostock =
                    _db.Stockhistorico.SingleOrDefault(
                        f =>
                            f.empresa == model.empresa && f.lote == item.lote && f.loteid == item.loteid &&
                            f.fkalmacenes == model.fkalmacen && f.fkarticulos == item.fkarticulos &&
                            f.fkalmaceneszona != model.fkzonaalmacen);
                if (historicostock != null)
                {
                   
                    historicostock.fkalmaceneszona = model.fkzonaalmacen;
                    //_db.Stockhistorico.AddOrUpdate(historicostock);
                }
            }
        }
    }
}
