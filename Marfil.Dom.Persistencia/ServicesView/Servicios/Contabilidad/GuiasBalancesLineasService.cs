using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Contabilidad;
using Marfil.Dom.Persistencia.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Contabilidad
{
    public interface IGuiasBalancesLineas { }
    public class GuiasBalancesLineasService : GestionService<GuiasBalancesLineasModel,GuiasBalancesLineas>, IGuiasBalancesLineas
    {
        public GuiasBalancesLineasService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }
        public override IEnumerable<T> GetAll<T>()
        {
            return base.GetAll<T>();
        }
        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var fmodel = new FModel();
            var obj = fmodel.GetModel<GuiasBalancesLineasModel>(_context);
            var instance = obj as IModelView;
            var extension = obj as IModelViewExtension;
            var display = obj as ICanDisplayName;
            var model = new ListIndexModel()
            {
                Entidad = display.DisplayName,
                List = GetAllGuiasBalancesLineas<GuiasBalancesLineasModel>(),
                PrimaryColumnns = extension.primaryKey.Select(f => f.Name).ToList(),
                VarSessionName = "__" + t.Name,
                Properties = instance.getProperties(),
                Controller = controller,
                PermiteEliminar = canEliminar,
                PermiteModificar = canModificar,
                ExcludedColumns = new[] { "Toolbar" }
            };

            return model;
        }
        public IEnumerable<T> GetAllGuiasBalancesLineas<T>() where T : GuiasBalancesLineasModel
        {
            var a = _db.Database.SqlQuery<T>("select * from guiasbalanceslineas").ToList();
            return a;
        }
    }
}
