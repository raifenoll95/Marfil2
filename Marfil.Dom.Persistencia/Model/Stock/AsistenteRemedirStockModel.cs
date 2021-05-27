using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;
using RAlbaranesCompras = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.AlbaranesCompras;
using RArticulos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Articulos;
namespace Marfil.Dom.Persistencia.Model.Stock
{

    public class StRemedir
    {
        public string Lotesreferencia { get; set; }
        public string ReturnUrl { get; set; }
        public double? Nuevolargo { get; set; }
        public double? Nuevoancho { get; set; }
        public double? Nuevogrueso { get; set; }
        public bool? Sumarlargo { get; set; }
        public bool? Sumarancho { get; set; }
        public bool? Sumargrueso { get; set; }
        public string Loteproveedor { get; set; }
        public string Fkincidenciasmaterial { get; set; }
        public string Zona { get; set; }
        public string Fkcalificacioncomercial { get; set; }
        public string Fktipograno { get; set; }
        public string Fktonomaterial { get; set; }
        public string Fkvariedades { get; set; }
        public double? Pesopieza { get; set; }
    }

    public class ToolbarAsistenteRemedirLotesModel : ToolbarModel
    {
        public ToolbarAsistenteRemedirLotesModel()
        {
            Operacion = TipoOperacion.Custom;
            Titulo = RAlbaranesCompras.Remedirlotes;
        }

        public override string GetCustomTexto()
        {
            return RAlbaranesCompras.AsistenteRemedir;
        }
    }

    public class AsistenteRemedirStockModel: IToolbar
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

        public string Fkalmacen { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkproveedores")]
        public string Fkproveedores { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Desderecepcionstock")]
        public string Desderecepcionstock { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Hastarecepcionstock")]
        public string Hastarecepcionstock { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "FechaDesde")]
        public DateTime? Fechadesde { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "FechaHasta")]
        public DateTime? Fechahasta { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "DesdeLote")]
        public string Desdelote { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "HastaLote")]
        public string Hastalote { get; set; }
        
        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Nuevolargo")]
        public double? Nuevolargo { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Sumarlargo")]
        public bool Sumarlargo { get; set; }
        
        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Nuevoancho")]
        public double? Nuevoancho { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Sumarancho")]
        public bool Sumarancho { get; set; }
        
        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Nuevogrueso")]
        public double? Nuevogrueso { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Sumargrueso")]
        public bool Sumargrueso { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Bundle")]
        public string Bundle { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Loteproveedor")]
        public string Loteproveedor { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkincidenciasmaterial")]
        public string Fkincidenciasmaterial { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Zona")]
        public string Zona { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkcalificacioncomercial")]
        public string Fkcalificacioncomercial { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fktipograno")]
        public string Fktipograno { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fktonomaterial")]
        public string Fktonomaterial { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkvariedades")]
        public string Fkvariedades { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Kilosud")]
        public double? Pesopieza { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Pesoum")]
        public double? Pesoum { get; set; }

        #endregion



        public AsistenteRemedirStockModel(IContextService context)
        {
            _toolbar = new ToolbarAsistenteRemedirLotesModel();
            Fkalmacen = context.Fkalmacen;
        }
    }
}
