using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;
using Marfil.Dom.ControlsUI.Bloqueo;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;
using RFormasPago = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Formaspago;
using Marfil.Inf.Genericos;

namespace Marfil.Dom.Persistencia.Model.FicherosGenerales
{
    [Serializable]
    public class FormasPagoLinModel
    {
        private double _porcentajePago = 0;

        #region Properties
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(ResourceType = typeof(RFormasPago), Name = "DiasVencimiento")]
        public short DiasVencimiento { get; set; }

        [Display(ResourceType = typeof(RFormasPago),Name="PorcentajePago")]
        [Range(0, 100, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "Range")]
        public double PorcentajePago
        {
            get { return _porcentajePago; }
            set { _porcentajePago = value; }
        }

        #endregion

        public FormasPagoLinModel()
        {
            Id = 0;
            DiasVencimiento = 0;
            PorcentajePago = 0;
        }
    }

    public enum Tipodocsventaimprimircuenta
    {
        [StringValue(typeof(RFormasPago), "Ninguna")]
        Ninguna,
        [StringValue(typeof(RFormasPago), "Tercero")]
        Tercero,
        [StringValue(typeof(RFormasPago), "Tesoreria")]
        Tesoreria
    }

    public class FormasPagoModel : BaseModel<FormasPagoModel, FormasPago>
    {
        #region members

        private double _recargoFinanciero = 0.0;

        #endregion

        #region Properties

        [Display(ResourceType = typeof(RFormasPago),Name="Id")]
        [Required]
        public int Id { get; set; }

        [Required]
        [CustomDisplayDescription(typeof(RFormasPago), "Nombre", CustomDisplayDescriptionAttribute.TipoDescripcion.Principal)]
        public string Nombre { get; set; }

        
        [CustomDisplayDescription(typeof(RFormasPago), "Nombre2", CustomDisplayDescriptionAttribute.TipoDescripcion.Secundaria)]
        public string Nombre2 { get; set; }

        [Display(ResourceType = typeof(RFormasPago),Name="ImprimirVencimientoFacturas")]
        public bool ImprimirVencimientoFacturas { get; set; }

        [Display(ResourceType = typeof(RFormasPago), Name = "ExcluirFestivos")]
        public bool ExcluirFestivos { get; set; }

        [Display(ResourceType = typeof(RFormasPago),Name="RecargoFinanciero")]
        [Range(0, 100, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "Range")]
        public double RecargoFinanciero
        {
            get { return _recargoFinanciero; }
            set { _recargoFinanciero = value; }
        }

        public bool Efectivo { get; set; }
        public bool Remesable { get; set; }
        public bool Mandato { get; set; }
       

        [Display(ResourceType = typeof(RFormasPago),Name="ModoPago")]
        public string ModoPago { get; set; }

        public BloqueoEntidadModel BloqueoModel { get; set; }
        public bool Bloqueado
        {
            get { return BloqueoModel?.Bloqueada ?? false; }
        }

        [Required]
        [Display(ResourceType = typeof(RFormasPago), Name = "FkGruposformaspago")]
        public string FkGruposformaspago { get; set; }
        public string Gruposformaspago { get; set;}

        [Display(ResourceType = typeof(RFormasPago), Name = "Docsventaimprimircuenta")]
        public Tipodocsventaimprimircuenta Docsventaimprimircuenta { get; set; }

        public IEnumerable<TablasVariasModosPagoModel> ListModosPago { get; set; } 

        private List<FormasPagoLinModel> _lineas = new List<FormasPagoLinModel>();
        

        public IEnumerable<FormasPagoLinModel> Lineas
        {
            get { return _lineas; }
            set { _lineas = value.ToList(); }
        }

        public int NumeroVencimientos => Lineas.Count();

        public override void createNewPrimaryKey()
        {
            primaryKey = getProperties().Where(f => f.property.Name == "Id").Select(f => f.property);
        }



        #endregion

        #region CTR

        public FormasPagoModel()
        {

        }

        public FormasPagoModel(IContextService context) : base(context)
        {

        }

        #endregion

        public override string DisplayName => RFormasPago.TituloEntidad;

        public override object generateId(string id)
        {
            return int.Parse(id);
        }

       
    }
}
