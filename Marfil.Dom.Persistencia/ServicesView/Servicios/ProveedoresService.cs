using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.ControlsUI.BusquedaTerceros;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Converter;
using Marfil.Inf.Genericos.Helper;
using System.Data;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IProveedoresService
    {

    }

    public class ProveedoresService : GestionService<ProveedoresModel, Proveedores>, IMobileTercerosService, IProveedoresService
    {
        #region CTR

        public ProveedoresService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        #region Create

        public override void create(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as ProveedoresModel;

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
                var model = obj as ProveedoresModel;

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
                var model = obj as ProveedoresModel;

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
            var model = CuentasService.GetTercerosIndexModel<ProveedoresModel>(_context, controller, canEliminar, canModificar);
            model.List = GetAll<CuentasBusqueda>();
            return model;
        }

        public override string GetSelectPrincipal()
        {
            return string.Format(CuentasService.SelectCuentasTerceros, (int)TiposCuentas.Proveedores, Empresa, TiposCuentas.Proveedores);
        }

        #endregion

        #region Tipo cuentas

        public struct StExiste
        {
            public bool Existe { get; set; }
            public bool Valido { get; set; }
        }
        public ProveedoresService.StExiste GetCompañia(TiposCuentas tipo, string id)
        {
            var result = new ProveedoresService.StExiste();

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

        private void DeleteCuentas(ProveedoresModel model)
        {
            var fservice = FService.Instance;
            var cuentasService = fservice.GetService(typeof(CuentasModel), _context, _db) as CuentasService;
            cuentasService.FlagDeleteFromThird = true;
            cuentasService.delete(model.Cuentas);
        }

        private void GuardarCuentas(ProveedoresModel model)
        {
            model.Cuentas.Tiposcuentas = (int)TiposCuentas.Proveedores;
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

        private void ProcessContactos(ProveedoresModel model)
        {
            foreach (var item in model.Contactos.Contactos)
            {
                item.Empresa = model.Empresa;
                item.Fkentidad = model.Fkcuentas;
                item.Tipotercero = TiposCuentas.Proveedores;
                if (item.Id < 0)
                {
                    item.Id = GetNextContactoId(model);
                }
            }
        }

        private int GetNextContactoId(ProveedoresModel model)
        {
            var result = 1;

            if (model.Contactos != null && model.Contactos.Contactos.Any())
            {
                result = model.Contactos.Contactos.Max(f => f.Id) + 1;
            }

            return result;
        }

        private void GuardarContactos(ProveedoresModel model)
        {
            if (model.Contactos == null) return;
            var fservice = FService.Instance;
            var contactosService = fservice.GetService(typeof(ContactosLinModel), _context, _db) as ContactosService;
            ProcessContactos(model);

            contactosService.CleanAllContactos(TiposCuentas.Proveedores, model.Fkcuentas);
            foreach (var item in model.Contactos.Contactos)
            {
                contactosService.create(item);
            }
        }

        private void DeleteContactos(ProveedoresModel model)
        {
            if (model.Contactos == null) return;
            var fservice = FService.Instance;
            var contactosService = fservice.GetService(typeof(ContactosLinModel), _context, _db);
            foreach (var item in model.Contactos.Contactos)
            {
                contactosService.delete(item);
            }
        }

        #endregion

        #region Helper Direcciones

        private void ProcessDirecciones(ProveedoresModel model)
        {
            foreach (var item in model.Direcciones.Direcciones)
            {
                item.Empresa = model.Empresa;
                item.Fkentidad = model.Fkcuentas;
                item.Tipotercero = (int)TiposCuentas.Proveedores;
                if (item.Id < 0)
                {
                    item.Id = GetNextId(model);
                }
            }
        }

        private int GetNextId(ProveedoresModel model)
        {
            var result = 1;

            if (model.Direcciones != null && model.Direcciones.Direcciones.Any())
            {
                result = model.Direcciones.Direcciones.Max(f => f.Id) + 1;
            }

            return result;
        }

        private void GuardarDirecciones(ProveedoresModel model)
        {
            if (model.Direcciones == null) return;
            var fservice = FService.Instance;
            var direccionesService = fservice.GetService(typeof(DireccionesLinModel), _context, _db) as DireccionesService;
            ProcessDirecciones(model);

            direccionesService.CleanAllDirecciones(Empresa, TiposCuentas.Proveedores, model.Fkcuentas);
            foreach (var item in model.Direcciones.Direcciones)
            {
                direccionesService.create(item);
            }
        }

        private void DeleteDirecciones(ProveedoresModel model)
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

        private void ProcessBancos(ProveedoresModel model)
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

        private string GetNextBancosId(ProveedoresModel model)
        {
            var result = Funciones.RellenaCod("1", 3);

            if (model.BancosMandatos != null && model.BancosMandatos.BancosMandatos.Any())
            {
                var max = model.BancosMandatos.BancosMandatos.Max(f => f.Idnumerico) > 0
                    ? model.BancosMandatos.BancosMandatos.Max(f => f.Idnumerico)
                    : 1;
                result = Funciones.GetNextCode(max.ToString() , 3);
            }

            return result;
        }

        private void GuardarBancos(ProveedoresModel model)
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


        private void DeleteBancos(ProveedoresModel model)
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

        public IEnumerable<CuentasBusqueda> GetProveedoresAndAcreedores(bool primeracarga = false)
        {
            var result = new List<CuentasBusqueda>();
            var service = FService.Instance.GetService(typeof(CuentasModel), _context, _db) as CuentasService;

            result.AddRange(service.GetCuentas(TiposCuentas.Proveedores).Where(f => primeracarga || !(f.Bloqueado ?? false)));
            result.AddRange(service.GetCuentas(TiposCuentas.Acreedores).Where(f => primeracarga || !(f.Bloqueado ?? false)));

            return result;
        }

        public IProveedorAcreedor GetProveedoresAcreedores(string id)
        {
            var serviceClientes = FService.Instance.GetService(typeof(AcreedoresModel), _context, _db);
            if (serviceClientes.exists(id))
                return serviceClientes.get(id) as IProveedorAcreedor;
            if (exists(id))
                return get(id) as IProveedorAcreedor;

            return null;
        }

        #region Busqueda de terceros

        public IEnumerable<IItemResultadoMovile> BuscarTercero(string cuenta)
        {
            var model = get(cuenta) as ProveedoresModel;

            var result = new List<IItemResultadoMovile>();
            result.Add(new ItemCabeceraResultadoMoviles()
            {
                Valor = Inf.ResourcesGlobalization.Textos.Entidades.Proveedores.TituloEntidad
            });

            result.Add(new ItemResultadoMovile()
            {
                Campo = Inf.ResourcesGlobalization.Textos.Entidades.Proveedores.Fkcuentas,
                Valor = model.Fkcuentas
            });

            result.Add(new ItemResultadoMovile()
            {
                Campo = Inf.ResourcesGlobalization.Textos.Entidades.Proveedores.Descripcion,
                Valor = model.Descripcion
            });

            result.Add(new ItemResultadoMovile()
            {
                Campo = Inf.ResourcesGlobalization.Textos.Entidades.Proveedores.RazonSocial,
                Valor = model.RazonSocial
            });

            result.Add(new ItemResultadoMovile()
            {
                Campo = Inf.ResourcesGlobalization.Textos.Entidades.Proveedores.Pais,
                Valor = model.Pais
            });

            result.Add(new ItemResultadoMovile()
            {
                Campo = Inf.ResourcesGlobalization.Textos.Entidades.Proveedores.Nif,
                Valor = model.Nif
            });

            result.Add(new ItemSeparadorResultadoMoviles());

            //Direcciones
            result.Add(new ItemCabeceraResultadoMoviles()
            {
                Valor = Inf.ResourcesGlobalization.Textos.Entidades.Direcciones.TituloEntidad
            });

            if (model.Direcciones != null)
                foreach (var item in model.Direcciones.Direcciones.OrderBy(f => f.Defecto))
                {
                    result.Add(new ItemResultadoMovile()
                    {
                        Campo = Inf.ResourcesGlobalization.Textos.Entidades.Direcciones.Descripcion,
                        Valor = model.Descripcion
                    });
                    result.Add(new ItemResultadoMovile()
                    {
                        Campo = Inf.ResourcesGlobalization.Textos.Entidades.Direcciones.Direccion,
                        Valor = model.Direccion
                    });
                    result.Add(new ItemResultadoMovile()
                    {
                        Campo = Inf.ResourcesGlobalization.Textos.Entidades.Direcciones.Cp,
                        Valor = model.Cp
                    });
                    result.Add(new ItemResultadoMovile()
                    {
                        Campo = Inf.ResourcesGlobalization.Textos.Entidades.Direcciones.Poblacion,
                        Valor = model.Poblacion
                    });
                }

            result.Add(new ItemSeparadorResultadoMoviles());

            //Contactos
            result.Add(new ItemCabeceraResultadoMoviles()
            {
                Valor = Inf.ResourcesGlobalization.Textos.Entidades.Contactos.TituloEntidad
            });

            if (model.Contactos != null)
                foreach (var item in model.Contactos.Contactos)
                {
                    result.Add(new ItemResultadoMovile()
                    {
                        Campo = Inf.ResourcesGlobalization.Textos.Entidades.Contactos.Nombre,
                        Valor = item.Nombre
                    });
                    result.Add(new ItemResultadoMovile()
                    {
                        Campo = Inf.ResourcesGlobalization.Textos.Entidades.Contactos.Telefono,
                        Valor = item.Telefono
                    });
                    result.Add(new ItemResultadoMovile()
                    {
                        Campo = Inf.ResourcesGlobalization.Textos.Entidades.Contactos.Telefonomovil,
                        Valor = item.Telefonomovil
                    });
                    result.Add(new ItemResultadoMovile()
                    {
                        Campo = Inf.ResourcesGlobalization.Textos.Entidades.Contactos.Email,
                        Valor = item.Email
                    });

                }

            return result;
        }

        public void Importar(DataTable dt, int idPeticion, IContextService contextService, ImportarModel model)
        {
            throw new NotImplementedException();
        }

        public int CrearPeticionImportacion(IContextService contextService)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
