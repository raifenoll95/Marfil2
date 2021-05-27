using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;
using RCRM = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.CRM;
using RCampañas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Campañas;
using System.Linq;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Marfil.Dom.Persistencia.Model.Configuracion;

namespace Marfil.Dom.Persistencia.Model.CRM
{

    public class CampañasModel : BaseModel<CampañasModel, Campañas>
    {
        private readonly FModel fModel = new FModel();
        private List<SeguimientosModel> _seguimientos = new List<SeguimientosModel>();
        private List<CampañasTerceroModel> _terceros = new List<CampañasTerceroModel>();

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

        [Display(ResourceType = typeof(RCRM), Name = "Asunto")]
        public string Asunto { get; set; }

        [Display(ResourceType = typeof(RCRM), Name = "Fechaultimoseguimiento")]
        public DateTime? Fechaultimoseguimiento { get; set; }

        [Display(ResourceType = typeof(RCRM), Name = "Fechaproximoseguimiento")]
        public DateTime? Fechaproximoseguimiento { get; set; }

        [Display(ResourceType = typeof(RCRM), Name = "Prioridad")]
        public TipoPrioridad Prioridad { get; set; }

        [Display(ResourceType = typeof(RCRM), Name = "Modocontacto")]
        public string Fkmodocontacto { get; set; }

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

        public IEnumerable<SeguimientosModel> Seguimientos
        {
            get { return _seguimientos; }
            set { _seguimientos = value.ToList(); }
        }

        [Display(ResourceType = typeof(RCRM), Name = "Etapa")]
        public string Fketapa { get; set; }

        public IEnumerable<CampañasTerceroModel> Campañas
        {
            get { return _terceros; }
            set { _terceros = value.ToList(); }
        }


        public IEnumerable<EstadosModel> EstadosAsociados
        {
            get
            {
                using (var serviceEstados = new EstadosService(Context))
                {
                    return serviceEstados.GetStates(DocumentoEstado.Campañas, Tipoestado(Context));
                }
            }
        }

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

        public override string DisplayName => RCRM.TituloEntidadCampañas;

        #region CTR

        public CampañasModel()
        {

        }

        public CampañasModel(IContextService context) : base(context)
        {

        }

        #endregion
    }


    [Serializable]
    public class CampañasTerceroModel : BaseModel<CampañasTerceroModel, CampañasTercero>
    {

        public CampañasTerceroModel(IContextService context)
        {
        }

        public CampañasTerceroModel()
        {

        }

        [Display(ResourceType = typeof(RCampañas), Name = "Empresa")]
        public string Empresa { get; set; }

        [Display(ResourceType = typeof(RCampañas), Name = "Fkcampañas")]
        public int Fkcampañas { get; set; }

        [Required]
        [Key]
        [Display(ResourceType = typeof(RCampañas), Name = "Id")]
        public int Id { get; set; }

        [Display(ResourceType = typeof(RCampañas), Name = "Codtercero")]
        public string Codtercero { get; set; }

        [Display(ResourceType = typeof(RCampañas), Name = "Descripciontercero")]
        public string Descripciontercero { get; set; }

        [MaxLength(120, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RCampañas), Name = "Poblacion")]
        public string Poblacion { get; set; }

        [MaxLength(120, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RCampañas), Name = "Fkprovincia")]
        public string Fkprovincia { get; set; }

        [MaxLength(120, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RCampañas), Name = "Fkpais")]
        public string Fkpais { get; set; }

        [Display(ResourceType = typeof(RCampañas), Name = "Email")]
        public string Email { get; set; }

        [Display(ResourceType = typeof(RCampañas), Name = "Telefono")]
        public string Telefono { get; set; }

        public override string DisplayName => RCampañas.TituloEntidad;

        public override object generateId(string id)
        {
            return id;
        }
    }
}
