using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Validation;
using Marfil.Inf.Genericos.Helper;
using Resources;
using System;
using RImputacionCostes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.ImputacionCostes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Stock;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using System.Data.Entity.Migrations;
using Marfil.Inf.Genericos;
using System.Collections;
using Marfil.Dom.Persistencia.Model.Documentos.Albaranes;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{

    public interface IImputacionCostesService
    {

    }

    public class ImputacionCostesService : GestionService<ImputacionCostesModel, ImputacionCostes>, IDocumentosServices, IImputacionCostesService
    {

        private string _ejercicioId;
        public string EjercicioId
        {
            get { return _ejercicioId; }
            set
            {
                _ejercicioId = value;
                ((ImputacionCostesValidation)_validationService).EjercicioId = value;
            }
        }

        public bool ValidarEstado(ImputacionCostesModel imp)
        {
            var impdb = _db.ImputacionCostes.Single(f => f.empresa == Empresa && f.referencia == imp.Referencia);
            var a = ((ImputacionCostesValidation)_validationService);
            return a.ValidarGrabar(impdb);
        }

        #region ctr
        public ImputacionCostesService(IContextService context, MarfilEntities db = null) : base(context, db)
        {
        }
        #endregion


        public void SetEstado(IModelView model, EstadosModel nuevoEstado)
        {
        }

        #region list index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var estadosService = FService.Instance.GetService(typeof(EstadosModel), _context, _db) as EstadosService;
            var st = base.GetListIndexModel(t, true, true, controller);

            st.List = st.List.OfType<ImputacionCostesModel>().OrderByDescending(f => f.Referencia);
            var propiedadesVisibles = new[] { "Referencia", "Fechadocumento", "Fkoperarios", "Fkestados"};
            var propiedades = Helper.getProperties<ImputacionCostesModel>();
            st.PrimaryColumnns = new[] { "Id" };
            st.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();

            //PRIMARY COLUMNS
            st.PrimaryColumnns = new[] { "Id" };

            //COMBO
            st.ColumnasCombo.Add("Fkestados", estadosService.GetStates(DocumentoEstado.ImputacionCostes, TipoMovimientos.Todos).Select(f => new Tuple<string, string>(f.CampoId, f.Descripcion)));
            return st;
        }

        public override string GetSelectPrincipal()
        {
            return string.Format("select * from ImputacionCostes where empresa='{0}'", Empresa);
        }

        #endregion

        public ImputacionCostesModel GetByReferencia(string referencia)
        {
            var obj = _db.ImputacionCostes.Single(f => f.empresa == Empresa && f.referencia == referencia);
            return _converterModel.GetModelView(obj) as ImputacionCostesModel;
        }

        public bool ExisteReferencia(string referencia)
        {
            return _db.ImputacionCostes.Any(f => f.empresa == _context.Empresa && f.referencia == referencia);
        }

        public int NextId()
        {
            return _db.ImputacionCostes.Any() ? _db.ImputacionCostes.Max(f => f.id) + 1 : 1;
        }


        #region crud

        public override void create(IModelView obj)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as ImputacionCostesModel;

                var validation = _validationService as ImputacionCostesValidation;
                validation.EjercicioId = EjercicioId;

                model.Id = NextId();
                var appService = new ApplicationHelper(_context);
                if (model.Fechadocumento == null)
                    model.Fechadocumento = DateTime.Now;
                var contador = ServiceHelper.GetNextId<ImputacionCostes>(_db, Empresa, model.Fkseries);
                var identificadorsegmento = "";
                model.Referencia = ServiceHelper.GetReference<ImputacionCostes>(_db, model.Empresa, model.Fkseries, contador, model.Fechadocumento.Value, out identificadorsegmento);
                model.Identificadorsegmento = identificadorsegmento;

                //Calculo id entradas
                int index = 1;
                foreach (var lin in model.LineasLotes)
                {
                    lin.Empresa = model.Empresa;
                    lin.Fkimputacioncostes = model.Id.Value;
                    lin.Id = index;
                    lin.Empresa = Empresa;
                    index++;
                }

                ////Calculo id salidas
                //index = 1;
                //foreach (var lin in model.LineasCostes)
                //{
                //    lin.Empresa = model.Empresa;
                //    lin.Fkimputacioncostes = model.Id.Value;
                //    lin.Id = index;
                //    lin.Empresa = Empresa;
                //    index++;
                //}

                //Llamamos al base
                base.create(model);

                //Guardamos los cambios
                _db.SaveChanges();
                tran.Complete();
            }
        }

        //Editar
        public override void edit(IModelView obj)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                var original = get(Funciones.Qnull(obj.get("id"))) as ImputacionCostesModel;
                var editado = obj as ImputacionCostesModel;

                foreach (var lote in editado.LineasLotes)
                {
                    lote.Empresa = editado.Empresa;
                    lote.Fkimputacioncostes = editado.Id.Value;
                }

                if (original.Integridadreferencialflag == editado.Integridadreferencialflag)
                {

                    var validation = _validationService as ImputacionCostesValidation;
                    validation.EjercicioId = EjercicioId;

                    var estadosService = FService.Instance.GetService(typeof(EstadosModel), _context, _db) as EstadosService;
                    var listestados = estadosService.GetStates(DocumentoEstado.DivisionesLotes); //Lista de estados
                    var estadoFinalizado = listestados.First(f => f.Tipoestado == TipoEstado.Finalizado);

                    //Repartimos costes lineas y actualizamos las tablas del stock
                    if (editado.Fkestados == estadoFinalizado.CampoId)
                    {

                        //Calcular costesadicionales costexm2 o costexm3
                        CalcularCosteTotalMetros(editado.LineasLotes.ToList(), editado.LineasCostes.ToList());
                        //Repartimos los costes en las lineas 
                        RepartirCostesLineas(editado.LineasLotes.ToList(), editado.LineasCostes.ToList());
                        //Generamos el movimiento
                        GenerarMovimientosLineas(editado.LineasLotes, editado, TipoOperacionService.InsertarCostes);
                    }

                    editado.Identificadorsegmento = original.Identificadorsegmento;

                    base.edit(editado);
                    _db.SaveChanges();
                    tran.Complete();
                }
                else
                    throw new IntegridadReferencialException(string.Format(General.ErrorIntegridadReferencial, RImputacionCostes.TituloEntidad, original.Referencia));
            }
        }

        //Generamos la pieza y decimos que tipo de operacion es
        private void GenerarMovimientosLineas(IEnumerable<ImputacionCostesLinModel> lineas, ImputacionCostesModel nuevo, TipoOperacionService movimiento)
        {
            var movimientosStockService = new MovimientosstockService(_context, _db);
            var articulosService = FService.Instance.GetService(typeof(ArticulosModel), _context, _db) as ArticulosService;
            var serializer = new Serializer<AlbaranesComprasDiarioStockSerializable>();
            var vectorArticulos = new Hashtable();

            //Cuando se eliminen costes, se multiplicara por -1
            var operacion = 1;
            if (movimiento == TipoOperacionService.EliminarCostes)
                operacion = -1;

            //para cada una de las lineas se genera un movimiento de stock//SON ARTICULOS!!!!!!
            foreach (var linea in lineas)
            {

                if(linea.Id == 12)
                {
                    var a = 3;
                }


                ArticulosModel articuloObj;
                if (vectorArticulos.ContainsKey(linea.Fkarticulos))
                    articuloObj = vectorArticulos[linea.Fkarticulos] as ArticulosModel;
                else
                {
                    articuloObj = articulosService.get(linea.Fkarticulos) as ArticulosModel;
                    vectorArticulos.Add(linea.Fkarticulos, articuloObj);
                }

                var aux = Funciones.ConverterGeneric<AlbaranesComprasLinSerialized>(linea);

                if (articuloObj?.Gestionstock ?? false)
                {
                    var model = new MovimientosstockModel
                    {
                        Empresa = nuevo.Empresa,
                        Fkalmacenes = _db.Stockhistorico.Where(f => f.empresa == Empresa && f.lote == linea.Lote && f.loteid == linea.Tabla.Value.ToString()).Select(f => f.fkalmacenes).SingleOrDefault(),
                        Fkalmaceneszona = _db.Stockhistorico.Where(f => f.empresa == Empresa && f.lote == linea.Lote && f.loteid == linea.Tabla.Value.ToString()).Select(f => f.fkalmaceneszona).SingleOrDefault(),
                        Fkarticulos = linea.Fkarticulos,
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
                            new AlbaranesComprasDiarioStockSerializable
                            {
                                Id = nuevo.Id.Value,
                                Referencia = nuevo.Referencia,
                                Fechadocumento = nuevo.Fechadocumento,
                                //-----------------OPERARIO (NUEVO)-------------|->------
                                Codigoproveedor = nuevo.Fkoperarios,
                                Linea = aux
                            }),
                        Fkusuarios = Usuarioid,
                        //Tipooperacion = operacion
                        //Tipodealmacenlote = nuevo.Tipodealmacenlote,
                        Cantidad = (linea.Cantidad ?? 0) * operacion,
                        Metros = (linea.Metros ?? 0) * operacion,
                        Pesoneto = ((articuloObj.Kilosud ?? 0) * linea.Metros) * operacion,
                        Costeadicionalmaterial = linea.Costeadicionalmaterial * operacion,
                        Costeadicionalotro = linea.Costeadicionalotro * operacion,
                        Costeadicionalvariable = linea.Costeadicionalvariable * operacion,
                        Costeadicionalportes = linea.Costeadicionalportes * operacion,

                        Tipomovimiento = movimiento //InsertarCostes o EliminarCostes
                    };

                    //la operacion es insertar los costes de imputacion costes
                    var operacionServicio = TipoOperacionService.InsertarImputacionCostes;

                    /*
                    //Definimos la operacion de servicio
                    var operacionServicio = linea.Nueva
                        ? TipoOperacionService.InsertarCostes
                        : TipoOperacionService.EliminarCostes;
                        */
                    /*
                    if (nuevo.Tipoalbaranenum == TipoAlbaran.Devolucion)
                    {
                        operacionServicio = linea.Nueva
                        ? TipoOperacionService.InsertarRecepcionStockDevolucion
                        : TipoOperacionService.ActualizarRecepcionStockDevolucion;
                    }
                    */
                    movimientosStockService.GenerarMovimiento(model, operacionServicio);
                }

            }
        }

        //Calculat Coste total metros
        public void CalcularCosteTotalMetros(List<ImputacionCostesLinModel> lineas, List<ImputacionCostesCostesadicionalesModel> costes)
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

        public override void delete(IModelView obj)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as ImputacionCostesModel;

                //Genera los movimientos de linea sólo si el estado es finalizado
                var estadosService = FService.Instance.GetService(typeof(EstadosModel), _context, _db) as EstadosService;
                var listestados = estadosService.GetStates(DocumentoEstado.DivisionesLotes); //Lista de estados
                var estadoFinalizado = listestados.First(f => f.Tipoestado == TipoEstado.Finalizado);

                if (model.Fkestados == estadoFinalizado.CampoId)
                {
                    GenerarMovimientosLineas(model.LineasLotes, model, TipoOperacionService.EliminarCostes);
                }
                
                base.delete(model);
                _db.SaveChanges();
                tran.Complete();
            }
        }

        #endregion

        #region agregar lineas controller

        public List<ImputacionCostesLinModel> CrearLineasLotes(List<ImputacionCostesLinModel> listado, ImputacionCostesLinVistaModel model)
        {

            var stockactualService = new StockactualService(_context, _db);
            var articulosService = FService.Instance.GetService(typeof(ArticulosModel), _context, _db) as ArticulosService;
            var familiasService = FService.Instance.GetService(typeof(FamiliasproductosModel), _context, _db) as FamiliasproductosService;
            var unidadesService = FService.Instance.GetService(typeof(UnidadesModel), _context, _db) as UnidadesService;
            var tiposivaService = FService.Instance.GetService(typeof(TiposIvaModel), _context, _db) as TiposivaService;

            var maxId = listado.Any() ? listado.Max(f => f.Id) + 1 : 1;
            foreach (var linea in model.Lineas)
            {
                if (!listado.Any(f => f.Lote == linea.Lote && f.Tabla == Funciones.Qint(linea.Loteid)))
                {

                    var articuloObj = articulosService.GetArticulo(linea.Fkarticulos, model.Fkcuenta, model.Fkmonedas, model.Fkregimeniva, model.Flujo);

                    var familiaObj = familiasService.get(ArticulosService.GetCodigoFamilia(linea.Fkarticulos)) as FamiliasproductosModel;

                    var ancho = linea.Anchoentrada;
                    var largo = linea.Largoentrada;
                    var grueso = linea.Gruesoentrada;
                    //if (model.Modificarmedidas)
                    //{
                    //    ancho = model.Ancho;
                    //    largo = model.Largo;
                    //    grueso = model.Grueso;
                    //}
                    //else
                    //{
                    //    var item = familiaObj.Gestionstock
                    //    ? stockactualService.GetArticuloPorLoteOCodigoHistorico(
                    //        string.Format("{0}{1}", linea.Lote, Funciones.RellenaCod(linea.Loteid, 3)), model.Fkalmacen,
                    //        Empresa) as MovimientosstockModel : null;
                    //    ancho = item?.Ancho ?? linea.Anchoentrada;
                    //    largo = item?.Largo ?? linea.Largoentrada;
                    //    grueso = item?.Grueso ?? linea.Gruesoentrada;
                    //}

                    var metros = linea.MetrosEntrada;

                    var unidadesObj = unidadesService.get(familiaObj.Fkunidadesmedida) as UnidadesModel;

                    ////lotes en stock historico
                    //if(linea.Cantidad==0 && linea.Metros>0)
                    //{
                    //    linea.Metros = linea.Metros;
                    //}

                    //else
                    //{
                    //    var metros = UnidadesService.CalculaResultado(unidadesObj, linea.Cantidad, largo, ancho, grueso, linea.MetrosEntrada);
                    //    linea.Metros = metros;
                    //}

                    listado.Add(new ImputacionCostesLinModel(_context)
                    {
                        Id = maxId++,
                        Fkarticulos = linea.Fkarticulos,
                        Descripcion = articuloObj.Descripcion,
                        Lote = linea.Lote,
                        Tabla = Funciones.Qint(linea.Loteid),
                        Cantidad = linea.Cantidadentrada,
                        Largo = largo,
                        Ancho = ancho,
                        Grueso = grueso,
                        Fkunidades = articuloObj.Fkunidades,
                        Metros = metros,
                        Precio = model.Precio,
                        Decimalesmedidas = unidadesObj.Decimalestotales,
                        Decimalesmonedas = 0,
                        Canal = model.Canal
                    }
                     );
                }

            }

            ValidarKit(listado, model);

            return listado;
        }

        private void ValidarKit(List<ImputacionCostesLinModel> listado, ImputacionCostesLinVistaModel model)
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

        private void RepartirCostesLineas(List<ImputacionCostesLinModel> lineas, List<ImputacionCostesCostesadicionalesModel> costes, List<ImputacionCostesCostesadicionalesModel> costesOriginal = null)
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

        //Asignamos el coste total y coste por unidad, tipo reparto y tipo de coste
        private void ReparteCoste(List<ImputacionCostesLinModel> lineas,
            List<ImputacionCostesCostesadicionalesModel> costes, dynamic reparto)
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
                throw new ValidationException(RImputacionCostes.ErrorRepartoImporte);
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

        private void RepartirCosteEnLinea(List<ImputacionCostesLinModel> lineas, TipoReparto reparto, TipoCoste coste, double costeTotal, double costeUnidad)
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
                    throw new ValidationException(RImputacionCostes.ErrorRepartoImporte);
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
                    costeTotal = Math.Round(costeTotal, ultimaLinea.Decimalesmonedas ?? 3);

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

        //Modificamos en el stock actual y el historico los costes generados
        public void ModificarCostesActualOrHistorico(List<ImputacionCostesLinModel> lineas)
        {

            foreach(var model in lineas)
            {

                model.Empresa = Empresa;

                var item = _db.Stockactual.SingleOrDefault(f =>
                f.empresa == model.Empresa && f.fkarticulos == model.Fkarticulos && f.lote == model.Lote && f.loteid == model.Tabla.ToString());

                var historicoitem = _db.Stockhistorico.SingleOrDefault(f =>
                    f.empresa == model.Empresa && f.fkarticulos == model.Fkarticulos && f.lote == model.Lote && f.loteid == model.Tabla.ToString());

                if (item != null)
                {
                    item.costeadicionalmaterial = Math.Round((double)((item.costeadicionalmaterial ?? 0) + model.Costeadicionalmaterial), model.Decimalesmonedas.Value);
                    item.costeadicionalportes = Math.Round((double)((item.costeadicionalportes ?? 0) + model.Costeadicionalportes), model.Decimalesmonedas.Value);
                    item.costeadicionalotro = Math.Round((double)((item.costeadicionalotro ?? 0) + model.Costeadicionalotro), model.Decimalesmonedas.Value);
                    item.costeacicionalvariable = Math.Round((double)((item.costeacicionalvariable ?? 0) + model.Costeadicionalvariable), model.Decimalesmonedas.Value);

                    _db.Stockactual.AddOrUpdate(item);
                }

                historicoitem.costeadicionalmaterial = Math.Round((double)((historicoitem.costeadicionalmaterial ?? 0) + model.Costeadicionalmaterial), model.Decimalesmonedas.Value);
                historicoitem.costeadicionalportes = Math.Round((double)((historicoitem.costeadicionalportes ?? 0) + model.Costeadicionalportes), model.Decimalesmonedas.Value);
                historicoitem.costeadicionalotro = Math.Round((double)((historicoitem.costeadicionalotro ?? 0) + model.Costeadicionalotro), model.Decimalesmonedas.Value);
                historicoitem.costeacicionalvariable = Math.Round((double)((historicoitem.costeacicionalvariable ?? 0) + model.Costeadicionalvariable), model.Decimalesmonedas.Value);

                _db.Stockhistorico.AddOrUpdate(historicoitem);

            }
        }

        public Task SetEstadoAsync(IModelView model, EstadosModel nuevoEstado)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
