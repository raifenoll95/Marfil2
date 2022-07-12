using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Security;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.Empresa;
using Marfil.Dom.Persistencia.Model.Configuracion.Planesgenerales;
using Marfil.Dom.Persistencia.Model.Documentos.Presupuestos;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;
using System.IO;
using System.Data;
using System.Text;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.ControlsUI.NifCif;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Startup;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Contabilidad;
using Marfil.Dom.Persistencia.Model.Contabilidad;
using Marfil.Dom.Persistencia.Model.Documentos.CobrosYPagos;

namespace Marfil.App.WebMain.Controllers
{
    public class EmpresasController:GenericController<EmpresaModel>
    {
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }

        protected override void CargarParametros()
        {
            
            IsActivado = ContextService.IsSuperAdmin;
            CanModificar = CanCrear = CanEliminar = IsActivado;
        }

        #region CTR

        public EmpresasController(IContextService context) : base(context)
        {

        }

        #endregion

        #region Create

        public override ActionResult Create()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());

            var modelview = Helper.fModel.GetModel<EmpresaModel>(ContextService);

            var aux = TempData["model"] == null ? modelview : TempData["model"] as EmpresaModel;
            aux.Paises = modelview.Paises;
            aux.PlanesGenerales = modelview.PlanesGenerales;
            aux.LstTarifasVentas = modelview.LstTarifasVentas;
            aux.LstTarifasCompras = modelview.LstTarifasCompras;
            using (var gestionService = FService.Instance.GetService(typeof(EmpresaModel), ContextService))
            {
                ((IToolbar)aux).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Alta, aux);
            }
            return View(aux);
        }

        //create operacion empresas
        public override ActionResult CreateOperacion(EmpresaModel model)
        {
            try
            {
                using (var gestionService = createService(model) as EmpresasService)
                {
                    ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Alta, model);
                    if (ModelState.IsValid)
                    {
                        gestionService.create(model);
                        HostingEnvironment.QueueBackgroundWorkItem(async token => await generarCircuitosTesoreria(model, token));
                        HostingEnvironment.QueueBackgroundWorkItem(async token => await generarPlanContabilidad(model, token));
                        HostingEnvironment.QueueBackgroundWorkItem(async token => await generarSeriesContables(model, token));
                        HostingEnvironment.QueueBackgroundWorkItem(async token => await generarGuiasBalances(model, token));
                        HostingEnvironment.QueueBackgroundWorkItem(async token => await generarDocumentos(model, token));
                        TempData["Success"] = General.MensajeExitoOperacion;
                        return RedirectToAction("Index");
                    }
                }
                TempData["errors"] = string.Join("; ", ViewData.ModelState.Values
                   .SelectMany(x => x.Errors)
                   .Select(x => x.ErrorMessage));
                TempData["model"] = model;
                return RedirectToAction("Create");
            }
            catch (Exception ex)
            {
                model.Context = ContextService;
                TempData["errors"] = ex.Message;
                TempData["model"] = model;
                return RedirectToAction("Create");
            }
        }
        private async Task generarCircuitosTesoreria(EmpresaModel model, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var service = new CircuitosTesoreriaCobrosService(ContextService);
                var csvFile = ContextService.ServerMapPath("~/App_Data/Otros/circuitostesoreria.csv");

                using (var reader = new StreamReader(csvFile, Encoding.Default, true))
                {
                    var contenido = reader.ReadToEnd();
                    await Task.Run(() => CrearCircuitos(contenido));
                }
            }
            catch (TaskCanceledException tce)
            {
            }
            catch (Exception ex)
            {
            }
        }

        private void CrearCircuitos(string xml)
        {
            var lineas = xml.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var item in lineas)
            {
                if (!string.IsNullOrEmpty(item))
                    CrearCircuito(item);
            }

        }

        private void CrearCircuito(string linea)
        {
            var vector = linea.Split(';');
            var model = new CircuitoTesoreriaCobrosModel()
            {
                Empresa = vector[0],
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
                Importecuentacargo1 = (TipoImporte)Int32.Parse(vector[18]),
                Importecuentacargo2 = (TipoImporte)Int32.Parse(vector[19]),
                Importecuentaabono1 = (TipoImporte)Int32.Parse(vector[20]),
                Importecuentaabono2 = (TipoImporte)Int32.Parse(vector[21]),
                Importecuentacargorel = (TipoImporte)Int32.Parse(vector[22]),
                Importecuentaabonorel = (TipoImporte)Int32.Parse(vector[23]),
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


            var service = new CircuitosTesoreriaCobrosService(ContextService);
            service.create(model);
        }

        private async Task generarGuiasBalances(EmpresaModel model, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var service = new SeriesContablesService(ContextService);
                var csvFile = ContextService.ServerMapPath("~/App_data/Otros/guiasbalancescabecera.csv");

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
            var newContext = new ContextLogin()
            {
                BaseDatos = ContextService.BaseDatos,
                Empresa = empresa.Id,
                Id = ContextService.Id,
                RoleId = ContextService.RoleId
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
            var csvFile = ContextService.ServerMapPath("~/App_data/Otros/guiasbalanceslineas.csv");
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
                var _db = MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos);
                var service = new FStartup(ContextService, _db);
                var itemService = new DocumentosStartup(ContextService, _db);
                //var csvFile = ContextService.ServerMapPath("~/App_Data/Otros/documentos.csv");
                await Task.Run(() => itemService.CrearDatos("~/App_Data/Otros/documentos.csv",model.Id));
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
                var service = new PlanesGeneralesService(ContextService);
                var item = service.get(model.Fkplangeneralcontable) as PlanesGeneralesModel;
                var csvFile = ContextService.ServerMapPath(item.Fichero);

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
            var newContext = new ContextLogin()
            {
                BaseDatos = ContextService.BaseDatos,
                Empresa = empresa.Id,
                Id = ContextService.Id,
                RoleId = ContextService.RoleId
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
                var service = new SeriesContablesService(ContextService);
                var csvFile = ContextService.ServerMapPath("~/App_data/Otros/seriescontables.csv");

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
            var newContext = new ContextLogin()
            {
                BaseDatos = ContextService.BaseDatos,
                Empresa = empresa.Id,
                Id = ContextService.Id,
                RoleId = ContextService.RoleId
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
                Fkejercicios = ContextService.Ejercicio
            };

            var service = new SeriesContablesService(newContext);

            service.create(model);
        }

        public async Task GenerarContabilidadAsync(EmpresaModel model, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var service = FService.Instance.GetService(typeof(EmpresaModel), ContextService) as EmpresasService)
            {
                await Task.Run(() => service.CrearPlanGeneral(model.Id, model.Fkplangeneralcontable, model.Fkpais));
            }
        }

        #endregion

        #region Edit

        public override ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());

            var modelview = Helper.fModel.GetModel<EmpresaModel>(ContextService);
            using (var gestionService = createService(modelview as IModelView))
            {
                EmpresaModel model;
                if (TempData["model"] != null)
                {
                    model = TempData["model"] as EmpresaModel;
                    Session["_empresa_" + id] = null;
                }
                else
                    model = gestionService.get(id) as EmpresaModel;

                if (model == null)
                {
                    return HttpNotFound();
                }

                model.Paises = modelview.Paises;
                model.PlanesGenerales = modelview.PlanesGenerales;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                
                
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult EditOperacion(EmpresaModel model)
        {
            var obj = model as IModelView;
            var objExt = model as IModelViewExtension;
            try
            {
                if (ModelState.IsValid)
                {
                    using (var gestionService = createService(model))
                    {
                        gestionService.edit(model);
                        TempData[Constantes.VariableMensajeExito] = General.MensajeExitoOperacion;
                        return RedirectToAction("Index");
                    }
                }
                TempData["errors"] = string.Join("; ", ViewData.ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage));
                TempData["model"] = model;
                return RedirectToAction("Edit", new { id = obj.get(objExt.primaryKey.First().Name) });
            }
            catch (Exception ex)
            {
                model.Context = ContextService;
                TempData["errors"] = ex.Message;
                TempData["model"] = model;
                return RedirectToAction("Edit", new { id = obj.get(objExt.primaryKey.First().Name) });
            }
        }

        #endregion

        #region Details

        public override ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var modelview = Helper.fModel.GetModel<EmpresaModel>(ContextService);
            using (var gestionService = createService(modelview as IModelView))
            {
                
                var model = gestionService.get(id) as EmpresaModel;
                if (model == null)
                {
                    return HttpNotFound();
                }

                model.Paises = modelview.Paises;
                model.PlanesGenerales = modelview.PlanesGenerales;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Ver, model);

                return View(model);
            }
        }

        #endregion

        #region delete

        public override ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());

            using (var gestionService = FService.Instance.GetService(typeof(EmpresaModel), ContextService) as EmpresasService)
            {
                var model = gestionService.get(id);
                if (model == null)
                {
                    return HttpNotFound();
                }
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Baja, model);
                return View(model);
            }
        }

        // POST: Paises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public override ActionResult DeleteConfirmed(string id)
        {
            try
            {
                if(Empresa != "0001") 
                {
                    throw new ValidationException("Para borrar una empresa debe encontrarse en la empresa por defecto. ID 0001");
                }

                var newmodel = Helper.fModel.GetModel<EmpresaModel>(ContextService);
                using (var gestionService = FService.Instance.GetService(typeof(EmpresaModel), ContextService) as EmpresasService)
                {

                    var model = gestionService.get(id);
                    gestionService.delete(model);
                    TempData[Constantes.VariableMensajeExito] = General.MensajeExitoOperacion;
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {

                TempData["errors"] = ex.Message;
                return RedirectToAction("Delete", new { id = id });
            }

        }

        #endregion

    }
}