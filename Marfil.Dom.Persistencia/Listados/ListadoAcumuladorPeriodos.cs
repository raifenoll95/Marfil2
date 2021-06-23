using Marfil.Dom.Persistencia.Listados.Base;
using Marfil.Dom.Persistencia.Model.Contabilidad.Movs;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RAPeriodos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.ListadoAcomuladoPeriodos;
using System.Data.Entity.Validation;

namespace Marfil.Dom.Persistencia.Listados
{
    public class ListadoAcumuladorPeriodos : ListadosModel
    {
        #region Propiedades defecto
        public override string TituloListado => "Acumulador de Periodos";
        public override string IdListado => FListadosModel.ListadoAcumuladorPeriodos;
        #endregion

        #region Propiedades

        [Display(ResourceType = typeof(RAPeriodos), Name = "FechaDesde")]
        public DateTime FechaDesde { get; set; }

        [Display(ResourceType = typeof(RAPeriodos), Name = "FechaHasta")]
        public DateTime FechaHasta { get; set; }

        [Display(ResourceType = typeof(RAPeriodos), Name = "SeccionDesde")]
        public string SeccionDesde { get; set; }

        [Display(ResourceType = typeof(RAPeriodos), Name = "SeccionHasta")]
        public string SeccionHasta { get; set; }

        [Display(ResourceType = typeof(RAPeriodos), Name = "GrupoSeccion")]
        public string GrupoSeccion { get; set; }

        [Display(ResourceType = typeof(RAPeriodos), Name = "Ejercicio")]
        public bool Ejercicio { get; set; }

        [Display(ResourceType = typeof(RAPeriodos), Name = "Existencia")]
        public bool Existencia { get; set; }

        [Display(ResourceType = typeof(RAPeriodos), Name = "Grupos")]
        public bool Grupos { get; set; }

        [Display(ResourceType = typeof(RAPeriodos), Name = "CierreEjercicio")]
        public bool CierreEjercicio { get; set; }

        [Display(ResourceType = typeof(RAPeriodos), Name = "IncluirAsientosSimulacion")]
        public bool IncluirAsientosSimulacion { get; set; }

        [Display(ResourceType = typeof(RAPeriodos), Name = "ExcluirAsientosSimulacion")]
        public bool ExcluirAsientosSimulacion { get; set; }

        [Display(ResourceType = typeof(RAPeriodos), Name = "IncluirAjusteExistenciaPeriodo")]
        public bool IncluirAjusteExistenciaPeriodo { get; set; }

        [Display(ResourceType = typeof(RAPeriodos), Name = "IncluirProrrateoAmortizaciones")]
        public bool IncluirProrrateoAmortizaciones { get; set; }
        public string Settings { get; set; }
        public string EnProceso { get; set; }
        [Display(ResourceType = typeof(RAPeriodos), Name = "FechaHora")]
        public DateTime FechaHora { get; set; }
        public string Recalculo { get; set; }
        public int fkEjercicio { get; set; }
        public string Empresa { get; set; }
        public string fkUsuario { get; set; }
        public int vueltas { get; set; }
        #endregion

        public ListadoAcumuladorPeriodos()
        {

        }
        
        public ListadoAcumuladorPeriodos(IContextService context) : base(context)
        {
            fkEjercicio = int.Parse(context.Ejercicio);
            vueltas = 0;
            Empresa = context.Empresa;
            Context = context;
        }
        internal override string GenerarFiltrosColumnas()
        {
            var db = MarfilEntities.ConnectToSqlServer(Context.BaseDatos);
            fkUsuario = Context.Usuario;
            vueltas++;

            var sb = new StringBuilder();
            string filtro = string.Empty;
            bool flag = false;
            string valor = "'F1',";
            string[] asientos = new string[8];
            asientos[0] = "F1";
            /*if (!string.IsNullOrEmpty(FechaDesde.ToShortDateString()) || !string.IsNullOrEmpty(FechaHasta.ToShortDateString()))
            {
                filtro = $" FechaHora BETWEEN '{FechaDesde.Year}-{FechaDesde.Month}-{FechaDesde.Day}' AND '{FechaHasta.Year}-{FechaHasta.Month}-{FechaHasta.Day}'";
            }*/
            if (!Ejercicio)
            {
                if (flag)
                    valor += ",";
                valor += "'R1','R2'";
                asientos[1] = "R1";
                asientos[2] = "R2";
                flag = true;
            }
            if (!Existencia)
            {
                if (flag)
                    valor += ",";

                valor += "'R3'";
                asientos[3] = "R3";
                flag = true;
            }
            if (!Grupos)
            {
                if (flag)
                    valor += ",";

                valor += "'R4'";
                asientos[4] = "R4";
                flag = true;
            }
            if (!CierreEjercicio)
            {
                if (flag)
                    valor += ",";

                valor += "'R5'";
                asientos[5] = "R5";
                flag = true;
            }
            if (IncluirAsientosSimulacion)
            {
                if (flag)
                    valor += ",";

                valor += "'F2'";
                asientos[6] = "F2";
                flag = true;
            }
            if (!ExcluirAsientosSimulacion)
            {
                if (flag)
                    valor += ",";

                valor += "'F3'";
                asientos[7] = "F3";
                flag = true;
            }

            if (flag)
            {
                filtro += $" empresa = {Empresa} AND fkejercicio = {fkEjercicio} AND usuario = '{fkUsuario}' order by fkcuentas";
            }
            sb.Append(filtro);

            if (vueltas == 1) { GenerarMovimiento(db, this, asientos); }

            return sb.ToString();
        }
        internal override string GenerarSelect()
        {
            var sb = new StringBuilder();
            sb.Append("Select fkcuentas as Cuenta ,nivel as Nivel, debe as Debe,haber as Haber from AcumuladorPeriodos");
            return sb.ToString();
        }


        public void GenerarMovimiento(MarfilEntities _db, ListadoAcumuladorPeriodos model, string[] asientos)// short multiplo)
        {
            try
            {
                ObtenerFiltros(_db, model);
                List<MovsLin> linMovimientos = ObtenerMovs(_db, model, asientos);

                CrearNivel0(_db, model, linMovimientos);
                CrearNiveles(_db, model, linMovimientos);
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
            }
        }

        private void CrearNiveles(MarfilEntities _db, ListadoAcumuladorPeriodos model, List<MovsLin> m)
        {
            // nivel 1 - 4
            for (int nivel = 4; nivel > 0; nivel--)
            {
                foreach (var item in m.GroupBy(l => l.fkcuentas.Substring(0, nivel)))
                {
                    string fkcuentas = item.Key;//.Substring(0, nivel);

                    var itemmaes = _db.AcumuladorPeriodos.SingleOrDefault(f => f.empresa == model.Empresa && f.fkcuentas == fkcuentas && f.fkejercicio == model.fkEjercicio && f.usuario == model.fkUsuario)
                                   ?? _db.AcumuladorPeriodos.Create();

                    if (string.IsNullOrWhiteSpace(itemmaes.empresa))
                    {
                        itemmaes.empresa = model.Empresa;
                        itemmaes.fkcuentas = fkcuentas;
                        itemmaes.fkejercicio = model.fkEjercicio;
                        itemmaes.nivel = nivel;
                        itemmaes.usuario = fkUsuario;
                        itemmaes.FechaHora = DateTime.Now;
                    }

                    itemmaes.debe = item.Where(l => l.esdebe == 1).Sum(l => l.importe);
                    itemmaes.haber = item.Where(l => l.esdebe == -1).Sum(l => l.importe);
                    itemmaes.saldo = (itemmaes.debe ?? 0) - (itemmaes.haber ?? 0);

                    _db.AcumuladorPeriodos.AddOrUpdate(itemmaes);
                }
            }
            _db.SaveChanges();
        }

        private static void CrearNivel0(MarfilEntities _db, ListadoAcumuladorPeriodos model, List<MovsLin> m)
        {
            // nivel 0
            foreach (var item in m.GroupBy(l => l.fkcuentas))
            {
                string fkcuentas = item.Key;

                var itemmaes = _db.AcumuladorPeriodos.SingleOrDefault(f => f.empresa == model.Empresa && f.fkcuentas == fkcuentas && f.fkejercicio == model.fkEjercicio && f.usuario == model.fkUsuario)
                              ?? _db.AcumuladorPeriodos.Create();

                if (string.IsNullOrWhiteSpace(itemmaes.empresa))
                {
                    itemmaes.empresa = model.Empresa;
                    itemmaes.fkcuentas = fkcuentas;
                    itemmaes.fkejercicio = model.fkEjercicio;
                    itemmaes.nivel = 0;
                    itemmaes.usuario = model.fkUsuario;
                    itemmaes.FechaHora = DateTime.Now;
                }


                itemmaes.debe = item.Where(l => l.esdebe == 1).Sum(l => l.importe);
                itemmaes.haber = item.Where(l => l.esdebe == -1).Sum(l => l.importe);
                itemmaes.saldo = (itemmaes.debe ?? 0) - (itemmaes.haber ?? 0);

                _db.AcumuladorPeriodos.AddOrUpdate(itemmaes);
            }
            _db.SaveChanges();
        }

        private static List<MovsLin> ObtenerMovs(MarfilEntities _db, ListadoAcumuladorPeriodos model, string[] asientos)
        {
            //Obtenemos los movimientos en base a los filtros del acumulador
            List<Movs> movimientos = new List<Movs>();
            List<MovsLin> linMovimientos = new List<MovsLin>();
            for (int x = 0; x < asientos.Length; x++)
            {
                string asiento = asientos[x];

                if (asiento != null)
                {
                    if (model.GrupoSeccion == null)
                    {
                        var movs = _db.Movs.Include(i => i.MovsLin).
                            Where(w => w.empresa == model.Empresa && w.fecha >= model.FechaDesde && w.fecha <= model.FechaHasta && w.fkejercicio == model.fkEjercicio && w.tipoasiento == asiento).ToList();
                        movimientos.AddRange(movs);
                    }
                    else
                    {
                        var movs = _db.Movs.Include(i => i.MovsLin).
                            Where(w => w.MovsLin.Any(i => i.fkseccionesanaliticas == model.GrupoSeccion) && w.empresa == model.Empresa && w.fecha >= model.FechaDesde && w.fecha <= model.FechaHasta && w.fkejercicio == model.fkEjercicio && w.tipoasiento == asiento).ToList();
                        movimientos.AddRange(movs);
                    }
                }
            }

            foreach (var m in movimientos)
            {
                var lin = m.MovsLin;
                linMovimientos.AddRange(lin);
            }

            return linMovimientos;
        }

        private static void ObtenerFiltros(MarfilEntities _db, ListadoAcumuladorPeriodos model)
        {
            //Eliminamos en anterior cálculo
            var id = from a in _db.AcumuladorPeriodos
                     where a.empresa == model.Empresa
                     where a.fkejercicio == model.fkEjercicio
                     where a.usuario == model.fkUsuario
                     select a.id;
            foreach (var a in id)
            {
                var anteriorCalculo = _db.AcumuladorPeriodos.Find(model.Empresa, a);
                _db.AcumuladorPeriodos.Remove(anteriorCalculo);
            }

            //Eliminamos los filtros anteriores
            var ident = from a in _db.FiltrosAcumulador
                        where a.empresa == model.Empresa
                        where a.fkejercicio == model.fkEjercicio
                        where a.usuario == model.fkUsuario
                        select a.id;
            foreach (var a in ident)
            {
                var anteriorCalculo = _db.FiltrosAcumulador.Find(model.Empresa, a);
                _db.FiltrosAcumulador.Remove(anteriorCalculo);
            }

            //Creamos el nuevo registro con los filtros
            var filtros = _db.FiltrosAcumulador.Create();

            if (string.IsNullOrWhiteSpace(filtros.empresa))
            {
                filtros.empresa = model.Empresa;
                filtros.fkejercicio = model.fkEjercicio;
                filtros.usuario = model.fkUsuario;
                filtros.fechaDesde = model.FechaDesde;
                filtros.fechaHasta = model.FechaHasta;
                filtros.seccion = model.GrupoSeccion;
                filtros.ejercicio = model.Ejercicio;
                filtros.existencia = model.Existencia;
                filtros.grupos = model.Grupos;
                filtros.cierreEjercicio = model.CierreEjercicio;
                filtros.incluirAsientos = model.IncluirAsientosSimulacion;
                filtros.excluirAsientos = model.ExcluirAsientosSimulacion;
                _db.FiltrosAcumulador.Add(filtros);
            }

            _db.SaveChanges();
        }
    }
}