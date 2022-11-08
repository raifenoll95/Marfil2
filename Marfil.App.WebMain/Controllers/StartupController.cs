using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Marfil.App.WebMain.Misc;
using Marfil.App.WebMain.Services;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.Empresa;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Login;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Threading;
using System.IO;
using System.Text;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Dom.Persistencia.Model.Documentos.CobrosYPagos;
using Marfil.Dom.Persistencia.Model.Contabilidad;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Contabilidad;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Startup;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.ControlsUI.NifCif;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Configuracion.Planesgenerales;

namespace Marfil.App.WebMain.Controllers
{
    public class StartupController : Controller
    {

        private readonly string _dominio;

        public StartupController()
        {
            _dominio = System.Web.HttpContext.Current.Request.Url.DnsSafeHost;
        }

        #region Usuarios admin

        public ActionResult Admin(string id)
        {
            return View(new RootModel() { DataBase = id });
        }

        [HttpPost]
        public ActionResult Admin(RootModel model)
        {

            if (ModelState.IsValid)
            {
                var context = new ContextService();
                context.BaseDatos = model.DataBase;
                using (var service = new StartupService(context,model.DataBase))
                {
                    //Create user password
                    service.CreateAdmin(model.Password);
                    using (var lservice = new LoginService())
                    {
                        HttpCookie cookie;
                        lservice.Login(_dominio,ApplicationHelper.UsuariosAdministrador, model.Password, out cookie, ApplicationHelper.EmpresaMock,string.Empty,string.Empty);

                        Response.Cookies.Add(cookie);
                    }
                    return RedirectToAction("DatosDefecto", new { id = model.DataBase });
                }
            }
            return View(model);
        }

        #endregion

        #region Cargar Datos por defecto
        
        public ActionResult DatosDefecto(string id)
        {
            var context = new ContextService();
            context.BaseDatos = id;
            using (var service = new StartupService(context,id))
            {
                return View(service.GetDatosDefecto());
            }
        }

        [HttpPost]
        public ActionResult DatosDefecto(string id, IEnumerable<DatosDefectoItemModel> model)
        {
            var context = new ContextService();
            context.BaseDatos = id;
            using (var service = new StartupService(context, id))
            {
                var serviceConfiguracion = new ConfiguracionService(context);
                serviceConfiguracion.SetCargaDatos(1);

                if (model != null)
                    HostingEnvironment.QueueBackgroundWorkItem(async token => await GetAsync(id, model, token, context));
                //service.CreateDatosDefecto(model);

                //NOV2022 - Lo cambiamos al ActionResult porque si no nunca aparece la pantalla de creación de empresa,
                //al ser async busca la configuración antes de que se inserte
                //serviceConfiguracion.SetCargaDatos(2);

                return RedirectToAction("Empresa", new { id });
            }
        }

        private async Task GetAsync(string id, IEnumerable<DatosDefectoItemModel> entidades, CancellationToken cancellationToken, ContextService _context)
        {
            var context = _context;
            context.BaseDatos = id;
            var serviceConfiguracion = new ConfiguracionService(context);
            //serviceConfiguracion.SetCargaDatos(1);

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var service = new StartupService(context, id))
                {
                    await Task.Run(() => service.CreateDatosDefecto(entidades));
                    //NOV2022 - Lo cambiamos al ActionResult porque si no nunca aparece la pantalla de creación de empresa,
                    //al ser async busca la configuración antes de que se inserte
                    serviceConfiguracion.SetCargaDatos(2);
                    return;
                }

            }
            catch (TaskCanceledException tce)
            {                                
                serviceConfiguracion.SetCargaDatos(-1);

            }
            catch (Exception ex)
            {                
                serviceConfiguracion.SetCargaDatos(-1);
            }
        }

        #endregion

        #region Empresa

        public ActionResult Empresa(string id)
        {
            var context = new ContextService();
            context.BaseDatos = id;
            using (var service = new StartupService(context, id))
            {
                var serviceConfiguracion = new ConfiguracionService(context, service.Db);
                var estadoImportacion = serviceConfiguracion.GetCargaDatos();
                EmpresaModel model;

                if (estadoImportacion == 2)
                {
                    ViewBag.database = id;
                    model = Helper.fModel.GetModel<EmpresaModel>(context, service.Db);
                    model.EstadoImportacion = estadoImportacion;
                }
                else
                {
                    model = new EmpresaModel();
                    model.EstadoImportacion = estadoImportacion;
                }
                

                return View(model);
            }

        }

        [HttpPost]
        public ActionResult Empresa(string database, EmpresaModel model)
        {
            ViewBag.database = database;
            if (ModelState.IsValid)
            {
                var context = new ContextService();
                context.BaseDatos = database;
                using (var service = new StartupService(context, database))
                {
                    try
                    {
                        if (model != null)
                        {
                            var nModel = Helper.fModel.GetModel<EmpresaModel>(context,service.Db);
                            model.Paises = nModel.Paises;
                            model.PlanesGenerales = nModel.PlanesGenerales;
                            var aux = Helper.fModel.GetModel<EmpresaModel>(context,service.Db);
                            model.PlanesGenerales = aux.PlanesGenerales;
                            model.Paises = aux.Paises;
                            model.LstTarifasVentas = aux.LstTarifasVentas;
                            model.LstTarifasCompras = aux.LstTarifasCompras;
                            service.CreateEmpresa(model);
                            //NOV2022 - Carga en segundo plano
                            HostingEnvironment.QueueBackgroundWorkItem(async token => await generarCircuitosTesoreria(model, token));
                            HostingEnvironment.QueueBackgroundWorkItem(async token => await generarPlanContabilidad(model, token));
                            HostingEnvironment.QueueBackgroundWorkItem(async token => await generarSeriesContables(model, token));
                            HostingEnvironment.QueueBackgroundWorkItem(async token => await generarGuiasBalances(model, token));
                            HostingEnvironment.QueueBackgroundWorkItem(async token => await generarDocumentos(model, token));
                            HostingEnvironment.QueueBackgroundWorkItem(async token => await generarTiposFacturas(model, token));
                            using (var loginService = new LoginService())
                            {
                                HttpCookie securityCookie;
                                FormsAuthentication.SignOut();
                                loginService.Forzardesconexion(database,ApplicationHelper.UsuariosAdministrador);
                                loginService.SetEmpresaUserAdmin(_dominio,database, model.Id,string.Empty,string.Empty,Guid.NewGuid(), out securityCookie);
                                Response.Cookies.Add(securityCookie);
                            }
                        }

                        return RedirectToAction("Index", "Home");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", ex.Message);
                        


                    }

                }
            }
          
           
            return View(model);
        }

        #endregion

        #region segundoplano

        private async Task generarTiposFacturas(EmpresaModel model, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var context = new ContextService();
                var service = new TiposFacturasIvaService(context);
                var csvFile = context.ServerMapPath("~/App_Data/Otros/tiposfacturas.csv");

                using (var reader = new StreamReader(csvFile, Encoding.Default, true))
                {
                    var contenido = reader.ReadToEnd();
                    await Task.Run(() => CrearTipos(contenido, model));
                }
            }
            catch (TaskCanceledException tce)
            {
            }
            catch (Exception ex)
            {
            }
        }

        private void CrearTipos(string xml, EmpresaModel model)
        {
            var lineas = xml.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var item in lineas)
            {
                if (!string.IsNullOrEmpty(item))
                    CrearTipo(item, model);
            }

        }

        private void CrearTipo(string linea, EmpresaModel empresa)
        {
            var context = new ContextService();
            var vector = linea.Split(';');
            var model = new TiposFacturasIvaModel()
            {
                Empresa = empresa.Id,
                Id = Int32.Parse(vector[0]),
                Tipocircuito = (TipoFactura)Int32.Parse(vector[1]),
                Descripcion = vector[2],
                Regimeniva = vector[3],
                Tipofacturadefecto = string.Equals(vector[4], '0') ? false : true,
                Ivadeducible = string.Equals(vector[5], '0') ? false : true,
                Requiereirpf = string.Equals(vector[6], '0') ? false : true,
                Operacionesexcluidas = string.Equals(vector[7], '0') ? false : true,
                Bieninversion = string.Equals(vector[8], '0') ? false : true,
                Cuentacargo1 = !string.Equals(vector[9], "NULL") ? vector[9] : null,
                Cuentacargo2 = !string.Equals(vector[10], "NULL") ? vector[10] : null,
                Cuentacargo3 = !string.Equals(vector[11], "NULL") ? vector[11] : null,
                Cuentaabono1 = !string.Equals(vector[12], "NULL") ? vector[12] : null,
                Cuentaabono2 = !string.Equals(vector[13], "NULL") ? vector[13] : null,
                Cuentaabono3 = !string.Equals(vector[14], "NULL") ? vector[14] : null,
                Importecuentacargo1 = (Dom.Persistencia.Model.Iva.TipoImporte)Int32.Parse(vector[15]),
                Importecuentacargo2 = (Dom.Persistencia.Model.Iva.TipoImporte)Int32.Parse(vector[16]),
                Importecuentaabono1 = (Dom.Persistencia.Model.Iva.TipoImporte)Int32.Parse(vector[17]),
                Importecuentaabono2 = (Dom.Persistencia.Model.Iva.TipoImporte)Int32.Parse(vector[18]),
                Importecuentacargo3 = (Dom.Persistencia.Model.Iva.TipoImporte)Int32.Parse(vector[19]),
                Importecuentaabono3 = (Dom.Persistencia.Model.Iva.TipoImporte)Int32.Parse(vector[20]),
                Desccuentacargo1 = !string.Equals(vector[21], "NULL") ? vector[21] : null,
                Desccuentacargo2 = !string.Equals(vector[22], "NULL") ? vector[22] : null,
                Desccuentacargo3 = !string.Equals(vector[23], "NULL") ? vector[23] : null,
                Desccuentaabono1 = !string.Equals(vector[24], "NULL") ? vector[24] : null,
                Desccuentaabono2 = !string.Equals(vector[25], "NULL") ? vector[25] : null,
                Desccuentaabono3 = !string.Equals(vector[26], "NULL") ? vector[26] : null,
                Tipocuenta = (TipoCuenta)Int32.Parse(vector[27]),
                Tipocuenta3 = (TipoCuenta)Int32.Parse(vector[28]),
                Tipoabono2 = (TipoCuenta)Int32.Parse(vector[29]),
                Tipoabono3 = (TipoCuenta)Int32.Parse(vector[30]),
                Formulacargo1 = !string.Equals(vector[31], "NULL") ? vector[31] : null,
                Formulacargo2 = !string.Equals(vector[32], "NULL") ? vector[32] : null,
                Formulacargo3 = !string.Equals(vector[33], "NULL") ? vector[33] : null,
                Formulaabono1 = !string.Equals(vector[34], "NULL") ? vector[34] : null,
                Formulaabono2 = !string.Equals(vector[35], "NULL") ? vector[35] : null,
                Formulaabono3 = !string.Equals(vector[36], "NULL") ? vector[36] : null
            };

            var service = new TiposFacturasIvaService(context);
            service.create(model);
        }

        private async Task generarCircuitosTesoreria(EmpresaModel model, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var context = new ContextService();
                var service = new CircuitosTesoreriaCobrosService(context);
                var csvFile = context.ServerMapPath("~/App_Data/Otros/circuitostesoreria.csv");

                using (var reader = new StreamReader(csvFile, Encoding.Default, true))
                {
                    var contenido = reader.ReadToEnd();
                    await Task.Run(() => CrearCircuitos(contenido, model));
                }
            }
            catch (TaskCanceledException tce)
            {
            }
            catch (Exception ex)
            {
            }
        }

        private void CrearCircuitos(string xml, EmpresaModel model)
        {
            var lineas = xml.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var item in lineas)
            {
                if (!string.IsNullOrEmpty(item))
                    CrearCircuito(item, model);
            }

        }

        private void CrearCircuito(string linea, EmpresaModel empresa)
        {
            var vector = linea.Split(';');
            var context = new ContextService();
            var model = new CircuitoTesoreriaCobrosModel()
            {
                Empresa = empresa.Id,
                Id = Int32.Parse(vector[1]),
                Descripcion = vector[2],
                Situacioninicial = vector[3],
                Situacionfinal = vector[4],
                //Datos = !String.IsNullOrEmpty(vector[5]) ? Int32.Parse(vector[5]) : null,
                Asientocontable = string.Equals(vector[6], '0') ? false : true,
                Fecharemesa = string.Equals(vector[7], '0') ? false : true,
                Fechapago = string.Equals(vector[8], '0') ? false : true,
                Liquidariva = string.Equals(vector[9], '0') ? false : true,
                Conciliacion = string.Equals(vector[10], '0') ? false : true,
                Datosdocumento = string.Equals(vector[11], '0') ? false : true,
                Cuentacargo1 = !string.Equals(vector[12], "NULL") ? vector[12] : null,
                Cuentacargo2 = !string.Equals(vector[13], "NULL") ? vector[13] : null,
                Cuentacargorel = !string.Equals(vector[14], "NULL") ? vector[14] : null,
                Cuentaabono1 = !string.Equals(vector[15], "NULL") ? vector[15] : null,
                Cuentaabono2 = !string.Equals(vector[16], "NULL") ? vector[16] : null,
                Cuentaabonorel = !string.Equals(vector[17], "NULL") ? vector[17] : null,
                Importecuentacargo1 = (Dom.Persistencia.Model.Documentos.CobrosYPagos.TipoImporte)Int32.Parse(vector[18]),
                Importecuentacargo2 = (Dom.Persistencia.Model.Documentos.CobrosYPagos.TipoImporte)Int32.Parse(vector[19]),
                Importecuentaabono1 = (Dom.Persistencia.Model.Documentos.CobrosYPagos.TipoImporte)Int32.Parse(vector[20]),
                Importecuentaabono2 = (Dom.Persistencia.Model.Documentos.CobrosYPagos.TipoImporte)Int32.Parse(vector[21]),
                Importecuentacargorel = (Dom.Persistencia.Model.Documentos.CobrosYPagos.TipoImporte)Int32.Parse(vector[22]),
                Importecuentaabonorel = (Dom.Persistencia.Model.Documentos.CobrosYPagos.TipoImporte)Int32.Parse(vector[23]),
                Desccuentacargo1 = !string.Equals(vector[24], "NULL") ? vector[24] : null,
                Desccuentacargo2 = !string.Equals(vector[25], "NULL") ? vector[25] : null,
                Desccuentacargorel = !string.Equals(vector[26], "NULL") ? vector[26] : null,
                Desccuentaabono1 = !string.Equals(vector[27], "NULL") ? vector[27] : null,
                Desccuentaabono2 = !string.Equals(vector[28], "NULL") ? vector[28] : null,
                Desccuentaabonorel = !string.Equals(vector[29], "NULL") ? vector[29] : null,
                Tipocircuito = (TipoCircuito)Int32.Parse(vector[30]),
                Codigodescripcionasiento = vector[31],
                Documentocartera = string.Equals(vector[32], '0') ? false : true,
                Actualizarcobrador = string.Equals(vector[33], '0') ? false : true,
                Anularremesa = string.Equals(vector[34], '0') ? false : true,
                Desvalorizacioncartera = string.Equals(vector[35], '0') ? false : true,
                Fkmodopagopreferido = vector[36]
            };

            if (!string.Equals(vector[5], "NULL"))
            {
                model.Datos = Int32.Parse(vector[5]);
            }
            else
            {
                model.Datos = null;
            }


            var service = new CircuitosTesoreriaCobrosService(context);
            service.create(model);
        }

        private async Task generarGuiasBalances(EmpresaModel model, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var context = new ContextService();
                var service = new SeriesContablesService(context);
                var csvFile = context.ServerMapPath("~/App_data/Otros/guiasbalancescabecera.csv");

                using (var reader = new StreamReader(csvFile, Encoding.Default, true))
                {
                    var contenido = reader.ReadToEnd();
                    await Task.Run(() => CrearGuiasBalances(contenido, model));
                }
            }
            catch (TaskCanceledException tce)
            {
            }
            catch (Exception ex)
            {
            }
        }

        private void CrearGuiasBalances(string xml, EmpresaModel model)
        {
            var lineas = xml.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var item in lineas)
            {
                if (!string.IsNullOrEmpty(item))
                    CrearGuiaBalances(item, model);
            }
        }

        private void CrearGuiaBalances(string linea, EmpresaModel empresa)
        {
            var context = new ContextService();
            var newContext = new ContextLogin()
            {
                BaseDatos = context.BaseDatos,
                Empresa = empresa.Id,
                Id = context.Id,
                RoleId = context.RoleId
            };

            var vector = linea.Split(';');
            var model = new GuiasBalancesModel()
            {
                Empresa = empresa.Id,
                Id = int.Parse(vector[0]),
                InformeId = (GuiasBalancesModel.TipoInformeE)int.Parse(vector[1]),
                GuiaId = (GuiasBalancesModel.TipoGuiaE)int.Parse(vector[2]),
                TextoGrupo = vector[3],
                Orden = vector[5],
                Actpas = vector[4],
                Detfor = vector[6],
                Formula = vector[7],
                RegDig = vector[8],
                Descrip = vector[9],
                Listado = vector[10]
            };

            //Obtenemos las líneas
            var csvFile = context.ServerMapPath("~/App_data/Otros/guiasbalanceslineas.csv");
            var lineas = new List<GuiasBalancesLineasModel>();

            using (var reader = new StreamReader(csvFile, Encoding.Default, true))
            {
                var contenido = reader.ReadToEnd();
                lineas = CrearGuiasBalancesLineas(contenido, model.Id, empresa.Id);
            }

            model.Lineas = lineas;

            var service = new GuiasBalancesService(newContext);

            service.create(model);
        }

        private List<GuiasBalancesLineasModel> CrearGuiasBalancesLineas(string xml, int guiabalancesId, string empresa)
        {
            var lineas = xml.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            var lista = new List<GuiasBalancesLineasModel>();

            foreach (var item in lineas)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    var vector = item.Split(';');
                    if (guiabalancesId == int.Parse(vector[0]))
                    {
                        var model = new GuiasBalancesLineasModel()
                        {
                            Empresa = empresa,
                            GuiasBalancesId = int.Parse(vector[0]),
                            InformeId = (GuiasBalancesLineasModel.TipoInformeE)int.Parse(vector[1]),
                            GuiaId = (GuiasBalancesLineasModel.TipoGuiaE)int.Parse(vector[2]),
                            Orden = vector[3],
                            Cuenta = vector[4],
                            Signo = vector[5],
                            Signoea = vector[6]
                        };

                        lista.Add(model);
                    }
                }
            }

            return lista;
        }

        private async Task generarDocumentos(EmpresaModel model, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var context = new ContextService();
                var _db = MarfilEntities.ConnectToSqlServer(context.BaseDatos);
                var service = new FStartup(context, _db);
                var itemService = new DocumentosStartup(context, _db);
                //var csvFile = ContextService.ServerMapPath("~/App_Data/Otros/documentos.csv");
                await Task.Run(() => itemService.CrearDatos("~/App_Data/Otros/documentos.csv", model.Id));
            }
            catch (TaskCanceledException tce)
            {
            }
            catch (Exception ex)
            {
            }
        }

        //Plan de cuentas en segundo plano
        public async Task generarPlanContabilidad(EmpresaModel model, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var context = new ContextService();
                var service = new PlanesGeneralesService(context);
                var item = service.get(model.Fkplangeneralcontable) as PlanesGeneralesModel;
                var csvFile = context.ServerMapPath(item.Fichero);

                using (var reader = new StreamReader(csvFile, Encoding.Default, true))
                {
                    var contenido = reader.ReadToEnd();
                    await Task.Run(() => CrearModels(contenido, model));
                }
            }
            catch (TaskCanceledException tce)
            {
            }
            catch (Exception ex)
            {
            }
        }

        public void CrearModels(string xml, EmpresaModel model)
        {
            var lineas = xml.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var item in lineas)
            {
                if (!string.IsNullOrEmpty(item))
                    CrearModel(item, model);
            }

        }

        public void CrearModel(string linea, EmpresaModel empresa)
        {
            var context = new ContextService();
            var newContext = new ContextLogin()
            {
                BaseDatos = context.BaseDatos,
                Empresa = empresa.Id,
                Id = context.Id,
                RoleId = context.RoleId
            };

            var vector = linea.Split(';');
            var model = new CuentasModel()
            {
                Empresa = empresa.Id,
                Id = vector[0],
                Descripcion2 = vector[1],
                Descripcion = vector[2],
                Nivel = int.Parse(vector[3]),
                FkPais = empresa.Fkpais,
                UsuarioId = Guid.Empty.ToString(),
                Nif = new NifCifModel(),
                Fechaalta = DateTime.Now,
                FechaModificacion = DateTime.Now
            };

            var service = new CuentasService(newContext);

            service.create(model);
        }

        //Series contables en segundo plano
        public async Task generarSeriesContables(EmpresaModel model, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var context = new ContextService();
                var service = new SeriesContablesService(context);
                var csvFile = context.ServerMapPath("~/App_data/Otros/seriescontables.csv");

                using (var reader = new StreamReader(csvFile, Encoding.Default, true))
                {
                    var contenido = reader.ReadToEnd();
                    await Task.Run(() => CrearSeriesContables(contenido, model));
                }
            }
            catch (TaskCanceledException tce)
            {
            }
            catch (Exception ex)
            {
            }
        }

        public void CrearSeriesContables(string xml, EmpresaModel model)
        {
            var lineas = xml.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var item in lineas)
            {
                if (!string.IsNullOrEmpty(item))
                    CrearSerieContable(item, model);
            }

        }

        public void CrearSerieContable(string linea, EmpresaModel empresa)
        {
            var context = new ContextService();
            var newContext = new ContextLogin()
            {
                BaseDatos = context.BaseDatos,
                Empresa = empresa.Id,
                Id = context.Id,
                RoleId = context.RoleId
            };

            var vector = linea.Split(';');
            var model = new SeriesContablesModel()
            {
                Empresa = empresa.Id,
                Tipodocumento = vector[0],
                Id = vector[1],
                Descripcion = vector[2],
                Fkmonedas = int.Parse(vector[3]),
                Fkcontadores = vector[4],
                Fkejercicios = context.Ejercicio
            };

            var service = new SeriesContablesService(newContext);

            service.create(model);
        }

        #endregion

        #region Helpers

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        #endregion
    }
}