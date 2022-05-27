using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RRegistroIVA = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.RegistroIvaRepercutido;
using Marfil.Inf.Genericos;
using System.ComponentModel.DataAnnotations;

namespace Marfil.Dom.Persistencia.Model.Iva
{
    public enum TipoOrigen
    {
        [StringValue(typeof(RRegistroIVA), "Factura")]
        Factura,
        [StringValue(typeof(RRegistroIVA), "IVA")]
        IVA
    }

    public enum TipoCriterioIva
    {
        [StringValue(typeof(RRegistroIVA), "Devengo")]
        Devengo,
        [StringValue(typeof(RRegistroIVA), "Caja")]
        Caja
    }

    public class RegistroIvaRepercutidoModel : BaseModel<RegistroIvaRepercutidoModel, RegistroIVARepercutido>
    {
        #region CTR
        public RegistroIvaRepercutidoModel()
        {
        }

        public RegistroIvaRepercutidoModel(IContextService context) : base(context)
        {
        }
        #endregion

        #region properties

        [Required]
        public int? Id { get; set; }

        [Required]
        public string Empresa { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Origendoc")]
        public TipoOrigen Origendoc { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Tipofactura")]
        public string Tipofactura { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Referencia")]
        public string Referencia { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Fecharegistro")]
        public DateTime Fecharegistro { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Fechafactura")]
        public DateTime Fechafactura { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Fechaoperacion")]
        public DateTime Fechaoperacion { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Periodo")]
        public string Periodo { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Numfacturacliente")]
        public string Numfacturacliente { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Cuentacliente")]
        public string Cuentacliente { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Cuentaventas")]
        public string Cuentaventas { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Criterioivageneral")]
        public TipoCriterioIva Criterioivageneral { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Fkregimeniva")]
        public string Fkregimeniva { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Facturarectificativa")]
        public bool Facturarectificativa { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Numfacturarectificar")]
        public string Numfacturarectificar { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Fechafacturaoriginal")]
        public DateTime Fechafacturaoriginal { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Motivorectificacion")]
        public string Motivorectificacion { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Criterioivafactoriginal")]
        public TipoCriterioIva Criterioivafactoriginal { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Criterioivafactura")]
        public TipoCriterioIva Criterioivafactura { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Bieninversion")]
        public bool Bieninversion { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Fktiporetencion")]
        public string Fktiporetencion { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Baseretencion")]
        public double Baseretencion { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Porcentajeretencion")]
        public double Porcentajeretencion { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Importeretencion")]
        public double Importeretencion { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Inmueble")]
        public string Inmueble { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Contabilizar")]
        public bool Contabilizar { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Fkcuentastesoreria")]
        public string Fkcuentastesoreria { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Operacionesexluidasbi")]
        public double Operacionesexluidasbi { get; set; }

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

        public override string DisplayName => RRegistroIVA.TituloEntidad;

        #endregion

    }
}
