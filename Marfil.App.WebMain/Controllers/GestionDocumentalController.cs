using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.Mvc;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Ficheros;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Pre.Ficheros;
using FileManagerSettings = DevExpress.Web.Mvc.FileManagerSettings;

namespace Marfil.App.WebMain.Controllers
{
    [Authorize]
    public class GestionDocumentalController : Controller
    {
        protected IContextService  ContextService { get; set; }

        public GestionDocumentalController(IContextService context)
        {
            ContextService = context;
        }
        public ActionResult Index()
        {
            
            var dataSource = new LinqFileSystemProvider(ContextService.BaseDatos, ContextService,Server);
            Session["galeria"] = dataSource;
            return View(dataSource);
        }

        [HttpPost]
        public ActionResult Index([Bind]LinqFileSystemProvider options)
        {
            
            return View(options);
        }

        [ValidateInput(false)]
        public ActionResult Galeria()
        {
            return PartialView("Galeria", Session["galeria"]);
        }

        [ValidateInput(false)]
        public FileStreamResult DownloadFiles()
        {
            FileStreamResult result;

            var param = HttpContext.Request.Params["DXMVCFileManagerDownloadArgument"];
            var vector = param.Split('|');
            var id = vector[2];
            var ficherosService = FService.Instance.GetService(typeof (FicherosGaleriaModel), ContextService);
            var ficheroModel = ficherosService.get(id) as FicherosGaleriaModel;
            var mappath =
                Path.Combine(LinqFileSystemProvider.GetRootFolder(Server, ContextService.Empresa, ContextService.BaseDatos),
                    string.Format("{0}{1}", ficheroModel.Id, ficheroModel.Tipo));

            if (string.IsNullOrEmpty(ContextService.Azureblob))
            {
                result = new FileStreamResult(new MemoryStream(System.IO.File.ReadAllBytes(mappath)), ficheroModel.Tipo)
                {
                    FileDownloadName = ficheroModel.Nombre
                };
            }
            else
            {
                FicherosService _ficherosService = new FicherosService(this.ContextService);

                result = new FileStreamResult
                    (new MemoryStream(_ficherosService.ReadAllBytesAzure(ContextService.Empresa,
                                                                                ContextService.Azureblob,
                                                                                ficheroModel.Id,
                                                                                ficheroModel.Tipo))
                     , ficheroModel.Tipo)
                {
                    FileDownloadName = ficheroModel.Nombre
                };
            }

            return result;

        }

        #region Api gestion desde documentos

        [HttpPost]
        public ActionResult Listar(string id)
        {
            
            var ficherosService = FService.Instance.GetService(typeof (FicherosGaleriaModel), ContextService) as FicherosService;
            var carpetaService = FService.Instance.GetService(typeof(CarpetasModel),ContextService) as CarpetasService;
            var carpetaModel = carpetaService.get(id) as CarpetasModel;
            var listado = ficherosService.GetFicherosDeCarpetaId(carpetaModel.Id).Select(f=>new FicheroGaleria(f, ContextService));
            HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
            return Json(listado);
        }

        [HttpPost]
        public ActionResult Agregar(string directorioId)
        {

            
            var ficherosService = FService.Instance.GetService(typeof(FicherosGaleriaModel),ContextService) as FicherosService;
            var carpetaService = FService.Instance.GetService(typeof(CarpetasModel),ContextService) as CarpetasService;

            if (HttpContext.Request.Files.Count > 0)
            {
                var ficheros = new List<StFicherosDocumentos>();
               for(var i=0;i< HttpContext.Request.Files.Count;i++)
                   ficheros.Add(new StFicherosDocumentos()
                   {
                       Nombre= HttpContext.Request.Files[i].FileName,
                       Datos = HttpContext.Request.Files[i].InputStream
                   });

                var carpetaModel = carpetaService.get(directorioId) as CarpetasModel;


                if (String.IsNullOrEmpty(ContextService.Azureblob))
                {
                    ficherosService.AgregarFicheros(carpetaModel, ficheros, LinqFileSystemProvider.GetRootFolder(Server, ContextService.Empresa, ContextService.BaseDatos));
                }
                else
                {
                    ficherosService.AgregarFicherosAzure(carpetaModel, ficheros, LinqFileSystemProvider.GetRootFolder(Server, ContextService.Empresa, ContextService.BaseDatos), ContextService.Empresa, ContextService.Azureblob);
                }


                HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;

            }
            else
                HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult Quitar(string id)
        {
            
            var ficherosService = FService.Instance.GetService(typeof(FicherosGaleriaModel),ContextService) as FicherosService;


            if (String.IsNullOrEmpty(ContextService.Azureblob))
            {
                ficherosService.DeleteFichero(id, LinqFileSystemProvider.GetRootFolder(Server, ContextService.Empresa, ContextService.BaseDatos));
            }
            else
            {
                ficherosService.DeleteFicheroAzure(id, ContextService.Empresa, ContextService.Azureblob);
            }


            HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
            return new EmptyResult();
        }

        public ActionResult Fichero(string id)
        {
            Byte[] datos = null ;

            var ficherosService = FService.Instance.GetService(typeof(FicherosGaleriaModel),ContextService) as FicherosService;

            var ficheroModel = ficherosService.get(id) as FicherosGaleriaModel;


            if (String.IsNullOrEmpty(ContextService.Azureblob))
            {
                 datos = System.IO.File.ReadAllBytes(Path.Combine(LinqFileSystemProvider.GetRootFolder(Server, ContextService.Empresa, ContextService.BaseDatos), string.Format("{0}{1}", ficheroModel.Id, ficheroModel.Tipo)));
            }
            else
            {
                 datos = ficherosService.ReadAllBytesAzure(ContextService.Empresa, ContextService.Azureblob, ficheroModel.Id, ficheroModel.Tipo);
            }

            var cd = new System.Net.Mime.ContentDisposition
            {
                // for example foo.bak
                FileName = ficheroModel.Nombre,

                // always prompt the user for downloading, set to true if you want 
                // the browser to try to show the file inline
                Inline = false,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(datos, ficheroModel.Tipo.Replace(".", ""));
        }

        public ActionResult Thumbnail(string id)
        {
            Byte[] datos = null;

            var ficherosService = FService.Instance.GetService(typeof(FicherosGaleriaModel),ContextService) as FicherosService;

            var ficheroModel = ficherosService.get(id) as FicherosGaleriaModel;


            if (String.IsNullOrEmpty(ContextService.Azureblob))
            {
                
                datos = System.IO.File.ReadAllBytes(Path.Combine(LinqFileSystemProvider.GetRootFolder(Server, ContextService.Empresa, ContextService.BaseDatos), string.Format("{0}{1}", ficheroModel.Id, ficheroModel.Tipo)));
            }
            else
            {
                datos = ficherosService.ReadAllBytesAzure(ContextService.Empresa, ContextService.Azureblob, ficheroModel.Id, ficheroModel.Tipo);
            }
           

            var cd = new System.Net.Mime.ContentDisposition
            {
                // for example foo.bak
                FileName = ficheroModel.Nombre,

                // always prompt the user for downloading, set to true if you want 
                // the browser to try to show the file inline
                Inline = false,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(datos, ficheroModel.Tipo.Replace(".", ""));
        }

        #endregion

    }
}