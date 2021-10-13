using System;
using System.Linq;
using System.Web.Mvc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView;
using Resources;
using System.IO;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using System.Text;
using System.Data;
using System.Web.Hosting;
using System.Threading.Tasks;
using System.Threading;
using Marfil.Dom.Persistencia;
using System.Collections.Generic;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.App.WebMain.Controllers
{
    public class ImportarController : GenericController<ImportarModel>
    {
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            //IsActivado = ContextService.IsSuperAdmin;
            //CanCrear = false;
            //CanModificar = false;
            //CanEliminar = false;
            MenuName = "importar";
            var permisos = appService.GetPermisosMenu("importar");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        #region CTR

        public ImportarController(IContextService context) : base(context)
        {

        }

        #endregion

        #region ImportarStock

        public ActionResult ImportarStock()
        {
            ImportarModel model = new ImportarModel();

            using (var db = MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos))
            {
                model.Serie = db.Series.Where(f => f.empresa == Empresa && f.entradasvarias == true)
                            .Select(f => new SelectListItem() { Value = f.id, Text = f.descripcion }).ToList();
            }

            model.TipoLote = new List<SelectListItem> {
                new SelectListItem { Text = Enum.GetName(typeof(TipoAlmacenlote), TipoAlmacenlote.Mercaderia), Value = TipoAlmacenlote.Mercaderia.ToString() },
                new SelectListItem { Text = Enum.GetName(typeof(TipoAlmacenlote), TipoAlmacenlote.Propio), Value = TipoAlmacenlote.Propio.ToString() },
                new SelectListItem { Text = Enum.GetName(typeof(TipoAlmacenlote), TipoAlmacenlote.Gestionado), Value = TipoAlmacenlote.Gestionado.ToString() }
            };


            return View("ImportarStock", model);
        }

        [HttpPost]
        public ActionResult ImportarStock(ImportarModel model)
        {
            var idPeticion = 0;
            var file = model.Fichero;
            char delimitador = model.Delimitador.ToCharArray()[0];
            int albaran = int.Parse(model.Albaran);
            string serie = model.SelectedId;
            int tipoLote = Funciones.Qint(model.SelectedIdTipoAlmacenLote) ?? 0;

            using (var db = MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos))
            {
                model.Serie = db.Series.Where(f => f.empresa == Empresa && f.entradasvarias == true)
                            .Select(f => new SelectListItem() { Value = f.id, Text = f.descripcion }).ToList();
            }

            model.TipoLote = new List<SelectListItem> {
                new SelectListItem { Text = Enum.GetName(typeof(TipoAlmacenlote), TipoAlmacenlote.Mercaderia), Value = TipoAlmacenlote.Mercaderia.ToString() },
                new SelectListItem { Text = Enum.GetName(typeof(TipoAlmacenlote), TipoAlmacenlote.Propio), Value = TipoAlmacenlote.Propio.ToString() },
                new SelectListItem { Text = Enum.GetName(typeof(TipoAlmacenlote), TipoAlmacenlote.Gestionado), Value = TipoAlmacenlote.Gestionado.ToString() }
            };

            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                    if (file.FileName.ToLower().EndsWith(".csv"))
                    {
                        var service = FService.Instance.GetService(typeof(RecepcionesStockModel), ContextService) as RecepcionStockService;
                        StreamReader sr = new StreamReader(file.InputStream, Encoding.UTF8);
                        StringBuilder sb = new StringBuilder();
                        DataTable dt = new DataTable();
                        DataRow dr;
                        string s;
                        int j = 0;

                        dt.Columns.Add("CodArticulo");
                        dt.Columns.Add("Descripcion");
                        dt.Columns.Add("Lote");
                        dt.Columns.Add("Tabla");
                        dt.Columns.Add("Cantidad");                        
                        dt.Columns.Add("Largo");
                        dt.Columns.Add("Ancho");
                        dt.Columns.Add("Grueso");
                        dt.Columns.Add("Metros");
                        dt.Columns.Add("Precio");                                       

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
                                            dr[dt.Columns[i]] = str[i].Replace("\"", string.Empty).ToString() ?? string.Empty;
                                        }
                                        catch (Exception ex)
                                        {
                                            ModelState.AddModelError("File", General.ErrorDelimitadorFormato);
                                            return View("ImportarStock", model);
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
                            HostingEnvironment.QueueBackgroundWorkItem(async token => await GetAsync(dt, albaran, serie, tipoLote, idPeticion, token));
                            //service.Importar(dt, model.Seriecontable.ToString(), ContextService);
                            sr.Close();
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

                        //TempData["Success"] = "Importado correctamente!";
                        TempData["Success"] = "Ejecutando, proceso con id = " + idPeticion.ToString() + ", para comprobar su ejecución ir al menú de peticiones asíncronas";
                        return RedirectToAction("ImportarStock", "Importar");
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

            return View("ImportarStock", model);
        }

        private async Task GetAsync(DataTable dt, int albaran, string serie, int tipoLote, int idPeticion, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var service = FService.Instance.GetService(typeof(RecepcionesStockModel), ContextService) as RecepcionStockService)
                {
                    await Task.Run(() => service.Importar(dt, albaran, serie, tipoLote, idPeticion, ContextService));
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

        #region ImportarArticulos

        public ActionResult ImportarArticulos()
        {
            ImportarModel model = new ImportarModel();

            //using (var db = MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos))
            //{
            //    model.Serie = db.Series.Where(f => f.empresa == Empresa && f.entradasvarias == true)
            //                .Select(f => new SelectListItem() { Value = f.id, Text = f.descripcion }).ToList();
            //}

            return View("ImportarArticulos", model);
        }

        [HttpPost]
        public ActionResult ImportarArticulos(ImportarModel model)
        {
            var idPeticion = 0;
            var file = model.Fichero;
            char delimitador = model.Delimitador.ToCharArray()[0];                        

            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                    if (file.FileName.ToLower().EndsWith(".csv"))
                    {
                        var service = FService.Instance.GetService(typeof(ArticulosModel), ContextService) as ArticulosService;
                        StreamReader sr = new StreamReader(file.InputStream, Encoding.UTF8);
                        StringBuilder sb = new StringBuilder();
                        DataTable dt = new DataTable();
                        DataRow dr;
                        string s;
                        int j = 0;                        
                        
                        dt.Columns.Add("CodArticulo");                        
                        dt.Columns.Add("Descripcion");
                        dt.Columns.Add("Descripcion2");                        
                        dt.Columns.Add("Largo");
                        dt.Columns.Add("Ancho");
                        dt.Columns.Add("Grueso");
                        dt.Columns.Add("KilosUd");
                        dt.Columns.Add("MedidaLibre");
                        dt.Columns.Add("ExcluirComisiones");
                        dt.Columns.Add("ExentoRetencion");
                        dt.Columns.Add("PrecioVenta");
                        dt.Columns.Add("PrecioCompra");                        

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
                                            dr[dt.Columns[i]] = str[i].Replace("\"", string.Empty).ToString() ?? string.Empty;
                                        }
                                        catch (Exception ex)
                                        {
                                            ModelState.AddModelError("File", General.ErrorDelimitadorFormato);
                                            return View("ImportarArticulos", model);
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
                            HostingEnvironment.QueueBackgroundWorkItem(async token => await GetAsyncArticulos(dt, idPeticion, token));
                            //service.Importar(dt, model.Seriecontable.ToString(), ContextService);
                            sr.Close();
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

                        //TempData["Success"] = "Importado correctamente!";
                        TempData["Success"] = "Ejecutando, proceso con id = " + idPeticion.ToString() + ", para comprobar su ejecución ir al menú de peticiones asíncronas";
                        return RedirectToAction("ImportarArticulos", "Importar");
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

            return View("ImportarArticulos", model);
        }

        private async Task GetAsyncArticulos(DataTable dt, int idPeticion, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var service = FService.Instance.GetService(typeof(ArticulosModel), ContextService) as ArticulosService)
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