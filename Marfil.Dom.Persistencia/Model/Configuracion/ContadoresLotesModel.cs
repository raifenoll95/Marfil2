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
using RContadoresLotes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Contadores;
namespace Marfil.Dom.Persistencia.Model.Configuracion
{
    public enum TipoLoteContador
    {
        [StringValue(typeof(RContadoresLotes), "TipoContadorMultidocumento")]
        Multidocumento,
        [StringValue(typeof(RContadoresLotes), "TipoContadorPrivado")]
        Privado
    }

    public enum TipoLoteInicio
    {
        [StringValue(typeof(RContadoresLotes), "TipoInicioSinreinicio")]
        Sinreinicio,
        [StringValue(typeof(RContadoresLotes), "TipoInicioAnual")]
        Anual,
        [StringValue(typeof(RContadoresLotes), "TipoInicioMensual")]
        Mensual,
        [StringValue(typeof(RContadoresLotes), "TipoInicioSemanal")]
        Semanal
    }

    public enum TiposLoteSegmentos
    {
        [StringValue(typeof(RContadoresLotes), "TiposSegmentosConstante")]
        Constante,
        [StringValue(typeof(RContadoresLotes), "TiposSegmentosAño")]
        Año,
        [StringValue(typeof(RContadoresLotes), "TiposSegmentosMes")]
        Mes,
        [StringValue(typeof(RContadoresLotes), "TiposSegmentosSemana")]
        Semana,
        [StringValue(typeof(RContadoresLotes), "TiposSegmentosSecuencia")]
        Secuencia
    }

    public class ContadoresLotesLinModel
    {
        public int Id { get; set; }

        [Required]
        [Display(ResourceType = typeof(RContadoresLotes), Name = "Tiposegmento")]
        public TiposLoteSegmentos Tiposegmento { get; set; }

        [Required]
        [Display(ResourceType = typeof(RContadoresLotes), Name = "Longitud")]
        public int Longitud { get; set; }

        [Display(ResourceType = typeof(RContadoresLotes), Name = "Valor")]
        public string Valor { get; set; }
    }

    public class ContadorLoteModel : IContadoresLotesModel
    {
        #region CTR

        public ContadorLoteModel()
        {

        }
        public ContadorLoteModel(ContadoresLotesModel model)
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

        public List<ContadoresLotesLinModel> Lineas
        {
            get;

            set;
        }

        public int Primerdocumento
        {
            get;

            set;
        }

        public TipoLoteContador Tipocontador
        {
            get;

            set;
        }

        public TipoLoteInicio Tipoinicio
        {
            get; set;
        }

        #endregion
    }

    public class ContadoresLotesModel : BaseModel<ContadoresLotesModel, ContadoresLotes>
    {
        private List<ContadoresLotesLinModel> _lineas = new List<ContadoresLotesLinModel>();
        private TipoLoteContador _tipocontador = TipoLoteContador.Multidocumento;

        #region Properties

        public string Empresa { get; set; }

        [Required]
        [MaxLength(12, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RContadoresLotes), Name = "Id")]
        public string Id { get; set; }

        [Required]
        [MaxLength(40, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RContadoresLotes), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Required]
        [Display(ResourceType = typeof(RContadoresLotes), Name = "Tipoinicio")]
        public TipoLoteInicio Tipoinicio { get; set; }

        [Required]
        [Display(ResourceType = typeof(RContadoresLotes), Name = "Primerdocumento")]
        public int Primerdocumento { get; set; }

        [Required]
        [Display(ResourceType = typeof(RContadoresLotes), Name = "Offset")]
        public int? Offset { get; set; }

        [Display(ResourceType = typeof(RContadoresLotes), Name = "Tipocontador")]
        public TipoLoteContador Tipocontador
        {
            get { return _tipocontador; }
            set { _tipocontador = value; }
        }

        public List<ContadoresLotesLinModel> Lineas
        {
            get { return _lineas; }
            set { _lineas = value; }
        }

        #endregion

        #region CTR

        public ContadoresLotesModel()
        {

        }

        public ContadoresLotesModel(IContextService context) : base(context)
        {

        }

        #endregion

        public override object generateId(string id)
        {
            return id;
        }

        public override string DisplayName => RContadoresLotes.TituloEntidad;
    }
}
