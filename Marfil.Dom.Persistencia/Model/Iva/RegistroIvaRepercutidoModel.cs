using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RRegistroIVA = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.RegistroIvaRepercutido;
using Marfil.Inf.Genericos;
using System.ComponentModel.DataAnnotations;
using RFacturas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Facturas;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.Model.Iva
{
    public enum TipoOrigen
    {
        [StringValue(typeof(RRegistroIVA), "Factura")]
        Factura,
        [StringValue(typeof(RRegistroIVA), "IVA")]
        IVA
    }

    public enum TipoEnvio
    {
        [StringValue(typeof(RRegistroIVA), "Alta")]
        A0,
        [StringValue(typeof(RRegistroIVA), "Errores")]
        A1
    }

    public enum TipoEstadoEnvio
    {
        [StringValue(typeof(RRegistroIVA), "Pendiente")]
        Pendiente,
        [StringValue(typeof(RRegistroIVA), "Aceptada")]
        Aceptada,
        [StringValue(typeof(RRegistroIVA), "Rechazada")]
        Rechazada
    }

    public enum TipoCriterioIva
    {
        [StringValue(typeof(RRegistroIVA), "Devengo")]
        Devengo,
        [StringValue(typeof(RRegistroIVA), "Caja")]
        Caja
    }

    public enum TipoIdentificacion
    {
        [StringValue(typeof(RRegistroIVA), "Nif")]
        Nif,
        [StringValue(typeof(RRegistroIVA), "Otro")]
        Otro
    }
    public enum TipoFacturaRectificativa
    {
        [StringValue(typeof(RRegistroIVA), "Sustitucion")]
        Sustitucion,
        [StringValue(typeof(RRegistroIVA), "Diferencias")]
        Diferencias
    }


    public class RegistroIvaRepercutidoModel : BaseModel<RegistroIvaRepercutidoModel, RegistroIVARepercutido>
    {
        #region CTR
        public RegistroIvaRepercutidoModel()
        {
        }

        public RegistroIvaRepercutidoModel(IContextService context) : base(context)
        {
        }
        #endregion

        #region properties

        private List<RegistroIvaRepercutidoTotalesModel> _totales = new List<RegistroIvaRepercutidoTotalesModel>();

        
        public int? Id { get; set; }

        [Required]
        public string Empresa { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Origendoc")]
        public TipoOrigen Origendoc { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Tipofactura")]
        public string Tipofactura { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Referencia")]
        public string Referencia { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Fecharegistro")]
        public DateTime Fecharegistro { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Fechafactura")]
        public DateTime Fechafactura { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Fechaoperacion")]
        public DateTime Fechaoperacion { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Periodo")]
        public string Periodo { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Numfacturacliente")]
        public string Numfacturacliente { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Cuentacliente")]
        public string Cuentacliente { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Cuentaventas")]
        public string Cuentaventas { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Criterioivageneral")]
        public TipoCriterioIva Criterioivageneral { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Fkregimeniva")]
        public string Fkregimeniva { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Facturarectificativa")]
        public bool Facturarectificativa { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Numfacturarectificar")]
        public string Numfacturarectificar { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Fechafacturaoriginal")]
        public DateTime Fechafacturaoriginal { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Motivorectificacion")]
        public string Motivorectificacion { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Criterioivafactoriginal")]
        public TipoCriterioIva Criterioivafactoriginal { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Criterioivafactura")]
        public TipoCriterioIva Criterioivafactura { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Bieninversion")]
        public bool Bieninversion { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Fktiporetencion")]
        public string Fktiporetencion { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Baseretencion")]
        public double Baseretencion { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Porcentajeretencion")]
        public double Porcentajeretencion { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Importeretencion")]
        public double Importeretencion { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Inmueble")]
        public string Inmueble { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Contabilizar")]
        public bool Contabilizar { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Fkcuentastesoreria")]
        public string Fkcuentastesoreria { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Operacionesexluidasbi")]
        public double Operacionesexluidasbi { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Totalfactura")]
        public double Totalfactura { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Tipoenvio")]
        public TipoEnvio Tipoenvio { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Fechaalta")]
        public DateTime Fechaalta { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Estado")]
        public TipoEstadoEnvio Estado { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Csv")]
        public string Csv { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Descripcionerror")]
        public string Descripcionerror { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Variosdestinatarios")]
        public bool Variosdestinatarios { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Cuentaclientecontraparte")]
        public string Cuentaclientecontraparte { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Nombrecontraparte")]
        public string Nombrecontraparte { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Tipoidentificacion")]
        public TipoIdentificacion Tipoidentificacion { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Codigopais")]
        public string Codigopais { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Nifcontraparte")]
        public string Nifcontraparte { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Tipoidentificacionotro")]
        public string Tipoidentificacionotro { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Identificacion")]
        public string Identificacion { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Clavetipofactura")]
        public string Clavetipofactura { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Tipofacturarectificativa")]
        public TipoFacturaRectificativa Tipofacturarectificativa { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Baserectificada")]
        public double Baserectificada { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Cuotaivarectificada")]
        public double Cuotaivarectificada { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Cuotarectificadaeq")]
        public double Cuotarectificadaeq { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Facturaemitidaterceros")]
        public bool Facturaemitidaterceros { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Importetotalemisor")]
        public double Importetotalemisor { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Importetransmisoremisor")]
        public double Importetransmisoremisor { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Claveregimenespecial")]
        public string Claveregimenespecial { get; set; }

        [Display(ResourceType = typeof(RRegistroIVA), Name = "Descripcionoperacionemisor")]
        public string Descripcionoperacionemisor { get; set; }
        #endregion

        #region Totales

        public List<RegistroIvaRepercutidoTotalesModel> Totales
        {
            get { return _totales; }
            set { _totales = value; }
        }

        #endregion

        #region atributos

        public override object generateId(string id)
        {
            return id;
        }

        public override void createNewPrimaryKey()
        {
            primaryKey = getProperties().Where(f => f.property.Name.ToLower() == "id").Select(f => f.property);
        }

        public override string GetPrimaryKey()
        {
            return Id.ToString();
        }

        public override string DisplayName => RRegistroIVA.TituloEntidad;

        #endregion

    }

    public class RegistroIvaRepercutidoTotalesModel
    {
        public int? Decimalesmonedas { get; set; }

        public int Id { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Fktiposiva")]
        public string Fktiposiva { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Porcentajeiva")]
        public double? Porcentajeiva { get; set; }

        //[Display(ResourceType = typeof(RFacturas), Name = "BrutoTotal")]
        //public double? Brutototal { get; set; }

        //[Display(ResourceType = typeof(RFacturas), Name = "BrutoTotal")]
        //public string SBrutototal
        //{
        //    get { return (Brutototal ?? 0.0).ToString("N" + Decimalesmonedas); }
        //    set { Brutototal = Funciones.Qdouble(value); }
        //}

        [Display(ResourceType = typeof(RFacturas), Name = "Basetotal")]
        public double? Baseimponible { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Basetotal")]
        public string SBaseimponible
        {
            get { return (Baseimponible ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Baseimponible = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RFacturas), Name = "Cuotaiva")]
        public double? Cuotaiva { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Cuotaiva")]
        public string SCuotaiva
        {
            get { return (Cuotaiva ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Cuotaiva = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RFacturas), Name = "Porcentajerecargoequivalencia")]
        public double? Porcentajerecargoequivalencia { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Cuotarecargoequivalencia")]
        public double? Importerecargoequivalencia { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Cuotarecargoequivalencia")]
        public string SImporterecargoequivalencia
        {
            get { return (Importerecargoequivalencia ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Importerecargoequivalencia = Funciones.Qdouble(value); }
        }

        //[Display(ResourceType = typeof(RFacturas), Name = "Porcentajedescuentoprontopago")]
        //public double? Porcentajedescuentoprontopago { get; set; }

        //[Display(ResourceType = typeof(RFacturas), Name = "Importedescuentoprontopago")]
        //public double? Importedescuentoprontopago { get; set; }

        //[Display(ResourceType = typeof(RFacturas), Name = "Importedescuentoprontopago")]
        //public string SImportedescuentoprontopago
        //{
        //    get { return (Importedescuentoprontopago ?? 0.0).ToString("N" + Decimalesmonedas); }
        //    set { Importedescuentoprontopago = Funciones.Qdouble(value); }
        //}

        //[Display(ResourceType = typeof(RFacturas), Name = "Porcentajedescuentocomercial")]
        //public double? Porcentajedescuentocomercial { get; set; }

        //[Display(ResourceType = typeof(RFacturas), Name = "Importedescuentocomercial")]
        //public double? Importedescuentocomercial { get; set; }

        //[Display(ResourceType = typeof(RFacturas), Name = "Importedescuentocomercial")]
        //public string SImportedescuentocomercial
        //{
        //    get { return (Importedescuentocomercial ?? 0.0).ToString("N" + Decimalesmonedas); }
        //    set { Importedescuentocomercial = Funciones.Qdouble(value); }
        //}

        //[Display(ResourceType = typeof(RFacturas), Name = "Baseretencion")]
        //public double? Baseretencion { get; set; }

        //[Display(ResourceType = typeof(RFacturas), Name = "Baseretencion")]
        //public string SBaseretencion
        //{
        //    get { return (Baseretencion ?? 0.0).ToString("N" + Decimalesmonedas); }
        //    set { Baseretencion = Funciones.Qdouble(value); }
        //}

        //[Display(ResourceType = typeof(RFacturas), Name = "Porcentajeretencion")]
        //public double? Porcentajeretencion { get; set; }

        //[Display(ResourceType = typeof(RFacturas), Name = "Importeretencion")]
        //public double? Importeretencion { get; set; }

        //[Display(ResourceType = typeof(RFacturas), Name = "Importeretencion")]
        //public string SImporteretencion
        //{
        //    get { return (Importeretencion ?? 0.0).ToString("N" + Decimalesmonedas); }
        //    set { Importeretencion = Funciones.Qdouble(value); }
        //}

        [Display(ResourceType = typeof(RFacturas), Name = "Subtotal")]
        public double? Subtotal { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Subtotal")]
        public string SSubtotal
        {
            get { return (Subtotal ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Subtotal = Funciones.Qdouble(value); }
        }
    }
}
