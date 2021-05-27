using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos;
using Resources;
using RContadores = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Contadores;
namespace Marfil.Dom.Persistencia.Model.Configuracion
{
    public enum TipoContador
    {
        [StringValue(typeof(RContadores), "TipoContadorMultidocumento")]
        Multidocumento,
        [StringValue(typeof(RContadores), "TipoContadorPrivado")]
        Privado
    }

    public enum TipoInicio
    {
        [StringValue(typeof(RContadores), "TipoInicioSinreinicio")]
        Sinreinicio,
        [StringValue(typeof(RContadores), "TipoInicioAnual")]
        Anual,
        [StringValue(typeof(RContadores), "TipoInicioMensual")]
        Mensual
    }

    public enum TiposSegmentos
    {
        [StringValue(typeof(RContadores), "TiposSegmentosConstante")]
        Constante,
        [StringValue(typeof(RContadores), "TiposSegmentosSerie")]
        Serie,
        [StringValue(typeof(RContadores), "TiposSegmentosAño")]
        Año,
        [StringValue(typeof(RContadores), "TiposSegmentosMes")]
        Mes,
        [StringValue(typeof(RContadores), "TiposSegmentosSecuencia")]
        Secuencia
    }

    public class ContadoresLinModel
    {
        public int Id { get; set; }

        [Required]
        [Display(ResourceType = typeof(RContadores), Name = "Tiposegmento")]
        public TiposSegmentos Tiposegmento { get; set; }

        [Required]
        [Display(ResourceType = typeof(RContadores), Name = "Longitud")]
        public int Longitud { get; set; }

        [Display(ResourceType = typeof(RContadores), Name = "Valor")]
        public string Valor { get; set; }
    }

    public class ContadorModel : IContadoresModel
    {
        #region CTR

        public ContadorModel()
        {

        }
        public ContadorModel(ContadoresModel model)
        {
            Descripcion = model.Descripcion;
            Empresa = model.Empresa;
            Id = model.Id;
            Lineas = model.Lineas;
            Primerdocumento = model.Primerdocumento;
            Tipocontador = model.Tipocontador;
            Tipoinicio = model.Tipoinicio;
        }

        #endregion


        #region Properties

        public string Descripcion
        {
            get; set;
        }

        public string Empresa
        {
            get;

            set;
        }

        public string Id
        {
            get;

            set;
        }

        public List<ContadoresLinModel> Lineas
        {
            get;

            set;
        }

        public int Primerdocumento
        {
            get;

            set;
        }

        public TipoContador Tipocontador
        {
            get;

            set;
        }

        public TipoInicio Tipoinicio
        {
            get; set;
        }

        #endregion
    }


    public class ContadoresModel : BaseModel<ContadoresModel, Contadores>
    {
        private List<ContadoresLinModel> _lineas = new List<ContadoresLinModel>();
        private TipoContador _tipocontador = TipoContador.Multidocumento;

        #region Properties

        public string Empresa { get; set; }

        [Required]
        [MaxLength(12, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RContadores), Name = "Id")]
        public string Id { get; set; }

        [Required]
        [MaxLength(40, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RContadores), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Required]
        [Display(ResourceType = typeof(RContadores), Name = "Tipoinicio")]
        public TipoInicio Tipoinicio { get; set; }

        [Required]
        [Display(ResourceType = typeof(RContadores), Name = "Primerdocumento")]
        public int Primerdocumento { get; set; }

        [Display(ResourceType = typeof(RContadores), Name = "Tipocontador")]
        public TipoContador Tipocontador
        {
            get { return _tipocontador; }
            set { _tipocontador = value; }
        }

        public List<ContadoresLinModel> Lineas
        {
            get { return _lineas; }
            set { _lineas = value; }
        }

        #endregion

        #region CTR

        public ContadoresModel()
        {

        }

        public ContadoresModel(IContextService context) : base(context)
        {

        }

        #endregion

        public override object generateId(string id)
        {
            return id;
        }

        public override string DisplayName => RContadores.TituloEntidad;
    }
}
