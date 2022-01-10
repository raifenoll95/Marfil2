using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos;
using Resources;
using REjecicios=Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Ejercicios;
namespace Marfil.Dom.Persistencia.Model.Configuracion
{
    public enum EstadoEjercicio
    {
        [StringValue(typeof(REjecicios),"EstadoEjercicioAbierto")]
        Abierto,
        [StringValue(typeof(REjecicios), "EstadoEjercicioExistencias")]
        Existencias,
        [StringValue(typeof(REjecicios), "EstadoEjercicioGrupos")]
        Grupos,
        [StringValue(typeof(REjecicios), "EstadoEjercicioCerrado")]
        Cerrado
    }

    public class EjerciciosModel : BaseModel<EjerciciosModel, Ejercicios>
    {
        private List<ContadoresLinModel> _lineas=new List<ContadoresLinModel>();

        #region Properties

        public string Empresa { get; set; }

        [Required]
        [Display(ResourceType = typeof(REjecicios), Name = "Id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(120, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(REjecicios), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Required]
        [MaxLength(4, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(REjecicios), Name = "Descripcioncorta")]
        public string Descripcioncorta { get; set; }

        [Required]
        [Display(ResourceType = typeof(REjecicios), Name = "Desde")]
        public DateTime? Desde { get; set; }

        [Required]
        [Display(ResourceType = typeof(REjecicios), Name = "Hasta")]
        public DateTime? Hasta { get; set; }

        [Display(ResourceType = typeof(REjecicios), Name = "FkseriescontablesAST")]
        public string FkseriescontablesAST { get; set; }

        [Display(ResourceType = typeof(REjecicios), Name = "FkseriescontablesIVS")]
        public string FkseriescontablesIVS { get; set; }

        [Display(ResourceType = typeof(REjecicios), Name = "FkseriescontablesIVP")]
        public string FkseriescontablesIVP { get; set; }

        [Display(ResourceType = typeof(REjecicios), Name = "FkseriescontablesPRC")]
        public string FkseriescontablesPRC { get; set; }

        [Display(ResourceType = typeof(REjecicios), Name = "FkseriescontablesPRP")]
        public string FkseriescontablesPRP { get; set; }

        [Display(ResourceType = typeof(REjecicios), Name = "FkseriescontablesCRC")]
        public string FkseriescontablesCRC { get; set; }

        [Display(ResourceType = typeof(REjecicios), Name = "FkseriescontablesCRP")]
        public string FkseriescontablesCRP { get; set; }

        [Display(ResourceType = typeof(REjecicios), Name = "FkseriescontablesREM")]
        public string FkseriescontablesREM { get; set; }

        [Display(ResourceType = typeof(REjecicios), Name = "FkseriescontablesINM")]
        public string FkseriescontablesINM { get; set; }
        public string DescSerieContable { get; set; }
        public string DesdeCadena {
            get { return Desde?.ToShortDateString().ToString(CultureInfo.CurrentUICulture) ?? string.Empty; }
        }

        public string HastaCadena
        {
            get { return Hasta?.ToShortDateString().ToString(CultureInfo.CurrentUICulture) ?? string.Empty; }
        }

        [Display(ResourceType = typeof(REjecicios), Name = "Estado")]
        public EstadoEjercicio Estado { get; set; }
        
        [Display(ResourceType = typeof(REjecicios), Name = "Contabilidadcerradahasta")]
        public DateTime? Contabilidadcerradahasta { get; set; }
        
        [Display(ResourceType = typeof(REjecicios), Name = "Registroivacerradohasta")]
        public DateTime? Registroivacerradohasta { get; set; }

        [Display(ResourceType = typeof(REjecicios), Name = "Criterioiva")]
        public int? Criterioiva { get; set; }

        public CriterioIVA? CustomCriterioIva
        {
            get { return (CriterioIVA?) Criterioiva; }
            set { Criterioiva = (int?) value; }
        }
        [Display(ResourceType = typeof(REjecicios), Name = "Fkejercicios")]
        public int? Fkejercicios { get; set; }

        #endregion

        #region CTR

        public EjerciciosModel()
        {

        }

        public EjerciciosModel(IContextService context) : base(context)
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

        public override string DisplayName => REjecicios.TituloEntidad;
    }
}
