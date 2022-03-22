using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.ControlsUI.Bloqueo;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RTarifa= Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Tarifas;
namespace Marfil.Dom.Persistencia.Model.FicherosGenerales
{
    public enum TipoFlujo
    {
        
        [StringValue(typeof(RTarifa), "TipoFlujoVenta")]
        Venta,
            [StringValue(typeof(RTarifa), "TipoFlujoCompra")]
        Compra
    }

    public enum TipoTarifa
    {
        [StringValue(typeof(RTarifa), "TipoTarifaSistema")]
        Sistema,
        [StringValue(typeof(RTarifa), "TipoTarifaEspecifica")]
        Especifica
    }

    public class TarifasLinModel
    {
        [Display(ResourceType = typeof(RTarifa), Name = "Fkarticulos")]
        public string Fkarticulos { get; set; }

        [Display(ResourceType = typeof(RTarifa), Name = "Unidades")]
        public string Unidades { get; set; }

        [Display(ResourceType = typeof(RTarifa), Name = "Precio")]
        public double? Precio { get; set; }

        [Display(ResourceType = typeof(RTarifa), Name = "Precio")]
        public string SPrecio
        {
            get { return (Precio ?? 0.0).ToString(); }
            set { Precio = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RTarifa), Name = "Descuento")]
        [Range(0,100,ErrorMessageResourceType = typeof(Unobtrusive),ErrorMessageResourceName = "RangeClient")]
        public double? Descuento { get; set; }

    }

    public class TarifasModel : BaseModel<TarifasModel, Tarifas>
    {
        private List<TarifasLinModel> _lineas=new List<TarifasLinModel>();

        #region Properties

        public string Empresa { get; set; }

        [Required]
        [Display(ResourceType = typeof(RTarifa), Name = "Id")]
        public string Id { get; set; }

        [Required]
        [Display(ResourceType = typeof(RTarifa), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Required]
        [Display(ResourceType = typeof(RTarifa), Name = "Tipoflujo")]
        public TipoFlujo Tipoflujo { get; set; }

        [Display(ResourceType = typeof(RTarifa), Name = "Tipotarifa")]
        [Required]
        public TipoTarifa Tipotarifa { get; set; }

        [Display(ResourceType = typeof(RTarifa), Name = "Fkcuentas")]
        public string Fkcuentas { get; set; }

        [Display(ResourceType = typeof(RTarifa), Name = "Fkmonedas")]
        public int? Fkmonedas { get; set; }

        [Display(ResourceType = typeof(RTarifa), Name = "Asignartarifaalcreararticulos")]
        public bool Asignartarifaalcreararticulos { get; set; }

        [Display(ResourceType = typeof(RTarifa), Name = "Precioobligatorio")]
        public bool Precioobligatorio { get; set; }

        [Display(ResourceType = typeof(RTarifa), Name = "Validodesde")]
        public DateTime? Validodesde { get; set; }

        [Display(ResourceType = typeof(RTarifa), Name = "Validohasta")]
        public DateTime? Validohasta { get; set; }

        [Display(ResourceType = typeof(RTarifa), Name = "Ivaincluido")]
        public bool Ivaincluido { get; set; }

        [Display(ResourceType = typeof(RTarifa), Name = "Observaciones")]
        public string Observaciones { get; set; }

        [Display(ResourceType = typeof(RTarifa), Name = "Precioautomaticobase")]
        public string Precioautomaticobase { get; set; }

        [Display(ResourceType = typeof(RTarifa), Name = "Precioautomaticoporcentajebase")]
        [Range(0, 100, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "RangeClient")]
        public double Precioautomaticoporcentajebase { get; set; }

        [Display(ResourceType = typeof(RTarifa), Name = "Precioautomaticoporcentajefijo")]
        [Range(0, 100, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "RangeClient")]
        public double Precioautomaticoporcentajefijo { get; set; }

        [Display(ResourceType = typeof(RTarifa), Name = "Precioautomaticofkfamiliasproductosdesde")]
        public string Precioautomaticofkfamiliasproductosdesde { get; set; }

        [Display(ResourceType = typeof(RTarifa), Name = "Precioautomaticofkfamiliasproductoshasta")]
        public string Precioautomaticofkfamiliasproductoshasta { get; set; }

        [Display(ResourceType = typeof(RTarifa), Name = "Precioautomaticofkmaterialesdesde")]
        public string Precioautomaticofkmaterialesdesde { get; set; }

        [Display(ResourceType = typeof(RTarifa), Name = "Precioautomaticofkmaterialeshasta")]
        public string Precioautomaticofkmaterialeshasta { get; set; }

        [Display(ResourceType = typeof(RTarifa), Name = "Valorarcomponentes")]
        public bool Valorarcomponentes { get; set; }


        public List<TarifasLinModel> Lineas
        {
            get { return _lineas; }
            set { _lineas = value; }
        }

        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.Cuentas), Name = "BloqueoModel")]
        public BloqueoEntidadModel BloqueoModel { get; set; }

        public bool Bloqueado
        {
            get { return BloqueoModel?.Bloqueada ?? false; }
        }

        #endregion

        #region CTR

        public TarifasModel()
        {

        }

        public TarifasModel(IContextService context) : base(context)
        {

        }

        #endregion

        public override object generateId(string id)
        {
            return id;
        }

        public override string DisplayName => RTarifa.TituloEntidad;

        public IEnumerable<ArticulosComboModel> GetArticles()
        {
            using (var service = FService.Instance.GetService(typeof (ArticulosModel), Context) as ArticulosService)
            {
                return service.GetAll<ArticulosModel>().Select(f=>new ArticulosComboModel() {Id=f.Id,Descripcion = f.Id +" - " + f.Descripcion}).OrderBy(f=>f.Id);
            }
        }
    }

    public class ArticulosComboModel
    {
        public string Id { get; set; }
        public string Descripcion { get; set; }
    }
}
