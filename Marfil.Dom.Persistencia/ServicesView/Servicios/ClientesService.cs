using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Marfil.Dom.ControlsUI.BusquedaTerceros;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Documentos.Albaranes;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Converter;
using Marfil.Inf.Genericos.Helper;
using RClientes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Clientes;
using RDirecciones = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Direcciones;
using RContactos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Contactos;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IClientesService
    {

    }

    public class ClientesService : GestionService<ClientesModel, Clientes>, IMobileTercerosService, IClientesService
    {
        #region CTR

        public ClientesService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        #region Create

        public override void create(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as ClientesModel;

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
                var model = obj as ClientesModel;

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
                var model = obj as ClientesModel;

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
            var model = CuentasService.GetTercerosIndexModel<ClientesModel>(_context, controller, canEliminar, canModificar);
            model.List = GetAll<CuentasBusqueda>();
            return model;
        }

        public override string GetSelectPrincipal()
        {
            var a = string.Format(CuentasService.SelectCuentasTerceros, (int)TiposCuentas.Clientes, Empresa, TiposCuentas.Clientes);
            return a;
        }


        #endregion

        #region Tipo cuentas

        public struct StExiste
        {
            public bool Existe { get; set; }
            public bool Valido { get; set; }
        }
        public ClientesService.StExiste GetCompañia(TiposCuentas tipo, string id)
        {
            var result = new ClientesService.StExiste();
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

        private void DeleteCuentas(ClientesModel model)
        {
            var fservice = FService.Instance;
            var cuentasService = fservice.GetService(typeof(CuentasModel), _context, _db) as CuentasService;
            cuentasService.FlagDeleteFromThird = true;
            cuentasService.delete(model.Cuentas);
        }

        private void GuardarCuentas(ClientesModel model)
        {
            model.Cuentas.Tiposcuentas = (int)TiposCuentas.Clientes;
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

        private void ProcessContactos(ClientesModel model)
        {
            foreach (var item in model.Contactos.Contactos)
            {
                item.Empresa = model.Empresa;
                item.Fkentidad = model.Fkcuentas;
                item.Tipotercero = TiposCuentas.Clientes;
                if (item.Id < 0)
                {
                    item.Id = GetNextContactoId(model);
                }
            }
        }

        private int GetNextContactoId(ClientesModel model)
        {
            var result = 1;

            if (model.Contactos != null && model.Contactos.Contactos.Any())
            {
                result = model.Contactos.Contactos.Max(f => f.Id) + 1;
            }

            return result;
        }

        private void GuardarContactos(ClientesModel model)
        {
            if (model.Contactos == null) return;
            var fservice = FService.Instance;
            var contactosService = fservice.GetService(typeof(ContactosLinModel), _context, _db) as ContactosService;
            ProcessContactos(model);

            contactosService.CleanAllContactos(TiposCuentas.Clientes, model.Fkcuentas);
            foreach (var item in model.Contactos.Contactos)
            {
                contactosService.create(item);
            }
        }

        private void DeleteContactos(ClientesModel model)
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

        private void ProcessDirecciones(ClientesModel model)
        {
            foreach (var item in model.Direcciones.Direcciones)
            {
                item.Empresa = model.Empresa;
                item.Fkentidad = model.Fkcuentas;
                item.Tipotercero = (int)TiposCuentas.Clientes;
                if (item.Id < 0)
                {
                    item.Id = GetNextId(model);
                }
            }
        }

        private int GetNextId(ClientesModel model)
        {
            var result = 1;

            if (model.Direcciones != null && model.Direcciones.Direcciones.Any())
            {
                result = model.Direcciones.Direcciones.Max(f => f.Id) + 1;
            }

            return result;
        }

        private void GuardarDirecciones(ClientesModel model)
        {
            if (model.Direcciones == null) return;
            var fservice = FService.Instance;
            var direccionesService = fservice.GetService(typeof(DireccionesLinModel), _context, _db) as DireccionesService;
            ProcessDirecciones(model);

            direccionesService.CleanAllDirecciones(Empresa, TiposCuentas.Clientes, model.Fkcuentas);
            foreach (var item in model.Direcciones.Direcciones)
            {
                direccionesService.create(item);
            }
        }

        private void DeleteDirecciones(ClientesModel model)
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

        private void ProcessBancos(ClientesModel model)
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

        private string GetNextBancosId(ClientesModel model)
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

        private void GuardarBancos(ClientesModel model)
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


        private void DeleteBancos(ClientesModel model)
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

        #region FActuracion

        public IEnumerable<ClientesModel> GetClientesPendientesFacturar()
        {
            var cuentasService = FService.Instance.GetService(typeof(CuentasModel), _context);
            var list = _db.Clientes.Where(h => h.empresa == Empresa && _db.Albaranes.Join(_db.Estados, p => p.fkestados, e => e.documento + "-" + e.id, (a, b) => new { a, b }).Any(
                 f =>
                     f.b.tipoestado <= (int)TipoEstado.Curso &&
                     f.a.empresa == Empresa && f.a.fkclientes == h.fkcuentas &&
                     !_db.FacturasLin.Any(j => j.fkalbaranesreferencia == f.a.referencia))).ToList().Select(f => _converterModel.GetModelView(f) as ClientesModel).ToList();
            foreach (var item in list)
            {
                item.Cuentas = cuentasService.get(item.Fkcuentas) as CuentasModel;
            }

            return list;
        }

        #endregion

        #region Busqueda de terceros

        public IEnumerable<IItemResultadoMovile> BuscarTercero(string cuenta)
        {
            var model = get(cuenta) as ClientesModel;

            var result = new List<IItemResultadoMovile>();
            result.Add(new ItemCabeceraResultadoMoviles()
            {
                Valor = RClientes.TituloEntidad
            });

            result.Add(new ItemResultadoMovile()
            {
                Campo = RClientes.Fkcuentas,
                Valor = model.Fkcuentas
            });

            result.Add(new ItemResultadoMovile()
            {
                Campo = RClientes.Descripcion,
                Valor = model.Descripcion
            });

            result.Add(new ItemResultadoMovile()
            {
                Campo = RClientes.RazonSocial,
                Valor = model.RazonSocial
            });

            result.Add(new ItemResultadoMovile()
            {
                Campo = RClientes.Pais,
                Valor = model.Pais
            });

            result.Add(new ItemResultadoMovile()
            {
                Campo = RClientes.Nif,
                Valor = model.Nif
            });

            result.Add(new ItemSeparadorResultadoMoviles());

            //Direcciones
            result.Add(new ItemCabeceraResultadoMoviles()
            {
                Valor = RDirecciones.TituloEntidad
            });

            if (model.Direcciones != null)
                foreach (var item in model.Direcciones.Direcciones.OrderBy(f => f.Defecto))
                {
                    result.Add(new ItemResultadoMovile()
                    {
                        Campo = RDirecciones.Descripcion,
                        Valor = model.Descripcion
                    });
                    result.Add(new ItemResultadoMovile()
                    {
                        Campo = RDirecciones.Direccion,
                        Valor = model.Direccion
                    });
                    result.Add(new ItemResultadoMovile()
                    {
                        Campo = RDirecciones.Cp,
                        Valor = model.Cp
                    });
                    result.Add(new ItemResultadoMovile()
                    {
                        Campo = RDirecciones.Poblacion,
                        Valor = model.Poblacion
                    });
                }

            result.Add(new ItemSeparadorResultadoMoviles());

            //Contactos
            result.Add(new ItemCabeceraResultadoMoviles()
            {
                Valor = RContactos.TituloEntidad
            });

            if (model.Contactos != null)
                foreach (var item in model.Contactos.Contactos)
                {
                    result.Add(new ItemResultadoMovile()
                    {
                        Campo = RContactos.Nombre,
                        Valor = item.Nombre
                    });
                    result.Add(new ItemResultadoMovile()
                    {
                        Campo = RContactos.Telefono,
                        Valor = item.Telefono
                    });
                    result.Add(new ItemResultadoMovile()
                    {
                        Campo = RContactos.Telefonomovil,
                        Valor = item.Telefonomovil
                    });
                    result.Add(new ItemResultadoMovile()
                    {
                        Campo = RContactos.Email,
                        Valor = item.Email
                    });
                   
                }

            return result;
        }

        #endregion
    }
}
