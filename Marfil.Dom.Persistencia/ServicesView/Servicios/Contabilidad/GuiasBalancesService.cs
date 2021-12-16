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
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var propiedadesVisibles = new[] { "InformeId", "GuiaId", "TextoGrupo", "Orden", "Actpas", "Detfor", "Formula", "RegDig", "Descrip", "Listado" };
            var propiedades = Helpers.Helper.getProperties<CuadernosBancariosModel>();

            //model.PrimaryColumnns = new[] { "id" };
            model.ExcludedColumns = propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            model.OrdenColumnas.Add("InformeId", 0);
            model.OrdenColumnas.Add("GuiaId", 1);
            model.OrdenColumnas.Add("TextoGrupo", 2);
            model.OrdenColumnas.Add("Orden", 3);
            model.OrdenColumnas.Add("Actpas", 4);
            model.OrdenColumnas.Add("Detfor", 5);
            model.OrdenColumnas.Add("Formula", 6);
            model.OrdenColumnas.Add("RegDig", 7);
            model.OrdenColumnas.Add("Descrip", 8);
            model.OrdenColumnas.Add("Listado", 9);

            return model;
        }
        public override string GetSelectPrincipal()
        {
            return string.Format("select * from GuiasBalances");
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

        public string GuiaInformeConf()
        {
            var Guia = _db.Empresas.Where(f => f.id == Empresa).FirstOrDefault().guiaperdidas;

            return Guia;
        }

        public void DeleteAllLin()
        {
            var lin = _db.GuiasBalancesLineas.Where(f => f.guiasBalancesId == null).ToList();

            if (lin != null)
            {
                _db.GuiasBalancesLineas.RemoveRange(lin);
                _db.SaveChanges();

            }

        }
    }
}
