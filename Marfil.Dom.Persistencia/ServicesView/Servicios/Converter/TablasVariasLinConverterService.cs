using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    class TablasVariasLinConverterService<TModelLin> : BaseConverterModel<TModelLin, TablasvariasLin> where TModelLin : class
    {
        #region Members
        [XmlIgnore]
        private readonly  Serializer<TModelLin> _serializer=new Serializer<TModelLin>();

        #endregion
  
        public TablasVariasLinConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {

        }

        public override IModelView GetModelView(TablasvariasLin obj)
        {
            return _serializer.SetXml(obj.xml) as IModelView;
        }
    }
}
