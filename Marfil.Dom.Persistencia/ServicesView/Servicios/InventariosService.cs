using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Marfil.Dom.ControlsUI.Busquedas;
using Marfil.Dom.ControlsUI.BusquedaTerceros;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Listados;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Documentos;
using Marfil.Dom.Persistencia.Model.Documentos.Inventarios;
using Marfil.Dom.Persistencia.Model.Documentos.Reservasstock;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Converter;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos.BusquedasMovil;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Stock;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Validation;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RInventarios = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Inventarios;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IInventariosService
    {

    }

    public class InventariosService : GestionService<InventariosModel, Inventarios>,IBuscarDocumento, IDocumentosVentasPorReferencia<InventariosModel>, IInventariosService
    {
        #region CTR

        public InventariosService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        #region API

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var st= base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var propiedadesVisibles = new[] { "Referencia", "Fechadocumento", "Descripcion" };
            var propiedades = Helpers.Helper.getProperties<InventariosModel>();
            st.PrimaryColumnns = new[] { "Id" };
            st.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();

            return st;
        }

        public override void create(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as InventariosModel;

                //Calculo ID
                var tipodocumento = "INV"; //Inventarios
                var contador = ServiceHelper.GetNextId<Inventarios>(_db, Empresa, model.Fkseries, tipodocumento);
                var identificadorsegmento = "";
                model.Referencia = ServiceHelper.GetReference<Inventarios>(_db, model.Empresa, model.Fkseries, tipodocumento, contador, model.Fechadocumento, out identificadorsegmento);
                model.Identificadorsegmento = identificadorsegmento;

                base.create(obj);

                _db.SaveChanges();
                tran.Complete();
            }
        }

        public override void edit(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var original = get(Funciones.Qnull(obj.get("id"))) as InventariosModel;
                var editado = obj as InventariosModel;
                if (original.Integridadreferencial == editado.Integridadreferencial)
                {
                    base.edit(obj);

                    _db.SaveChanges();
                    tran.Complete();
                }
                else throw new IntegridadReferencialException(string.Format(General.ErrorIntegridadReferencial, RInventarios.TituloEntidad, original.Referencia));
            }

        }

        #region Calcular listado inventario
        private class StockActualVistaModel
        {
            public string Empresa { get; set; }


            public DateTime Fecha { get; set; }

            public string Fkalmacenes { get; set; }

            public int? Fkalmaceneszona { get; set; }

            public string Fkalmaceneszonadescripcion { get; set; }

            public string Fkarticulos { get; set; }

            public string Descripcion { get; set; }

            public int Decimalesmedidas { get; set; }

            public string Referenciaproveedor { get; set; }

            public string Lote { get; set; }

            public string Loteid { get; set; }

            public string Tag { get; set; }

            public string Fkunidadesmedida { get; set; }

            public string Fkunidadesmedidadescripcion { get; set; }

            public double Cantidaddisponible { get; set; }

            public double Largo { get; set; }

            public double Ancho { get; set; }

            public double Grueso { get; set; }

            public double? Metros { get; set; }

            public string Fkcalificacioncomercial { get; set; }

            public string Fktipograno { get; set; }

            public string Fktonomaterial { get; set; }

            public string Fkincidenciasmaterial { get; set; }

            public string Fkvariedades { get; set; }

            public Guid Integridadreferencialflag { get; set; }
        }
        public List<InventariosLinModel> CalcularListadoInventarios(InventariosModel model)
        {
            var result = new List<InventariosLinModel>();
            var i = 1;

            result =
                _db.Database.SqlQuery<StockActualVistaModel>(string.Format("{0}{1}", GetSelect(), GenerarFiltrosColumnas(model)),
                    new SqlParameter("fkalmacen",model.Fkalmacenes),
                    new SqlParameter("tipodealmacenlote", (model.Tipodealmacenlote).HasValue ? ((int)model.Tipodealmacenlote).ToString() : string.Empty ),
                    new SqlParameter("fkzonasalmacen", model.Fkalmaceneszonas?.ToString()??string.Empty),
                    new SqlParameter("fkarticulosdesde", model.Fkarticulosdesde??string.Empty),
                    new SqlParameter("fkarticuloshasta", model.Fkarticuloshasta ?? string.Empty),
                    new SqlParameter("fkfamiliasmateriales", model.Fkfamiliamaterial ?? string.Empty),
                    new SqlParameter("fkfamiliasdesde", model.Fkfamiliaproductodesde ?? string.Empty),
                    new SqlParameter("fkfamiliashasta", model.Fkfamiliaproductohasta ?? string.Empty),
                     new SqlParameter("fkmaterialdesde", model.Fkmaterialdesde ?? string.Empty),
                    new SqlParameter("Fkmaterialhasta", model.Fkmaterialhasta ?? string.Empty),
                    new SqlParameter("fkcaracteristicasdesde", model.Fkcaracteristicadesde ?? string.Empty),
                    new SqlParameter("fkcaracteristicashasta", model.Fkcaracteristicahasta ?? string.Empty),
                    new SqlParameter("fkgrosoresdesde", model.Fkgrosordesde ?? string.Empty),
                    new SqlParameter("fkgrosoreshasta", model.Fkgrosorhasta ?? string.Empty),
                    new SqlParameter("fkacabadosdesde", model.Fkacabadodesde ?? string.Empty),
                    new SqlParameter("fkacabadoshasta", model.Fkacabadohasta ?? string.Empty))
                    .Select(f=>new InventariosLinModel()
                    {
                        Id= i++,
                        Fecha = f.Fecha,
                        Fkarticulos = f.Fkarticulos,
                        Descripcion = f.Descripcion,
                        Lote = f.Lote,
                        Loteid= f.Loteid,
                        Referenciaproveedor = f.Referenciaproveedor,
                        Tag = f.Tag,
                        Fkunidadesmedida = f.Fkunidadesmedida,
                        Cantidad= f.Cantidaddisponible,
                        Largo =f.Largo,
                        Ancho =f.Ancho,
                        Grueso=f.Grueso,
                        Metros = f.Metros,
                        Fkcalificacioncomercial = f.Fkcalificacioncomercial,
                        Fktipograno = f.Fktipograno,
                        Pesonetolote = 0,
                        Fktonomaterial = f.Fktonomaterial,
                        Fkincidenciasmaterial = f.Fkincidenciasmaterial,
                        Fkvariedades = f.Fkvariedades,
                        Estado=EstadoLineaInventario.Pendiente,
                        Decimalesmedidas = f.Decimalesmedidas,
                        Fkunidadesmedidadescripcion = f.Fkunidadesmedidadescripcion
                    }).ToList();
           

            return result;
        }

        private string GetSelect()
        {
        
            var sb = new StringBuilder();
            sb.AppendFormat("select s.*,a.descripcion as [Descripcion],u.decimalestotales as [Decimalesmedidas],u.codigounidad as [Fkunidadesmedidadescripcion] ");
            sb.AppendFormat(" from stockactual as s ");
            sb.AppendFormat(" inner join articulos as a on a.id = s.fkarticulos and a.empresa= s.empresa ");
            sb.AppendFormat(" inner join familiasproductos as fp on fp.id = substring(s.fkarticulos, 0, 3) and fp.empresa= a.empresa ");
            sb.AppendFormat(" inner join unidades as u on u.id = fp.fkunidadesmedida");
            sb.AppendFormat(" left join materiales as ml on ml.id = substring(s.fkarticulos, 3, 3) and ml.empresa= a.empresa ");
            sb.AppendFormat(" left join Familiamateriales  as fm on fm.valor=ml.fkfamiliamateriales ");
            return sb.ToString();
        
        }

        private string GenerarFiltrosColumnas(InventariosModel model)
        {
            var sb = new StringBuilder();
            var flag = true;
           
            sb.AppendFormat(" where s.empresa='{0}' ", Empresa);
            if (!string.IsNullOrEmpty(model.Fkalmacenes))
            {
                if (flag)
                    sb.Append(" AND ");
                sb.Append(" s.fkalmacenes = @fkalmacen  ");
                flag = true;
            }

            if (model.Tipodealmacenlote.HasValue)
            {
                if (flag)
                    sb.Append(" AND ");
                sb.Append(" s.Tipoalmacenlote = @Tipodealmacenlote  ");

                flag = true;
            }

            if (model.Fkalmaceneszonas.HasValue)
            {
                if (flag)
                    sb.Append(" AND ");
                sb.Append(" s.fkalmaceneszona = @fkzonasalmacen  ");
                
                flag = true;
            }

            if (!string.IsNullOrEmpty(model.Fkarticulosdesde))
            {
                if (flag)
                    sb.Append(" AND ");

               
                sb.Append(" s.fkarticulos >= @fkarticulosdesde  ");
               
                flag = true;
            }

            if (!string.IsNullOrEmpty(model.Fkarticuloshasta))
            {
                if (flag)
                    sb.Append(" AND ");

             
                sb.Append(" s.fkarticulos <= @fkarticuloshasta  ");
             
                flag = true;
            }

            if (!string.IsNullOrEmpty(model.Fkfamiliamaterial))
            {
                if (flag)
                    sb.Append(" AND ");

               
                sb.Append("  exists(select mm.* from materiales as mm where mm.id=Substring(s.fkarticulos,3,3) and mm.fkfamiliamateriales=@fkfamiliasmateriales)  ");
               
                flag = true;
            }

            if (!string.IsNullOrEmpty(model.Fkfamiliaproductodesde))
            {
                if (flag)
                    sb.Append(" AND ");

               
                sb.Append(" Substring(s.fkarticulos,0,3) >= @fkfamiliasdesde  ");
               
                flag = true;
            }

            if (!string.IsNullOrEmpty(model.Fkfamiliaproductohasta))
            {
                if (flag)
                    sb.Append(" AND ");

              
                sb.Append(" Substring(s.fkarticulos,0,3) <= @fkfamiliashasta  ");
              
                flag = true;
            }

            if (!string.IsNullOrEmpty(model.Fkmaterialdesde))
            {
                if (flag)
                    sb.Append(" AND ");


                sb.Append(" Substring(s.fkarticulos,3,3) >= @fkmaterialdesde  ");

                flag = true;
            }

            if (!string.IsNullOrEmpty(model.Fkmaterialhasta))
            {
                if (flag)
                    sb.Append(" AND ");

              
                sb.Append(" Substring(s.fkarticulos,3,3) <= @fkmaterialeshasta  ");
              
                flag = true;
            }

            if (!string.IsNullOrEmpty(model.Fkcaracteristicadesde))
            {
                if (flag)
                    sb.Append(" AND ");

                
                sb.Append(" Substring(s.fkarticulos,6,2) >= @fkcaracteristicasdesde  ");
                
                flag = true;
            }

            if (!string.IsNullOrEmpty(model.Fkcaracteristicahasta))
            {
                if (flag)
                    sb.Append(" AND ");

               
                sb.Append(" Substring(s.fkarticulos,6,2) <= @fkcaracteristicashasta  ");
               
                flag = true;
            }

            if (!string.IsNullOrEmpty(model.Fkgrosordesde))
            {
                if (flag)
                    sb.Append(" AND ");

              
                sb.Append(" Substring(s.fkarticulos,8,2) >= @fkgrosoresdesde  ");
              
                flag = true;
            }

            if (!string.IsNullOrEmpty(model.Fkgrosorhasta))
            {
                if (flag)
                    sb.Append(" AND ");

               
                sb.Append(" Substring(s.fkarticulos,8,2) <= @fkgrosoreshasta  ");
               
                flag = true;
            }

            if (!string.IsNullOrEmpty(model.Fkacabadodesde))
            {
                if (flag)
                    sb.Append(" AND ");

               
                sb.Append(" Substring(s.fkarticulos,10,2) >= @fkacabadosdesde  ");
               
                flag = true;
            }

            if (!string.IsNullOrEmpty(model.Fkacabadohasta))
            {
                if (flag)
                    sb.Append(" AND ");

               
                sb.Append(" Substring(s.fkarticulos,10,2) <= @fkacabadoshasta  ");
               
                flag = true;
            }



            return sb.ToString();
        }

        #endregion

        #endregion

        #region Buscar documento

        public IEnumerable<DocumentosBusqueda> Buscar(IDocumentosFiltros filtros, out int registrostotales)
        {
            var service = new BuscarDocumentosInventarioService(_db, Empresa);
            return service.Buscar(filtros, out registrostotales);
        }

        public IEnumerable<IItemResultadoMovile> BuscarDocumento(string referencia)
        {
            var service = new BuscarDocumentosInventarioService(_db, Empresa);
            return service.Get<InventariosModel, InventariosLinModel>(this, referencia);
        }

        #endregion

        #region Buscar por referencia

        public InventariosModel GetByReferencia(string referencia)
        {
            var obj = _db.Inventarios.Single(f => f.empresa == Empresa && f.referencia == referencia);
            
            return _converterModel.GetModelView(obj) as InventariosModel;
        }

        #endregion

        #region Validar linea inventario mobile

        public AgregarLineaInventariosDocumentosModel AgregarLinea(string referencia, string lote)
        {
            return OperarLinea(referencia, lote, true);
        }

        public AgregarLineaInventariosDocumentosModel EliminarLinea(string referencia, string lote)
        {
            return OperarLinea(referencia, lote, false);
        }

        private AgregarLineaInventariosDocumentosModel OperarLinea(string referencia, string lote, bool agregar)
        {
            var result = new AgregarLineaInventariosDocumentosModel();
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = GetByReferencia(referencia);
                model = get(model.Id.ToString()) as InventariosModel;

                result.Referencia = model.Referencia;
                result.Fecha = model.Fechadocumentocadena;
                model.Lineas = EditarLineasInventarios(model.Lineas, lote, agregar);
                    

                edit(model);

                result.Lineas = model.Lineas
                    .Select(f => new AgregarLineaInventariosDocumentosLinModel()
                    {
                        Lote = string.Format("{0}{1}", f.Lote, Funciones.RellenaCod(f.Loteid, 3)),
                        Largo = f.SLargo,
                        Ancho = f.SAncho,
                        Grueso = f.SGrueso,
                        Cantidad = f.Cantidad.ToString(),
                        Descripcion = f.Descripcion,
                        Fkarticulos = f.Fkarticulos,
                        Metros = f.SMetros,
                        Estado= Funciones.GetEnumByStringValueAttribute(f.Estado),
                        Codigoestado = ((int)f.Estado).ToString()
                    }).ToList();

                tran.Complete();
            }


            return result;
        }

        private List<InventariosLinModel> EditarLineasInventarios(List<InventariosLinModel> lineas, string referencialote, bool agregar)
        {
            string lote;
            string loteid;
            StockactualService.CalcularPartesLote(referencialote,out lote,out loteid);
            var linea = lineas.Single(f => f.Lote == lote && f.Loteid == loteid);
            
            linea.Estado=agregar ? EstadoLineaInventario.Punteado :EstadoLineaInventario.Pendiente;
            

            return lineas;
        }

        #endregion
    }
}
