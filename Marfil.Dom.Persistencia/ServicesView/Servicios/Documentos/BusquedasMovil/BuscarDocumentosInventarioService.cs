using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.ControlsUI.Busquedas;
using Marfil.Dom.ControlsUI.BusquedaTerceros;
using Marfil.Dom.Persistencia.Model.Documentos;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RPresupuestos=Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Presupuestos;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos.BusquedasMovil
{
    internal class BuscarDocumentosInventarioService
    {
        private readonly MarfilEntities _db;
        private readonly string _empresa;

        public BuscarDocumentosInventarioService(MarfilEntities db,string empresa)
        {
            _db = db;
            _empresa = empresa;
        }

        #region Buscar documentos

        public IEnumerable<DocumentosBusqueda> Buscar(IDocumentosFiltros filtros,out int registrostotales)
        {
            var tipo = (TipoDocumento)Enum.Parse(typeof(TipoDocumento), filtros.Tipodocumento);
            var sbtotales=new StringBuilder();

            sbtotales.AppendFormat("Select count(1) from inventarios  as d where d.empresa='{0}' {1}", _empresa,GenerarFiltros(filtros));
            registrostotales = _db.Database.SqlQuery<int>(sbtotales.ToString()).Single();

            var sb = new StringBuilder();
            sb.AppendFormat("select d.* from inventarios as d where d.empresa = '{0}' {1}",  _empresa,GenerarFiltros(filtros));
            sb.AppendFormat(" order by d.id desc offset {0}*({1}-1) rows fetch next {0} rows only option (recompile)",filtros.RegistrosPagina,filtros.Pagina);

            return _db.Database.SqlQuery<DocumentosBusqueda>(sb.ToString()).ToList();
        }

        

        private string GenerarFiltros(IDocumentosFiltros filtros)
        {
            var sb=new StringBuilder();

            if (!string.IsNullOrEmpty(filtros.Filtros))
            {
                sb.AppendFormat(" AND ( ");
                sb.AppendFormat(" d.Referencia like '%{0}%' ", filtros.Filtros);
                sb.AppendFormat(" OR d.descripcion like '%{0}%' ", filtros.Filtros);
                sb.AppendFormat(" OR convert(varchar(20),d.fechadocumento,103) = '{0}' ", filtros.Filtros);
                sb.AppendFormat(" ) ");
            }

            return sb.ToString();
        }

        #endregion

        #region Buscar un documento

        public IEnumerable<IItemResultadoMovile> Get<T,TLineas>(IGestionService service,string referencia)
        {
            var serviceReferencia =  service as IDocumentosVentasPorReferencia<T>;
            var model = serviceReferencia.GetByReferencia(referencia) as IModelView;
            model = service.get(Funciones.Qnull(model.get("Id")));
            var result = new List<IItemResultadoMovile>();

            result.Add(new ItemCabeceraResultadoMoviles() {Valor= General.LblGeneral});
            result.Add(new ItemResultadoMovile() {Campo= RPresupuestos.Referencia,Valor = Funciones.Qnull(model.get("Referencia"))});
            result.Add(new ItemResultadoMovile() { Campo = RPresupuestos.Fechadocumento, Valor = Funciones.Qnull(model.get("Fechadocumento")) });
           

            result.Add(new ItemCabeceraResultadoMoviles() { Valor = General.LblDesglose });
            var lineas = model.get("Lineas") as List<TLineas>;

            foreach (var elem in lineas)
            {
                var item = elem as ILineasDocumentosBusquedaMovil;
                result.Add(new ItemResultadoMovile() { Campo = RPresupuestos.Fkarticulos, Valor = item.Fkarticulos });
                result.Add(new ItemResultadoMovile() { Campo = RPresupuestos.Descripcion, Valor = item.Descripcion });
                result.Add(new ItemResultadoMovile() { Campo = RPresupuestos.Cantidad, Valor = item.Cantidad?.ToString()??string.Empty });
                result.Add(new ItemResultadoMovile() { Campo = RPresupuestos.Largo, Valor = item.SLargo });
                result.Add(new ItemResultadoMovile() { Campo = RPresupuestos.Ancho, Valor = item.SAncho });
                result.Add(new ItemResultadoMovile() { Campo = RPresupuestos.Grueso, Valor = item.SGrueso });
                result.Add(new ItemResultadoMovile() { Campo = RPresupuestos.Metros, Valor = item.SMetros });
                if (elem.GetType().GetProperties().Any(f => f.Name == "Estado"))
                {
                    result.Add(new ItemResultadoMovile() { Campo = RPresupuestos.Estado, Valor = Funciones.Qnull(elem.GetType().GetProperty("Estado").GetValue(elem)) });
                }
                
                result.Add(new ItemSeparadorResultadoMoviles());
            }

            


            return result;
        }

        #endregion
    }
}
