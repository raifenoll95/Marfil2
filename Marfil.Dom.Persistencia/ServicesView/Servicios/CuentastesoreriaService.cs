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
using Marfil.Inf.Genericos.Helper;
using Marfil.Dom.Persistencia.Model.Documentos.CobrosYPagos;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface ICuentastesoreriaService
    {

    }

    public class CuentastesoreriaService : GestionService<CuentastesoreriaModel, Cuentastesoreria>, ICuentastesoreriaService
    {
        #region CTR

        public CuentastesoreriaService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        #region Create

        public override void create(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as CuentastesoreriaModel;

                base.create(obj);

                //guardar direcciones
                GuardarCuentas(model);
                GuardarDirecciones(model);
                GuardarContactos(model);
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
                var model = obj as CuentastesoreriaModel;

                base.edit(obj);

                //guardar direcciones
                GuardarCuentas(model);
                GuardarDirecciones(model);
                GuardarContactos(model);
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
                var model = obj as CuentastesoreriaModel;

                base.delete(obj);

                DeleteCuentas(model);
                DeleteDirecciones(model);
                DeleteContactos(model);
                DeleteBancos(model);
                tran.Complete();
            }
        }


        #endregion

        #region List Index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = CuentasService.GetTercerosIndexModel<CuentastesoreriaModel>(_context, controller, canEliminar, canModificar);
            model.List = GetAll<CuentasBusqueda>();
            return model;
        }

        public override string GetSelectPrincipal()
        {
            return string.Format(CuentasService.SelectCuentasTerceros, (int)TiposCuentas.Cuentastesoreria, Empresa, TiposCuentas.Cuentastesoreria);
        }

        #endregion

        #region Tipo cuentas

        public struct StExiste
        {
            public bool Existe { get; set; }
            public bool Valido { get; set; }
        }
        public CuentastesoreriaService.StExiste GetCompañia(TiposCuentas tipo, string id)
        {
            var result = new CuentastesoreriaService.StExiste();
            var tiposcuentasService =
                FService.Instance.GetService(typeof (TiposCuentasModel), _context) as TiposcuentasService;
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

        private void DeleteCuentas(CuentastesoreriaModel model)
        {
            var fservice = FService.Instance;
            var cuentasService = fservice.GetService(typeof(CuentasModel), _context, _db) as CuentasService;
            cuentasService.FlagDeleteFromThird = true;
            cuentasService.delete(model.Cuentas);
        }

        private void GuardarCuentas(CuentastesoreriaModel model)
        {
            model.Cuentas.Tiposcuentas = (int)TiposCuentas.Cuentastesoreria;
            var fservice = FService.Instance;

            var cuentasService = fservice.GetService(typeof(CuentasModel), _context, _db) as CuentasService;
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

        #region Helper contactos

        private void ProcessContactos(CuentastesoreriaModel model)
        {
            foreach (var item in model.Contactos.Contactos)
            {
                item.Empresa = model.Empresa;
                item.Fkentidad = model.Fkcuentas;
                item.Tipotercero = TiposCuentas.Cuentastesoreria;
                if (item.Id < 0)
                {
                    item.Id = GetNextContactoId(model);
                }
            }
        }

        private int GetNextContactoId(CuentastesoreriaModel model)
        {
            var result = 1;

            if (model.Contactos != null && model.Contactos.Contactos.Any())
            {
                result = model.Contactos.Contactos.Max(f => f.Id) + 1;
            }

            return result;
        }

        private void GuardarContactos(CuentastesoreriaModel model)
        {
            if (model.Contactos == null) return;
            var fservice = FService.Instance;
            var contactosService = fservice.GetService(typeof(ContactosLinModel), _context, _db) as ContactosService;
            ProcessContactos(model);

            contactosService.CleanAllContactos(TiposCuentas.Cuentastesoreria, model.Fkcuentas);
            foreach (var item in model.Contactos.Contactos)
            {
                contactosService.create(item);
            }
        }

        private void DeleteContactos(CuentastesoreriaModel model)
        {
            if (model.Contactos == null) return;
            var fservice = FService.Instance;
            var contactosService = fservice.GetService(typeof(ContactosLinModel), _context, _db);
            ProcessContactos(model);
            foreach (var item in model.Contactos.Contactos)
            {
                contactosService.delete(item);
            }
        }

        #endregion

        #region Helper Direcciones

        private void ProcessDirecciones(CuentastesoreriaModel model)
        {
            foreach (var item in model.Direcciones.Direcciones)
            {
                item.Empresa = model.Empresa;
                item.Fkentidad = model.Fkcuentas;
                item.Tipotercero = (int)TiposCuentas.Cuentastesoreria;
                if (item.Id < 0)
                {
                    item.Id = GetNextId(model);
                }
            }
        }

        private int GetNextId(CuentastesoreriaModel model)
        {
            var result = 1;

            if (model.Direcciones != null && model.Direcciones.Direcciones.Any())
            {
                result = model.Direcciones.Direcciones.Max(f => f.Id) + 1;
            }

            return result;
        }

        private void GuardarDirecciones(CuentastesoreriaModel model)
        {
            if (model.Direcciones == null) return;
            var fservice = FService.Instance;
            var direccionesService = fservice.GetService(typeof(DireccionesLinModel), _context, _db) as DireccionesService;
            ProcessDirecciones(model);

            direccionesService.CleanAllDirecciones(Empresa, TiposCuentas.Cuentastesoreria, model.Fkcuentas);
            foreach (var item in model.Direcciones.Direcciones)
            {
                direccionesService.create(item);
            }
        }

        private void DeleteDirecciones(CuentastesoreriaModel model)
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

        private void ProcessBancos(CuentastesoreriaModel model)
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

        private string GetNextBancosId(CuentastesoreriaModel model)
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

        private void GuardarBancos(CuentastesoreriaModel model)
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


        private void DeleteBancos(CuentastesoreriaModel model)
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

        public IEnumerable<CuentasModel> getCuentasTesoreriaExclusive(string tipoasignacion, string fkcuentas, string importe)
        {
            List<CuentasModel> cuentas = new List<CuentasModel>();

            var vencimientosService = new VencimientosService(_context);
            List<VencimientosModel> vencimientos = new List<VencimientosModel>();
            vencimientos.AddRange(vencimientosService.getVencimientos(tipoasignacion, "", fkcuentas, importe));

            var cuentasService = new CuentasService(_context);

            foreach (var vencimiento in vencimientos)
            {
                cuentas.Add(cuentasService.get(vencimiento.Fkcuentatesoreria) as CuentasModel);
            }

            return cuentas;
        }

        public IEnumerable<CuentasModel> getCuentasTesoreria()
        {
            List<CuentasModel> cuentas = new List<CuentasModel>();
            var cuentastesoreria = _db.Cuentastesoreria.Where(f => f.empresa == Empresa);
            var cuentascaja = _db.Empresas.Where(f => f.id == Empresa).Select(f => f.cuentascaja).FirstOrDefault();
            var listcuentascaja = _db.Cuentas.Where(f => f.empresa == Empresa && f.nivel == 0 && f.id.StartsWith(cuentascaja)).ToList();

            var cuentasService = new CuentasService(_context);

            foreach (var cuenta in cuentastesoreria)
            {
                var cuentaModel = cuentasService.get(cuenta.fkcuentas) as CuentasModel;

                if(!cuentaModel.Bloqueado)
                {
                    cuentas.Add(cuentaModel);
                }             
            }

            foreach (var cuentacaja in listcuentascaja)
            {
                var cuentaModel = cuentasService.get(cuentacaja.id) as CuentasModel;

                if (!cuentaModel.Bloqueado)
                {
                    cuentas.Add(cuentaModel);
                }
            }

            return cuentas;
        }
    }
}
