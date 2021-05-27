using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;
using RAseguradoras = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Aseguradoras;
using RDirecciones = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Direcciones;
using RAcreedores = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Acreedores;

namespace Marfil.Dom.Persistencia.Model.Terceros
{
    public class AseguradorasBusqueda : IModelView
    {
        [Display(ResourceType = typeof(RAseguradoras), Name = "Fkcuentas")]
        public string Fkcuentas { get; set; }

        [Display(ResourceType = typeof(RAseguradoras), Name = "Numeropoliza")]
        public string Numeropoliza { get; set; }

        [Display(ResourceType = typeof(RAseguradoras), Name = "Fechainicio")]
        public DateTime? Fechainicio { get; set; }

        [Display(ResourceType = typeof(RAseguradoras), Name = "Fechafin")]
        public DateTime? Fechafin { get; set; }

        [Display(ResourceType = typeof(RAseguradoras), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(RAseguradoras), Name = "Razonsocial")]
        public string Razonsocial { get; set; }

        

        public bool? Bloqueado { get; set; }

       

        public IEnumerable<ViewProperty> getProperties()
        {
            var listNames = typeof(AseguradorasBusqueda).GetProperties().Select(f => f.Name).Except(typeof(IModelView).GetProperties().Select(h => h.Name));
            var properties = typeof(AseguradorasBusqueda).GetProperties().Where(f => listNames.Any(h => h == f.Name));

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
    public class AseguradorasModel : BaseModel<AseguradorasModel, Aseguradoras>
    {
        private readonly FModel fModel = new FModel();
        
        #region CTR

        public AseguradorasModel()
        {
            
        }

        public AseguradorasModel(IContextService context) : base(context)
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
        [Display(ResourceType = typeof(RAseguradoras), Name = "Fkcuentas")]
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

        [Display(ResourceType = typeof(RAcreedores), Name = "Pais")]
        public string Pais
        {
            get { return Direcciones?.Direcciones.FirstOrDefault(f => f.Defecto)?.Pais; }
        }
        [Display(ResourceType = typeof(RAcreedores), Name = "Provincia")]
        public string Provincia
        {
            get
            {
                return Direcciones?.Direcciones.FirstOrDefault(f => f.Defecto)?.Provincia;
            }
        }
        [Display(ResourceType = typeof(RAcreedores), Name = "Poblacion")]
        public string Poblacion
        {
            get
            {
                return Direcciones?.Direcciones.FirstOrDefault(f => f.Defecto)?.Poblacion;
            }
        }

        public CuentasModel Cuentas { get; set; }

        public bool Bloqueado
        {
            get { return Cuentas?.Bloqueado ?? false; }
        }

        [Display(ResourceType = typeof(RAseguradoras), Name = "Descripcion")]
        public string Descripcion { get { return Cuentas?.Descripcion; } }

        [Display(ResourceType = typeof(RAseguradoras), Name = "RazonSocial")]
        public string RazonSocial { get { return Cuentas?.Descripcion2; } }

        [Required]
        [Display(ResourceType = typeof(RAseguradoras), Name = "Numeropoliza")]
        [MaxLength(20,ErrorMessageResourceType = typeof(Unobtrusive),ErrorMessageResourceName = "MaxLength")]
        public string Numeropoliza { get; set; }

        private DateTime _fechainicio=DateTime.Now;
        [Required]
        [Display(ResourceType = typeof(RAseguradoras), Name = "Fechainicio")]
        public DateTime Fechainicio
        {
            get { return _fechainicio; }
            set { _fechainicio = value; }
        }

        private DateTime _fechafin = DateTime.Now;
        [Required]
        [Display(ResourceType = typeof(RAseguradoras), Name = "Fechafin")]
        public DateTime Fechafin
        {
            get { return _fechafin; }
            set { _fechafin = value; }
        }

        [Display(ResourceType = typeof(RAseguradoras), Name = "Diasimpago")]
        public int Diasimpago { get; set; }

        [Display(ResourceType = typeof(RAseguradoras), Name = "Diasimpagovencimientoprorrogado")]
        public int Diasimpagovencimientoprorrogado { get; set; }

        [Display(ResourceType = typeof(RAseguradoras), Name = "Numeroprorrogas")]
        public int Numeroprorrogas { get; set; }

        [Display(ResourceType = typeof(RAseguradoras), Name = "Primaanual")]
        public double Primaanual { get; set; }

        [Display(ResourceType = typeof(RAseguradoras), Name = "Numerorecibos")]
        public int Numerorecibos { get; set; }

        [Display(ResourceType = typeof(RAseguradoras), Name = "Porcentajecoberturariesgo")]
        [Range(0.0,100.0,ErrorMessageResourceType = typeof(Unobtrusive),ErrorMessageResourceName = "Range")]
        public double Porcentajecoberturariesgo { get; set; }

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

        #region Overrides

        public override void createNewPrimaryKey()
        {
            primaryKey = new [] { GetType().GetProperty("Fkcuentas") };
        }

        public override object generateId(string id)
        {
            return id;
        }

        public override string DisplayName => RAseguradoras.TituloEntidad;

        #endregion

       
    }
}
