using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;
using RMunicipios = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Municipios;
using System.Web.Mvc;

namespace Marfil.Dom.Persistencia.Model.FicherosGenerales
{
    public class MunicipiosModel : BaseModel<MunicipiosModel, Municipios>
    {
        private string _cod;
        private const char Separator = '-';

        #region Properties
        
        public string CustomId
        {
            get { return string.Format("{0}-{1}-{2}", Codigopais, Codigoprovincia, Cod); }
        }

        [Required]
        [Display(ResourceType = typeof(RMunicipios), Name = "Codigopais")]
        public string Codigopais { get; set; }

        [Display(ResourceType = typeof(RMunicipios), Name = "DescripcionPais")]
        public string DescripcionPais { get; set; }

        [Required]
        [Display(ResourceType = typeof(RMunicipios), Name = "Codigoprovincia")]
        public string Codigoprovincia { get; set; }

        [Display(ResourceType = typeof(RMunicipios), Name = "DescripcionProvincia")]
        public string DescripcionProvincia { get; set; }

        //[Required]
        [Display(ResourceType = typeof(RMunicipios), Name = "Id")]
        [MaxLength(3, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [MinLength(1, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MinLength")]
        public string Cod
        {
            get { return _cod; }
            set { _cod = value; }
        }

        [Display(ResourceType = typeof(RMunicipios), Name = "Id")]
        public string codigoMunicipioCompleto
        {
            get { return Codigoprovincia + Cod; }
        }

        [Display(ResourceType = typeof(RMunicipios), Name = "Nombre")]
        public string Nombre { get; set; }

        #endregion

        #region CTR

        public MunicipiosModel()
        {

        }

        public MunicipiosModel(IContextService context) : base(context)
        {

        }

        #endregion

        public string GetCodigo()
        {
            return Codigopais + Separator + Codigoprovincia + Separator + Cod;
        }

        public override string GetPrimaryKey()
        {
            return GetCodigo();
        }

        public override object generateId(string id)
        {
            return id.Split(Separator);
        }

        public override string DisplayName => RMunicipios.TituloEntidad;
    }
}
