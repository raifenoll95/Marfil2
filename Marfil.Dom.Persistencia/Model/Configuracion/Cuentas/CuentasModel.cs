using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.ControlsUI.Bloqueo;
using Marfil.Dom.ControlsUI.NifCif;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RCuentas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Cuentas;
namespace Marfil.Dom.Persistencia.Model.Configuracion.Cuentas
{
    [Serializable]
    public class CuentasModel : BaseModel<CuentasModel, Persistencia.Cuentas>
    {

        #region Properties
        [Required]
        public string Empresa { get; set; }

        [Required]
        [Display(ResourceType = typeof(RCuentas), Name = "Id")]
        [MaxLength(15, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        public string Id { get; set; }

        [Display(ResourceType = typeof(RCuentas), Name = "Descripcion2")]
        public string Descripcion2 { get; set; }

        [Required]
        [Display(ResourceType = typeof(RCuentas), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Required]
        [Display(ResourceType = typeof(RCuentas), Name = "Nivel")]
        public int Nivel { get; set; }

        [Display(ResourceType = typeof(RCuentas), Name = "Tiposcuentas")]
        public int? Tiposcuentas { get; set; }

        [Display(ResourceType = typeof(RCuentas), Name = "FkPais")]
        public string FkPais { get; set; }

        [Display(ResourceType = typeof(RCuentas), Name = "Nifrequired")]
        public bool Nifrequired { get; set; }


        [Display(ResourceType = typeof(RCuentas), Name = "Nif")]
        public NifCifModel Nif { get; set; }

        [Display(ResourceType = typeof(RCuentas), Name = "Contrapartida")]
        public string Contrapartida { get; set; }

        [Display(ResourceType = typeof(RCuentas), Name = "ContrapartidaDescripcion")]
        public string ContrapartidaDescripcion { get; set; }

        //bloqueo

        [Display(ResourceType = typeof(RCuentas), Name = "BloqueoModel")]
        public BloqueoEntidadModel BloqueoModel { get; set; }

        public bool Bloqueado
        {
            get { return BloqueoModel?.Bloqueada ?? false; }
        }

        [Display(ResourceType = typeof(RCuentas), Name = "Usuario")]
        public string Usuario { get; set; }

        public string UsuarioId { get; set; }

        [Display(ResourceType = typeof(RCuentas), Name = "DescripcionLarga")]
        public string DescripcionLarga
        {
            get { return Id + " - " + Descripcion; }
        }

        [Display(ResourceType = typeof(RCuentas), Name = "FechaModificacion")]
        public DateTime FechaModificacion { get; set; }

        #endregion

        #region CTR

        public CuentasModel()
        {

        }

        public CuentasModel(IContextService context) : base(context)
        {

        }

        #endregion

        public override object generateId(string id)
        {
            return id;
        }

        public override string GetPrimaryKey()
        {
            return Id;
        }

        public override string DisplayName => RCuentas.TituloEntidad;
        public DateTime Fechaalta { get; set; }
    }

    public class CuentasRegularizacionModel : CuentasModel
    {
        [Display(ResourceType = typeof(RCuentas), Name = "Id")]
        public string Cuentaexistencias { get; set; }
        public decimal? Saldoexistenciasiniciales { get; set; }
        public string SSaldoexistenciasiniciales
        {
            ///get { return (Importe ?? 0).ToString(string.Format("N{0}", (Decimalesmonedas ?? 0))); }
            get { return (Saldoexistenciasiniciales ?? 0).ToString(string.Format("N{0}", (2))); }
            set { Saldoexistenciasiniciales = (Funciones.Qdecimal(value) ?? 0); }
        }
        public string Cuentavariacion { get; set; }
        public decimal? Saldoexistenciasfinales { get; set; }
        public string SSaldoexistenciasfinales
        {
            ///get { return (Importe ?? 0).ToString(string.Format("N{0}", (Decimalesmonedas ?? 0))); }
            get { return (Saldoexistenciasfinales ?? 0).ToString(string.Format("N{0}", (2))); }
            set { Saldoexistenciasfinales = (Funciones.Qdecimal(value) ?? 0); }
        }

    }

    public class CuentasRegularizacionGruposModel : CuentasModel
    {
        [Display(ResourceType = typeof(RCuentas), Name = "Id")]
        public string Cuentagrupos { get; set; }
        public decimal? SaldoDeudor { get; set; }
        public string SSaldoDeudor
        {
            ///get { return (Importe ?? 0).ToString(string.Format("N{0}", (Decimalesmonedas ?? 0))); }
            get { return (SaldoDeudor ?? 0).ToString(string.Format("N{0}", (2))); }
            set { SaldoDeudor = (Funciones.Qdecimal(value) ?? 0); }
        }
        public decimal? SaldoAcreedor { get; set; }
        public string SSaldoAcreedor
        {
            ///get { return (Importe ?? 0).ToString(string.Format("N{0}", (Decimalesmonedas ?? 0))); }
            get { return (SaldoAcreedor ?? 0).ToString(string.Format("N{0}", (2))); }
            set { SaldoAcreedor = (Funciones.Qdecimal(value) ?? 0); }
        }

    }
}
