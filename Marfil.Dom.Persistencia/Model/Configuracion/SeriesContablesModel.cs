using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.ControlsUI.Bloqueo;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos;
using Resources;
using RSeriesContables = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.SeriesContables;
namespace Marfil.Dom.Persistencia.Model.Configuracion
{

    
    
    public class SeriesContablesSerializableModel
    {
        public SeriesContablesSerializableModel()
        {

        }


        public SeriesContablesSerializableModel(SeriesContablesModel model)
        {
            var list = typeof(SeriesContablesSerializableModel).GetProperties();
            foreach (var item in list)
            {
                item.SetValue(this, model.get(item.Name));
            }
        }

        public string Empresa { get; set; }

        [Required]
        [Display(ResourceType = typeof(RSeriesContables), Name = "TipoDocumento")]
        public string Tipodocumento { get; set; }

        [Required]
        [MaxLength(3, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RSeriesContables), Name = "Id")]
        public string Id { get; set; }

        [Required]
        [MaxLength(40, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RSeriesContables), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(RSeriesContables), Name = "Fkmonedas")]
        public int? Fkmonedas { get; set; }

       [Display(ResourceType = typeof(RSeriesContables), Name = "Fkcontadores")]
        public string Fkcontadores { get; set; }

        [Display(ResourceType = typeof(RSeriesContables), Name = "Fkejercicios")]
        public string Fkejercicios { get; set; }


        [Display(ResourceType = typeof(RSeriesContables), Name = "Fkejercicios")]
        public string Fkejerciciosdescripcion { get; set; }
    }

    public class SeriesContablesModel : BaseModel<SeriesContablesModel, SeriesContables>
    {
        #region CTR

        public SeriesContablesModel()
        {

        }

        public SeriesContablesModel(IContextService context) : base(context)
        {

        }


        public SeriesContablesModel(SeriesContablesSerializableModel model)
        {
            var list = typeof(SeriesContablesSerializableModel).GetProperties();
            foreach (var item in list)
            {
                GetType().GetProperty(item.Name).SetValue(this, model.GetType().GetProperty(item.Name).GetValue(model));
            }
        }

        #endregion

        #region Properties

        public string CustomId
        {
            get { return string.Format("{0}-{1}", Tipodocumento, Id); }
            //get { return Id; }
        }

        public string Empresa { get; set; }

        [Required]
        [Display(ResourceType = typeof(RSeriesContables), Name = "TipoDocumento")]
        public string Tipodocumento { get; set; }

        [Required]
        [MaxLength(3, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RSeriesContables), Name = "Id")]
        public string Id { get; set; }

        [Required]
        [MaxLength(40, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RSeriesContables), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(RSeriesContables), Name = "Fkmonedas")]
        public int? Fkmonedas { get; set; }


        [Display(ResourceType = typeof(RSeriesContables), Name = "Fkcontadores")]
        public string Fkcontadores { get; set; }

        [Display(ResourceType = typeof(RSeriesContables), Name = "Fkejercicios")]
        public string Fkejercicios { get; set; }


        [Display(ResourceType = typeof(RSeriesContables), Name = "Fkejercicios")]
        public string Fkejerciciosdescripcion { get; set; }


//        [Display(ResourceType = typeof(RSeriesContables), Name = "Tipoimpresion")]
//        public TipoImpresion Tipoimpresion { get; set; }


        public BloqueoEntidadModel Bloqueo { get; set; }

        [Display(ResourceType = typeof(RSeriesContables), Name = "Bloqueo")]
        public bool Bloqueado
        {
            get { return Bloqueo?.Bloqueada ?? false; }
        }



        #endregion

        public override string GetPrimaryKey()
        {
            return Tipodocumento + "-" + Id;
            //return Id;
        }

        public override object generateId(string id)
        {
            return id.Split('-');
            //return id;
        }

        public override string DisplayName => RSeriesContables.TituloEntidad;

    }
}
