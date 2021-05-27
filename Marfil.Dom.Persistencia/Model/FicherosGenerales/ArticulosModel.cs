using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos;
using Resources;
using RArticulos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Articulos;
using RFamiliasproductos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Familiasproductos;
using RMateriales = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Materiales;
using System.Globalization;
using Marfil.Dom.Persistencia.Model.GaleriaImagenes;

namespace Marfil.Dom.Persistencia.Model.FicherosGenerales
{
    public enum TipoCategoria
    {
        [StringValue(typeof(RArticulos), "TipoCategoriaAmbas")]
        Ambas,
        [StringValue(typeof(RArticulos), "TipoCategoriaVentas")]
        Ventas,
        [StringValue(typeof(RArticulos), "TipoCategoriaCompras")]
        Compras
    }

    public class ArticulosBusqueda
    {
        public string Id { get; set; }
        public string Descripcion { get; set; }
    }

    public enum Tipogestionlotes
    {
        [StringValue(typeof(RArticulos),"Singestion")]
        Singestion,
        [StringValue(typeof(RArticulos), "Loteopcional")]
        Loteopcional,
        [StringValue(typeof(RArticulos), "Loteobligatorio")]
        Loteobligatorio
       
    }

    public class TarifasEspecificasArticulosViewModel
    {
        public string Id { get; set; }
        [Display(ResourceType = typeof(RArticulos), Name = "TarifaDescripcion")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "TarifaCuenta")]
        public string Fkcuenta { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Precio")]
        public double? Precio { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Descuento")]
        public double? Descuento { get; set; }

        public bool Obligatorio { get; set; }
    }

    public class TarifaEspecificaArticulo
    {
        private List<TarifasEspecificasArticulosViewModel> _lineas = new List<TarifasEspecificasArticulosViewModel>();

        public TipoFlujo Tipo { get; set; }

        public List<TarifasEspecificasArticulosViewModel> Lineas
        {
            get { return _lineas; }
            set { _lineas = value; }
        }
    }

    public class TarifasSistemaArticulosViewModel
    {
        public string Id { get; set; }
        public string Descripcion { get; set; }

        [Required]
        public double Precio { get; set; }

        public bool Obligatorio { get; set; }
    }
    
    public class ArticulosModel : BaseModel<ArticulosModel, Articulos>
    {
        #region Members

        //Lista de articulos de tercero
        private List<ArticulosTerceroModel> _articulosTercero = new List<ArticulosTerceroModel>();
        private List<ArticulosComponentesModel> _articulosComponentes = new List<ArticulosComponentesModel>();
        private List<TarifasSistemaArticulosViewModel> _tarifasSistemaVenta=new List<TarifasSistemaArticulosViewModel>();
        private List<TarifasSistemaArticulosViewModel> _tarifasSistemaCompra = new List<TarifasSistemaArticulosViewModel>();
        private TarifaEspecificaArticulo _tarifasEspecificasVentas = new TarifaEspecificaArticulo() { Tipo =TipoFlujo.Venta};
        private TarifaEspecificaArticulo _tarifasEspecificasCompras = new TarifaEspecificaArticulo() { Tipo = TipoFlujo.Compra };
        private bool _validarmateriales;
        private bool _validarcaracteristicas;
        private bool _validargrosores;
        private bool _validaracabados;
        private GaleriaModel _galeria;

        #endregion

        #region Properties

        public string Empresa { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Codigolibre")]
        public string Codigolibre { get; set; }

        [Required]
        [MaxLength(2, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RArticulos), Name = "Familia")]
        public string Familia { get; set; }

        public string FamiliaDescripcion { get; set; }

        public int Tipofamilia { get; set; }

        
        [MaxLength(3, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RArticulos), Name = "Materiales")]
        public string Materiales { get; set; }

        public string MaterialesDescripcion { get; set; }

        
        [MaxLength(2, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RArticulos), Name = "Caracteristicas")]
        public string Caracteristicas { get; set; }

        public string CaracteristicasDescripcion { get; set; }

        
        [MaxLength(2, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RArticulos), Name = "Grosores")]
        public string Grosores { get; set; }

        public string GrosoresDescripcion { get; set; }

        
        [MaxLength(2, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RArticulos), Name = "Acabados")]
        public string Acabados { get; set; }

        public string AcabadosDescripcion { get; set; }

        [Key]
        [Display(ResourceType = typeof(RArticulos), Name = "Id")]
        public string Id { get; set; }

        [Required]
        [CustomDisplayDescription(typeof(RArticulos), "Descripcion", CustomDisplayDescriptionAttribute.TipoDescripcion.Principal)]
        [MaxLength(120, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        public string Descripcion { get; set; }

        [CustomDisplayDescription(typeof(RArticulos), "Descripcion", CustomDisplayDescriptionAttribute.TipoDescripcion.Secundaria)]
        [MaxLength(120, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        public string Descripcion2 { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Descripcionabreviada")]
        [MaxLength(60, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        public string Descripcionabreviada { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Coste")]
        public double? Coste { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Preciomateriaprima")]
        public double? Preciomateriaprima { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Porcentajemerma")]
        public double? Porcentajemerma { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Costemateriaprima")]
        public double? Costemateriaprima { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Costeelaboracionmateriaprima")]
        public double? Costeelaboracionmateriaprima { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Costeportes")]
        public double? Costeportes { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Otroscostes")]
        public double? Otroscostes { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Costefabricacion")]
        public double? Costefabricacion { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Costeindirecto")]
        public double? Costeindirecto { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Preciominimoventa")]
        public double? Preciominimoventa { get; set; }

        [Required]
        [Display(ResourceType = typeof(RArticulos), Name = "Fkgruposiva")]
        public string Fkgruposiva { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Fkguiascontables")]
        public string Fkguiascontables { get; set; }

        [Display(ResourceType = typeof(RFamiliasproductos), Name = "Editarlargo")]
        public bool Editarlargo { get; set; }

        [Display(ResourceType = typeof(RFamiliasproductos), Name = "Editarancho")]
        public bool Editarancho { get; set; }

        [Display(ResourceType = typeof(RFamiliasproductos), Name = "Editargrueso")]
        public bool Editargrueso { get; set; }

        [Display(ResourceType = typeof(RMateriales), Name = "Fkgruposmateriales")]
        public string Fkgruposmateriales { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Partidaarancelaria")]
        [MaxLength(10,ErrorMessageResourceType = typeof(Unobtrusive),ErrorMessageResourceName = "MaxLength")]
        public string Partidaarancelaria { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Kilosud")]
        public double? Kilosud { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Rendimientom2m3")]
        public double? Rendimientom2m3 { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Medidalibre")]
        public bool Medidalibre { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Labor")]
        public bool Labor  { get; set; }

        public string Fklabores { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Excluircomisiones")]
        public bool Excluircomisiones { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Exentoretencion")]
        public bool Exentoretencion { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Gestionarcaducidad")]
        public bool Gestionarcaducidad { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Tiempostandardfabricacion")]
        public double? Tiempostandardfabricacion { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Gestionstock")]
        public bool Gestionstock { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Tipogestionlotes")]
        public Tipogestionlotes Tipogestionlotes { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Stocknegativoautorizado")]
        public bool Stocknegativoautorizado { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Lotefraccionable")]
        public bool Lotefraccionable { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Existenciasminimasmetros")]
        public double? Existenciasminimasmetros { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Existenciasmaximasmetros")]
        public double? Existenciasmaximasmetros { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Existenciasminimasunidades")]
        public double? Existenciasminimasunidades { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Existenciasmaximasunidades")]
        public double? Existenciasmaximasunidades { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Web")]
        public bool Web { get; set; }

        [Display(ResourceType = typeof(RFamiliasproductos), Name = "Articulonegocio")]
        public bool Articulonegocio { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Fkcontadores")]
        public string Fkcontadores { get; set; }      

        [Display(ResourceType = typeof(RArticulos), Name = "Clasificacion")]
        public string Clasificacion { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Consumibles")]
        public bool Consumibles { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Ean13")]
        public string Ean13 { get; set; }

        public List<ArticulosTerceroModel> ArticulosTercero
        {
            get { return _articulosTercero; }
            set { _articulosTercero = value; }
        }

        public List<TarifasSistemaArticulosViewModel> TarifasSistemaVenta
        {
            get { return _tarifasSistemaVenta; }
            set { _tarifasSistemaVenta = value; }
        }

        public List<ArticulosComponentesModel> ArticulosComponentes
        {
            get { return _articulosComponentes; }
            set { _articulosComponentes = value; }
        }

        public List<TarifasSistemaArticulosViewModel> TarifasSistemaCompra
        {
            get { return _tarifasSistemaCompra; }
            set { _tarifasSistemaCompra = value; }
        }

        public TarifaEspecificaArticulo TarifasEspecificasVentas
        {
            get { return _tarifasEspecificasVentas; }
            set { _tarifasEspecificasVentas = value; }
        }

        public TarifaEspecificaArticulo TarifasEspecificasCompras
        {
            get { return _tarifasEspecificasCompras; }
            set { _tarifasEspecificasCompras = value; }
        }

        public bool Validarmateriales
        {
            get { return _validarmateriales; }
            set { _validarmateriales = value; }
        }

        public bool Validarcaracteristicas
        {
            get { return _validarcaracteristicas; }
            set { _validarcaracteristicas = value; }
        }

        public bool Validargrosores
        {
            get { return _validargrosores; }
            set { _validargrosores = value; }
        }

        public bool Validaracabados
        {
            get { return _validaracabados; }
            set { _validaracabados = value; }
        }

        [Display(ResourceType = typeof(RArticulos), Name = "Tipoivavariable")]
        public bool Tipoivavariable { get; set; }

        // Última compra y última venta
        [Display(ResourceType = typeof(RArticulos), Name = "Movimientos")]
        public string Movimientos { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Fecha")]        
        public DateTime? Fechaultimaentrada { get; set; }

        public string Fechaultimaentradacadena
        {
            get { return Fechaultimaentrada?.ToShortDateString().ToString(CultureInfo.InvariantCulture) ?? ""; }
        }

        [Display(ResourceType = typeof(RArticulos), Name = "UltimaEntrada")]
        public string Ultimaentrada { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Fecha")]        
        public DateTime? Fechaultimasalida { get; set; }

        public string Fechaultimasalidacadena
        {
            get { return Fechaultimasalida?.ToShortDateString().ToString(CultureInfo.InvariantCulture) ?? ""; }
        }

        [Display(ResourceType = typeof(RArticulos), Name = "UltimaSalida")]
        public string Ultimasalida { get; set; }


        public int idAlbaranEntrada { get; set; }
        public int modoAlbaranEntrada { get; set; }
        public string urlAlbaranEntrada { get; set; }

        public int idAlbaranSalida { get; set; }
        public int modoAlbaranSalida { get; set; }  
        public string urlAlbaranSalida { get; set; }

        public Guid? Fkcarpetas { get; set; }

        public GaleriaModel Galeria
        {
            get
            {
                _galeria = new GaleriaModel();
                if (Fkcarpetas.HasValue)
                {
                    _galeria.Empresa = Empresa;
                    _galeria.DirectorioId = Fkcarpetas.Value;
                }
                return _galeria;
            }
        }

        #region Campos documentos

        public double? Largo { get; set; }

        public double? Ancho { get; set; }

        public double? Grueso { get; set; }

        public bool Permitemodificarlargo { get { return Editarlargo;} }

        public bool Permitemodificarancho { get { return Editarancho; } }

        public bool Permitemodificargrueso { get { return Editargrueso; } }

        public bool Permitemodificarmetros { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Fkunidades")]
        public string Fkunidades { get; set; }

        public string Unidadesdescripcion { get; set; }

        public int? Decimalestotales { get; set; }

        public TipoStockFormulas Formulas { get; set; }

        public double? Precio { get; set; }

        public string Fktiposiva { get; set; }

        public string Descripcioniva { get; set; }

        public double? Porcentajeiva { get; set; }

        public double? Porcentajerecargoequivalencia { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Articulocomentario")]
        public bool? Articulocomentario { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Articulocomentario")]
        public bool Articulocomentariovista
        {
            get { return Articulocomentario ?? false; }
            set { Articulocomentario = value; }
        }

        [Display(ResourceType = typeof(RArticulos), Name = "Piezascaja")]
        [Range(0,9999,ErrorMessageResourceType = typeof(Unobtrusive),ErrorMessageResourceName = "RangeClient")]
        public int? Piezascaja { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Categoria")]
        public TipoCategoria Categoria { get; set; }

        #endregion

        #endregion

        #region CTR

        public ArticulosModel()
        {

        }

        public ArticulosModel(IContextService context) : base(context)
        {

        }

        #endregion

        public override object generateId(string id)
        {
            return id;
        }

        public override string DisplayName => RArticulos.TituloEntidad;

       
    }

    public class ArticulosDocumentosModel
    {
        public string Id { get; set; }
     
        public string Descripcion { get; set; }

        public string Descripcion2 { get; set; }

        public double? Largo { get; set; }

        public double? Ancho { get; set; }

        public double? Grueso { get; set; }

        public bool Permitemodificarlargo { get; set; }

        public bool Permitemodificarancho { get; set; }

        public bool Permitemodificargrueso { get; set; }

        public bool Permitemodificarmetros { get; set; }

        public string Fkunidades { get; set; }

        public int? Decimalestotales { get; set; }

        public TipoStockFormulas Formulas { get; set; }

        public TipoFamilia Tipofamilia { get; set; }

        public Tipogestionlotes Tipogestionlotes { get; set; }

        public double? Precio { get; set; }

        public double? Coste { get; set; }

        public string Fktiposiva { get; set; }

        public string Descripcioniva { get; set; }

        public double? Porcentajeiva { get; set; }

        public double? Porcentajerecargoequivalencia { get; set; }

        public int? Fkmonedas { get; set; }

        public int? Decimalesmonedas { get; set; }

        public double? Cambiomonedabase { get; set; }

        public double? Cambiomonedaadicional { get; set; }

        public bool Articulocomentario { get; set; }

        public string Fkcontador { get; set; }

        public bool Lotefraccionable { get; set; }
        
        public bool Tipoivavariable { get; set; }

        public int? Piezascaja { get; set; }

        public TipoCategoria Categoria { get; set; }
    }
}

