using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using DevExpress.XtraReports.UI;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Documentos;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;

namespace Marfil.App.WebMain.Misc.Designer
{
    public class CustomReportStorageWebExtension : DevExpress.XtraReports.Web.Extensions.ReportStorageWebExtension
    {
        private readonly Regex _reportIdRegex = new Regex(".+;.+;.+");

        private ICustomPrincipal GetCustomPrincipal()
        {
            var aux= HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];


            var authTicket = FormsAuthentication.Decrypt(aux.Value);
            var serializer = new JavaScriptSerializer();
            var serializeModel = serializer.Deserialize<SecurityTicket>(authTicket.UserData);
            return new CustomPrincipal(authTicket.Name)
            {
                Id = serializeModel.Id,
                RoleId = serializeModel.RoleId,
                Usuario = serializeModel.Usuario,
                BaseDatos = serializeModel.BaseDatos,
                Roles = serializeModel.Roles,
                Empresa = serializeModel.Empresa,
                Fkalmacen = serializeModel.Fkalmacen
            };
        }

        private DocumentosUsuarioService _service;
        private ICustomPrincipal _user;
        private IContextService _context;

        public CustomReportStorageWebExtension()
        {            
        }


        public override bool CanSetData(string url)
        {
            if (!_reportIdRegex.IsMatch(url))
                return false;

            _user = GetCustomPrincipal();
            //_context = GetContext(_user);
            _service = new DocumentosUsuarioService(_user.Empresa,MarfilEntities.ConnectToSqlServer(_user.BaseDatos));
            // Check if the URL is available in the report storage.
            TipoDocumentoImpresion tipoDocumentoImpresion;
            Guid usuario;
            string name;
            DocumentosUsuarioService.GetFromCustomId(url, out tipoDocumentoImpresion, out usuario, out name);
            return _service.ExisteDocumento(tipoDocumentoImpresion, usuario, name);
        }


        public override byte[] GetData(string url)
        {
            _user = GetCustomPrincipal();
            _context = _user as IContextService;
            //_context = GetContext(_user);
            _service = new DocumentosUsuarioService(_user.Empresa, MarfilEntities.ConnectToSqlServer(_user.BaseDatos));
            // Get the report data from the storage.
            TipoDocumentoImpresion TipoDocumentoImpresion;
            Guid usuario;
            string name;
            DocumentosUsuarioService.GetFromCustomId(url, out TipoDocumentoImpresion, out usuario, out name);
            var obj = _service.GetDocumento(TipoDocumentoImpresion, usuario, name);

            return obj.Datos;
        }


        public override Dictionary<string, string> GetUrls()
        {
            _user = GetCustomPrincipal();
            _context = _user as IContextService;
            //_context = GetContext(_user);
            _service = new DocumentosUsuarioService(_user.Empresa, MarfilEntities.ConnectToSqlServer(_user.BaseDatos));
            // Get URLs and display names for all reports available in the storage.
            var result= new Dictionary<string, string>();
            try
            {
                var reportId = HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.PathAndQuery).GetValues("reportId")[0];
                TipoDocumentoImpresion TipoDocumentoImpresion;
                Guid usuario;
                string name;
                DocumentosUsuarioService.GetFromCustomId(reportId, out TipoDocumentoImpresion, out usuario, out name);
                var items = _service.GetDocumentos(TipoDocumentoImpresion, _user.Id);
                foreach (var item in items)
                    result.Add(item.CustomId, item.Nombre);
            }
            catch (Exception ex)
            {
                string errores = ex.Message;
            }
            

           return result;
        }


        public override bool IsValidUrl(string url)
        {
            // Check if the specified URL is valid for the current report storage.
            // In this example, a URL should be a string containing a numeric value that is used as a data row primary key.
            return true;
        }

        public override void SetData(XtraReport report, string url)
        {
            if (!_reportIdRegex.IsMatch(url))
                return;

            _user = GetCustomPrincipal();
            _context = _user as IContextService;
            //_context = GetContext(_user);
            _service = new DocumentosUsuarioService(_user.Empresa, MarfilEntities.ConnectToSqlServer(_user.BaseDatos));
            // Write a report to the storage under the specified URL.
            using (var stream = new MemoryStream())
            {
                TipoDocumentoImpresion TipoDocumentoImpresion;
                Guid usuario;
                string name;
                
                DocumentosUsuarioService.GetFromCustomId(url, out TipoDocumentoImpresion, out usuario, out name);
                report.Name = url;
                report.DisplayName = name;
                report.SaveLayout(stream);
                //_service.SetPreferencia(TipoDocumentoImpresion, usuario, name, stream.ToArray());
            }
        }


        public override string SetNewData(XtraReport report, string defaultUrl)
        {
            if (_reportIdRegex.IsMatch(defaultUrl))
                throw new Exception("No se pude guardar el nuevo report");

            _user = GetCustomPrincipal();
            _context = _user as IContextService;
            //_context = GetContext(_user);
            _service = new DocumentosUsuarioService(_user.Empresa, MarfilEntities.ConnectToSqlServer(_user.BaseDatos));
            using (var stream = new MemoryStream())
            {
                var objTag = report.Name;
                var vector = objTag.Split(';');
                var TipoDocumentoImpresion =(TipoDocumentoImpresion)Enum.Parse(typeof(TipoDocumentoImpresion),vector[1]);
                var usuario = new Guid(vector[0]);
                var idReport= DocumentosUsuarioService.CreateCustomId(TipoDocumentoImpresion, usuario, defaultUrl);
                report.Name = idReport;
                report.DisplayName = defaultUrl;
                report.SaveLayout(stream);
                //_service.SetPreferencia(TipoDocumentoImpresion, usuario, defaultUrl, stream.ToArray());
                return idReport;
            }
        }
    }
}