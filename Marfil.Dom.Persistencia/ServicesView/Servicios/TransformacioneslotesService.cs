using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using DevExpress.Data.Helpers;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Documentos.Albaranes;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Dom.Persistencia.Model.Documentos.Transformacioneslotes;
using Marfil.Dom.Persistencia.Model.Documentos.Pedidos;
using Marfil.Dom.Persistencia.Model.Documentos.PedidosCompras;
using Marfil.Dom.Persistencia.Model.Documentos.Presupuestos;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Converter;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos.Importar;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Stock;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Validation;
using Marfil.Inf.Genericos;
using Marfil.Inf.Genericos.Helper;
using Resources;


using RTransformacioneslotes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Transformaciones;
using RAlbaranesCompras = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.AlbaranesCompras;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface ITransformacioneslotesService
    {

    }

    public  class TransformacioneslotesService : GestionService<TransformacioneslotesModel, Transformacioneslotes>, IDocumentosServices, ITransformacioneslotesService
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
                ((TransformacioneslotesValidation)_validationService).EjercicioId = value;
            }
        }

        #endregion

        #region CTR

        public TransformacioneslotesService(IContextService context, MarfilEntities db = null) : base(context, db)
        {
            _importarService = new ImportacionService(context);
        }

        #endregion

        #region List index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var estadosService = new EstadosService(_context, _db);
            var st = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            st.List = st.List.OfType<TransformacioneslotesModel>().OrderByDescending(f => f.Referencia);
            var propiedadesVisibles = new[] { "Referencia", "Fechadocumento", "Fkproveedores", "Nombreproveedor","Trabajosdescripcion", "Fkestados" };
            var propiedades = Helper.getProperties<TransformacioneslotesModel>();
            st.PrimaryColumnns = new[] { "Id" };
            st.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            st.ColumnasCombo.Add("Fkestados", estadosService.GetStates(DocumentoEstado.Transformacioneslotes, TipoMovimientos.Todos).Select(f => new Tuple<string, string>(f.CampoId, f.Descripcion)));
            return st;
        }

        public override string GetSelectPrincipal()
        {
            return string.Format("select tl.*,t.descripcion as [Trabajosdescripcion] from Transformacioneslotes as tl inner join trabajos as t on t.empresa= tl.empresa and t.id=tl.fktrabajos where tl.empresa='{0}'", Empresa );
        }

        #endregion
        

        public TransformacioneslotesModel GetByReferencia(string referencia)
        {
            var obj = _db.Transformacioneslotes.Single(f => f.empresa == Empresa && f.referencia == referencia);
            return _converterModel.GetModelView(obj) as TransformacioneslotesModel;
        }

        public bool ExisteReferencia(string referencia)
        {
            return _db.Transformacioneslotes.Any(f => f.empresa == _context.Empresa && f.referencia == referencia);
        }

        #region Api main

        public TransformacioneslotesModel Clonar(string id)
        {
            throw new NotImplementedException();
        }

        //Rai
        public bool ValidarAcabadoInicial(TransformacioneslotesModel model)
        {
            var trabajosService = FService.Instance.GetService(typeof(TrabajosModel), _context) as TrabajosService;
            var trabajosObj = trabajosService.get(model.Fktrabajos) as TrabajosModel;

            bool acabados = true;
            var articulosService = FService.Instance.GetService(typeof(ArticulosModel), _context) as ArticulosService;

            foreach (var linea in model.Lineas) {
                var articulosObj = articulosService.get(linea.Fkarticulos) as ArticulosModel;

                if (linea.Fkarticulos.Substring(9, 2) != trabajosObj.Fkacabadoinicial)
                {
                    acabados = false;
                }

            }

            return acabados;
        } 

        public override void create(IModelView obj)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as TransformacioneslotesModel;

                if(ValidarAcabadoInicial(model))
                {
                    // Calcular costesadicionales costexm2 o costexm3

                    //DocumentosHelpers.GenerarCarpetaAsociada(model, TipoDocumentos.TransformacionesAcabados, _context, _db);

                    CalcularCosteTotalMetros(model.Lineas, model.Costes);

                    var validation = _validationService as TransformacioneslotesValidation;
                    validation.EjercicioId = EjercicioId;
                    RepartirCostesLineas(model.Lineas, model.Costes);
                    //Calculo ID
                    var contador = ServiceHelper.GetNextId<Transformacioneslotes>(_db, Empresa, model.Fkseries);
                    var identificadorsegmento = "";
                    model.Referencia = ServiceHelper.GetReference<Transformacioneslotes>(_db, model.Empresa, model.Fkseries, contador, model.Fechadocumento.Value, out identificadorsegmento);
                    model.Identificadorsegmento = identificadorsegmento;
                    //CalcularPrecioPiezas(model.Lineas, model.Lineas.Sum(f => f.Precio * f.Metros) ?? 0);

                    foreach (var item in model.Lineas)
                    {
                        item.Precio = Math.Round((double)(item.Precio * item.Metros), 2);
                    }

                    base.create(obj);
                    var trobj = _db.Transformacioneslotes.Single(f => f.empresa == model.Empresa && f.referencia == model.Referencia);
                    model.Id = trobj.id;
                    ModificarArticuloLote(model);

                    _db.SaveChanges();
                    tran.Complete();
                }
                
                else
                {
                    throw new ValidationException("El acabado de los artículos debe ser el inicial del trabajo seleccionado");
                }
            }
        }

        public override void edit(IModelView obj)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                var original = get(Funciones.Qnull(obj.get("id"))) as TransformacioneslotesModel;
                var editado = obj as TransformacioneslotesModel;

                if (ValidarAcabadoInicial(editado))
                {
                    if (original.Integridadreferencialflag == editado.Integridadreferencialflag)
                    {
                        var validation = _validationService as TransformacioneslotesValidation;
                        validation.EjercicioId = EjercicioId;

                        DocumentosHelpers.GenerarCarpetaAsociada(editado, TipoDocumentos.TransformacionesAcabados, _context, _db);

                        // Calcular costesadicionales costexm2 o costexm3
                        CalcularCosteTotalMetros(editado.Lineas, editado.Costes);

                        RepartirCostesLineas(editado.Lineas, editado.Costes, original.Costes);

                        base.edit(obj);
                        var trabajosService = FService.Instance.GetService(typeof(TrabajosModel), _context) as TrabajosService;
                        var trabajosObj = trabajosService.get(editado.Fktrabajos) as TrabajosModel;

                        var materialesObj = new MaterialesModel();

                        if (!String.IsNullOrEmpty(editado.Fkmateriales))
                        {
                            var materialesService = FService.Instance.GetService(typeof(MaterialesModel), _context) as MaterialesService;
                            materialesObj = materialesService.get(editado.Fkmateriales) as MaterialesModel;
                        }

                        //ActualizarStock(original,editado, trabajosObj);
                        //GenerarMovimientosLineas(original.Lineas, original, TipoOperacionService.ActualizarTransformacionloteStock, trabajosObj);
                        //GenerarMovimientosLineas(editado.Lineas, editado, TipoOperacionService.InsertarTransformacionloteStock, trabajosObj);
                        GenerarMovimientosLineas(original.Lineas, original, TipoOperacionService.ActualizarTransformacionloteStock, trabajosObj, materialesObj, false);
                        GenerarMovimientosLineas(editado.Lineas, editado, TipoOperacionService.InsertarTransformacionloteStock, trabajosObj, materialesObj, false);
                        _db.SaveChanges();
                        tran.Complete();
                    }
                    else throw new IntegridadReferencialException(string.Format(General.ErrorIntegridadReferencial, RTransformacioneslotes.TituloEntidad, original.Referencia));

                }

                else
                {
                    throw new ValidationException("El acabado de los artículos debe ser el inicial del trabajo seleccionado");
                }
            }
        }


        public void CalcularCosteTotalMetros(List<TransformacioneslotesLinModel> lineas, List<TransformacioneslotesCostesadicionalesModel> costes)
        {
            foreach (var i in costes)
            {
                var tipoDocumento = i.Tipodocumento;
                var codUnidadMedida = "-1";

                if (tipoDocumento == TipoCosteAdicional.Costexm2)
                {
                    codUnidadMedida = "02";
                }
                else if (tipoDocumento == TipoCosteAdicional.Costexm3)
                {
                    codUnidadMedida = "03";
                }

                if (tipoDocumento == TipoCosteAdicional.Costexm2 || tipoDocumento == TipoCosteAdicional.Costexm3)
                {

                    var totalMetros = 0.0d;
                    foreach (var j in lineas)
                    {
                        // Comprobar unidad de medida
                        var unidadMedida = _db.Articulos.Where(f => f.id == j.Fkarticulos).Select(f => j.Fkunidades).SingleOrDefault();
                        if (unidadMedida.Equals(codUnidadMedida))
                        {
                            totalMetros += Math.Round((double)j.Metros, 3);
                        }
                    }

                    i.Total = Math.Round((double)i.Importe * totalMetros, 2);
                }
            }
        }

        #region Modificar codigo articulo del lote

        private void ModificarArticuloLote(TransformacioneslotesModel editado)
        {
            var trabajosService = FService.Instance.GetService(typeof (TrabajosModel), _context) as TrabajosService;
            var trabajosObj = trabajosService.get(editado.Fktrabajos) as TrabajosModel;
            var materialesObj = new MaterialesModel();

            if (!String.IsNullOrEmpty(editado.Fkmateriales))
            {
                var materialesService = FService.Instance.GetService(typeof(MaterialesModel), _context) as MaterialesService;
                materialesObj = materialesService.get(editado.Fkmateriales) as MaterialesModel;
            }

            if (trabajosObj.Fkacabadoinicial==trabajosObj.Fkacabadofinal && !editado.Costes.Any())
                throw new Exception("No se han añadido costes adicionales. El parte no se grabará al no haber transformación.");

            //CrearStock(editado, trabajosObj);
            GenerarMovimientosLineas(editado.Lineas, editado, TipoOperacionService.InsertarTransformacionloteStock, trabajosObj, materialesObj);
        }



        #endregion

        public override void delete(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                base.delete(obj);
                var model = obj as TransformacioneslotesModel;
                //foreach (var item in model.Lineas)
                //    item.Cantidad *= -1;
                
                EliminarStock(obj as TransformacioneslotesModel);
                tran.Complete();
            }
        }

        #endregion

        #region Movimientos stock

        private void EliminarStock(TransformacioneslotesModel nuevo)
        {
            var trabajosService = FService.Instance.GetService(typeof(TrabajosModel), _context) as TrabajosService;
            var trabajosObj = trabajosService.get(nuevo.Fktrabajos) as TrabajosModel;
            var materialesObj = new MaterialesModel();

            if(!String.IsNullOrEmpty(nuevo.Fkmateriales))
            {
                var materialesService = FService.Instance.GetService(typeof(MaterialesModel), _context) as MaterialesService;
                materialesObj = materialesService.get(nuevo.Fkmateriales) as MaterialesModel;
            }

            if (trabajosObj.Fkacabadoinicial == trabajosObj.Fkacabadofinal && !nuevo.Costes.Any())
                throw new Exception("No se han añadido costes adicionales. El parte no se grabará al no haber transformación.");

            GenerarMovimientosLineas(nuevo.Lineas, nuevo, TipoOperacionService.ActualizarTransformacionloteStock, trabajosObj, materialesObj, true);
        }

        private void FinalizarStock(TransformacioneslotesModel original, TransformacioneslotesModel nuevo, TrabajosModel trabajosObj, MaterialesModel materialesObj)
        {
            GenerarMovimientosLineas(nuevo.Lineas, nuevo, TipoOperacionService.FinalizarTransformacionloteStock, trabajosObj, materialesObj, true);
        }

        private void ActualizarStock(TransformacioneslotesModel original, TransformacioneslotesModel nuevo, TrabajosModel trabajosObj, MaterialesModel materialesObj)
        {
            var list = new List<TransformacioneslotesLinModel>();
            var lineasModificadas = nuevo.Lineas.Where(f => !original.Lineas.Any(j => j.Flagidentifier == f.Flagidentifier)).ToList();
            

            var lineasEliminadas = original.Lineas.Where(f => !nuevo.Lineas.Where(j => !lineasModificadas.Any(h => h.Flagidentifier == j.Flagidentifier)).Any(j => j.Flagidentifier == f.Flagidentifier)).ToList();
            foreach (var item in lineasEliminadas)
                item.Cantidad *= -1;

            list = lineasEliminadas.Union(lineasModificadas).ToList();
            GenerarMovimientosLineas(list, nuevo, TipoOperacionService.ActualizarTransformacionloteStock, trabajosObj, materialesObj);
        }


        private void GenerarMovimientosLineas(IEnumerable<TransformacioneslotesLinModel> lineas, TransformacioneslotesModel nuevo, TipoOperacionService movimiento, TrabajosModel trabajosObj, MaterialesModel materialesObj, bool finalizarstock=false)
        {
            var movimientosStockService = new MovimientosstockService(_context, _db);
            var articulosService = FService.Instance.GetService(typeof (ArticulosModel), _context, _db);
            var serializer = new Serializer<TransformacioneslotesDiarioStockSerializable>();
            var vectorArticulos = new Hashtable();

            var operacion = 1;
            if (movimiento == TipoOperacionService.InsertarTransformacionloteStock)
                operacion = -1;

            //Rai
            // Ordenamos lineas por articulo y cantidad para procesar primero las positivas evitando error stock negativo no autorizado
            // (ya que se procesan las lineas antes existentes con cantidad negativa para quitarlas
            //   y luego se añade las lineas de grid cantidad +)
            var mylineas = lineas.OrderBy(x => x.Fkarticulos).ThenByDescending(x => x.Cantidad).ToList();

            foreach (var linea in mylineas)
            {
                //Rai -- sustituye el codigo del acabado y el codigo del material en caso de que sea necesario
                var codigoarticulonuevo = linea.Fkarticulos;
                var acabado = !string.IsNullOrEmpty(trabajosObj.Fkacabadofinal) ? trabajosObj.Fkacabadofinal : ArticulosService.GetCodigoAcabado(linea.Fkarticulos);
                var material = !string.IsNullOrEmpty(materialesObj.Id) ? materialesObj.Id : ArticulosService.GetCodigoMaterial(linea.Fkarticulos);

                codigoarticulonuevo = string.Format("{0}{1}{2}{3}{4}", ArticulosService.GetCodigoFamilia(linea.Fkarticulos),
                        material, ArticulosService.GetCodigoCaracteristica(linea.Fkarticulos),
                        ArticulosService.GetCodigoGrosor(linea.Fkarticulos), acabado);
                
                if (!articulosService.exists(codigoarticulonuevo))
                    throw new Exception(string.Format("El articulo {0} no existe", codigoarticulonuevo));

                ArticulosModel articuloObj;
                if (vectorArticulos.ContainsKey(linea.Fkarticulos))
                    articuloObj = vectorArticulos[linea.Fkarticulos] as ArticulosModel;
                else
                {
                    articuloObj = articulosService.get(linea.Fkarticulos) as ArticulosModel;
                    vectorArticulos.Add(linea.Fkarticulos, articuloObj);
                }

                var aux = Funciones.ConverterGeneric<TransformacionesloteslinSerialized>(linea);

                if (articuloObj?.Gestionstock ?? false)
                {
                    var model = new MovimientosstockModel
                    {
                        Empresa = nuevo.Empresa,
                        Fkalmacenes = nuevo.Fkalmacen,
                        Fkalmaceneszona = Funciones.Qint(nuevo.Fkzonas),
                        Fkarticulos = codigoarticulonuevo, //linea.Fkarticulos
                        Referenciaproveedor = "",
                        Fkcontadorlote = linea.Fkcontadoreslotes,
                        Lote = linea.Lote,
                        Loteid = (linea.Tabla ?? 0).ToString(),
                        Tag = "",
                        Fkunidadesmedida = linea.Fkunidades,                        
                        Largo = linea.Largo ?? 0,
                        Ancho = linea.Ancho ?? 0,
                        Grueso = linea.Grueso ?? 0,                        
                        Documentomovimiento = serializer.GetXml(
                            new TransformacioneslotesDiarioStockSerializable
                            {
                                Id = nuevo.Id.Value,
                                Referencia = nuevo.Referencia,
                                Fechadocumento = nuevo.Fechadocumento,
                                Codigoproveedor = nuevo.Fkproveedores,
                                Fkarticulosnuevo = codigoarticulonuevo,
                                Linea = aux
                            }),
                        Fkusuarios = Usuarioid,
                        //Tipooperacion = operacion,

                        Cantidad = (linea.Cantidad ?? 0) * operacion,
                        Metros = (linea.Metros ?? 0) * operacion,
                        Costeadicionalmaterial = linea.Costeadicionalmaterial,
                        Costeadicionalotro = linea.Costeadicionalotro,
                        Costeadicionalvariable = linea.Costeadicionalvariable,
                        Costeadicionalportes = linea.Costeadicionalportes,

                        Tipomovimiento = movimiento
                    };

                    var tipooperacion = finalizarstock
                        ? TipoOperacionService.FinalizarTransformacionloteStock
                        : linea.Nueva
                            ? TipoOperacionService.InsertarTransformacionloteStock
                            : TipoOperacionService.ActualizarTransformacionloteStock;
                    var currentValidationService = _validationService as TransformacioneslotesValidation;


                    if (currentValidationService.ModificarCostes)
                    {
                        tipooperacion = TipoOperacionService.ActualizarcosteTransformacionloteStock;
                    }


                    if (tipooperacion == TipoOperacionService.InsertarTransformacionloteStock)
                    {
                        model.Cantidad = -model.Cantidad;
                    }
                    movimientosStockService.GenerarMovimiento(model, tipooperacion);

                }

            }
        }

        #endregion

        #region Helper

        #region Reparto de costes

        private void RepartirCostesLineas(List<TransformacioneslotesLinModel> lineas, List<TransformacioneslotesCostesadicionalesModel> costes, List<TransformacioneslotesCostesadicionalesModel> costesOriginal = null)
        {
           foreach (var item in lineas)
            {
                item.Costeadicionalmaterial = 0;
                item.Costeadicionalotro = 0;
                item.Costeadicionalportes = 0;
                item.Costeadicionalvariable = 0;
                item.Flagidentifier = Guid.NewGuid();
            }
            var costesGrupo = costes.GroupBy(f => new { f.Tipocoste, f.Tiporeparto });
            foreach (var item in costesGrupo)
            {
                ReparteCoste(lineas, costes, item.Key);
            }


        }

        private bool SonIgualesCostesOriginalEditado(List<TransformacioneslotesCostesadicionalesModel> costes,
            List<TransformacioneslotesCostesadicionalesModel> costesOriginal)
        {
            var result = true;

            try
            {
                if (costes.Count != costesOriginal.Count || costes.Any(item => !costesOriginal.Any(
                    f =>
                        f.Id == item.Id && f.Total == item.Total && f.Tipodocumento == item.Tipodocumento &&
                        f.Tipocoste == item.Tipocoste && f.Tiporeparto == item.Tiporeparto)))
                {
                    result = false;
                }
            }
            catch (Exception)
            {

                result = false;
            }

            return result;
        }

        private void ReparteCoste(List<TransformacioneslotesLinModel> lineas,
            List<TransformacioneslotesCostesadicionalesModel> costes, dynamic reparto)
        {
            TipoReparto tiporeparto = reparto.Tiporeparto;
            TipoCoste tipocoste = reparto.Tipocoste;
            var costeTotal = costes.Where(f => f.Tiporeparto == tiporeparto && f.Tipocoste == tipocoste).Sum(f => f.Total);
            

            var costeUnidad = 0.0;
            if (tiporeparto == TipoReparto.Cantidad)
            {
                var d = costeTotal / lineas.Sum(f => f.Cantidad);
                if (d != null)
                    costeUnidad = (double)d;
            }
            else if (tiporeparto == TipoReparto.Importe)
            {
                throw new ValidationException(RTransformacioneslotes.ErrorRepartoImporte);
            }
            else if (tiporeparto == TipoReparto.Metros)
            {
                var d = costeTotal / lineas.Sum(f => f.Metros);
                if (d != null)
                    costeUnidad = (double)d;
            }
            else if (tiporeparto == TipoReparto.Peso)
            {
                var d = costeTotal / lineas.Sum(f => (f.Largo * f.Ancho * f.Grueso));
                if (d != null)
                    costeUnidad = (double)d;
                //var unidadesService = FService.Instance.GetService(typeof(UnidadesModel), _context, _db) as UnidadesService;
                //var d = costeTotal / lineas.Sum(f => ModeloNegocioFunciones.CalculaEquivalentePeso(((UnidadesModel)unidadesService.get(f.Fkunidades)).Formula == TipoStockFormulas.Superficie,f.Metros ?? 0, f.Grueso ?? 0));
                //if (d != null)
                //    costeUnidad = (double)d;
            }
            
            RepartirCosteEnLinea(lineas, tiporeparto, tipocoste, costeTotal ?? 0, costeUnidad);
            

        }

        private void RepartirCosteEnLinea(List<TransformacioneslotesLinModel> lineas, TipoReparto reparto, TipoCoste coste, double costeTotal, double costeUnidad)
        {
            foreach (var item in lineas)
            {
                item.Flagidentifier = Guid.NewGuid();
                var costeLineas = 0.0;
                if (reparto == TipoReparto.Cantidad)
                {
                    var d = item.Cantidad * costeUnidad;
                    if (d != null) costeLineas = (double)d;
                }
                else if (reparto == TipoReparto.Importe)
                {
                    throw new ValidationException(RTransformacioneslotes.ErrorRepartoImporte);
                }
                else if (reparto == TipoReparto.Metros)
                {
                    var d = item.Metros * costeUnidad;
                    if (d != null) costeLineas = (double)d;
                }
                else if (reparto == TipoReparto.Peso)
                {
                    var d = item.Largo * item.Ancho * item.Grueso * costeUnidad;
                    if (d != null) costeLineas = (double)d;
                    //var unidadesService = FService.Instance.GetService(typeof(UnidadesModel), _context, _db) as UnidadesService;
                    //var d = ModeloNegocioFunciones.CalculaEquivalentePeso(((UnidadesModel)unidadesService.get(item.Fkunidades)).Formula == TipoStockFormulas.Superficie,item.Metros ?? 0, item.Grueso ?? 0) * costeUnidad;
                    //if (d != null) costeLineas = (double)d;
                }

                costeLineas = Math.Round(costeLineas, item.Decimalesmonedas ?? 2);

                if (coste == TipoCoste.Material)
                {
                    item.Costeadicionalmaterial += costeLineas;
                }
                else if (coste == TipoCoste.Otros)
                {
                    item.Costeadicionalotro += costeLineas;
                }
                else if (coste == TipoCoste.Portes)
                {
                    item.Costeadicionalportes += costeLineas;
                }
                costeTotal -= costeLineas;
            }
            //Esto lo hacemos para asegurarnos de que los costes cuadran con lo que se debe asignar ya que puede haber problemas de redondeos
            if (costeTotal != 0)
            {
                var ultimaLinea = lineas.LastOrDefault();
                if (ultimaLinea != null)
                {
                    costeTotal = Math.Round(costeTotal, ultimaLinea.Decimalesmonedas ?? 2);

                    if (coste == TipoCoste.Material)
                    {
                        ultimaLinea.Costeadicionalmaterial += costeTotal;
                    }
                    else if (coste == TipoCoste.Otros)
                    {
                        ultimaLinea.Costeadicionalotro += costeTotal;
                    }
                    else if (coste == TipoCoste.Portes)
                    {
                        ultimaLinea.Costeadicionalportes += costeTotal;
                    }
                }
                
            }
        }

        #endregion

        private void CalcularPrecioPiezas(List<TransformacioneslotesLinModel> lineas, double costetotal)
        {
            var unidadesService = FService.Instance.GetService(typeof(UnidadesModel), _context, _db) as UnidadesService;
            var preciolinea = costetotal / lineas.Sum(f => ModeloNegocioFunciones.CalculaEquivalentePeso(((UnidadesModel)unidadesService.get(f.Fkunidades)).Formula == TipoStockFormulas.Superficie,f.Metros ?? 0, f.Grueso ?? 0));
            foreach (var item in lineas)
            {
                if (item.Precio != preciolinea)
                {
                    item.Precio = preciolinea *  item.Metros??0;
                    item.Flagidentifier = Guid.NewGuid();
                }
                
            }
        }

        #endregion

        #region Crear lineas entrega de stock

        public List<TransformacioneslotesLinModel> CrearNuevasLineas(List<TransformacioneslotesLinModel> listado, TransformacioneslotesLinVistaModel model)
        {
            var articulosService = FService.Instance.GetService(typeof(ArticulosModel), _context, _db) as ArticulosService;
            var maxId = listado.Any() ? listado.Max(f => f.Id) + 1 : 1;
            var articuloObj = articulosService.GetArticulo(model.Fkarticulos, model.Fkcuenta, model.Fkmonedas, model.Fkregimeniva, model.Flujo);
            if (articuloObj != null && articuloObj.Tipogestionlotes == Tipogestionlotes.Singestion)
            {
                return GenerarLineasSinStock(listado, model, articuloObj, maxId);
            }

            return GenerarLineasConStock(listado, model, articuloObj, maxId);
        }

        public List<TransformacioneslotesLinModel> GenerarLineasSinStock(List<TransformacioneslotesLinModel> listado, TransformacioneslotesLinVistaModel model, ArticulosDocumentosModel articuloObj, int maxId)
        {
            var familiasService = FService.Instance.GetService(typeof(FamiliasproductosModel), _context, _db) as FamiliasproductosService;
            var unidadesService = FService.Instance.GetService(typeof(UnidadesModel), _context, _db) as UnidadesService;
           
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
            var metros = UnidadesService.CalculaResultado(unidadesObj, model.Cantidad, largo, ancho, grueso, model.Metros);
            model.Metros = metros;

            listado.Add(new TransformacioneslotesLinModel()
            {
                Id = maxId++,
                Fkarticulos = model.Fkarticulos,
                Descripcion = articuloObj.Descripcion,
                Cantidad = model.Cantidad,
                Largo = largo,
                Ancho = ancho,
                Grueso = grueso,
                Fkunidades = articuloObj.Fkunidades,
                Metros = metros,
                Decimalesmedidas = unidadesObj.Decimalestotales,
                Decimalesmonedas = monedasObj.Decimales,
                Canal = model.Canal,
               
            }
             );

            return listado;
        }

        private List<TransformacioneslotesLinModel> GenerarLineasConStock(List<TransformacioneslotesLinModel> listado, TransformacioneslotesLinVistaModel model, ArticulosDocumentosModel articuloObj, int maxId)
        {
            var stockactualService = new StockactualService(_context,_db);
            var articulosService = FService.Instance.GetService(typeof(ArticulosModel), _context, _db) as ArticulosService;
            var familiasService = FService.Instance.GetService(typeof(FamiliasproductosModel), _context, _db) as FamiliasproductosService;
            var unidadesService = FService.Instance.GetService(typeof(UnidadesModel), _context, _db) as UnidadesService;
            
            var lotesService = new LotesService(_context);
           
            foreach (var linea in model.Lineas)
            {
                if (!listado.Any(f => f.Lote == linea.Lote && f.Tabla == Funciones.Qint(linea.Loteid)))
                {


                    var stockObj =
                        _db.Stockhistorico.Single(
                            f =>
                                f.fkalmacenes == _context.Fkalmacen && f.empresa == _context.Empresa &&
                                f.lote == linea.Lote && f.loteid == linea.Loteid);
                    var loteObj = lotesService.Get(stockObj.id.ToString());
                    articuloObj = articulosService.GetArticulo(linea.Fkarticulos, model.Fkcuenta, model.Fkmonedas, model.Fkregimeniva, model.Flujo);
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
                    if (linea.Cantidad > item.Cantidad)
                        throw new ValidationException(string.Format("La cantidad indicada para el lote {0} es superior a la que hay en el stock actual", string.Format("{0}{1}", linea.Lote, Funciones.RellenaCod(linea.Loteid, 3))));
                    var unidadesObj = unidadesService.get(familiaObj.Fkunidadesmedida) as UnidadesModel;
                    
                    var metros = UnidadesService.CalculaResultado(unidadesObj, articuloObj.Lotefraccionable ? model.Cantidad : linea.Cantidad, largo, ancho, grueso, model.Metros);
                    linea.Metros = metros;

                    listado.Add(new TransformacioneslotesLinModel()
                    {
                        Id = maxId++,
                        Fkarticulos = linea.Fkarticulos,
                        Descripcion = articuloObj.Descripcion,
                        Lote = linea.Lote,
                        Tabla = Funciones.Qint(linea.Loteid),
                        Cantidad = articuloObj.Lotefraccionable ? model.Cantidad : linea.Cantidad,
                        Largo = largo,
                        Ancho = ancho,
                        Grueso = grueso,
                        Fkunidades = articuloObj.Fkunidades,
                        Metros = metros,
                        Decimalesmedidas = unidadesObj.Decimalestotales,
                        Canal = model.Canal,
                        Flagidentifier = Guid.NewGuid(),
                        Precio= loteObj.Costenetocompra,
                        Tipodealmacenlote = loteObj.Tipodealmacenlote
                    }
                     );
                }

            }

            ValidarKit(listado, model);

            return listado;
        }

        private void ValidarKit(List<TransformacioneslotesLinModel> listado, TransformacioneslotesLinVistaModel model)
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

        #endregion

        #region Gestion de estados

        public void SetEstado(IModelView model, EstadosModel nuevoEstado)
        {
            var currentValidationService = _validationService as TransformacioneslotesValidation;
            currentValidationService.CambiarEstado = true;

            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                var editado = model as TransformacioneslotesModel;
                var original = get(editado.Id.ToString()) as TransformacioneslotesModel;


                if (original.Integridadreferencialflag == editado.Integridadreferencialflag)
                {
                    var estadosService = FService.Instance.GetService(typeof(EstadosModel), _context, _db) as EstadosService;
                    var originalStateObj = estadosService.get(original.Fkestados) as EstadosModel;
                    if (originalStateObj.Tipoestado < TipoEstado.Finalizado)
                    {
                        editado.Fkestados = nuevoEstado.CampoId;
                        base.edit(editado);
                        if (nuevoEstado.Tipoestado == TipoEstado.Finalizado)
                        {
                            var trabajosService = FService.Instance.GetService(typeof(TrabajosModel), _context) as TrabajosService;
                            var trabajosObj = trabajosService.get(editado.Fktrabajos) as TrabajosModel;
                            var materialesObj = new MaterialesModel();

                            if (!String.IsNullOrEmpty(editado.Fkmateriales))
                            {
                                var materialesService = FService.Instance.GetService(typeof(MaterialesModel), _context) as MaterialesService;
                                materialesObj = materialesService.get(editado.Fkmateriales) as MaterialesModel;
                            }

                            RepartirCostesLineas(editado.Lineas, editado.Costes, original.Costes);
                            FinalizarStock(original, editado, trabajosObj, materialesObj);
                        }

                        _db.SaveChanges();
                        tran.Complete();
                    }
                    else
                        throw new Exception("Sólo se pueden modificar transformaciones en estado: Curso o Diseño");
                }
                else throw new IntegridadReferencialException(string.Format(General.ErrorIntegridadReferencial, RTransformacioneslotes.TituloEntidad, original.Referencia));

            }
            currentValidationService.CambiarEstado = false;
        }

        #endregion

        #region Modificar costes

        //public void ModificarCostes(TransformacioneslotesModel model)
        //{
        //    var currentValidationService = _validationService as TransformacioneslotesValidation;
        //    currentValidationService.ModificarCostes = true;
        //    edit(model);
        //    currentValidationService.ModificarCostes = false;
        //}

        public void ModificarCostes(TransformacioneslotesModel model)//, List<TransformacioneslotesModel> costesOriginal = null)
        {
            var currentValidationService = _validationService as TransformacioneslotesValidation;
            currentValidationService.ModificarCostes = true;

            //jmm
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                var original = get(Funciones.Qnull(model.get("id"))) as TransformacioneslotesModel;

                foreach (var item in model.Lineas)
                    item.Nueva = false;

                CalcularCosteTotalMetros(model.Lineas, model.Costes);
                RepartirCostesLineas(model.Lineas, model.Costes);

                //edit(model);
                base.edit(model);

                var trabajosService = FService.Instance.GetService(typeof(TrabajosModel), _context) as TrabajosService;
                var trabajosObj = trabajosService.get(model.Fktrabajos) as TrabajosModel;

                GenerarMovimientosLineasCostes(original.Lineas, original, TipoOperacionService.EliminarCostes, trabajosObj);
                GenerarMovimientosLineasCostes(model.Lineas, model, TipoOperacionService.InsertarCostes, trabajosObj);

                _db.SaveChanges();
                tran.Complete();
            }
            currentValidationService.ModificarCostes = false;
        }


        private void GenerarMovimientosLineasCostes(IEnumerable<TransformacioneslotesLinModel> lineas, TransformacioneslotesModel nuevo, TipoOperacionService movimiento, TrabajosModel trabajosObj, bool finalizarstock = false)
        {
            var movimientosStockService = new MovimientosstockService(_context, _db);
            var articulosService = FService.Instance.GetService(typeof(ArticulosModel), _context, _db) as ArticulosService;
            var serializer = new Serializer<TransformacioneslotesDiarioStockSerializable>();
            var vectorArticulos = new Hashtable();

            var operacion = 1;
            if (movimiento == TipoOperacionService.EliminarCostes)
                operacion = -1;

            foreach (var linea in lineas)
            {
                var codigoarticulonuevo = linea.Fkarticulos;

                if (!string.IsNullOrEmpty(trabajosObj.Fkacabadofinal))
                {

                    codigoarticulonuevo = string.Format("{0}{1}{2}{3}{4}", ArticulosService.GetCodigoFamilia(linea.Fkarticulos),
                        ArticulosService.GetCodigoMaterial(linea.Fkarticulos), ArticulosService.GetCodigoCaracteristica(linea.Fkarticulos),
                        ArticulosService.GetCodigoGrosor(linea.Fkarticulos), trabajosObj.Fkacabadofinal);
                    if (!articulosService.exists(codigoarticulonuevo))
                        throw new Exception(string.Format("El articulo {0} no existe", codigoarticulonuevo));
                }

                ArticulosModel articuloObj;
                if (vectorArticulos.ContainsKey(linea.Fkarticulos))
                    articuloObj = vectorArticulos[linea.Fkarticulos] as ArticulosModel;
                else
                {
                    articuloObj = articulosService.get(linea.Fkarticulos) as ArticulosModel;
                    vectorArticulos.Add(linea.Fkarticulos, articuloObj);
                }

                var aux = Funciones.ConverterGeneric<TransformacionesloteslinSerialized>(linea);

                if (articuloObj?.Gestionstock ?? false)
                {
                    var model = new MovimientosstockModel
                    {
                        Empresa = nuevo.Empresa,
                        Fkalmacenes = nuevo.Fkalmacen.ToString(),
                        Fkalmaceneszona = Funciones.Qint(nuevo.Fkzonas),
                        Fkarticulos = codigoarticulonuevo, //linea.Fkarticulos,
                        Referenciaproveedor = "",
                        Fkcontadorlote = linea.Fkcontadoreslotes,
                        Lote = linea.Lote,
                        Loteid = (linea.Tabla ?? 0).ToString(),
                        Tag = "",
                        Fkunidadesmedida = linea.Fkunidades,
                        //Cantidad = 0,
                        //Largo = linea.Largo ?? 0,
                        //Ancho = linea.Ancho ?? 0,
                        //Grueso = linea.Grueso ?? 0,
                        //Metros = (linea.Metros ?? 0) * operacion,
                        //Pesoneto = ((articuloObj.Kilosud ?? 0) * linea.Metros) * operacion,
                        Documentomovimiento = serializer.GetXml(
                            new TransformacioneslotesDiarioStockSerializable
                            {
                                Id = nuevo.Id ?? 0,
                                Referencia = nuevo.Referencia,
                                Fechadocumento = nuevo.Fechadocumento,
                                Codigoproveedor = nuevo.Fkproveedores,
                                Linea = aux
                            }),
                        //Fkusuarios = Usuarioid,
                        //Tipooperacion = operacion,
                        Costeadicionalmaterial = linea.Costeadicionalmaterial * operacion,
                        Costeadicionalotro = linea.Costeadicionalotro * operacion,
                        Costeadicionalvariable = linea.Costeadicionalvariable * operacion,
                        Costeadicionalportes = linea.Costeadicionalportes * operacion,
                        Tipodealmacenlote = linea.Tipodealmacenlote,

                        Tipomovimiento = movimiento
                    };

                    movimientosStockService.GenerarMovimiento(model, movimiento);
                }

            }

        }



        #endregion
    }
}
