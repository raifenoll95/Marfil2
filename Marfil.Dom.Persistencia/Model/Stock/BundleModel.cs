using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Marfil.Dom.Persistencia.Model.Diseñador;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos.Helper;
using Marfil.Inf.ResourcesGlobalization.Textos.Entidades;
using Resources;
using RBundle = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Bundle;
using RAlbaranes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Albaranes;
using RKit = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Kit;
namespace Marfil.Dom.Persistencia.Model.Stock
{
    public class BundleLinModel
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(RBundle), Name = "Fkalmacen")]
        public string Fkalmacenes { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Lote")]
        public string Lote { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Loteid")]
        public string Loteid { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fkarticulos")]
        public string Fkarticulos { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Coste")]
        public double? Coste { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Cantidad")]
        public double? Cantidad { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Largo")]
        public double? Largo { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Ancho")]
        public double? Ancho { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Grueso")]
        public double? Grueso { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Metros")]
        public double? Metros { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fkunidades")]
        public string Fkunidades { get; set; }

        public int? Decimalesunidades { get; set; }

        public int? Decimalesprecio { get; set; }
    }

    public class BundleModel : BaseModel<BundleModel, Bundle>
    {
        #region Members

        private List<BundleLinModel> _lineas = new List<BundleLinModel>();

        #endregion

        #region Properties

        public string CampoId
        {
            get { return Lote + ";" + Codigo; }
        }

        [Required]
        [Display(ResourceType = typeof(RBundle), Name = "Empresa")]
        public string Empresa { get; set; }

        [Required]
        [Display(ResourceType = typeof(RAlbaranes), Name = "Lote")]
        public string Lote { get; set; }

        [Required]
        [Display(ResourceType = typeof(RBundle), Name = "Id")]
        public string Codigo { get; set; }

        
        [Display(ResourceType = typeof(RBundle), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(RKit), Name = "Pesoneto")]
        public double? Pesoneto { get; set; }

        [Display(ResourceType = typeof(RKit), Name = "Pesobruto")]
        public double? Pesobruto { get; set; }

        [Required]
        [Display(ResourceType = typeof(RBundle), Name = "Fecha")]
        public DateTime Fecha { get; set; }

        [Required]
        [Display(ResourceType = typeof(RBundle), Name = "Fkalmacen")]
        public string Fkalmacen { get; set; }

        [Display(ResourceType = typeof(RBundle), Name = "Fkalmacen")]
        public string Fkalmacendescripcion { get; set; }

        [Display(ResourceType = typeof(RBundle), Name = "Fkzonaalmacen")]
        public int? Fkzonaalmacen { get; set; }

        [Display(ResourceType = typeof(RKit), Name = "Estado")]
        public EstadoKit Estado { get; set; }

        public string Fkzonaalmacenvista
        {
            get { return Fkzonaalmacen?.ToString() ?? string.Empty; }
            set { Fkzonaalmacen = Funciones.Qint(value); }
        }

        [Display(ResourceType = typeof(RBundle), Name = "Fkzonaalmacendescripcion")]
        public string Fkzonaalmacendescripcion { get; set; }

        
        [Display(ResourceType = typeof(RBundle), Name = "Fkoperarios")]
        public string Fkoperarios { get; set; }

        [Display(ResourceType = typeof(RBundle), Name = "Fkoperarios")]
        public string Fkoperariosdescripcion { get; set; }

        [Display(ResourceType = typeof(RBundle), Name = "Notas")]
        public string Notas { get; set; }

        [Display(ResourceType = typeof(RBundle), Name = "Piezas")]
        public int? Piezas { get; set; }

        public List<BundleLinModel> Lineas
        {
            get { return _lineas; }
            set { _lineas = value; }
        }

        public DocumentosBotonImprimirModel DocumentosImpresion { get; set; }


        #endregion

        public override object generateId(string id)
        {
            return id;
        }

        public override string DisplayName => RBundle.TituloEntidad;

        public override string GetPrimaryKey()
        {
            return Lote + ";" + Codigo;
        }

        #region CTR

        public BundleModel()
        {

        }

        public BundleModel(IContextService context) : base(context)
        {

        }

        #endregion
    }
}
