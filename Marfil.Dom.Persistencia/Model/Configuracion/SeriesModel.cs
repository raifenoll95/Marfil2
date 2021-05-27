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

using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;

using RSeries=Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Series;
namespace Marfil.Dom.Persistencia.Model.Configuracion
{
   

    public enum TipoImpresion
    {
        [StringValue(typeof(RSeries), "TiposSegmentosSecuencia")]
        Secuencia
    }

    public class SeriesSerializableModel
    {
        public SeriesSerializableModel()
        {
            
        }


        public SeriesSerializableModel(SeriesModel model)
        {
            var list = typeof (SeriesSerializableModel).GetProperties();
            foreach (var item in list)
            {
                item.SetValue(this,model.get(item.Name));
            }
        }

        public string Empresa { get; set; }

        [Required]
        [Display(ResourceType = typeof(RSeries), Name = "Tipodocumento")]
        public string Tipodocumento { get; set; }

        [Required]
        [MaxLength(3, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RSeries), Name = "Id")]
        public string Id { get; set; }

        [Required]
        [MaxLength(40, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RSeries), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(RSeries), Name = "Fkmonedas")]
        public int? Fkmonedas { get; set; }

        [Display(ResourceType = typeof(RSeries), Name = "Fkregimeniva")]
        public string Fkregimeniva { get; set; }

        [Display(ResourceType = typeof(RSeries), Name = "Fkcontadores")]
        public string Fkcontadores { get; set; }

        [Display(ResourceType = typeof(RSeries), Name = "Fkejercicios")]
        public string Fkejercicios { get; set; }


        [Display(ResourceType = typeof(RSeries), Name = "Fkejercicios")]
        public string Fkejerciciosdescripcion { get; set; }


        [Display(ResourceType = typeof(RSeries), Name = "Tipoimpresion")]
        public TipoImpresion Tipoimpresion { get; set; }

        [Display(ResourceType = typeof(RSeries), Name = "Riesgo")]
        public bool Riesgo { get; set; }

        [Display(ResourceType = typeof(RSeries), Name = "Borrador")]
        public bool Borrador { get; set; }

        [Display(ResourceType = typeof(RSeries), Name = "Rectificativa")]
        public bool Rectificativa { get; set; }

        public string Fkserieasociadatipodocumento { get; set; }

        [Display(ResourceType = typeof(RSeries), Name = "Fkseriesasociada")]
        public string Fkseriesasociada { get; set; }
        
        [Display(ResourceType = typeof(RSeries), Name = "TipoAlmacenLote")]
        public TipoAlmacenlote? Tipodealmacenlote { get; set; }

    }

    public class SeriesModel : BaseModel<SeriesModel, Series>
    {
        #region CTR

        public SeriesModel()
        {

        }

        public SeriesModel(IContextService context) : base(context)
        {

        }


        public SeriesModel(SeriesSerializableModel model)
        {
            var list = typeof(SeriesSerializableModel).GetProperties();
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
        }

        public string Empresa { get; set; }
        
        [Required]
        [Display(ResourceType = typeof(RSeries), Name = "Tipodocumento")]
        public string Tipodocumento { get; set; }

        [Required]
        [MaxLength(3,ErrorMessageResourceType = typeof(Unobtrusive),ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RSeries), Name = "Id")]
        public string Id { get; set; }

        [Required]
        [MaxLength(40, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RSeries), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(RSeries), Name = "Fkmonedas")]
        public int? Fkmonedas { get; set; }

        [Display(ResourceType = typeof(RSeries), Name = "Fkregimeniva")]
        public string Fkregimeniva { get; set; }

        [Display(ResourceType = typeof(RSeries), Name = "Fkcontadores")]
        public string Fkcontadores { get; set; }

        [Display(ResourceType = typeof(RSeries), Name = "Fkejercicios")]
        public string Fkejercicios { get; set; }


        [Display(ResourceType = typeof(RSeries), Name = "Fkejercicios")]
        public string Fkejerciciosdescripcion { get; set; }


        [Display(ResourceType = typeof(RSeries), Name = "Tipoimpresion")]
        public TipoImpresion Tipoimpresion { get; set; }

        [Display(ResourceType = typeof(RSeries), Name = "Riesgo")]
        public bool Riesgo { get; set; }

        [Display(ResourceType = typeof(RSeries), Name = "Borrador")]
        public bool Borrador { get; set; }

        [Display(ResourceType = typeof(RSeries), Name = "Rectificativa")]
        public bool Rectificativa { get; set; }

        public string Fkserieasociadatipodocumento { get; set; }

        [Display(ResourceType = typeof(RSeries), Name = "Fkseriesasociada")]
        public string Fkseriesasociada { get; set; }
        
        public BloqueoEntidadModel Bloqueo { get; set; }

        [Display(ResourceType = typeof(RSeries), Name = "Bloqueo")]
        public bool Bloqueado
        {
            get { return Bloqueo?.Bloqueada??false; }
        }


        [Display(ResourceType = typeof(RSeries), Name = "TipoAlmacenLote")]
        public TipoAlmacenlote? Tipodealmacenlote { get; set; }

        [Display(ResourceType = typeof(RSeries), Name = "EntradasVarias")]
        public bool Entradasvarias { get; set; }

        [Display(ResourceType = typeof(RSeries), Name = "SalidasVarias")]
        public bool Salidasvarias { get; set; }

        [Display(ResourceType = typeof(RSeries), Name = "Fkgruposusuarios")]
        public string Fkgruposusuarios { get; set; }

        #endregion

        public override string GetPrimaryKey()
        {
            return Tipodocumento + "-" + Id;
        }

        public override object generateId(string id)
        {
            return id.Split('-');
        }

        public override string DisplayName => RSeries.TituloEntidad;
       
    }
}
