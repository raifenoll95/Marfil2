using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Resources;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using RAcreedores = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Acreedores;
using RCuentastesoreria = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Cuentastesoreria;
using RDirecciones = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Direcciones;
namespace Marfil.Dom.Persistencia.Model.Terceros
{

    public class CuentastesoreriaModel : BaseModel<CuentastesoreriaModel, Cuentastesoreria>
    {
        private readonly FModel _fModel = new FModel();
       
        #region CTR

        public CuentastesoreriaModel()
        {
            
        }

        public CuentastesoreriaModel(IContextService context) : base(context)
        {
            _direcciones = _fModel.GetModel<DireccionesModel>(context);
            _contactos = _fModel.GetModel<ContactosModel>(context);
            _bancosMandatos = _fModel.GetModel<BancosMandatosModel>(context);

        }

        #endregion

        #region Properties

        [Required]
        public string Empresa { get; set; }

        [Required]
        [Display(ResourceType = typeof(RAcreedores), Name = "Fkcuentas")]
        public string Fkcuentas { get; set; }

        public CuentasModel Cuentas { get; set; }

        [Display(ResourceType = typeof(RAcreedores), Name = "Descripcion")]
        public string Descripcion { get { return Cuentas?.Descripcion; } }

        [Display(ResourceType = typeof(RAcreedores), Name = "RazonSocial")]
        public string RazonSocial { get { return Cuentas?.Descripcion2; } }

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

        private ContactosModel _contactos;
        public ContactosModel Contactos
        {
            get
            {
                _contactos = _contactos ?? _fModel.GetModel<ContactosModel>(Context);
                return _contactos;
            }
            set { _contactos = value; }
        }

        private DireccionesModel _direcciones;
        public DireccionesModel Direcciones
        {
            get
            {
                _direcciones = _direcciones ?? _fModel.GetModel<DireccionesModel>(Context);
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
                _bancosMandatos = _bancosMandatos ?? _fModel.GetModel<BancosMandatosModel>(Context);
                return _bancosMandatos;
            }
            set { _bancosMandatos = value; }
        }

        #endregion

        public override void createNewPrimaryKey()
        {
            primaryKey = new[] { GetType().GetProperty("Fkcuentas") };
        }

        public override string DisplayName => RCuentastesoreria.TituloEntidad;

        public override object generateId(string id)
        {
            return id.Split('-');
        }
    }
}
