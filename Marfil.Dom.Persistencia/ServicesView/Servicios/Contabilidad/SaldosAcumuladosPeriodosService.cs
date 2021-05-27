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
                    itemmaes.saldo = (itemmaes.debe ?? 0) - (itemmaes.haber ?? 0);

                    _db.Maes.AddOrUpdate(itemmaes);
                }
            }
            //if (model.Ejercicio)//R2
            //{
            //    var r2 = movs.Where(w => w.tipoasiento.ToUpper() == "R2");
            //}

            //if (model.Existencia) //R3
            //{
            //    var r3 = movs.Where(w => w.tipoasiento.ToUpper() == "R3");
            //}
            //if (model.Grupos) //R4
            //{
            //    var r4 = movs.Where(w => w.tipoasiento.ToUpper() == "R4");
            //}

            //if (model.CierreEjercicio)//R5
            //{
            //    var r5 = movs.Where(w => w.tipoasiento.ToUpper() == "R5");
            //}

            //if (model.IncluirAsientosSimulacion)//F2
            //{
            //    var f2 = movs.Where(w => w.tipoasiento.ToUpper() == "F2");
            //}

            //if (model.ExcluirAsientosSimulacion)//F3
            //{
            //    var f3 = movs.Where(w => w.tipoasiento.ToUpper() == "F3");
            //}

        }
    }
}
