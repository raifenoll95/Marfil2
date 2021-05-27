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
using RProvincias = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Provincias;
namespace Marfil.Dom.Persistencia.Model.FicherosGenerales
{
    public class ProvinciasModel : BaseModel<ProvinciasModel, Provincias>
    {
        private string _id;
        private const char Separator = '-';

        #region Properties

        public string CustomId
        {
            get { return string.Format("{0}-{1}", Codigopais, Id); }
        }

        [Required]
        [Display(ResourceType = typeof(RProvincias), Name = "Codigopais")]
        public string Codigopais { get; set; }

        [Display(ResourceType = typeof(RProvincias), Name = "DescripcionPais")]
        public string DescripcionPais { get; set; }

        [Required]
        [Display(ResourceType = typeof(RProvincias),Name="Id")]
        [MaxLength(2, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [MinLength(1, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MinLength")]
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [Display(ResourceType = typeof(RProvincias), Name = "Nombre")]
        public string Nombre { get; set; }

        #endregion

        #region CTR

        public ProvinciasModel()
        {

        }

        public ProvinciasModel(IContextService context) : base(context)
        {

        }

        #endregion

        public string GetCodigo()
        {
            return Codigopais + Separator + Id;

        }

        public override string GetPrimaryKey()
        {
            return GetCodigo();
        }

        public override object generateId(string id)
        {
            return id.Split(Separator);
        }

        public override string DisplayName => RProvincias.TituloEntidad;
    }
}
