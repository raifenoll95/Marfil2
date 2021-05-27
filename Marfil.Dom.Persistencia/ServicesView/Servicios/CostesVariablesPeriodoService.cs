
using DevExpress.Data.Utils;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.CRM;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using System;
using System.Linq;
using Marfil.Inf.Genericos.Helper;
using Marfil.Dom.Persistencia.Helpers;
using System.Text;
using Marfil.Dom.Persistencia.Model.Configuracion;
using System.Collections.Generic;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Validation;

public interface ICostesVariablesPeriodoService
{
}

public class CostesVariablesPeriodoService : GestionService<CostesVariablesPeriodoModel, CostesVariablesPeriodo>
{
    public CostesVariablesPeriodoService(IContextService context, MarfilEntities db = null) : base(context, db)
    {
    }

    public override string GetSelectPrincipal()
    {
        var result = new StringBuilder();

        result.Append("select c.fkejercicio, e.descripcion as [Descripcion_ejercicio]");
        result.Append(" from CostesVariablesPeriodo as c");
        result.Append(" left join Ejercicios as e ON c.fkejercicio=e.id");
        result.AppendFormat(" where c.empresa ='{0}' ", _context.Empresa);

        return result.ToString();
    }

    public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
    {
        /*
        var st = base.GetListIndexModel(t, canEliminar, canModificar, controller);
        st.List = st.List.OfType<CostesVariablesPeriodoModel>().OrderByDescending(f => f.Fkejercicio);
        var propiedadesVisibles = new[] { "Fkejercicio", "Descripcion_ejercicio" };
        //var propiedadesVisibles = new[] { "Fkejercicios", };
        var propiedades = Helper.getProperties<CostesVariablesPeriodoModel>();
        st.PrimaryColumnns = new[] { "Id" };
        st.ExcludedColumns =
            propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
        return st;
        */

        var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
        var propiedadesVisibles = new[] { "Fkejercicio", "Descripcion_ejercicio" };
        var propiedades = Helper.getProperties<CostesVariablesPeriodoModel>();
        model.ExcludedColumns =
            propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
        model.PrimaryColumnns = new[] { "Fkejercicio" };

        return model;
    }


    public IEnumerable<EjerciciosModel> getListEjercicios()
    {
        return _db.Database.SqlQuery<EjerciciosModel>(string.Format("select id, descripcion from Ejercicios where empresa = '" + Empresa + "'"));
    }


    //Crea un coste variable periodo
    public override void create(IModelView obj)
    {
        using (var tran = TransactionScopeBuilder.CreateTransactionObject())
        {
            var model = obj as CostesVariablesPeriodoModel;
            model.Descripcion_ejercicio = _db.Ejercicios.Where(f => f.id == model.Fkejercicio && f.empresa == model.Empresa).Select(f => f.descripcion).ToString();

            int cont = 1;
            foreach (var linea in model._costes)
            {
                linea.Fkejercicio = model.Fkejercicio;
                cont++;
            }

            base.create(obj);

            _db.SaveChanges();
            tran.Complete();
        }
    }
}



