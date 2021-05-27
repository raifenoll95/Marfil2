using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RKit = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Kit;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class KitValidation : BaseValidation<Kit>
    {
        public KitValidation(IContextService context, MarfilEntities db) : base(context,db)
        {
        }

        public override bool ValidarGrabar(Kit model)
        {

            if (model.fechadocumento == null)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RKit.Fecha));

            if (string.IsNullOrEmpty(model.descripcion))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RKit.Descripcion));

            if (string.IsNullOrEmpty(model.fkalmacen))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RKit.Fkalmacen));

            ValidarLineas(model);

            return base.ValidarGrabar(model);
        }

        private void ValidarLineas(Kit model)
        {
            model.pesoneto = 0;
            foreach (var item in model.KitLin)
            {
                ValidarUsoKit(item);

                if (
                    _db.KitLin.Any(
                        f =>
                            f.empresa == model.empresa && f.lote == item.lote && f.loteid == item.loteid &&
                            f.fkarticulos == item.fkarticulos && f.fkalmacenes == item.fkalmacenes &&
                            f.fkkit != model.id))
                {
                    throw new ValidationException(string.Format(RKit.ErrorLoteUtilizado,item.lote + Funciones.RellenaCod(item.loteid,3)));
                }

                if (
                   _db.BundleLin.Any(
                       f =>
                           f.empresa == model.empresa && f.lote == item.lote && f.loteid == item.loteid &&
                           f.fkarticulos == item.fkarticulos && f.fkalmacenes == item.fkalmacenes))
                {
                    throw new ValidationException(string.Format(RKit.ErrorLoteUtilizado, item.lote + Funciones.RellenaCod(item.loteid, 3)));
                }

                var stock =
                    _db.Stockactual.SingleOrDefault(
                        f =>
                            f.empresa == model.empresa && f.lote == item.lote && f.loteid == item.loteid &&
                            f.fkalmacenes == model.fkalmacen && f.fkarticulos == item.fkarticulos &&
                            f.fkalmaceneszona != model.fkzonalamacen);
                if (stock != null)
                {
                    model.pesoneto += stock.pesonetolote ?? 0;
                    stock.fkalmaceneszona = model.fkzonalamacen;
                    _db.Stockactual.AddOrUpdate(stock);
                }

                var historicostock =
                    _db.Stockhistorico.SingleOrDefault(
                        f =>
                            f.empresa == model.empresa && f.lote == item.lote && f.loteid == item.loteid &&
                            f.fkalmacenes == model.fkalmacen && f.fkarticulos == item.fkarticulos &&
                            f.fkalmaceneszona != model.fkzonalamacen);
                if (historicostock != null)
                {
                    historicostock.fkalmaceneszona = model.fkzonalamacen;
                    //_db.Stockhistorico.AddOrUpdate(historicostock);
                }
            }
        }

        private void ValidarUsoKit(KitLin linea)
        {
            if (
                _db.AlbaranesLin.Any(
                    f => f.empresa == linea.empresa && f.lote == linea.lote && f.tabla.ToString() == linea.loteid))
            {
                var albobj = _db.Albaranes.Include("AlbaranesLin").First(
                    f => f.empresa == linea.empresa && f.AlbaranesLin.Any(j=>j.lote == linea.lote && j.tabla.ToString() == linea.loteid));
                throw new ValidationException(string.Format("No se puede actualizar el Kit porque el lote {0}{1} ha sido utilizado en el albarán {2}",linea.lote,Funciones.RellenaCod(linea.loteid,3), albobj.referencia));
            }
        }
    }
}
