using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Interfaces;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class GuiascontablesConverterService : BaseConverterModel<GuiascontablesModel, Guiascontables>
    {
        

        public GuiascontablesConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
            
        }


        public override IEnumerable<IModelView> GetAll()
        {
            var list = _db.Guiascontables.Where(f => f.empresa == Empresa).ToList();

            var result = new List<GuiascontablesModel>();
            foreach (var item in list)
            {
                result.Add(GetModelView(item) as GuiascontablesModel);
            }

            return result;
        }

        public override bool Exists(string id)
        {
            return _db.Set<Guiascontables>().Any(f => f.id == id && f.empresa == Empresa);
        }

        public override IModelView CreateView(string id)
        {
            var obj = _db.Set<Guiascontables>().Single(f => f.id == id && f.empresa == Empresa);
            var result = GetModelView(obj) as GuiascontablesModel;
           
            return result;
        }

        public override Guiascontables CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as GuiascontablesModel;
            var result = _db.Set<Guiascontables>().Create();
            result.empresa = viewmodel.Empresa;
            result.id = viewmodel.Id;
            result.descripcion = viewmodel.Descripcion;
            result.fkcuentascompras = viewmodel.Fkcuentascompras;
            result.fkcuentasventas = viewmodel.Fkcuentasventas;
            result.fkcuentasdevolucioncompras = viewmodel.Fkcuentasdevolucioncompras;
            result.fkcuentasdevolucionventas = viewmodel.Fkcuentasdevolucionventas;
            result.defecto = viewmodel.Defecto;

            result.GuiascontablesLin.Clear();
            foreach (var item in viewmodel.Lineas)
            {
                var newItem = _db.Set<GuiascontablesLin>().Create();
                newItem.empresa = item.Empresa;
                newItem.fkguiascontables = viewmodel.Id;
                newItem.id = item.Id;
                newItem.fkregimeniva = item.Fkregimeniva;
                newItem.fkcuentasventas = item.Fkcuentasventas;
                newItem.fkcuentascompras = item.Fkcuentascompras;
                newItem.fkcuentasdevolucioncompras = item.Fkcuentasdevolucioncompras;
                newItem.fkcuentasdevolucionventas = item.Fkcuentasdevolucionventas;
                result.GuiascontablesLin.Add(newItem);
            }
            return result;
        }

        public override Guiascontables EditPersitance(IModelView obj)
        {
            var viewmodel = obj as GuiascontablesModel;
            var result = _db.Guiascontables.Single(f => f.id == viewmodel.Id && f.empresa == viewmodel.Empresa);
            result.empresa = viewmodel.Empresa;
            result.id = viewmodel.Id;
            result.descripcion = viewmodel.Descripcion;
            result.fkcuentascompras = viewmodel.Fkcuentascompras;
            result.fkcuentasventas = viewmodel.Fkcuentasventas;
            result.fkcuentasdevolucioncompras = viewmodel.Fkcuentasdevolucioncompras;
            result.fkcuentasdevolucionventas = viewmodel.Fkcuentasdevolucionventas;
            result.defecto = viewmodel.Defecto;

            result.GuiascontablesLin.Clear();
            foreach (var item in viewmodel.Lineas)
            {
                var newItem = _db.Set<GuiascontablesLin>().Create();
                newItem.empresa = item.Empresa;
                newItem.fkguiascontables = viewmodel.Id;
                newItem.id = item.Id;
                newItem.fkregimeniva = item.Fkregimeniva;
                newItem.fkcuentasventas = item.Fkcuentasventas;
                newItem.fkcuentascompras = item.Fkcuentascompras;
                newItem.fkcuentasdevolucioncompras = item.Fkcuentasdevolucioncompras;
                newItem.fkcuentasdevolucionventas = item.Fkcuentasdevolucionventas;
                result.GuiascontablesLin.Add(newItem);
            }

            return result;
        }

        public override IModelView GetModelView(Guiascontables obj)
        {

            var result= base.GetModelView(obj) as GuiascontablesModel;
            result.Lineas = obj.GuiascontablesLin.Select(f => new GuiascontablesLinModel() { Empresa = f.empresa, Fkguiascontables = f.fkguiascontables, Id = f.id, Fkregimeniva = f.fkregimeniva, Fkcuentasventas = f.fkcuentasventas, Fkcuentascompras = f.fkcuentascompras, Fkcuentasdevolucionventas = f.fkcuentasdevolucionventas, Fkcuentasdevolucioncompras = f.fkcuentasdevolucioncompras }).ToList();
            //var result = new GuiascontablesModel
            //{
            //    Empresa = obj.empresa,
            //    Id = obj.id,
            //    Descripcion = obj.descripcion,
            //    Fkcuentasventas = obj.fkcuentasventas,
            //    Fkcuentascompras = obj.fkcuentascompras,
            //    Fkcuentasdevolucionventas = obj.fkcuentasdevolucionventas,
            //    Fkcuentasdevolucioncompras = obj.fkcuentasdevolucioncompras,
            //    Lineas= obj.GuiascontablesLin.Select(f=> new GuiascontablesLinModel() {Empresa = f.empresa, Fkguiascontables = f.fkguiascontables, Id = f.id,Fkregimeniva =f.fkregimeniva ,Fkcuentasventas = f.fkcuentasventas, Fkcuentascompras = f.fkcuentascompras,Fkcuentasdevolucionventas = f.fkcuentasdevolucionventas,Fkcuentasdevolucioncompras = f.fkcuentasdevolucioncompras}).ToList()
            //};



            return result;
        }
    }
}
