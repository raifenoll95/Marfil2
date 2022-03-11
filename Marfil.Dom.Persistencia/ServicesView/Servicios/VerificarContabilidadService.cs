using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public class VerificarContabilidadService : GestionService<VerificarContabilidadModel, VerificarContabilidad>
    {

        #region CTR

        public VerificarContabilidadService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        #region ListIndexModel

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            CargarTabla();

            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var propiedadesVisibles = new[] { "Nivel", "SDebe", "SHaber", "SSaldo" };
            var propiedades = Helpers.Helper.getProperties<VerificarContabilidadModel>();

            //model.PrimaryColumnns = new[] { "id" };
            model.ExcludedColumns = propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            model.OrdenColumnas.Add("Nivel", 0);
            model.OrdenColumnas.Add("SDebe", 1);
            model.OrdenColumnas.Add("SHaber", 2);
            model.OrdenColumnas.Add("SSaldo", 3);

            return model;
        }

        public override string GetSelectPrincipal()
        {
            return string.Format("select * from VerificarContabilidad");
        }

        #endregion

        public void CargarTabla()
        {
            try
            {


                //Vaciar tabla
                foreach (var iddelete in _db.VerificarContabilidad.Select(e => e.id))
                {
                    var entity = new VerificarContabilidad { id = iddelete };
                    _db.VerificarContabilidad.Attach(entity);
                    _db.VerificarContabilidad.Remove(entity);
                }

                _db.SaveChanges();

                //Nuevos datos
                List<VerificarContabilidadModel> lista = new List<VerificarContabilidadModel>();
                var ejercicio = int.Parse(_context.Ejercicio);
                var debelin = 0d;
                var haberlin = 0d;
                var saldolin = 0d;

                if (_db.Maes.Where(f => f.empresa == Empresa && f.fkejercicio == ejercicio).FirstOrDefault() != null)
                {
                    var grupo = new VerificarContabilidadModel()
                    {
                        Id = 1,
                        Nivel = "Grupos X_______",
                        Debe = _db.Maes.Where(f => f.empresa == Empresa && f.fkejercicio == ejercicio && f.fkcuentas.Length == 1).Sum(x => x.debe),
                        Haber = _db.Maes.Where(f => f.empresa == Empresa && f.fkejercicio == ejercicio && f.fkcuentas.Length == 1).Sum(x => x.haber),
                        Saldo = _db.Maes.Where(f => f.empresa == Empresa && f.fkejercicio == ejercicio && f.fkcuentas.Length == 1).Sum(x => x.saldo)
                    };

                    lista.Add(grupo);

                    var subgrupo = new VerificarContabilidadModel()
                    {
                        Id = 2,
                        Nivel = "SubGrupos XX______",
                        Debe = _db.Maes.Where(f => f.empresa == Empresa && f.fkejercicio == ejercicio && f.fkcuentas.Length == 2).Sum(x => x.debe),
                        Haber = _db.Maes.Where(f => f.empresa == Empresa && f.fkejercicio == ejercicio && f.fkcuentas.Length == 2).Sum(x => x.haber),
                        Saldo = _db.Maes.Where(f => f.empresa == Empresa && f.fkejercicio == ejercicio && f.fkcuentas.Length == 2).Sum(x => x.saldo)
                    };

                    lista.Add(subgrupo);

                    var mayor = new VerificarContabilidadModel()
                    {
                        Id = 3,
                        Nivel = "Mayor XXX_____",
                        Debe = _db.Maes.Where(f => f.empresa == Empresa && f.fkejercicio == ejercicio && f.fkcuentas.Length == 3).Sum(x => x.debe),
                        Haber = _db.Maes.Where(f => f.empresa == Empresa && f.fkejercicio == ejercicio && f.fkcuentas.Length == 3).Sum(x => x.haber),
                        Saldo = _db.Maes.Where(f => f.empresa == Empresa && f.fkejercicio == ejercicio && f.fkcuentas.Length == 3).Sum(x => x.saldo)
                    };

                    lista.Add(mayor);

                    var submayor = new VerificarContabilidadModel()
                    {
                        Id = 4,
                        Nivel = "SubMayor XXXX____",
                        Debe = _db.Maes.Where(f => f.empresa == Empresa && f.fkejercicio == ejercicio && f.fkcuentas.Length == 4).Sum(x => x.debe),
                        Haber = _db.Maes.Where(f => f.empresa == Empresa && f.fkejercicio == ejercicio && f.fkcuentas.Length == 4).Sum(x => x.haber),
                        Saldo = _db.Maes.Where(f => f.empresa == Empresa && f.fkejercicio == ejercicio && f.fkcuentas.Length == 4).Sum(x => x.saldo)
                    };

                    lista.Add(submayor);

                    var subcuenta = new VerificarContabilidadModel()
                    {
                        Id = 5,
                        Nivel = "SubCuenta XXXXXXXX",
                        Debe = _db.Maes.Where(f => f.empresa == Empresa && f.fkejercicio == ejercicio && f.fkcuentas.Length > 4).Sum(x => x.debe),
                        Haber = _db.Maes.Where(f => f.empresa == Empresa && f.fkejercicio == ejercicio && f.fkcuentas.Length > 4).Sum(x => x.haber),
                        Saldo = _db.Maes.Where(f => f.empresa == Empresa && f.fkejercicio == ejercicio && f.fkcuentas.Length > 4).Sum(x => x.saldo)
                    };

                    lista.Add(subcuenta);
                }
                else
                {
                    var grupo = new VerificarContabilidadModel()
                    {
                        Id = 1,
                        Nivel = "Grupos X_______",
                        Debe = 0,
                        Haber = 0,
                        Saldo = 0
                    };

                    lista.Add(grupo);

                }

                if (_db.Movs.Where(f => f.empresa == Empresa && f.fkejercicio == ejercicio).FirstOrDefault() != null)
                {
                    var listascab = _db.Movs.Where(f => f.empresa == Empresa && f.fkejercicio == ejercicio).Select(e => e.id).ToList();

                    var movs = new VerificarContabilidadModel()
                    {
                        Id = 6,
                        Nivel = "Fichero de movimientos",
                        Debe = _db.Movs.Where(f => f.empresa == Empresa && f.fkejercicio == ejercicio).Sum(x => x.debe),
                        Haber = _db.Movs.Where(f => f.empresa == Empresa && f.fkejercicio == ejercicio).Sum(x => x.haber),
                        Saldo = _db.Movs.Where(f => f.empresa == Empresa && f.fkejercicio == ejercicio).Sum(x => x.saldo)
                    };

                    lista.Add(movs);

                    foreach (var idmov in listascab)
                    {
                        debelin += (double)_db.MovsLin.Where(f => f.empresa == Empresa && f.fkmovs == idmov && f.esdebe == 1).Sum(x => x.importe);
                        haberlin += (double)_db.MovsLin.Where(f => f.empresa == Empresa && f.fkmovs == idmov && f.esdebe == -1).Sum(x => x.importe);
                    }

                    saldolin = debelin - haberlin;

                    var movslin = new VerificarContabilidadModel()
                    {
                        Id = 7,
                        Nivel = "Fichero movs. lineas",
                        Debe = (decimal)debelin,
                        Haber = (decimal)haberlin,
                        Saldo = (decimal)saldolin
                    };

                    lista.Add(movslin);
                }
                else
                {
                    var movs = new VerificarContabilidadModel()
                    {
                        Id = 6,
                        Nivel = "Fichero de movimientos",
                        Debe = 0,
                        Haber = 0,
                        Saldo = 0
                    };

                    lista.Add(movs);

                }

                foreach (var fila in lista)
                {
                    var item = _db.VerificarContabilidad.Create();
                    item.Nivel = fila.Nivel;
                    item.Debe = fila.Debe;
                    item.Haber = fila.Haber;
                    item.Saldo = fila.Saldo;

                    _db.VerificarContabilidad.AddOrUpdate(item);
                }

                _db.SaveChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
