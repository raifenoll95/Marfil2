using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using Marfil.Dom.ControlsUI.Busquedas;
using Marfil.Dom.ControlsUI.BusquedaTerceros;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Documentos;
using Marfil.Dom.Persistencia.Model.Documentos.Presupuestos;
using Marfil.Dom.Persistencia.Model.Ficheros;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Converter;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos.BusquedasMovil;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Validation;
using Marfil.Inf.Genericos.Helper;
using Resources;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IPresupuestosService
    {

    }

    public class PresupuestosService : GestionService<PresupuestosModel, Presupuestos>, IDocumentosServices,IBuscarDocumento, IDocumentosVentasPorReferencia<PresupuestosModel>, IPresupuestosService
    {
        #region Members

        private string _ejercicioId;

        #endregion

        #region Properties

        public string EjercicioId
        {
            get { return _ejercicioId; }
            set
            {
                _ejercicioId = value;
                ((PresupuestosValidation) _validationService).EjercicioId = value;
            }
        }

        #endregion

        #region CTR

        public PresupuestosService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var st = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var estadosService=new EstadosService(_context,_db);
            st.List = st.List.OfType<PresupuestosModel>().OrderByDescending(f => f.Fechadocumento).ThenByDescending(f => f.Referencia);
            var propiedadesVisibles = new[] { "Referencia", "Fechadocumento", "Fkclientes", "Nombrecliente", "Fkestados", "Importebaseimponible" };
            var propiedades = Helpers.Helper.getProperties<PresupuestosModel>();
            st.PrimaryColumnns = new[] { "Id" };
            st.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            st.ColumnasCombo.Add("Fkestados", estadosService.GetStates(DocumentoEstado.PresupuestosVentas, TipoMovimientos.Todos).Select(f=>new Tuple<string, string>(f.CampoId,f.Descripcion)));
            return st;
        }

        public IEnumerable<PresupuestosLinModel> RecalculaLineas(IEnumerable<PresupuestosLinModel> model,
            double descuentopp, double descuentocomercial, string fkregimeniva, double portes, int decimalesmoneda)
        {
            var result = new List<PresupuestosLinModel>();

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

        public IEnumerable<PresupuestosTotalesModel> Recalculartotales(IEnumerable<PresupuestosLinModel> model, double descuentopp, double descuentocomercial, double portes, int decimalesmoneda)
        {
            var result = new List<PresupuestosTotalesModel>();

            var vector = model.GroupBy(f => f.Fktiposiva);

            foreach (var item in vector)
            {
                var newItem = new PresupuestosTotalesModel();
                var objIva = _db.TiposIva.Single(f => f.empresa == Empresa && f.id == item.Key);
                newItem.Decimalesmonedas = decimalesmoneda;
                newItem.Fktiposiva = item.Key;
                newItem.Porcentajeiva = objIva.porcentajeiva;
                newItem.Brutototal = Math.Round((item.Sum(f => (f.Metros * f.Precio)) - item.Sum(f => f.Importedescuento)) ?? 0, decimalesmoneda);
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

        public PresupuestosModel Clonar(string id)
        {
            var appService=new ApplicationHelper(_context);
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                
                var obj = _converterModel.CreateView(id) as PresupuestosModel;

                obj.Fechadocumento = DateTime.Now;
                obj.Fechavalidez = DateTime.Now.AddMonths(1);
                obj.Fkestados = appService.GetConfiguracion().Estadoinicial;
                foreach (var presupuestosLinModel in obj.Lineas)
                {
                    presupuestosLinModel.Cantidadpedida = 0;
                }
                var contador = ServiceHelper.GetNextId<Presupuestos>(_db, Empresa, obj.Fkseries);
                var identificadorsegmento = "";
                obj.Referencia = ServiceHelper.GetReference<Presupuestos>(_db, obj.Empresa, obj.Fkseries, contador, obj.Fechadocumento.Value, out identificadorsegmento);
                obj.Identificadorsegmento = identificadorsegmento;
                var newItem = _converterModel.CreatePersitance(obj);

                if (_validationService.ValidarGrabar(newItem))
                {//generar carpeta
                    PresupuestosModel result;
                    result = _converterModel.GetModelView(newItem) as PresupuestosModel;
                    //generar carpeta
                    DocumentosHelpers.GenerarCarpetaAsociada(result, TipoDocumentos.PresupuestosVentas, _context, _db);
                    newItem.fkcarpetas = result.Fkcarpetas;
                    _db.Set<Presupuestos>().Add(newItem);
                    try
                    {
                        _db.SaveChanges();
                        

                        tran.Complete();
                        result.Id= newItem.id;
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

        public PresupuestosModel GetByReferencia(string referencia)
        {
            var obj =
                _db.Presupuestos.Include("PresupuestosLin")
                    .Include("PresupuestosTotales")
                    .Single(f => f.empresa == Empresa && f.referencia == referencia);

            return _converterModel.GetModelView(obj) as PresupuestosModel;
        }

        public override void create(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as PresupuestosModel;
                var validation = _validationService as PresupuestosValidation;
                validation.EjercicioId = EjercicioId;

                //Calculo nuevo ID
                var contador = ServiceHelper.GetNextId<Presupuestos>(_db, Empresa, model.Fkseries);
                var identificadorsegmento = "";
                model.Referencia = ServiceHelper.GetReference<Presupuestos>(_db, model.Empresa, model.Fkseries, contador, model.Fechadocumento.Value, out identificadorsegmento);
                model.Identificadorsegmento = identificadorsegmento;

                // Obtener el pais del cliente(tercero)
                //var cuentasService = new CuentasService(_context);
                //var cuenta = cuentasService.get(model.Fkclientes) as Model.Configuracion.Cuentas.CuentasModel;

                //using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
                //{
                //    model.Clientepais = service.GetListPaises().Where(f => f.Valor == cuenta.FkPais).Select(f => f.Descripcion).SingleOrDefault();
                //}

                DocumentosHelpers.GenerarCarpetaAsociada(model,TipoDocumentos.PresupuestosVentas, _context, _db);

                base.create(model);

                //generar carpeta
                tran.Complete();
            }
        }

       

        public override void edit(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var validation = _validationService as PresupuestosValidation;
                validation.EjercicioId = EjercicioId;
                DocumentosHelpers.GenerarCarpetaAsociada(obj, TipoDocumentos.PresupuestosVentas, _context, _db);

                var model = obj as PresupuestosModel;

                // Obtener el pais del cliente(tercero)
                //var cuentasService = new CuentasService(_context);
                //var cuenta = cuentasService.get(model.Fkclientes) as Model.Configuracion.Cuentas.CuentasModel;

                //using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
                //{
                //    model.Clientepais = service.GetListPaises().Where(f => f.Valor == cuenta.FkPais).Select(f => f.Descripcion).SingleOrDefault();
                //}
                
                base.edit(model);
                tran.Complete();
            }
                
        }

        public IEnumerable<PresupuestosModel> BuscarPresupuestosImportar(string cliente)
        {
            
              var list = _db.Presupuestos.Include("PresupuestosLin")
                    .Where(
                        f =>
                            f.empresa == Empresa && f.fkclientes == cliente &&
                            f.PresupuestosLin.Any(j => ((j.cantidad ?? 0) - (j.cantidadpedida ?? 0) > 0))).Join(_db.Estados, p => p.fkestados, e => e.documento + "-" + e.id, (a, b) => new { a, b }).ToList();
            return list.Where(f => f.b.tipoestado <= (int)TipoEstado.Curso).Select(f => _converterModel.GetModelView(f.a) as PresupuestosModel).OrderByDescending(f => f.Id);
        }

        public IEnumerable<PresupuestosLinImportarModel> BuscarLineasPresupuestos(string cliente, string presupuestodesde, string presupuestohasta)
        {

            return _db.PresupuestosLin.Include("Presupuestos").Where(f => f.empresa == Empresa && f.Presupuestos.fkclientes == cliente && (string.IsNullOrEmpty(presupuestodesde) || (string.Compare(f.Presupuestos.referencia, presupuestodesde, StringComparison.Ordinal) >= 0)) &&
                                                                         (string.IsNullOrEmpty(presupuestohasta) || (string.Compare(f.Presupuestos.referencia, presupuestohasta, StringComparison.Ordinal) <= 0)) && ((f.cantidad??0) - (f.cantidadpedida??0) > 0)).Join(_db.Estados, c => c.Presupuestos.fkestados, h => h.documento + "-" + h.id, (c, h) => new { Linea = c, Estado = h }).Where(x => x.Estado.tipoestado <= (int)TipoEstado.Curso).Select(y => y.Linea).OrderByDescending(f => f.id).ToList().Select(f => new PresupuestosLinImportarModel()
                                                                         {
                                                                             Ancho = f.ancho,
                                                                             Canal = f.canal,
                                                                             Cantidad = (f.cantidad??0) - (f.cantidadpedida??0),
                                                                             Cantidadpedida = f.cantidadpedida,
                                                                             Cuotaiva = f.cuotaiva,
                                                                             Cuotarecargoequivalencia = f.cuotarecargoequivalencia,
                                                                             Decimalesmedidas = f.decimalesmedidas,
                                                                             Decimalesmonedas = f.decimalesmonedas,
                                                                             Descripcion = f.descripcion,
                                                                             Fkarticulos = f.fkarticulos,
                                                                             Fktiposiva = f.fktiposiva,
                                                                             Fkunidades = f.fkunidades,
                                                                             Grueso = f.grueso,
                                                                             Id = f.id,
                                                                             Importe = f.importe,
                                                                             Importedescuento = f.importedescuento,
                                                                             Largo = f.largo,
                                                                             Lote = f.lote,
                                                                             Metros = f.metros,
                                                                             Notas = f.notas,
                                                                             Porcentajedescuento = f.porcentajedescuento,
                                                                             Porcentajeiva = f.porcentajeiva,
                                                                             Porcentajerecargoequivalencia = f.porcentajerecargoequivalencia,
                                                                             Precio = f.precio,
                                                                             Precioanterior = f.precioanterior,
                                                                             Revision = f.revision,
                                                                             Tabla = f.tabla,
                                                                             Fkpresupuestos = f.Presupuestos.id,
                                                                             Fkpresupuestosreferencia = f.Presupuestos.referencia,
                                                                             Fkmonedas = f.Presupuestos.fkmonedas,
                                                                             Cabecera =  _converterModel.GetModelView(f.Presupuestos) as PresupuestosModel
                                                                         });

        }

        public void SetEstado(IModelView model, EstadosModel nuevoEstado)
        {
            var currentValidationService = _validationService as PresupuestosValidation;

            currentValidationService.CambiarEstado = true;
            model.set("fkestados", nuevoEstado.CampoId);
            edit(model);
            currentValidationService.CambiarEstado = false;


        }


        public IEnumerable<DocumentosBusqueda> Buscar(IDocumentosFiltros filtros, out int registrostotales)
        {
            var service= new BuscarDocumentosService(_db,Empresa);
            return service.Buscar(filtros, out registrostotales);
        }

        public IEnumerable<IItemResultadoMovile> BuscarDocumento(string referencia)
        {
            var service = new BuscarDocumentosService(_db, Empresa);
            return service.Get<PresupuestosModel,PresupuestosLinModel,PresupuestosTotalesModel>(this,referencia);
        }

        public IEnumerable<PresupuestosLinModel> articulosComponentes(string id)
        {
            var model = get(id) as PresupuestosModel;
            List<PresupuestosLinModel> lineas = new List<PresupuestosLinModel>();
            var componentes = _db.ArticulosComponentes.Where(f => f.empresa == Empresa).ToList();

            foreach(var articulo in model.Lineas)
            {
                if(componentes.Any(f => f.fkarticulo == articulo.Fkarticulos))
                {
                    lineas.Add(articulo);
                }
            }

            return lineas;
        }
    }
}
