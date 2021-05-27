using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Marfil.Dom.Persistencia.Model.Diseñador;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RCRM = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.CRM;
using System.Linq;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Helpers;

namespace Marfil.Dom.Persistencia.Model.CRM
{

    public enum TipoPrioridad
    {
        [Display(ResourceType = typeof(RCRM), Name = "TipoPrioridadSinDefinir")]
        Sindefinir,
        [Display(ResourceType = typeof(RCRM), Name = "TipoPrioridadBaja")]
        Baja,
        [Display(ResourceType = typeof(RCRM), Name = "TipoPrioridadMedia")]
        Media,
        [Display(ResourceType = typeof(RCRM), Name = "TipoPrioridadAlta")]
        Alta,
        [Display(ResourceType = typeof(RCRM), Name = "TipoPrioridadMuyAlta")]
        Muyalta,
    }

    public class OportunidadesModel : BaseModel<OportunidadesModel, Oportunidades>
    {
        private readonly FModel fModel = new FModel();
        private List<SeguimientosModel> _seguimientos = new List<SeguimientosModel>();

        public bool ReadOnly { get; set; }

        #region Properties

        [Required]        
        public string Empresa { get; set; }

        [Required]        
        public int Id { get; set; }
            
        [Required]
        [Display(ResourceType = typeof(RCRM), Name = "Serie")]
        public string Fkseries { get; set; }
        
        public string Identificadorsegmento { get; set; }

        [Display(ResourceType = typeof(RCRM), Name = "Referencia")]
        public string Referencia { get; set; }

        [Display(ResourceType = typeof(RCRM), Name = "Fechaapertura")]
        public DateTime? Fechadocumento { get; set; }

        public String FechaAperturaStr { get; set; }

        [Display(ResourceType = typeof(RCRM), Name = "Asunto")]
        public string Asunto { get; set; }

        [Display(ResourceType = typeof(RCRM), Name = "Fechaultimoseguimiento")]
        public DateTime? Fechaultimoseguimiento { get; set; }

        [Display(ResourceType = typeof(RCRM), Name = "Fechaproximoseguimiento")]
        public DateTime? Fechaproximoseguimiento { get; set; }

        public string fechaproximo { get; set; }

        public string fechaultimo { get; set; }

        [Display(ResourceType = typeof(RCRM), Name = "Empresa")]
        public string Fkempresa { get; set; }

        [Display(ResourceType = typeof(RCRM), Name = "Descripciontercero")]
        public string Descripciontercero { get; set; }

        [Display(ResourceType = typeof(RCRM), Name = "Contacto")]
        public string Fkcontacto { get; set; }

        public string Nombrecontacto { get; set; }

        [Display(ResourceType = typeof(RCRM), Name = "Origen")]
        public string Fkorigen { get; set; }

        [Display(ResourceType = typeof(RCRM), Name = "Prioridad")]
        public TipoPrioridad Prioridad { get; set; }

        [Display(ResourceType = typeof(RCRM), Name = "Margen")]
        public string Fkmargen { get; set; }

        [Display(ResourceType = typeof(RCRM), Name = "Probabilidadcierre")]
        public string Probabilidadcierre { get; set; }

        [Display(ResourceType = typeof(RCRM), Name = "Operario")]
        public string Fkoperario { get; set; }

        [Display(ResourceType = typeof(RCRM), Name = "Comercial")]
        public string Fkcomercial { get; set; }

        [Display(ResourceType = typeof(RCRM), Name = "Agente")]
        public string Fkagente { get; set; }

        [Display(ResourceType = typeof(RCRM), Name = "Notas")]
        public string Notas { get; set; }

        [Display(ResourceType = typeof(RCRM), Name = "Fechacierre")]
        public DateTime? Fechacierre { get; set; }

        [Display(ResourceType = typeof(RCRM), Name = "Coste")]
        public int Coste { get; set; }

        [Display(ResourceType = typeof(RCRM), Name = "Cerrado")]
        public bool Cerrado { get; set; }

        [Display(ResourceType = typeof(RCRM), Name = "Reaccion")]
        public string Fkreaccion { get; set; }

        public List<SeguimientosModel> Seguimientos
        {
            get { return _seguimientos; }
            set { _seguimientos = value; }
        }

        [Display(ResourceType = typeof(RCRM), Name = "Etapa")]
        public string Fketapa { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        public IEnumerable<EstadosModel> EstadosAsociados
        {
            get
            {
                using (var serviceEstados = new EstadosService(Context))
                {
                    return serviceEstados.GetStates(DocumentoEstado.Oportunidades, Tipoestado(Context));
                }
            }
        }

        //[XmlIgnore]
        //[IgnoreDataMember]
        //public EstadosModel Estado
        //{
        //    get
        //    {
        //        if (!string.IsNullOrEmpty(Fketapa))
        //        {
        //            using (var estadosService = FService.Instance.GetService(typeof(EstadosModel), Context))
        //            {
        //                return estadosService.get(Fketapa) as EstadosModel;
        //            }
        //        }
        //        return null;
        //    }
        //}

        //[XmlIgnore]
        //[IgnoreDataMember]
        //[Display(ResourceType = typeof(RCRM), Name = "Etapa")]
        //public string Etapadescripcion
        //{
        //    get { return Estado?.Descripcion; }
        //}

        public TipoEstado Tipoestado(IContextService context)
        {
            if (!string.IsNullOrEmpty(Fketapa))
            {
                using (var estadosService = FService.Instance.GetService(typeof(EstadosModel), Context))
                {
                    var estadoObj = estadosService.get(Fketapa) as EstadosModel;
                    return estadoObj?.Tipoestado ?? TipoEstado.Diseño;
                }
            }
            return TipoEstado.Diseño;
        }

        public override void createNewPrimaryKey()
        {
            primaryKey = getProperties().Where(f => f.property.Name == "Id").Select(f => f.property);
        }

        #endregion

        public override object generateId(string id)
        {
            return id;
        }

        public override string DisplayName => RCRM.TituloEntidadOportunidades;

        #region CTR

        public OportunidadesModel()
        {

        }

        public OportunidadesModel(IContextService context) : base(context)
        {

        }

        #endregion
    }
}
