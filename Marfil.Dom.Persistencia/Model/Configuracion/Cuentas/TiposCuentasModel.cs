using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RTiposcuentas=Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Tiposcuentas;
namespace Marfil.Dom.Persistencia.Model.Configuracion.Cuentas
{
    public enum CategoriasCuentas
    {
        [StringValue(typeof(RTiposcuentas), "CategoriasCuentasContables")]
        Contables,
        [StringValue(typeof(RTiposcuentas), "CategoriasCuentasExtracontables")]
        Extracontables
    }

    public enum TiposCuentas
    {
        [StringValue(typeof(RTiposcuentas), "TiposCuentasClientes")]
        Clientes,
        [StringValue(typeof(RTiposcuentas), "TiposCuentasProveedores")]
        Proveedores,
        [StringValue(typeof(RTiposcuentas), "TiposCuentasAcreedores")]
        Acreedores,
        [StringValue(typeof(RTiposcuentas), "TiposCuentasCobradores")]
        Cuentastesoreria,
        [StringValue(typeof(RTiposcuentas), "TiposCuentasTransportistas")]
        Transportistas,
        [StringValue(typeof(RTiposcuentas), "TiposCuentasAgentes")]
        Agentes,
        [StringValue(typeof(RTiposcuentas), "TiposCuentasAseguradoras")]
        Aseguradoras,
        [StringValue(typeof(RTiposcuentas), "TiposCuentasOperarios")]
        Operarios,
        [StringValue(typeof(RTiposcuentas), "TiposCuentasComerciales")]
        Comerciales,
        [StringValue(typeof(RTiposcuentas), "TiposCuentasProspectos")]
        Prospectos,
        Todas = 99
    }

    [Serializable]
    public class TiposCuentasLinModel
    {
        
        [Required]
        public string Empresa { get; set; }

        [Display(ResourceType = typeof(RTiposcuentas), Name = "Tipo")]
        public TiposCuentas Tipo { get; set; }

        [Key]
        [Required]
        [Display(ResourceType = typeof(RTiposcuentas), Name = "Cuenta")]
        public string Cuenta { get; set; }

        [Required]
        [Display(ResourceType = typeof(RTiposcuentas), Name = "Descripcion")]
        public string Descripcion { get; set; }
    }

    public class TiposCuentasModel : BaseModel<TiposCuentasModel, Tiposcuentas>
    {
        #region Properties
        [Required]
        public string Empresa { get; set; }

        [Required]
        [Display(ResourceType = typeof(RTiposcuentas), Name = "Tipo")]
        public TiposCuentas Tipos { get; set; }

        [Required]
        [Display(ResourceType = typeof(RTiposcuentas), Name = "Categoria")]
        public CategoriasCuentas Categoria { get; set; }

        [Display(ResourceType = typeof(RTiposcuentas), Name = "CadenaTipocuenta")]
        public string CadenaTipocuenta {
            get { return Funciones.GetEnumByStringValueAttribute(Tipos); }
        }

        [Required]
        [MaxLength(10, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RTiposcuentas), Name = "Cuenta")]
        public string Cuenta { get; set; }

        [Display(ResourceType = typeof(RTiposcuentas), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(RTiposcuentas), Name = "Nifobligatorio")]
        public bool Nifobligatorio { get; set; }

        private List<TiposCuentasLinModel> _lineas = new List<TiposCuentasLinModel>();
        public IList<TiposCuentasLinModel> Lineas
        {
            get { return _lineas; }
            set { _lineas = value.ToList(); }
        }

        public int TipoInt
        {
            get { return (int) Tipos; }
        }

        #endregion

        #region CTR

        public TiposCuentasModel()
        {
            
        }

        public TiposCuentasModel(IContextService context) : base(context)
        {
            
        }

        #endregion

        public override string GetPrimaryKey()
        {
            return ((int)Tipos).ToString();
        }

        public override object generateId(string id)
        {
            return id;
        }

        public override string DisplayName => RTiposcuentas.TituloEntidad;
    }
}
