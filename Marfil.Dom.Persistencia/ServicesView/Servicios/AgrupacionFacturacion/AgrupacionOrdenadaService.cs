using System;
using System.Collections;
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
    class AgrupacionOrdenadaService : IAgrupacionService
    {
        
        public IEnumerable<ILineaImportar> GetLineasImportarAlbaran(AlbaranesService service, MarfilEntities db, string referencia)
        {
            var albaran = service.GetByReferencia(referencia);
            return GetLineas(service.get(albaran.Id.ToString()) as AlbaranesModel, service, db);
        }

        private List<ILineaImportar> GetLineas(AlbaranesModel albaran,AlbaranesService service,MarfilEntities db)
        {
            var agrupacionService = FService.Instance.GetService(typeof(CriteriosagrupacionModel), service._context, db) as CriteriosagrupacionService;
            var criterio = agrupacionService.get(albaran.Fkcriteriosagrupacion) as CriteriosagrupacionModel;

            var id = 1;
            var cadenaAnterior = "";
            var result = new List<ILineaImportar>();
            foreach (var linea in albaran.Lineas.OrderBy(f=>f.Orden))
            {
                var cadenaActual = CreateIdCadena(criterio, linea);
                if (cadenaAnterior == cadenaActual)
                {
                    EditarLineaImportar(result[result.Count-1], linea,criterio);
                }
                else
                {
                    var nuevalinea = CreateLineaImportar(id++, albaran, linea);
                    result.Add(nuevalinea);
                }
                cadenaAnterior = cadenaActual;
            }

            return result;
        }

        private string CreateIdCadena(CriteriosagrupacionModel model, AlbaranesLinModel linea)
        {
            var formato = "";
            formato +=string.Format("{0};{1};{2}",linea.Fkarticulos,linea.Precio,linea.Porcentajedescuento);
            var properties = linea.GetType();
            foreach (var item in model.Lineas.OrderBy(f=>f.Orden))
            {
                formato += string.Format(";{0}",properties.GetProperty(item.Campoenum.ToString()).GetValue(linea));
            }
            return formato;
        }

        private ILineaImportar CreateLineaImportar(int id,AlbaranesModel albaran, AlbaranesLinModel linea)
        {
            return new LineaImportarModel()
            {
                Id = id,
                Canal =linea.Canal,
                Cantidad = linea.Cantidad??0,
                Cuotaiva = linea.Cuotaiva ??0,
                Cuotarecargoequivalencia = linea.Cuotarecargoequivalencia ?? 0,
                Decimalesmedidas = linea.Decimalesmedidas ?? 0,
                Decimalesmonedas = linea.Decimalesmonedas ?? 0,
                Descripcion =linea.Descripcion,
                Fkregimeniva = linea.Fkregimeniva,
                Fkunidades = linea.Fkunidades,
                Metros = linea.Metros ?? 0,
                Precio = linea.Precio ?? 0,
                Fkarticulos = linea.Fkarticulos,
                Fktiposiva = linea.Fktiposiva,
                Ancho = linea.Ancho,
                Grueso = linea.Grueso,
                Largo = linea.Largo,
                Importe =linea.Importe??0,
                Importedescuento = linea.Importedescuento ?? 0,
                Porcentajedescuento = linea.Porcentajedescuento ?? 0,
                Porcentajeiva = linea.Porcentajeiva?? 0,
                Porcentajerecargoequivalencia = linea.Porcentajerecargoequivalencia ?? 0,
                Lote = linea.Lote,
                Notas = string.Empty,//Funciones.Qnull(row["Notas"]),
                Precioanterior = 0,
                Revision = "",
                Tabla = null,
                Fkdocumento = albaran.Id.ToString(),
                Fkdocumentoid = "",
                Fkdocumentoreferencia = ""
            };
        }

        private void EditarLineaImportar(ILineaImportar item,AlbaranesLinModel linea, CriteriosagrupacionModel agrupacion)
        {
            item.Cantidad += linea.Cantidad??0;
            item.Metros += linea.Metros ?? 0;
            item.Importe += linea.Importe ?? 0;
            item.Importedescuento += linea.Importedescuento ?? 0;
            item.Cuotaiva += linea.Cuotaiva ?? 0;
            item.Cuotarecargoequivalencia += linea.Cuotarecargoequivalencia ?? 0;

            if (!agrupacion.Lineas.Any(f => f.Campoenum == CamposAgrupacionAlbaran.Largo))
            {
                item.Largo = item.Largo == linea.Largo ? item.Largo : 0;
            }
            if (!agrupacion.Lineas.Any(f => f.Campoenum == CamposAgrupacionAlbaran.Ancho))
            {
                item.Ancho = item.Ancho == linea.Ancho ? item.Ancho : 0;
            }
            if (!agrupacion.Lineas.Any(f => f.Campoenum == CamposAgrupacionAlbaran.Grueso))
            {
                item.Grueso = item.Grueso == linea.Grueso ? item.Grueso : 0;
            }
        }
       
    }
}
