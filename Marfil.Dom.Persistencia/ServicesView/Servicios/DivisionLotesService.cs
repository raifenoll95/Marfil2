using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Stock;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Validation;
using Marfil.Inf.Genericos.Helper;
using Marfil.Dom.Persistencia.Model.Documentos.DivisionLotes;
using System.Collections;
using Marfil.Inf.Genericos;

using RDivisionLotes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.DivisionLotes;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{

    public interface IDivisionLotesService
    {

    }

    public class DivisionLotesService : GestionService<DivisionLotesModel, DivisionLotes>, IDocumentosServices, IDivisionLotesService
    {

        private string _ejercicioId;
        public string EjercicioId
        {
            get { return _ejercicioId; }
            set
            {
                _ejercicioId = value;
                //((DivisionLotesValidation)_validationService).EjercicioId = value;
            }
        }
        



        #region CONSTRUCTOR
        public DivisionLotesService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }
        #endregion

        #region List index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var estadosService = FService.Instance.GetService(typeof(EstadosModel), _context, _db) as EstadosService;
            var st = base.GetListIndexModel(t, false, false, controller);

            //EXCLUDED COLUMNS
            var propiedadesVisiblesDivisionLotesSalida = new[] { "Lote", "Fkarticulos", "Descripcion"};
            var propiedadesDivisionLotesSalida = Helper.getProperties<DivisionLotessalidaLinModel>().Where(f => propiedadesVisiblesDivisionLotesSalida.Any(j => j == f.property.Name)).ToList();

            var propiedadesVisiblesDivisionLotes = new[] { "Referencia", "Fechadocumento" };
            var propiedadesDivisionLotes = Helper.getProperties<DivisionLotesModel>().Where(f => propiedadesVisiblesDivisionLotes.Any(j => j == f.property.Name)).ToList(); ;

            st.Properties = propiedadesDivisionLotes.Concat(propiedadesDivisionLotesSalida);


            var properties2 = st.Properties.ToList();

            //properties2[0] = st.Properties[0];

            //PRIMARY COLUMNS
            st.PrimaryColumnns = new[] { "Id" };

            //COMBO
            st.ColumnasCombo.Add("Fkestados", estadosService.GetStates(DocumentoEstado.DivisionesLotes, TipoMovimientos.Todos).Select(f => new Tuple<string, string>(f.CampoId, f.Descripcion)));
            return st;
        }

        public override string GetSelectPrincipal()
        {

            return string.Format("select d.id, d.referencia, d.fechadocumento, s.lote, s.fkarticulos, s.descripcion from DivisionLotes as d, DivisionLotessalidalin as s where d.empresa = s.empresa and d.id = s.fkdivisioneslotes and d.empresa='{0}'", Empresa);
            //return string.Format("select * from DivisionLotes where empresa='{0}'", Empresa);
        }

        public DivisionLotesModel GetByReferencia(string referencia)
        {
            var obj = _db.DivisionLotes.Single(f => f.empresa == Empresa && f.referencia == referencia);
            return _converterModel.GetModelView(obj) as DivisionLotesModel;
        }

        public bool ExisteReferencia(string referencia)
        {
            return _db.DivisionLotes.Any(f => f.empresa == _context.Empresa && f.referencia == referencia);
        }

        #endregion

        public void SetEstado(IModelView model, EstadosModel nuevoEstado)
        {
        }


        public List<DivisionLotessalidaLinModel> CrearNuevasLineasSalida(List<DivisionLotessalidaLinModel> listado, DivisionLotessalidaLinVistaModel model)
        {
            var articulosService = FService.Instance.GetService(typeof(ArticulosModel), _context, _db) as ArticulosService;
            var maxId = listado.Any() ? listado.Max(f => f.Id) + 1 : 1;
            var articuloObj = articulosService.GetArticulo(model.Fkarticulos, model.Fkcuenta, model.Fkmonedas, model.Fkregimeniva, model.Flujo);
            if (articuloObj != null && articuloObj.Tipogestionlotes == Tipogestionlotes.Singestion)
            {
                throw new Exception("Error, el articulo debe tener gestión de lotes");
            }

            return GenerarLineasConStock(listado, model, articuloObj, maxId);
        }


        public List<DivisionLotesentradaLinModel> CrearDosNuevasLineasEntrada(List<DivisionLotessalidaLinModel> listadoSalida, List<DivisionLotesentradaLinModel> listadoEntrada)
        {

            if (listadoEntrada == null)
            {
                listadoEntrada = new List<DivisionLotesentradaLinModel>();
            }

            var CantidadSalida = listadoSalida.First().Cantidad;

            //Bloque o Tabla
            if(CantidadSalida==1)
            {
                listadoEntrada = CrearUnaNuevaLineaEntrada(listadoSalida.First(), listadoEntrada, 1);
                listadoEntrada = CrearUnaNuevaLineaEntrada(listadoSalida.First(), listadoEntrada, 1);
            }

            //Losas u otros (temporal)
            else
            {
                //Si es par, mitad y mitad
                if(CantidadSalida%2 == 0)
                {
                    listadoEntrada = CrearUnaNuevaLineaEntrada(listadoSalida.First(), listadoEntrada, CantidadSalida/2);
                    listadoEntrada = CrearUnaNuevaLineaEntrada(listadoSalida.First(), listadoEntrada, CantidadSalida/2);
                }

                //Impar
                else
                {
                    listadoEntrada = CrearUnaNuevaLineaEntrada(listadoSalida.First(), listadoEntrada, (CantidadSalida+1) / 2);
                    listadoEntrada = CrearUnaNuevaLineaEntrada(listadoSalida.First(), listadoEntrada, (CantidadSalida-1) / 2);
                }
            }
            return listadoEntrada;
        }
        

        public List<DivisionLotesentradaLinModel> CrearUnaNuevaLineaEntrada(DivisionLotessalidaLinModel first, List<DivisionLotesentradaLinModel> listadoEntrada, double? cantidad)
        {

            //CODE
            DivisionLotesentradaLinModel d1 = new DivisionLotesentradaLinModel();
            d1.Cantidad = 1;

            if(listadoEntrada?.Count==0)
            {
                d1.Id = 1;
            }

            else
            {
                d1.Id = (listadoEntrada?.Max(l => l.Id) ?? 0) + 1;
            }

            //Obtenemos la familoa a la que pertenece ese articulo
            var familiaService = FService.Instance.GetService(typeof(FamiliasproductosModel), _context, _db);
            var familiaObj = familiaService.get(ArticulosService.GetCodigoFamilia(first.Fkarticulos)) as FamiliasproductosModel;

            //Si es una tabla, son metros cuadrados y el lote es el mismo pero la tabla es el maximo de la BD + 1
            if (familiaObj.Tipofamilia == TipoFamilia.Tabla)
            {
                d1.Cantidad = cantidad;

                d1.Largo = first.Largo / 2;
                d1.Ancho = first.Ancho;
                d1.Grueso = first.Grueso;
                d1.Metros = d1.Ancho * d1.Largo * d1.Cantidad;

                d1.Lote = first.Lote;
                d1.LoteAutomatico = false;

                d1.Decimalesmonedas = first.Decimalesmonedas;
                d1.Decimalesmedidas = first.Decimalesmedidas;

                
            }

            //Si es un bloque, metros 3 y el lote es automatico
            else if(familiaObj.Tipofamilia == TipoFamilia.Bloque)
            {
                d1.Cantidad = cantidad;

                d1.Largo = first.Largo / 2;
                d1.Ancho = first.Ancho;
                d1.Grueso = first.Grueso;
                d1.Metros = d1.Ancho * d1.Largo * d1.Grueso * d1.Cantidad;

                d1.Tabla = 0;
                d1.Lote = "Lote " + ((listadoEntrada?.Count() ?? 0) + 1).ToString();
                d1.LoteAutomatico = true;

                d1.Decimalesmonedas = first.Decimalesmonedas;
                d1.Decimalesmedidas = first.Decimalesmedidas;   
            }

            //Si son losas
            else
            {
                d1.Cantidad = cantidad;

                d1.Largo = first.Largo;
                d1.Ancho = first.Ancho;
                d1.Grueso = first.Grueso;

                d1.Metros = d1.Largo * d1.Ancho * d1.Cantidad;
                d1.Tabla = 0;
                d1.Lote = "Lote " + ((listadoEntrada?.Count() ?? 0) + 1).ToString();
                d1.LoteAutomatico = true;

                d1.Decimalesmonedas = first.Decimalesmonedas;
                d1.Decimalesmedidas = first.Decimalesmedidas;

                
            }

            d1.Fkarticulos = first.Fkarticulos;
            d1.Descripcion = first.Descripcion;
            d1.Tipodealmacenlote = first.Tipodealmacenlote;
            d1.Fkunidades = first.Fkunidades;
            d1.Canal = first.Canal;

            listadoEntrada.Add(d1);

            return listadoEntrada;
        }

        public List<DivisionLotessalidaLinModel> GenerarLineasSinStock(List<DivisionLotessalidaLinModel> listado, DivisionLotessalidaLinVistaModel model, ArticulosDocumentosModel articuloObj, int maxId)
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

            listado.Add(new DivisionLotessalidaLinModel(_context)
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

        private List<DivisionLotessalidaLinModel> GenerarLineasConStock(List<DivisionLotessalidaLinModel> listado, DivisionLotessalidaLinVistaModel model, ArticulosDocumentosModel articuloObj, int maxId)
        {
            var stockactualService = new StockactualService(_context, _db);
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




                    listado.Add(new DivisionLotessalidaLinModel(_context)
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
                        Precio = loteObj.Costenetocompra,
                        Tipodealmacenlote = (TipoAlmacenlote?)stockObj.tipoalmacenlote,

                    }
                     );
                }

            }

            ValidarKit(listado, model);

            return listado;
        }


        //COMPROBAMOS SI LAS ENTRADAS INTRODUCIDAS SON CORRECTAS
        public void entradasCorrectas(IModelView obj)
        {
            var model = obj as DivisionLotesModel;
            DivisionLotesValidation v = new DivisionLotesValidation(_context, _db);

            //COMPROBAMOS SI LAS ENTRADAS INTRODUCIDAS SON CORRECTAS
            v.ValidarEntradas(model);
        }

        private void ValidarKit(List<DivisionLotessalidaLinModel> listado, DivisionLotessalidaLinVistaModel model)
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

        #region CRUD
        public override void create(IModelView obj)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as DivisionLotesModel;

                //Calculo id entradas
                int index = 1;
                foreach(var lin in model.LineasEntrada)
                {
                    lin.Id = index;
                    index++;
                }

                //Calculo id salidas
                index = 1;
                foreach (var lin in model.LineasSalida)
                {
                    lin.Id = index;
                    index++;
                }

                //Validacion previa para las salidas.
                var validacionPrevia = _validationService as DivisionLotesValidation;

                validacionPrevia.ValidarSalidas(model);
                validacionPrevia.ValidarEntradas(model);

                //Calculamos los lotes de las lineas generadas
                ModificarLotesLineas(model);

                //Costes de las entradas
                var lotesService = new LotesService(_context);
                CalcularPrecioPiezasEntrada(model.LineasEntrada, model.LineasSalida.Sum(f => lotesService.GetByReferencia(f.Lote, f.Tabla.ToString()).Costeneto) ?? 0);
                //CalcularPrecioPiezasEntrada(model.LineasEntrada, model.LineasSalida.Sum(f => lotesService.GetByReferencia(string.Format("{0}{1}", f.Lote, f.Tabla)).Costeneto) ?? 0);

                //Costes adicionales
                RepartirCostesLineas(model.LineasEntrada, model.Costes);

                //Creamos la referencia
                var tipodocumento = "DIV";//Division Lotes
                var contador = ServiceHelper.GetNextId<DivisionLotes>(_db, Empresa, model.Fkseries, tipodocumento);
                var identificadorsegmento = "";                
                model.Referencia = ServiceHelper.GetReference<DivisionLotes>(_db, model.Empresa, model.Fkseries, tipodocumento, contador, model.Fechadocumento.Value, out identificadorsegmento);
                model.Identificadorsegmento = identificadorsegmento;

                //Cuando se crea la Division de lotes, pasa de en curso a finalizado
                var estadosService = FService.Instance.GetService(typeof(EstadosModel), _context, _db) as EstadosService;
                var listestados = estadosService.GetStates(DocumentoEstado.DivisionesLotes); //Lista de estados
                var estado = listestados.First(f => f.Tipoestado == TipoEstado.Finalizado);
                model.Fkestados = estado.CampoId;

                //Llamamos al base
                base.create(obj);

                //Generamos movimientos de salida
                GenerarMovimientosLineasSalida(model.LineasSalida, model, TipoOperacionService.InsertarDivisionLotesSalidaStock);
                //Generamos movimientos de entrada
                GenerarMovimientosLineasEntrada(model.LineasEntrada.ToList(), model, TipoOperacionService.InsertarDivisionLotesEntradaStock);

                //Guardamos los cambios
                _db.SaveChanges();
                tran.Complete();
            }
        }

        //GENERAR MOVIMIENTOS DE SALIDA
        private void GenerarMovimientosLineasSalida(IEnumerable<DivisionLotessalidaLinModel> lineas, DivisionLotesModel nuevo, TipoOperacionService movimiento)//, TipoOperacionService serviciotipo = null)
        {
            var movimientosStockService = new MovimientosstockService(_context, _db);
            var articulosService = FService.Instance.GetService(typeof(ArticulosModel), _context, _db) as ArticulosService;
            var serializer = new Serializer<DivisionLotesSalidaSerializable>();
            var vectorArticulos = new Hashtable();

            //LA CANTIDAD SERA POSITIVA SI QUITAMOS ESE LOTE DEL STOCK Y NEGATIVA SI LO INTRODUCIMOS (LA SALIDA)
            var operacion = 1;
            if (movimiento == TipoOperacionService.InsertarDivisionLotesSalidaStock)
                operacion = -1;

            foreach (var linea in lineas)
            {
                ArticulosModel articuloObj;
                if (vectorArticulos.ContainsKey(linea.Fkarticulos))
                    articuloObj = vectorArticulos[linea.Fkarticulos] as ArticulosModel;
                else
                {
                    articuloObj = articulosService.get(linea.Fkarticulos) as ArticulosModel;
                    vectorArticulos.Add(linea.Fkarticulos, articuloObj);
                }

                var aux = Funciones.ConverterGeneric<DivisionLotesSalidaLinSerialized>(linea);

                if (articuloObj?.Gestionstock ?? false)
                {
                    var model = new MovimientosstockModel
                    {
                        Empresa = nuevo.Empresa,
                        Fkalmacenes = nuevo.Fkalmacen.ToString(),
                        Fkalmaceneszona = Funciones.Qint(nuevo.Fkzonas),
                        Fkarticulos = linea.Fkarticulos,
                        Referenciaproveedor = "",
                        Lote = linea.Lote,
                        Loteid = (linea.Tabla ?? 0).ToString(),
                        Tag = "",
                        Fkunidadesmedida = linea.Fkunidades,
                        Cantidad = (linea.Cantidad ?? 0) * operacion,
                        Largo = linea.Largo ?? 0,
                        Ancho = linea.Ancho ?? 0,
                        Grueso = linea.Grueso ?? 0,
                        Metros = (linea.Metros ?? 0) * operacion,
                        Pesoneto = ((articuloObj.Kilosud ?? 0) * linea.Metros) * operacion,
                        Documentomovimiento = serializer.GetXml(
                            new DivisionLotesSalidaSerializable
                            {
                                Id = nuevo.Id ?? 0,
                                Referencia = nuevo.Referencia,
                                Fechadocumento = nuevo.Fechadocumento,
                                Linea = aux
                            }),
                        Fkusuarios = Usuarioid,
                        Tipodealmacenlote = linea.Tipodealmacenlote,
                        Tipomovimiento = movimiento
                    };
                    movimientosStockService.GenerarMovimiento(model, movimiento);
                }
            }
        }


        //GENERAR MOVIMIENTOS DE ENTRADA
        private void GenerarMovimientosLineasEntrada(IEnumerable<DivisionLotesentradaLinModel> lineas, DivisionLotesModel nuevo, TipoOperacionService movimiento)
        {
            var movimientosStockService = new MovimientosstockService(_context, _db);
            var articulosService = FService.Instance.GetService(typeof(ArticulosModel), _context, _db) as ArticulosService;
            var serializer = new Serializer<DivisionLotesEntradaSerializable>();
            var vectorArticulos = new Hashtable();

            var operacion = 1;


            foreach (var linea in lineas)
            {
                ArticulosModel articuloObj;
                if (vectorArticulos.ContainsKey(linea.Fkarticulos))
                    articuloObj = vectorArticulos[linea.Fkarticulos] as ArticulosModel;
                else
                {
                    articuloObj = articulosService.get(linea.Fkarticulos) as ArticulosModel;
                    vectorArticulos.Add(linea.Fkarticulos, articuloObj);
                }

                var aux = Funciones.ConverterGeneric<DivisionLotesEntradaLinSerialized>(linea);

                if (articuloObj?.Gestionstock ?? false)
                {
                    var model = new MovimientosstockModel
                    {
                        Empresa = nuevo.Empresa,
                        Fkalmacenes = nuevo.Fkalmacen.ToString(),
                        Fkalmaceneszona = Funciones.Qint(nuevo.Fkzonas),
                        Fkarticulos = linea.Fkarticulos,
                        Referenciaproveedor = "",
                        Fkcontadorlote = "",
                        Lote = linea.Lote,
                        Loteid = (linea.Tabla ?? 0).ToString(),
                        Tag = "",
                        Fkunidadesmedida = linea.Fkunidades,
                        Cantidad = (linea.Cantidad ?? 0) * operacion,
                        Largo = linea.Largo ?? 0,
                        Ancho = linea.Ancho ?? 0,
                        Grueso = linea.Grueso ?? 0,
                        Metros = (linea.Metros ?? 0) * operacion,

                        Pesoneto = ((articuloObj.Kilosud ?? 0) * linea.Metros) * operacion,
                        Documentomovimiento = serializer.GetXml(
                            new DivisionLotesEntradaSerializable
                            {
                                Id = nuevo.Id ?? 0,
                                Referencia = nuevo.Referencia,
                                Fechadocumento = nuevo.Fechadocumento,
                                Codigoproveedor = "",
                                Linea = aux
                            }),
                        Fkusuarios = Usuarioid,
                        //Tipooperacion = operacion,         
                        Costeadicionalmaterial = linea.Costeadicionalmaterial,
                        Costeadicionalotro = linea.Costeadicionalotro,
                        Costeadicionalvariable = linea.Costeadicionalvariable,
                        Costeadicionalportes = linea.Costeadicionalportes,
                        Tipodealmacenlote = linea.Tipodealmacenlote,
                        Tipomovimiento = movimiento
                    };

                    movimientosStockService.GenerarMovimiento(model, TipoOperacionService.InsertarDivisionLotesEntradaStock);
                }
            }
        }


        private void ModificarLotesLineas(DivisionLotesModel model)
        {
            var articulosService = FService.Instance.GetService(typeof(ArticulosModel), _context, _db) as ArticulosService;
            var familiaService = FService.Instance.GetService(typeof(FamiliasproductosModel), _context, _db);
            var contadorlotesService = FService.Instance.GetService(typeof(ContadoresLotesModel), _context, _db) as ContadoresLotesService;
            var vecetorincrementocontadores = new Dictionary<string, int>();

            //Obtenemos el tipo de familia de la salida
            String excepcionesGeneradas = "";
            int loteidMax = 0;
            var familiaObj = familiaService.get(ArticulosService.GetCodigoFamilia(model.LineasEntrada.First().Fkarticulos)) as FamiliasproductosModel;

            //OBTENEMOS CUAL ES EL ID MAX DE LA TABLA EN LA BASE DE DATOS
            if (familiaObj.Tipofamilia == TipoFamilia.Tabla)
            {
                //Maximo id tabla
                string lotehistorico = model.LineasEntrada.First().Lote ?? "";
                List <String> lotesidString = _db.Stockhistorico.Where(f => f.empresa == model.Empresa && f.lote == lotehistorico).Select(f => f.loteid).ToList();
                List<int> lotesidInt = new List<int>();

                //Tenemos que pasar los string a Int porque no nos deja hacer converisones de datos en una consulta a la BD
                foreach(var lid in lotesidString)
                {
                    int aux;
                    if (int.TryParse(lid, out aux))
                        lotesidInt.Add(aux);
                }

                //Si ya hay una cantidad establecida, sacamos el maximo
                if(lotesidInt.Count>0)
                    loteidMax = lotesidInt.Max();

                loteidMax++;      
            }

            //PARA CADA UNA DE LAS ENTRADAS, LE VAMOS ASIGNANDO SU NUMERO DE TABLA CORRESPONDIENTE
            foreach (var item in model.LineasEntrada)
            {
                //Hay que generar el lote buscando el numero de la tabla mas alto en la bd
                if(!item.LoteAutomatico)
                {
                    //Si es una tabla
                    if (familiaObj.Tipofamilia == TipoFamilia.Tabla)
                    {
                        item.Lote = model.LineasSalida.First().Lote; //Mismo lote
                        item.Tabla = loteidMax; //Distinto numero de tabla
                        loteidMax++;
                    }
                }

                //Bloques u otros
                if (item.LoteAutomatico)
                {               
                    //Si es un bloque
                    if (familiaObj.Tipofamilia == TipoFamilia.Bloque || familiaObj.Tipofamilia == TipoFamilia.General)
                    {

                        if(!vecetorincrementocontadores.ContainsKey(familiaObj.Fkcontador))
                        {
                            vecetorincrementocontadores.Add(familiaObj.Fkcontador, 0);
                        }    

                        var loteObj = contadorlotesService.get(familiaObj.Fkcontador) as ContadoresLotesModel; //Bl
                        var incremento = vecetorincrementocontadores[familiaObj.Fkcontador]; //190020
                        item.Lote = contadorlotesService.CreateLoteId(loteObj, ref incremento); //Bl-190020
                        vecetorincrementocontadores[familiaObj.Fkcontador] = incremento;
                        item.Tabla = 0;
                    }

                    if (_db.Stockhistorico.Any(f => f.empresa == model.Empresa && f.lote == item.Lote && f.loteid == item.Tabla.ToString()))
                    {
                        excepcionesGeneradas += string.Format("El Lote: {0}.{1} ya existe en el Stock", item.Lote, item.Tabla);
                    }
                        
                }

                /*
                //automatico
                else
                {
                    if (familiaObj.Tipofamilia == TipoFamilia.Tabla)
                    {
                        excepcionesGeneradas += " El tipo Tabla no puede generar lote automático";
                    }

                    else
                    {
                        if (familiaObj.Tipogestionlotes > Tipogestionlotes.Singestion)
                        {
                            if (!vecetorincrementocontadores.ContainsKey(familiaObj.Fkcontador))
                                vecetorincrementocontadores.Add(familiaObj.Fkcontador, 0);

                            var loteObj = contadorlotesService.get(familiaObj.Fkcontador) as ContadoresLotesModel;

                            var incremento = vecetorincrementocontadores[familiaObj.Fkcontador];
                            item.Lote = contadorlotesService.CreateLoteId(loteObj, ref incremento);
                            item.Tabla = 0;

                            vecetorincrementocontadores[familiaObj.Fkcontador] = incremento;
                        }
                    }
                }
                */
            }

            if (!String.IsNullOrEmpty(excepcionesGeneradas))
            {
                throw new ValidationException(excepcionesGeneradas);
            }
        }


        private void CalcularPrecioPiezasEntrada(List<DivisionLotesentradaLinModel> lineas, double costetotal)
        {
            var unidadesService = FService.Instance.GetService(typeof(UnidadesModel), _context, _db) as UnidadesService;
            var preciolinea = costetotal / lineas.Sum(f => ModeloNegocioFunciones.CalculaEquivalentePeso(((UnidadesModel)unidadesService.get(f.Fkunidades)).Formula == TipoStockFormulas.Superficie, f.Metros ?? 0, f.Grueso ?? 0));
            foreach (var item in lineas)
            {
                if (item.Precio != preciolinea)
                {
                    item.Precio = preciolinea * item.Metros ?? 0;
                    item.Flagidentifier = Guid.NewGuid();
                }

            }
        }


        private void RepartirCostesLineas(List<DivisionLotesentradaLinModel> lineas, List<DivisionLotesCostesadicionalesModel> costes, List<DivisionLotesCostesadicionalesModel> costesOriginal = null)
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


        private void ReparteCoste(List<DivisionLotesentradaLinModel> lineas,
            List<DivisionLotesCostesadicionalesModel> costes, dynamic reparto)
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
                throw new ValidationException(RDivisionLotes.ErrorRepartoImporte);
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
                //var d = costeTotal / lineas.Sum(f => ModeloNegocioFunciones.CalculaEquivalentePeso(((UnidadesModel)unidadesService.get(f.Fkunidades)).Formula == TipoStockFormulas.Superficie, f.Metros ?? 0, f.Grueso ?? 0));
                //if (d != null)
                //    costeUnidad = (double)d;
            }

            RepartirCosteEnLinea(lineas, tiporeparto, tipocoste, costeTotal ?? 0, costeUnidad);
        }

        private void RepartirCosteEnLinea(List<DivisionLotesentradaLinModel> lineas, TipoReparto reparto, TipoCoste coste, double costeTotal, double costeUnidad)
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
                    throw new ValidationException(RDivisionLotes.ErrorRepartoImporte);
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
                    //var d = ModeloNegocioFunciones.CalculaEquivalentePeso(((UnidadesModel)unidadesService.get(item.Fkunidades)).Formula == TipoStockFormulas.Superficie, item.Metros ?? 0, item.Grueso ?? 0) * costeUnidad;
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
    }
}
