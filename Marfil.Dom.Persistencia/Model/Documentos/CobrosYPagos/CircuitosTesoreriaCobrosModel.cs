using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;
using RCircuitosTesoreria = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.CircuitosTesoreria;
using RMovs = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Movs;
using Marfil.Inf.Genericos;

namespace Marfil.Dom.Persistencia.Model.Documentos.CobrosYPagos
{

    public enum TipoImporte
    {
        [StringValue(typeof(RCircuitosTesoreria), "Blanco")]
        Blanco,
        [StringValue(typeof(RCircuitosTesoreria), "Importelineapunteada")]
        Importelineapunteada,
        [StringValue(typeof(RCircuitosTesoreria), "Sumaimportelineaspunteadas")]
        Sumaimportelineaspunteadas,
        [StringValue(typeof(RCircuitosTesoreria), "Importecuentacargo2")]
        Importecuentacargo2,
        [StringValue(typeof(RCircuitosTesoreria), "Importe2menosimporte3")]
        Importe2menosimporte3,
        [StringValue(typeof(RCircuitosTesoreria), "Importe2masimporte3")]
        Importe2masimporte3,
        [StringValue(typeof(RCircuitosTesoreria), "Importecuentaabono2")]
        Importecuentaabono2,
        [StringValue(typeof(RCircuitosTesoreria), "Importe2menosimporte6")]
        Importe2menosimporte6,
        [StringValue(typeof(RCircuitosTesoreria), "Importe2masimporte6")]
        Importe2masimporte6
    }

    public enum TipoCircuito
    {
        [StringValue(typeof(RCircuitosTesoreria), "Cobros")]
        Cobros,
        [StringValue(typeof(RCircuitosTesoreria), "Pagos")]
        Pagos
    }


    public class CircuitoTesoreriaCobrosModel : BaseModel<CircuitoTesoreriaCobrosModel, Persistencia.CircuitosTesoreriaCobros>
    {

        #region CTR
        public CircuitoTesoreriaCobrosModel()
        {
        }

        public CircuitoTesoreriaCobrosModel(IContextService context) : base(context)
        {
        }
        #endregion

        #region properties

        public int? Id { get; set; }

        public string Empresa { get; set; }

        [Required]
        [Display(ResourceType = typeof(RCircuitosTesoreria), Name = "Tipocircuito")]
        public TipoCircuito Tipocircuito { get; set; }

        //Factura a la que hace referecia
        [Display(ResourceType = typeof(RCircuitosTesoreria), Name = "Descripcion")]
        public String Descripcion { get; set; }

        [Display(ResourceType = typeof(RCircuitosTesoreria), Name = "Situacioninicial")]
        public String Situacioninicial { get; set; }

        [Display(ResourceType = typeof(RCircuitosTesoreria), Name = "Situacionfinal")]
        public String Situacionfinal { get; set; }

        [Display(ResourceType = typeof(RCircuitosTesoreria), Name = "Situacioninicial")]
        public string Situacioninicialdescripcion { get; set; }

        [Display(ResourceType = typeof(RCircuitosTesoreria), Name = "Situacionfinal")]
        public string Situacionfinaldescripcion { get; set; }

        [Display(ResourceType = typeof(RCircuitosTesoreria), Name = "DescripcionAsiento")]
        public string Codigodescripcionasiento { get; set; }

        //Modo de pago preferido
        [Display(ResourceType = typeof(RCircuitosTesoreria), Name = "Fkmodopagopreferido")]
        public string Fkmodopagopreferido { get; set; }

        [Display(ResourceType = typeof(RCircuitosTesoreria), Name = "Datos")]
        public int? Datos { get; set; }

        [Display(ResourceType = typeof(RCircuitosTesoreria), Name = "Asientocontable")]
        public bool Asientocontable { get; set; }

        [Display(ResourceType = typeof(RCircuitosTesoreria), Name = "Fecharemesa")]
        public bool Fecharemesa { get; set; }

        [Display(ResourceType = typeof(RCircuitosTesoreria), Name = "Fechapago")]
        public bool Fechapago { get; set; }

        [Display(ResourceType = typeof(RCircuitosTesoreria), Name = "Liquidariva")]
        public bool Liquidariva { get; set; }

        [Display(ResourceType = typeof(RCircuitosTesoreria), Name = "Conciliacion")]
        public bool Conciliacion { get; set; }

        [Display(ResourceType = typeof(RCircuitosTesoreria), Name = "Datosdocumento")]
        public bool Datosdocumento { get; set; }

        [Display(ResourceType = typeof(RCircuitosTesoreria), Name = "Actualizarcobrador")]
        public bool Actualizarcobrador { get; set; }

        [Display(ResourceType = typeof(RCircuitosTesoreria), Name = "Documentocartera")]
        public bool Documentocartera { get; set; }

        [Display(ResourceType = typeof(RCircuitosTesoreria), Name = "Cuentacargo1")]
        public String Cuentacargo1 { get; set; }

        [Display(ResourceType = typeof(RCircuitosTesoreria), Name = "Cuentacargo2")]
        public String Cuentacargo2 { get; set; }

        [Display(ResourceType = typeof(RCircuitosTesoreria), Name = "Cuentacargorel")]
        public String Cuentacargorel { get; set; }

        [Display(ResourceType = typeof(RCircuitosTesoreria), Name = "Cuentaabono1")]
        public String Cuentaabono1 { get; set; }

        [Display(ResourceType = typeof(RCircuitosTesoreria), Name = "Cuentaabono2")]
        public String Cuentaabono2 { get; set; }

        [Display(ResourceType = typeof(RCircuitosTesoreria), Name = "Cuentaabonorel")]
        public String Cuentaabonorel { get; set; }

        public TipoImporte Importecuentacargo1 { get; set; }

        public TipoImporte Importecuentacargo2 { get; set; }

        public TipoImporte Importecuentaabono1 { get; set; }

        public TipoImporte Importecuentaabono2 { get; set; }

        public TipoImporte Importecuentacargorel { get; set; }

        public TipoImporte Importecuentaabonorel { get; set; }

        public String Desccuentacargo1 { get; set; }

        public String Desccuentacargo2 { get; set; }

        public String Desccuentacargorel { get; set; }

        public String Desccuentaabono1 { get; set; }

        public String Desccuentaabono2 { get; set; }

        public String Desccuentaabonorel { get; set; }

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

        public override string DisplayName => RCircuitosTesoreria.TituloEntidad1;

        #endregion
    }

}
