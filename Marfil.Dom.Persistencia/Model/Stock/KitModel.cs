using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Marfil.Dom.Persistencia.Model.Diseñador;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos;
using Marfil.Inf.Genericos.Helper;
using Marfil.Inf.ResourcesGlobalization.Textos.Entidades;
using Resources;
using RKit = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Kit;
using RAlbaranes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Albaranes;
namespace Marfil.Dom.Persistencia.Model.Stock
{
    public enum EstadoKit
    {
        [StringValue(typeof(RKit), "EnumEnProceso")]
        EnProceso,
        [StringValue(typeof(RKit), "EnumMontado")]
        Montado,
        [StringValue(typeof(RKit), "EnumDesmontado")]
        Desmontado,
        [StringValue(typeof(RKit), "EnumVendido")]
        Vendido
    }

    public class KitLinModel
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fkalmacen")]
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
        public int? Cantidad { get; set; }

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

    public class KitModel : BaseModel<KitModel, Kit>
    {
        #region Members

        private List<KitLinModel> _lineas = new List<KitLinModel>();

        #endregion

        #region Properties

        public int Id { get; set; }

        [Required]
        [Display(ResourceType = typeof(RAlbaranes), Name = "Fkseries")]
        public string Fkseries { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Identificadorsegmento")]
        public string Identificadorsegmento { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Referencia")]
        public string Referencia { get; set; }

        [Required]
        [Display(ResourceType = typeof(RKit), Name = "Empresa")]
        public string Empresa { get; set; }
        
        [Display(ResourceType = typeof(RKit), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(RKit), Name = "Fecha")]
        public DateTime Fechadocumento { get; set; }

        [Display(ResourceType = typeof(RKit), Name = "Pesoneto")]
        public double? Pesoneto { get; set; }

        [Display(ResourceType = typeof(RKit), Name = "Pesobruto")]
        public double? Pesobruto { get; set; }

        [Required]
        [Display(ResourceType = typeof(RKit), Name = "Fkalmacen")]
        public string Fkalmacen { get; set; }

        [Display(ResourceType = typeof(RKit), Name = "Fkalmacen")]
        public string Fkalmacendescripcion { get; set; }

        [Display(ResourceType = typeof(RKit), Name = "Fkzonaalmacen")]
        public int? Fkzonalamacen { get; set; }

        public string Fkzonaalmacenvista
        {
            get { return Fkzonalamacen?.ToString() ?? string.Empty; }
            set { Fkzonalamacen = Funciones.Qint(value); }
        }

        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.Bundle), Name = "Fkzonaalmacendescripcion")]
        public string Fkzonaalmacendescripcion { get; set; }

        [Display(ResourceType = typeof(RKit), Name = "Fkoperarios")]
        public string Fkoperarios { get; set; }

        [Display(ResourceType = typeof(RKit), Name = "Fkoperarios")]
        public string Fkoperariosdescripcion { get; set; }

        [Display(ResourceType = typeof(RKit), Name = "Notas")]
        public string Notas { get; set; }

        [Display(ResourceType = typeof(RKit), Name = "Estado")]
        public EstadoKit Estado { get; set; }

        [Display(ResourceType = typeof(RKit), Name = "Piezas")]
        public int? Piezas { get; set; }

        public List<KitLinModel> Lineas
        {
            get { return _lineas; }
            set { _lineas = value; }
        }

        #endregion

        public override object generateId(string id)
        {
            return id;
        }

        public override string DisplayName => RKit.TituloEntidad;

        public DocumentosBotonImprimirModel DocumentosImpresion { get; set; }

        #region CTR

        public KitModel()
        {
           
        }

        public KitModel(IContextService context) : base(context)
        {

        }

        #endregion
    }
}
