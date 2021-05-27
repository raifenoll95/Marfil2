using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Marfil.Dom.ControlsUI.NifCif;
using Marfil.Dom.Persistencia.Model.Configuracion.Planesgenerales;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos;
using Resources;
using REmpresas= Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Empresas;
using RCliente = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Clientes;
namespace Marfil.Dom.Persistencia.Model.Configuracion.Empresa
{
    public enum CriterioIva
    {
        [StringValue(typeof(REmpresas), "CriterioIvaDevengo")]
        Devengo,
        [StringValue(typeof(REmpresas), "CriterioIvaCaja")]
        Caja
    }

    public enum LiquidacionIva
    {
        [StringValue(typeof(REmpresas), "LiquidacionIvaTrimestral")]
        Trimestral,
        [StringValue(typeof(REmpresas), "LiquidacionIvaMensual")]
        Mensual   
    }

    public class EmpresaModel : BaseModel<EmpresaModel, Empresas>
    {
        #region Members

        private readonly FModel fModel = new FModel();

        #endregion

        #region Properties

        public string Id { get; set; }

        [Required]
        [Display(ResourceType = typeof(REmpresas), Name = "Nombre")]
        public string Nombre { get; set; }


        [Display(ResourceType = typeof(REmpresas), Name = "Razonsocial")]
        public string Razonsocial { get; set; }

        [Display(ResourceType = typeof(RCliente), Name = "Nif")]
        public NifCifModel Nif { get; set; }


        [Required]
        [Display(ResourceType = typeof(REmpresas), Name = "Fkpais")]
        public string Fkpais { get; set; }

        [Required]
        [Display(ResourceType = typeof(REmpresas), Name = "Fkplangeneralcontable")]
        public string Fkplangeneralcontable { get; set; }

        public IEnumerable<TablasVariasPaisesModel>  Paises { get; set; }

        public IEnumerable<PlanesGeneralesModel> PlanesGenerales { get; set; }

        [Required]
        [Display(ResourceType = typeof(REmpresas), Name = "Administrador")]
        public string Administrador { get; set; }

        [Display(ResourceType = typeof(RCliente), Name = "Nif")]
        public string Nifdescripcion { get; set; }

        [Required]
        [Display(ResourceType = typeof(REmpresas), Name = "NifCifAdministrador")]
        public NifCifModel NifCifAdministrador { get; set; }

        [Display(ResourceType = typeof(REmpresas), Name = "Ean13")]
        public string Ean13 { get; set; }

        //actividad
        [Display(ResourceType = typeof(REmpresas), Name = "TipoEmpresa")]
        public string TipoEmpresa { get; set; }

        [Display(ResourceType = typeof(REmpresas), Name = "ActividadPrincipal")]
        public string ActividadPrincipal { get; set; }

        [Display(ResourceType = typeof(REmpresas), Name = "Cnae")]
        public string Cnae { get; set; }

        [Display(ResourceType = typeof(REmpresas), Name = "Nivel")]
        public string Nivel { get; set; }

        [Display(ResourceType = typeof(REmpresas), Name = "Criterioiva")]
        public CriterioIva Criterioiva { get; set; }

        [Display(ResourceType = typeof(REmpresas), Name = "Liquidacioniva")]
        public LiquidacionIva Liquidacioniva { get; set; }

        //Contabilidad
        [Required]
        [Display(ResourceType = typeof(REmpresas), Name = "FkMonedaBase")]
        public string FkMonedaBase { get; set; }

        [Display(ResourceType = typeof(REmpresas), Name = "FkMonedaAdicional")]
        public string FkMonedaAdicional { get; set; }

        [Required]
        [Display(ResourceType = typeof(REmpresas), Name = "DigitosCuentas")]
        public string DigitosCuentas { get; set; }

        [Required]
        [Display(ResourceType = typeof(REmpresas), Name = "NivelCuentas")]
        public string NivelCuentas { get; set; }

        [Display(ResourceType = typeof(REmpresas), Name = "CuentasAnuales")]
        public string CuentasAnuales { get; set; }

        [Display(ResourceType = typeof(REmpresas), Name = "CuentasPerdidas")]
        public string CuentasPerdidas { get; set; }

        [Display(ResourceType = typeof(REmpresas), Name = "Datosregistrales")]
        public string Datosregistrales { get; set; }

        [Required]
        [Display(ResourceType = typeof(REmpresas), Name = "Fktarifasventas")]
        public string Fktarifasventas { get; set; }

        [Required]
        [Display(ResourceType = typeof(REmpresas), Name = "Fktarifascompras")]
        public string Fktarifascompras { get; set; }

        [Display(ResourceType = typeof(REmpresas), Name = "Fkregimeniva")]
        public string Fkregimeniva { get; set; }


        [Display(ResourceType = typeof(REmpresas), Name = "FkCuentaEntradasVariasAlmacen")]
        public string FkCuentaEntradasVariasAlmacen { get; set; }


        [Display(ResourceType = typeof(REmpresas), Name = "FkCuentaSalidasVariasAlmacen")]
        public string FkCuentaSalidasVariasAlmacen { get; set; }

        [Display(ResourceType = typeof(REmpresas), Name = "Decimalesprecios")]
        public int? Decimalesprecios { get; set; }

        private DireccionesModel _direcciones;
        private IEnumerable<EjerciciosModel> _ejercicios= Enumerable.Empty<EjerciciosModel>();
        private EjerciciosModel _ejercicioNuevo=new EjerciciosModel();

        public DireccionesModel Direcciones
        {
            get
            {
                _direcciones = _direcciones ?? fModel.GetModel<DireccionesModel>(Context);
                return _direcciones;
            }
            set { _direcciones = value; }
        }

        public IEnumerable<EjerciciosModel> Ejercicios
        {
            get { return _ejercicios; }
            set { _ejercicios = value; }
        }

        public EjerciciosModel EjercicioNuevo
        {
            get { return _ejercicioNuevo; }
            set { _ejercicioNuevo = value; }
        }

        public int EstadoImportacion { get; set; }
        
        #endregion

        #region CTR

        public EmpresaModel()
        {
            Id = "-1";
        }

        public EmpresaModel(IContextService context) : base(context)
        {
            Id = "-1";
        }

        #endregion

        public override object generateId(string id)
        {
            return id;
        }

        public override string DisplayName => REmpresas.TituloEntidad;

        public IEnumerable<SelectListItem> LstTarifasCompras { get; set; }
        public IEnumerable<SelectListItem> LstTarifasVentas { get; set; }
    }
}
