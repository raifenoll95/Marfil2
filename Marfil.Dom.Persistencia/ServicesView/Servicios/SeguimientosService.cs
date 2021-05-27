using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Inf.ResourcesGlobalization.Textos.Entidades;
using Marfil.Dom.Persistencia.Model.CRM;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Validation;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Inf.Genericos.Helper;
using Resources;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System.Globalization;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{

    public interface ISeguimientosService
    {

    }


    public class SeguimientosService : GestionService<SeguimientosModel, Seguimientos>, ISeguimientosService
    {

        #region CTR

        public SeguimientosService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion


        #region List Index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var st = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var estadosService = new EstadosService(_context, _db);
            st.List = st.List.OfType<SeguimientosModel>().OrderByDescending(f => f.Fechadocumento).ThenByDescending(f => f.Id);
            var propiedadesVisibles = new[] { "Origen", "Fechadocumento", "Asunto", "Descripciontercero", "Fkaccion", "Fketapa" };
            var propiedades = Helpers.Helper.getProperties<SeguimientosModel>();
            st.PrimaryColumnns = new[] { "Id" };
            st.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            st.ColumnasCombo.Add("Fketapa", estadosService.GetStates(DocumentoEstado.Oportunidades, TipoMovimientos.Todos).Select(f => new Tuple<string, string>(f.CampoId, f.Descripcion)));           
            st.ColumnasCombo.Add("Fkaccion", _appService.GetListAcciones().Select(f => new Tuple<string, string>(f.Valor, f.Descripcion)));
            return st;
        }


        public override string GetSelectPrincipal()
        {
            var result = new StringBuilder();

            result.Append("SELECT s.*, c.descripcion AS [Descripciontercero]");
            result.Append(" FROM Seguimientos as s");
            result.Append(" LEFT JOIN Cuentas AS c ON c.empresa = s.empresa AND c.id = s.fkempresa");
            result.AppendFormat(" where s.empresa ='{0}' ", _context.Empresa);
            return result.ToString();
        }

        #endregion

        public int NextId()
        {
            return _db.Seguimientos.Any() ? _db.Seguimientos.Max(f => f.id) + 1 : 1;
        }


        public override void create(IModelView obj)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as SeguimientosModel;
                var currentValidationService = _validationService as SeguimientosValidation;
                var estadoFinalizado = new Estados();
                model.Id = NextId();
                
                if (model.Fechadocumento == null)
                    model.Fechadocumento = DateTime.Now;                            

                if (model.Tipo == (int)DocumentoEstado.Oportunidades)
                {
                    var modelPadre = _db.Oportunidades.Where(f => f.empresa == Empresa && f.referencia == model.Origen).FirstOrDefault();

                    if (model.Cerrado)
                    {
                        modelPadre.cerrado = true;
                        modelPadre.fechacierre = model.Fecharesolucion;
                        modelPadre.fkreaccion = model.Fkreaccion;
                        estadoFinalizado = _db.Estados.Where(f => f.documento == (int)DocumentoEstado.Oportunidades && f.tipoestado == (int)TipoEstado.Finalizado).SingleOrDefault()
                        ?? _db.Estados.Where(f => f.documento == (int)DocumentoEstado.Todos && f.tipoestado == (int)TipoEstado.Finalizado).SingleOrDefault();
                    }

                    modelPadre.fketapa = model.Fketapa;
                    modelPadre.fechaultimoseguimiento = model.Fechadocumento;
                    if (model.Fechaproximoseguimiento != null)
                        modelPadre.fechaproximoseguimiento = model.Fechaproximoseguimiento;

                    //Rai
                    //Se van totalizando los costes imputados de los seguimientos
                    modelPadre.coste += model.Coste;

                    var converterModel = FConverterModel.Instance.CreateConverterModelService<OportunidadesModel, Oportunidades>(_context, _db, Empresa);
                    var modelview = converterModel.GetModelView(modelPadre);

                    var service = FService.Instance.GetService(typeof(OportunidadesModel), _context);
                    service.edit(modelview);
                }
                else if (model.Tipo == (int)DocumentoEstado.Proyectos)
                {
                    var modelPadre = _db.Proyectos.Where(f => f.empresa == Empresa && f.referencia == model.Origen).FirstOrDefault();

                    if (model.Cerrado)
                    {
                        modelPadre.cerrado = true;
                        modelPadre.fechacierre = model.Fecharesolucion;
                        modelPadre.fkreaccion = model.Fkreaccion;
                        estadoFinalizado = _db.Estados.Where(f => f.documento == (int)DocumentoEstado.Proyectos && f.tipoestado == (int)TipoEstado.Finalizado).SingleOrDefault()
                        ?? _db.Estados.Where(f => f.documento == (int)DocumentoEstado.Todos && f.tipoestado == (int)TipoEstado.Finalizado).SingleOrDefault();
                    }

                    modelPadre.fketapa = model.Fketapa;
                    modelPadre.fechaultimoseguimiento = model.Fechadocumento;
                    if (model.Fechaproximoseguimiento != null)
                        modelPadre.fechaproximoseguimiento = model.Fechaproximoseguimiento;

                    //Rai
                    //Se van totalizando los costes imputados de los seguimientos
                    modelPadre.coste += model.Coste;

                    var converterModel = FConverterModel.Instance.CreateConverterModelService<ProyectosModel, Proyectos>(_context, _db, Empresa);
                    var modelview = converterModel.GetModelView(modelPadre);

                    var service = FService.Instance.GetService(typeof(ProyectosModel), _context);
                    service.edit(modelview);
                }
                else if (model.Tipo == (int)DocumentoEstado.Campañas)
                {
                    var modelPadre = _db.Campañas.Where(f => f.empresa == Empresa && f.referencia == model.Origen).FirstOrDefault();

                    if (model.Cerrado)
                    {
                        modelPadre.cerrado = true;
                        modelPadre.fechacierre = model.Fecharesolucion;
                        modelPadre.fkreaccion = model.Fkreaccion;
                        estadoFinalizado = _db.Estados.Where(f => f.documento == (int)DocumentoEstado.Campañas && f.tipoestado == (int)TipoEstado.Finalizado).SingleOrDefault()
                        ?? _db.Estados.Where(f => f.documento == (int)DocumentoEstado.Todos && f.tipoestado == (int)TipoEstado.Finalizado).SingleOrDefault();
                    }

                    modelPadre.fketapa = model.Fketapa;
                    modelPadre.fechaultimoseguimiento = model.Fechadocumento;
                    if (model.Fechaproximoseguimiento != null)
                        modelPadre.fechaproximoseguimiento = model.Fechaproximoseguimiento;

                    //Rai
                    //Se van totalizando los costes imputados de los seguimientos
                    modelPadre.coste += model.Coste;

                    var converterModel = FConverterModel.Instance.CreateConverterModelService<CampañasModel, Campañas>(_context, _db, Empresa);
                    var modelview = converterModel.GetModelView(modelPadre);

                    var service = FService.Instance.GetService(typeof(CampañasModel), _context);
                    service.edit(modelview);
                }
                else if (model.Tipo == (int)DocumentoEstado.Incidencias)
                {
                    var modelPadre = _db.IncidenciasCRM.Where(f => f.empresa == Empresa && f.referencia == model.Origen).FirstOrDefault();

                    if (model.Cerrado)
                    {
                        modelPadre.cerrado = true;
                        modelPadre.fechacierre = model.Fecharesolucion;
                        modelPadre.fkreaccion = model.Fkreaccion;
                        estadoFinalizado = _db.Estados.Where(f => f.documento == (int)DocumentoEstado.Incidencias && f.tipoestado == (int)TipoEstado.Finalizado).SingleOrDefault()
                        ?? _db.Estados.Where(f => f.documento == (int)DocumentoEstado.Todos && f.tipoestado == (int)TipoEstado.Finalizado).SingleOrDefault();
                    }

                    modelPadre.fketapa = model.Fketapa;
                    modelPadre.fechaultimoseguimiento = model.Fechadocumento;
                    if (model.Fechaproximoseguimiento != null)
                        modelPadre.fechaproximoseguimiento = model.Fechaproximoseguimiento;

                    //Rai
                    //Se van totalizando los costes imputados de los seguimientos
                    modelPadre.coste += model.Coste;

                    var converterModel = FConverterModel.Instance.CreateConverterModelService<IncidenciasCRMModel, IncidenciasCRM>(_context, _db, Empresa);
                    var modelview = converterModel.GetModelView(modelPadre);

                    var service = FService.Instance.GetService(typeof(IncidenciasCRMModel), _context);
                    service.edit(modelview);
                }

                var etapaAnterior = _db.Seguimientos.Where(f => f.empresa == model.Empresa && f.id == model.Id).Select(f => f.fketapa).SingleOrDefault();
                var estadoAnterior = 0;
                if (etapaAnterior != null)
                {
                    var s = etapaAnterior.Split('-');
                    var documento = Funciones.Qint(s[0]);
                    var id = s[1];
                    estadoAnterior = _db.Estados.Where(f => f.documento == documento && f.id == id).Select(f => f.tipoestado).SingleOrDefault() ?? 0;
                }

                if (model.Cerrado && (estadoAnterior != (int)TipoEstado.Finalizado && estadoAnterior != (int)TipoEstado.Caducado && estadoAnterior != (int)TipoEstado.Anulado))
                {
                    model.Fketapa = estadoFinalizado.documento + "-" + estadoFinalizado.id;
                    currentValidationService.CambiarEstado = true;
                }

                foreach (var linea in model.Correos)
                {
                    linea.Empresa = model.Empresa;
                    linea.Context = model.Context;
                    linea.Fkseguimientos = model.Id;
                    linea.Fkorigen = model.Origen;
                }

                base.create(obj);

                _db.SaveChanges();
                tran.Complete();
            }
        }

        public override void edit(IModelView obj)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as SeguimientosModel;
                var currentValidationService = _validationService as SeguimientosValidation;
                var estadoFinalizado = new Estados();

                if (model.Tipo == (int)DocumentoEstado.Oportunidades)
                {
                    var modelPadre = _db.Oportunidades.Where(f => f.empresa == Empresa && f.referencia == model.Origen).FirstOrDefault();

                    if (model.Cerrado)
                    {
                        modelPadre.cerrado = true;
                        modelPadre.fechacierre = model.Fecharesolucion;
                        modelPadre.fkreaccion = model.Fkreaccion;
                        estadoFinalizado = _db.Estados.Where(f => f.documento == (int)DocumentoEstado.Oportunidades && f.tipoestado == (int)TipoEstado.Finalizado).SingleOrDefault()
                        ?? _db.Estados.Where(f => f.documento == (int)DocumentoEstado.Todos && f.tipoestado == (int)TipoEstado.Finalizado).SingleOrDefault();
                    }

                    modelPadre.fketapa = model.Fketapa;
                    modelPadre.fechaultimoseguimiento = model.Fechadocumento;
                    if (model.Fechaproximoseguimiento != null)
                        modelPadre.fechaproximoseguimiento = model.Fechaproximoseguimiento;                    

                    var converterModel = FConverterModel.Instance.CreateConverterModelService<OportunidadesModel, Oportunidades>(_context, _db, Empresa);
                    var modelview = converterModel.GetModelView(modelPadre);

                    //Rai
                    //Hay que tener en cuenta que si se edita un seguimiento, podrían cambiarle el coste, 
                    //luego al grabar hay que restar el coste que tenga y sumar el que haya al grabarlo.
                    var antiguocosteSeguimiento = _db.Seguimientos.Where(f => f.empresa == Empresa && f.origen == model.Origen && f.id == model.Id).Select(f => f.coste).FirstOrDefault();
                    modelPadre.coste -= antiguocosteSeguimiento;
                    modelPadre.coste += model.Coste;

                    var service = FService.Instance.GetService(typeof(OportunidadesModel), _context);
                    service.edit(modelview);
                }
                else if (model.Tipo == (int)DocumentoEstado.Proyectos)
                {
                    var modelPadre = _db.Proyectos.Where(f => f.empresa == Empresa && f.referencia == model.Origen).FirstOrDefault();

                    if (model.Cerrado)
                    {
                        modelPadre.cerrado = true;
                        modelPadre.fechacierre = model.Fecharesolucion;
                        modelPadre.fkreaccion = model.Fkreaccion;
                        estadoFinalizado = _db.Estados.Where(f => f.documento == (int)DocumentoEstado.Proyectos && f.tipoestado == (int)TipoEstado.Finalizado).SingleOrDefault()
                        ?? _db.Estados.Where(f => f.documento == (int)DocumentoEstado.Todos && f.tipoestado == (int)TipoEstado.Finalizado).SingleOrDefault();
                    }

                    modelPadre.fketapa = model.Fketapa;
                    modelPadre.fechaultimoseguimiento = model.Fechadocumento;
                    if (model.Fechaproximoseguimiento != null)
                        modelPadre.fechaproximoseguimiento = model.Fechaproximoseguimiento;

                    var converterModel = FConverterModel.Instance.CreateConverterModelService<ProyectosModel, Proyectos>(_context, _db, Empresa);
                    var modelview = converterModel.GetModelView(modelPadre);

                    //Rai
                    //Hay que tener en cuenta que si se edita un seguimiento, podrían cambiarle el coste, 
                    //luego al grabar hay que restar el coste que tenga y sumar el que haya al grabarlo.
                    var antiguocosteSeguimiento = _db.Seguimientos.Where(f => f.empresa == Empresa && f.origen == model.Origen && f.id == model.Id).Select(f => f.coste).FirstOrDefault();
                    modelPadre.coste -= antiguocosteSeguimiento;
                    modelPadre.coste += model.Coste;

                    var service = FService.Instance.GetService(typeof(ProyectosModel), _context);
                    service.edit(modelview);
                }
                else if (model.Tipo == (int)DocumentoEstado.Campañas)
                {
                    var modelPadre = _db.Campañas.Where(f => f.empresa == Empresa && f.referencia == model.Origen).FirstOrDefault();

                    if (model.Cerrado)
                    {
                        modelPadre.cerrado = true;
                        modelPadre.fechacierre = model.Fecharesolucion;
                        modelPadre.fkreaccion = model.Fkreaccion;
                        estadoFinalizado = _db.Estados.Where(f => f.documento == (int)DocumentoEstado.Campañas && f.tipoestado == (int)TipoEstado.Finalizado).SingleOrDefault()
                        ?? _db.Estados.Where(f => f.documento == (int)DocumentoEstado.Todos && f.tipoestado == (int)TipoEstado.Finalizado).SingleOrDefault();
                    }

                    modelPadre.fketapa = model.Fketapa;
                    modelPadre.fechaultimoseguimiento = model.Fechadocumento;
                    if (model.Fechaproximoseguimiento != null)
                        modelPadre.fechaproximoseguimiento = model.Fechaproximoseguimiento;

                    var converterModel = FConverterModel.Instance.CreateConverterModelService<CampañasModel, Campañas>(_context, _db, Empresa);
                    var modelview = converterModel.GetModelView(modelPadre);

                    //Rai
                    //Hay que tener en cuenta que si se edita un seguimiento, podrían cambiarle el coste, 
                    //luego al grabar hay que restar el coste que tenga y sumar el que haya al grabarlo.
                    var antiguocosteSeguimiento = _db.Seguimientos.Where(f => f.empresa == Empresa && f.origen == model.Origen && f.id == model.Id).Select(f => f.coste).FirstOrDefault();
                    modelPadre.coste -= antiguocosteSeguimiento;
                    modelPadre.coste += model.Coste;

                    var service = FService.Instance.GetService(typeof(CampañasModel), _context);
                    service.edit(modelview);
                }
                else if (model.Tipo == (int)DocumentoEstado.Incidencias)
                {
                    var modelPadre = _db.IncidenciasCRM.Where(f => f.empresa == Empresa && f.referencia == model.Origen).FirstOrDefault();

                    if (model.Cerrado)
                    {
                        modelPadre.cerrado = true;
                        modelPadre.fechacierre = model.Fecharesolucion;
                        modelPadre.fkreaccion = model.Fkreaccion;
                        estadoFinalizado = _db.Estados.Where(f => f.documento == (int)DocumentoEstado.Incidencias && f.tipoestado == (int)TipoEstado.Finalizado).SingleOrDefault()
                        ?? _db.Estados.Where(f => f.documento == (int)DocumentoEstado.Todos && f.tipoestado == (int)TipoEstado.Finalizado).SingleOrDefault();
                    }                       

                    modelPadre.fketapa = model.Fketapa;
                    modelPadre.fechaultimoseguimiento = model.Fechadocumento;
                    if (model.Fechaproximoseguimiento != null)
                        modelPadre.fechaproximoseguimiento = model.Fechaproximoseguimiento;

                    var converterModel = FConverterModel.Instance.CreateConverterModelService<IncidenciasCRMModel, IncidenciasCRM>(_context, _db, Empresa);
                    var modelview = converterModel.GetModelView(modelPadre);

                    //Rai
                    //Hay que tener en cuenta que si se edita un seguimiento, podrían cambiarle el coste, 
                    //luego al grabar hay que restar el coste que tenga y sumar el que haya al grabarlo.
                    var antiguocosteSeguimiento = _db.Seguimientos.Where(f => f.empresa == Empresa && f.origen == model.Origen && f.id == model.Id).Select(f => f.coste).FirstOrDefault();
                    modelPadre.coste -= antiguocosteSeguimiento;
                    modelPadre.coste += model.Coste;

                    var service = FService.Instance.GetService(typeof(IncidenciasCRMModel), _context);
                    service.edit(modelview);
                }

                var etapaAnterior = _db.Seguimientos.Where(f => f.empresa == model.Empresa && f.id == model.Id).Select(f => f.fketapa).SingleOrDefault();
                var s = etapaAnterior.Split('-');
                var documento = Funciones.Qint(s[0]);
                var id = s[1];
                var estadoAnterior = _db.Estados.Where(f => f.documento == documento && f.id == id).Select(f => f.tipoestado).SingleOrDefault();

                if (model.Cerrado && (estadoAnterior != (int)TipoEstado.Finalizado && estadoAnterior != (int)TipoEstado.Caducado && estadoAnterior != (int)TipoEstado.Anulado))
                {                    
                    model.Fketapa = estadoFinalizado.documento + "-" + estadoFinalizado.id;
                    currentValidationService.CambiarEstado = true;
                }

                base.edit(obj);

                _db.SaveChanges();
                tran.Complete();
            }
        }

        public override void delete(IModelView obj)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as SeguimientosModel;

                var s = model.Fketapa.Split('-');
                var documento = Funciones.Qint(s[0]);
                var id = s[1];
                var tipoEstado = _db.Estados.Where(f => f.documento == documento && f.id == id).Select(f => f.tipoestado).SingleOrDefault();

                if (tipoEstado != (int)TipoEstado.Finalizado && tipoEstado != (int)TipoEstado.Caducado && tipoEstado != (int)TipoEstado.Anulado)
                {
                    base.delete(obj);
                    _db.SaveChanges();
                    tran.Complete();
                }
                else
                {
                    throw new ValidationException(General.ErrorDocumentoFinalizado);
                }
            }
        }


        //A partir del seguimiento, obtengo sus lineas de correo
        public List<SeguimientosCorreoModel> createLineasCorreos(SeguimientosModel model)
        {
            List<SeguimientosCorreoModel> seguimientos = new List<SeguimientosCorreoModel>();
            var seguimientosBD = _db.SeguimientosCorreo.Where(f => f.empresa == model.Empresa && f.fkseguimientos == model.Id && f.fkorigen == model.Origen).ToList();

            foreach(var seg in seguimientosBD)
            {
                seguimientos.Add(new SeguimientosCorreoModel
                {
                    Empresa = seg.empresa,
                    Fkseguimientos = seg.fkseguimientos,
                    Fkorigen = seg.fkorigen,
                    Id = seg.id,
                    Correo = seg.correo,
                    Asunto = seg.asunto,
                    Fecha = seg.fecha.Value       
                });
            }

            return seguimientos;
        }

        //Rai - Devuelve el coste del CRM en funcion de la accion y de la fecha
        public int getCosteCRM(string accion, string fecha)
        {
            var coste = 0;
            DateTime? dt = DateTime.ParseExact(fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            if(dt.HasValue)
            {
                var ejercicioId = _db.Ejercicios.Where(f => f.empresa == Empresa && f.desde <= dt && dt <= f.hasta).SingleOrDefault();
                var serviceCostesVariablePeriodo = FService.Instance.GetService(typeof(CostesVariablesPeriodoModel), _context);

                //Es posible que no se ha creado ningun coste variable con un ejercicio correspondiente a la fecha del seguimiento
                if (ejercicioId != null)
                {
                    var modelCostes = serviceCostesVariablePeriodo.get(ejercicioId.id.ToString()) as CostesVariablesPeriodoModel;
                    var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos));
                    var descripcionaccion = service.GetListAcciones().Where(f => f.Valor == accion).Select(f => f.Descripcion).SingleOrDefault().ToString();

                    foreach (var linea in modelCostes._costes)
                    {
                        if (linea.Descripcion == descripcionaccion)
                        {
                            coste = (int)linea.Precio;
                        }
                    }
                }     
            }

            return coste;
        }

        public IEnumerable<SeguimientosModel> getDocumentosRelacionados(string tipodocumento, string tercero)
        {
            return _db.Database.SqlQuery<SeguimientosModel>(string.Format(
            "SELECT t.referencia, FORMAT(t.fechadocumento, 'dd/MM/yyyy') AS Fecha, t.basetotal AS Baseimponible"
            + " FROM ("
            + " SELECT referencia, fechadocumento, fkseries, fkclientes AS [Tercero], pt.basetotal "
            + " FROM Presupuestos AS p"
            + " LEFT JOIN PresupuestosTotales AS pt ON pt.empresa = p.empresa AND pt.fkpresupuestos = p.id"
            + " UNION"
            + " SELECT referencia, fechadocumento, fkseries, fkproveedores AS [Tercero], pt.basetotal"
            + " FROM PresupuestosCompras AS p"
            + " LEFT JOIN PresupuestosComprasTotales AS pt ON pt.empresa = p.empresa AND pt.fkpresupuestoscompras = p.id"
            + " UNION"
            + " SELECT referencia, fechadocumento, fkseries, fkclientes AS [Tercero], pt.basetotal"
            + " FROM Pedidos AS p"
            + " LEFT JOIN PedidosTotales AS pt ON pt.empresa = p.empresa AND pt.fkpedidos = p.id"
            + " UNION"
            + " SELECT referencia, fechadocumento, fkseries, fkproveedores AS [Tercero], pt.basetotal"
            + " FROM PedidosCompras AS p"
            + " LEFT JOIN PedidosComprasTotales AS pt ON pt.empresa = p.empresa AND pt.fkpedidoscompras = p.id"
            + " UNION"
            + " SELECT referencia, fechadocumento, fkseries, fkclientes AS [Tercero], at.basetotal"
            + " FROM Albaranes AS a"
            + " LEFT JOIN AlbaranesTotales AS at ON at.empresa = a.empresa AND at.fkalbaranes = a.id"
            + " UNION"
            + " SELECT referencia, fechadocumento, fkseries, fkproveedores AS [Tercero], at.basetotal"
            + " FROM AlbaranesCompras AS a"
            + " LEFT JOIN AlbaranesComprasTotales AS at ON at.empresa = a.empresa AND at.fkalbaranes = a.id"
            + " UNION"
            + " SELECT referencia, fechadocumento, fkseries, fkclientes AS [Tercero], ft.basetotal"
            + " FROM Facturas AS f"
            + " LEFT JOIN FacturasTotales AS ft ON ft.empresa = f.empresa AND ft.fkfacturas = f.id"
            + " UNION"
            + " SELECT referencia, fechadocumento, fkseries, fkproveedores AS [Tercero], ft.basetotal"
            + " FROM FacturasCompras AS f"
            + " LEFT JOIN FacturasComprasTotales AS ft ON ft.empresa = f.empresa AND ft.fkfacturascompras = f.id"
            + " )t"
            + " LEFT JOIN Series s ON s.id = t.fkseries"
            + " WHERE s.tipodocumento = '" + tipodocumento + "' AND"
            + " [Tercero] = '" + tercero + "' AND"
            + " s.empresa = '" + Empresa + "'"));
        }

        public IEnumerable<int> BuscarIdDocumentoRelaciondo(string referencia)
        {            
            return  _db.Database.SqlQuery<int>(string.Format(
            "SELECT t.id"
            + " FROM("
            + " SELECT id, empresa, referencia, fkseries, fkclientes AS[Tercero]"
            + " FROM Presupuestos"
            + " UNION"
            + " SELECT id, empresa, referencia, fkseries, fkproveedores AS[Tercero]"
            + " FROM PresupuestosCompras"
            + " UNION"
            + " SELECT id, empresa, referencia, fkseries, fkclientes AS[Tercero]"
            + " FROM Pedidos"
            + " UNION"
            + " SELECT id, empresa, referencia, fkseries, fkproveedores AS[Tercero]"
            + " FROM PedidosCompras"
            + " UNION"
            + " SELECT id, empresa, referencia, fkseries, fkclientes AS[Tercero]"
            + " FROM Albaranes"
            + " UNION"
            + " SELECT id, empresa, referencia, fkseries, fkproveedores AS[Tercero]"
            + " FROM AlbaranesCompras"
            + " UNION"
            + " SELECT id, empresa, referencia, fkseries, fkclientes AS[Tercero]"
            + " FROM Facturas"
            + " UNION"
            + " SELECT id, empresa, referencia, fkseries, fkproveedores AS[Tercero]"
            + " FROM FacturasCompras"
            + " )t"
            + " WHERE referencia = '" + referencia + "' AND"
            + " empresa = '" + Empresa + "'"));
        }

    }

}
