using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RInmuebles = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Inmuebles;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos;
using System.ComponentModel;

namespace Marfil.Dom.Persistencia.Model.Configuracion.Inmueble
{
    public class InmueblesModel : BaseModel<InmueblesModel, Inmuebles>
    {

        #region CTR

        public InmueblesModel()
        {

        }

        public InmueblesModel(IContextService context) : base(context)
        {

        }

        #endregion

        public enum TipoSituacion
        {
            [StringValue(typeof(RInmuebles), "ConEspaña")]
            ConEspaña,
            [StringValue(typeof(RInmuebles), "ConPaisVasco")]
            ConPaisVasco,
            [StringValue(typeof(RInmuebles), "SinEspaña")]
            SinEspaña,
            [StringValue(typeof(RInmuebles), "Extranjero")]
            Extranjero
        }

        #region properties

        [Required]
        public string Empresa { get; set; }

        [Required]
        [Display(ResourceType = typeof(RInmuebles), Name = "Id")]
        [MaxLength(4, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        public string Id { get; set; }

        [Display(ResourceType = typeof(RInmuebles), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(RInmuebles), Name = "RefCatastral")]
        public string RefCatastral { get; set; }

        [Display(ResourceType = typeof(RInmuebles), Name = "Situacion")]
        /*public string[] Situacion = {
            "Inmueble CON ref. catastral situado en España, excepto País Vasco y Navarra",
            "Inmueble CON ref. catastral situado en el País Vasco y Navarra",
            "Inmueble SIN ref. catastral situado en España",
            "Inmueble situado en el extranjero"
        };*/
        public TipoSituacion Situacion { get; set; }

        [Display(ResourceType = typeof(RInmuebles), Name = "FkPais")]
        public string FkPais { get; set; }

        [Display(ResourceType = typeof(RInmuebles), Name = "DescripcionPais")]
        public string DescripcionPais { get; set; }

        [Display(ResourceType = typeof(RInmuebles), Name = "FkProvinciaCod")]
        public string FkProvinciaCod { get; set; }

        [Display(ResourceType = typeof(RInmuebles), Name = "DescripcionProvincia")]
        public string DescripcionProvincia { get; set; }

        [Display(ResourceType = typeof(RInmuebles), Name = "FkMunicipioCod")]
        public string FkMunicipioCod { get; set; }

        [Display(ResourceType = typeof(RInmuebles), Name = "FkMunicipioNom")]
        public string FkMunicipioNom { get; set; }

        [Display(ResourceType = typeof(RInmuebles), Name = "CodPostal")]
        public string CodPostal { get; set; }

        [Display(ResourceType = typeof(RInmuebles), Name = "FkTipoVia")]
        public string FkTipoVia { get; set; }

        [Display(ResourceType = typeof(RInmuebles), Name = "NombreVia")]
        public string NombreVia { get; set; }

        [Display(ResourceType = typeof(RInmuebles), Name = "TipoNumeracion")]
        public string TipoNumeracion { get; set; }

        [Display(ResourceType = typeof(RInmuebles), Name = "NumCasa")]
        public string NumCasa { get; set; }

        [Display(ResourceType = typeof(RInmuebles), Name = "CalifNumero")]
        public string CalifNumero { get; set; }

        [Display(ResourceType = typeof(RInmuebles), Name = "Bloque")]
        public string Bloque { get; set; }

        [Display(ResourceType = typeof(RInmuebles), Name = "Portal")]
        public string Portal { get; set; }

        [Display(ResourceType = typeof(RInmuebles), Name = "Escalera")]
        public string Escalera { get; set; }

        [Display(ResourceType = typeof(RInmuebles), Name = "Planta")]
        public string Planta { get; set; }

        [Display(ResourceType = typeof(RInmuebles), Name = "Puerta")]
        public string Puerta { get; set; }

        [Display(ResourceType = typeof(RInmuebles), Name = "Complemento")]
        public string Complemento { get; set; }

        [Display(ResourceType = typeof(RInmuebles), Name = "PoblaDiferente")]
        public string PoblaDiferente { get; set; }

        public override string DisplayName => RInmuebles.TituloEntidad;

        public override object generateId(string id)
        {
            return id;
        }

        #endregion
    }
}
