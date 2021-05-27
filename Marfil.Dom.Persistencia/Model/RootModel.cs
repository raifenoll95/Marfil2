using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Resources;

namespace Marfil.Dom.Persistencia.Model
{
    public class RootModel
    {
        [Required]
        public string DataBase { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(5)]
        [MaxLength(12)]
        [Display(Name = "Contraseña")]
        [IgnoreDataMember]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(5)]
        [MaxLength(12)]
        [Compare("Password", ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "EqualTo")]
        [Display(Name = "Confirmación contraseña")]
        [IgnoreDataMember]
        public string ConfirmacionPassword { get; set; }
    }
}
