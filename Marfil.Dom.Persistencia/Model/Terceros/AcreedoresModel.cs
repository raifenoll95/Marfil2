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
using RAcreedores = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Acreedores;
using RDirecciones = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Direcciones;




namespace Marfil.Dom.Persistencia.Model.Terceros
{

    public class AcreedoresModel : BaseModel<AcreedoresModel, Acreedores>, IProveedorAcreedor
    {
        private readonly FModel fModel = new FModel();

        #region CTR

        public AcreedoresModel()
        {
            
        }

        public AcreedoresModel(IContextService context):base(context)
        {
            _direcciones = fModel.GetModel<DireccionesModel>(context);
            _contactos = fModel.GetModel<ContactosModel>(context);
            _bancosMandatos = fModel.GetModel<BancosMandatosModel>(context);
        }

        #endregion
        
        #region Properties

        [Required]
        public string Empresa { get; set; }

        [Required]
        [Display(ResourceType = typeof(RAcreedores), Name = "Fkcuentas")]
        public string Fkcuentas { get; set; }


        [Display(ResourceType = typeof(RDirecciones), Name = "Direccion")]
        public string Direccion
        {
            get
            {
                return Direcciones?.Direcciones.FirstOrDefault(f => f.Defecto)?.Direccion;
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

        public CuentasModel Cuentas { get; set; }

        [Display(ResourceType = typeof(RAcreedores), Name = "Descripcion")]
        public string Descripcion { get { return Cuentas?.Descripcion; } }

        [Display(ResourceType = typeof(RAcreedores), Name = "RazonSocial")]
        public string Razonsocial { get { return Cuentas?.Descripcion2; } }

        [Display(ResourceType = typeof(RAcreedores), Name = "Nif")]
        public string Nif {
            get { return Cuentas?.Nif?.Nif; }
        }
        [Display(ResourceType = typeof(RAcreedores), Name = "Pais")]
        public string Pais {
            get { return Direcciones?.Direcciones.FirstOrDefault(f => f.Defecto)?.Pais; }
        }
        [Display(ResourceType = typeof(RAcreedores), Name = "Provincia")]
        public string Provincia {
            get
            {
                return Direcciones?.Direcciones.FirstOrDefault(f=>f.Defecto)?.Provincia;
            }
        }
        [Display(ResourceType = typeof(RAcreedores), Name = "Poblacion")]
        public string Poblacion {
            get
            {
                return Direcciones?.Direcciones.FirstOrDefault(f => f.Defecto)?.Poblacion;
            }
        }

        public bool Bloqueado
        {
            get { return Cuentas?.Bloqueado ?? false; }
        }

        [Required]
        [Display(ResourceType = typeof(RAcreedores), Name = "Fkidiomas")]
        public string Fkidiomas { get; set; }

        [Display(ResourceType = typeof(RAcreedores), Name = "Fkfamiliaacreedor")]
        public string Fkfamiliaacreedor { get; set; }

        [Display(ResourceType = typeof(RAcreedores), Name = "Fkzonaacreedor")]
        public string Fkzonaacreedor { get; set; }

        [Display(ResourceType = typeof(RAcreedores), Name = "Fktipoempresa")]
        public string Fktipoempresa { get; set; }

        [Display(ResourceType = typeof(RAcreedores), Name = "Fkunidadnegocio")]
        public string Fkunidadnegocio { get; set; }

        [Display(ResourceType = typeof(RAcreedores), Name = "Fkincoterm")]
        public string Fkincoterm { get; set; }

        private PuertoscontrolModel _fkpuertos = new PuertoscontrolModel();

        public PuertoscontrolModel Fkpuertos
        {
            get { return _fkpuertos; }
            set { _fkpuertos = value; }
        }

        [Required]
        [Display(ResourceType = typeof(RAcreedores), Name = "Fkmonedas")]
        public int Fkmonedas { get; set; }

        [Display(ResourceType = typeof(RAcreedores), Name = "Fktipofactura")]
        public string Fktipofactura { get; set; }

        [Display(ResourceType = typeof(RAcreedores), Name = "Fkregimeniva")]
        public string Fkregimeniva { get; set; }

        [Display(ResourceType = typeof(RAcreedores), Name = "Fkgruposiva")]
        public string Fkgruposiva { get; set; }

        [Display(ResourceType = typeof(RAcreedores), Name = "Criterioiva")]
        public CriterioIVA Criterioiva { get; set; }

        [Display(ResourceType = typeof(RAcreedores), Name = "Fktiposretencion")]
        public string Fktiposretencion { get; set; }

        [Display(ResourceType = typeof(RAcreedores), Name = "Fktransportistahabitual")]
        public string Fktransportistahabitual { get; set; }

        [Display(ResourceType = typeof(RAcreedores), Name = "Tipodeportes")]
        public Tipoportes? Tipodeportes { get; set; }

        [Display(ResourceType = typeof(RAcreedores), Name = "Cuentatesoreria")]
        public string Cuentatesoreria { get; set; }

        [Required]
        [Display(ResourceType = typeof(RAcreedores), Name = "Fkformaspago")]
        public int Fkformaspago { get; set; }

        [Display(ResourceType = typeof(RAcreedores), Name = "Descuentoprontopago")]
        [Range(0, 100, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "Range")]
        public double Descuentoprontopago { get; set; }

        [Display(ResourceType = typeof(RAcreedores), Name = "Descuentocomercial")]
        [Range(0, 100, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "Range")]
        public double Descuentocomercial { get; set; }

        [Display(ResourceType = typeof(RAcreedores), Name = "Diafijopago1")]
        [Range(0, 31, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "Range")]
        public int Diafijopago1 { get; set; }

        [Display(ResourceType = typeof(RAcreedores), Name = "Diafijopago2")]
        [Range(0, 31, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "Range")]
        public int Diafijopago2 { get; set; }

        [Display(ResourceType = typeof(RAcreedores), Name = "Periodonopagodesde")]
        public string Periodonopagodesde { get; set; }

        [Display(ResourceType = typeof(RAcreedores), Name = "Periodonopagohasta")]
        public string Periodonopagohasta { get; set; }

        [Display(ResourceType = typeof(RAcreedores), Name = "Notas")]
        public string Notas { get; set; }

        [Display(ResourceType = typeof(RAcreedores), Name = "Tarifa")]
        public string Tarifa { get; set; }

        [Display(ResourceType = typeof(RAcreedores), Name = "Fkcriteriosagrupacion")]
        public string Fkcriteriosagrupacion { get; set; }

        
        [Display(ResourceType = typeof(RAcreedores), Name = "PrevisionPagosPeriodicos")]
        public PrevisionPagosPeriodicos? Previsionpagosperiodicos { get; set; }


        private List<SelectListItem> _lstTarifas = new List<SelectListItem>();
        public IEnumerable<SelectListItem> LstTarifas
        {
            get { return _lstTarifas; }
            set { _lstTarifas = value.ToList(); }
        }








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
                ;
            }
            set { _direcciones = value; }
        }


        private BancosMandatosModel _bancosMandatos;
        public BancosMandatosModel BancosMandatos
        {
            get
            {
                _bancosMandatos = _bancosMandatos ?? fModel.GetModel<BancosMandatosModel>(Context);
                return _bancosMandatos;
            }
            set { _bancosMandatos = value; }
        }

        #endregion

        public override void createNewPrimaryKey()
        {
            primaryKey = new[] { GetType().GetProperty("Fkcuentas") };
        }

        public override string DisplayName => RAcreedores.TituloEntidad;

        public override object generateId(string id)
        {
            return id.Split('-');
        }
    }
}
