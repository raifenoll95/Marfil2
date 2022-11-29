using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RMovs = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Movs;
using RTiposFacturas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.TiposFacturasIva;
using Marfil.Inf.Genericos;
using System.ComponentModel.DataAnnotations;

namespace Marfil.Dom.Persistencia.Model.Iva
{
    public enum TipoImporte
    {
        [StringValue(typeof(RTiposFacturas), "Blanco")]
        Blanco,
        [StringValue(typeof(RTiposFacturas), "BaseImponibleInterior")]
        BaseImponibleInterior,
        [StringValue(typeof(RTiposFacturas), "BaseImponibleImportacion")]
        BaseImponibleImportacion,
        [StringValue(typeof(RTiposFacturas), "OperacionesExcluidas")]
        OperacionesExcluidas,
        [StringValue(typeof(RTiposFacturas), "CuotaIvaInterior")]
        CuotaIvaInterior,
        [StringValue(typeof(RTiposFacturas), "CuotaIvaImportacion")]
        CuotaIvaImportacion,
        [StringValue(typeof(RTiposFacturas), "Importeirpf")]
        Importeirpf,
        [StringValue(typeof(RTiposFacturas), "Descuentos")]
        Descuentos,
        [StringValue(typeof(RTiposFacturas), "InteresesIncluidos")]
        InteresesIncluidos,
        [StringValue(typeof(RTiposFacturas), "TotalFactura")]
        TotalFactura,
        [StringValue(typeof(RTiposFacturas), "Formula")]
        Formula
    }

    public enum TipoFactura
    {
        [StringValue(typeof(RTiposFacturas), "Soportado")]
        Soportado,
        [StringValue(typeof(RTiposFacturas), "Repercutido")]
        Repercutido
    }

    public enum TipoCuenta
    {
        [StringValue(typeof(RTiposFacturas), "Blanco")]
        Blanco,
        [StringValue(typeof(RTiposFacturas), "Iva")]
        Iva,
        [StringValue(typeof(RTiposFacturas), "Retencion")]
        Retencion,
        [StringValue(typeof(RTiposFacturas), "General")]
        General
    }

    public class TiposFacturasIvaModel : BaseModel<TiposFacturasIvaModel, TiposFacturas>
    {
        #region CTR
        public TiposFacturasIvaModel()
        {
        }

        public TiposFacturasIvaModel(IContextService context) : base(context)
        {
        }
        #endregion

        #region properties

        public int? Id { get; set; }

        public string Empresa { get; set; }

        [Required]
        [Display(ResourceType = typeof(RTiposFacturas), Name = "Tipocircuito")]
        public TipoFactura Tipocircuito { get; set; }

        [Display(ResourceType = typeof(RTiposFacturas), Name = "Tipocuenta")]
        public TipoCuenta Tipocuenta { get; set; }

        [Display(ResourceType = typeof(RTiposFacturas), Name = "Tipocuenta")]
        public TipoCuenta Tipocuenta3 { get; set; }

        [Display(ResourceType = typeof(RTiposFacturas), Name = "Tipocuenta")]
        public TipoCuenta Tipoabono2 { get; set; }

        [Display(ResourceType = typeof(RTiposFacturas), Name = "Tipocuenta")]
        public TipoCuenta Tipoabono3 { get; set; }

        [Display(ResourceType = typeof(RTiposFacturas), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(RTiposFacturas), Name = "Regimeniva")]
        public string Regimeniva { get; set; }

        [Display(ResourceType = typeof(RTiposFacturas), Name = "Tipofacturadefecto")]
        public bool Tipofacturadefecto { get; set; }

        [Display(ResourceType = typeof(RTiposFacturas), Name = "Ivadeducible")]
        public bool Ivadeducible { get; set; }

        [Display(ResourceType = typeof(RTiposFacturas), Name = "Requiereirpf")]
        public bool Requiereirpf { get; set; }

        [Display(ResourceType = typeof(RTiposFacturas), Name = "Operacionesexcluidas2")]
        public bool Operacionesexcluidas { get; set; }

        [Display(ResourceType = typeof(RTiposFacturas), Name = "Bieninversion")]
        public bool Bieninversion { get; set; }

        [Display(ResourceType = typeof(RTiposFacturas), Name = "Cuentacargo1")]
        public string Cuentacargo1 { get; set; }

        [Display(ResourceType = typeof(RTiposFacturas), Name = "Cuentacargo2")]
        public string Cuentacargo2 { get; set; }

        [Display(ResourceType = typeof(RTiposFacturas), Name = "Cuentacargo3")]
        public string Cuentacargo3 { get; set; }

        [Display(ResourceType = typeof(RTiposFacturas), Name = "Cuentaabono1")]
        public string Cuentaabono1 { get; set; }

        [Display(ResourceType = typeof(RTiposFacturas), Name = "Cuentaabono2")]
        public string Cuentaabono2 { get; set; }

        [Display(ResourceType = typeof(RTiposFacturas), Name = "Cuentaabono3")]
        public string Cuentaabono3 { get; set; }

        public TipoImporte Importecuentacargo1 { get; set; }

        public TipoImporte Importecuentacargo2 { get; set; }

        public TipoImporte Importecuentaabono1 { get; set; }

        public TipoImporte Importecuentaabono2 { get; set; }

        public TipoImporte Importecuentacargo3 { get; set; }

        public TipoImporte Importecuentaabono3 { get; set; }

        public string Desccuentacargo1 { get; set; }

        public string Desccuentacargo2 { get; set; }

        public string Desccuentacargo3 { get; set; }

        public string Desccuentaabono1 { get; set; }

        public string Desccuentaabono2 { get; set; }

        public string Desccuentaabono3 { get; set; }

        public string Formulacargo1 { get; set; }

        public string Formulacargo2 { get; set; }

        public string Formulacargo3 { get; set; }

        public string Formulaabono1 { get; set; }

        public string Formulaabono2 { get; set; }

        public string Formulaabono3 { get; set; }

        #endregion

        #region atributos

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

        public override string DisplayName => RTiposFacturas.TituloEntidad;

        #endregion
    }
}
