using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Inf.ResourcesGlobalization.Textos.Entidades;

namespace Marfil.Dom.ControlsUI.ConvertirProspectosClientes
{
    public class ConvertirProspectoClienteModel
    {
        [Required]
        [Display(ResourceType = typeof(Prospectos), Name = "TituloEntidadSingular")]
        public string ProspectoId { get; set; }

        [Required]
        [Display(ResourceType = typeof(Clientes), Name = "TituloEntidadSingular")]
        public string ClienteId { get; set; }
    }
}
