using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.ControlsUI.Bloqueo
{
    public class BloqueoEntidadModel
    {
        public string FkUsuario { get; set; }
        public string FkUsuarioNombre { get; set; }
        public DateTime Fecha { get; set; }
        [Display(Name="Bloqueada")]
        public bool Bloqueada { get; set; }
        [Display(Name = "Motivo de bloqueo")]
        public string FkMotivobloqueo { get; set; }
        public string FkMotivobloqueoDescripcion { get; set; }
        public string Descripcionbloqueo {
            get
            {
                var descripcionBloqueo = string.Empty;
                if (!string.IsNullOrEmpty(FkMotivobloqueoDescripcion) && !string.IsNullOrEmpty(FkUsuario))
                {
                    descripcionBloqueo = string.Format("{0}Usuario:{1}, Fecha de {2}:{3}",
                        Bloqueada ? "<strong>" + FkMotivobloqueoDescripcion + "</strong>,<br/>" : string.Empty,
                        FkUsuarioNombre,
                        Bloqueada ? "bloqueo" : "desbloqueo",
                        Fecha.ToShortDateString());
                }

                return descripcionBloqueo;
            }
        }
        public string Entidad { get; set; }

        public bool Readonly { get; set; }

        public string Campoclaveprimaria { get; set; }
        public string Valorclaveprimaria { get; set; }

        public string Controller { get; set; }
    }
}
