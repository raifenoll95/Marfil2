using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RInformeMargen = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.InformeMargen;

namespace Marfil.Dom.Persistencia.Model.Documentos
{
    public class InformeMargenModel : BaseModel<InformeMargenModel, InformeMargen>
    {

        #region CTR

        public InformeMargenModel()
        {

        }

        public InformeMargenModel(IContextService context) : base(context)
        {

        }

        #endregion

        #region properties

        [Required]
        public string Empresa { get; set; }
        [Required]
        public int Id { get; set; }
        [Display(ResourceType = typeof(RInformeMargen), Name = "FkArticulosCod")]
        public string FkArticulosCod { get; set; }
        [Display(ResourceType = typeof(RInformeMargen), Name = "FkArticulosNom")]
        public string FkArticulosNom { get; set; }
        [Display(ResourceType = typeof(RInformeMargen), Name = "Lote")]
        public string Lote { get; set; }
        [Display(ResourceType = typeof(RInformeMargen), Name = "TablaId")]
        public int TablaId { get; set; }
        [Display(ResourceType = typeof(RInformeMargen), Name = "Cantidad")]
        public double Cantidad { get; set; }
        [Display(ResourceType = typeof(RInformeMargen), Name = "ReferenciaAlbVenta")]
        public string ReferenciaAlbVenta { get; set; }
        [Display(ResourceType = typeof(RInformeMargen), Name = "FkClientesCod")]
        public string FkClientesCod { get; set; }
        [Display(ResourceType = typeof(RInformeMargen), Name = "FkClientesNom")]
        public string FkClientesNom { get; set; }
        [Display(ResourceType = typeof(RInformeMargen), Name = "MetrosVenta")]
        public double MetrosVenta { get; set; }
        [Display(ResourceType = typeof(RInformeMargen), Name = "PrecioTotalVenta")]
        public double PrecioTotalVenta { get; set; }
        [Display(ResourceType = typeof(RInformeMargen), Name = "PrecioVtaMetro")]
        public double PrecioVtaMetro { get; set; }
        [Display(ResourceType = typeof(RInformeMargen), Name = "ReferenciaAlbCompra")]
        public string ReferenciaAlbCompra { get; set; }
        [Display(ResourceType = typeof(RInformeMargen), Name = "FkProveedorCod")]
        public string FkProveedorCod { get; set; }
        [Display(ResourceType = typeof(RInformeMargen), Name = "FkProveedorNom")]
        public string FkProveedorNom { get; set; }
        [Display(ResourceType = typeof(RInformeMargen), Name = "MetrosCompra")]
        public double MetrosCompra { get; set; }
        [Display(ResourceType = typeof(RInformeMargen), Name = "PrecioTotalCompra")]
        public double PrecioTotalCompra { get; set; }
        [Display(ResourceType = typeof(RInformeMargen), Name = "PrecioCompraMetro")]
        public double PrecioCompraMetro { get; set; }
        [Display(ResourceType = typeof(RInformeMargen), Name = "DiferenciaMetros")]
        public double DiferenciaMetros { get; set; }
        [Display(ResourceType = typeof(RInformeMargen), Name = "Margen")]
        public double Margen { get; set; }
        [Display(ResourceType = typeof(RInformeMargen), Name = "MargenPorcentaje")]
        public int MargenPorcentaje { get; set; }

        #endregion

        public override string DisplayName => throw new NotImplementedException();

        public override object generateId(string id)
        {
            return id;
        }
    }
}
