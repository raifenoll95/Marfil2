using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IContadoresLotesService
    {

    }

    public class ContadoresLotesService : GestionService<ContadoresLotesModel, ContadoresLotes>, IContadoresLotesService
    {
        #region CTR

        public ContadoresLotesService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var st= base.GetListIndexModel(t, canEliminar, canModificar, controller);
            st.ExcludedColumns = new[] {"Empresa", "Lineas","Toolbar","Tipocontador"};
            return st;
        }

        public string CreateLoteId(ContadoresLotesModel model,ref int incremento)
        {
            var movimientos = _db.Database.SqlQuery<Movimientosstock>(GenerearQuery(model));
            var maxid = movimientos.Any() ? movimientos.Max(f =>f.lote) : string.Empty;

            var longitudpartefija = 0;
            foreach (var item in model.Lineas)
            {
                if (item.Tiposegmento != TiposLoteSegmentos.Secuencia)
                    longitudpartefija += item.Longitud;
            }

            var partesecuencia = !string.IsNullOrEmpty(maxid) ? maxid.Substring(longitudpartefija) : "0";
            var partesecuencianumerico = (Funciones.Qint(partesecuencia) ??0) + incremento;
            var sb=new  StringBuilder();
            
            var loopBusquedaLote = true;
            do
            {
                sb = new StringBuilder();
                foreach (var item in model.Lineas)
                {
                    if (item.Tiposegmento == TiposLoteSegmentos.Año)
                    {
                        var offset = model.Offset ?? 0;

                        //Obtenemos el ejercicio en el que estamos logueados
                        var ejercicioid = Int32.Parse(_context.Ejercicio);
                        var ejercicio = _db.Ejercicios.Where(f => f.empresa == _context.Empresa && f.id == ejercicioid).Select(f => f.desde).FirstOrDefault();
                        var yearstr = ejercicio.Value.Year.ToString().Substring(2,2);
                        var year = Int32.Parse(yearstr);

                        //var year =
                        //    Funciones.Qint(item.Longitud == 2
                        //        ? DateTime.Now.ToString("yy")
                        //        : DateTime.Now.ToString("yyyy"));

                        year += offset;
                        year =year % (item.Longitud == 2
                                ? 100
                                : 10000);

                        sb.Append(Funciones.RellenaCod(year.ToString(),item.Longitud));
                    }
                    else if (item.Tiposegmento == TiposLoteSegmentos.Mes)
                    {
                        sb.Append(DateTime.Now.ToString("MM"));
                    }
                    else if (item.Tiposegmento == TiposLoteSegmentos.Semana)
                    {
                        sb.Append(Funciones.RellenaCod(CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(DateTime.Now, CultureInfo.InvariantCulture.DateTimeFormat.CalendarWeekRule, CultureInfo.InvariantCulture.DateTimeFormat.FirstDayOfWeek).ToString(), 2));
                    }
                    else if (item.Tiposegmento == TiposLoteSegmentos.Constante)
                    {
                        sb.Append(item.Valor);
                    }
                    else if (item.Tiposegmento == TiposLoteSegmentos.Secuencia)
                    {
                        sb.Append(Funciones.RellenaCod((++partesecuencianumerico).ToString(), item.Longitud));
                    }


                }
                var cadenaLoteActual = sb.ToString();
                loopBusquedaLote = _db.Movimientosstock.Any(f => f.empresa == model.Empresa && f.lote == cadenaLoteActual);
                incremento++;

            } while (loopBusquedaLote);
            
            return sb.ToString();

        }

        private string GenerearQuery(ContadoresLotesModel model)
        {

            var condicionfecha = "";
            if (model.Tipoinicio == TipoLoteInicio.Anual)
                condicionfecha=" and Year(m.fecha) = Year(GETDATE()) ";
            else if (model.Tipoinicio == TipoLoteInicio.Mensual)
                condicionfecha = " and Month(m.fecha) = Month(GETDATE()) ";
            else if (model.Tipoinicio == TipoLoteInicio.Semanal)
                condicionfecha = " and DATEPART(m.fecha) = DATEPART(GETDATE()) ";


            return
                string.Format("select m.* from movimientosstock as m where m.empresa='{0}' and m.fkcontadorlote='{1}' {2}",
                    model.Empresa, model.Id, condicionfecha);
        }
    }
}
