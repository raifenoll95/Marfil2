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
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Documentos.PresupuestosCompras;
using Marfil.Dom.Persistencia.Model.Ficheros;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Converter;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Validation;
using Marfil.Inf.Genericos.Helper;
using Resources;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IPresupuestosComprasService
    {

    }

    public class PresupuestosComprasService : GestionService<PresupuestosComprasModel, PresupuestosCompras>, IDocumentosServices, IPresupuestosComprasService
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
                ((PresupuestosComprasValidation) _validationService).EjercicioId = value;
            }
        }

        #endregion

        #region CTR

        public PresupuestosComprasService(IContextService context, MarfilEntities db = null) : base(context, db)
        {
           
        }

        #endregion

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var st = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var estadosService=new EstadosService(_context,_db);
            st.List = st.List.OfType<PresupuestosComprasModel>().OrderByDescending(f => f.Fechadocumento).ThenByDescending(f => f.Referencia);
            var propiedadesVisibles = new[] { "Referencia", "Fechadocumento", "Fkproveedores", "Nombrecliente", "Fkestados", "Importebaseimponible" };
            var propiedades = Helpers.Helper.getProperties<PresupuestosComprasModel>();
            st.PrimaryColumnns = new[] { "Id" };
            st.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            st.ColumnasCombo.Add("Fkestados", estadosService.GetStates(DocumentoEstado.PresupuestosCompras, TipoMovimientos.Todos).Select(f=>new Tuple<string, string>(f.CampoId,f.Descripcion)));
            return st;
        }

        public IEnumerable<PresupuestosComprasLinModel> RecalculaLineas(IEnumerable<PresupuestosComprasLinModel> model,
            double descuentopp, double descuentocomercial, string fkregimeniva, double portes, int decimalesmoneda)
        {
            var result = new List<PresupuestosComprasLinModel>();

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

        public IEnumerable<PresupuestosComprasTotalesModel> Recalculartotales(IEnumerable<PresupuestosComprasLinModel> model, double descuentopp, double descuentocomercial, double portes, int decimalesmoneda)
        {
            var result = new List<PresupuestosComprasTotalesModel>();

            var vector = model.GroupBy(f => f.Fktiposiva);

            foreach (var item in vector)
            {
                var newItem = new PresupuestosComprasTotalesModel();
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

        public PresupuestosComprasModel Clonar(string id)
        {
            var appService=new ApplicationHelper(_context);
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                
                var obj = _converterModel.CreateView(id) as PresupuestosComprasModel;

                obj.Fechadocumento = DateTime.Now;
                obj.Fechavalidez = DateTime.Now.AddMonths(1);
                obj.Fkestados = appService.GetConfiguracion().Estadoinicial;
                foreach (var PresupuestosComprasLinModel in obj.Lineas)
                {
                    PresupuestosComprasLinModel.Cantidadpedida = 0;
                }
                var contador = ServiceHelper.GetNextId<PresupuestosCompras>(_db, Empresa, obj.Fkseries);
                var identificadorsegmento = "";
                obj.Referencia = ServiceHelper.GetReference<PresupuestosCompras>(_db, obj.Empresa, obj.Fkseries, contador, obj.Fechadocumento.Value, out identificadorsegmento);
                obj.Identificadorsegmento = identificadorsegmento;
                var newItem = _converterModel.CreatePersitance(obj);

                if (_validationService.ValidarGrabar(newItem))
                {//generar carpeta
                    PresupuestosComprasModel result;
                    result = _converterModel.GetModelView(newItem) as PresupuestosComprasModel;
                    //generar carpeta
                    DocumentosHelpers.GenerarCarpetaAsociada(result, TipoDocumentos.PresupuestosCompras, _context, _db);
                    newItem.fkcarpetas = result.Fkcarpetas;
                    _db.Set<PresupuestosCompras>().Add(newItem);
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

        public PresupuestosComprasModel GetByReferencia(string referencia)
        {
            var obj =
                _db.PresupuestosCompras.Include("PresupuestosComprasLin")
                    .Include("PresupuestosComprasTotales")
                    .Single(f => f.empresa == Empresa && f.referencia == referencia);

            return _converterModel.GetModelView(obj) as PresupuestosComprasModel;
        }

        public override void create(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as PresupuestosComprasModel;
                var validation = _validationService as PresupuestosComprasValidation;
                validation.EjercicioId = EjercicioId;

                //Calculo nuevo ID
                var contador = ServiceHelper.GetNextId<PresupuestosCompras>(_db, Empresa, model.Fkseries);
                var identificadorsegmento = "";
                model.Referencia = ServiceHelper.GetReference<PresupuestosCompras>(_db, model.Empresa, model.Fkseries, contador, model.Fechadocumento.Value, out identificadorsegmento);
                model.Identificadorsegmento = identificadorsegmento;

                DocumentosHelpers.GenerarCarpetaAsociada(model,TipoDocumentos.PresupuestosCompras, _context, _db);

                base.create(model);

                //generar carpeta
                tran.Complete();
            }
        }

       

        public override void edit(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var validation = _validationService as PresupuestosComprasValidation;
                validation.EjercicioId = EjercicioId;
                DocumentosHelpers.GenerarCarpetaAsociada(obj, TipoDocumentos.PresupuestosCompras, _context, _db);
                base.edit(obj);

               

                tran.Complete();
            }
                
        }

        public IEnumerable<PresupuestosComprasModel> BuscarPresupuestosComprasImportar(string cliente)
        {
            
              var list = _db.PresupuestosCompras.Include("PresupuestosComprasLin")
                    .Where(
                        f =>
                            f.empresa == Empresa && f.fkproveedores == cliente &&
                            f.PresupuestosComprasLin.Any(j => ((j.cantidad ?? 0) - (j.cantidadpedida ?? 0) > 0))).Join(_db.Estados, p => p.fkestados, e => e.documento + "-" + e.id, (a, b) => new { a, b }).ToList();
            return list.Where(f => f.b.tipoestado <= (int)TipoEstado.Curso).Select(f => _converterModel.GetModelView(f.a) as PresupuestosComprasModel).OrderByDescending(f => f.Id);
        }

        public IEnumerable<PresupuestosComprasLinImportarModel> BuscarLineasPresupuestosCompras(string cliente, string presupuestodesde, string presupuestohasta)
        {

            return _db.PresupuestosComprasLin.Include("PresupuestosCompras").Where(f => f.empresa == Empresa && f.PresupuestosCompras.fkproveedores == cliente && (string.IsNullOrEmpty(presupuestodesde) || (string.Compare(f.PresupuestosCompras.referencia, presupuestodesde, StringComparison.Ordinal) >= 0)) &&
                                                                         (string.IsNullOrEmpty(presupuestohasta) || (string.Compare(f.PresupuestosCompras.referencia, presupuestohasta, StringComparison.Ordinal) <= 0)) && ((f.cantidad??0) - (f.cantidadpedida??0) > 0)).Join(_db.Estados, c => c.PresupuestosCompras.fkestados, h => h.documento + "-" + h.id, (c, h) => new { Linea = c, Estado = h }).Where(x => x.Estado.tipoestado <= (int)TipoEstado.Curso).Select(y => y.Linea).OrderByDescending(f => f.id).ToList().Select(f => new PresupuestosComprasLinImportarModel()
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
                                                                             FkPresupuestosCompras = f.PresupuestosCompras.id,
                                                                             FkPresupuestosComprasreferencia = f.PresupuestosCompras.referencia,
                                                                             Fkmonedas = f.PresupuestosCompras.fkmonedas,
                                                                             Cabecera =  _converterModel.GetModelView(f.PresupuestosCompras) as PresupuestosComprasModel
                                                                         });

        }

        public void SetEstado(IModelView model, EstadosModel nuevoEstado)
        {
            var currentValidationService = _validationService as PresupuestosComprasValidation;

            currentValidationService.CambiarEstado = true;
            model.set("fkestados", nuevoEstado.CampoId);
            edit(model);
            currentValidationService.CambiarEstado = false;


        }

        
    }
}
