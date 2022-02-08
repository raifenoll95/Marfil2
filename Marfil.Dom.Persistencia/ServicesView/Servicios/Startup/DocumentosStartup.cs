using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DevExpress.XtraReports.UI;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using Marfil.Dom.Persistencia.Model.Diseñador;
using Marfil.Dom.Persistencia.Model.Documentos;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Startup
{
    public class DocumentosStartup : IStartup
    {
        #region Members

        private readonly DocumentosUsuarioService _documentosService;
        private readonly IContextService _context;
        #endregion

        #region Properties

        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Clase { get; set; }
        public TipoTablaVaria Tipo { get; set; }
        public bool NoEditable { get; set; }

        #endregion

        #region CTR

        public DocumentosStartup(IContextService context,MarfilEntities db)
        {
            _context = context;
            _documentosService = new DocumentosUsuarioService(_context,db);
        }

        #endregion

        public void CrearDatos(string fichero,string empresa)
        {
            var csvFile = _context.ServerMapPath(fichero);
            using (var reader = new StreamReader(csvFile, Encoding.Default, true))
            {
                var contenido = reader.ReadToEnd();
                CreateModel(contenido, empresa);
            }
        }

        private void CreateModel(string contenido, string empresa)
        {
            var lineas = contenido.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var item in lineas)
            {
                if (!string.IsNullOrEmpty(item))
                    CrearModel(item, empresa);
            }
        }

        private void CrearModel(string linea, string empresa)
        {
            var vector = linea.Split(';');

            var report = new XtraReport();
            using (var ms = new MemoryStream(File.ReadAllBytes(_context.ServerMapPath(Path.Combine("~/App_Data/Documentos/", vector[4])))))
            {
                report.LoadLayout(ms);
                report.DataSource = FDocumentosDatasourceReport.CreateReport((TipoDocumentoImpresion)Enum.Parse(typeof(TipoDocumentoImpresion), vector[0]), _context).DataSource;
                
                using (var stream = new MemoryStream())
                {
                    report.SaveLayout(stream);
                    var model = new DocumentosModel()
                    {
                        Usuario = Guid.Empty.ToString(),
                        Tipo = (TipoDocumentoImpresion)Enum.Parse(typeof(TipoDocumentoImpresion), vector[0]),
                        Nombre = vector[1],
                        Tipoprivacidad = (TipoPrivacidadDocumento)Enum.Parse(typeof(TipoPrivacidadDocumento), vector[2]),
                        Tiporeport = (TipoReport)Enum.Parse(typeof(TipoReport), vector[3]),
                        Datos = stream.ToArray(),
                        Defecto = Funciones.Qbool(vector[5]),
                        Empresa = empresa

                    };

                    _documentosService.SetPreferencia(model.Tipo, new Guid(model.Usuario), model.Tipoprivacidad, model.Tiporeport, model.Nombre, model.Datos, model.Defecto);
                }
            }

            
        }

        public void Dispose()
        {
            _documentosService?.Dispose();
        }

        public void CrearDatos(string fichero)
        {
            throw new NotImplementedException();
        }
    }
}
