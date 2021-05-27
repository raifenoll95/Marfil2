using Marfil.Dom.Persistencia.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;
using RComerciales= Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Comerciales;
using RDirecciones = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Direcciones;
using RAcreedores = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Acreedores;
namespace Marfil.Dom.Persistencia.Model.Terceros
{
    public class ComercialesModel : BaseModel<ComercialesModel, Comerciales>
    {
        private readonly FModel fModel = new FModel();

        #region CTR

        public ComercialesModel()
        {
            
        }

        public ComercialesModel(IContextService context) : base(context)
        {
            _direcciones = fModel.GetModel<DireccionesModel>(context);
            _bancosMandatos = fModel.GetModel<BancosMandatosModel>(context);

        }

        #endregion

        #region Properties

        [Required]
        public string Empresa { get; set; }

        [Required]
        [Display(ResourceType = typeof(RComerciales), Name = "Fkcuentas")]
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

        [Display(ResourceType = typeof(RComerciales), Name = "Descripcion")]
        public string Descripcion { get { return Cuentas?.Descripcion; } }

        [Display(ResourceType = typeof(RComerciales), Name = "RazonSocial")]
        public string RazonSocial { get { return Cuentas?.Descripcion2; } }

        public bool Bloqueado
        {
            get { return Cuentas?.Bloqueado ?? false; }
        }
        
        [Display(ResourceType = typeof(RComerciales), Name = "Fktipoirpf")]
        public string Fktipoirpf { get; set; }

        [Required]
        [Display(ResourceType = typeof(RComerciales), Name = "Fkregimeniva")]
        public string Fkregimeniva { get; set; }

        [Display(ResourceType = typeof(RComerciales), Name = "Fkformapago")]
        public string Fkformapago { get; set; }

        [Range(0, 100, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "Range")]
        [Display(ResourceType = typeof(RComerciales), Name = "Porcentajecomision")]
        public double? Porcentajecomision { get; set; }

        [Display(ResourceType = typeof(RComerciales), Name = "Comisionporm2")]
        public double? Comisionporm2 { get; set; }

        [Display(ResourceType = typeof(RComerciales), Name = "Comisionporm3")]
        public double? Comisionporm3 { get; set; }

        [Range(0,100,ErrorMessageResourceType = typeof(Unobtrusive),ErrorMessageResourceName = "Range")]
        [Display(ResourceType = typeof(RComerciales), Name = "Porcentajeincrementosobreptb")]
        public double? Porcentajeincrementosobreptb { get; set; }

        [Range(0, 100, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "Range")]
        [Display(ResourceType = typeof(RComerciales), Name = "Porcentajedecrementosobreptb")]
        public double? Porcentajedecrementosobreptb { get; set; }

        [Display(ResourceType = typeof(RComerciales), Name = "Primaincrementosobreptb")]
        public double? Primaincrementosobreptb { get; set; }

        [Display(ResourceType = typeof(RComerciales), Name = "Primadecrementosobreptb")]
        public double? Primadecrementosobreptb { get; set; }

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

        public override string DisplayName => RComerciales.TituloEntidad;

        public override object generateId(string id)
        {
            return id.Split('-');
        }
    }
}
