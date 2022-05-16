using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Resources;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using RClientes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Clientes;
using Marfil.Dom.Persistencia.Model.CRM;
using Marfil.Dom.Persistencia.ServicesView;

namespace Marfil.Dom.Persistencia.Model.Terceros
{
    public class CuentasBusqueda:IModelView
    {
        [Display(ResourceType = typeof(RClientes), Name = "Fkcuentas")]
        public string Fkcuentas { get; set; }

        public string Id { get { return Fkcuentas; } }

        [Display(ResourceType = typeof(RClientes), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Razonsocial")]
        public string Razonsocial { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Nif")]
        public string Nif { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Pais")]
        public string Pais { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Provincia")]
        public string Provincia { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Poblacion")]
        public string Poblacion { get; set; }

        
        public bool? Bloqueado { get; set; }

        public IEnumerable<ViewProperty> getProperties()
        {
            var listNames = typeof(CuentasBusqueda).GetProperties().Select(f => f.Name).Except(typeof(IModelSearch).GetProperties().Select(h => h.Name));
            var properties = typeof(CuentasBusqueda).GetProperties().Where(f => listNames.Any(h => h == f.Name));

            return properties.Select(item => new ViewProperty
            {
                property = item,
                attributes = item.GetCustomAttributes(true)
            }).ToList();
        }

        public object get(string propertyName)
        {
            throw new NotImplementedException();
        }

        public void set(string propertyName, object value)
        {
            throw new NotImplementedException();
        }

        public void createNewPrimaryKey()
        {
            throw new NotImplementedException();
        }

        public string GetPrimaryKey()
        {
            throw new NotImplementedException();
        }
    }

    public class ClientesModel : BaseModel<ClientesModel, Clientes>, IProspectoCliente
    {
        private readonly FModel fModel = new FModel();

        #region CTR

        public ClientesModel()
        {

        }

        public ClientesModel(IContextService context) : base(context)
        {
            _direcciones = fModel.GetModel<DireccionesModel>(context);
            _contactos = fModel.GetModel<ContactosModel>(context);
            _bancosMandatos = fModel.GetModel<BancosMandatosModel>(context);

        }

        #endregion

        #region Properties

        private List<OportunidadesModel> _oportunidades = new List<OportunidadesModel>();

        public bool EsProspecto { get { return false; } }

        [Required]
        public string Empresa { get; set; }

        [Required]
        [Display(ResourceType = typeof(RClientes), Name = "Fkcuentas")]
        public string Fkcuentas { get; set; }

        public CuentasModel Cuentas { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Descripcion")]
        public string Descripcion { get { return Cuentas?.Descripcion; } }

        [Display(ResourceType = typeof(RClientes), Name = "RazonSocial")]
        public string RazonSocial { get { return Cuentas?.Descripcion2; } }

        [Display(ResourceType = typeof(RClientes), Name = "Nif")]
        public string Nif
        {
            get { return Cuentas?.Nif?.Nif; }
        }

        public string DireccionId
        {
            get { return Direcciones?.Direcciones.FirstOrDefault(f => f.Defecto)?.Id.ToString(); }
        }

        [Display(ResourceType = typeof(RClientes), Name = "Pais")]
        public string Direccion
        {
            get { return Direcciones?.Direcciones.FirstOrDefault(f => f.Defecto)?.Direccion; }
        }

        [Display(ResourceType = typeof(RClientes), Name = "Pais")]
        public string Pais
        {
            get { return Direcciones?.Direcciones.FirstOrDefault(f => f.Defecto)?.Pais; }
        }
        [Display(ResourceType = typeof(RClientes), Name = "Provincia")]
        public string Provincia
        {
            get
            {
                return Direcciones?.Direcciones.FirstOrDefault(f => f.Defecto)?.Provincia;
            }
        }
        [Display(ResourceType = typeof(RClientes), Name = "Poblacion")]
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

        [Required]
        [Display(ResourceType = typeof(RClientes), Name = "Fkidiomas")]
        public string Fkidiomas { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Fkfamiliacliente")]
        public string Fkfamiliacliente { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Fkzonacliente")]
        public string Fkzonacliente { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Fktipoempresa")]
        public string Fktipoempresa { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Fkunidadnegocio")]
        public string Fkunidadnegocio { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Fkincoterm")]
        public string Fkincoterm { get; set; }

        private PuertoscontrolModel _fkpuertos = new PuertoscontrolModel();

        public PuertoscontrolModel Fkpuertos
        {
            get { return _fkpuertos; }
            set { _fkpuertos = value; }
        }

        [Required]
        [Display(ResourceType = typeof(RClientes), Name = "Fkmonedas")]
        public int Fkmonedas { get; set; }

        public int? Moneda { get { return Fkmonedas; } }

        [Display(ResourceType = typeof(RClientes), Name = "Fkregimeniva")]
        public string Fkregimeniva { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Fktipofactura")]
        public string Fktipofactura { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Fkgruposiva")]
        public string Fkgruposiva { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Criterioiva")]
        public CriterioIVA Criterioiva { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Fktiposretencion")]
        public string Fktiposretencion { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Fktransportistahabitual")]
        public string Fktransportistahabitual { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Tipodeportes")]
        public Tipoportes? Tipodeportes { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Cuentatesoreria")]
        public string Cuentatesoreria { get; set; }

        [Required]
        [Display(ResourceType = typeof(RClientes), Name = "Fkformaspago")]
        public int Fkformaspago { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Descuentoprontopago")]
        [Range(0, 100, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "Range")]
        public double Descuentoprontopago { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Descuentocomercial")]
        [Range(0, 100, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "Range")]
        public double Descuentocomercial { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Diafijopago1")]
        [Range(0, 31, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "Range")]
        public int Diafijopago1 { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Diafijopago2")]
        [Range(0, 31, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "Range")]
        public int Diafijopago2 { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Periodonopagodesde")]
        public string Periodonopagodesde { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Periodonopagohasta")]
        public string Periodonopagohasta { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Notas")]
        public string Notas { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Tarifa")]
        public string Fktarifas { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Numerocopiasfactura")]
        public int Numerocopiasfactura { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Fkcuentasagente")]
        public string Fkcuentasagente { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Fkcuentascomercial")]
        public string Fkcuentascomercial { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Perteneceagrupo")]
        public string Perteneceagrupo { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Fkcuentasaseguradoras")]
        public string Fkcuentasaseguradoras { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Suplemento")]
        [MaxLength(10,ErrorMessageResourceType = typeof(Unobtrusive),ErrorMessageResourceName = "MaxLength")]
        public string Suplemento { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Porcentajeriesgocomercial")]
        [Range(0,100,ErrorMessageResourceType = typeof(Unobtrusive),ErrorMessageResourceName = "Range")]
        public double Porcentajeriesgocomercial { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Porcentajeriesgopolitico")]
        [Range(0, 100, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "Range")]
        public double Porcentajeriesgopolitico { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Riesgoconcedidoempresa")]
        [Range(0, 999999999, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "Range")]
        public int Riesgoconcedidoempresa { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Riesgosolicitado")]
        [Range(0, 999999999, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "Range")]
        public int Riesgosolicitado { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Riesgoaseguradora")]
        [Range(0, 999999999, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "Range")]
        public int Riesgoaseguradora { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Fechaclasificacion")]
        public DateTime? Fechaclasificacion { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Fechaultimasolicitud")]
        public DateTime? Fechaultimasolicitud { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Diascondecidos")]
        public int Diascondecidos { get; set; }

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

        public List<OportunidadesModel> Oportunidades
        {
            get { return _oportunidades; }
            set { _oportunidades = value; }
        }


        private BancosMandatosModel _bancosMandatos;
        private List<SelectListItem> _lstTarifas=new List<SelectListItem>();

        public BancosMandatosModel BancosMandatos
        {
            get
            {
                _bancosMandatos = _bancosMandatos ?? fModel.GetModel<BancosMandatosModel>(Context);
                return _bancosMandatos;
            }
            set { _bancosMandatos = value; }
        }

        public IEnumerable<SelectListItem> LstTarifas
        {
            get { return _lstTarifas; }
            set { _lstTarifas = value.ToList(); }
        }
        
        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.Criteriosagrupacion), Name = "TituloEntidadSingular")]
        public string Fkcriteriosagrupacion { get; set; }

        #endregion

        public override void createNewPrimaryKey()
        {
            primaryKey = new[] { GetType().GetProperty("Fkcuentas") };
        }

        public override string DisplayName => RClientes.TituloEntidad;

        public override object generateId(string id)
        {
            return id.Split('-');
        }
    }
}
