using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Resources;

namespace Marfil.Dom.ControlsUI.NifCif
{
    public class NifCifModel
    {
        private readonly Guid _id;
        public NifCifModel()
        {
            _id = Guid.NewGuid();
        }

        public string Id => _id.ToString().Replace("-", "");

        [Display(Name = "NIF/CIF")]
        [MaxLength(15, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        public string Nif { get; set; }
        public string FkpaisControlId { get; set; }
        public bool Readonly { get; set; }
        public bool Obligatorio { get; set; }
        public string TipoNif { get; set; }
        public override string ToString()
        {
            return Nif;
        }
    }
}
