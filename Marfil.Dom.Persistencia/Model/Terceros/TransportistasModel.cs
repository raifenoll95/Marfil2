using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;
using RTransportistas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Transportistas;
namespace Marfil.Dom.Persistencia.Model.Terceros
{
    public class TransportistasModel:AcreedoresModel
    {
        public override string DisplayName => RTransportistas.TituloEntidad;

        #region Propiedades

        [Display(ResourceType = typeof(RTransportistas), Name = "Conductorhabitual")]
        [MaxLength(30,ErrorMessageResourceType = typeof(Unobtrusive),ErrorMessageResourceName = "MaxLength")]
        public string Conductorhabitual { get; set; }

        [Display(ResourceType = typeof(RTransportistas), Name = "Matricula")]
        [MaxLength(12, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        public string Matricula { get; set; }

        [Display(ResourceType = typeof(RTransportistas), Name = "Remolque")]
        [MaxLength(12, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        public string Remolque { get; set; }

        [Display(ResourceType = typeof(RTransportistas), Name = "Tipotransporte")]
        public string Tipotransporte { get; set; }

        #endregion

        #region CTR

        public TransportistasModel(IContextService context) : base(context)
        {
            
        }

        public TransportistasModel()
        {
            
        }

        #endregion
    }
}
