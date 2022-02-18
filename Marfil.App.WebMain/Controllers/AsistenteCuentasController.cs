using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.ControlsUI.Busquedas;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Newtonsoft.Json;
using Resources;
using System.IO;
using System.Data;
using Marfil.Dom.Persistencia.Model.Contabilidad;
using System.Web.UI.WebControls;
using System.Web.Hosting;
using System.Threading.Tasks;
using System.Threading;

namespace Marfil.App.WebMain.Controllers
{
    [Authorize]
    public class AsistenteCuentasController : GenericController<AsistenteCuentasModel>
    {
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "asistentecuentas";
            var permisos = appService.GetPermisosMenu("asistentecuentas");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
            CanBloquear = permisos.CanBloquear;
        }

        #region CTR

        public AsistenteCuentasController(IContextService context) : base(context)
        {

        }

        #endregion

        #region AsistenteImportación

        public ActionResult AsistenteCuentas()
        {
            AsistenteCuentasModel model = new AsistenteCuentasModel();

            model.Iso = new List<SelectListItem> {
                new SelectListItem { Text = "Iso alfanumérico 2", Value = "CodigoIsoAlfa2" },
                new SelectListItem { Text = "Iso alfanumérico 3", Value = "CodigoIsoAlfa3" },
                new SelectListItem { Text = "Iso numérico", Value = "CodigoIsoNumerico" }
                };
            //Ayuda
            var aux = model as IToolbar;
            aux.Toolbar.Acciones = HelpItem();

            return View("AsistenteCuentas", model);
        }

        [HttpPost]
        public ActionResult AsistenteCuentas(AsistenteCuentasModel model)
        {
            var idPeticion = 0;

            // Para que no de error al devolver la vista
            model.Iso = new List<SelectListItem> {
                new SelectListItem { Text = "Iso alfanumérico 2", Value = "CodigoIsoAlfa2" },
                new SelectListItem { Text = "Iso alfanumérico 3", Value = "CodigoIsoAlfa3" },
                new SelectListItem { Text = "Iso numérico", Value = "CodigoIsoNumerico" }
            };

            var file = model.Fichero;
            char delimitador = model.Delimitador.ToCharArray()[0];
            string iso = model.SelectedId;

            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                    if (file.FileName.ToLower().EndsWith(".csv") || file.FileName.ToLower().EndsWith(".CSV"))
                    {
                        var service = FService.Instance.GetService(typeof(CuentasModel), ContextService) as CuentasService;
                        StreamReader sr = new StreamReader(file.InputStream);
                        StringBuilder sb = new StringBuilder();
                        DataTable dt = new DataTable();
                        DataRow dr;
                        string s;
                        int j = 0;

                        dt.Columns.Add("Cuenta");
                        dt.Columns.Add("Descripcion");
                        dt.Columns.Add("Razonsocial");
                        dt.Columns.Add("Nif");
                        dt.Columns.Add("TipoNif");
                        dt.Columns.Add(iso);

                        while (!sr.EndOfStream)
                        {
                            while ((s = sr.ReadLine()) != null)
                            {
                                //Ignorar cabecera                    
                                if (j > 0 || !model.Cabecera)
                                {
                                    string[] str = s.Split(delimitador);
                                    dr = dt.NewRow();

                                    for (int i = 0; i < dt.Columns.Count; i++)
                                    {
                                        try
                                        {
                                            dr[dt.Columns[i]] = str[i].Replace("\"", string.Empty).ToString();
                                        }
                                        catch (Exception ex)
                                        {
                                            ModelState.AddModelError("File", General.ErrorDelimitadorFormato);
                                            return View("AsistenteCuentas", model);
                                        }
                                    }
                                    dt.Rows.Add(dr);
                                }

                                j++;
                            }
                        }
                        try
                        {
                            idPeticion = service.CrearPeticionImportacion(ContextService);
                            HostingEnvironment.QueueBackgroundWorkItem(async token => await GetAsync(dt, idPeticion, token));
                            //service.Importar(dt, ContextService);                            
                        }
                        catch (ValidationException ex)
                        {
                            if (string.IsNullOrEmpty(ex.Message))
                            {
                                TempData["Errors"] = null;
                            }
                            else
                            {
                                TempData["Errors"] = ex.Message;
                            }
                        }
                        sr.Close();

                        //TempData["Success"] = "Importado correctamente!";
                        TempData["Success"] = "Ejecutando, proceso con id = " + idPeticion.ToString() + ", para comprobar su ejecución ir al menú de peticiones asíncronas";
                        return RedirectToAction("AsistenteCuentas", "AsistenteCuentas");
                    }
                    else
                    {
                        ModelState.AddModelError("File", General.ErrorFormatoFichero);
                    }
                }
                else
                {
                    ModelState.AddModelError("File", General.ErrorFichero);
                }
            }

            return View("AsistenteCuentas", model);
        }

        private async Task GetAsync(DataTable dt, int idPeticion, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var service = FService.Instance.GetService(typeof(CuentasModel), ContextService) as CuentasService)
                {
                    await Task.Run(() => service.Importar(dt, idPeticion, ContextService));
                    return;
                }

            }
            catch (TaskCanceledException tce)
            {

            }
            catch (Exception ex)
            {
                using (var service = FService.Instance.GetService(typeof(PeticionesAsincronasModel), ContextService) as PeticionesAsincronasService)
                {
                    service.CambiarEstado(EstadoPeticion.Error, idPeticion, ex.Message);
                }
            }
        }

        #endregion
    }
}