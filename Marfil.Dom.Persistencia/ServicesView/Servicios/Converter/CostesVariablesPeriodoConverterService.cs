using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.CRM;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos.Helper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class CostesVariablesPeriodoConverterService : BaseConverterModel<CostesVariablesPeriodoModel, CostesVariablesPeriodo>
    {

        #region constructor
        public CostesVariablesPeriodoConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override IModelView CreateView(string id)
        {
            var identificador = Funciones.Qint(id);
            var obj = _db.Set<CostesVariablesPeriodo>().Where(f => f.empresa == Empresa && f.fkejercicio == identificador).Single();
            var result = GetModelView(obj) as CostesVariablesPeriodoModel;
            return result;
        }
        
        public override IModelView GetModelView(CostesVariablesPeriodo obj)
        {
            var result = base.GetModelView(obj) as CostesVariablesPeriodoModel;

            var ejercicioService = FService.Instance.GetService(typeof(EjerciciosModel), Context, _db);
            var ejerciciomodel = ejercicioService.get(obj.fkejercicio.ToString()) as EjerciciosModel;
            result.Descripcion_ejercicio = ejerciciomodel?.Descripcion;

            result._costes = obj.CostesVariablesPeriodoLin.ToList().Select(f => new CostesVariablesPeriodoLinModel()
            {
                Id = f.id,
                Tablavaria = f.tablavaria,
                Descripcion = f.descripcion,
                Precio = (float)f.precio
            }).ToList();

            return result;
        }

        public override CostesVariablesPeriodo CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as CostesVariablesPeriodoModel;
            var result = _db.CostesVariablesPeriodo.Create();

            foreach (var item in result.GetType().GetProperties())
            {
                if (typeof(CostesVariablesPeriodoModel).GetProperties().Any(f => f.Name.ToLower() == item.Name.ToLower()) && item.Name.ToLower() != "Tipofamilia")
                    item.SetValue(result, viewmodel.get(item.Name));
            }

            result.CostesVariablesPeriodoLin.Clear();
            foreach (var item in viewmodel._costes)
            {
                var newitem = _db.CostesVariablesPeriodoLin.Create();
                newitem.empresa = result.empresa;
                newitem.fkejercicio = result.fkejercicio;
                newitem.id = item.Id;
                newitem.tablavaria = item.Tablavaria;
                newitem.descripcion = item.Descripcion;
                newitem.precio = item.Precio;
                result.CostesVariablesPeriodoLin.Add(newitem);
            }
            return result;
        }

        public override CostesVariablesPeriodo EditPersitance(IModelView obj)
        {
            var viewmodel = obj as CostesVariablesPeriodoModel;
            var result = _db.CostesVariablesPeriodo.Single(f => f.fkejercicio == viewmodel.Fkejercicio && f.empresa == viewmodel.Empresa);

            foreach (var item in result.GetType().GetProperties())
            {
                if (typeof(CostesVariablesPeriodoModel).GetProperties().Any(f => f.Name.ToLower() == item.Name.ToLower()) && item.Name.ToLower() != "Tipofamilia")
                    item.SetValue(result, viewmodel.get(item.Name));
            }

            result.CostesVariablesPeriodoLin.Clear();
            foreach (var item in viewmodel._costes)
            {
                var newitem = _db.CostesVariablesPeriodoLin.Create();
                newitem.empresa = result.empresa;
                newitem.fkejercicio = result.fkejercicio;
                newitem.id = item.Id;
                newitem.tablavaria = item.Tablavaria;
                newitem.descripcion = item.Descripcion;
                newitem.precio = item.Precio;

                result.CostesVariablesPeriodoLin.Add(newitem);
            }

            return result;
        }

        #endregion
    }
}
