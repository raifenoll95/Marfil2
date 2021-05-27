using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.Xpo;
using DevExpress.Web.Mvc;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
using DevExpress.DataAccess.Sql;
using DevExpress.XtraReports.Parameters;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Diseñador;
using Marfil.Dom.Persistencia.Model.Documentos;
using Marfil.Dom.Persistencia.Model.Documentos.Presupuestos;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Inf.Genericos.Helper;
using Marfil.Inf.ResourcesGlobalization.Textos.Entidades;
using Newtonsoft.Json;
using Resources;

namespace Marfil.App.WebMain.Controllers
{
    public class DesignerController : Controller
    {
        protected IContextService ContextService { get; set; }
        public DesignerController(IContextService context)
        {
            ContextService = context;
        }

        public ActionResult Index(string reportId, string returnUrl, bool nuevo)
        {
          
            var service = new DocumentosUsuarioService(MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));
            var userService = FService.Instance.GetService(typeof(UsuariosModel),ContextService) as UsuariosService;

            TipoDocumentoImpresion tipoDocumento;
            Guid usuario;
            string name;

            DocumentosUsuarioService.GetFromCustomId(reportId, out tipoDocumento, out usuario, out name);
            ViewBag.Tipo = (int)tipoDocumento;
            ViewBag.Usuario = usuario.ToString();
            var usuarioObj = usuario == Guid.Empty ? "Admin" : ((UsuariosModel)userService.get(usuario.ToString())).Usuario;
            var documentoModel = service.GetDocumento(tipoDocumento, usuario, name);
            return View(new DesignModel
            {
                Nuevo = nuevo,
                Tiporeport = documentoModel?.Tiporeport ?? TipoReport.Report,
                Tipodocumento = tipoDocumento,
                Tipoprivacidad = documentoModel?.Tipoprivacidad ?? TipoPrivacidadDocumento.Publico,
                UsuarioId = usuario,
                Usuarionombre = usuarioObj,
                ReturnUrl = returnUrl,
                Url = reportId,
                Report = documentoModel?.Datos,
                DataSource = FDocumentosDatasourceReport.CreateReport(tipoDocumento, ContextService).DataSource,
                Name = name
            });
        }

        public JsonResult SaveReport()
        {
            //string usuario,string tipodocumento,string tiporeport,string tipoprivacidad,string nombre
            var jsonSerializer = new JavaScriptSerializer();
            dynamic parametros = jsonSerializer.DeserializeObject(Request.Params["args"]);

            
            var reportByteVector = ReportDesignerExtension.GetReportXml("Reportdesigner");
            var service = new DocumentosUsuarioService(MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));
            var tipoDocumento = (TipoDocumentoImpresion)Enum.Parse(typeof(TipoDocumentoImpresion), parametros["tipodocumento"]);
            var tipoprivacidad = (TipoPrivacidadDocumento)Enum.Parse(typeof(TipoPrivacidadDocumento), parametros["tipoprivacidad"]);
            var tiporeport = (TipoReport)Enum.Parse(typeof(TipoReport), parametros["tiporeport"]);
            var defecto =Funciones.Qbool( parametros["defecto"]);
            // Write a report to the storage under the specified URL.
            using (var stream = new MemoryStream(reportByteVector))
            {
                using (var streamReportToSave = new MemoryStream())
                {

                    var url = DocumentosUsuarioService.CreateCustomId(tipoDocumento, new Guid(parametros["usuario"]), parametros["nombre"]);
                    var report = XtraReport.FromStream(stream, true);
                    report.Name = url;
                    report.DisplayName = parametros["nombre"];
                    report.SaveLayout(streamReportToSave);
                    service.SetPreferencia(tipoDocumento, new Guid(parametros["usuario"]), tipoprivacidad, tiporeport, parametros["nombre"], streamReportToSave.ToArray(), defecto);
                }

            }
            return Json(new { success = true, error = "none", Result = string.Format("{0}", DocumentosUsuarioService.CreateCustomId(tipoDocumento, new Guid(parametros["usuario"]), parametros["nombre"])) });
        }

        public ActionResult Export(string id)
        {
            
            var service = new DocumentosUsuarioService(MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));
            TipoDocumentoImpresion tipoDocumento;
            Guid usuario;
            string name;
            DocumentosUsuarioService.GetFromCustomId(id, out tipoDocumento, out usuario, out name);
            var model = service.GetDocumento(tipoDocumento, usuario, name);
            var document = model.Datos;
            var cd = new System.Net.Mime.ContentDisposition
            {
                // for example foo.bak
                FileName = string.Format("{0}.repx",model.Nombre),

                // always prompt the user for downloading, set to true if you want 
                // the browser to try to show the file inline
                Inline = false,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(document, "rpt");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import()
        {
            
            var Tipodocumento = Request.Params["Tipodocumento"];
            var Returnurl = Request.Params["Returnurl"];
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    byte[] data;
                    using (Stream inputStream = file.InputStream)
                    {
                        MemoryStream memoryStream = inputStream as MemoryStream;
                        if (memoryStream == null)
                        {
                            memoryStream = new MemoryStream();
                            inputStream.CopyTo(memoryStream);
                        }
                        data = memoryStream.ToArray();
                    }

                    var tipodocumento = (TipoDocumentoImpresion) Enum.Parse(typeof (TipoDocumentoImpresion), Tipodocumento);
                    return View("Index", new DesignModel
                    {
                        Nuevo = true,
                        Tiporeport = TipoReport.Report,
                        Tipodocumento = tipodocumento,
                        Tipoprivacidad = TipoPrivacidadDocumento.Publico,
                        UsuarioId = ContextService.Id,
                        Usuarionombre = ContextService.Usuario,
                        ReturnUrl = Returnurl,
                        Url = DocumentosUsuarioService.CreateCustomId(tipodocumento, ContextService.Id, "Nuevo documento"),
                        Report = data,
                        DataSource = FDocumentosDatasourceReport.CreateReport(tipodocumento, ContextService).DataSource,
                        Name = "Nuevo documento"
                    });
                }
            }

            return Redirect(Returnurl);
        }

        public ActionResult Visualizar(string tipo, string reportId, string primarykey)
        {
           
            var service = new DocumentosUsuarioService(MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));

            TipoDocumentoImpresion tipoDocumento = (TipoDocumentoImpresion)Enum.Parse(typeof(TipoDocumentoImpresion), tipo);
            Guid usuario = ContextService.Id;
            string name = reportId;

            Dictionary<string, object> dictionary = null;
            if (primarykey.IsValidJson())
            {
                dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(primarykey);
            }

            ViewBag.Tipo = (int)tipoDocumento;
            ViewBag.Usuario = usuario.ToString();
            var model = new DesignModel
            {
                Url = reportId,
                Report = service.GetDocumentoParaImprimir(tipoDocumento, usuario, name)?.Datos,
                DataSource = FDocumentosDatasourceReport.CreateReport(tipoDocumento, ContextService, primarykey).DataSource,
                Parameters = dictionary,
                Name = name
            };
            Session["ReportViewer"] = model;
            return View(model);
        }

        public ActionResult Descargar(string tipo, string reportId, string primarykey)
        {
            
            var service = new DocumentosUsuarioService(MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));

            var tipoDocumento = (TipoDocumentoImpresion)Enum.Parse(typeof(TipoDocumentoImpresion), tipo);
            Guid usuario = ContextService.Id;
            string name = reportId;


            ViewBag.Tipo = (int)tipoDocumento;
            ViewBag.Usuario = usuario.ToString();
           
            
            var model = new DesignModel
            {
                Url = reportId,
                Report = service.GetDocumentoParaImprimir(tipoDocumento, usuario, name)?.Datos,
                DataSource = FDocumentosDatasourceReport.CreateReport(tipoDocumento, ContextService, primarykey).DataSource,
                Name = name
            };

            var report = new XtraReport();
            using (var ms = new MemoryStream(model.Report))
            {
                report.LoadLayout(ms);
                report.DataSource = model.DataSource;
                report.Name = model.Name;
                using (var stream = new MemoryStream())
                {
                    report.ExportToPdf(stream);
                    return File(stream.GetBuffer(), "application/pdf");
                }
            }

        }

        public ActionResult ReportViewerPartial()
        {
            return PartialView("ReportViewerPartial", Session["ReportViewer"] as DesignModel);
        }

        public ActionResult ExportDocumentViewer()
        {
            var model = Session["ReportViewer"] as DesignModel;
            var report = XtraReport.FromStream(new MemoryStream(model.Report), true);
            report.DataSource = model.DataSource;

            return DocumentViewerExtension.ExportTo(report);
        }
    }
}