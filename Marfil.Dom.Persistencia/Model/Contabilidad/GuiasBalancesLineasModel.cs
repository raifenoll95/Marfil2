using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RGuiasBLineas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.GuiasBalancesLineas;

namespace Marfil.Dom.Persistencia.Model.Contabilidad
{
    public class GuiasBalancesLineasModel : BaseModel<GuiasBalancesLineasModel, GuiasBalancesLineas>
    {
        #region Propiedades
        public int Id { get; set; }

        [Display(ResourceType = typeof(RGuiasBLineas), Name = "informe")]
        public int InformeId { get; set; }

        [Display(ResourceType = typeof(RGuiasBLineas), Name = "informe")]
        public TipoInforme TipoInforme { get; set; }

        [Display(ResourceType = typeof(RGuiasBLineas), Name = "guia")]
        public int GuiaId { get; set; }

        [Display(ResourceType = typeof(RGuiasBLineas), Name = "guia")]
        public TipoGuia TipoGuia { get; set; }

        [Display(ResourceType = typeof(RGuiasBLineas), Name = "GuiasBalancesId")]
        public int? GuiasBalancesId { get; set; }

        [Display(ResourceType = typeof(RGuiasBLineas), Name = "orden")]
        public string orden { get; set; }

        [Display(ResourceType = typeof(RGuiasBLineas), Name = "cuenta")]
        public string cuenta { get; set; }

        [Display(ResourceType = typeof(RGuiasBLineas), Name = "signo")]
        public string signo { get; set; }

        [Display(ResourceType = typeof(RGuiasBLineas), Name = "signoea")]
        public string signoea { get; set; }
        #endregion

        public GuiasBalancesLineasModel()
        {

        }

        public GuiasBalancesLineasModel(IContextService context) : base(context) { }

        public override string DisplayName => "Guias de Balances Lineas";

        public override object generateId(string id)
        {
            return Id;
        }
    }
}
