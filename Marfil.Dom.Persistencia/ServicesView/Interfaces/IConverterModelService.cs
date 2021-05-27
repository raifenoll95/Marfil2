using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Interfaces;

namespace Marfil.Dom.Persistencia.ServicesView.Interfaces
{
    internal interface IConverterModelService<TView,TPersistance>
    {
        
        string Empresa { get; set; }

        IEnumerable<IModelView> GetAll();
        IModelView CreateView(string id);
        TPersistance CreatePersitance(IModelView obj);
        TPersistance EditPersitance(IModelView obj);
        IModelView GetModelView(TPersistance obj);
        bool Exists(string id);
        void AsignaId(TPersistance objPer, ref IModelView objView);
    }
}
