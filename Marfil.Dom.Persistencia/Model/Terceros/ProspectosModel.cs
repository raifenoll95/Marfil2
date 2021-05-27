using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.CRM;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RProspectos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Prospectos;
namespace Marfil.Dom.Persistencia.Model.Terceros
{
    public class ProspectosModel : BaseModel<ProspectosModel, Prospectos>, IProspectoCliente
    {
        private readonly FModel fModel = new FModel();
        private List<SelectListItem> _lstTarifas = new List<SelectListItem>();
        private List<OportunidadesModel> _oportunidades = new List<OportunidadesModel>();

        #region CTR

        public ProspectosModel()
        {
            
        }
        public ProspectosModel(IContextService context) : base(context)
        {
            _direcciones = fModel.GetModel<DireccionesModel>(context);
            _contactos = fModel.GetModel<ContactosModel>(context);
           
        }

        #endregion

        #region Propiedades

        public bool EsProspecto { get { return true; } }

        [Required]
        public string Empresa { get; set; }

        [Required]
        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.Clientes), Name = "Fkcuentas")]
        public string Fkcuentas { get; set; }

        public CuentasModel Cuentas { get; set; }

        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.Clientes), Name = "Nif")]
        public string Nif
        {
            get { return Cuentas?.Nif?.Nif; }
        }

        public double Descuentocomercial {
            get { return 0; }
        }
        public double Descuentoprontopago { get { return 0; } }
        public string Fktransportistahabitual { get { return string.Empty; } }

        public string DireccionId
        {
            get { return Direcciones?.Direcciones.FirstOrDefault(f => f.Defecto)?.Id.ToString(); }
        }

        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.Clientes), Name = "Pais")]
        public string Direccion
        {
            get { return Direcciones?.Direcciones.FirstOrDefault(f => f.Defecto)?.Direccion; }
        }

        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.Clientes), Name = "Pais")]
        public string Pais
        {
            get { return Direcciones?.Direcciones.FirstOrDefault(f => f.Defecto)?.Pais; }
        }
        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.Clientes), Name = "Provincia")]
        public string Provincia
        {
            get
            {
                return Direcciones?.Direcciones.FirstOrDefault(f => f.Defecto)?.Provincia;
            }
        }
        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.Clientes), Name = "Poblacion")]
        public string Poblacion
        {
            get
            {
                return Direcciones?.Direcciones.FirstOrDefault(f => f.Defecto)?.Poblacion;
            }
        }

        public string Cp
        {
            get { return Direcciones?.Direcciones.FirstOrDefault(f => f.Defecto)?.Cp; }
        }

        public string Telefono
        {
            get { return Direcciones?.Direcciones.FirstOrDefault(f => f.Defecto)?.Telefono + " - " + Direcciones?.Direcciones.FirstOrDefault(f => f.Defecto)?.Telefonomovil; }
        }

        public string Fax
        {
            get { return Direcciones?.Direcciones.FirstOrDefault(f => f.Defecto)?.Fax; }
        }

        public string Email
        {
            get { return Direcciones?.Direcciones.FirstOrDefault(f => f.Defecto)?.Email; }
        }

        public bool Bloqueado
        {
            get { return Cuentas?.Bloqueado ?? false; }
        }

        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.Clientes), Name = "Descripcion")]
        public string Descripcion { get { return Cuentas?.Descripcion; } }

        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.Clientes), Name = "RazonSocial")]
        public string RazonSocial { get { return Cuentas?.Descripcion2; } }

        [Required]
        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.Clientes), Name = "Fkidiomas")]
        public string Fkidiomas { get; set; }

        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.Clientes), Name = "Fkfamiliacliente")]
        public string Fkfamiliacliente { get; set; }

        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.Clientes), Name = "Fkzonacliente")]
        public string Fkzonacliente { get; set; }

        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.Clientes), Name = "Fktipoempresa")]
        public string Fktipoempresa { get; set; }

        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.Clientes), Name = "Fkunidadnegocio")]
        public string Fkunidadnegocio { get; set; }

        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.Clientes), Name = "Fkincoterm")]
        public string Fkincoterm { get; set; }

        private PuertoscontrolModel _fkpuertos = new PuertoscontrolModel();

        public PuertoscontrolModel Fkpuertos
        {
            get { return _fkpuertos; }
            set { _fkpuertos = value; }
        }

        public List<OportunidadesModel> Oportunidades
        {
            get { return _oportunidades; }
            set { _oportunidades = value; }
        }

        [Required]
        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.Clientes), Name = "Fkmonedas")]
        public string Fkmonedas { get; set; }

        public int? Moneda
        {
            get { return Funciones.Qint(Fkmonedas); }
        }

        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.Clientes), Name = "Fkregimeniva")]
        public string Fkregimeniva { get; set; }

        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.Clientes), Name = "Fkcuentasagente")]
        public string Fkcuentasagente { get; set; }

        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.Clientes), Name = "Fkcuentascomercial")]
        public string Fkcuentascomercial { get; set; }

        [Required]
        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.Clientes), Name = "Fkformaspago")]
        public int Fkformaspago { get; set; }

        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.Clientes), Name = "Tarifa")]
        public string Fktarifas { get; set; }

        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.Clientes), Name = "Notas")]
        public string Observaciones { get; set; }

       

        private ContactosModel _contactos;
        public ContactosModel Contactos
        {
            get
            {
                _contactos = _contactos ?? fModel.GetModel<ContactosModel>(Context);
                return _contactos;
            }
            set { _contactos = value; }
        }

        private DireccionesModel _direcciones;
        public DireccionesModel Direcciones
        {
            get
            {
                _direcciones = _direcciones ?? fModel.GetModel<DireccionesModel>(Context);
                return _direcciones;
            }
            set { _direcciones = value; }
        }

        public IEnumerable<SelectListItem> LstTarifas
        {
            get { return _lstTarifas; }
            set { _lstTarifas = value.ToList(); }
        }

        [Display(ResourceType = typeof(RProspectos), Name = "Fkmodocontacto")]
        public string Fkmodocontacto { get; set; }

        #endregion
        public override void createNewPrimaryKey()
        {
            primaryKey = new[] { GetType().GetProperty("Fkcuentas") };
        }
        public override object generateId(string id)
        {
            return id.Split('-');
        }

        public override string DisplayName => RProspectos.TituloEntidad;
    }
}
