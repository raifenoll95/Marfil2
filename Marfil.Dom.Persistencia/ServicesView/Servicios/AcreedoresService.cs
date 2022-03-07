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
using Marfil.Dom.ControlsUI.NifCif;
using System.Globalization;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IAcreedoresService
    {
        
    }

    public class AcreedoresService : GestionService<AcreedoresModel, Acreedores>, IMobileTercerosService, IAcreedoresService
    {
        #region CTR

        public AcreedoresService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        #region Create

        public override void create(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as AcreedoresModel;

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
                var model = obj as AcreedoresModel;

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
                var model = obj as AcreedoresModel;

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
            var model = CuentasService.GetTercerosIndexModel<AcreedoresModel>(_context, controller, canEliminar, canModificar);
            model.List = GetAll<CuentasBusqueda>();
            return model;
        }

        public override string GetSelectPrincipal()
        {
            return string.Format(CuentasService.SelectCuentasTerceros, (int) TiposCuentas.Acreedores, Empresa, TiposCuentas.Acreedores);
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

        private void DeleteCuentas(AcreedoresModel model)
        {
            var fservice = FService.Instance;
            var cuentasService = fservice.GetService(typeof(CuentasModel), _context, _db) as CuentasService;
            cuentasService.FlagDeleteFromThird = true;
            cuentasService.delete(model.Cuentas);
        }

        private void GuardarCuentas(AcreedoresModel model)
        {
            model.Cuentas.Tiposcuentas = (int)TiposCuentas.Acreedores;
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

        private void ProcessContactos(AcreedoresModel model)
        {
            foreach (var item in model.Contactos.Contactos)
            {
                item.Empresa = model.Empresa;
                item.Fkentidad = model.Fkcuentas;
                item.Tipotercero = TiposCuentas.Acreedores;
                if (item.Id < 0)
                {
                    item.Id = GetNextContactoId(model);
                }
            }
        }

        private int GetNextContactoId(AcreedoresModel model)
        {
            var result = 1;

            if (model.Contactos != null && model.Contactos.Contactos.Any())
            {
                result = model.Contactos.Contactos.Max(f => f.Id) + 1;
            }

            return result;
        }

        private void GuardarContactos(AcreedoresModel model)
        {
            if (model.Contactos == null) return;
            var fservice = FService.Instance;
            var contactosService = fservice.GetService(typeof(ContactosLinModel), _context, _db) as ContactosService;
            ProcessContactos(model);

            contactosService.CleanAllContactos(TiposCuentas.Acreedores, model.Fkcuentas);
            foreach (var item in model.Contactos.Contactos)
            {
                contactosService.create(item);
            }
        }

        private void DeleteContactos(AcreedoresModel model)
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

        private void ProcessDirecciones(AcreedoresModel model)
        {
            foreach (var item in model.Direcciones.Direcciones)
            {
                item.Empresa = model.Empresa;
                item.Fkentidad = model.Fkcuentas;
                item.Tipotercero = (int)TiposCuentas.Acreedores;
                if (item.Id < 0)
                {
                    item.Id = GetNextId(model);
                }
            }
        }

        private int GetNextId(AcreedoresModel model)
        {
            var result = 1;

            if (model.Direcciones != null && model.Direcciones.Direcciones.Any())
            {
                result = model.Direcciones.Direcciones.Max(f => f.Id) + 1;
            }

            return result;
        }

        private void GuardarDirecciones(AcreedoresModel model)
        {
            if (model.Direcciones == null) return;
            var fservice = FService.Instance;
            var direccionesService = fservice.GetService(typeof(DireccionesLinModel), _context, _db) as DireccionesService;
            ProcessDirecciones(model);

            direccionesService.CleanAllDirecciones(Empresa, TiposCuentas.Acreedores, model.Fkcuentas);
            foreach (var item in model.Direcciones.Direcciones)
            {
                direccionesService.create(item);
            }
        }

        private void DeleteDirecciones(AcreedoresModel model)
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

        private void ProcessBancos(AcreedoresModel model)
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

        private string GetNextBancosId(AcreedoresModel model)
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

        private void GuardarBancos(AcreedoresModel model)
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


        private void DeleteBancos(AcreedoresModel model)
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

        #region Busqueda de terceros

        public IEnumerable<IItemResultadoMovile> BuscarTercero(string cuenta)
        {
            var model = get(cuenta) as AcreedoresModel;

            var result = new List<IItemResultadoMovile>();
            result.Add(new ItemCabeceraResultadoMoviles()
            {
                Valor = Inf.ResourcesGlobalization.Textos.Entidades.Acreedores.TituloEntidad
            });

            result.Add(new ItemResultadoMovile()
            {
                Campo = Inf.ResourcesGlobalization.Textos.Entidades.Acreedores.Fkcuentas,
                Valor = model.Fkcuentas
            });

            result.Add(new ItemResultadoMovile()
            {
                Campo = Inf.ResourcesGlobalization.Textos.Entidades.Acreedores.Descripcion,
                Valor = model.Descripcion
            });

            result.Add(new ItemResultadoMovile()
            {
                Campo = Inf.ResourcesGlobalization.Textos.Entidades.Acreedores.RazonSocial,
                Valor = model.Razonsocial
            });

            result.Add(new ItemResultadoMovile()
            {
                Campo = Inf.ResourcesGlobalization.Textos.Entidades.Acreedores.Pais,
                Valor = model.Pais
            });

            result.Add(new ItemResultadoMovile()
            {
                Campo = Inf.ResourcesGlobalization.Textos.Entidades.Acreedores.Nif,
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
            List<AcreedoresModel> ListaAcreedores = new List<AcreedoresModel>();

            foreach (DataRow row in dt.Rows)
            {
                AcreedoresModel acreedor = new FModel().GetModel<AcreedoresModel>(contextService);

                var fkcuentas = row["codigo"].ToString();

                var existeAcreedor = _db.Acreedores.Where(f => f.empresa == Empresa && f.fkcuentas == fkcuentas).SingleOrDefault();

                if (existeAcreedor == null)
                {
                    //proveedor
                    acreedor.Fkcuentas = fkcuentas;
                    if (row["vacac1"].ToString() != "")
                    {
                        if (row["vacac1"].ToString().Length > 5)
                        {
                            acreedor.Periodonopagodesde = row["vacac1"].ToString().Substring(0,5);
                        }
                        else
                        {
                            acreedor.Periodonopagodesde = row["vacac1"].ToString();
                        }                       
                        
                    }
                    if (row["vacac2"].ToString() != "")
                    {
                        if (row["vacac2"].ToString().Length > 5)
                        {
                            acreedor.Periodonopagohasta = row["vacac2"].ToString().Substring(0, 5);
                        }
                        else
                        {
                            acreedor.Periodonopagohasta = row["vacac2"].ToString();
                        }
                    }
                    acreedor.Diafijopago1 = int.Parse(row["diapago1"].ToString());
                    acreedor.Diafijopago2 = int.Parse(row["diapago2"].ToString());
                    acreedor.Descuentoprontopago = double.Parse(row["dtopp"].ToString().Replace('.', ','), CultureInfo.CreateSpecificCulture("es-ES"));
                    acreedor.Descuentocomercial = double.Parse(row["dtocial"].ToString().Replace('.', ','), CultureInfo.CreateSpecificCulture("es-ES"));
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
                        acreedor.Fkformaspago = formapago;
                    }
                    //acreedor.Fkcuentasagente = row["codagte"].ToString();
                    //acreedor.Fkcuentascomercial = row["codcomer"].ToString();
                    var zona = row["czona"].ToString();
                    if (zona != "" && !ExisteZona(zona))
                    {
                        errores += fkcuentas + ";" + " La zona no existe" + Environment.NewLine;
                        continue;
                    }
                    else
                    {
                        acreedor.Fkzonaacreedor = row["czona"].ToString();
                    }
                    var inco = row["codinco"].ToString();
                    if (inco != "" && !ExisteInco(inco))
                    {
                        errores += fkcuentas + ";" + " El Incoterm no existe" + Environment.NewLine;
                        continue;
                    }
                    else
                    {
                        acreedor.Fkincoterm = row["codinco"].ToString();
                    }
                    var transportista = row["ctransp"].ToString();
                    if (transportista != "" && _db.Transportistas.Where(f => f.fkcuentas == transportista).FirstOrDefault() == null)
                    {
                        errores += fkcuentas + ";" + " El Transportista no existe" + Environment.NewLine;
                        continue;
                    }
                    else
                    {
                        acreedor.Fktransportistahabitual = row["ctransp"].ToString();
                    }
                    acreedor.Notas = row["notas"].ToString();
                    //acreedor.Fkcuentasaseguradoras = row["ciaseg"].ToString();
                    //acreedor.Suplemento = row["ciasupl"].ToString();
                    //acreedor.Riesgoconcedidoempresa = int.Parse(row["riescemp"].ToString());
                    //acreedor.Riesgosolicitado = int.Parse(row["riessol"].ToString());
                    //acreedor.Riesgoaseguradora = int.Parse(row["riesccia"].ToString());
                    //acreedor.Porcentajeriesgocomercial = int.Parse(row["riescom"].ToString());
                    //acreedor.Porcentajeriesgopolitico = int.Parse(row["riespol"].ToString());
                    //acreedor.Diascondecidos = int.Parse(row["riesdia"].ToString());
                    var cuentatesoreria = row["cobrador"].ToString();
                    if (row["cobrador"].ToString() != "" && _db.Cuentastesoreria.Where(f => f.fkcuentas == cuentatesoreria).FirstOrDefault() == null)
                    {
                        errores += fkcuentas + ";" + " La cuenta de tesorería no existe" + Environment.NewLine;
                        continue;
                    }
                    else
                    {
                        acreedor.Cuentatesoreria = row["cobrador"].ToString();
                    }
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
                    acreedor.Fkmonedas = moneda;
                    acreedor.Criterioiva = (CriterioIVA)_db.Empresas.Where(f => f.id == Empresa).FirstOrDefault().criterioiva; //Equivalencia empresa
                    acreedor.Fkgruposiva = "NORM"; //Equivalencia todos a normal
                    var tipoopeparse = int.Parse(row["tipoope"].ToString());
                    if (_db.RegimenIva.Where(f => f.empresa == Empresa && f.tipooperacionclassic == tipoopeparse).FirstOrDefault() != null)
                    {
                        acreedor.Fkregimeniva = _db.RegimenIva.Where(f => f.empresa == Empresa && f.tipooperacionclassic == tipoopeparse).FirstOrDefault().id;
                    }
                    else
                    {
                        acreedor.Fkregimeniva = "NORMA";
                    }
                    acreedor.Fktiposretencion = row["tiporet"].ToString();
                    acreedor.Tarifa = _db.Empresas.Where(f => f.id == Empresa).FirstOrDefault().fktarifascompras;
                    //acreedor.Perteneceagrupo = row["cligrupo"].ToString();
                    var familia = row["tipocli"].ToString();
                    if (familia != "" && !ExisteFamilia(familia))
                    {
                        errores += fkcuentas + ";" + " La familia no existe" + Environment.NewLine;
                        continue;
                    }
                    else
                    {
                        acreedor.Fkfamiliaacreedor = row["tipocli"].ToString();
                    }
                    if (row["paisiso"].ToString() == "")
                    {
                        errores += fkcuentas + ";" + "El pais ISO no pude ser vacío" + Environment.NewLine;
                        continue;
                    }
                    else
                    {
                        acreedor.Fkidiomas = GetPaisISO(row["paisiso"].ToString());
                    }
                    //acreedor.Numerocopiasfactura = int.Parse(row["ncopiasfac"].ToString());

                    //Cuentas
                    acreedor.Cuentas.Id = fkcuentas;
                    acreedor.Cuentas.Descripcion = row["nombre"].ToString() + " " + row["nombre2"].ToString();
                    acreedor.Cuentas.Descripcion2 = row["rsocial"].ToString();
                    acreedor.Cuentas.Nivel = 0;
                    var nifModel = new NifCifModel();
                    nifModel.Nif = row["nif"].ToString();
                    nifModel.TipoNif = row["tiponif"].ToString();
                    acreedor.Cuentas.Nif = nifModel;
                    DateTime fechaAlta;
                    if (DateTime.TryParseExact(row["fechaalta"].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaAlta))
                    {
                        acreedor.Cuentas.Fechaalta = fechaAlta;
                    }
                    /*else
                    {
                        errores += fkcuentas + ";" + row["fechaalta"].ToString() + " " + "La fecha fechaalta no se ha podido convertir";
                        continue;
                    }*/
                    if (row["paisiso"].ToString() == "")
                    {
                        errores += fkcuentas + ";" + "El pais ISO no pude ser vacío" + Environment.NewLine;
                        continue;
                    }
                    else
                    {
                        acreedor.Cuentas.FkPais = GetPaisISO(row["paisiso"].ToString());
                    }
                    acreedor.Cuentas.Usuario = contextService.Usuario;
                    acreedor.Cuentas.UsuarioId = contextService.RoleId.ToString();

                    //Direcciones
                    var direccion = new DireccionesLinModel();
                    var direccion2 = new DireccionesLinModel();
                    var direcciones = new List<DireccionesLinModel>();
                    //dir 1
                    if (row["direccion"].ToString() != "")
                    {
                        direccion.Id = -1;//Valor negativo para el proceso de creación le asigne el que corresponda.
                        if (row["provincia"].ToString() == "")
                        {
                            direccion.Fkprovincia = row["provincia"].ToString();
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
                        if (row["paisiso"].ToString() == "")
                        {
                            errores += fkcuentas + ";" + "El pais ISO no pude ser vacío" + Environment.NewLine;
                            continue;
                        }
                        else
                        {
                            direccion.Fkpais = GetPaisISO(row["paisiso"].ToString());
                        }
                        direccion.Descripcion = "Dirección Principal";
                        direcciones.Add(direccion);
                        acreedor.Direcciones.Direcciones = direcciones;
                    }

                    //Contactos

                    //BancosMandatos
                    if (row["iban"].ToString() != "")
                    {
                        var banco = new BancosMandatosLinModel();
                        var bancos = new List<BancosMandatosLinModel>();
                        banco.Id = "-2";
                        banco.Descripcion = row["banconom"].ToString() != "" ? row["banconom"].ToString() : "Banco Principal";
                        if (row["bancodir"] != null && row["bancodi2"] != null)
                        {
                            banco.Direccion = row["bancodir"].ToString() + Environment.NewLine + row["bancodi2"].ToString();
                        }
                        else if (row["bancodir"] != null && row["bancodi2"] == null)
                        {
                            banco.Direccion = row["bancodir"].ToString();
                        }
                        else
                        {
                            banco.Direccion = "";
                        }
                        banco.Iban = row["iban"].ToString();
                        var entidad = banco.Iban.Substring(4, 4);
                        if (row["bic"].ToString() != "")
                        {
                            banco.Bic = row["bic"].ToString();
                        }
                        else if (_db.Bancos.Where(f => f.id == entidad).FirstOrDefault() != null)
                        {
                            banco.Bic = _db.Bancos.Where(f => f.id == entidad).FirstOrDefault().bic;
                        }
                        else
                        {
                            errores += fkcuentas + ";" + "El BIC no existe para el banco " + entidad + Environment.NewLine;
                            continue;
                        }
                        banco.Fkpaises = GetPaisISO(row["paisiso"].ToString());
                        bancos.Add(banco);
                        acreedor.BancosMandatos.BancosMandatos = bancos;
                    }
                    

                    ListaAcreedores.Add(acreedor);

                }
                else
                {
                    errores += fkcuentas + " El código del acreedor ya existe" + Environment.NewLine;
                }
            }

            //Creamos los clientes
            foreach (var acre in ListaAcreedores)
            {
                try
                {
                    create(acre);
                }
                catch (Exception ex)
                {
                    errores += acre.Fkcuentas + ";" + acre.Descripcion + ";" + ex.Message + Environment.NewLine;
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
            item.configuracion = TipoImportacion.ImportarAcreedores.ToString();

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

                if (ISO == paisISO)
                {
                    XmlNodeList nodo = datosParse.GetElementsByTagName("Valor");
                    var pais = nodo[0].InnerText;
                    return pais;
                }

            }

            return "";
        }
        public bool ExisteZona(string zona)
        {
            XmlDocument doc = new XmlDocument();
            var datos = _db.TablasvariasLin.Where(f => f.fkTablasvarias == 2).Select(c => c.xml).ToList();
            foreach (var item in datos)
            {
                doc.LoadXml(item);
                XmlElement datosParse = doc.DocumentElement;

                XmlNodeList nodoIso = datosParse.GetElementsByTagName("Valor");
                var valor = nodoIso[0].InnerText;

                if (valor == zona)
                {
                    return true;
                }

            }

            return false;
        }
        public bool ExisteInco(string inco)
        {
            XmlDocument doc = new XmlDocument();
            var datos = _db.TablasvariasLin.Where(f => f.fkTablasvarias == 5).Select(c => c.xml).ToList();
            foreach (var item in datos)
            {
                doc.LoadXml(item);
                XmlElement datosParse = doc.DocumentElement;

                XmlNodeList nodoIso = datosParse.GetElementsByTagName("Valor");
                var valor = nodoIso[0].InnerText;

                if (valor == inco)
                {
                    return true;
                }

            }

            return false;
        }
        public bool ExisteFamilia(string familia)
        {
            XmlDocument doc = new XmlDocument();
            var datos = _db.TablasvariasLin.Where(f => f.fkTablasvarias == 1).Select(c => c.xml).ToList();
            foreach (var item in datos)
            {
                doc.LoadXml(item);
                XmlElement datosParse = doc.DocumentElement;

                XmlNodeList nodoIso = datosParse.GetElementsByTagName("Valor");
                var valor = nodoIso[0].InnerText;

                if (valor == familia)
                {
                    return true;
                }

            }

            return false;
        }
        #endregion
    }
}
