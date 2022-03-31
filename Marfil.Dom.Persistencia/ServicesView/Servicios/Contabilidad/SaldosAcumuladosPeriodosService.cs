using Marfil.Dom.Persistencia.Listados;
using Marfil.Dom.Persistencia.Model.Contabilidad;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Contabilidad
{
    public class SaldosAcumuladosPeriodosService : GestionService<SaldosAcumuladorPeriodosModel, SaldosAcomuladosPeriodos>
    {
        public SaldosAcumuladosPeriodosService(IContextService context, MarfilEntities db = null) : base(context, db)
        {
        }

        public void GenerarMovimiento(ListadoAcumuladorPeriodos model, TipoOperacionMaes tipo)// short multiplo)
        {
            var movs = _db.Movs.Include(i => i.MovsLin)
                .Where(w => w.empresa == model.Empresa && w.fechaalta <= Convert.ToDateTime(model.SeccionDesde) && w.fechaalta >= Convert.ToDateTime(model.SeccionHasta) && w.fkejercicio == Convert.ToInt32(model.fkEjercicio) && w.empresa == model.Empresa);

            foreach (var m in movs.ToList())
            {
                foreach (var lineas in m.MovsLin.GroupBy(g => g.fkcuentas))
                {
                    string keyGroup = lineas.Key;
                    var itemmaes = _db.Maes.SingleOrDefault(f => f.empresa == model.Empresa && f.fkcuentas == keyGroup && f.fkejercicio == Convert.ToInt32(model.fkEjercicio))
                              ?? _db.Maes.Create();

                    if (string.IsNullOrWhiteSpace(itemmaes.empresa))
                    {
                        itemmaes.empresa = model.Empresa;
                        itemmaes.fkcuentas = keyGroup;
                        itemmaes.fkejercicio = Convert.ToInt32(model.fkEjercicio);
                    }
                    int multiplo = (tipo == TipoOperacionMaes.Alta ? 1 : -1);

                    itemmaes.debe = (itemmaes.debe ?? 0) + (lineas.Where(l => l.esdebe == 1).Sum(l => l.importe) * (multiplo));
                    itemmaes.haber = (itemmaes.haber ?? 0) + (lineas.Where(l => l.esdebe == -1).Sum(l => l.importe) * (multiplo));

                    _db.Maes.AddOrUpdate(itemmaes);
                }
            }
        }

        public DateTime? GetRecalculoAnt(string usuario, int ejercicio)
        {
            var ejercicioant = _db.Ejercicios.Where(f => f.empresa == Empresa && f.id == ejercicio).FirstOrDefault().fkejercicios;
            var p = _db.AcumuladorPeriodos.Where(f=> f.empresa == Empresa && f.usuario == usuario && f.fkejercicio == ejercicioant).Count();
            if ( p > 0)
            {
                return _db.Set<AcumuladorPeriodos>().Where(f => f.empresa == Empresa && f.usuario == usuario && f.fkejercicio == ejercicioant).OrderByDescending(t => t.FechaHora).FirstOrDefault().FechaHora;
            }
            else
            {
                return null;
            }
            
        }

        public DateTime? GetRecalculo(string usuario, int ejercicio)
        {
            var p = _db.AcumuladorPeriodos.Where(f => f.empresa == Empresa && f.usuario == usuario && f.fkejercicio == ejercicio).Count();
            if (p > 0)
            {
                return _db.Set<AcumuladorPeriodos>().Where(f => f.empresa == Empresa && f.usuario == usuario && f.fkejercicio == ejercicio).OrderByDescending(t => t.FechaHora).FirstOrDefault().FechaHora;
            }
            else
            {
                return null;
            }

        }

        public FiltrosAcumulador GetFiltros(string usuario, int ejercicio)
        {
            return _db.FiltrosAcumulador.Where(f => f.empresa == Empresa && f.fkejercicio == ejercicio && f.usuario == usuario).FirstOrDefault();
        }

        public FiltrosAcumulador GetFiltrosAnt(string usuario, int ejercicio)
        {
            var ejercicioant = _db.Ejercicios.Where(f => f.empresa == Empresa && f.id == ejercicio).FirstOrDefault().fkejercicios;
            return _db.FiltrosAcumulador.Where(f => f.empresa == Empresa && f.fkejercicio == ejercicioant && f.usuario == usuario).FirstOrDefault();
        }
    }
}
