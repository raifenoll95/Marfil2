using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;
using ROperarios = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Operarios;
using RDirecciones = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Direcciones;
using RAcreedores = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Acreedores;
namespace Marfil.Dom.Persistencia.Model.Terceros
{
    public class OperariosBusqueda : IModelView
    {
        [Display(ResourceType = typeof(ROperarios), Name = "Fkcuentas")]
        public string Fkcuentas { get; set; }

        [Display(ResourceType = typeof(ROperarios), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(ROperarios), Name = "Nif")]
        public string Nif { get; set; }

        [Display(ResourceType = typeof(ROperarios), Name = "Numeroseguridadsocial")]
        public string Numeroseguridadsocial { get; set; }

        [Display(ResourceType = typeof(ROperarios), Name = "Poblacion")]
        public string Poblacion { get; set; }

        public bool? Bloqueado { get; set; }



        public IEnumerable<ViewProperty> getProperties()
        {
            var listNames = typeof(OperariosBusqueda).GetProperties().Select(f => f.Name).Except(typeof(IModelView).GetProperties().Select(h => h.Name));
            var properties = typeof(OperariosBusqueda).GetProperties().Where(f => listNames.Any(h => h == f.Name));

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
    public class OperariosModel : BaseModel<OperariosModel, Operarios>
    {
        private readonly FModel fModel = new FModel();
        
        #region CTR

        public OperariosModel()
        {
            
        }

        public OperariosModel(IContextService context) : base(context)
        {
            _direcciones = fModel.GetModel<DireccionesModel>(context);
            _bancosMandatos = fModel.GetModel<BancosMandatosModel>(context);

        }

        #endregion

        #region Properties

        //cuenta

        [Required]
        public string Empresa { get; set; }

        [Required]
        [Display(ResourceType = typeof(ROperarios), Name = "Fkcuentas")]
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

        [Display(ResourceType = typeof(ROperarios), Name = "Descripcion")]
        public string Descripcion { get { return Cuentas?.Descripcion; } }

        [Display(ResourceType = typeof(ROperarios), Name = "RazonSocial")]
        public string RazonSocial { get { return Cuentas?.Descripcion2; } }

        public bool Bloqueado
        {
            get { return Cuentas?.Bloqueado ?? false; }
        }

        //operarios

        [Display(ResourceType = typeof(ROperarios), Name = "Nif")]
        public string Nif
        {
            get { return Cuentas?.Nif?.Nif; }
        }

        [MaxLength(20,ErrorMessageResourceType = typeof(Unobtrusive),ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(ROperarios), Name = "Numeroseguridadsocial")]
        public string Numeroseguridadsocial { get; set; }

        [Display(ResourceType = typeof(ROperarios), Name = "Fechanacimiento")]
        public DateTime? Fechanacimiento { get; set; }

        [Display(ResourceType = typeof(ROperarios), Name = "Fkestadocivil")]
        public string Fkestadocivil { get; set; }

        [Display(ResourceType = typeof(ROperarios), Name = "Numerohijos")]
        public int? Numerohijos { get; set; }

        [Display(ResourceType = typeof(ROperarios), Name = "Fechaingreso")]
        public DateTime? Fechaingreso { get; set; }

        [Display(ResourceType = typeof(ROperarios), Name = "Fkcargo")]
        public string Fkcargo { get; set; }

        [Display(ResourceType = typeof(ROperarios), Name = "Fkcontratoactual")]
        public string Fkcontratoactual { get; set; }

        [Display(ResourceType = typeof(ROperarios), Name = "Fechaalta")]
        public DateTime? Fechaalta { get; set; }

        [Display(ResourceType = typeof(ROperarios), Name = "Vencimientocontrato")]
        public DateTime? Vencimientocontrato { get; set; }

        [Display(ResourceType = typeof(ROperarios), Name = "Ultimafechabaja")]
        public DateTime? Ultimafechabaja { get; set; }

        [Display(ResourceType = typeof(ROperarios), Name = "Ultimafechaalta")]
        public DateTime? Ultimafechaalta { get; set; }

        [MaxLength(4, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(ROperarios), Name = "Tallacamisa")]
        public string Tallacamisa { get; set; }

        [MaxLength(4, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(ROperarios), Name = "Tallapantalon")]
        public string Tallapantalon { get; set; }

        [MaxLength(4, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(ROperarios), Name = "Tallacalzado")]
        public string Tallacalzado { get; set; }

        [Display(ResourceType = typeof(ROperarios), Name = "Notas")]
        public string Notas { get; set; }

        [Display(ResourceType = typeof(ROperarios), Name = "Fkcuentatesoreria")]
        public string Fkcuentatesoreria { get; set; }


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

        public override object generateId(string id)
        {
            return id;
        }

        public override string DisplayName => ROperarios.TituloEntidad;
    }
}
