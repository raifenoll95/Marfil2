using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using DevExpress.XtraReports.UI;
using Marfil.Dom.ControlsUI.Email;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Diseñador;
using Marfil.Dom.Persistencia.Model.Documentos;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Mailing;

namespace Marfil.App.WebMain.Controllers
{
    [Authorize]
    public class EmailController : Controller
    {
        class ArchivoEmail
        {
            public string filename { get; set; }
        }
        protected IContextService ContextService { get; set; }

        public EmailController(IContextService context)
        {
            ContextService = context;
        }
        // GET: Email
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Enviaremail(EmailModel model)
        {
            
            
            using (var usuariosService = FService.Instance.GetService(typeof (UsuariosModel), ContextService))
            {
                var emailConfiguration = usuariosService.get(ContextService.Id.ToString()) as UsuariosModel;

                //emailConfiguration.Usuariomail = "alertas@totware.com";
                //emailConfiguration.Passwordmail = "Fr56QdKLv3f";
                //emailConfiguration.Nombre = "Marfil Team";
                //emailConfiguration.Email = "alertas@totware.com";
                //emailConfiguration.Smtp = "smtp.totware.com";
                //emailConfiguration.Puerto = 587;

                if (emailConfiguration.IsValidEmailConfiguration)
                {
                    var emailService = new EmailsService(ContextService);
                    var errorEnvio = string.Empty;
                    emailService.EnviarEmail(string.Format("{0};{1}", model.Destinatario, model.DestinatarioCc)
                        , model.DestinatarioBcc
                        , emailConfiguration.Nombre
                        , emailConfiguration.Email //Remitente
                        , emailConfiguration.Usuariomail //
                        , emailConfiguration.Passwordmail
                        , emailConfiguration.Smtp
                        , emailConfiguration.Puerto.Value
                        , model.Asunto
                        , string.Format("{0} {1}",model.Contenido, emailConfiguration.Firma)
                        , true
                        , null
                        , LeerUrls(model.Ficheros)
                        , emailConfiguration.Ssl
                        , emailConfiguration.Copiaremitente
                        , ref errorEnvio
                        , model.Id
                        , model.Tipo);

                    HttpContext.Response.StatusCode = string.IsNullOrEmpty(errorEnvio) ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest;
                    return Json(errorEnvio);
                }
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Configuracion de email incorrecta");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Subirarchivoemail()
        {
            
            if (HttpContext.Request.Files.Count > 0)
            {
                var filename = Path.GetTempFileName();
                Thread.Sleep(5000);
                HttpContext.Request.Files[0].SaveAs(filename);

                return Json(new ArchivoEmail() { filename= filename});
            }
            HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json("No existe ningun archivo");
        }

        private List<StMensajeriaAdjuntos> LeerUrls(IEnumerable<FicherosEmailModel> fichero)
        {
            var result = new List<StMensajeriaAdjuntos>();
            if(fichero!=null && fichero.Any())
            {
                result.AddRange(fichero.Select(item => new StMensajeriaAdjuntos()
                {
                    Nombre = item.Nombre, Adjunto = item.Tipo == TipoFicherosEmail.Url ? UrlFile(item) : LocalFile(item)
                }));
            }

            return result;
            
        }

        private byte[] LocalFile(FicherosEmailModel item)
        {
            var result = new  byte[0];
            try
            {

                if (System.IO.File.Exists(item.Url))
                {
                    result = System.IO.File.ReadAllBytes(item.Url);
                }
            }
            catch (Exception)
            {
                
                
            }

            return result;
        }

        private byte[] UrlFile(FicherosEmailModel item)
        {
            var mierror = "";
            var result = new  byte[0];
            try
            {

              
                using (var service = new DocumentosUsuarioService(ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos)))
                {
                    var vector = item.Url.Split(';');
                    var primarykey = vector[0];
                    var TipoDocumentoImpresion = (TipoDocumentoImpresion)Enum.Parse(typeof(TipoDocumentoImpresion),vector[1]);
                    var reportId = vector[2];
                    var model = new DesignModel
                    {
                        Url = reportId,
                        Report = service.GetDocumento(TipoDocumentoImpresion, ContextService.Id, reportId)?.Datos,
                        DataSource = FDocumentosDatasourceReport.CreateReport(TipoDocumentoImpresion, ContextService, primarykey).DataSource,
                        Name = string.Format("{0}",item.Nombre)
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
                            result = stream.GetBuffer();
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                mierror = ex.Message;
            }
            
            return result;
        }

        private string ConvertRelativeUrlToAbsoluteUrl(string relativeUrl)
        {
            return string.Format("http{0}://{1}{2}",
                (Request.IsSecureConnection) ? "s" : "",
                Request.Url.Host,
                relativeUrl
            );
        }
    }
}