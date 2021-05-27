using System;
using System.Linq;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.Dom.Persistencia.Model.Configuracion
{
    public class TarifasbaseModel : BaseModel<TarifasbaseModel, Tarifasbase>
    {
        #region Properties

        public TipoFlujo Tipoflujo { get; set; }

        public string Fktarifa { get; set; }

        public string Descripcion { get; set; }

        #endregion

        #region CTR

        public TarifasbaseModel()
        {

        }

        public TarifasbaseModel(IContextService context) : base(context)
        {

        }

        #endregion

        public override object generateId(string id)
        {
            return id;
        }


        public override void createNewPrimaryKey()
        {
            primaryKey = getProperties().Where(f => f.property.Name.ToLower() == "Fktarifa").Select(f => f.property);
        }

        public override string DisplayName => "Tarifas base";
    }
}
