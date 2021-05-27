using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Terceros;
using Resources;
using RTiposcuentas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Tiposcuentas;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class TiposcuentasValidation : BaseValidation<Tiposcuentas>
    {
        public TiposcuentasValidation(IContextService context, MarfilEntities db) : base(context,db)
        {
        }

        #region Validar grabar

        public override bool ValidarGrabar(Tiposcuentas model)
        {
            if (string.IsNullOrEmpty(model.cuenta))
                throw new ValidationException(General.ErrorCuentaObligatoria);

            if(model.categoria==(int)CategoriasCuentas.Extracontables && !Regex.IsMatch(model.cuenta,"^[A-Z]"))
                throw new ValidationException(RTiposcuentas.ErrorInicioCuentaExtracontable);

            ValidarCambioCuenta(model);

            ValidarBucles(model);

            ValidarLineas(model.TiposcuentasLin);

            UpdateModel(model);
            UpdateLines(model.TiposcuentasLin);

            return base.ValidarGrabar(model);
        }

        private void ValidarCambioCuenta(Tiposcuentas model)
        {
            if (ExisteAlgunTipoDeTercero(model))
            {
                
                
                    using (var serviceTipocuenta = FService.Instance.GetService(typeof(TiposCuentasModel), Context))
                    {
                        if (serviceTipocuenta.exists(model.tipos.ToString()))//si estamos actualizando
                        {
                            var tipocuentaobj = serviceTipocuenta.get(model.tipos.ToString()) as TiposCuentasModel;
                            if (model.cuenta != tipocuentaobj.Cuenta)
                            {
                                throw new ValidationException(string.Format(RTiposcuentas.ErrorUpdateTipoCuenta, (TiposCuentas)model.tipos));
                            }
                        }
                    }
                
            }
        }


        private bool ExisteAlgunTipoDeTercero(Tiposcuentas model)
        {
            using (var dbnew = MarfilEntities.ConnectToSqlServer(_db.Database.Connection.Database))
            {
                return dbnew.Cuentas.Any(f => f.tipocuenta == model.tipos);
            }
        }

        

        private void ValidarLineas(ICollection<TiposcuentasLin> tiposcuentasLin)
        {

            foreach (var item in tiposcuentasLin)
            {
                if (tiposcuentasLin.Count(f => f.cuenta == item.cuenta) > 1)
                    throw new ValidationException(string.Format(RTiposcuentas.ErrorCuentaDuplicada, item.cuenta));
            }
        }

        private void ValidarBucles(Tiposcuentas model)
        {

            foreach (var item in model.TiposcuentasLin)
            {
                if (
                    _db.Tiposcuentas.Any(
                        f => f.empresa == item.empresa && f.tipos == item.fkTiposcuentas && f.cuenta == item.cuenta))
                    throw new ValidationException(string.Format(RTiposcuentas.ErrorCuentaLoop, model.cuenta));

                var list =
             _db.Tiposcuentas.Where(
                 f => item.cuenta == f.cuenta && f.tipos != item.fkTiposcuentas && f.empresa == item.empresa);
                foreach (var item2 in list)
                {
                    ValidarBuclesLineas(item2, model);
                }
            }



        }

        private void ValidarBuclesLineas(Tiposcuentas linea, Tiposcuentas actual)
        {
            foreach (var item in linea.TiposcuentasLin)
            {
                if (item.cuenta == actual.cuenta)
                    throw new ValidationException(string.Format(RTiposcuentas.ErrorCuentaLoopEncontrado, item.cuenta));

                var list =
             _db.Tiposcuentas.Where(
                 f => item.cuenta == f.cuenta && f.tipos != item.fkTiposcuentas && f.empresa == item.empresa);
                foreach (var item2 in list)
                {
                    ValidarBuclesLineas(item2, actual);
                }
            }
        }

        private void UpdateModel(Tiposcuentas model)
        {
            if (string.IsNullOrEmpty(model.descripcion))
            {
                model.descripcion = _db.Cuentas.Any(f => f.empresa == model.empresa && f.id == model.cuenta) ? _db.Cuentas.Single(f => f.empresa == model.empresa && f.id == model.cuenta).descripcion : string.Empty;

            }

        }

        private void UpdateLines(ICollection<TiposcuentasLin> tiposcuentasLin)
        {
            foreach (var item in tiposcuentasLin)
            {
                if (string.IsNullOrEmpty(item.descripcion))
                {
                    item.descripcion = _db.Cuentas.Any(f => f.empresa == item.empresa && f.id == item.cuenta) ? _db.Cuentas.Single(f => f.empresa == item.empresa && f.id == item.cuenta).descripcion : string.Empty;
                }
            }
        }

        #endregion

        #region Validar eliminar

        public override bool ValidarBorrar(Tiposcuentas model)
        {
            if (ExisteAlgunTipoDeTercero(model))
                throw new ValidationException(RTiposcuentas.ErrorBorrar);

            return base.ValidarBorrar(model);
        }

        #endregion
    }
}
