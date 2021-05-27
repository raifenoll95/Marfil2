using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.ResourcesGlobalization.Textos.Entidades;
using Resources;
using RObras= Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Obras;
namespace Marfil.Dom.Persistencia.Model.FicherosGenerales
{
    public class ObrasModel : BaseModel<ObrasModel, Obras>
    {
        #region Properties
        [Required]
        public string Empresa { get; set; }
        
        [Required]
        [Display(ResourceType=typeof(RObras),Name = "Id")]
        public int Id { get; set; }

        [Required]
        [Display(ResourceType = typeof(RObras), Name = "Fkclientes")]
        public string Fkclientes { get; set; }

        [Display(ResourceType = typeof(RObras), Name = "Nombrecliente")]
        public string Nombrecliente { get; set; }

        [Required]
        [Display(ResourceType = typeof(RObras), Name = "Nombreobra")]
        public string Nombreobra { get; set; }
        
        [Display(ResourceType = typeof(RObras), Name = "Direccion")]
        public int? Fkdirecciones { get; set; }

        [Display(ResourceType = typeof(RObras), Name = "Fkagentes")]
        public string Fkagentes { get; set; }

        [Display(ResourceType = typeof(RObras), Name = "Fkcomerciales")]
        public string Fkcomerciales { get; set; }

        [Display(ResourceType = typeof(RObras), Name = "Fkregimeniva")]
        public string Fkregimeniva { get; set; }

        [Range(0,100,ErrorMessageResourceType = typeof(Unobtrusive),ErrorMessageResourceName = "RangeClient")]
        [Display(ResourceType = typeof(RObras), Name = "Retencion")]
        public double? Retencion { get; set; }

        [Display(ResourceType = typeof(RObras), Name = "Fechainicio")]
        public DateTime? Fechainicio { get; set; }

        [Display(ResourceType = typeof(RObras), Name = "Fechafin")]
        public DateTime? Fechafin { get; set; }

        [Display(ResourceType = typeof(RObras), Name = "Fecharevision")]
        public DateTime? Fecharevision { get; set; }

        [Display(ResourceType = typeof(RObras), Name = "Fechainicio")]
        public string Fechainiciocadena
        {
            get { return Fechainicio?.ToShortDateString().ToString(CultureInfo.InvariantCulture); }
        }

        [Display(ResourceType = typeof(RObras), Name = "Fechafin")]
        public string Fechafincadena
        {
            get { return Fechafin?.ToShortDateString().ToString(CultureInfo.InvariantCulture); }
        }
        [Display(ResourceType = typeof(RObras), Name = "Fktiposobra")]
        public string Fktiposobra { get; set; }
        [Display(ResourceType = typeof(RObras), Name = "Certificado")]
        public string Certificado { get; set; }

        [Display(ResourceType = typeof(RObras), Name = "Notas")]
        public string Notas { get; set; }

        [Display(ResourceType = typeof(RObras), Name = "Finalizada")]
        public bool? Finalizada { get; set; }
        [Display(ResourceType = typeof(RObras), Name = "Finalizada")]
        public bool Finalizadavista
        {
            get { return Finalizada ?? false; }
            set { Finalizada = value; }
        }
        #endregion

        #region CTR

        public ObrasModel()
        {

        }

        public ObrasModel(IContextService context) : base(context)
        {

        }

        #endregion

        public override object generateId(string id)
        {
            return int.Parse(id);
        }

        public override void createNewPrimaryKey()
        {
            primaryKey = new[] { GetType().GetProperty("Id") };
        }

        public override string DisplayName => RObras.TituloEntidad;
    }
}
