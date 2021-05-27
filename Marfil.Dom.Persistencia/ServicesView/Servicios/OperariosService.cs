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
    public interface IOperariosService
    {

    }

    public class OperariosService : GestionService<OperariosModel, Operarios>, IOperariosService
    {
        #region CTR

        public OperariosService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        #region Create

        public override void create(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as OperariosModel;

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
                var model = obj as OperariosModel;

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
                var model = obj as OperariosModel;

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
           

            var instance = new OperariosBusqueda();
            
            var ctor = t.GetConstructor(new[] { typeof(IContextService) });
            var agentesObj = ctor.Invoke(new object[] { _context }) as ICanDisplayName;
            var model = new ListIndexModel()
            {
                Entidad = agentesObj.DisplayName,

                PrimaryColumnns = new[] { "Fkcuentas" },
                VarSessionName = "__" + t.Name,
                Properties = instance.getProperties(),
                Controller = controller,
                PermiteEliminar = canEliminar,
                PermiteModificar = canModificar,
                ExcludedColumns = new[] { "Toolbar" }
            };

            var propiedadesVisibles = new[] { "Fkcuentas", "Descripcion", "Nif", "Numeroseguridadsocial", "Poblacion" };
            var propiedades = Helpers.Helper.getProperties<OperariosBusqueda>();
            model.PrimaryColumnns = new[] { "Fkcuentas" };
            model.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            model.BloqueoColumn = "Bloqueado";
            model.List = GetAll<OperariosBusqueda>();

            return model;

            return model;
        }

        public override string GetSelectPrincipal()
        {
            return string.Format("select c.id as [Fkcuentas],c.descripcion as [Descripcion], c.Nif as [Nif], cli.numeroseguridadsocial,d.poblacion as [Poblacion], c.bloqueada as [Bloqueado] from operarios as cli " +
            " inner join cuentas as c on c.id=cli.fkcuentas and c.empresa=cli.empresa " +
             " left join direcciones as d on d.empresa= c.empresa and d.tipotercero={1} and d.fkentidad=c.id and d.defecto=1 " +
            " where cli.empresa='{0}' ", Empresa,(int)TiposCuentas.Operarios);

        }

        #endregion

        #region Tipo cuentas

        public struct StExiste
        {
            public bool Existe { get; set; }
            public bool Valido { get; set; }
        } 

        public OperariosService.StExiste GetCompañia(TiposCuentas tipo, string id)
        {
            var result = new OperariosService.StExiste();

            var tiposcuentasService =
                FService.Instance.GetService(typeof(TiposCuentasModel), _context) as TiposcuentasService;
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

        private void DeleteCuentas(OperariosModel model)
        {
            var fservice = FService.Instance;
            var cuentasService = fservice.GetService(typeof(CuentasModel), _context, _db) as CuentasService;
            cuentasService.FlagDeleteFromThird = true;
            cuentasService.delete(model.Cuentas);
        }

        private void GuardarCuentas(OperariosModel model)
        {
            model.Cuentas.Tiposcuentas = (int)TiposCuentas.Operarios;
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

        #region Helper Direcciones

        private void ProcessDirecciones(OperariosModel model)
        {
            foreach (var item in model.Direcciones.Direcciones)
            {
                item.Empresa = model.Empresa;
                item.Fkentidad = model.Fkcuentas;
                item.Tipotercero = (int)TiposCuentas.Operarios;
                if (item.Id < 0)
                {
                    item.Id = GetNextId(model);
                }
            }
        }

        private int GetNextId(OperariosModel model)
        {
            var result = 1;

            if (model.Direcciones != null && model.Direcciones.Direcciones.Any())
            {
                result = model.Direcciones.Direcciones.Max(f => f.Id) + 1;
            }

            return result;
        }

        private void GuardarDirecciones(OperariosModel model)
        {
            if (model.Direcciones == null) return;
            var fservice = FService.Instance;
            var direccionesService = fservice.GetService(typeof(DireccionesLinModel), _context, _db) as DireccionesService;
            ProcessDirecciones(model);

            direccionesService.CleanAllDirecciones(Empresa, TiposCuentas.Operarios, model.Fkcuentas);
            foreach (var item in model.Direcciones.Direcciones)
            {
                direccionesService.create(item);
            }
        }

        private void DeleteDirecciones(OperariosModel model)
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

        private void ProcessBancos(OperariosModel model)
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

        private string GetNextBancosId(OperariosModel model)
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

        private void GuardarBancos(OperariosModel model)
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


        private void DeleteBancos(OperariosModel model)
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
