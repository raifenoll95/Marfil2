//using Marfil.Dom.Persistencia.Model.FicherosGenerales;
//using System.Collections.Generic;
//using System.Linq;
//using Marfil.Dom.Persistencia.Model.Interfaces;
//using Marfil.Dom.Persistencia.Model.Documentos.CobrosYPagos;
//using System;

//namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
//{
//    internal class PrevisionesCarteraConverterService : BaseConverterModel<PrevisionesCarteraModel, Persistencia.PrevisionesCartera>
//    {
//        public PrevisionesCarteraConverterService(IContextService context, MarfilEntities db) : base(context, db)
//        {
//        }

//        public override IEnumerable<IModelView> GetAll()
//        {
//            return _db.Set<Persistencia.PrevisionesCartera>().Select(GetModelView);
//        }

//        public override IModelView CreateView(string id)
//        {
//            var idnum = Int32.Parse(id);
//            var obj = _db.Set<Persistencia.PrevisionesCartera>().Single(f => f.empresa == Empresa/* && f.id == idnum*/);
//            var result = GetModelView(obj) as PrevisionesCarteraModel;
//            return result;
//        }

//        public override Persistencia.PrevisionesCartera CreatePersitance(IModelView obj)
//        {
//            var viewmodel = obj as PrevisionesCarteraModel;
//            var result = _db.Set<Persistencia.PrevisionesCartera>().Create();
//            result.empresa = viewmodel.Empresa;
//            result.idvencimiento = viewmodel.Idvencimiento.Value;     
//            result.idcartera = viewmodel.Idcartera.Value;
//            return result;
//        }

//        public override Persistencia.PrevisionesCartera EditPersitance(IModelView obj)
//        {
//            var viewmodel = obj as PrevisionesCarteraModel;
//            var result = _db.PrevisionesCartera.Where(f => f.idvencimiento == viewmodel.Idvencimiento && f.idcartera == viewmodel.Idcartera && f.empresa == Empresa).Single();
//            result.empresa = viewmodel.Empresa;
//            result.idvencimiento = viewmodel.Idvencimiento.Value;
//            result.idcartera = viewmodel.Idcartera.Value;
//            return result;
//        }

//        public override IModelView GetModelView(Persistencia.PrevisionesCartera viewmodel)
//        {
//            var result = new PrevisionesCarteraModel
//            {
//                Empresa = viewmodel.empresa,
//                Idvencimiento = viewmodel.idvencimiento,
//                Idcartera = viewmodel.idcartera,
//            };
           
//            return result;
//        }
//    }
//}
