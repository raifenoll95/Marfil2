using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;
using RDivisionLotes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.DivisionLotes;
using RAlbaranesCompras = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.AlbaranesCompras;
using Marfil.Inf.Genericos;

namespace Marfil.Dom.Persistencia.Model.Documentos.CobrosYPagos
{

    //public class VencimientosCarteraModel : BaseModel<VencimientosCarteraModel, Persistencia.Vencimientoscartera>, IDocument
    //{

    //    #region CTR
    //    public VencimientosCarteraModel()
    //    {
    //    }

    //    public VencimientosCarteraModel(IContextService context) : base(context)
    //    {
    //    }
    //    #endregion

    //    #region properties

    //    public string Empresa { get; set; }

    //    [Display(ResourceType = typeof(RCobrosYPagos), Name = "Fkvencimientos")]
    //    public int Fkvencimientos { get; set; }

    //    [Display(ResourceType = typeof(RCobrosYPagos), Name = "Fkcartera")]
    //    public int Fkcartera { get; set; }

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
