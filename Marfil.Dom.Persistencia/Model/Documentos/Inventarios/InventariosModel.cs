using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Diseñador;
using Marfil.Dom.Persistencia.Model.Documentos.Presupuestos;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.GaleriaImagenes;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Preferencias;
using Marfil.Inf.Genericos;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RInventarios=Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Inventarios;
using RCliente = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Clientes;
using RCriteriosagrupacion = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Criteriosagrupacion;
using RStockactual = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Stock;
using RAlbaranesCompras = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.AlbaranesCompras;

namespace Marfil.Dom.Persistencia.Model.Documentos.Inventarios
{
    public enum EstadoLineaInventario
    {
        [StringValue(typeof(RInventarios), "EstadoLineaInventarioPendiente")]
        Pendiente,
        [StringValue(typeof(RInventarios), "EstadoLineaInventarioPunteado")]
        Punteado,
        [StringValue(typeof(RInventarios), "EstadoLineaInventarioVendido")]
        Vendido
    }


    public enum TipoAlmacenlote
    {
        [StringValue(typeof(RAlbaranesCompras), "TipoAlmacenLoteMercaderia")]
        Mercaderia,
        [StringValue(typeof(RAlbaranesCompras), "TipoAlmacenLotePropio")]
        Propio,
        [StringValue(typeof(RAlbaranesCompras), "TipoAlmacenLoteGestionado")]
        Gestionado,
    }

    public class AgregarLineaInventariosDocumentosLinModel
    {
        public string Fkarticulos { get; set; }
        public string Descripcion { get; set; }
        public string Lote { get; set; }
        public string Cantidad { get; set; }
        public string Largo { get; set; }
        public string Ancho { get; set; }
        public string Grueso { get; set; }
        public string Metros { get; set; }
        public string Estado { get; set; }
        public string Codigoestado { get; set; }
    }

    public class AgregarLineaInventariosDocumentosModel
    {
        public string Error { get; set; }
        public string Referencia { get; set; }
        public string Fecha { get; set; }
        public List<AgregarLineaInventariosDocumentosLinModel> Lineas { get; set; }
    }

    public class InventariosModel : BaseModel<InventariosModel, Persistencia.Inventarios>, IDocument
    {
        private List<InventariosLinModel> _lineas = new List<InventariosLinModel>();

        #region Properties

        public DocumentosBotonImprimirModel DocumentosImpresion { get; set; }

        public int Id { get; set; }
        
        public string Empresa { get; set; }

        [Required]
        [Display(ResourceType = typeof(RInventarios), Name = "Fkseries")]
        public string Fkseries { get; set; }

        [Required]
        [Display(ResourceType = typeof(RInventarios), Name = "Fkejercicio")]
        public int Fkejercicio { get; set; }

        
        [Display(ResourceType = typeof(RInventarios), Name = "Identificadorsegmento")]
        public string Identificadorsegmento { get; set; }

        [Display(ResourceType = typeof(RInventarios), Name = "Referencia")]
        public string Referencia { get; set; }

        [Required]
        [Display(ResourceType = typeof(RInventarios), Name = "Fechadocumento")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Fechadocumento { get; set; }

        public string Fechadocumentocadena
        {
            get { return Fechadocumento.ToShortDateString().ToString(CultureInfo.InvariantCulture) ?? ""; }
        }

        [Required]
        [Display(ResourceType = typeof(RInventarios), Name = "Fkalmacen")]
        public string Fkalmacenes { get; set; }

        [Display(ResourceType = typeof(RInventarios), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(RInventarios), Name = "Fkzonas")]
        public int? Fkalmaceneszonas { get; set; }

        [Display(ResourceType = typeof(RInventarios), Name = "FkarticulosDesde")]
        public string Fkarticulosdesde { get; set; }

        [Display(ResourceType = typeof(RInventarios), Name = "FkarticulosHasta")]
        public string Fkarticuloshasta { get; set; }

        [Display(ResourceType = typeof(RInventarios), Name = "FamiliaMateriales")]
        public string Fkfamiliamaterial { get; set; }

        [Display(ResourceType = typeof(RInventarios), Name = "FamiliaDesde")]
        public string Fkfamiliaproductodesde { get; set; }

        [Display(ResourceType = typeof(RInventarios), Name = "FamiliaHasta")]
        public string Fkfamiliaproductohasta { get; set; }

        [Display(ResourceType = typeof(RInventarios), Name = "CaracteristicasDesde")]
        public string Fkcaracteristicadesde { get; set; }

        [Display(ResourceType = typeof(RInventarios), Name = "CaracteristicasHasta")]
        public string Fkcaracteristicahasta { get; set; }

        [Display(ResourceType = typeof(RInventarios), Name = "MaterialesDesde")]
        public string Fkmaterialdesde { get; set; }

        [Display(ResourceType = typeof(RInventarios), Name = "MaterialesHasta")]
        public string Fkmaterialhasta { get; set; }

        [Display(ResourceType = typeof(RInventarios), Name = "GrosoresDesde")]
        public string Fkgrosordesde { get; set; }

        [Display(ResourceType = typeof(RInventarios), Name = "GrosoresHasta")]
        public string Fkgrosorhasta { get; set; }

        [Display(ResourceType = typeof(RInventarios), Name = "AcabadosDesde")]
        public string Fkacabadodesde { get; set; }

        [Display(ResourceType = typeof(RInventarios), Name = "AcabadosHasta")]
        public string Fkacabadohasta { get; set; }

        public Guid Integridadreferencial { get; set; }

        public TipoEstado Tipoestado(IContextService context)
        {
            return TipoEstado.Diseño;
        }

        public string Fkestados { get; set; }


        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "TipoAlmacenLote")]
        public TipoAlmacenlote? Tipodealmacenlote { get; set; }


        #endregion

        #region Líneas

        public List<InventariosLinModel> Lineas
        {
            get { return _lineas; }
            set { _lineas = value; }
        }

        #endregion

        #region CTR

        public InventariosModel()
        {

        }

        public InventariosModel(IContextService context) : base(context)
        {

        }

        #endregion

        public override object generateId(string id)
        {
            return id;
        }

        public override void createNewPrimaryKey()
        {
            primaryKey = getProperties().Where(f => f.property.Name.ToLower() == "id").Select(f => f.property);
        }

        public override string GetPrimaryKey()
        {
            return Id.ToString();
        }

        public override string DisplayName => RInventarios.TituloEntidad;
        

        public DocumentosBotonImprimirModel GetListFormatos()
        {
            var user = Context;
            using (var db = MarfilEntities.ConnectToSqlServer(user.BaseDatos))
            {
                var servicePreferencias = new PreferenciasUsuarioService(db);
                var doc = servicePreferencias.GetDocumentosImpresionMantenimiento(user.Id, TipoDocumentoImpresion.Inventarios.ToString(), "Defecto") as PreferenciaDocumentoImpresionDefecto;
                var service = new DocumentosUsuarioService(Context, db);
                {
                    var lst =
                        service.GetDocumentos(TipoDocumentoImpresion.Inventarios, user.Id)
                            .Where(f => f.Tiporeport == TipoReport.Report);
                    return new DocumentosBotonImprimirModel()
                    {
                        Tipo = TipoDocumentoImpresion.Inventarios,
                        Lineas = lst.Select(f => f.Nombre),
                        Primarykey = Referencia,
                        Defecto = doc?.Name
                    };
                }
            }

        }


        
    }
    
    public class InventariosLinModel: ILineasDocumentosBusquedaMovil
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(RInventarios), Name = "Fechadocumento")]
        public DateTime Fecha { get; set; }

        [Display(ResourceType = typeof(RInventarios), Name = "Fkarticulos")]
        public string Fkarticulos { get; set; }

        [Display(ResourceType = typeof(RInventarios), Name = "Descripcion")]
        public string Descripcion { get; set; }
        
        [Display(ResourceType = typeof(RInventarios), Name = "Lote")]
        public string Lote { get; set; }

        [Display(ResourceType = typeof(RInventarios), Name = "Loteid")]
        public string Loteid { get; set; }

        
        public string Referenciaproveedor { get; set; }

        
        public string Tag { get; set; }

        [Display(ResourceType = typeof(RInventarios), Name = "Fkunidades")]
        public string Fkunidadesmedida { get; set; }

        [Display(ResourceType = typeof(RInventarios), Name = "Cantidad")]
        public double? Cantidad { get; set; }

        [Display(ResourceType = typeof(RInventarios), Name = "Largo")]
        public double Largo { get; set; }

        [Display(ResourceType = typeof(RInventarios), Name = "Ancho")]
        public double Ancho { get; set; }

        [Display(ResourceType = typeof(RInventarios), Name = "Grueso")]
        public double Grueso { get; set; }

        [Display(ResourceType = typeof(RInventarios), Name = "Metros")]
        public double? Metros { get; set; }

        [Display(ResourceType = typeof(RInventarios), Name = "Largo")]
        public string SLargo { get { return Largo.ToString("N" + (Decimalesmedidas ?? 0)); } }

        [Display(ResourceType = typeof(RInventarios), Name = "Ancho")]
        public string SAncho { get { return Ancho.ToString("N" + (Decimalesmedidas ?? 0)); } }

        [Display(ResourceType = typeof(RInventarios), Name = "Grueso")]
        public string SGrueso { get { return Grueso.ToString("N" + (Decimalesmedidas ?? 0)); } }

        [Display(ResourceType = typeof(RInventarios), Name = "Metros")]
        public string SMetros { get { return Metros?.ToString("N" + (Decimalesmedidas ?? 0)) ?? 0.ToString("N" + (Decimalesmedidas ?? 0)); } }

        public string SPrecio { get; }
        public double? Porcentajedescuento { get; }
        public string SImporte { get; }


        public string Fkcalificacioncomercial { get; set; }

        public string Fktipograno { get; set; }

        public double? Pesonetolote { get; set; }

        public string Fktonomaterial { get; set; }

        public string Fkincidenciasmaterial { get; set; }

        public string Fkvariedades { get; set; }

        [Display(ResourceType = typeof(RInventarios), Name = "Estado")]
        public EstadoLineaInventario  Estado { get; set; }

        public int? Decimalesmedidas { get; set; }

        [Display(ResourceType = typeof(RInventarios), Name = "Fkunidades")]
        public string Fkunidadesmedidadescripcion { get; set; }
    }

  

}
