using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Converter;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IComercialesService
    {

    }

    public class ComercialesService : GestionService<ComercialesModel, Comerciales>, IComercialesService
    {
        #region CTR

        public ComercialesService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        #region Create

        public override void create(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as ComercialesModel;

                base.create(obj);

                //guardar direcciones
                GuardarCuentas(model);
                GuardarDirecciones(model);
                GuardarBancos(model);
                tran.Complete();
            }

        }

        #endregion

        #region Edit

        public override void edit(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as ComercialesModel;

                base.edit(obj);

                //guardar direcciones
                GuardarCuentas(model);
                GuardarDirecciones(model);
                GuardarBancos(model);
                tran.Complete();
            }
        }

        #endregion

        #region Delete

        public override void delete(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as ComercialesModel;

                base.delete(obj);

                DeleteCuentas(model);
                DeleteDirecciones(model);
                DeleteBancos(model);
                tran.Complete();
            }
        }


        #endregion

        #region List Index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = CuentasService.GetTercerosIndexModel<ComercialesModel>(_context, controller, canEliminar, canModificar);
            model.List = GetAll<CuentasBusqueda>();
            return model;
        }

        public override string GetSelectPrincipal()
        {
            return string.Format(CuentasService.SelectCuentasTerceros, (int)TiposCuentas.Comerciales, Empresa, TiposCuentas.Comerciales);
        }

        #endregion

        #region Tipo cuentas

        public struct StExiste
        {
            public bool Existe { get; set; }
            public bool Valido { get; set; }
        }
        public StExiste GetCompañia(TiposCuentas tipo, string id)
        {
            var result=new StExiste();
            var tiposcuentasService = FService.Instance.GetService(typeof (TiposCuentasModel), _context) as TiposcuentasService;
            var subcuenta = tiposcuentasService.GetMascaraFromTipoCuenta(tipo);
            result.Valido = id.Contains(subcuenta);
            result.Existe = false;
            if (result.Valido)
            {
                result.Existe = exists(id);
            }
            return result;
        }

        #endregion

        #region Helper cuetnas

        private void DeleteCuentas(ComercialesModel model)
        {
            var fservice = FService.Instance;
            var cuentasService = fservice.GetService(typeof(CuentasModel), _context, _db) as CuentasService;
            cuentasService.FlagDeleteFromThird = true;
            cuentasService.delete(model.Cuentas);
        }

        private void GuardarCuentas(ComercialesModel model)
        {
            model.Cuentas.Tiposcuentas = (int)TiposCuentas.Comerciales;
            var fservice = FService.Instance;
           
            var cuentasService = fservice.GetService(typeof (CuentasModel), _context, _db) as CuentasService;
            if (cuentasService.exists(model.Cuentas.Id))
            {
                var fserviceOriginal = FService.Instance;
                var cuentasServiceOriginal = fserviceOriginal.GetService(typeof(CuentasModel), _context);
                var dbmodel = cuentasServiceOriginal.get(model.Fkcuentas) as CuentasModel;
                dbmodel.Descripcion = model.Cuentas.Descripcion;
                dbmodel.Descripcion2 = model.Cuentas.Descripcion2;
                dbmodel.FkPais = model.Cuentas.FkPais;
                dbmodel.Nif = model.Cuentas.Nif;

                cuentasService.edit(dbmodel);
            }
            else
            {
                cuentasService.create(model.Cuentas);
            }
           
        }

        #endregion

        #region Helper Direcciones

        private void ProcessDirecciones(ComercialesModel model)
        {
            foreach (var item in model.Direcciones.Direcciones)
            {
                item.Empresa = model.Empresa;
                item.Fkentidad = model.Fkcuentas;
                item.Tipotercero = (int)TiposCuentas.Comerciales;
                if (item.Id < 0)
                {
                    item.Id = GetNextId(model);
                }
            }
        }

        private int GetNextId(ComercialesModel model)
        {
            var result = 1;

            if (model.Direcciones != null && model.Direcciones.Direcciones.Any())
            {
                result = model.Direcciones.Direcciones.Max(f => f.Id) + 1;
            }

            return result;
        }

        private void GuardarDirecciones(ComercialesModel model)
        {
            if (model.Direcciones == null) return;
            var fservice = FService.Instance;
            var direccionesService = fservice.GetService(typeof(DireccionesLinModel), _context, _db) as DireccionesService;
            ProcessDirecciones(model);

            direccionesService.CleanAllDirecciones(Empresa, TiposCuentas.Comerciales, model.Fkcuentas);
            foreach (var item in model.Direcciones.Direcciones)
            {
                direccionesService.create(item);
            }
        }

        private void DeleteDirecciones(ComercialesModel model)
        {
            if (model.Direcciones == null) return;
            var fservice = FService.Instance;
            var direccionesService = fservice.GetService(typeof(DireccionesLinModel), _context, _db);
            //ProcessDirecciones(model);
            foreach (var item in model.Direcciones.Direcciones)
            {
                direccionesService.delete(item);
            }
        }

        #endregion

        #region Helper Bancos

        private void ProcessBancos(ComercialesModel model)
        {
            foreach (var item in model.BancosMandatos.BancosMandatos)
            {
                item.Empresa = model.Empresa;
                item.Fkcuentas = model.Fkcuentas;
                if (Funciones.Qint(item.Id) < 0)
                {
                    item.Id = GetNextBancosId(model);
                }
            }
        }

        private string GetNextBancosId(ComercialesModel model)
        {
            var result = Funciones.RellenaCod("1", 3);

            if (model.BancosMandatos != null && model.BancosMandatos.BancosMandatos.Any())
            {
                var max = model.BancosMandatos.BancosMandatos.Max(f => f.Idnumerico) > 0
                    ? model.BancosMandatos.BancosMandatos.Max(f => f.Idnumerico)
                    : 1;
                result = Funciones.GetNextCode(max.ToString(), 3);
            }

            return result;
        }

        private void GuardarBancos(ComercialesModel model)
        {
            if (model.BancosMandatos == null) return;
            var fservice = FService.Instance;
            var bancosService = fservice.GetService(typeof(BancosMandatosLinModel), _context, _db) as BancosMandatosService;
            ProcessBancos(model);
            bancosService.CleanAllBancosMandatos(model.Fkcuentas);
            foreach (var item in model.BancosMandatos.BancosMandatos)
            {
                bancosService.create(item);
            }
        }


        private void DeleteBancos(ComercialesModel model)
        {
            if (model.BancosMandatos == null) return;
            var fservice = FService.Instance;
            var bancosmandatosService = fservice.GetService(typeof(BancosMandatosLinModel), _context, _db);
            foreach (var item in model.BancosMandatos.BancosMandatos)
            {
                bancosmandatosService.delete(item);
            }
        }

        #endregion
    }
}
