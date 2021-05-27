using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using Marfil.Dom.ControlsUI.ConvertirProspectosClientes;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Documentos.Albaranes;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Converter;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IProspectosService
    {

    }

    public class ProspectosService : GestionService<ProspectosModel, Prospectos>, IProspectosService
    {
        #region CTR

        public ProspectosService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        #region Create

        public override void create(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as ProspectosModel;

                base.create(obj);

                //guardar direcciones
                GuardarCuentas(model);
                GuardarDirecciones(model);
                GuardarContactos(model);
                tran.Complete();
            }

        }

        #endregion

        #region Edit

        public override void edit(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as ProspectosModel;

                base.edit(obj);

                //guardar direcciones
                GuardarCuentas(model);
                GuardarDirecciones(model);
                GuardarContactos(model);
                tran.Complete();
            }
        }

        #endregion

        #region Delete

        public override void delete(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as ProspectosModel;

                base.delete(obj);

                DeleteCuentas(model);
                DeleteDirecciones(model);
                DeleteContactos(model);
                tran.Complete();
            }
        }


        #endregion

        #region List Index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = CuentasService.GetTercerosIndexModel<ProspectosModel>(_context, controller, canEliminar, canModificar);
            model.List = GetAll<CuentasBusqueda>();
            return model;
        }

        public override string GetSelectPrincipal()
        {
            return string.Format(CuentasService.SelectCuentasTerceros, (int)TiposCuentas.Prospectos, Empresa, TiposCuentas.Prospectos);
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
            var result = new StExiste();
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

        private void DeleteCuentas(ProspectosModel model)
        {
            var fservice = FService.Instance;
            var cuentasService = fservice.GetService(typeof(CuentasModel), _context, _db) as CuentasService;
            cuentasService.FlagDeleteFromThird = true;
            cuentasService.delete(model.Cuentas);
        }

        private void GuardarCuentas(ProspectosModel model)
        {
            model.Cuentas.Tiposcuentas = (int)TiposCuentas.Prospectos;
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

        private void ProcessContactos(ProspectosModel model)
        {
            foreach (var item in model.Contactos.Contactos)
            {
                item.Empresa = model.Empresa;
                item.Fkentidad = model.Fkcuentas;
                item.Tipotercero = TiposCuentas.Prospectos;
                if (item.Id < 0)
                {
                    item.Id = GetNextContactoId(model);
                }
            }
        }

        private int GetNextContactoId(ProspectosModel model)
        {
            var result = 1;

            if (model.Contactos != null && model.Contactos.Contactos.Any())
            {
                result = model.Contactos.Contactos.Max(f => f.Id) + 1;
            }

            return result;
        }

        private void GuardarContactos(ProspectosModel model)
        {
            if (model.Contactos == null) return;
            var fservice = FService.Instance;
            var contactosService = fservice.GetService(typeof(ContactosLinModel), _context, _db) as ContactosService;
            ProcessContactos(model);

            contactosService.CleanAllContactos(TiposCuentas.Prospectos, model.Fkcuentas);
            foreach (var item in model.Contactos.Contactos)
            {
                contactosService.create(item);
            }
        }

        private void DeleteContactos(ProspectosModel model)
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

        private void ProcessDirecciones(ProspectosModel model)
        {
            foreach (var item in model.Direcciones.Direcciones)
            {
                item.Empresa = model.Empresa;
                item.Fkentidad = model.Fkcuentas;
                item.Tipotercero = (int)TiposCuentas.Prospectos;
                if (item.Id < 0)
                {
                    item.Id = GetNextId(model);
                }
            }
        }

        private int GetNextId(ProspectosModel model)
        {
            var result = 1;

            if (model.Direcciones != null && model.Direcciones.Direcciones.Any())
            {
                result = model.Direcciones.Direcciones.Max(f => f.Id) + 1;
            }

            return result;
        }

        private void GuardarDirecciones(ProspectosModel model)
        {
            if (model.Direcciones == null) return;
            var fservice = FService.Instance;
            var direccionesService = fservice.GetService(typeof(DireccionesLinModel), _context, _db) as DireccionesService;
            ProcessDirecciones(model);

            direccionesService.CleanAllDirecciones(Empresa, TiposCuentas.Prospectos, model.Fkcuentas);
            foreach (var item in model.Direcciones.Direcciones)
            {
                direccionesService.create(item);
            }
        }

        private void DeleteDirecciones(ProspectosModel model)
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

        public IEnumerable<CuentasBusqueda> GetProspectosAndClientes(bool primeracarga=false)
        {
            var result = new List<CuentasBusqueda>();
            var service = FService.Instance.GetService(typeof (CuentasModel), _context, _db) as CuentasService;

            result.AddRange(service.GetCuentas(TiposCuentas.Clientes).Where(f => primeracarga || !(f.Bloqueado ?? false)));
            result.AddRange(service.GetCuentas(TiposCuentas.Prospectos).Where(f => primeracarga || !(f.Bloqueado ?? false)));

            return result;
        }

        public IEnumerable<CuentasBusqueda> GetProspectos(bool primeracarga = false)
        {
            var result = new List<CuentasBusqueda>();
            var service = FService.Instance.GetService(typeof(CuentasModel), _context, _db) as CuentasService;

            result.AddRange(service.GetCuentas(TiposCuentas.Prospectos).Where(f => primeracarga || !(f.Bloqueado ?? false)));

            return result;
        }


        public IProspectoCliente GetProspectoCliente(string id)
        {
            var serviceClientes = FService.Instance.GetService(typeof(ClientesModel), _context, _db);
            if(serviceClientes.exists(id))
                return serviceClientes.get(id) as IProspectoCliente;
            if(exists(id))
                return get(id) as IProspectoCliente;

            return null;
        }

        public void ConvertirProspectoEnCliente(ConvertirProspectoClienteModel model)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var prospectoObj = get(model.ProspectoId) as ProspectosModel;

                var clienteService = FService.Instance.GetService(typeof (ClientesModel), _context, _db);
                var clienteObj = GenerarObjetoCliente(model,prospectoObj);
                clienteService.create(clienteObj);
                ActualizarPresupuesosConProspectoActual(prospectoObj,clienteObj);
                delete(prospectoObj);


                tran.Complete();
            }
        }

        private void ActualizarPresupuesosConProspectoActual(ProspectosModel prospectoObj, ClientesModel clienteObj)
        {
            var result =_db.Presupuestos.Where(f => f.empresa == Empresa && f.fkclientes == prospectoObj.Fkcuentas);
            foreach (var item in result)
                item.fkclientes = clienteObj.Fkcuentas;

            _db.SaveChanges();
        }

        private ClientesModel GenerarObjetoCliente(ConvertirProspectoClienteModel model,ProspectosModel prospectoObj)
        {
            var user = _context;
            var result=new ClientesModel();
            result.Empresa = Empresa;
            result.Fkcuentas = model.ClienteId;
            result.Cuentas = new CuentasModel()
            {
                Empresa=Empresa,
                Id = model.ClienteId,
                Descripcion = prospectoObj.Descripcion,
                Descripcion2 = prospectoObj.RazonSocial,
                FkPais = prospectoObj.Cuentas.FkPais,
                Nif = prospectoObj.Cuentas.Nif,
                UsuarioId = user.Id.ToString()
            };
            result.Fkidiomas = prospectoObj.Fkidiomas;
            result.Fkfamiliacliente = prospectoObj.Fkfamiliacliente;
            result.Fkzonacliente = prospectoObj.Fkzonacliente;
            result.Fktipoempresa = prospectoObj.Fktipoempresa;
            result.Fkunidadnegocio = prospectoObj.Fkunidadnegocio;
            result.Fkincoterm = prospectoObj.Fkincoterm;
            result.Fkpuertos = prospectoObj.Fkpuertos;
            var qint = Funciones.Qint(prospectoObj.Fkmonedas);
            if (qint != null)
                result.Fkmonedas = qint.Value;
            result.Fkregimeniva = prospectoObj.Fkregimeniva;
            result.Fkcuentasagente = prospectoObj.Fkcuentasagente;
            result.Fkcuentascomercial = prospectoObj.Fkcuentascomercial;
            result.Fkformaspago = prospectoObj.Fkformaspago;
            result.Fktarifas = prospectoObj.Fktarifas;
            result.Notas = prospectoObj.Observaciones;
            result.Contactos = new ContactosModel();//prospectoObj.Contactos;
            result.Direcciones = new DireccionesModel();// prospectoObj.Direcciones;
            result.Contactos.Empresa = Empresa;
            result.Contactos.Id = Guid.NewGuid();
            result.Contactos.Tipotercero = (int) TiposCuentas.Clientes;
            result.Direcciones.Empresa = Empresa;
            result.Direcciones.Id = Guid.NewGuid();
            result.Direcciones.Tipotercero = (int)TiposCuentas.Clientes;
            var i = 1;
            var listcontactos=new List<ContactosLinModel>();
            var propertiescontactos = typeof (ContactosLinModel).GetProperties();
            foreach (var item in prospectoObj.Contactos.Contactos.ToList())
            {
                var newItem=new ContactosLinModel();
                foreach (var p in propertiescontactos)
                {
                    if (p.CanWrite)
                        p.SetValue(newItem,p.GetValue(item));
                }
                newItem.Id = (i++)*-1;
                newItem.Tipotercero=TiposCuentas.Clientes;
                newItem.Fkentidad = result.Fkcuentas;

                listcontactos.Add(newItem);


            }
            result.Contactos.Contactos = listcontactos;
            i = 1;
            var listdirecciones = new List<DireccionesLinModel>();
            var propertiesdirecciones = typeof(DireccionesLinModel).GetProperties();
            foreach (var item in prospectoObj.Direcciones.Direcciones.ToList())
            {
                var newItem=new DireccionesLinModel();
                foreach (var p in propertiesdirecciones)
                {
                    if(p.CanWrite)
                    p.SetValue(newItem, p.GetValue(item));
                }
                newItem.Id = (i++)*-1;
                newItem.Tipotercero = (int)TiposCuentas.Clientes;
                newItem.Fkentidad = result.Fkcuentas;

                listdirecciones.Add(newItem);
            }
            result.Direcciones.Direcciones = listdirecciones;
            return result;
        }

    }
}
