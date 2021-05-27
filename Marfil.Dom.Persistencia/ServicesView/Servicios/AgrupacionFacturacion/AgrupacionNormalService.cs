using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Documentos.Albaranes;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos.Importar;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.AgrupacionFacturacion
{
    class AgrupacionNormalService : IAgrupacionService
    {


        #region Generar lineas

        public IEnumerable<ILineaImportar> GetLineasImportarAlbaran(AlbaranesService service, MarfilEntities db, string referencia)
        {
            var result = new List<ILineaImportar>();
            using (var con = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                var albaran = service.GetByReferencia(referencia);
                using (var cmd = new SqlCommand(GenerarCadena(referencia, albaran, service, db), con))
                {
                    cmd.Parameters.AddWithValue("empresa", service.Empresa);
                    cmd.Parameters.AddWithValue("referencia", referencia);
                    using (var ad = new SqlDataAdapter(cmd))
                    {

                        var lineas = new DataTable();
                        ad.Fill(lineas);
                        var id = 1;

                        foreach (DataRow row in lineas.Rows)
                        {
                            result.Add(new LineaImportarModel()
                            {
                                Id = id++,

                                Canal = lineas.Columns.Contains("Canal") ? Funciones.Qnull(row["Canal"]) : string.Empty,
                                Cantidad = Funciones.Qdouble(row["Cantidad"]) ?? 0,
                                Cuotaiva = Funciones.Qdouble(row["Cuotaiva"]) ?? 0,
                                Cuotarecargoequivalencia = Funciones.Qdouble(row["Cuotarecargoequivalencia"]) ?? 0,
                                Decimalesmedidas = Funciones.Qint(row["Decimalesmedidas"]) ?? 0,
                                Decimalesmonedas = Funciones.Qint(row["Decimalesmonedas"]) ?? 0,
                                Fkregimeniva = Funciones.Qnull(row["Fkregimeniva"]),
                                Fkunidades = Funciones.Qnull(row["Fkunidades"]),
                                Metros = Funciones.Qdouble(row["Metros"]) ?? 0,
                                Precio = Funciones.Qdouble(row["Precio"]) ?? 0,
                                Fkarticulos = Funciones.Qnull(row["Fkarticulos"]),
                                Descripcion = Funciones.Qnull(row["Descripcion"]),
                                Fktiposiva = Funciones.Qnull(row["Fktiposiva"]),
                                Contenedor = Funciones.Qnull(row["Contenedor"]) ?? "",
                                Ancho = lineas.Columns.Contains("Ancho") ? Funciones.Qdouble(row["Ancho"]) : null,
                                Grueso = lineas.Columns.Contains("Grueso") ? Funciones.Qdouble(row["Grueso"]) : null,
                                Largo = lineas.Columns.Contains("Largo") ? Funciones.Qdouble(row["Largo"]) : null,
                                Importe = Funciones.Qdouble(row["Importe"]) ?? 0,
                                Importedescuento = Funciones.Qdouble(row["Importedescuento"]) ?? 0,
                                Porcentajedescuento = Funciones.Qdouble(row["Porcentajedescuento"]) ?? 0,
                                Porcentajeiva = Funciones.Qdouble(row["Porcentajeiva"]) ?? 0,
                                Porcentajerecargoequivalencia = Funciones.Qdouble(row["Porcentajerecargoequivalencia"]) ?? 0,
                                Lote = lineas.Columns.Contains("Lote") ? Funciones.Qnull(row["Lote"]) : string.Empty,
                                Caja = lineas.Columns.Contains("Caja") ? Funciones.Qint(row["Caja"]) : null,
                                Sello = lineas.Columns.Contains("Sello") ? Funciones.Qnull(row["Sello"]) : string.Empty,
                                Pesoneto = lineas.Columns.Contains("Pesoneto") ? Funciones.Qdouble(row["Pesoneto"]) : 0,
                                Notas = string.Empty,//Funciones.Qnull(row["Notas"]),
                                Precioanterior = 0,
                                Revision = "",
                                Tabla = null,
                                Fkdocumento = albaran.Id.ToString(),
                                Fkdocumentoid = "",
                                Fkdocumentoreferencia = ""
                            });
                        }
                    }
                }
            }

            return result;
        }

        private string GenerarCadena(string referencia, AlbaranesModel albaran, AlbaranesService service, MarfilEntities db)
        {
            var sb = new StringBuilder();

            string agrupacion;
            var columnas = GenerarColumnas(referencia, out agrupacion, albaran, service, db);
            sb.AppendFormat("select {0} from albaraneslin as al inner join albaranes as a on a.empresa=@empresa and a.referencia=@referencia and a.id=al.fkalbaranes and a.empresa=al.empresa left join articulos as art on art.id=al.fkarticulos where al.empresa=@empresa {1}", columnas, agrupacion);

            return sb.ToString();
        }

        private string GenerarColumnas(string referencia, out string agrupacion, AlbaranesModel albaran, AlbaranesService service, MarfilEntities db)
        {
            var agrupacionService = FService.Instance.GetService(typeof(CriteriosagrupacionModel), service._context, db) as CriteriosagrupacionService;

            var criterio = agrupacionService.get(albaran.Fkcriteriosagrupacion) as CriteriosagrupacionModel;

            var sb = new StringBuilder();

            if (criterio.Lineas.Any())
            {
                var lineas = criterio.Lineas.OrderBy(f => f.Orden);
                agrupacion = lineas.Any() ? "Group by " : string.Empty;
                agrupacion += string.Join(",", lineas.Select(f => "al." + f.Campoenum.ToString()));

                if (!lineas.Any(f => f.Campoenum == CamposAgrupacionAlbaran.Fkarticulos))
                    agrupacion += ",al.Fkarticulos";
                if (!lineas.Any(f => f.Campoenum == CamposAgrupacionAlbaran.Precio))
                    agrupacion += ",al.Precio";
                if (!lineas.Any(f => f.Campoenum == CamposAgrupacionAlbaran.Porcentajedescuento))
                    agrupacion += ",al.Porcentajedescuento";
                if (!lineas.Any(f => f.Campoenum == CamposAgrupacionAlbaran.Porcentajedescuento))
                    agrupacion += ",al.Porcentajedescuento";

                agrupacion += ",al.empresa,al.Fkunidades,al.Fktiposiva,al.Sello,al.Pesoneto,al.Porcentajerecargoequivalencia,al.Porcentajeiva,al.Decimalesmedidas,al.Decimalesmonedas";

                sb.Append(" al.Descripcion as Descripcion,Min(al.Id) as Id,Sum(al.cantidad) as Cantidad,sum(al.Metros) as Metros,al.Porcentajeiva,Sum(al.Cuotaiva) as Cuotaiva," +
                          " al.Porcentajerecargoequivalencia,sum(al.Cuotarecargoequivalencia) as Cuotarecargoequivalencia,'" + albaran.Fkregimeniva + "' as Fkregimeniva," +
                          " al.Fkunidades, al.Fktiposiva, al.Sello, al.Pesoneto, Sum(al.Importe) as Importe,Sum(al.importedescuento) as Importedescuento,al.Decimalesmedidas,al.Decimalesmonedas, " + string.Join(",", lineas.Select(f => "al." + f.Campoenum.ToString())));

                if (!lineas.Any(f => f.Campoenum == CamposAgrupacionAlbaran.Fkarticulos))
                    sb.Append(", al.Fkarticulos");
                if (!lineas.Any(f => f.Campoenum == CamposAgrupacionAlbaran.Precio))
                    sb.Append(", al.Precio");
                if (!lineas.Any(f => f.Campoenum == CamposAgrupacionAlbaran.Porcentajedescuento))
                    sb.Append(", al.Porcentajedescuento");
                if (!lineas.Any(f => f.Campoenum == CamposAgrupacionAlbaran.Descripcion))
                    sb.Append(", (select top 1 art.descripcion from articulos as art where art.id=al.fkarticulos and art.empresa=al.empresa) as Descripcion");


                var cadenaCondiciones = "";
                var vectorAgrupacion = agrupacion.Replace("Group by", "").Split(',');
                foreach (var item in vectorAgrupacion)
                {
                    cadenaCondiciones += " and isnull(" + item + ",-1)= isnull(" + item.Replace("al.", "al2.") + ",-1) ";
                }

                var cadenaFormat =
                    ", (select (case when count(distinct al2.{0}) >1 then 0 else min(al2.{0}) end) from albaraneslin as al2 where al2.empresa=@empresa and al2.fkalbaranes={2} {3} {1}) as {0}  ";
                if (!lineas.Any(f => f.Campoenum == CamposAgrupacionAlbaran.Largo))
                    sb.AppendFormat(cadenaFormat, CamposAgrupacionAlbaran.Largo, agrupacion.Replace("al.", "al2."), albaran.Id, cadenaCondiciones);

                if (!lineas.Any(f => f.Campoenum == CamposAgrupacionAlbaran.Ancho))
                    sb.AppendFormat(cadenaFormat, CamposAgrupacionAlbaran.Ancho, agrupacion.Replace("al.", "al2."), albaran.Id, cadenaCondiciones);

                if (!lineas.Any(f => f.Campoenum == CamposAgrupacionAlbaran.Grueso))
                    sb.AppendFormat(cadenaFormat, CamposAgrupacionAlbaran.Grueso, agrupacion.Replace("al.", "al2."), albaran.Id, cadenaCondiciones);

            }
            else
            {
                agrupacion = string.Empty;
                sb.Append(" al.*,a.Fkregimeniva");
            }


            return sb.ToString();
        }

        #endregion
    }
}
