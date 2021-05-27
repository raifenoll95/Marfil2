using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using RDocumentos= Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Documentos;
namespace Marfil.Dom.Persistencia.Model.Diseñador
{
    public class DocumentosWrapperModel : IToolbar
    {
        public TipoDocumentoImpresion Tipo { get; set; }
        public string Titulo { get; set; }
        public IEnumerable<DocumentosModel> Lineas { get; set; }
        public ToolbarModel Toolbar { get; set; }
    }

    public class DocumentosDeleteModel : IToolbar
    {
        public TipoDocumentoImpresion Tipo { get; set; }
        public string Titulo { get; set; }
        public IEnumerable<DocumentosModel> Lineas { get; set; }
        public ToolbarModel Toolbar { get; set; }
        [Display(ResourceType = typeof(RDocumentos), Name = "CustomId")]
        public string CustomId { get; set; }
        
        [Display(ResourceType = typeof(RDocumentos), Name = "Nombre")]
        public string Nombre { get; set; }
    }

    public class DocumentosModel
    {
        public bool Defecto { get; set; }

        [Display(ResourceType = typeof(RDocumentos), Name = "CustomId")]
        public string CustomId { get; set; }

        [Display(ResourceType = typeof(RDocumentos), Name = "Tipoprivacidad")]
        public TipoPrivacidadDocumento Tipoprivacidad { get; set; }

        [Display(ResourceType = typeof(RDocumentos), Name = "Tiporeport")]
        public TipoReport Tiporeport { get; set; }
        
        public TipoDocumentoImpresion Tipo { get; set; }

        [Display(ResourceType = typeof(RDocumentos), Name = "Usuario")]
        public string Usuario { get; set; }

        [Display(ResourceType = typeof(RDocumentos), Name = "Nombre")]
        public string Nombre { get; set; }

        public byte[] Datos { get; set; }

        [Display(ResourceType = typeof(RDocumentos), Name = "Action")]
        public Action Action { get; set; }
    }


    public class DocumentosBotonImprimirModel
    {
        public string Primarykey { get; set; }
        public TipoDocumentoImpresion Tipo { get; set; }
        public string Defecto { get; set; }
        public IEnumerable<string> Lineas { get; set; }
    }
}
