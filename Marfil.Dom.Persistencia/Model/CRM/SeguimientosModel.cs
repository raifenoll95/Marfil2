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
using RSeguimientos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Seguimientos;
using RSeguimientosCorreo = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.SeguimientosCorreo;
using System.Linq;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Helpers;

namespace Marfil.Dom.Persistencia.Model.CRM
{

    public class SeguimientosModel : BaseModel<SeguimientosModel, Seguimientos>
    {

        private List<SeguimientosCorreoModel> _correos = new List<SeguimientosCorreoModel>();

        #region Properties    

        [Required]        
        public string Empresa { get; set; }

        [Required]        
        public int Id { get; set; }

        [Display(ResourceType = typeof(RSeguimientos), Name = "Origen")]
        public string Origen { get; set; }
       
        public int Tipo { get; set; }

        public string Usuario { get; set; }

        [Display(ResourceType = typeof(RSeguimientos), Name = "Fechadocumento")]
        public DateTime? Fechadocumento { get; set; }

        [Display(ResourceType = typeof(RSeguimientos), Name = "Asunto")]
        public string Asunto { get; set; }

        [Display(ResourceType = typeof(RSeguimientos), Name = "Empresa")]
        public string Fkempresa { get; set; }

        [Display(ResourceType = typeof(RSeguimientos), Name = "Descripciontercero")]
        public string Descripciontercero { get; set; }

        [Display(ResourceType = typeof(RSeguimientos), Name = "Contacto")]
        public string Fkcontacto { get; set; }

        public string Nombrecontacto { get; set; }

        [Display(ResourceType = typeof(RSeguimientos), Name = "Accion")]
        public string Fkaccion { get; set; }

        [Display(ResourceType = typeof(RSeguimientos), Name = "Accion")]
        public string Descripcionaccion { get; set; }

        [Display(ResourceType = typeof(RSeguimientos), Name = "Clavecoste")]
        public string Fkclavecoste { get; set; }

        [Display(ResourceType = typeof(RSeguimientos), Name = "Coste")]
        public int Coste { get; set; }

        [Display(ResourceType = typeof(RSeguimientos), Name = "Notas")]
        public string Notas { get; set; }

        [Display(ResourceType = typeof(RSeguimientos), Name = "Fechaproximoseguimiento")]
        public DateTime? Fechaproximoseguimiento { get; set; }

        [Display(ResourceType = typeof(RSeguimientos), Name = "Tipodocumentorelacionado")]
        public string Fkdocumentorelacionado { get; set; }

        [Display(ResourceType = typeof(RSeguimientos), Name = "Documentorelacionado")]
        public string Fkreferenciadocumentorelacionado { get; set; }

        public int Idrelacionado { get; set; }

        [Display(ResourceType = typeof(RSeguimientos), Name = "Fecharesolucion")]
        public DateTime? Fecharesolucion { get; set; }

        [Display(ResourceType = typeof(RSeguimientos), Name = "Cerrado")]
        public bool Cerrado { get; set; }

        [Display(ResourceType = typeof(RSeguimientos), Name = "Reaccion")]
        public string Fkreaccion { get; set; }

        [Display(ResourceType = typeof(RSeguimientos), Name = "Etapa")]
        public string Fketapa { get; set; }

        public List<SeguimientosCorreoModel> Correos
        {
            get { return _correos; }
            set { _correos = value.ToList(); }
        }

        //[XmlIgnore]
        //[IgnoreDataMember]
        //public IEnumerable<EstadosModel> EstadosAsociados
        //{
        //    get
        //    {
        //        using (var serviceEstados = new EstadosService(Context))
        //        {
        //            var appService = new ApplicationHelper(Context);

        //            if (Tipo == (int)DocumentoEstado.Oportunidades)
        //            {
        //                var configuracion = appService.GetConfiguracion();
        //                Fketapa = configuracion?.Estadooportunidadesinicial ?? string.Empty;
        //                return serviceEstados.GetStates(DocumentoEstado.Oportunidades, Tipoestado(Context));                        
        //            }
        //            else if (Tipo == (int)DocumentoEstado.Proyectos)
        //            {
        //                var configuracion = appService.GetConfiguracion();
        //                Fketapa = configuracion?.Estadoproyectosinicial ?? string.Empty;
        //                return serviceEstados.GetStates(DocumentoEstado.Proyectos, Tipoestado(Context));
        //            }
        //            else if(Tipo == (int)DocumentoEstado.Campañas)
        //            {
        //                var configuracion = appService.GetConfiguracion();
        //                Fketapa = configuracion?.Estadocampañasinicial ?? string.Empty;
        //                return serviceEstados.GetStates(DocumentoEstado.Campañas, Tipoestado(Context));
        //            }
        //            else if (Tipo == (int)DocumentoEstado.Incidencias)
        //            {
        //                var configuracion = appService.GetConfiguracion();
        //                Fketapa = configuracion?.Estadoincidenciasinicial ?? string.Empty;
        //                return serviceEstados.GetStates(DocumentoEstado.Incidencias, Tipoestado(Context));
        //            }

        //            return serviceEstados.GetStates(DocumentoEstado.Todos, Tipoestado(Context));
        //        }
        //    }
        //}

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

        public string Referencia { get; set; }
        public string Fecha { get; set; }
        public double? Baseimponible { get; set; }

        public int idPadre { get; set; }
        public string urlPadre { get; set; }
        public string urlDocumentoRelacionado { get; set; }

        public override void createNewPrimaryKey()
        {
            primaryKey = getProperties().Where(f => f.property.Name == "Id").Select(f => f.property);
        }

        #endregion

        public override object generateId(string id)
        {
            return id;
        }

        public override string DisplayName => RSeguimientos.TituloEntidad;

        #region CTR

        public SeguimientosModel()
        {

        }

        public SeguimientosModel(IContextService context) : base(context)
        {

        }

        #endregion
    }

    [Serializable]
    public class SeguimientosCorreoModel : BaseModel<SeguimientosCorreoModel, SeguimientosCorreo>
    {

        public SeguimientosCorreoModel(IContextService context)
        {
        }

        public SeguimientosCorreoModel()
        {

        }

        [Display(ResourceType = typeof(RSeguimientosCorreo), Name = "Empresa")]
        public string Empresa { get; set; }

        [Display(ResourceType = typeof(RSeguimientosCorreo), Name = "Fkseguimientos")]
        public int Fkseguimientos { get; set; }

        [Display(ResourceType = typeof(RSeguimientosCorreo), Name = "Fkorigen")]
        public string Fkorigen { get; set; }

        [Required]
        [Key]
        [Display(ResourceType = typeof(RSeguimientosCorreo), Name = "Id")]
        public int Id { get; set; }

        [Display(ResourceType = typeof(RSeguimientosCorreo), Name = "Correo")]
        public string Correo { get; set; }

        [Display(ResourceType = typeof(RSeguimientosCorreo), Name = "Asunto")]
        public string Asunto { get; set; }

        [Display(ResourceType = typeof(RSeguimientosCorreo), Name = "Fecha")]
        public DateTime Fecha { get; set; }

        public override string DisplayName => RSeguimientosCorreo.TituloEntidad;

        public override object generateId(string id)
        {
            return id;
        }
    }
}
