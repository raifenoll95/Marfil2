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
using RProveedores = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Proveedores;
using RDirecciones = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Direcciones;
namespace Marfil.Dom.Persistencia.Model.Terceros
{
    public enum CriterioIVA
    {
        Devengo,
        Caja
    }

    public enum Tipoportes
    {
        Pagados,
        Debidos
    }

    public enum PrevisionPagosPeriodicos
    {
        Factura,
        Plantilla
    }

    public class ProveedoresModel : BaseModel<ProveedoresModel, Proveedores>, IProveedorAcreedor
    {
        private readonly FModel fModel = new FModel();

        #region CTR

        public ProveedoresModel()
        {
            
        }
        
        public ProveedoresModel(IContextService context) : base(context)
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
        [Display(ResourceType = typeof(RProveedores), Name = "Fkcuentas")]
        public string Fkcuentas { get; set; }

        public CuentasModel Cuentas { get; set; }

        [Display(ResourceType = typeof(RProveedores), Name = "Descripcion")]
        public string Descripcion { get { return Cuentas?.Descripcion; } }

        [Display(ResourceType = typeof(RDirecciones), Name = "Direccion")]
        public string Direccion
        {
            get
            {
                return Direcciones?.Direcciones.FirstOrDefault(f => f.Defecto)?.Direccion;
            }
        }

        [Display(ResourceType = typeof(RProveedores), Name = "RazonSocial")]
        public string RazonSocial { get { return Cuentas?.Descripcion2; } }

        [Display(ResourceType = typeof(RProveedores), Name = "Nif")]
        public string Nif
        {
            get { return Cuentas?.Nif?.Nif; }
        }
        [Display(ResourceType = typeof(RProveedores), Name = "Pais")]
        public string Pais
        {
            get { return Direcciones?.Direcciones.FirstOrDefault(f => f.Defecto)?.Pais; }
        }
        [Display(ResourceType = typeof(RProveedores), Name = "Provincia")]
        public string Provincia
        {
            get
            {
                return Direcciones?.Direcciones.FirstOrDefault(f => f.Defecto)?.Provincia;
            }
        }
        [Display(ResourceType = typeof(RProveedores), Name = "Poblacion")]
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
        [Display(ResourceType = typeof(RProveedores), Name = "Fkidiomas")]
        public string Fkidiomas { get; set; }

        [Display(ResourceType = typeof(RProveedores), Name = "Fkfamiliaproveedor")]
        public string Fkfamiliaproveedor { get; set; }

        [Display(ResourceType = typeof(RProveedores), Name = "Fkzonaproveedor")]
        public string Fkzonaproveedor { get; set; }

        [Display(ResourceType = typeof(RProveedores), Name = "Fktipoempresa")]
        public string Fktipoempresa { get; set; }

        [Display(ResourceType = typeof(RProveedores), Name = "Fkunidadnegocio")]
        public string Fkunidadnegocio { get; set; }

        [Display(ResourceType = typeof(RProveedores), Name = "Fkincoterm")]
        public string Fkincoterm { get; set; }

        private PuertoscontrolModel _fkpuertos = new PuertoscontrolModel();

        public PuertoscontrolModel Fkpuertos
        {
            get { return _fkpuertos; }
            set { _fkpuertos = value; }
        }

        [Required]
        [Display(ResourceType = typeof(RProveedores), Name = "Fkmonedas")]
        public int Fkmonedas { get; set; }

        [Display(ResourceType = typeof(RProveedores), Name = "Fkregimeniva")]
        public string Fkregimeniva { get; set; }

        [Display(ResourceType = typeof(RProveedores), Name = "Fkgruposiva")]
        public string Fkgruposiva { get; set; }

        [Display(ResourceType = typeof(RProveedores), Name = "Criterioiva")]
        public CriterioIVA Criterioiva { get; set; }

        [Display(ResourceType = typeof(RProveedores), Name = "Fktiposretencion")]
        public string Fktiposretencion { get; set; }

        [Display(ResourceType = typeof(RProveedores), Name = "Fktransportistahabitual")]
        public string Fktransportistahabitual { get; set; }

        [Display(ResourceType = typeof(RProveedores), Name = "Tipodeportes")]
        public Tipoportes? Tipodeportes { get; set; }

        [Display(ResourceType = typeof(RProveedores), Name = "Cuentatesoreria")]
        public string Cuentatesoreria { get; set; }

        [Required]
        [Display(ResourceType = typeof(RProveedores), Name = "Fkformaspago")]
        public int Fkformaspago { get; set; }

        [Display(ResourceType = typeof(RProveedores), Name = "Descuentoprontopago")]
        [Range(0, 100, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "Range")]
        public double Descuentoprontopago { get; set; }

        [Display(ResourceType = typeof(RProveedores), Name = "Descuentocomercial")]
        [Range(0, 100, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "Range")]
        public double Descuentocomercial { get; set; }

        [Display(ResourceType = typeof(RProveedores), Name = "Diafijopago1")]
        [Range(0, 31, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "Range")]
        public int Diafijopago1 { get; set; }

        [Display(ResourceType = typeof(RProveedores), Name = "Diafijopago2")]
        [Range(0, 31, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "Range")]
        public int Diafijopago2 { get; set; }

        [Display(ResourceType = typeof(RProveedores), Name = "Periodonopagodesde")]
        public string Periodonopagodesde { get; set; }

        [Display(ResourceType = typeof(RProveedores), Name = "Periodonopagohasta")]
        public string Periodonopagohasta { get; set; }

        [Display(ResourceType = typeof(RProveedores), Name = "Notas")]
        public string Notas { get; set; }

        [Display(ResourceType = typeof(RProveedores), Name = "Tarifa")]
        public string Tarifa { get; set; }

        [Display(ResourceType = typeof(RProveedores), Name = "Fkcriteriosagrupacion")]
        public string Fkcriteriosagrupacion { get; set; }

        [Display(ResourceType = typeof(RProveedores), Name = "PrevisionPagosPeriodicos")]
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

        public override string DisplayName => RProveedores.TituloEntidad;

        public override object generateId(string id)
        {
            return id.Split('-');
        }
    }
}
