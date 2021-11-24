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
    public interface IGuiasBalances { }
    public class GuiasBalancesService : GestionService<GuiasBalancesModel,GuiasBalances> , IGuiasBalances
    {
        public GuiasBalancesService(IContextService context, MarfilEntities db = null) : base(context,db)
        {

        }

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            //var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var fmodel = new FModel();
            var obj = fmodel.GetModel<GuiasBalancesModel>(_context);
            var instance = obj as IModelView;
            var extension = obj as IModelViewExtension;
            var display = obj as ICanDisplayName;
            var model = new ListIndexModel()
            {
                Entidad = display.DisplayName,
                List = GetAllGuiasBalances<GuiasBalancesModel>(),
                PrimaryColumnns = extension.primaryKey.Select(f => f.Name).ToList(),
                VarSessionName = "__" + t.Name,
                Properties = instance.getProperties(),
                Controller = controller,
                PermiteEliminar = canEliminar,
                PermiteModificar = canModificar,
                ExcludedColumns = new[] { "Toolbar" }
            };
            var propiedadesVisibles = new[] { "TipoInformeE", "TipoGuiaE", "TextoGrupo", "Orden", "Actpas", "Detfor", "Formula", "RegDig", "Descrip", "Listado" };
            var propiedades = Helpers.Helper.getProperties<GuiasBalancesModel>();
            model.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            model.FiltroColumnas.Add("Id", FiltroColumnas.EmpiezaPor);
            //model.ColumnasCombo.Add("Fkgruposmateriales", ListGruposmateriales());
            return model;
        }
        public IEnumerable<T> GetAllGuiasBalances<T>() where T : GuiasBalancesModel
        {
            var a = _db.Database.SqlQuery<T>("select *, InformeId as TipoInformeE, GuiaId as TipoGuiaE from guiasbalances").ToList();
            return a;
        }

        public string GetFiltroAcumulador()
        {
            var ejercicioParse = int.Parse(_context.Ejercicio);
            var registro = _db.FiltrosAcumulador.Where(f => f.empresa == Empresa && f.fkejercicio == ejercicioParse && f.usuario == _context.Usuario).FirstOrDefault();
            if (registro != null)
            {
                return _context.Ejercicio + "-" + _context.Usuario;
            } else
            {
                return "";
            }
        }

        public bool HayCuentasNoAsignadas()
        {
            var cuentas = _db.CuentasNoAsignadas.Where(f => f.procesado == false).Count();

            if (cuentas > 0)
            {
                return true;
            }

            return false;
        }
    }
}
