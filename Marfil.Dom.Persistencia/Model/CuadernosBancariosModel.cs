using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RCuadernos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.CuadernosBancarios;

namespace Marfil.Dom.Persistencia.Model.Contabilidad
{


    public class CuadernosBancariosLinModel
    {
        public enum TipoCampos
        {
            [StringValue(typeof(RCuadernos), "Alfanumerico")]
            Alfanumerico,
            [StringValue(typeof(RCuadernos), "Numerico")]
            Numerico,
            [StringValue(typeof(RCuadernos), "FechaAAMMDD")]
            FechaAAMMDD,
            [StringValue(typeof(RCuadernos), "FechaAAAAMMDD")]
            FechaAAAAMMDD,
            [StringValue(typeof(RCuadernos), "Año")]
            Año,
            [StringValue(typeof(RCuadernos), "Mes")]
            Mes,
            [StringValue(typeof(RCuadernos), "Dia")]
            Dia,
            [StringValue(typeof(RCuadernos), "Binario")]
            Binario
        }

        #region properties
        [Display(ResourceType = typeof(RCuadernos), Name = "Id")]
        public int Id { get; set; }
        public string Empresa { get; set; }
        public string IdCab { get; set; }

        [Display(ResourceType = typeof(RCuadernos), Name = "Orden")]
        public short Orden { get; set; }
        public string Registro { get; set; }
        [Required]
        [Display(ResourceType = typeof(RCuadernos), Name = "Posicion")]
        public short Posicion { get; set; }
        [Required]
        [Display(ResourceType = typeof(RCuadernos), Name = "Longitud")]
        public short Longitud { get; set; }
        [Required]
        [Display(ResourceType = typeof(RCuadernos), Name = "TipoCampo")]
        public TipoCampos TipoCampo { get; set; }
        [Display(ResourceType = typeof(RCuadernos), Name = "EtiquetaIni")]
        public string EtiquetaIni { get; set; }
        [Display(ResourceType = typeof(RCuadernos), Name = "Campo")]
        public string Campo { get; set; }
        [Display(ResourceType = typeof(RCuadernos), Name = "Obligatorio")]
        public int Obligatorio { get; set; }
        [Display(ResourceType = typeof(RCuadernos), Name = "EtiquetaFin")]
        public string EtiquetaFin { get; set; }
        [Display(ResourceType = typeof(RCuadernos), Name = "Condicion")]
        public string Condicion { get; set; }
        [Display(ResourceType = typeof(RCuadernos), Name = "DescripcionLin")]
        public string DescripcionLin { get; set; }

        #endregion
    }

    public class CuadernosBancariosModel : BaseModel<CuadernosBancariosModel, CuadernosBancarios>
    {
        public enum TipoRegistros
        {
            [StringValue(typeof(RCuadernos), "Cabecera")]
            Cabecera,
            [StringValue(typeof(RCuadernos), "Detalle")]
            Detalle,
            [StringValue(typeof(RCuadernos), "Total")]
            Total
        }

        public enum TipoFormatos
        {
            [StringValue(typeof(RCuadernos), "Fijo")]
            Fijo,
            [StringValue(typeof(RCuadernos), "Variable")]
            Variable
        }

        public enum TipoCampos
        {
            [StringValue(typeof(RCuadernos), "Alfanumerico")]
            Alfanumerico,
            [StringValue(typeof(RCuadernos), "Numerico")]
            Numerico,
            [StringValue(typeof(RCuadernos), "FechaAAMMDD")]
            FechaAAMMDD,
            [StringValue(typeof(RCuadernos), "FechaAAAAMMDD")]
            FechaAAAAMMDD,
            [StringValue(typeof(RCuadernos), "Año")]
            Año,
            [StringValue(typeof(RCuadernos), "Mes")]
            Mes,
            [StringValue(typeof(RCuadernos), "Dia")]
            Dia,
            [StringValue(typeof(RCuadernos), "Binario")]
            Binario
        }
        #region CTR

        public CuadernosBancariosModel()
        {

        }

        public CuadernosBancariosModel(IContextService context) : base(context)
        {

        }

        #endregion

        #region properties
        private List<CuadernosBancariosLinModel> _lineas = new List<CuadernosBancariosLinModel>();

        [Required]
        [Display(ResourceType = typeof(RCuadernos), Name = "Id")]
        public int Id { get; set; }
        [Required]
        public string Empresa { get; set; }
        [Required]
        [Display(ResourceType = typeof(RCuadernos), Name = "Clave")]
        public string Clave { get; set; }
        [Display(ResourceType = typeof(RCuadernos), Name = "Descripcion")]
        public string Descripcion { get; set; }
        [Display(ResourceType = typeof(RCuadernos), Name = "Banco")]
        public string Banco { get; set; }
        [Display(ResourceType = typeof(RCuadernos), Name = "TipoRegistro")]
        public TipoRegistros TipoRegistro { get; set; }
        [Display(ResourceType = typeof(RCuadernos), Name = "TipoFormato")]
        public string TipoFormato { get; set; }
        [Display(ResourceType = typeof(RCuadernos), Name = "Formato")]
        public TipoFormatos Formato { get; set; }
        [Display(ResourceType = typeof(RCuadernos), Name = "Orden")]
        public short Orden { get; set; }
        [Display(ResourceType = typeof(RCuadernos), Name = "Posicion")]
        public short Posicion { get; set; }
        [Display(ResourceType = typeof(RCuadernos), Name = "Longitud")]
        public short Longitud { get; set; }
        [Display(ResourceType = typeof(RCuadernos), Name = "TipoCampo")]
        public TipoCampos TipoCampo { get; set; }
        [Display(ResourceType = typeof(RCuadernos), Name = "EtiquetaIni")]
        public string EtiquetaIni { get; set; }
        [Display(ResourceType = typeof(RCuadernos), Name = "Campo")]
        public string Campo { get; set; }
        [Display(ResourceType = typeof(RCuadernos), Name = "Obligatorio")]
        public int Obligatorio { get; set; }
        [Display(ResourceType = typeof(RCuadernos), Name = "EtiquetaFin")]
        public string EtiquetaFin { get; set; }
        [Display(ResourceType = typeof(RCuadernos), Name = "Condicion")]
        public string Condicion { get; set; }
        [Display(ResourceType = typeof(RCuadernos), Name = "DescripcionLin")]
        public string DescripcionLin { get; set; }
        [Display(ResourceType = typeof(RCuadernos), Name = "FechaCrea")]
        public DateTime FechaCrea { get; set; }
        [Display(ResourceType = typeof(RCuadernos), Name = "FechaMod")]
        public DateTime FechaMod { get; set; }
        [Display(ResourceType = typeof(RCuadernos), Name = "UsuarioCrea")]
        public string UsuarioCrea { get; set; }
        [Display(ResourceType = typeof(RCuadernos), Name = "UsuarioMod")]
        public string UsuarioMod { get; set; }
        public List<CuadernosBancariosLinModel> Lineas
        {
            get { return _lineas; }
            set { _lineas = value; }
        }
        #endregion

        public override string DisplayName => RCuadernos.TituloEntidad;

        public override object generateId(string id)
        {
            return id;
        }

    }
}
