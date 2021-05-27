using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;
using RDivisionLotes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.DivisionLotes;
using RAlbaranesCompras = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.AlbaranesCompras;
using Marfil.Inf.Genericos;

namespace Marfil.Dom.Persistencia.Model.Documentos.CobrosYPagos
{

    //public class CarteraModel : BaseModel<CarteraModel, Persistencia.Cartera>, IDocument
    //{

    //    #region CTR
    //    public CarteraModel()
    //    {
    //    }

    //    public CarteraModel(IContextService context) : base(context)
    //    {
    //    }
    //    #endregion

    //    #region properties

    //    public int? Id { get; set; }

    //    public string Empresa { get; set; }

    //    //Factura a la que hace referecia
    //    //[Display(ResourceType = typeof(RCobrosYPagos), Name = "Fkfacturas")]
    //    public int Fkfacturas { get; set; }

    //    //[Display(ResourceType = typeof(RCobrosYPagos), Name = "Importe")]
    //    public double Importe { get; set; }

    //    //[Display(ResourceType = typeof(RCobrosYPagos), Name = "Fechavencimiento")]
    //    public DateTime Fechavencimiento { get; set; }

    //    #endregion

    //    #region atributos

    //    public override object generateId(string id)
    //    {
    //        return id;
    //    }

    //    public override void createNewPrimaryKey()
    //    {
    //        primaryKey = getProperties().Where(f => f.property.Name.ToLower() == "id").Select(f => f.property);
    //    }

    //    public override string GetPrimaryKey()
    //    {
    //        return Id.ToString();
    //    }

    //    public override string DisplayName => RDivisionLotes.TituloEntidad;

    //    #endregion
    //}
}
