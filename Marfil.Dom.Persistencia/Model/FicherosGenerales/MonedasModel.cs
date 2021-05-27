using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;
using RMonedas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Monedas;
namespace Marfil.Dom.Persistencia.Model.FicherosGenerales
{
    [Serializable]
    public class CambioMonedasLogModel
    {
        [Display(ResourceType = typeof(RMonedas), Name = "Id")]
        public int Id { get; set; }

        [Display(ResourceType = typeof(RMonedas), Name = "CambioMonedaBase")]
        public double CambioMonedaBase { get; set; }

        [Display(ResourceType = typeof(RMonedas), Name = "CambioMonedaAdicional")]
        public double CambioMonedaAdicional { get; set; }

        [Display(ResourceType = typeof(RMonedas), Name = "FechaModificacion")]
        public DateTime FechaModificacion { get; set; }

        public Guid UsuarioId { get; set; }

        [Display(ResourceType = typeof(RMonedas), Name = "Usuario")]
        public string Usuario { get; set; }

       
    }

    public class MonedasModel : BaseModel<MonedasModel, Monedas>
    {
        #region Properties

        [Key]
        [Required]
        [Display(ResourceType=typeof(RMonedas),Name = "Id")]
        public int Id { get; set; }

        [Required]
        [Display(ResourceType = typeof(RMonedas), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Required]
        [MaxLength(3, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [MinLength(3, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MinLength")]
        [Display(ResourceType = typeof(RMonedas), Name = "Abreviatura")]
        public string Abreviatura { get; set; }

        [Required]
        [Display(ResourceType = typeof(RMonedas), Name = "Decimales")]
        public int Decimales { get; set; }

        [Required]
        [Display(ResourceType = typeof(RMonedas), Name = "CambioMonedaBase")]
        public double CambioMonedaBase { get; set; }

        [Required]
        [Display(ResourceType = typeof(RMonedas), Name = "CambioMonedaAdicional")]
        public double CambioMonedaAdicional { get; set; }

        [Display(ResourceType = typeof(RMonedas), Name = "Activado")]
        public bool Activado { get; set; }

        private List<CambioMonedasLogModel> _log = new List<CambioMonedasLogModel>();
        public IEnumerable<CambioMonedasLogModel> Log
        {
            get { return _log; }
            set { _log = value.ToList(); }
        }

        [Display(ResourceType = typeof(RMonedas), Name = "FechaModificacion")]
        public DateTime FechaModificacion { get; set; }

        [Display(ResourceType = typeof(RMonedas), Name = "Usuario")]
        public string Usuario { get; set; }

        public Guid UsuarioId { get; set; }
        #endregion

        #region CTR

        public MonedasModel()
        {

        }

        public MonedasModel(IContextService context) : base(context)
        {

        }

        #endregion

        public override object generateId(string id)
        {
            return int.Parse(id);
        }

        public override string DisplayName => RMonedas.TituloEntidad;
    }
}
