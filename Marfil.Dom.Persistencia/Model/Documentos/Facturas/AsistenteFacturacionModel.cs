using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using RFacturas=Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Facturas;
namespace Marfil.Dom.Persistencia.Model.Documentos.Facturas
{
    public class ToolbarAsistenteFacturacionModel: ToolbarModel
    {
        public ToolbarAsistenteFacturacionModel()
        {
            Operacion = TipoOperacion.Custom;
            Titulo = RFacturas.Generarfacturas;
        }

        public override string GetCustomTexto()
        {
            return RFacturas.AsistenteFacturacion;
        }
    }

    public class AsistenteFacturacionModel: IToolbar
    {
        #region Members

        private ToolbarModel _toolbar;

        #endregion

        #region Properties

        public ToolbarModel Toolbar
        {
            get { return _toolbar; }
            set { _toolbar = value; }
        }

        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.Clientes), Name = "TituloEntidadSingular")]
        public string Fkclientes { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Fkalbaraninicio")]
        public string Fkalbaraninicio { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Fkalbaranfin")]
        public string Fkalbaranfin { get; set; }

        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.Series), Name = "TituloEntidadSingular")]
        public string Fkseriefactura { get; set; }

        [Required]
        [Display(ResourceType = typeof(RFacturas), Name = "Fechadocumento")]
        public DateTime Fechafactura { get; set; }

        [Required]
        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.Criteriosagrupacion), Name = "TituloEntidadSingular")]
        public string Fkcriterioagrupacion { get; set; }

        public IEnumerable<CriteriosagrupacionModel> Criteriosagrupacion { get; set; }

        #endregion

        public AsistenteFacturacionModel()
        {
            _toolbar = new ToolbarAsistenteFacturacionModel();
        }
    }
}
