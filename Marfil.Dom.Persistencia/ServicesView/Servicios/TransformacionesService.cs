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
using Marfil.Dom.Persistencia.Model.Documentos.Transformaciones;
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

using RTransformaciones = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Transformaciones;
using RAlbaranesCompras = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.AlbaranesCompras;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface ITransformacionesService
    {

    }

    public  class ImputacionCosteservice : GestionService<TransformacionesModel, Transformaciones>, IDocumentosServices, ITransformacionesService
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
                ((TransformacionesValidation)_validationService).EjercicioId = value;
            }
        }

        #endregion

        #region CTR

        public ImputacionCosteservice(IContextService context, MarfilEntities db = null) : base(context, db)
        {
            _importarService = new ImportacionService(context);
        }

        #endregion

        #region List index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var estadosService = FService.Instance.GetService(typeof(EstadosModel),_context,_db) as EstadosService;
            var st = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            st.List = st.List.OfType<TransformacionesModel>().OrderByDescending(f => f.Referencia);
            var propiedadesVisibles = new[] { "Referencia", "Fechadocumento", "Fkproveedores", "Nombreproveedor", "Fkestados" };
            var propiedades = Helper.getProperties<TransformacionesModel>();
            st.PrimaryColumnns = new[] { "Id" };
            st.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            st.ColumnasCombo.Add("Fkestados", estadosService.GetStates(DocumentoEstado.Transformacioneslotes,TipoMovimientos.Todos).Select(f => new Tuple<string, string>(f.CampoId, f.Descripcion)));
            return st;
        }

        public override string GetSelectPrincipal()
        {
            return string.Format("select * from Transformaciones where empresa='{0}'", Empresa );
        }

        #endregion

        public TransformacionesModel GetByReferencia(string referencia)
        {
            var obj = _db.Transformaciones.Single(f => f.empresa == Empresa && f.referencia == referencia);
            return _converterModel.GetModelView(obj) as TransformacionesModel;
        }

        public bool ExisteReferencia(string referencia)
        {
            return _db.Transformaciones.Any(f => f.empresa == _context.Empresa && f.referencia == referencia);
        }

        #region Api main

        public TransformacionesModel Clonar(string id)
        {
            throw new NotImplementedException();
        }

        public override void create(IModelView obj)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as TransformacionesModel;
                // Calcular costesadicionales costexm2 o costexm3
                CalcularCosteTotalMetros(model.Lineasentrada, model.Costes);

                var validation = _validationService as TransformacionesValidation;
                validation.EjercicioId = EjercicioId;
                RepartirCostesLineas(model.Lineasentrada, model.Costes);

                //Repartir coste trabajo aserrado
                var trabajoservice = new TrabajosService(_context, _db);

                var familiassalida = model.Lineassalida.GroupBy(elem => elem.Fkarticulos).Select(group => group.First().Fkarticulos.Substring(0, 2)).ToList();
                var familiasM2 = _db.Familiasproductos.Where(f => f.empresa == Empresa && f.fkunidadesmedida == "02").Select(x => x.id).ToList();

                if (familiassalida.Except(familiasM2).Count() == 0) //M2 - Aserrado 
                {
                    var id = _db.Trabajos.Where(f => f.empresa == Empresa && f.tipotrabajo == 0 && f.tipoimputacion == 1).Select(x => x.id).FirstOrDefault();
                    if (!String.IsNullOrEmpty(id))
                    {
                        var trabajo = trabajoservice.get(id) as TrabajosModel;
                        //Cambiamos el precio solo para poder usarlo posteriormente, no lo guardamos
                        trabajo.Precio = (double)(trabajo.Precio * model.Lineassalida.Sum(f => f.Metros));

                        RepartirTrabajoLineas(model.Lineasentrada, trabajo);
                    }
                    
                }
                else //M3 - Aserrado
                {
                    var id = _db.Trabajos.Where(f => f.empresa == Empresa && f.tipotrabajo == 0 && f.tipoimputacion == 0).Select(x => x.id).FirstOrDefault();
                    if (!String.IsNullOrEmpty(id))
                    {
                        var trabajo = trabajoservice.get(id) as TrabajosModel;
                        //Cambiamos el precio solo para poder usarlo posteriormente, no lo guardamos
                        trabajo.Precio = (double)(trabajo.Precio * model.Lineassalida.Sum(f => f.Metros));

                        RepartirTrabajoLineas(model.Lineasentrada, trabajo);
                    }
                }
                //Repartir coste trabajo

                //Calculo ID
                var tipodocumento = "TRF"; //Transformaciones
                var contador = ServiceHelper.GetNextId<Transformaciones>(_db, Empresa, model.Fkseries, tipodocumento);
                var identificadorsegmento = "";
                model.Referencia = ServiceHelper.GetReference<Transformaciones>(_db, model.Empresa, model.Fkseries, tipodocumento, contador, model.Fechadocumento.Value, out identificadorsegmento);
                model.Identificadorsegmento = identificadorsegmento;
                //CalcularPrecioPiezasEntrada(model.Lineasentrada, model.Lineassalida.Sum(f => f.Precio * f.Metros) ?? 0);
                //ModificarLotesLineas(model);
                base.create(obj);

                //CrearStockSalida(model);
                //CrearStockEntrada(model);
                GenerarMovimientosLineasSalida(model.Lineassalida, model, TipoOperacionService.InsertarTransformacionSalidaStock);


                _db.SaveChanges();
                tran.Complete();
            }

        }

        public override void edit(IModelView obj)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                var original = get(Funciones.Qnull(obj.get("id"))) as TransformacionesModel;
                var editado = obj as TransformacionesModel;

                if (original.Integridadreferencialflag == editado.Integridadreferencialflag)
                {
                    var validation = _validationService as TransformacionesValidation;
                    validation.EjercicioId = EjercicioId;
                    //CalcularPrecioPiezasEntrada(editado.Lineasentrada, editado.Lineassalida.Sum(f => f.Precio * f.Metros) ?? 0);
                    RepartirCostesLineas(editado.Lineasentrada, editado.Costes,original.Costes);
                    //ModificarLotesLineas(editado);

                    //Repartir coste trabajo aserrado
                    var trabajoservice = new TrabajosService(_context, _db);

                    var familiassalida = editado.Lineassalida.GroupBy(elem => elem.Fkarticulos).Select(group => group.First().Fkarticulos.Substring(0, 2)).ToList();
                    var familiasM2 = _db.Familiasproductos.Where(f => f.empresa == Empresa && f.fkunidadesmedida == "02").Select(x => x.id).ToList();

                    if (familiassalida.Except(familiasM2).Count() == 0) //M2 - Aserrado 
                    {
                        var id = _db.Trabajos.Where(f => f.empresa == Empresa && f.tipotrabajo == 0 && f.tipoimputacion == 1).Select(x => x.id).FirstOrDefault();
                        if (!String.IsNullOrEmpty(id))
                        {
                            var trabajo = trabajoservice.get(id) as TrabajosModel;
                            //Cambiamos el precio solo para poder usarlo posteriormente, no lo guardamos
                            trabajo.Precio = (double)(trabajo.Precio * editado.Lineassalida.Sum(f => f.Metros));

                            RepartirTrabajoLineas(editado.Lineasentrada, trabajo);
                        }

                    }
                    else //M3 - Aserrado
                    {
                        var id = _db.Trabajos.Where(f => f.empresa == Empresa && f.tipotrabajo == 0 && f.tipoimputacion == 0).Select(x => x.id).FirstOrDefault();
                        if (!String.IsNullOrEmpty(id))
                        {
                            var trabajo = trabajoservice.get(id) as TrabajosModel;
                            //Cambiamos el precio solo para poder usarlo posteriormente, no lo guardamos
                            trabajo.Precio = (double)(trabajo.Precio * editado.Lineassalida.Sum(f => f.Metros));

                            RepartirTrabajoLineas(editado.Lineasentrada, trabajo);
                        }
                    }
                    //Repartir coste trabajo

                    base.edit(obj);

                    //ActualizarStockSalida(original,editado);
                    GenerarMovimientosLineasSalida(original.Lineassalida, original, TipoOperacionService.EliminarTransformacionSalidaStock);
                    GenerarMovimientosLineasSalida(editado.Lineassalida, editado, TipoOperacionService.InsertarTransformacionSalidaStock);

                    var currentValidationService = _validationService as TransformacionesValidation;
                    // TODO: COSTES
                    //if(currentValidationService.ModificarCostes)
                        //ActualizarStockEntrada(original,editado);


                    _db.SaveChanges();
                    tran.Complete();
                }
                else throw new IntegridadReferencialException(string.Format(General.ErrorIntegridadReferencial, RTransformaciones.TituloEntidad, original.Referencia));

            }

        }

        public override void delete(IModelView obj)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as TransformacionesModel;
                base.delete(obj);
                _db.SaveChanges();

                //EliminarStockSalida(obj as TransformacionesModel);
                // EliminarStockEntrada(obj as TransformacionesModel);
                GenerarMovimientosLineasSalida(model.Lineassalida, model, TipoOperacionService.EliminarTransformacionSalidaStock);

                tran.Complete();
            }

        }

        public void CalcularCosteTotalMetros(List<TransformacionesentradaLinModel> lineas, List<TransformacionesCostesadicionalesModel> costes)
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

        #endregion

        #region Movimientos stock entrada

        //private void CrearStockEntrada(TransformacionesModel nuevo, TipoAlmacenlote tipoalmacenlote)
        //{
        //    GenerarMovimientosLineasEntrada(nuevo.Lineasentrada, nuevo, TipoOperacionStock.Entrada);
        //}

        //private void ActualizarStockEntrada(TransformacionesModel original, TransformacionesModel nuevo)
        //{
        //    var list = nuevo.Lineasentrada.ToList();
        //    foreach (var item in list)
        //            {
        //                item.Tipodealmacenlote = original.Tipodealmacenlote;
        //            } 

        //    GenerarMovimientosLineasEntrada(list, nuevo, TipoOperacionStock.Actualizacion);
        //}

        //private void EliminarStockEntrada(TransformacionesModel nuevo, TipoAlmacenlote tipoalmacenlote)
        //{
        //    foreach (var item in nuevo.Lineasentrada)
        //        item.Cantidad *= -1;
        //    GenerarMovimientosLineasEntrada(nuevo.Lineasentrada, nuevo, TipoOperacionStock.Salida);
        //}

        
        private void GenerarMovimientosLineasEntrada(IEnumerable<TransformacionesentradaLinModel> lineas, TransformacionesModel nuevo, TipoOperacionService movimiento)
        {
            var movimientosStockService = new MovimientosstockService(_context, _db);
            var articulosService = FService.Instance.GetService(typeof(ArticulosModel), _context, _db) as ArticulosService;
            var serializer = new Serializer<TransformacionesentradaDiarioStockSerializable>();
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

                var aux = Funciones.ConverterGeneric<TransformacionesEntradaLinSerialized>(linea);

                if (articuloObj?.Gestionstock ?? false)
                {
                    var model = new MovimientosstockModel
                    {
                        Empresa = nuevo.Empresa,
                        Fkalmacenes = nuevo.Fkalmacen.ToString(),
                        Fkalmaceneszona = Funciones.Qint(nuevo.Fkzonas),
                        Fkarticulos = linea.Fkarticulos,
                        Referenciaproveedor = "",
                        Fkcontadorlote = linea.Fkcontadoreslotes,
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
                            new TransformacionesentradaDiarioStockSerializable
                            {
                                Id = nuevo.Id??0,
                                Referencia = nuevo.Referencia,
                                Fechadocumento = nuevo.Fechadocumento,
                                Codigoproveedor = nuevo.Fkproveedores,
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

                    movimientosStockService.GenerarMovimiento(model, linea.Nueva ? TipoOperacionService.InsertarTransformacionEntradaStock : TipoOperacionService.ActualizarTransformacionEntradaStock);
                }

            }
        }

        private async Task GenerarMovimientosLineasEntradaAsync(IEnumerable<TransformacionesentradaLinModel> lineas, TransformacionesModel nuevo, TipoOperacionService movimiento)
        {
            var movimientosStockService = new MovimientosstockService(_context, _db);
            var articulosService = FService.Instance.GetService(typeof(ArticulosModel), _context, _db) as ArticulosService;
            var serializer = new Serializer<TransformacionesentradaDiarioStockSerializable>();
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

                var aux = Funciones.ConverterGeneric<TransformacionesEntradaLinSerialized>(linea);

                if (articuloObj?.Gestionstock ?? false)
                {
                    var model = new MovimientosstockModel
                    {
                        Empresa = nuevo.Empresa,
                        Fkalmacenes = nuevo.Fkalmacen.ToString(),
                        Fkalmaceneszona = Funciones.Qint(nuevo.Fkzonas),
                        Fkarticulos = linea.Fkarticulos,
                        Referenciaproveedor = "",
                        Fkcontadorlote = linea.Fkcontadoreslotes,
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
                            new TransformacionesentradaDiarioStockSerializable
                            {
                                Id = nuevo.Id ?? 0,
                                Referencia = nuevo.Referencia,
                                Fechadocumento = nuevo.Fechadocumento,
                                Codigoproveedor = nuevo.Fkproveedores,
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

                    await movimientosStockService.GenerarMovimientoAsync(model, linea.Nueva ? TipoOperacionService.InsertarTransformacionEntradaStock : TipoOperacionService.ActualizarTransformacionEntradaStock);
                }

            }
        }

        #endregion

        #region Movimientos stock salida

        //private void CrearStockSalida(TransformacionesModel nuevo)
        //{
        //    foreach (var item in nuevo.Lineassalida)
        //        item.Cantidad *= -1;

        //    GenerarMovimientosLineasSalida(nuevo.Lineassalida, nuevo, TipoOperacionStock.Salida, TipoOperacionService.InsertarTransformacionSalidaStock);
        //}

        //private void ActualizarStockSalida(TransformacionesModel original, TransformacionesModel nuevo)
        //{
        //    var list = new List<TransformacionessalidaLinModel>();
        //    var lineasModificadas = nuevo.Lineassalida.Where(f => !original.Lineassalida.Any(j => j.Flagidentifier == f.Flagidentifier)).ToList();
        //    foreach (var item in lineasModificadas)
        //        item.Cantidad *= -1;
        //    var lineasEliminadas = original.Lineassalida.Where(f => !nuevo.Lineassalida.Where(j => !lineasModificadas.Any(h => h.Flagidentifier == f.Flagidentifier)).Any(j => j.Flagidentifier == f.Flagidentifier)).ToList();


        //    list = lineasModificadas.Union(lineasEliminadas).ToList();


        //    GenerarMovimientosLineasSalida(list, nuevo, TipoOperacionStock.Actualizacion, TipoOperacionService.InsertarTransformacionSalidaStock);
        //}

        //private void EliminarStockSalida(TransformacionesModel nuevo)
        //{

        //    GenerarMovimientosLineasSalida(nuevo.Lineassalida, nuevo, TipoOperacionStock.Entrada, TipoOperacionService.EliminarTransformacionSalidaStock);
        //}


        private void GenerarMovimientosLineasSalida(IEnumerable<TransformacionessalidaLinModel> lineas, TransformacionesModel nuevo, TipoOperacionService movimiento)//, TipoOperacionService serviciotipo = null)
        {
            var movimientosStockService = new MovimientosstockService(_context, _db);
            var articulosService = FService.Instance.GetService(typeof(ArticulosModel), _context, _db) as ArticulosService;
            var serializer = new Serializer<TransformacionessalidaDiarioStockSerializable>();
            var vectorArticulos = new Hashtable();

            var operacion = 1;
            if (movimiento == TipoOperacionService.InsertarTransformacionSalidaStock)
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

                var aux = Funciones.ConverterGeneric<TransformacionesSalidaLinSerialized>(linea);

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
                            new TransformacionessalidaDiarioStockSerializable
                            {
                                Id = nuevo.Id ?? 0,
                                Referencia = nuevo.Referencia,
                                Fechadocumento = nuevo.Fechadocumento,
                                Linea = aux
                            }),
                        Fkusuarios = Usuarioid,
                        //Tipooperacion = operacion,
                        Tipodealmacenlote = linea.Tipodealmacenlote,                        

                        Tipomovimiento = movimiento
                };

                    movimientosStockService.GenerarMovimiento(model, movimiento);
                }

            }
        }

        private async Task GenerarMovimientosLineasSalidaAsync(IEnumerable<TransformacionessalidaLinModel> lineas, TransformacionesModel nuevo, TipoOperacionService movimiento)//, TipoOperacionService serviciotipo = null)
        {
            var movimientosStockService = new MovimientosstockService(_context, _db);
            var articulosService = FService.Instance.GetService(typeof(ArticulosModel), _context, _db) as ArticulosService;
            var serializer = new Serializer<TransformacionessalidaDiarioStockSerializable>();
            var vectorArticulos = new Hashtable();

            var operacion = 1;
            if (movimiento == TipoOperacionService.InsertarTransformacionSalidaStock)
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

                var aux = Funciones.ConverterGeneric<TransformacionesSalidaLinSerialized>(linea);

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
                            new TransformacionessalidaDiarioStockSerializable
                            {
                                Id = nuevo.Id ?? 0,
                                Referencia = nuevo.Referencia,
                                Fechadocumento = nuevo.Fechadocumento,
                                Linea = aux
                            }),
                        Fkusuarios = Usuarioid,
                        //Tipooperacion = operacion,
                        Tipodealmacenlote = linea.Tipodealmacenlote,

                        Tipomovimiento = movimiento
                    };

                   await movimientosStockService.GenerarMovimientoAsync(model, movimiento);
                }

            }
        }

        #endregion

        #region Helper


        #region Reparto de costes

        private void RepartirCostesLineas(List<TransformacionesentradaLinModel> lineas, List<TransformacionesCostesadicionalesModel> costes, List<TransformacionesCostesadicionalesModel> costesOriginal = null)
        {
            //limpiar costes
           /* if (costesOriginal != null)
            {
                if (SonIgualesCostesOriginalEditado(costes, costesOriginal))
                {
                    return;
                }
            }*/

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

        private void RepartirTrabajoLineas(List<TransformacionesentradaLinModel> lineas, TrabajosModel trabajo)
        {
            /*foreach (var item in lineas)
            {
                item.Costeadicionalmaterial = 0;
                item.Costeadicionalotro = 0;
                item.Costeadicionalportes = 0;
                item.Costeadicionalvariable = 0;
                item.Flagidentifier = Guid.NewGuid();
            }*/

            TipoReparto tiporeparto = trabajo.Tipoimputacion == TipoImputacion.M2Real || trabajo.Tipoimputacion == TipoImputacion.M3 ? TipoReparto.Metros : TipoReparto.Peso;
            var costeTotal = trabajo.Precio;

            var costeUnidad = 0.0;

            if (tiporeparto == TipoReparto.Metros)
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

            RepartirCosteTrabajoEnLinea(lineas, tiporeparto, costeTotal, costeUnidad);

        }

        private bool SonIgualesCostesOriginalEditado(List<TransformacionesCostesadicionalesModel> costes,
            List<TransformacionesCostesadicionalesModel> costesOriginal)
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

        private void ReparteCoste(List<TransformacionesentradaLinModel> lineas,
            List<TransformacionesCostesadicionalesModel> costes, dynamic reparto)
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
                throw new ValidationException(RTransformaciones.ErrorRepartoImporte);
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

        private void RepartirCosteEnLinea(List<TransformacionesentradaLinModel> lineas, TipoReparto reparto, TipoCoste coste, double costeTotal, double costeUnidad)
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
                    throw new ValidationException(RTransformaciones.ErrorRepartoImporte);
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

        private void RepartirCosteTrabajoEnLinea(List<TransformacionesentradaLinModel> lineas, TipoReparto reparto, double costeTotal, double costeUnidad)
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
                    throw new ValidationException(RTransformaciones.ErrorRepartoImporte);
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

                item.Costeadicionalvariable += costeLineas;
                costeTotal -= costeLineas;
            }
            //Esto lo hacemos para asegurarnos de que los costes cuadran con lo que se debe asignar ya que puede haber problemas de redondeos
            if (costeTotal != 0)
            {
                var ultimaLinea = lineas.LastOrDefault();
                if (ultimaLinea != null)
                {
                    costeTotal = Math.Round(costeTotal, ultimaLinea.Decimalesmonedas ?? 2);

                    ultimaLinea.Costeadicionalvariable += costeTotal;
                }
            }
        }


        #endregion

        private void CalcularPrecioPiezasEntrada(List<TransformacionesentradaLinModel> lineas, double costetotal)
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

        private struct StLote
        {
            public string Lote { get; set; }
            public int Numero { get; set; }
        }

        private void ModificarLotesLineas(TransformacionesModel model)
        {
            var articulosService = FService.Instance.GetService(typeof(ArticulosModel), _context, _db) as ArticulosService;
            var familiaService = FService.Instance.GetService(typeof(FamiliasproductosModel), _context, _db);
            var contadorlotesService = FService.Instance.GetService(typeof(ContadoresLotesModel), _context, _db) as ContadoresLotesService;
            var vectoridentificadorlotes = new Dictionary<string, StLote>();
            var vecetorincrementocontadores = new Dictionary<string, int>();

            String excepcionesGeneradas = "";

            foreach (var item in model.Lineasentrada)
            {
                
                
                if (item.Nueva && string.IsNullOrEmpty(item.Loteautomaticoid))
                {

                    if (_db.Stockactual.Any(f => f.empresa == model.Empresa && f.lote == item.Lote && f.loteid == item.Tabla.ToString()))
                        excepcionesGeneradas += string.Format("El Lote: {0}.{1} ya existe en el Stock", item.Lote, item.Tabla);

                }
                else if (item.Nueva && !string.IsNullOrEmpty(item.Loteautomaticoid))
                {
                    
                    var familiaObj = familiaService.get(ArticulosService.GetCodigoFamilia(item.Fkarticulos)) as FamiliasproductosModel;
                    if (familiaObj.Tipogestionlotes > Tipogestionlotes.Singestion)
                    {
                        if (!vecetorincrementocontadores.ContainsKey(familiaObj.Fkcontador))
                            vecetorincrementocontadores.Add(familiaObj.Fkcontador, 0);

                        var loteObj = contadorlotesService.get(familiaObj.Fkcontador) as ContadoresLotesModel;
                        var objlote = new StLote();
                        if (vectoridentificadorlotes.ContainsKey(item.Loteautomaticoid))
                        {
                            objlote = vectoridentificadorlotes[item.Loteautomaticoid];
                        }
                        else
                        {
                            var incremento = vecetorincrementocontadores[familiaObj.Fkcontador];
                            objlote = new StLote() { Lote = contadorlotesService.CreateLoteId(loteObj, ref incremento), Numero = 0 };
                            vecetorincrementocontadores[familiaObj.Fkcontador] = incremento;
                        }

                        if (familiaObj.Tipofamilia != TipoFamilia.Tabla)
                            objlote.Numero = 0;
                        else
                            objlote.Numero++;

                        if (vectoridentificadorlotes.ContainsKey(item.Loteautomaticoid))
                            vectoridentificadorlotes.Remove(item.Loteautomaticoid);



                        item.Lote = objlote.Lote;
                        item.Tabla = objlote.Numero;
                        
                        vectoridentificadorlotes.Add(item.Loteautomaticoid, objlote);

                    }
                }

            }

            if (!String.IsNullOrEmpty(excepcionesGeneradas))
            {
                throw new ValidationException(excepcionesGeneradas);
            }
        }
      

        #endregion

        #region Api importar stock

        public IEnumerable<TransformacionesentradaLinModel> CrearNuevasLineasEntrada(List<TransformacionesentradaLinModel> lineas, TransformacionesentradaLinVistaModel linea)
        {
            var articulosService = FService.Instance.GetService(typeof(ArticulosModel), _context, _db) as ArticulosService;
            var familiasService = FService.Instance.GetService(typeof(FamiliasproductosModel), _context, _db) as FamiliasproductosService;
            var unidadesService = FService.Instance.GetService(typeof(UnidadesModel), _context, _db) as UnidadesService;
            

            var articuloObj = articulosService.get(linea.Fkarticulos) as ArticulosModel;
            var familiaObj = familiasService.get(ArticulosService.GetCodigoFamilia(linea.Fkarticulos)) as FamiliasproductosModel;
            var unidadesObj = unidadesService.get(familiaObj.Fkunidadesmedida) as UnidadesModel;
            

            var result = new List<TransformacionesentradaLinModel>();
            if (!string.IsNullOrEmpty(linea.Lote))
                linea.Lote = linea.Lote.ToUpper();
            if (articuloObj.Articulocomentariovista)
            {
                linea.Cantidad = 1;
            }
            var incremento = (familiaObj.Tipofamilia == TipoFamilia.Bloque ||
                              familiaObj.Tipofamilia == TipoFamilia.Tabla)
                ? 1
                : linea.Cantidad;

            string lote;
            string loteautomaticoid;
            int lotenuevocontador;
            int? tabla;
            AlbaranesComprasGesionLotesService.GestionarLote(articuloObj, familiaObj, lineas.Select(f=>(IDocumentosLinModel)f), linea, out lote, out loteautomaticoid, out lotenuevocontador, out tabla);

            var metros = UnidadesService.CalculaResultado(unidadesObj, incremento, linea.Largo, linea.Ancho,
               linea.Grueso, linea.Metros);
            for (var i = 0; i < linea.Cantidad; i += incremento)
            {
                linea.Metros = metros;

                result.Add(new TransformacionesentradaLinModel()
                {
                    Nueva = true,
                    Fkarticulos = linea.Fkarticulos,
                    Descripcion = articuloObj.Descripcion,
                    Fkcontadoreslotes = linea.Loteautomatico ? familiaObj.Fkcontador : string.Empty,
                    Lote = lote,
                    Loteautomaticoid = loteautomaticoid,
                    Lotenuevocontador = lotenuevocontador,
                    Tabla = tabla,
                    Cantidad = articuloObj.Articulocomentariovista ? 0 : incremento,
                    Largo = linea.Largo,
                    Ancho = linea.Ancho,
                    Grueso = linea.Grueso,
                    Fkunidades = linea.Fkunidades.ToString(),
                    Metros = articuloObj.Articulocomentariovista ? 0 : metros,
                    Decimalesmedidas = linea.Decimalesmedidas,
                    Decimalesmonedas = linea.Decimalesmonedas,
                    Precio=linea.Precio,
                    
                }
                 );

                if (tabla.HasValue)
                    tabla++;
            }

            return result;

        }

        #endregion

        #region Crear lineas entrega de stock

        public List<TransformacionessalidaLinModel> CrearNuevasLineasSalida(List<TransformacionessalidaLinModel> listado, TransformacionessalidaLinVistaModel model)
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

        public List<TransformacionessalidaLinModel> GenerarLineasSinStock(List<TransformacionessalidaLinModel> listado, TransformacionessalidaLinVistaModel model, ArticulosDocumentosModel articuloObj, int maxId)
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

            listado.Add(new TransformacionessalidaLinModel()
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

        private List<TransformacionessalidaLinModel> GenerarLineasConStock(List<TransformacionessalidaLinModel> listado, TransformacionessalidaLinVistaModel model, ArticulosDocumentosModel articuloObj, int maxId)
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
                    
                    
                    

                    listado.Add(new TransformacionessalidaLinModel()
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
                        Tipodealmacenlote= (TipoAlmacenlote?)stockObj.tipoalmacenlote,

                    }
                     );
                }

            }

            ValidarKit(listado, model);

            return listado;
        }

        private void ValidarKit(List<TransformacionessalidaLinModel> listado, TransformacionessalidaLinVistaModel model)
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
            var currentValidationService = _validationService as TransformacionesValidation;
            currentValidationService.CambiarEstado = true;

            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                var editado = model as TransformacionesModel;
                var original = get(editado.Id.ToString()) as TransformacionesModel;


                if (original.Integridadreferencialflag == editado.Integridadreferencialflag)
                {
                    var estadosService = FService.Instance.GetService(typeof(EstadosModel), _context, _db) as EstadosService;
                    var originalStateObj = estadosService.get(original.Fkestados) as EstadosModel;
                    if (originalStateObj.Tipoestado < TipoEstado.Finalizado)
                    {
                        editado.Fkestados = nuevoEstado.CampoId;
                        
                        if (nuevoEstado.Tipoestado == TipoEstado.Finalizado)
                        {
                            //RepartirCostesLineas(editado.Lineasentrada, editado.Costes, original.Costes);
                            FinalizarStock(original, editado);
                        }
                        else
                            base.edit(editado);

                        _db.SaveChanges();
                        tran.Complete();
                    }
                    else
                        throw new Exception("Sólo se pueden modificar transformaciones en estado: Curso o Diseño");
                }
                else throw new IntegridadReferencialException(string.Format(General.ErrorIntegridadReferencial, RTransformaciones.TituloEntidad, original.Referencia));

            }
            currentValidationService.CambiarEstado = false;
        }

        public async Task SetEstadoAsync(IModelView model, EstadosModel nuevoEstado)
        {
            var currentValidationService = _validationService as TransformacionesValidation;
            currentValidationService.CambiarEstado = true;

            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                var editado = model as TransformacionesModel;
                var original = get(editado.Id.ToString()) as TransformacionesModel;


                if (original.Integridadreferencialflag == editado.Integridadreferencialflag)
                {
                    var estadosService = FService.Instance.GetService(typeof(EstadosModel), _context, _db) as EstadosService;
                    var originalStateObj = estadosService.get(original.Fkestados) as EstadosModel;
                    if (originalStateObj.Tipoestado < TipoEstado.Finalizado)
                    {
                        editado.Fkestados = nuevoEstado.CampoId;

                        if (nuevoEstado.Tipoestado == TipoEstado.Finalizado)
                        {
                            //RepartirCostesLineas(editado.Lineasentrada, editado.Costes, original.Costes);
                            await FinalizarStockAsync(original, editado);
                        }
                        else
                            await base.editAsync(editado);

                        await _db.SaveChangesAsync();
                        tran.Complete();
                    }
                    else
                        throw new Exception("Sólo se pueden modificar transformaciones en estado: Curso o Diseño");
                }
                else throw new IntegridadReferencialException(string.Format(General.ErrorIntegridadReferencial, RTransformaciones.TituloEntidad, original.Referencia));
                
            }
            currentValidationService.CambiarEstado = false;
        }

        private void FinalizarStock(TransformacionesModel original, TransformacionesModel editado)
        {
            var lotesService = new LotesService(_context);
            CalcularPrecioPiezasEntrada(editado.Lineasentrada, editado.Lineassalida.Sum(f => lotesService.GetByReferencia(f.Lote, f.Tabla.ToString()).Costeneto) ?? 0);
            //CalcularPrecioPiezasEntrada(editado.Lineasentrada, editado.Lineassalida.Sum(f => lotesService.GetByReferencia(string.Format("{0}{1}",f.Lote,f.Tabla)).Costeneto) ?? 0);

            /*Ago22 - Si el producto de salida=1 elemento y es de tipo bloque  y la unidad de medida del/los producto de entrada es m2. Si y solo si existe en costes adicionales un coste x m3
            En ese caso, multiplicamos ese coste por los m3 del bloque (salida) y se reparte entre todos los metros de entrada (que serán tablas o losas o cualquier otro producto que vaya en m2)*/
            //CalcularTotalBloqueM3(editado); - Lo hemosCambiado al controlador para que antualice las pantallas con el total
            /*Ago22*/

            RepartirCostesLineas(editado.Lineasentrada, editado.Costes, original.Costes);

            //Repartir coste trabajo aserrado
            var trabajoservice = new TrabajosService(_context, _db);

            var familiassalida = editado.Lineassalida.GroupBy(elem => elem.Fkarticulos).Select(group => group.First().Fkarticulos.Substring(0, 2)).ToList();
            var familiasM2 = _db.Familiasproductos.Where(f => f.empresa == Empresa && f.fkunidadesmedida == "02").Select(x => x.id).ToList();

            if (familiassalida.Except(familiasM2).Count() == 0) //M2 - Aserrado 
            {
                var id = _db.Trabajos.Where(f => f.empresa == Empresa && f.tipotrabajo == 0 && f.tipoimputacion == 1).Select(x => x.id).FirstOrDefault();
                if (!String.IsNullOrEmpty(id))
                {
                    var trabajo = trabajoservice.get(id) as TrabajosModel;
                    //Cambiamos el precio solo para poder usarlo posteriormente, no lo guardamos
                    trabajo.Precio = (double)(trabajo.Precio * editado.Lineassalida.Sum(f => f.Metros));

                    RepartirTrabajoLineas(editado.Lineasentrada, trabajo);
                }

            }
            else //M3 - Aserrado
            {
                var id = _db.Trabajos.Where(f => f.empresa == Empresa && f.tipotrabajo == 0 && f.tipoimputacion == 0).Select(x => x.id).FirstOrDefault();
                if (!String.IsNullOrEmpty(id))
                {
                    var trabajo = trabajoservice.get(id) as TrabajosModel;
                    //Cambiamos el precio solo para poder usarlo posteriormente, no lo guardamos
                    trabajo.Precio = (double)(trabajo.Precio * editado.Lineassalida.Sum(f => f.Metros));

                    RepartirTrabajoLineas(editado.Lineasentrada, trabajo);
                }
            }
            //Repartir coste trabajo

            ModificarLotesLineas(editado);

            base.edit(editado);

            //ActualizarStockSalida(original, editado);
            //ActualizarStockEntrada(original, editado);
            //(TransformacionesModel original, TransformacionesModel nuevo)
            var list = editado.Lineasentrada.ToList();
            foreach (var item in list)
            {
                item.Tipodealmacenlote = original.Tipodealmacenlote;
            }

            GenerarMovimientosLineasEntrada(list, editado, TipoOperacionService.InsertarTransformacionEntradaStock);

        }

        public void CalcularTotalBloqueM3(TransformacionesModel editado)
        {
            var codBloq = _db.Familiasproductos.Where(f => f.empresa == Empresa && f.descripcion.Equals("BLOQUES")).FirstOrDefault().id;
            if (editado.Lineassalida.Count == 1 && editado.Lineassalida.FirstOrDefault().Fkarticulos.Substring(0, 2) == codBloq)
            {
                var familiasentrada = editado.Lineasentrada.GroupBy(elem => elem.Fkarticulos).Select(group => group.First().Fkarticulos.Substring(0, 2)).ToList();
                var familiasM2 = _db.Familiasproductos.Where(f => f.empresa == Empresa && f.fkunidadesmedida == "02").Select(x => x.id).ToList();

                if (familiasentrada.Except(familiasM2).Count() == 0 && editado.Costes.Where(f => f.Tipodocumento == TipoCosteAdicional.Costexm3).Count() > 0)
                {
                    foreach (var item in editado.Costes.Where(f => f.Tipodocumento == TipoCosteAdicional.Costexm3))
                    {
                        item.Total = item.Importe * editado.Lineassalida.FirstOrDefault().Metros;
                    }
                }
            }
        }

        public void CalcularTotalBloqueM3Pantalla(List<TransformacionessalidaLinModel> lineassalida, List<TransformacionesentradaLinModel> lineasentrada, TransformacionesCostesadicionalesModel coste)
        {
            var codBloq = _db.Familiasproductos.Where(f => f.empresa == Empresa && f.descripcion.Equals("BLOQUES")).FirstOrDefault().id;
            if (lineassalida.Count == 1 && lineassalida.FirstOrDefault().Fkarticulos.Substring(0, 2) == codBloq)
            {
                var familiasentrada = lineasentrada.GroupBy(elem => elem.Fkarticulos).Select(group => group.First().Fkarticulos.Substring(0, 2)).ToList();
                var familiasM2 = _db.Familiasproductos.Where(f => f.empresa == Empresa && f.fkunidadesmedida == "02").Select(x => x.id).ToList();

                if (familiasentrada.Except(familiasM2).Count() == 0 && coste.Tipodocumento == TipoCosteAdicional.Costexm3)
                {
                    coste.Total = coste.Importe * lineassalida.FirstOrDefault().Metros;
                }
            }
        }

        private async Task FinalizarStockAsync(TransformacionesModel original, TransformacionesModel editado)
        {
            var lotesService = new LotesService(_context);
            CalcularPrecioPiezasEntrada(editado.Lineasentrada, editado.Lineassalida.Sum(f => lotesService.GetByReferencia(f.Lote, f.Tabla.ToString()).Costeneto) ?? 0);
            //CalcularPrecioPiezasEntrada(editado.Lineasentrada, editado.Lineassalida.Sum(f => lotesService.GetByReferencia(string.Format("{0}{1}",f.Lote,f.Tabla)).Costeneto) ?? 0);
            RepartirCostesLineas(editado.Lineasentrada, editado.Costes, original.Costes);

            ModificarLotesLineas(editado);

            await base.editAsync(editado);

            //ActualizarStockSalida(original, editado);
            //ActualizarStockEntrada(original, editado);
            //(TransformacionesModel original, TransformacionesModel nuevo)
            var list = editado.Lineasentrada.ToList();
            foreach (var item in list)
            {
                item.Tipodealmacenlote = original.Tipodealmacenlote;
            }

            await GenerarMovimientosLineasEntradaAsync(list, editado, TipoOperacionService.InsertarTransformacionEntradaStock);

        }

        #endregion

        public void ModificarCostes(TransformacionesModel model, List<TransformacionesCostesadicionalesModel> costesOriginal = null)
        {
            var currentValidationService = _validationService as TransformacionesValidation;
            currentValidationService.ModificarCostes = true;

            //jmm
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {                
                var original = get(Funciones.Qnull(model.get("id"))) as TransformacionesModel;

                foreach (var item in model.Lineasentrada)
                    item.Nueva = false;

                // Hace falta esto?
                //CalcularCosteTotalMetros(original.Lineasentrada, original.Costes);
                //RepartirCostesLineas(original.Lineasentrada, original.Costes, costesOriginal);

                CalcularCosteTotalMetros(model.Lineasentrada, model.Costes);
                RepartirCostesLineas(model.Lineasentrada, model.Costes, costesOriginal);

                //edit(model);
                base.edit(model);

                GenerarMovimientosLineasCostes(original.Lineasentrada, original, TipoOperacionService.EliminarCostes);
                GenerarMovimientosLineasCostes(model.Lineasentrada, model, TipoOperacionService.InsertarCostes);

                _db.SaveChanges();
                tran.Complete();
            }
            currentValidationService.ModificarCostes = false;
        }

        private void GenerarMovimientosLineasCostes(IEnumerable<TransformacionesentradaLinModel> lineas, TransformacionesModel nuevo, TipoOperacionService movimiento)
        {
            var movimientosStockService = new MovimientosstockService(_context, _db);
            var articulosService = FService.Instance.GetService(typeof(ArticulosModel), _context, _db) as ArticulosService;
            var serializer = new Serializer<TransformacionesentradaDiarioStockSerializable>();
            var vectorArticulos = new Hashtable();

            var operacion = 1;
            if (movimiento == TipoOperacionService.EliminarCostes)
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

                var aux = Funciones.ConverterGeneric<TransformacionesEntradaLinSerialized>(linea);

                if (articuloObj?.Gestionstock ?? false)
                {
                    var model = new MovimientosstockModel
                    {
                        Empresa = nuevo.Empresa,
                        Fkalmacenes = nuevo.Fkalmacen.ToString(),
                        Fkalmaceneszona = Funciones.Qint(nuevo.Fkzonas),
                        Fkarticulos = linea.Fkarticulos,
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
                            new TransformacionesentradaDiarioStockSerializable
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
    }
}
