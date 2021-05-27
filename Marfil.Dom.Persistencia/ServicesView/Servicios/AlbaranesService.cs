using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Marfil.Dom.ControlsUI.Busquedas;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Documentos;
using Marfil.Dom.Persistencia.Model.Documentos.Albaranes;
using Marfil.Dom.Persistencia.Model.Documentos.Pedidos;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios.AgrupacionFacturacion;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Converter;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos.BusquedasMovil;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos.Importar;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Validation;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RAlbaranes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Albaranes;
using Marfil.Dom.ControlsUI.BusquedaTerceros;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Stock;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Helpers;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IAlbaranesService
    {
        
    }

    public class AlbaranesService : GestionService<AlbaranesModel, Albaranes>, IDocumentosServices, IBuscarDocumento, IDocumentosVentasPorReferencia<AlbaranesModel>, IAlbaranesService
    {
        #region Member

        private string _ejercicioId;
        private IImportacionService _importarService;
        #endregion

        #region Properties

        public string EjercicioId
        {
            get { return _ejercicioId; }
            set
            {
                _ejercicioId = value;
                ((AlbaranesValidation)_validationService).EjercicioId = value;
            }
        }

        #endregion

        #region CTR

        public AlbaranesService(IContextService context, MarfilEntities db = null) : base(context, db)
        {
            _importarService = new ImportacionService(context);

        }

        #endregion

        #region List index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var st = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var estadosService = new EstadosService(_context,_db);
            st.List = st.List.OfType<AlbaranesModel>().OrderByDescending(f => f.Fechadocumento).ThenByDescending(f => f.Referencia);
            var propiedadesVisibles = new[] { "Referencia", "Fechadocumento", "Fkclientes", "Nombrecliente", "Fkestados", "Importebaseimponible", "Tipoalbaran" };
            var propiedades = Helpers.Helper.getProperties<AlbaranesModel>();
            st.PrimaryColumnns = new[] { "Id" };
            st.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            st.ColumnasCombo.Add("Fkestados", estadosService.GetStates(DocumentoEstado.AlbaranesVentas,TipoMovimientos.Todos).Select(f => new Tuple<string, string>(f.CampoId, f.Descripcion)));
            st.ColumnasCombo.Add("Tipoalbaran", Enum.GetValues(typeof(TipoAlbaran)).OfType<TipoAlbaran>().Select(f => new Tuple<string, string>(((int)f).ToString(), Funciones.GetEnumByStringValueAttribute(f))));
            st.EstiloFilas.Add("Tipoalbaran", new EstiloFilas() { Estilos = new[] { new Tuple<object, string>(2, "#FCF8E3"), new Tuple<object, string>(3, "#F2DEDE") } });
            return st;
        }

        public override string GetSelectPrincipal()
        {
            return string.Format("{0}{1}", base.GetSelectPrincipal(), " and modo=" + (int)ModoAlbaran.Sinstock);
        }

        #endregion

        public IEnumerable<AlbaranesLinModel> RecalculaLineas(IEnumerable<AlbaranesLinModel> model,
            double descuentopp, double descuentocomercial, string fkregimeniva, double portes, int decimalesmoneda)
        {
            var result = new List<AlbaranesLinModel>();

            foreach (var item in model)
            {
                if (item.Fkregimeniva != fkregimeniva)
                {
                    var tiposivaService = FService.Instance.GetService(typeof(TiposIvaModel), _context) as TiposivaService;
                    var grupo = _db.Articulos.Single(f => f.empresa == Empresa && f.id == item.Fkarticulos);
                    if (!grupo.tipoivavariable)
                    {
                        var ivaObj = tiposivaService.GetTipoIva(grupo.fkgruposiva, fkregimeniva);
                        item.Fktiposiva = ivaObj.Id;
                        item.Porcentajeiva = ivaObj.PorcentajeIva;
                        item.Porcentajerecargoequivalencia = ivaObj.PorcentajeRecargoEquivalencia;
                        item.Fkregimeniva = fkregimeniva;
                    }
                    
                }

                result.Add(item);
            }

            return result;
        }

        public IEnumerable<AlbaranesTotalesModel> Recalculartotales(IEnumerable<AlbaranesLinModel> model, double descuentopp, double descuentocomercial, double portes, int decimalesmoneda)
        {
            var result = new List<AlbaranesTotalesModel>();

            var vector = model.GroupBy(f => f.Fktiposiva);

            foreach (var item in vector)
            {
                var newItem = new AlbaranesTotalesModel();
                var objIva = _db.TiposIva.Single(f => f.empresa == Empresa && f.id == item.Key);
                newItem.Decimalesmonedas = decimalesmoneda;
                newItem.Fktiposiva = item.Key;
                newItem.Porcentajeiva = objIva.porcentajeiva;
                newItem.Brutototal = Math.Round((item.Sum(f => f.Importe) - item.Sum(f => f.Importedescuento)) ?? 0, decimalesmoneda);
                newItem.Porcentajerecargoequivalencia = objIva.porcentajerecargoequivalente;
                newItem.Porcentajedescuentoprontopago = descuentopp;
                newItem.Porcentajedescuentocomercial = descuentocomercial;
                newItem.Importedescuentocomercial = Math.Round((double)((newItem.Brutototal * descuentocomercial ?? 0) / 100.0), decimalesmoneda);
                newItem.Importedescuentoprontopago = Math.Round((double)((double)(newItem.Brutototal - newItem.Importedescuentocomercial) * (descuentopp / 100.0)), decimalesmoneda);

                var baseimponible = (newItem.Brutototal ?? 0) - ((newItem.Importedescuentocomercial ?? 0) + (newItem.Importedescuentoprontopago ?? 0));
                newItem.Baseimponible = baseimponible;
                newItem.Cuotaiva = Math.Round(baseimponible * ((objIva.porcentajeiva ?? 0) / 100.0), decimalesmoneda);
                newItem.Importerecargoequivalencia = Math.Round(baseimponible * ((objIva.porcentajerecargoequivalente ?? 0) / 100.0), decimalesmoneda);
                newItem.Subtotal = Math.Round(baseimponible + (newItem.Cuotaiva ?? 0) + (newItem.Importerecargoequivalencia ?? 0), decimalesmoneda);
                result.Add(newItem);
            }

            return result;
        }

        public AlbaranesModel Clonar(string id)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var obj = _converterModel.CreateView(id) as AlbaranesModel;

                obj.Fechadocumento = DateTime.Now;
                obj.Fechavalidez = DateTime.Now.AddMonths(1);
                foreach (var AlbaranesLinModel in obj.Lineas)
                {
                    AlbaranesLinModel.Fkpedidos = null;
                    AlbaranesLinModel.Fkpedidosid = null;
                    AlbaranesLinModel.Fkpedidosreferencia = string.Empty;
                }
                foreach (var item in obj.Lineas)
                {
                    item.Cantidadpedida = 0;
                }
                obj.Fkestados = _appService.GetConfiguracion().Estadoalbaranesventasinicial;
                var contador = ServiceHelper.GetNextId<Albaranes>(_db, Empresa, obj.Fkseries);
                var identificadorsegmento = "";
                obj.Referencia = ServiceHelper.GetReference<Albaranes>(_db, obj.Empresa, obj.Fkseries, contador, obj.Fechadocumento.Value, out identificadorsegmento);
                obj.Identificadorsegmento = identificadorsegmento;
                var newItem = _converterModel.CreatePersitance(obj);
                if (_validationService.ValidarGrabar(newItem))
                {
                    AlbaranesModel result;
                    result = _converterModel.GetModelView(newItem) as AlbaranesModel;
                    //generar carpeta
                    DocumentosHelpers.GenerarCarpetaAsociada(result, TipoDocumentos.AlbaranesVentas, _context, _db);
                    newItem.fkcarpetas = result.Fkcarpetas;
                    _db.Set<Albaranes>().Add(newItem);
                    try
                    {
                        _db.SaveChanges();

                        tran.Complete();
                        result.Id = newItem.id;
                    }
                    catch (DbUpdateException ex)
                    {
                        if (ex.InnerException != null
                            && ex.InnerException.InnerException != null)
                        {
                            var inner = ex.InnerException.InnerException as SqlException;
                            if (inner != null)
                            {
                                if (inner.Number == 2627 || inner.Number == 2601)
                                {
                                    throw new ValidationException(General.ErrorRegistroExistente);
                                }
                            }
                        }


                        throw;
                    }

                    return result;
                }
            }

            throw new ValidationException(General.ErrorClonarGeneral);
        }

        public AlbaranesModel GetByReferencia(string referencia)
        {
            var obj =
                _db.Albaranes.Include("AlbaranesLin")
                    .Include("AlbaranesTotales")
                    .Single(f => f.empresa == Empresa && f.referencia == referencia);

            ((AlbaranesConverterService)_converterModel).Ejercicio = EjercicioId;
            return _converterModel.GetModelView(obj) as AlbaranesModel;
        }

        public override IModelView get(string id)
        {
            ((AlbaranesConverterService)_converterModel).Ejercicio = EjercicioId;
            return base.get(id);
        }

        #region Generar lineas

        public IEnumerable<ILineaImportar> GetLineasImportarAlbaran(string referencia)
        {
            var albaran = GetByReferencia(referencia);
            var agrupacionService = FService.Instance.GetService(typeof(CriteriosagrupacionModel), _context) as CriteriosagrupacionService;
            var criterio = agrupacionService.get(albaran.Fkcriteriosagrupacion) as CriteriosagrupacionModel;

            var serviceAgrupacion = FAgrupacionService.Instance.GetServicioAgrupacion(criterio);

            return serviceAgrupacion.GetLineasImportarAlbaran(this, _db, referencia);

        }

        #endregion

        #region Api

        public override void create(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as AlbaranesModel;
                var validation = _validationService as AlbaranesValidation;
                validation.EjercicioId = EjercicioId;

                //Calculo ID
                var contador = ServiceHelper.GetNextId<Albaranes>(_db, Empresa, model.Fkseries);
                var identificadorsegmento = "";
                model.Referencia = ServiceHelper.GetReference<Albaranes>(_db, model.Empresa, model.Fkseries, contador, model.Fechadocumento.Value, out identificadorsegmento);
                model.Identificadorsegmento = identificadorsegmento;

                //Obtener el pais del cliente (tercero)
                var cuentasService = new CuentasService(_context);
                var cuenta = cuentasService.get(model.Fkclientes) as CuentasModel;

                using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
                {
                    model.Clientepais = service.GetListPaises().Where(f => f.Valor == cuenta.FkPais).Select(f => f.Descripcion).SingleOrDefault(); 
                }

                var idprovincia = _db.Direcciones.Where(f => f.empresa == Empresa && f.fkentidad == model.Fkclientes && f.defecto == true).Select(f => f.fkprovincia).SingleOrDefault();
                model.Clienteprovincia = _db.Provincias.Where(f => f.codigopais == cuenta.FkPais && f.id == idprovincia).Select(f => f.nombre).SingleOrDefault();
                DocumentosHelpers.GenerarCarpetaAsociada(model, TipoDocumentos.AlbaranesVentas, _context, _db);

                //Actualizar precios si estos son = 0
                ApplicationHelper app = new ApplicationHelper(_context);

                if (app.GetListTiposAlbaranes().Where(f => f.EnumInterno == model.Tipoalbaran).Select(f => f.CosteAdq).SingleOrDefault())
                {
                    foreach (var l in model.Lineas)
                    {
                        if (l.Importe == null || l.Importe == 0)
                        {
                            var lotesService = new LotesService(_context);
                            l.Precio = Math.Round((double)_db.Stockhistorico.Where(f => f.empresa == _context.Empresa && f.lote == l.Lote && f.loteid == l.Tabla.ToString() && f.fkarticulos == l.Fkarticulos)
                                .Select(f => f.preciovaloracion + f.costeacicionalvariable / f.metrosentrada + f.costeadicionalmaterial / f.metrosentrada
                                + f.costeadicionalotro / f.metrosentrada + f.costeadicionalportes / f.metrosentrada).SingleOrDefault(), 2);
                            l.Importe = Math.Round((double)((decimal)l.Precio * (decimal)l.Metros), l.Decimalesmonedas ?? 2);
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(model.Fkpedidos))
                {
                    foreach (var lineaAlbaran in model.Lineas)
                    {                        
                        if (lineaAlbaran.Precio == 0)
                        {
                            var idPedido = _db.Pedidos.Where(f => f.empresa == model.Empresa && f.referencia == model.Fkpedidos).Select(f => f.id).SingleOrDefault();
                            var lineasPedido = _db.PedidosLin.Where(f => f.empresa == model.Empresa && f.fkpedidos == idPedido);

                            foreach (var lineaPedido in lineasPedido)
                            {
                                if (lineaAlbaran.Fkarticulos == lineaPedido.fkarticulos)
                                {
                                    lineaAlbaran.Precio = lineaPedido.precio;
                                    lineaAlbaran.Importe = Math.Round((double)((lineaAlbaran.Metros * lineaAlbaran.Precio) - (lineaAlbaran.Metros * lineaAlbaran.Precio *
                                        (lineaAlbaran.Porcentajedescuento / 100))), 2);
                                }
                            }
                        }
                    }
                }
                //fin actualizar precios

                base.create(obj);                
                
                ModificarCantidadesPedidasPedidos(obj as AlbaranesModel);
                ModificarMovimientosArticulos(obj as AlbaranesModel);

                _db.SaveChanges();
                tran.Complete();
            }

        }

        public override void edit(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var original = get(Funciones.Qnull(obj.get("id"))) as AlbaranesModel;
                var editado = obj as AlbaranesModel;

                if(editado.Estado.Tipoestado == TipoEstado.Anulado || editado.Estado.Tipoestado == TipoEstado.Caducado || editado.Estado.Tipoestado == TipoEstado.Finalizado)
                {
                    throw new ValidationException("No se pueden modificar documentos en estado finalizado");
                }

                else
                {
                    if (original.Integridadreferencial == editado.Integridadreferencial)
                    {
                        var validation = _validationService as AlbaranesValidation;
                        validation.EjercicioId = EjercicioId;
                        DocumentosHelpers.GenerarCarpetaAsociada(obj, TipoDocumentos.AlbaranesVentas, _context, _db);

                        //Obtener el pais del cliente (tercero)
                        var cuentasService = new CuentasService(_context);
                        var cuenta = cuentasService.get(editado.Fkclientes) as CuentasModel;

                        using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
                        {
                            editado.Clientepais = service.GetListPaises().Where(f => f.Valor == cuenta.FkPais).Select(f => f.Descripcion).SingleOrDefault();
                        }

                        base.edit(obj);

                        ModificarCantidadesPedidasPedidos(obj as AlbaranesModel);
                        _db.SaveChanges();
                        tran.Complete();
                    }
                    else throw new IntegridadReferencialException(string.Format(General.ErrorIntegridadReferencial, RAlbaranes.TituloEntidad, original.Referencia));
                } 
            }

        }

        public override void delete(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                base.delete(obj);
                _db.SaveChanges();
                ModificarCantidadesPedidasPedidos(obj as AlbaranesModel);
                tran.Complete();
            }

        }

        #endregion

        #region ModificarMovimientosArticulos

        private void ModificarMovimientosArticulos(AlbaranesModel model)
        {
            List<string> listaCodArticulos = new List<string>();

            foreach (var linea in model.Lineas)
            {
                if (string.IsNullOrEmpty(listaCodArticulos.Where(f => f.Contains(linea.Fkarticulos)).SingleOrDefault()))
                {
                    listaCodArticulos.Add(linea.Fkarticulos);
                }
            }

            var service = FService.Instance.GetService(typeof(ArticulosModel), _context) as ArticulosService;

            for (var i = 0; i < listaCodArticulos.Count; i++)
            {
                var articulo = service.GetArticulo(listaCodArticulos[i]);
                if (articulo.Fechaultimasalida < model.Fechadocumento || articulo.Fechaultimasalida == null)
                {
                    articulo.Ultimasalida = model.Referencia;
                    articulo.Fechaultimasalida = model.Fechadocumento;
                    service.edit(articulo);
                }
            }
        }

        #endregion    

        #region anterior y siguiente

        public ModoAlbaran Modo = ModoAlbaran.Sinstock;

        public override string LastRegister()
        {
            var keyNames = GetprimarykeyColumns();
            var enumerable = keyNames as string[] ?? keyNames.ToArray();
            var modoint = (int)Modo;
            var query = _db.Albaranes.Where(f => f.empresa == Empresa && f.modo == modoint);

            var flagFirst = true;
            IOrderedQueryable<Albaranes> orderedQuery = null;
            foreach (var item in enumerable)
            {

                orderedQuery = flagFirst ? query.OrderByDescending(item) : orderedQuery.ThenByDescending(item);
                flagFirst = false;

            }
            var obj = orderedQuery.FirstOrDefault();
            if (obj == null) return string.Empty;

            var modelObj = _converterModel.GetModelView(obj);
            return modelObj.GetPrimaryKey();
        }

        protected override string GetRegister(string id, TipoSelect tipo)
        {
            using (var con = new SqlConnection(_db.Database.Connection.ConnectionString))
            {
                var keyNames = GetprimarykeyColumns().ToArray();
                using (var cmd = new SqlCommand(GetSelect(keyNames, tipo), con))
                {
                    cmd.Parameters.AddWithValue("empresa", Empresa);
                    cmd.Parameters.AddWithValue("modo", (int)Modo);
                    var pkColumns = keyNames.Count(c => c != "empresa");
                    var pkvector = pkColumns > 1 ? id.Split(SeparatorPk) : new[] { id };
                    var j = 0;
                    foreach (var item in keyNames.Where(item => item != "empresa"))
                    {
                        cmd.Parameters.AddWithValue(item, pkvector[j++]);
                    }
                    var tabla = new DataTable();
                    using (var ad = new SqlDataAdapter(cmd))
                    {

                        ad.Fill(tabla);
                        if (tabla.Rows.Count > 0)
                        {
                            var vector = GetKeys(keyNames, tabla.Rows[0]);
                            var obj = _db.Set<Albaranes>().Find(vector);
                            return _converterModel.GetModelView(obj).GetPrimaryKey();
                        }
                    }
                }

            }

            return string.Empty;
        }

        protected override string GetSelect(string[] keyNames, TipoSelect tipo)
        {
            var sb = new StringBuilder();

            sb.AppendFormat("select top 1 * from Albaranes Where modo={0} ", (int)Modo);
            var flagFirst = false;
            for (var i = 0; i < keyNames.Count(); i++)
            {
                if (!flagFirst)
                    sb.Append(" AND ");

                if (i == keyNames.Count() - 1)
                    sb.AppendFormat("{0}{1}@{0}", keyNames[i], tipo == TipoSelect.Next ? ">" : "<");
                else
                    sb.AppendFormat("{0}=@{0}", keyNames[i]);
            }

            if (tipo == TipoSelect.Previous)
                sb.AppendFormat(" order by {0} desc", string.Join(",", keyNames));

            return sb.ToString();
        }

        public override string FirstRegister()
        {
            var modoint = (int)Modo;
            var obj = _db.Set<Albaranes>().FirstOrDefault(f => f.empresa == Empresa && f.modo == modoint);
            if (obj == null) return string.Empty;

            var modelObj = _converterModel.GetModelView(obj);
            return modelObj.GetPrimaryKey();
        }

        #endregion

        #region Importar Pedidos 

        public virtual AlbaranesModel ImportarPedido(PedidosModel presupuesto)
        {
            //calcular serie asociada
            if (presupuesto.Lineas.Any(f => (f.Cantidad ?? 0) - (f.Cantidadpedida ?? 0) > 0))
            {
                var result = Helper.fModel.GetModel<AlbaranesModel>(_context);
                result.Importado = true;
                ImportarCabecera(presupuesto, result);
                var maxId = result.Lineas.Any() ? result.Lineas.Max(f => f.Id) : 0;
                result.Lineas.AddRange(ImportarLineas(maxId, ConvertLineasModelToILineas(presupuesto.Id.ToString(), presupuesto.Referencia, presupuesto.Lineas)));
                EstablecerSerie(presupuesto.Fkseries, result);

                //recalculo importes lineas y totales
                RecalculaLineas(result.Lineas, result.Porcentajedescuentoprontopago ?? 0, result.Porcentajedescuentocomercial ?? 0, result.Fkregimeniva, result.Importeportes ?? 0, result.Decimalesmonedas);
                result.Totales = Recalculartotales(result.Lineas, result.Porcentajedescuentoprontopago ?? 0,
                    result.Porcentajedescuentocomercial ?? 0, result.Importeportes ?? 0,
                    result.Decimalesmonedas).ToList();

                return result;
            }

            throw new ValidationException(RAlbaranes.ErrorSinCantidadPendiente);
        }

        private IEnumerable<ILineaImportar> ConvertLineasModelToILineas
            
            (string idcabecera, string referencia, IEnumerable<PedidosLinModel> lineas)
        {
            var id = lineas.Any() ? lineas.Max(f => f.Id) : 0;
            id++;
            var articulosService = FService.Instance.GetService(typeof(ArticulosModel), _context, _db) as ArticulosService;
            var vectorArticulos = new Hashtable();
            var result = new List<LineaImportarModel>();
            foreach (var f in lineas)
            {
                ArticulosModel articulo;
                if (!vectorArticulos.ContainsKey(f.Fkarticulos))
                {
                    vectorArticulos.Add(f.Fkarticulos, articulosService.get(f.Fkarticulos) as ArticulosModel);
                }
                articulo = vectorArticulos[f.Fkarticulos] as ArticulosModel;

                if ((f.Cantidad ?? 0) - (f.Cantidadpedida ?? 0) != 0 || articulo.Articulocomentariovista)
                {
                    result.Add(new LineaImportarModel()
                    {
                        Id = id++,
                        Ancho = f.Ancho ?? 0,
                        Canal = f.Canal,
                        Cantidad = (f.Cantidad ?? 0) - (f.Cantidadpedida ?? 0),
                        Cuotaiva = f.Cuotaiva ?? 0,
                        Cuotarecargoequivalencia = f.Cuotarecargoequivalencia ?? 0,
                        Decimalesmedidas = f.Decimalesmedidas ?? 0,
                        Decimalesmonedas = f.Decimalesmonedas ?? 0,
                        Descripcion = f.Descripcion,
                        Fkregimeniva = f.Fkregimeniva,
                        Fkunidades = f.Fkunidades,
                        Metros = f.Metros ?? 0,
                        Precio = f.Precio ?? 0,
                        Fkarticulos = f.Fkarticulos,
                        Articulocomentario = articulo.Articulocomentariovista,
                        Fktiposiva = f.Fktiposiva,
                        Grueso = f.Grueso ?? 0,
                        Importe = f.Importe ?? 0,
                        Importedescuento = f.Importedescuento ?? 0,
                        Largo = f.Largo ?? 0,
                        Lote = f.Lote,
                        Notas = f.Notas,
                        Porcentajedescuento = f.Porcentajedescuento ?? 0,
                        Porcentajeiva = f.Porcentajeiva ?? 0,
                        Porcentajerecargoequivalencia = f.Porcentajerecargoequivalencia ?? 0,
                        Precioanterior = f.Precioanterior ?? 0,
                        Revision = f.Revision,
                        Tabla = f.Tabla ?? 0,
                        Fkdocumento = idcabecera,
                        Fkdocumentoid = f.Id.ToString(),
                        Fkdocumentoreferencia = referencia,
                        Orden = f.Orden
                    });
                }
            }
            return result;

        }

        protected void EstablecerSerie(string fkseries, AlbaranesModel result)
        {
            var service = FService.Instance.GetService(typeof(SeriesModel), _context);

            var serieObj = service.get(string.Format("{0}-{1}", SeriesService.GetSerieCodigo(TipoDocumento.PedidosVentas), fkseries)) as SeriesModel;
            var serieAsociada = serieObj.Fkseriesasociada;
            if (!string.IsNullOrEmpty(serieAsociada))
            {
                var serieasociadaObj = service.get(string.Format("{0}-{1}", SeriesService.GetSerieCodigo(TipoDocumento.AlbaranesVentas), serieAsociada)) as SeriesModel;
                result.Fkseries = serieasociadaObj.Id;
            }
            else
                throw new ValidationException(Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Presupuestos.ErrorNoExisteSerieAsociada);
        }

        public IEnumerable<AlbaranesLinModel> ImportarLineas(int maxId, IEnumerable<ILineaImportar> linea)
        {
            return linea.Where(f => f.Cantidad != 0 || f.Articulocomentario).Select(f => _importarService.ImportarLinea(f)).Select(f => ConvertILineaImportarToPedidoLinModel(++maxId, f));
        }

        private AlbaranesLinModel ConvertILineaImportarToPedidoLinModel(int Id, ILineaImportar linea)
        {
            var result = _importarService.ImportarLinea(linea);
            return new AlbaranesLinModel()
            {
                Cantidadpedida = 0,
                Id = Id,
                Ancho = result.Ancho,
                Canal = result.Canal,
                Cantidad = result.Cantidad,
                Cuotaiva = result.Cuotaiva,
                Cuotarecargoequivalencia = result.Cuotarecargoequivalencia,
                Decimalesmedidas = result.Decimalesmedidas,
                Decimalesmonedas = result.Decimalesmonedas,
                Descripcion = result.Descripcion,
                Fkregimeniva = result.Fkregimeniva,
                Fkunidades = result.Fkunidades,
                Metros = result.Metros,
                Precio = result.Precio,
                Fkarticulos = result.Fkarticulos,
                Fktiposiva = result.Fktiposiva,
                Grueso = result.Grueso,
                Importe = result.Importe,
                Importedescuento = result.Importedescuento,
                Largo = result.Largo,
                Lote = result.Lote,
                Notas = result.Notas,
                Porcentajedescuento = result.Porcentajedescuento,
                Porcentajeiva = result.Porcentajeiva,
                Porcentajerecargoequivalencia = result.Porcentajerecargoequivalencia,
                Precioanterior = result.Precioanterior,
                Revision = result.Revision,
                Tabla = result.Tabla,
                Fkpedidos = Funciones.Qint(result.Fkdocumento),
                Fkpedidosid = Funciones.Qint(result.Fkdocumentoid),
                Fkpedidosreferencia = result.Fkdocumentoreferencia,
                Orden = result.Orden ?? 0
            };

        }


        protected void ImportarCabecera(PedidosModel presupuesto, AlbaranesModel result)
        {
            var properties = typeof(AlbaranesModel).GetProperties();

            foreach (var item in properties)
            {
                if (item.Name != "Lineas" && item.Name != "Totales")
                {
                    var property = typeof(PedidosModel).GetProperty(item.Name);
                    if (property != null && property.CanWrite)
                    {

                        var value = property.GetValue(presupuesto);
                        item.SetValue(result, value);
                    }
                }
            }

            result.Fechadocumento = DateTime.Now;
            result.Fkestados = _appService.GetConfiguracion().Estadoalbaranesventasinicial;
        }

        public IEnumerable<AlbaranesModel> BuscarAlbaranesImportar(string cliente, string albaraninicio, string albaranfin)
        {
            int? fkalbaraninicio = null;
            int? fkalbaranfin = null;

            if (!string.IsNullOrEmpty(albaraninicio))
            {
                var objalbaran = _db.Albaranes.Single(f => f.empresa == Empresa && f.referencia == albaraninicio);
                fkalbaraninicio = objalbaran.id;

            }

            if (!string.IsNullOrEmpty(albaranfin))
            {
                var objalbaran = _db.Albaranes.Single(f => f.empresa == Empresa && f.referencia == albaranfin);
                fkalbaranfin = objalbaran.id;

            }

            var list = _db.Albaranes.Where(
                      f =>
                          f.empresa == Empresa && f.fkclientes == cliente &&
                          (!fkalbaraninicio.HasValue || f.id >= fkalbaraninicio.Value) &&
                          (!fkalbaranfin.HasValue || f.id <= fkalbaranfin.Value) &&
                          !_db.FacturasLin.Any(j => j.fkalbaranesreferencia == f.referencia)).ToList().Select(f => _converterModel.GetModelView(f) as AlbaranesModel).ToList();

            list = list.Join(_db.Estados, p => p.Fkestados, e => e.documento + "-" + e.id, (a, b) => new { a, b }).ToList().Where(f => f.b.tipoestado <= (int)TipoEstado.Curso).Select(f => f.a).OrderByDescending(f => f.Id).ToList();

            var filterItems = list.Where(f => !string.IsNullOrEmpty(f.Fkobras));
            foreach (var item in filterItems)
            {

                var obraInt = Funciones.Qint(item.Fkobras);
                if (obraInt.HasValue)
                {
                    var obraobj = _db.Obras.SingleOrDefault(f => f.empresa == Empresa && f.id == obraInt);
                    item.Obradescripcion = obraobj?.nombreobra;
                }

            }

            return list;




        }

        #endregion

        #region Helper

        private void ModificarCantidadesPedidasPedidos(AlbaranesModel model)
        {
            var PedidosService = new PedidosService(_context,_db);
            PedidosService.EjercicioId = EjercicioId;
            var vector = model.Lineas.Where(f => f.Fkpedidos.HasValue);

            foreach (var item in vector)
            {
                var pedido = _db.Pedidos.Include("PedidosLin").SingleOrDefault(
                    f =>
                        f.empresa == model.Empresa && f.id == item.Fkpedidos.Value);
                if (pedido != null)
                {
                    var cantidadpedida = _db.AlbaranesLin.Where(f => f.empresa == model.Empresa && f.fkpedidos == item.Fkpedidos &&
                        f.fkpedidosid == item.Fkpedidosid).Sum(f => f.cantidad);

                    var linea = pedido.PedidosLin.SingleOrDefault(f => f.id == item.Fkpedidosid);
                    if (linea != null)
                    {
                        linea.cantidadpedida = cantidadpedida;
                        var validationService = PedidosService._validationService as PedidosValidation;
                        validationService.EjercicioId = EjercicioId;
                        validationService.FlagActualizarCantidadesPedidas = true;
                        validationService.ValidarGrabar(pedido);
                        _db.Pedidos.AddOrUpdate(pedido);
                    }
                }



            }
            _db.SaveChanges();
        }

        #endregion

        public void SetEstado(IModelView model, EstadosModel nuevoEstado)
        {
            var currentValidationService = _validationService as AlbaranesValidation;
            currentValidationService.CambiarEstado = true;
            model.set("fkestados", nuevoEstado.CampoId);
            edit(model);
            currentValidationService.CambiarEstado = false;
        }

        #region Buscar documento

        public virtual IEnumerable<DocumentosBusqueda> Buscar(IDocumentosFiltros filtros, out int registrostotales)
        {
            var service = new BuscarDocumentosAlbaranesService(_db, Empresa);
            return service.Buscar(filtros, out registrostotales);
        }

        public virtual IEnumerable<IItemResultadoMovile> BuscarDocumento(string referencia)
        {
            var service = new BuscarDocumentosService(_db, Empresa);
            return service.Get<AlbaranesModel, AlbaranesLinModel, AlbaranesTotalesModel>(this, referencia);
        }

        public bool ExisteReferencia(string referencia)
        {
            return _db.Albaranes.Any(f => f.empresa == _context.Empresa && f.referencia == referencia);
        }

        #endregion

        #region Crear lineas entrega de stock

        public List<AlbaranesLinModel> CrearNuevasLineas(List<AlbaranesLinModel> listado, AlbaranesLinVistaModel model)
        {

            var articulosService = FService.Instance.GetService(typeof(ArticulosModel), _context, _db) as ArticulosService;



            var maxId = listado.Any() ? listado.Max(f => f.Id) + 1 : 1;
            var articuloObj = articulosService.GetArticulo(model.Fkarticulos, model.Fkcuenta, model.Fkmonedas, model.Fkregimeniva, model.Flujo);
            if (articuloObj!=null && articuloObj.Tipogestionlotes == Tipogestionlotes.Singestion)
            {
                return GenerarLineasSinStock(listado, model, articuloObj, maxId);
            }

            return GenerarLineasConStock(listado, model, articuloObj, maxId);
        }

        public List<AlbaranesLinModel> GenerarLineasSinStock(List<AlbaranesLinModel> listado, AlbaranesLinVistaModel model, ArticulosDocumentosModel articuloObj, int maxId)
        {
            var familiasService = FService.Instance.GetService(typeof(FamiliasproductosModel), _context, _db) as FamiliasproductosService;
            var unidadesService = FService.Instance.GetService(typeof(UnidadesModel), _context, _db) as UnidadesService;
            var tiposivaService = FService.Instance.GetService(typeof(TiposIvaModel), _context, _db) as TiposivaService;
            var monedasService = FService.Instance.GetService(typeof(MonedasModel), _context, _db) as MonedasService;
            var monedasObj = monedasService.get(model.Fkmonedas) as MonedasModel;
            var familiaObj = familiasService.get(ArticulosService.GetCodigoFamilia(model.Fkarticulos)) as FamiliasproductosModel;

            var ancho = model.Ancho;
            var largo = model.Largo;
            var grueso = model.Grueso;
            if (model.Modificarmedidas)
            {
                ancho = model.Ancho;
                largo = model.Largo;
                grueso = model.Grueso;
            }
            else
            {
                ancho = articuloObj.Ancho.Value;
                largo = articuloObj.Largo.Value;
                grueso = articuloObj.Grueso.Value;
            }

            var unidadesObj = unidadesService.get(familiaObj.Fkunidadesmedida) as UnidadesModel;
            var tiposivaObj = tiposivaService.get(articuloObj.Fktiposiva) as TiposIvaModel;
            var metros = UnidadesService.CalculaResultado(unidadesObj, model.Cantidad, largo, ancho, grueso, model.Metros);
            model.Metros = metros;
            var bruto = model.Metros * model.Precio;
            var importedescuento = Math.Round(((bruto) * model.Descuento / 100.0), model.Decimalesmonedas);
            var total = bruto - importedescuento;

            listado.Add(new AlbaranesLinModel()
            {
                Nueva = true,
                Id = maxId++,
                Fkarticulos = model.Fkarticulos,
                Descripcion = articuloObj.Descripcion,
                Cantidad = model.Cantidad,
                Largo = largo,
                Ancho = ancho,
                Grueso = grueso,
                Fkunidades = articuloObj.Fkunidades,
                Metros = metros,
                Precio = model.Precio,
                Porcentajedescuento = model.Descuento,
                Importedescuento = importedescuento,
                Importe = total,
                Decimalesmedidas = unidadesObj.Decimalestotales,
                Decimalesmonedas = monedasObj.Decimales,
                Fktiposiva = tiposivaObj.Id,
                Porcentajeiva = tiposivaObj.PorcentajeIva,
                Porcentajerecargoequivalencia = tiposivaObj.PorcentajeRecargoEquivalencia,
                Canal = model.Canal

            }
             );




            return listado;
        }

        private List<AlbaranesLinModel> GenerarLineasConStock(List<AlbaranesLinModel> listado, AlbaranesLinVistaModel model, ArticulosDocumentosModel articuloObj, int maxId)
        {
            var stockactualService = new StockactualService(_context,_db);
            var articulosService = FService.Instance.GetService(typeof(ArticulosModel), _context, _db) as ArticulosService;
            var familiasService = FService.Instance.GetService(typeof(FamiliasproductosModel), _context, _db) as FamiliasproductosService;
            var unidadesService = FService.Instance.GetService(typeof(UnidadesModel), _context, _db) as UnidadesService;
            var tiposivaService = FService.Instance.GetService(typeof(TiposIvaModel), _context, _db) as TiposivaService;
            var monedasService = FService.Instance.GetService(typeof(MonedasModel), _context, _db) as MonedasService;
            var monedasObj = monedasService.get(model.Fkmonedas) as MonedasModel;

            foreach (var linea in model.Lineas)
            {
                var articulosObj = articulosService.get(linea.Fkarticulos) as ArticulosModel;

                //Rai -- Puede entrar o bien porque sea una losa por ejemplo, o bien porque no coincide con lote y tabla de las lineas que ya tienes
                if (!listado.Any(f => f.Lote == linea.Lote && f.Tabla == Funciones.Qint(linea.Loteid)) || articulosObj.Tipofamilia != (int)TipoFamilia.Tabla)
                {


                    articuloObj= articulosService.GetArticulo(linea.Fkarticulos, model.Fkcuenta, model.Fkmonedas, model.Fkregimeniva, model.Flujo);
                    var familiaObj = familiasService.get(ArticulosService.GetCodigoFamilia(linea.Fkarticulos)) as FamiliasproductosModel;

                    var ancho = linea.Ancho;
                    var largo = linea.Largo;
                    var grueso = linea.Grueso;
                    var item = familiaObj.Gestionstock
                        ? stockactualService.GetArticuloPorLoteOCodigo(
                            string.Format("{0}{1}", linea.Lote, Funciones.RellenaCod(linea.Loteid, 3)), model.Fkalmacen,
                            Empresa) as MovimientosstockModel : null;
                    if (model.Modificarmedidas)
                    {
                        ancho = model.Ancho;
                        largo = model.Largo;
                        grueso = model.Grueso;
                    }
                    else
                    {
                        
                        ancho = item?.Ancho ?? linea.Ancho;
                        largo = item?.Largo ?? linea.Largo;
                        grueso = item?.Grueso ?? linea.Grueso;
                    }
                    if(linea.Cantidad>item.Cantidad)
                        throw new ValidationException(string.Format("La cantidad indicada para el lote {0} es superior a la que hay en el stock actual", string.Format("{0}{1}", linea.Lote, Funciones.RellenaCod(linea.Loteid, 3))));
                    var unidadesObj = unidadesService.get(familiaObj.Fkunidadesmedida) as UnidadesModel;
                    var tiposivaObj = tiposivaService.get(articuloObj.Fktiposiva) as TiposIvaModel;
                    var metros = UnidadesService.CalculaResultado(unidadesObj, articuloObj.Lotefraccionable ? model.Cantidad :linea.Cantidad, largo, ancho, grueso, model.Metros);
                    linea.Metros = metros;
                    var bruto = linea.Metros * model.Precio;
                    var importedescuento = Math.Round(((bruto) * model.Descuento / 100.0), model.Decimalesmonedas);
                    var total = bruto - importedescuento;

                    listado.Add(new AlbaranesLinModel()
                    {
                        Nueva = true,
                        Id = maxId++,
                        Fkarticulos = linea.Fkarticulos,
                        Descripcion = articuloObj.Descripcion,
                        Lote = linea.Lote,
                        Tabla = Funciones.Qint(linea.Loteid),
                        Tblnum = Funciones.Qint(linea.Loteid),
                        Cantidad = articuloObj.Lotefraccionable ? model.Cantidad : linea.Cantidad,
                        Largo = largo,
                        Ancho = ancho,
                        Grueso = grueso,
                        Fkunidades = articuloObj.Fkunidades,
                        Metros = metros,
                        Precio = model.Precio,
                        Porcentajedescuento = model.Descuento,
                        Importedescuento = importedescuento,
                        Importe = total,
                        Decimalesmedidas = unidadesObj.Decimalestotales,
                        Decimalesmonedas = monedasObj.Decimales,
                        Fktiposiva = tiposivaObj.Id,
                        Porcentajeiva = tiposivaObj.PorcentajeIva,
                        Porcentajerecargoequivalencia = tiposivaObj.PorcentajeRecargoEquivalencia,
                        Bundle = model.Tipopieza == TipoPieza.Bundle ? model.Lote.Replace(linea.Lote, string.Empty) : string.Empty,
                        Caja = model.Caja,
                        Canal = model.Canal,
                        Flagidentifier = Guid.NewGuid()

                    }
                     );
                }

            }

            ValidarKit(listado, model);

            return listado;
        }

        private void ValidarKit(List<AlbaranesLinModel> listado, AlbaranesLinVistaModel model)
        {
            var serviceKit = FService.Instance.GetService(typeof(KitModel), _context, _db);
            if (serviceKit.exists(model.Lote))
            {
                var kitobj = serviceKit.get(model.Lote) as KitModel;

                foreach (var item in kitobj.Lineas)
                {
                    if (!listado.Any(f => item.Lote == f.Lote && Funciones.Qint(item.Loteid) == f.Tabla))
                    {
                        throw new ValidationException(string.Format("El Kit {0} no está completo, falta añadir el lote {1}{2}", model.Lote, item.Lote, Funciones.RellenaCod(item.Loteid, 3)));
                    }
                }
            }

        }

        public double cantidadDisponible(string fkarticulo)
        {
            return _db.Stockactual.Where(f => f.empresa == Empresa && f.fkarticulos == fkarticulo).Select(f => f.cantidaddisponible).Sum();
        }

        #endregion
    }
}
