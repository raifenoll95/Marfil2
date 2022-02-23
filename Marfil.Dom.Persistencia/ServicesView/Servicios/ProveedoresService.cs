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
using System.Data.Entity.Migrations;
using System.Xml;
using System.Globalization;
using Marfil.Dom.ControlsUI.NifCif;

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

        #endregion

        #region Importar
        public void Importar(DataTable dt, int idPeticion, IContextService contextService, ImportarModel model)
        {
            string errores = "";
            List<ProveedoresModel> ListaProveedores = new List<ProveedoresModel>();

            foreach (DataRow row in dt.Rows)
            {
                ProveedoresModel proveedor = new FModel().GetModel<ProveedoresModel>(contextService);

                var fkcuentas = row["codigo"].ToString();

                var existeProveedor = _db.Proveedores.Where(f => f.empresa == Empresa && f.fkcuentas == fkcuentas).SingleOrDefault();

                if (existeProveedor == null)
                {
                    //proveedor
                    proveedor.Fkcuentas = fkcuentas;
                    if (row["vacac1"].ToString() != "")
                    {
                        if (row["vacac1"].ToString().Length > 5)
                        {
                            proveedor.Periodonopagodesde = row["vacac1"].ToString().Substring(0, 5);
                        }
                        else
                        {
                            proveedor.Periodonopagodesde = row["vacac1"].ToString();
                        }
                    }
                    if (row["vacac2"].ToString() != "")
                    {
                        if (row["vacac2"].ToString().Length > 5)
                        {
                            proveedor.Periodonopagohasta = row["vacac2"].ToString().Substring(0, 5);
                        }
                        else
                        {
                            proveedor.Periodonopagohasta = row["vacac2"].ToString();
                        }
                    }
                    proveedor.Diafijopago1 = int.Parse(row["diapago1"].ToString());
                    proveedor.Diafijopago2 = int.Parse(row["diapago2"].ToString());
                    proveedor.Descuentoprontopago = double.Parse(row["dtopp"].ToString().Replace('.', ','), CultureInfo.CreateSpecificCulture("es-ES"));
                    proveedor.Descuentocomercial = double.Parse(row["dtocial"].ToString().Replace('.', ','), CultureInfo.CreateSpecificCulture("es-ES"));
                    var formapago = int.Parse(row["forpago"].ToString());
                    if (row["forpago"].ToString() == "")
                    {
                        errores += fkcuentas + ";" + " La forma de pago está vacía" + Environment.NewLine;
                        continue;
                    }
                    else if (_db.FormasPago.Where(f => f.id == formapago).FirstOrDefault() == null)
                    {
                        errores += fkcuentas + ";" + " La forma de pago no existe" + Environment.NewLine;
                        continue;
                    }
                    else
                    {
                        proveedor.Fkformaspago = formapago;
                    }
                    //proveedor.Fkcuentasagente = row["codagte"].ToString();
                    //proveedor.Fkcuentascomercial = row["codcomer"].ToString();
                    proveedor.Fkzonaproveedor = row["czona"].ToString();
                    proveedor.Fkincoterm = row["codinco"].ToString();
                    proveedor.Fktransportistahabitual = row["ctransp"].ToString();
                    proveedor.Notas = row["notas"].ToString();
                    //proveedor.Fkcuentasaseguradoras = row["ciaseg"].ToString();
                    //proveedor.Suplemento = row["ciasupl"].ToString();
                    //proveedor.Riesgoconcedidoempresa = int.Parse(row["riescemp"].ToString());
                    //proveedor.Riesgosolicitado = int.Parse(row["riessol"].ToString());
                    //proveedor.Riesgoaseguradora = int.Parse(row["riesccia"].ToString());
                    //proveedor.Porcentajeriesgocomercial = int.Parse(row["riescom"].ToString());
                    //proveedor.Porcentajeriesgopolitico = int.Parse(row["riespol"].ToString());
                    //proveedor.Diascondecidos = int.Parse(row["riesdia"].ToString());
                    proveedor.Cuentatesoreria = row["cobrador"].ToString();
                    var moneda = 0;
                    if (row["moneda"].ToString() == "181")
                    {
                        moneda = 978;
                    }
                    else if (row["moneda"].ToString() == "103")
                    {
                        moneda = 840;
                    }
                    else
                    {
                        var monedaparse = int.Parse(row["moneda"].ToString());
                        if (_db.Monedas.Where(f => f.id == monedaparse).FirstOrDefault() != null)
                        {
                            moneda = _db.Monedas.Where(f => f.id == monedaparse).FirstOrDefault().id;
                        }
                        else
                        {
                            moneda = (int)_db.Empresas.Where(f => f.id == Empresa).FirstOrDefault().fkMonedabase;
                        }
                    }
                    proveedor.Fkmonedas = moneda;
                    proveedor.Criterioiva = (CriterioIVA)_db.Empresas.Where(f => f.id == Empresa).FirstOrDefault().criterioiva; //Equivalencia empresa
                    proveedor.Fkgruposiva = "NORM"; //Equivalencia todos a normal
                    var tipoopeparse = int.Parse(row["tipoope"].ToString());
                    if (_db.RegimenIva.Where(f => f.empresa == Empresa && f.tipooperacionclassic == tipoopeparse).FirstOrDefault() != null)
                    {
                        proveedor.Fkregimeniva = _db.RegimenIva.Where(f => f.empresa == Empresa && f.tipooperacionclassic == tipoopeparse).FirstOrDefault().id;
                    }
                    else
                    {
                        proveedor.Fkregimeniva = "NORMA";
                    }
                    proveedor.Fktiposretencion = row["tiporet"].ToString();
                    proveedor.Tarifa = _db.Empresas.Where(f => f.id == Empresa).FirstOrDefault().fktarifascompras;
                    //proveedor.Perteneceagrupo = row["cligrupo"].ToString();
                    proveedor.Fkfamiliaproveedor = row["tipocli"].ToString();
                    proveedor.Fkidiomas = GetIdiomaPrincipal();
                    //proveedor.Numerocopiasfactura = int.Parse(row["ncopiasfac"].ToString());

                    //Cuentas
                    proveedor.Cuentas.Id = fkcuentas;
                    proveedor.Cuentas.Descripcion = row["nombre"].ToString() + " " + row["nombre2"].ToString();
                    proveedor.Cuentas.Descripcion2 = row["rsocial"].ToString();
                    proveedor.Cuentas.Nivel = 0;
                    var nifModel = new NifCifModel();
                    nifModel.Nif = row["nif"].ToString();
                    nifModel.TipoNif = row["tiponif"].ToString();
                    proveedor.Cuentas.Nif = nifModel;
                    DateTime fechaAlta;
                    if (DateTime.TryParseExact(row["fechaalta"].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaAlta))
                    {
                        proveedor.Cuentas.Fechaalta = fechaAlta;
                    }
                    /*else
                    {
                        errores += fkcuentas + ";" + row["fechaalta"].ToString() + " " + "La fecha fechaalta no se ha podido convertir";
                        continue;
                    }*/
                    proveedor.Cuentas.FkPais = GetPaisISO(row["paisiso"].ToString());
                    proveedor.Cuentas.Usuario = contextService.Usuario;
                    proveedor.Cuentas.UsuarioId = contextService.RoleId.ToString();

                    //Direcciones
                    var direccion = new DireccionesLinModel();
                    var direccion2 = new DireccionesLinModel();
                    var direcciones = new List<DireccionesLinModel>();
                    //dir 1
                    if(row["direccion"].ToString() != "")
                    {
                        direccion.Id = -1;//Valor negativo para el proceso de creación le asigne el que corresponda.
                        if (row["provincia"].ToString() == "")
                        {
                            errores += fkcuentas + ";" + " La provincia no puede estar vacía" + Environment.NewLine;
                            continue;
                        }
                        else if (row["provincia"].ToString().Length == 2)
                        {
                            direccion.Fkprovincia = row["provincia"].ToString();
                        }
                        else if (row["provincia"].ToString().Length == 3)
                        {
                            direccion.Fkprovincia = row["provincia"].ToString().Substring(1, 2);
                        }
                        else if (row["provincia"].ToString().Length < 2)
                        {
                            direccion.Fkprovincia = ("000" + row["provincia"].ToString()).Substring(2, 2);
                        }
                        direccion.Direccion = row["direccion"].ToString() + Environment.NewLine + row["direc2"].ToString();
                        direccion.Poblacion = row["poblacion"].ToString();
                        direccion.Personacontacto = row["contacto"].ToString();
                        direccion.Cp = row["codpostal"].ToString();
                        direccion.Defecto = true;

                        if (row["telefono"].ToString().Length <= 15)
                        {
                            direccion.Telefono = row["telefono"].ToString();
                        }
                        else
                        {
                            direccion.Telefono = row["telefono"].ToString().Substring(0, 15);
                        }
                        if (row["movil"].ToString().Length <= 15)
                        {
                            direccion.Telefonomovil = row["movil"].ToString();
                        }
                        else
                        {
                            direccion.Telefonomovil = row["movil"].ToString().Substring(0, 15);
                        }
                        if (row["telexfax"].ToString().Length <= 15)
                        {
                            direccion.Fax = row["telexfax"].ToString();
                        }
                        else
                        {
                            direccion.Fax = row["telexfax"].ToString().Substring(0, 15);
                        }
                        direccion.Email = row["email"].ToString();
                        direccion.Web = row["web"].ToString();
                        direccion.Fkpais = GetPaisISO(row["paisiso"].ToString());
                        direccion.Descripcion = "Dirección Principal";
                        direcciones.Add(direccion);
                        proveedor.Direcciones.Direcciones = direcciones;
                    }                    

                    //Contactos

                    //BancosMandatos
                    if (row["iban"].ToString() != "")
                    {
                        var banco = new BancosMandatosLinModel();
                        var bancos = new List<BancosMandatosLinModel>();
                        banco.Id = "-2";
                        banco.Descripcion = row["banconom"].ToString() != "" ? row["banconom"].ToString() : "Banco Principal";
                        banco.Direccion = row["bancodir"].ToString() + Environment.NewLine + row["bancodir2"].ToString();
                        banco.Iban = row["iban"].ToString();
                        banco.Bic = row["bic"].ToString();
                        banco.Fkpaises = GetPaisISO(row["paisiso"].ToString());
                        bancos.Add(banco);
                        proveedor.BancosMandatos.BancosMandatos = bancos;
                    }
                   

                    ListaProveedores.Add(proveedor);

                }
                else
                {
                    errores += fkcuentas + " El código del proveedor ya existe" + Environment.NewLine;
                }               
            }

            //Creamos los clientes
            foreach (var pro in ListaProveedores)
            {
                try
                {
                    create(pro);
                }
                catch (Exception ex)
                {
                    errores += pro.Fkcuentas + ";" + pro.Descripcion + ";" + ex.Message + Environment.NewLine;
                }
            }

            var item = _db.PeticionesAsincronas.Where(f => f.empresa == Empresa && f.id == idPeticion).SingleOrDefault();

            if (errores == "")
            {
                item.estado = (int)EstadoPeticion.Finalizada;
                item.resultado = errores;
            }
            else
            {
                item.estado = (int)EstadoPeticion.FinalizadaLogs;
                item.resultado = errores;
            }

            _db.PeticionesAsincronas.AddOrUpdate(item);

            try
            {
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int CrearPeticionImportacion(IContextService contextService)
        {
            var item = _db.PeticionesAsincronas.Create();

            item.empresa = contextService.Empresa;
            item.id = _db.PeticionesAsincronas.Any() ? _db.PeticionesAsincronas.Max(f => f.id) + 1 : 1;
            item.usuario = contextService.Usuario;
            item.fecha = DateTime.Today;
            item.estado = (int)EstadoPeticion.EnCurso;
            item.tipo = (int)TipoPeticion.Importacion;
            item.configuracion = TipoImportacion.ImportarProveedores.ToString();

            _db.PeticionesAsincronas.AddOrUpdate(item);
            _db.SaveChanges();

            return item.id;
        }
        public string GetIdiomaPrincipal()
        {
            XmlDocument doc = new XmlDocument();
            var datos = _db.Configuracion.FirstOrDefault().xml;
            doc.LoadXml(datos);
            XmlElement datosParse = doc.DocumentElement;

            XmlNodeList nodo = datosParse.GetElementsByTagName("Fkidioma1");
            var idioma = nodo[0].InnerText;
            return idioma;
        }

        public string GetPaisISO(string paisISO)
        {
            XmlDocument doc = new XmlDocument();
            var datos = _db.TablasvariasLin.Where(f => f.fkTablasvarias == 3166).Select(c => c.xml).ToList();
            foreach (var item in datos)
            {
                doc.LoadXml(item);
                XmlElement datosParse = doc.DocumentElement;

                XmlNodeList nodoIso = datosParse.GetElementsByTagName("CodigoIsoAlfa2");
                var ISO = nodoIso[0].InnerText;

                if(ISO == paisISO)
                {
                    XmlNodeList nodo = datosParse.GetElementsByTagName("Valor");
                    var pais = nodo[0].InnerText;
                    return pais;
                }
                
            }

            return "";
        }

        #endregion

    }
}
