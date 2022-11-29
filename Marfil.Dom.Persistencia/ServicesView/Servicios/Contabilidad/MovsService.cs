using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Contabilidad.Movs;
using Marfil.Dom.Persistencia.Model.Contabilidad.Maes;

using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Converter;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Contabilidad;

//using Marfil.Dom.Persistencia.ServicesView.Servicios.Contabilidad.Importar;

using Marfil.Dom.Persistencia.ServicesView.Servicios.Validation;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RMovs = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Movs;
using RFacturas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Facturas;

using System.Collections;
using System.Globalization;
using Marfil.Dom.Persistencia.Model.Documentos.Facturas;
using System.Web.Mvc;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using Marfil.Dom.Persistencia.Model.Documentos.FacturasCompras;
using Marfil.Dom.Persistencia.Model.Documentos.CobrosYPagos;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{


    public interface IMovsService
    {

    }

    public class MovsService : GestionService<MovsModel, Movs>,  IMovsService
    {

        #region Member

        private string _ejercicioId;
        //private IImportacionContableService _importarService;
        private IMaesService _maesService;
        #endregion

        #region Properties

        public string EjercicioId
        {
            get { return _ejercicioId; }
            set
            {
                _ejercicioId = value;
                ((MovsValidation)_validationService).EjercicioId = value;
            }
        }

        #endregion

        #region CTR

        public MovsService(IContextService context, MarfilEntities db = null) : base(context, db)
        {
            EjercicioId = context.Ejercicio;
            //_importarService = new ImportacionContableService(context);
           //_maesService = new MaesService(context);

        }

        #endregion

        #region List index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            ApplicationHelper app = new ApplicationHelper(_context);
            var st = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            st.List = st.List.OfType<MovsModel>().OrderByDescending(f => f.Referencia);
            var propiedadesVisibles = new[] {"Tipoasiento", "Referencia", "Fecha", "Documento_num","Descripcionasiento" };
            var propiedades = Helpers.Helper.getProperties<MovsModel>();
            st.PrimaryColumnns = new[] { "Id" };
            st.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            //st.ColumnasCombo.Add("Tipoasiento", Enum.GetValues(typeof(TipoAsientoContable)).OfType<TipoAsientoContable>().Select(f => new Tuple<string, string>(((int)f).ToString(), Funciones.GetEnumByStringValueAttribute(f))));
            st.ColumnasCombo.Add("Tipoasiento", app.GetListTiposAsientos().Select(f => new Tuple<string, string>(f.Descripcion, f.Descripcion)));            
            st.EstiloFilas.Add("Tipoasiento", new EstiloFilas() { Estilos = new[] { new Tuple<object, string>(2, "#FCF8E3"), new Tuple<object, string>(3, "#F2DEDE") } });
            return st;
        }

        public override string GetSelectPrincipal()
        {            
            //return string.Format("select * from Movs where empresa='{0}' and fkejercicio={1}", Empresa,EjercicioId );

            return "SELECT m.id, m.referencia, m.fecha, t.Descripcion AS [Tipoasiento], m.descripcionasiento" +
                    " FROM Movs AS m" +
                    " LEFT JOIN TiposAsientos AS t ON m.tipoasiento = t.Valor" +
                    " WHERE m.empresa = " + Empresa + " AND m.fkejercicio = " + EjercicioId;
        }
        // using (var cmd = new SqlCommand(string.Format("select max({2}) from {0} where fkseries=@fkseries and empresa=@empresa {1}",typeof(T).Name, GetCadenaFiltroContador(tipoinicio,fecha), columna), con))
        #endregion

        public IEnumerable<MovsLinModel> RecalculaLineas(IEnumerable<MovsLinModel> model, int decimalesmoneda)
        {
            var result = new List<MovsLinModel>();

            foreach (var item in model)
            {
                //if (item.Fkregimeniva != fkregimeniva)
                //{
                //    var tiposivaService = FService.Instance.GetService(typeof(TiposIvaModel), _context) as TiposivaService;
                //    var grupo = _db.Articulos.Single(f => f.empresa == Empresa && f.id == item.Fkarticulos);
                //    if (!grupo.tipoivavariable)
                //    {
                //        var ivaObj = tiposivaService.GetTipoIva(grupo.fkgruposiva, fkregimeniva);
                //        item.Fktiposiva = ivaObj.Id;
                //        item.Porcentajeiva = ivaObj.PorcentajeIva;
                //        item.Porcentajerecargoequivalencia = ivaObj.PorcentajeRecargoEquivalencia;
                //        item.Fkregimeniva = fkregimeniva;
                //    }

                //}

                result.Add(item);
            }

            return result;
        }

        public MovsModel GetByReferencia(string referencia)
        {
            var obj =
                _db.Movs.Include("MovsLin")
                    .Single(f => f.empresa == Empresa && f.referencia == referencia);

            ((MovsConverterService)_converterModel).Ejercicio = EjercicioId;
            return _converterModel.GetModelView(obj) as MovsModel;
        }

        public MovsModel GetById(int id)
        {
            var obj =
                _db.Movs.Include("MovsLin")
                    .Single(f => f.empresa == Empresa && f.id == id);

            ((MovsConverterService)_converterModel).Ejercicio = EjercicioId;
            return _converterModel.GetModelView(obj) as MovsModel;
        }

        public override IModelView get(string id)
        {
            ((MovsConverterService)_converterModel).Ejercicio = EjercicioId;
            return base.get(id);
        }

       

        #region Api main

        public override void create(IModelView obj)
        {


            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var maesService = new MaesService(_context, _db);
                var model = obj as MovsModel;

                //Calculo ID
                var contador = ServiceHelper.GetNextIdContable<Movs>(_db, Empresa, model.Fkseriescontables);
                var identificadorsegmento = "";
                model.Referencia = ServiceHelper.GetReferenceContable<Movs>(_db, model.Empresa, model.Fkseriescontables, contador, model.Fecha.Value, out identificadorsegmento);
                model.Identificadorsegmento = identificadorsegmento;

                base.create(obj);

                maesService.GenerarMovimiento(model, TipoOperacionMaes.Alta);
                _db.SaveChanges();
                tran.Complete();
                
            }
        }

        public override void edit(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var original = get(Funciones.Qnull(obj.get("id"))) as MovsModel;
                var editado = obj as MovsModel;

                if (original.Integridadreferencial == editado.Integridadreferencial)
                {
                    var validation = _validationService as MovsValidation;
                    validation.EjercicioId = EjercicioId;

                    base.edit(obj);


                    ActualizarLineasMaes(original, editado);

                    _db.SaveChanges();
                    tran.Complete();
                }
                else throw new IntegridadReferencialException(string.Format(General.ErrorIntegridadReferencial, RMovs.TituloEntidad, original.Referencia));

            }

        }


        public override void delete(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                //Nov2021 - Cambio legal, no se permite borrar asientos
                var model = obj as MovsModel;
                var modelAnul = obj as MovsModel;

                //Ene2022 - Si el asiento es Normal - F1 o Contabilizadas - F4 se anula, si no se elimina
                if (model.Tipoasiento != "F1" || model.Tipoasiento != "F4")
                {
                    base.delete(obj);
                    var maesService = new MaesService(_context, _db);
                    maesService.GenerarMovimiento(model, TipoOperacionMaes.Baja);
                }
                else
                {
                    var serviceConfig = new ConfiguracionService(_context, _db);
                    var invertir = serviceConfig.GetInvertirAsiento();

                    if (invertir == "false")
                    {
                        foreach (var item in modelAnul.Lineas)
                        {
                            item.Debe = item.Debe * -1;
                            item.Haber = item.Haber * -1;
                        }
                        modelAnul.Debe = modelAnul.Debe * -1;
                        modelAnul.Haber = modelAnul.Haber * -1;
                    }
                    else
                    {
                        foreach (var item in modelAnul.Lineas)
                        {
                            var debeLin = item.Debe;
                            var haberLin = item.Haber;

                            item.Debe = haberLin;
                            item.Haber = debeLin;
                        }
                        var debe = modelAnul.Debe;
                        var haber = modelAnul.Haber;

                        modelAnul.Debe = haber;
                        modelAnul.Haber = debe;

                    }

                    modelAnul.Id = _db.Movs.Where(f => f.empresa == Empresa).Select(f => f.id).Max() + 1;
                    modelAnul.Fecha = model.Fecha;
                    modelAnul.Descripcionasiento = "ANULACIÓN " + model.Descripcionasiento;

                    create(modelAnul);
                }
                //Nov2021

                //cambiar estado factura relacionado
                if (model.Traza != null && model.Traza != "")
                {
                    var serie = model.Traza.Split('-')[0];
                    var tipoDocumento = _db.Series.Where(f => f.empresa == Empresa && f.id == serie).Select(f => f.tipodocumento).SingleOrDefault();

                    if (tipoDocumento == "FRA")
                    {
                        var service = FService.Instance.GetService(typeof(FacturasModel), _context) as FacturasService;
                        var idFactura = _db.Facturas.Where(f => f.empresa == Empresa && f.referencia == model.Traza).Select(f => f.id).SingleOrDefault().ToString();

                        var _converterModel = FConverterModel.Instance.CreateConverterModelService<FacturasModel, Facturas>(_context, _db, Empresa);
                        var modelview = _converterModel.CreateView(idFactura);

                        var serviceEstados = new EstadosService(_context);
                        var listEstadosInicial = serviceEstados.GetStates(DocumentoEstado.FacturasVentas, Model.Configuracion.TipoEstado.Diseño).Where(f => f.Tipoestado == Model.Configuracion.TipoEstado.Diseño);
                        var nuevoEstado = listEstadosInicial.Where(f => f.Documento == DocumentoEstado.FacturasVentas).SingleOrDefault() ?? listEstadosInicial.First();

                        service.SetEstado(modelview, nuevoEstado);
                    }
                    else
                    {
                        var service = FService.Instance.GetService(typeof(FacturasComprasModel), _context) as FacturasComprasService;
                        var idFactura = _db.FacturasCompras.Where(f => f.empresa == Empresa && f.referencia == model.Traza).Select(f => f.id).SingleOrDefault().ToString();

                        var _converterModel = FConverterModel.Instance.CreateConverterModelService<FacturasComprasModel, FacturasCompras>(_context, _db, Empresa);
                        var modelview = _converterModel.CreateView(idFactura);

                        var serviceEstados = new EstadosService(_context);
                        var listEstadosInicial = serviceEstados.GetStates(DocumentoEstado.FacturasCompras, Model.Configuracion.TipoEstado.Diseño).Where(f => f.Tipoestado == Model.Configuracion.TipoEstado.Diseño);
                        var nuevoEstado = listEstadosInicial.Where(f => f.Documento == DocumentoEstado.FacturasCompras).SingleOrDefault() ?? listEstadosInicial.First();

                        service.SetEstado(modelview, nuevoEstado);
                    }
                }
                
                _db.SaveChanges();
                tran.Complete();
            }

        }
        #endregion


        #region Movimientos maes
        private void ActualizarLineasMaes(MovsModel original, MovsModel nuevo)
        {
            GenerarMovimientosMaes( original, TipoOperacionMaes.Baja);
            GenerarMovimientosMaes(nuevo, TipoOperacionMaes.Alta);
        }

        private void GenerarMovimientosMaes( MovsModel movsmodel, TipoOperacionMaes operacion)
        {
            var maesService = FService.Instance.GetService(typeof(MaesModel), _context, _db) as MaesService;
            maesService.GenerarMovimiento(movsmodel, operacion);
        }
        #endregion Movimientos maes



        #region Importar

        public void Importar(DataTable dt, string serieContable, int idPeticion, IContextService context)
        {
            string errores = "";

            try
            {
                // Ordenar por Referencia
                DataView dv = dt.DefaultView;
                dv.Sort = "Referencia asc";
                DataTable sorted = dv.ToTable();

                List<MovsModel> ListaMovs = new List<MovsModel>();
                MovsModel documento = new FModel().GetModel<MovsModel>(context);

                foreach (DataRow row in sorted.Rows)
                {
                    // Referencia, a quien pertenece el MovsLin a añadir
                    var referenciacsv = row["Referencia"].ToString();       
                    var cabecera = ListaMovs.Where(f => f.Referencialibre == referenciacsv).SingleOrDefault();

                    // Crear documento si no existe
                    if (cabecera == null)
                    {
                        // Crear cabecera/documento 
                        documento = new FModel().GetModel<MovsModel>(context);
                        documento.Referencialibre = referenciacsv;
                        if (referenciacsv != "")
                        {
                            documento.Referencialibreint = int.Parse(referenciacsv);
                        }                       

                        // Tipo de asiento T.V 72
                        // F1 Normal en blanco
                        // F2 Simulación 
                        // F3 Asiento vinculado -> V
                        // R1 Apertura provisional -> B
                        // R2 Apertura -> A
                        // R3 Regularización existencias -> E
                        // R4 Regularización grupos 6 y 7 -> P
                        // R5 Cierre -> C
                        switch (row["TipoAsiento"].ToString())
                        {
                            case "V":
                                documento.Tipoasiento = "F3";
                                break;
                            case "B":
                                documento.Tipoasiento = "R1";
                                break;
                            case "A":
                                documento.Tipoasiento = "R2";
                                break;
                            case "E":
                                documento.Tipoasiento = "R3";
                                break;
                            case "P":
                                documento.Tipoasiento = "R4";
                                break;
                            case "C":
                                documento.Tipoasiento = "R5";
                                break;
                            default:
                                documento.Tipoasiento = "F1";
                                break;
                        }

                        DateTime fecha;

                        if (DateTime.TryParseExact(row["Fecha"].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
                        {
                            documento.Fecha = fecha;
                        }
                        else
                        {
                            errores += referenciacsv + ";" + row["Fecha"].ToString() + " " + "La fecha no se ha podido convertir";
                            continue;
                        }
                        //documento.Fecha = DateTime.TryParse(row["Fecha"].ToString());                                   
                        documento.Fkseriescontables = serieContable;
                        documento.Fkmonedas = _db.Empresas.Where(f => f.id == documento.Empresa).Select(f => f.fkMonedabase).SingleOrDefault();
                        documento.Decimalesmonedas = 2;
                        documento.Descripcionasiento = "Importación - " + referenciacsv;
                        ListaMovs.Add(documento);
                        cabecera = documento;
                    }

                    // MovsLin
                    MovsLinModel linea = new MovsLinModel();
                    linea.Id = documento.Lineas.Count + 1;

                    // Existe la cuenta?
                    var cuenta = row["Fkcuentas"].ToString();
                    var fkcuentas = _db.Cuentas.Where(f => f.empresa == documento.Empresa && f.id == cuenta).Select(f => f.id).SingleOrDefault();
                    if (string.IsNullOrEmpty(fkcuentas))
                    {
                        // Hay que crear la cuenta con descripción = * ALTA DE CUENTA AUTOMATICA                             
                    }

                    linea.Fkcuentas = cuenta;

                    // Esdebe en Marfil antiguo es T o F             
                    if (row["Esdebe"].Equals("T"))
                    {
                        linea.Esdebe = 1;
                        //linea.SDebe = row["Importe"].ToString();
                    }
                    else if (row["Esdebe"].Equals("F"))
                    {
                        linea.Esdebe = -1;
                        //linea.SHaber = row["Importe"].ToString();
                    }

                    linea.Importe = decimal.Parse(row["Importe"].ToString().Replace('.', ','), CultureInfo.CreateSpecificCulture("es-ES"));
                    //linea.Importe = Convert.ToDecimal(row["Importe"].ToString().Replace('.', ','));
                    linea.Comentario = row["Comentario"].ToString();

                    // Secciones analíticas
                    var seccion = row["Secc"].ToString().PadLeft(4, '0').ToUpper();
                    var existeSeccion = _db.Seccionesanaliticas.Where(f => f.empresa == documento.Empresa && f.id == seccion).Select(f => f.id).SingleOrDefault();
                    linea.Fkseccionesanaliticas = existeSeccion;

                    cabecera.Lineas.Add(linea);
                }

                // Ordenados por lista del csv
                List<MovsModel> ListaMovsOrdenados = ListaMovs.OrderBy(f => f.Referencialibreint).ToList();

                foreach (var mov in ListaMovsOrdenados)
                {
                    try
                    {
                        create(mov);
                    }
                    catch (Exception ex)
                    {
                        errores += mov.Referencialibre + " " +ex.Message + ";" + ex.InnerException?.InnerException.Message;
                        
                        Exception except=ex.InnerException;
                        while (except!=null)
                        {
                            errores += except.Message +";";
                            except = except.InnerException;
                        }
                        errores += Environment.NewLine;
                        //for (var j = 0; i < ListaMovs[i].Lineas.Count; i++)
                        //{
                        //    errores += ListaMovs[i].Referencialibre + ";" + Convert.ToDateTime(ListaMovs[i].Fecha).ToString("dd/MM/yyyy") + ";" + ListaMovs[i].Lineas[j].Esdebe + ";" + ListaMovs[i].Lineas[j].Fkcuentas + ";"
                        //        + ListaMovs[i].Lineas[j].Importe + ";" + ListaMovs[i].Lineas[j].Comentario + ";" + ListaMovs[i].Lineas[j].Fkseccionesanaliticas + ";" + ex.Message + Environment.NewLine;
                        //}
                    }
                }

                var item = _db.PeticionesAsincronas.Where(f => f.empresa == context.Empresa && f.id == idPeticion).SingleOrDefault();

                item.estado = (int)EstadoPeticion.Finalizada;
                item.resultado = errores;

                _db.PeticionesAsincronas.AddOrUpdate(item);
                _db.SaveChanges();

            }
            catch (Exception ex)
            {
                errores += ex.Message + ";" + ex.InnerException?.InnerException.Message + Environment.NewLine;
                throw new ValidationException(errores);
            }

            //throw new ValidationException(errores);
        }

        public int CrearPeticionImportacion(IContextService context)
        {
            var item = _db.PeticionesAsincronas.Create();

            item.empresa = context.Empresa;
            item.id = _db.PeticionesAsincronas.Any() ? _db.PeticionesAsincronas.Max(f => f.id) + 1 : 1;
            item.usuario = context.Usuario;
            item.fecha = DateTime.Today;
            item.estado = (int)EstadoPeticion.EnCurso;
            item.tipo = (int)TipoPeticion.Importacion;
            item.configuracion = (((int)TipoImportacion.ImportarMovs).ToString() + "-").ToString();            

            _db.PeticionesAsincronas.AddOrUpdate(item);
            _db.SaveChanges();

            return item.id;
        }

        #endregion

        #region Exportar

        public void Exportar()
        {
            var sb = new StringBuilder();                        
            int idEjercicio = Convert.ToInt32(EjercicioId);
            IEnumerable<Movs> movsList = _db.Movs.Include("MovsLin").Where(f => f.empresa == Empresa && f.fkejercicio == idEjercicio);
            
            foreach (var documento in movsList)
            {
                foreach (var linea in documento.MovsLin)
                {
                    string s = "";

                    // Cuenta
                    s += linea.fkcuentas.PadRight(12, ' ');

                    // Fecha                                        
                    s += documento.fecha.Value.ToString("ddMMyyyy").ToString().PadRight(8, ' ');                                     

                    // Documento
                    s += (documento.traza ?? string.Empty).PadRight(12, ' ');                    

                    // Departamento
                    s += string.Empty.PadRight(8, ' ');

                    // Número de asiento
                    s += documento.referencia.Split('-')[1].PadLeft(6, '0');

                    // Importe
                    s += numericFormatExport((double)linea.importe, 10, 2);
                    
                    // Descripción
                    //s += (linea.comentario ?? string.Empty).PadRight(30, ' ');
                    s += fixedLength((linea.comentario ?? string.Empty).Replace("\r\n", " ").Replace("\n", " "), 30);

                    // Tipo de apunte
                    if (linea.esdebe == 1)
                    {
                        s += "D";
                    }
                    else
                    {
                        s += "H";
                    }
                    
                    // Reservado
                    s += string.Empty.PadRight(1, ' ');

                    // Sección
                    s += (linea.fkseccionesanaliticas ?? string.Empty).PadLeft(4, '0');

                    sb.Append(s);
                    sb.Append(Environment.NewLine);
                }                         
            }
            //Get Current Response  
            var response = System.Web.HttpContext.Current.Response;
            response.BufferOutput = true;
            response.Clear();
            response.ClearHeaders();
            response.ContentEncoding = Encoding.GetEncoding("ISO-8859-1");
            response.AddHeader("content-disposition", "attachment;filename=APUNT000.ASC");
            response.ContentType = "text/plain";
            response.Write(sb.ToString());
            response.End();
        }       

        public void ExportarIVA()
        {            
            var sb = new StringBuilder();                        
            int idEjercicio = Convert.ToInt32(EjercicioId);
            var configuracionService = new ConfiguracionService(_context, _db);
            var configuracionModel = configuracionService.GetModel();

            IEnumerable<Facturas> facturasList = _db.Facturas.Include("FacturasTotales").Where(f => f.empresa == Empresa && f.fkejercicio == idEjercicio && f.fkestados == configuracionModel.Estadofacturasventastotal);
            IEnumerable<FacturasCompras> facturasComprasList = _db.FacturasCompras.Include("FacturasComprasTotales").Where(f => f.empresa == Empresa && f.fkejercicio == idEjercicio && f.fkestados == configuracionModel.Estadofacturascomprastotal);

            foreach (var factura in facturasList)
            {
                string s = "";

                // Cuenta
                s += factura.fkclientes.PadRight(12, ' ');

                // Razón social
                s += fixedLength(factura.nombrecliente, 40);

                // Nombre comercial
                s += fixedLength(factura.nombrecliente, 30);

                // Dirección
                s += fixedLength((factura.clientedireccion ?? string.Empty).Replace("\r\n", " ").Replace("\n", " "), 30);

                // Población                    
                s += fixedLength(factura.clientepoblacion ?? string.Empty, 30);

                // Código postal
                s += fixedLength(factura.clientecp ?? string.Empty, 5);

                // Provincia
                s += fixedLength(factura.clienteprovincia ?? string.Empty, 20);

                // Teléfono
                s += fixedLength(factura.clientetelefono ?? string.Empty, 20);

                // Fax
                s += fixedLength(factura.clientefax ?? string.Empty, 15);

                // DNI
                s += fixedLength(factura.clientenif, 15);

                // Número asiento / Buscar campo traza en movs
                s += (_db.Movs.Where(f => f.empresa == Empresa && f.fkejercicio == idEjercicio && f.traza == factura.referencia).Select(f => f.identificadorsegmento).SingleOrDefault() ?? string.Empty).PadLeft(6, '0');

                // Registro de iva
                s += string.Empty.PadLeft(6, '0');

                // Fecha factura (Fecha documento proveedor, sólo en facturas compras)
                s += factura.fechadocumento.Value.ToString("ddMMyyyy").ToString();

                // Fecha contable (fecha documento)
                s += factura.fechadocumento.Value.ToString("ddMMyyyy").ToString();

                // Número de factura
                s += factura.referencia.PadRight(12, ' ');

                // Tipo de IVA (Exportar como D = Deducible)
                s += "D";

                // Soportado / Repercutido (S/R)
                s += "R";

                // Base imponible 1
                // % de Iva 1
                // Cuota de Iva 1
                // % Recargo 1
                // Cuota de Recargo 1
                if (factura.FacturasTotales.Count >= 1)
                {
                    s += numericFormatExport(factura.FacturasTotales.ElementAt(0).basetotal ?? 0, 10, 2);
                    s += numericFormatExport(factura.FacturasTotales.ElementAt(0).porcentajeiva ?? 0, 2, 0);
                    s += numericFormatExport(factura.FacturasTotales.ElementAt(0).cuotaiva ?? 0, 10, 2);
                    s += numericFormatExport(factura.FacturasTotales.ElementAt(0).porcentajerecargoequivalencia ?? 0, 1, 1);
                    s += numericFormatExport(factura.FacturasTotales.ElementAt(0).importerecargoequivalencia ?? 0, 8, 2);
                }
                else
                {
                    s += string.Empty.PadLeft(38, ' ');
                }

                // Base imponible 2
                // % de Iva 2
                // Cuota de Iva 2
                // % Recargo 2
                // Cuota de Recargo 2
                if (factura.FacturasTotales.Count >= 2)
                {
                    s += numericFormatExport(factura.FacturasTotales.ElementAt(1).basetotal ?? 0, 10, 2);
                    s += numericFormatExport(factura.FacturasTotales.ElementAt(1).porcentajeiva ?? 0, 2, 0);
                    s += numericFormatExport(factura.FacturasTotales.ElementAt(1).cuotaiva ?? 0, 10, 2);
                    s += numericFormatExport(factura.FacturasTotales.ElementAt(1).porcentajerecargoequivalencia ?? 0, 1, 1);
                    s += numericFormatExport(factura.FacturasTotales.ElementAt(1).importerecargoequivalencia ?? 0, 8, 2);
                }
                else
                {
                    s += string.Empty.PadLeft(38, ' ');
                }                

                // Base imponible 3
                // % de Iva 3
                // Cuota de Iva 3
                // % Recargo 3
                // Cuota de Recargo 3
                if (factura.FacturasTotales.Count == 3)
                {
                    s += numericFormatExport(factura.FacturasTotales.ElementAt(2).basetotal ?? 0, 10, 2);
                    s += numericFormatExport(factura.FacturasTotales.ElementAt(2).porcentajeiva ?? 0, 2, 0);
                    s += numericFormatExport(factura.FacturasTotales.ElementAt(2).cuotaiva ?? 0, 10, 2);
                    s += numericFormatExport(factura.FacturasTotales.ElementAt(2).porcentajerecargoequivalencia ?? 0, 1, 1);
                    s += numericFormatExport(factura.FacturasTotales.ElementAt(2).importerecargoequivalencia ?? 0, 8, 2);
                }
                else
                {
                    s += string.Empty.PadLeft(38, ' ');
                }

                // % IRPF
                s += numericFormatExport(factura.porcentajeretencion ?? 0, 2, 0);

                // Cuota IRPF / Aún no está implementado el campo
                //s += string.Empty.PadLeft(12, '0');
                double cuotaIRPF = 0;
                for (var i = 0; i < factura.FacturasTotales.Count; i++)
                {
                    cuotaIRPF += factura.FacturasTotales.ElementAt(i).importeretencion ?? 0;
                }
                s += numericFormatExport(cuotaIRPF, 10, 2);

                // CodPais
                s += string.Empty.PadRight(2, ' ');

                // Reservado
                s += string.Empty.PadRight(48, ' ');

                // CuentaCV // Buscar en TiposIva        
                var regimeniva = factura.fkregimeniva;
                var codArticulo = factura.FacturasLin.First().fkarticulos;
                var guiaContable = _db.Articulos.Where(f => f.empresa == Empresa && f.id == codArticulo).Select(f => f.fkguiascontables).SingleOrDefault();
                s += (_db.GuiascontablesLin.Where(f => f.empresa == Empresa && f.fkguiascontables == guiaContable && f.fkregimeniva == regimeniva).Select(f => f.fkcuentasventas)
                    .SingleOrDefault() ?? _db.Guiascontables.Where(f => f.empresa == Empresa && f.id == guiaContable).Select(f => f.fkcuentasventas).SingleOrDefault()).PadRight(12, ' ');

                // Pagado / Cobrado (S/N)
                s += "N";

                // Cobrador      
                s += (factura.fkcuentastesoreria ?? string.Empty).PadRight(12, ' ');

                sb.Append(s);
                sb.Append(Environment.NewLine);
            }

            foreach (var factura in facturasComprasList)
            {
                string s = "";

                // Cuenta
                s += factura.fkproveedores.PadRight(12, ' ');

                // Razón social
                s += fixedLength(factura.nombrecliente, 40);

                // Nombre comercial
                s += fixedLength(factura.nombrecliente, 30);

                // Dirección
                s += fixedLength((factura.clientedireccion ?? string.Empty).Replace("\r\n", " ").Replace("\n", " "), 30);

                // Población                    
                s += fixedLength(factura.clientepoblacion ?? string.Empty, 30);

                // Código postal
                s += fixedLength(factura.clientecp ?? string.Empty, 5);

                // Provincia
                s += fixedLength(factura.clienteprovincia ?? string.Empty, 20);

                // Teléfono
                s += fixedLength(factura.clientetelefono ?? string.Empty, 20);

                // Fax
                s += fixedLength(factura.clientefax ?? string.Empty, 15);

                // DNI
                s += fixedLength(factura.clientenif, 15);

                // Número asiento / Buscar campo traza en movs
                s += (_db.Movs.Where(f => f.empresa == Empresa && f.fkejercicio == idEjercicio && f.traza == factura.referencia).Select(f => f.identificadorsegmento).SingleOrDefault() ?? string.Empty).PadLeft(6, '0');

                // Registro de iva
                s += string.Empty.PadLeft(6,'0');

                // Fecha factura (Fecha documento proveedor, sólo en facturas compras)
                s += factura.fechadocumentoproveedor.Value.ToString("ddMMyyyy").ToString();

                // Fecha contable (fecha documento)
                s += factura.fechadocumento.Value.ToString("ddMMyyyy").ToString();

                // Número de factura
                s += factura.referencia.PadRight(12, ' ');

                // Tipo de IVA (Exportar como D = Deducible)
                s += "D";

                // Soportado / Repercutido (S/R)
                s += "S";

                // Base imponible 1
                // % de Iva 1
                // Cuota de Iva 1
                // % Recargo 1
                // Cuota de Recargo 1
                if (factura.FacturasComprasTotales.Count >= 1)
                {
                    s += numericFormatExport(factura.FacturasComprasTotales.ElementAt(0).basetotal ?? 0, 10, 2);
                    s += numericFormatExport(factura.FacturasComprasTotales.ElementAt(0).porcentajeiva ?? 0, 2, 0);
                    s += numericFormatExport(factura.FacturasComprasTotales.ElementAt(0).cuotaiva ?? 0, 10, 2);
                    s += numericFormatExport(factura.FacturasComprasTotales.ElementAt(0).porcentajerecargoequivalencia ?? 0, 1, 1);
                    s += numericFormatExport(factura.FacturasComprasTotales.ElementAt(0).importerecargoequivalencia ?? 0, 8, 2);
                }
                else
                {
                    s += string.Empty.PadLeft(38, ' ');
                }

                // Base imponible 2
                // % de Iva 2
                // Cuota de Iva 2
                // % Recargo 2
                // Cuota de Recargo 2
                if (factura.FacturasComprasTotales.Count >= 2)
                {
                    s += numericFormatExport(factura.FacturasComprasTotales.ElementAt(1).basetotal ?? 0, 10, 2);
                    s += numericFormatExport(factura.FacturasComprasTotales.ElementAt(1).porcentajeiva ?? 0, 2, 0);
                    s += numericFormatExport(factura.FacturasComprasTotales.ElementAt(1).cuotaiva ?? 0, 10, 2);
                    s += numericFormatExport(factura.FacturasComprasTotales.ElementAt(1).porcentajerecargoequivalencia ?? 0, 1, 1);
                    s += numericFormatExport(factura.FacturasComprasTotales.ElementAt(1).importerecargoequivalencia ?? 0, 8, 2);
                }
                else
                {
                    s += string.Empty.PadLeft(38, ' ');
                }

                // Base imponible 3
                // % de Iva 3
                // Cuota de Iva 3
                // % Recargo 3
                // Cuota de Recargo 3
                if (factura.FacturasComprasTotales.Count == 3)
                {
                    s += numericFormatExport(factura.FacturasComprasTotales.ElementAt(2).basetotal ?? 0, 10, 2);
                    s += numericFormatExport(factura.FacturasComprasTotales.ElementAt(2).porcentajeiva ?? 0, 2, 0);
                    s += numericFormatExport(factura.FacturasComprasTotales.ElementAt(2).cuotaiva ?? 0, 10, 2);
                    s += numericFormatExport(factura.FacturasComprasTotales.ElementAt(2).porcentajerecargoequivalencia ?? 0, 1, 1);
                    s += numericFormatExport(factura.FacturasComprasTotales.ElementAt(2).importerecargoequivalencia ?? 0, 8, 2);
                }
                else
                {
                    s += string.Empty.PadLeft(38, ' ');
                }

                // % IRPF
                s += numericFormatExport(factura.porcentajeretencion ?? 0, 2, 0);

                // Cuota IRPF / Aún no está implementado el campo                
                double cuotaIRPF = 0;
                for (var i = 0; i < factura.FacturasComprasTotales.Count; i++)
                {
                    cuotaIRPF += factura.FacturasComprasTotales.ElementAt(i).importeretencion ?? 0;
                }
                s += numericFormatExport(cuotaIRPF, 10, 2);                

                // CodPais
                s += string.Empty.PadRight(2, ' ');

                // Reservado
                s += string.Empty.PadRight(48, ' ');

                // CuentaCV             
                var regimeniva = factura.fkregimeniva;
                var codArticulo = factura.FacturasComprasLin.First().fkarticulos;
                var guiaContable = _db.Articulos.Where(f => f.empresa == Empresa && f.id == codArticulo).Select(f => f.fkguiascontables).SingleOrDefault();
                s += (_db.GuiascontablesLin.Where(f => f.empresa == Empresa && f.fkguiascontables == guiaContable && f.fkregimeniva == regimeniva).Select(f => f.fkcuentascompras)
                    .SingleOrDefault() ?? _db.Guiascontables.Where(f => f.empresa == Empresa && f.id == guiaContable).Select(f => f.fkcuentascompras).SingleOrDefault()).PadRight(12, ' ');


                // Pagado / Cobrado (S/N)
                s += "N";

                // Cobrador                      
                s += (factura.fkcuentastesoreria ?? string.Empty).PadRight(12, ' ');

                sb.Append(s);
                sb.Append(Environment.NewLine);
            }
            //Get Current Response  
            var response = System.Web.HttpContext.Current.Response;
            response.BufferOutput = true;
            response.Clear();
            response.ClearHeaders();
            response.ContentEncoding = Encoding.GetEncoding("ISO-8859-1");
            response.AddHeader("content-disposition", "attachment;filename=LBIVA000.ASC");
            response.ContentType = "text/plain";
            response.Write(sb.ToString());
            response.End();
        }


        public void ExportarPrevisiones()
        {
            var sb = new StringBuilder();
            int idEjercicio = Convert.ToInt32(EjercicioId);
            var configuracionService = new ConfiguracionService(_context, _db);
            var configuracionModel = configuracionService.GetModel();

            IEnumerable<Facturas> facturasList = _db.Facturas.Include("FacturasVencimientos").Where(f => f.empresa == Empresa && f.fkejercicio == idEjercicio && f.fkestados == configuracionModel.Estadofacturasventastotal);
            IEnumerable<FacturasCompras> facturasComprasList = _db.FacturasCompras.Include("FacturasComprasVencimientos").Where(f => f.empresa == Empresa && f.fkejercicio == idEjercicio && f.fkestados == configuracionModel.Estadofacturascomprastotal);

            foreach (var factura in facturasList)
            {
                foreach (var linea in factura.FacturasVencimientos)
                {
                    string s = "";

                    // Cuenta
                    s += factura.fkclientes.PadRight(12, ' ');

                    // Número de asiento                    
                    s += (_db.Movs.Where(f => f.empresa == Empresa && f.fkejercicio == idEjercicio && f.traza == factura.referencia).Select(f => f.identificadorsegmento).SingleOrDefault() ?? string.Empty).PadLeft(6, '0');

                    // Fecha vencimiento
                    s += linea.fechavencimiento.Value.ToString("ddMMyyyy").ToString().PadRight(8, ' ');

                    // Descripción
                    s += string.Empty.PadRight(30, ' ');

                    // Documento
                    s += factura.referencia.PadRight(12, ' ');

                    // Cuenta cobro / pago                    
                    s += (factura.fkcuentastesoreria ?? string.Empty).PadRight(12, ' ');

                    // Tipo (C = Cobro, P = Pago)
                    s += "C";

                    // Importe
                    s += numericFormatExport(linea.importevencimiento ?? 0, 10, 2);

                    // ForPago
                    s += factura.fkformaspago.ToString().PadLeft(4, '0');

                    sb.Append(s);
                    sb.Append(Environment.NewLine);
                }                
            }

            foreach (var factura in facturasComprasList)
            {
                foreach (var linea in factura.FacturasComprasVencimientos)
                {
                    string s = "";

                    // Cuenta
                    s += factura.fkproveedores.PadRight(12, ' ');

                    // Número de asiento
                    s += (_db.Movs.Where(f => f.empresa == Empresa && f.fkejercicio == idEjercicio && f.traza == factura.referencia).Select(f => f.identificadorsegmento).SingleOrDefault() ?? string.Empty).PadLeft(6, '0');

                    // Fecha vencimiento
                    s += linea.fechavencimiento.Value.ToString("ddMMyyyy").ToString().PadRight(8, ' ');

                    // Descripción
                    s += string.Empty.PadRight(30, ' ');

                    // Documento
                    s += factura.referencia.PadRight(12, ' ');

                    // Cuenta cobro / pago        
                    s += (factura.fkcuentastesoreria ?? string.Empty).PadRight(12, ' ');

                    // Tipo (C = Cobro, P = Pago)
                    s += "P";

                    // Importe
                    s += numericFormatExport(linea.importevencimiento ?? 0, 10, 2);

                    // ForPago
                    s += factura.fkformaspago.ToString().PadLeft(4, '0');

                    sb.Append(s);
                    sb.Append(Environment.NewLine);
                }
            }
            //Get Current Response  
            var response = System.Web.HttpContext.Current.Response;
            response.BufferOutput = true;
            response.Clear();
            response.ClearHeaders();
            response.ContentEncoding = Encoding.GetEncoding("ISO-8859-1");
            response.AddHeader("content-disposition", "attachment;filename=PREVI000.ASC");
            response.ContentType = "text/plain";
            response.Write(sb.ToString());
            response.End();
        }


        private string fixedLength(string input, int length)
        {
            if (input.Length > length)
                return input.Substring(0, length);
            else
                return input.PadRight(length, ' ');
        }

        private string numericFormatExport(double imp, int parteEntera, int decimales)
        {
            if(imp < 0)
            {
                imp = imp * -1;
            }
            decimal importe = (decimal)imp;
            int valorDecimales = 0;
            if (Math.Abs(importe - (int)importe) > 0)
            {
                valorDecimales = (int)((importe - (int)importe) * (decimal)Math.Pow(10, decimales));
            }
            else
            {
                return ((int)importe).ToString().PadLeft(parteEntera, '0') + string.Empty.PadRight(decimales, '0');
            }
            return ((int)importe).ToString().PadLeft(parteEntera, '0') + valorDecimales.ToString().PadLeft(decimales, '0');
        }                                            

        #endregion

        #region "Contabilizar"
        public MovsModel Contabilizar(IDocument factura)
        {
            ApplicationHelper app = new ApplicationHelper(_context);

            //serie contable defecto
            var ejercicio = _db.Ejercicios.Where(e => e.empresa == Empresa && e.id == factura.Fkejercicio).SingleOrDefault();
            var FkSerieContable = ejercicio.fkseriescontablesAST;

            //tipoasientodefecto - Ene2022 => Se ha incluido el tipo F4 para estos casos 
            //var tipoAsientoDefecto = app.GetListTiposAsientos().Where(f => f.Defecto).Select(f => f.Valor).SingleOrDefault();
            var tipoAsientoDefecto = "F4";

            //guia contable defecto
            string GuiaContDef = _db.Guiascontables.Where(g=> g.empresa == Empresa && g.defecto ).FirstOrDefault()?.id;

            //comentarios defecto
            var configuracionService = new ConfiguracionService(_context, _db);
            var configuracionModel = configuracionService.GetModel();
            
            //comentarios asientos            
            var listaComentarios = app.GetListComentariosAsientos();
            var comentarioVentasEIva = "Fra. ";
            var comentarioFactura = "N/Fra. ";
            var comentarioFacturaCompra = "S/Fra. ";            
            var comentarioRetencion = "Ret. Fra. ";

            MovsModel documento = new FModel().GetModel<MovsModel>(_context);


            //Factura de venta
            if (factura is FacturasModel)
            {
                var facturaVenta = factura as FacturasModel;

                //Comprobamos si la forma de pago y el mandato es correcto - Jul2022 Problema en el cambio de forma de pago
                var _converterModel = FConverterModel.Instance.CreateConverterModelService<FacturasModel, Facturas>(_context, _db, Empresa);
                var newItem = _converterModel.CreatePersitance(facturaVenta);
                //var validationFactura = new FacturasValidation(_context, _db);
                //validationFactura.ValidarGrabar(newItem);
                var formapagoobj = _db.FormasPago.Single(f => f.id == newItem.fkformaspago);
                if (formapagoobj.mandato.HasValue && formapagoobj.mandato.Value && string.IsNullOrEmpty(newItem.fkbancosmandatos))
                {
                    var mandato = _db.BancosMandatos.SingleOrDefault(
                        f => f.fkcuentas == newItem.fkclientes && f.defecto == true && !string.IsNullOrEmpty(f.idmandato));
                    var vector = newItem.fkestados.Split('-');
                    var tipoestado = Funciones.Qint(vector[0]);
                    var idestado = vector[1];
                    var estadoObj = _db.Estados.Single(f => f.documento == tipoestado && f.id == idestado);
                    if (mandato == null && estadoObj.tipoestado == (int)Model.Configuracion.TipoEstado.Finalizado)
                    {
                        throw new ValidationException(RFacturas.ErrorFormaPagoMandatoRequerido);
                    }
                    else
                    {
                        throw new ValidationException(RFacturas.WarningFormaPagoMandatoRequerido);
                    }
                }


                short EsDevolucion = facturaVenta.Importebaseimponible > 0 ? (short)1 : (short)-1;
                documento.Fecha = facturaVenta.Fechadocumento;
                documento.Fkseriescontables = FkSerieContable;// facturaVenta.Fkseries;
                documento.Tipoasiento = tipoAsientoDefecto;
                documento.Fkmonedas = facturaVenta.Fkmonedas;
                documento.Decimalesmonedas = facturaVenta.Decimalesmonedas;
                documento.Codigodescripcionasiento = configuracionModel.DescripcionAsientoFacturaVenta;
                documento.Descripcionasiento = "Factura - " + facturaVenta.Referencia;
                documento.Referencialibre = facturaVenta.Referencia;
                documento.Traza = facturaVenta.Referencia;
                documento.Empresa = facturaVenta.Empresa;                

                //Cliente
                if (string.IsNullOrWhiteSpace(facturaVenta.Fkclientes)) throw new ValidationException(string.Format("No se encuentra cuenta asociada a cliente {0}", facturaVenta.Fkclientes));
                var lineaCli = new MovsLinModel();
                lineaCli.Id = documento.Lineas.Count + 1;
                lineaCli.Fkcuentas = facturaVenta.Fkclientes;
                lineaCli.Esdebe = (short)(1 * EsDevolucion);
                lineaCli.Importe = (decimal)Math.Abs(facturaVenta.Importetotaldoc.Value);
                lineaCli.Comentario = comentarioFactura + " " + facturaVenta.Referencia;
                documento.Lineas.Add(lineaCli);

                //retención
                if (!string.IsNullOrEmpty(facturaVenta.Fktiposretenciones))
                {
                    foreach (var linFacTot in facturaVenta.Totales)
                    {
                        var FkCuenta = _db.Tiposretenciones.Where(i => i.empresa == documento.Empresa && i.id == facturaVenta.Fktiposretenciones)?.SingleOrDefault()?.fkcuentascargo;
                        if (string.IsNullOrWhiteSpace(FkCuenta)) throw new ValidationException(string.Format("No se encuentra cuenta asociada a tipo retención {0}", facturaVenta.Fktiposretenciones));
                        var linea = documento.Lineas.Where(l => l.Fkcuentas == FkCuenta).SingleOrDefault();
                        if (linea == null)
                        {
                            linea = new MovsLinModel();
                            linea.Id = documento.Lineas.Count + 1;
                            linea.Fkcuentas = FkCuenta;
                            linea.Esdebe = (short)(1 * EsDevolucion);
                            linea.Comentario = comentarioRetencion + " " + facturaVenta.Referencia;
                            documento.Lineas.Add(linea);
                        }

                        //Esta fallando aqui
                        if(linFacTot.Importeretencion != null)
                        {
                            linea.Importe += (decimal)Math.Abs(linFacTot.Importeretencion.Value);
                        }
                    }
                }

                //recorrer las lineas
                List<MovsLinModel> listaMovsLin = new List<MovsLinModel>();

                foreach (var linFac in facturaVenta.Lineas)
                {
                    var FkGuiaCont = _db.Articulos.Where(a => a.empresa == documento.Empresa && a.id == linFac.Fkarticulos).SingleOrDefault()?.fkguiascontables;
                    if (string.IsNullOrWhiteSpace(FkGuiaCont)) FkGuiaCont = GuiaContDef;
                    var FkCuenta = _db.Guiascontables.Where(g => g.empresa == documento.Empresa && g.id == FkGuiaCont).SingleOrDefault()?.fkcuentasventas;
                    if (string.IsNullOrWhiteSpace(FkCuenta)) throw new ValidationException(string.Format("No se encuentra cuenta asociada a guia contable {0}", FkGuiaCont));

                    //var linea = documento.Lineas.Where(l => l.Fkcuentas == FkCuenta).SingleOrDefault();
                    var linea = listaMovsLin.Where(l => l.Fkcuentas == FkCuenta).SingleOrDefault();
                    if (linea == null)
                    {
                        linea = new MovsLinModel();                        
                        linea.Id = documento.Lineas.Count + 1 + listaMovsLin.Count;                                                
                        linea.Fkcuentas = FkCuenta;
                        //linea.Esdebe = (short)(-1 * EsDevolucion);
                        linea.Comentario = comentarioVentasEIva + " " + facturaVenta.Referencia + " " + facturaVenta.Nombrecliente;
                        listaMovsLin.Add(linea);
                        //documento.Lineas.Add(linea);
                    }

                    if(linFac.Importenetolinea!=null)
                    {
                        linea.Importe += (decimal)(linFac.Importenetolinea.Value);
                    }              
                }

                //Esdebe de las líneas de los artículos
                foreach (var item in listaMovsLin)
                {
                    if (item.Importe > 0)
                    {
                        item.Esdebe = -1;
                        item.Importe = Math.Abs(item.Importe);
                        documento.Lineas.Add(item);
                    }                        
                    else
                    {
                        item.Esdebe = 1;
                        item.Importe = Math.Abs(item.Importe);
                        documento.Lineas.Add(item);
                    }
                }

                //iva
                foreach (var linFacTot in facturaVenta.Totales)
                {
                    var FkCuenta = _db.TiposIva.Where(i => i.empresa == documento.Empresa && i.id == linFacTot.Fktiposiva)?.SingleOrDefault()?.fkcuentasivarepercutido;
                    if (string.IsNullOrWhiteSpace(FkCuenta)) throw new ValidationException(string.Format("No se encuentra cuenta asociada a tipo iva {0}", linFacTot.Fktiposiva));
                    var linea = documento.Lineas.Where(l => l.Fkcuentas == FkCuenta).SingleOrDefault();
                    if (linea == null)
                    {
                        linea = new MovsLinModel();
                        linea.Id = documento.Lineas.Count + 1;
                        linea.Fkcuentas = FkCuenta;
                        linea.Esdebe = (short)(-1 * EsDevolucion);
                        linea.Comentario = comentarioVentasEIva + " " + facturaVenta.Referencia + " " + facturaVenta.Nombrecliente;
                        documento.Lineas.Add(linea);
                    }

                    if(linFacTot.Cuotaiva != null)
                    {
                        linea.Importe += (decimal)Math.Abs(linFacTot.Cuotaiva.Value);
                    }             
                }

                var totaldebe = Math.Round(documento.Lineas.Where(f => f.Esdebe == 1).Sum(f => f.Importe), documento.Decimalesmonedas);
                var totalhaber = Math.Round(documento.Lineas.Where(f => f.Esdebe == -1).Sum(f => f.Importe), documento.Decimalesmonedas);

                if (totaldebe - totalhaber != 0)
                    throw new ValidationException(RMovs.ErrorDescuadreDebeHaber);

                //Nuevos Vencimientos y Pagos
                var vencimientosService = new VencimientosService(_context);

                //Vencimientosº 
                foreach(var vencimiento in facturaVenta.Vencimientos)
                {
                    var vencimientosModel = new VencimientosModel(_context);
                    vencimientosModel.Empresa = Empresa; //Empresa
                    vencimientosModel.Fkseriescontables = _db.SeriesContables.Where(f => f.empresa == Empresa && f.tipodocumento == "PRC").Select(f => f.id).SingleOrDefault();
                    vencimientosModel.Fecha = vencimiento.Fechavencimiento;
                    vencimientosModel.Situacion = _db.SituacionesTesoreria.Where(f => f.valorinicialcobros == true).Select(f => f.cod).SingleOrDefault();
                    vencimientosModel.Traza = facturaVenta.Referencia; //Traza//Referencia Factura
                    vencimientosModel.Tipo = TipoVencimiento.Cobros;
                    vencimientosModel.Origen = TipoOrigen.FacturaVenta;
                    vencimientosModel.Usuario = _context.Usuario;
                    vencimientosModel.Fkcuentas = facturaVenta.Fkclientes;
                    vencimientosModel.Fechacreacion = DateTime.Now;
                    vencimientosModel.Fechafactura = facturaVenta.Fechadocumento.Value;
                    vencimientosModel.Fecharegistrofactura = _db.Facturas.Where(f => f.empresa == Empresa && f.id == facturaVenta.Id).Select(f => f.fechaalta).SingleOrDefault();
                    vencimientosModel.Fechavencimiento = vencimiento.Fechavencimiento.Value;
                    vencimientosModel.Monedabase = _db.Empresas.Where(f => f.id == Empresa).Select(f => f.fkMonedabase.Value).SingleOrDefault();
                    vencimientosModel.Monedagiro = facturaVenta.Fkmonedas.Value;
                    vencimientosModel.Importegiro = vencimiento.Importevencimiento;
                    //vencimientosModel.Importefactura = Convert.ToDecimal(facturaVenta.Importetotaldoc);
                    vencimientosModel.Monedafactura = facturaVenta.Fkmonedas.Value;
                    vencimientosModel.Mandato = facturaVenta.Fkbancosmandatos;
                    vencimientosModel.Estado = Marfil.Dom.Persistencia.Model.Documentos.CobrosYPagos.TipoEstado.Inicial;
                    vencimientosModel.Fkformaspago = facturaVenta.Fkformaspago;
                    vencimientosModel.Fkcuentatesoreria = facturaVenta.Fkcuentastesoreria;

                    vencimientosService.create(vencimientosModel);
                }
            }

            //Factura de compra
            if (factura is Model.Documentos.FacturasCompras.FacturasComprasModel)
            {
                var facturaCompra = factura as FacturasComprasModel;
                short EsDevolucion = facturaCompra.Importebaseimponible > 0 ? (short)1 : (short)-1;
                documento.Fecha = facturaCompra.Fechadocumento;
                documento.Fkseriescontables = FkSerieContable;
                documento.Tipoasiento = tipoAsientoDefecto;
                documento.Fkmonedas = facturaCompra.Fkmonedas;
                documento.Decimalesmonedas = facturaCompra.Decimalesmonedas;
                documento.Codigodescripcionasiento = configuracionModel.DescripcionAsientoFacturaCompra;
                documento.Descripcionasiento = "Factura compra - " + facturaCompra.Referencia;
                documento.Referencialibre = facturaCompra.Referencia;
                documento.Traza = facturaCompra.Referencia;

                //lineas
                List<MovsLinModel> listaMovsLin = new List<MovsLinModel>();

                foreach (var linFac in facturaCompra.Lineas)
                {
                    var FkGuiaCont = _db.Articulos.Where(a => a.empresa == documento.Empresa && a.id == linFac.Fkarticulos).SingleOrDefault()?.fkguiascontables;
                    if (string.IsNullOrWhiteSpace(FkGuiaCont)) FkGuiaCont = GuiaContDef;

                    var FkCuenta = _db.Guiascontables.Where(g => g.empresa == documento.Empresa && g.id == FkGuiaCont).SingleOrDefault()?.fkcuentascompras;
                    if (string.IsNullOrWhiteSpace(FkCuenta)) throw new ValidationException(string.Format("No se encuentra cuenta asociada a guia contable {0}", FkGuiaCont));

                    //var linea = documento.Lineas.Where(l => l.Fkcuentas == FkCuenta).SingleOrDefault();
                    var linea = listaMovsLin.Where(l => l.Fkcuentas == FkCuenta).SingleOrDefault();
                    if (linea == null)
                    {
                        linea = new MovsLinModel();
                        linea.Id = documento.Lineas.Count + 1 + listaMovsLin.Count;                        
                        linea.Fkcuentas = FkCuenta;
                        linea.Esdebe = (short)(1 * EsDevolucion);
                        linea.Comentario = comentarioVentasEIva + " " + facturaCompra.Referencia + " " + facturaCompra.Nombrecliente;
                        listaMovsLin.Add(linea);
                        //documento.Lineas.Add(linea);
                    }

                    if(linFac.Importenetolinea!=null)
                    {
                        linea.Importe += (decimal)(linFac.Importenetolinea.Value);
                    }
                    
                }

                //Esdebe de las líneas de los artículos
                foreach (var item in listaMovsLin)
                {
                    if (item.Importe > 0)
                    {
                        item.Esdebe = 1;
                        item.Importe = Math.Abs(item.Importe);
                        documento.Lineas.Add(item);
                    }
                    else
                    {
                        item.Esdebe = -1;
                        item.Importe = Math.Abs(item.Importe);
                        documento.Lineas.Add(item);
                    }
                }

                //iva
                foreach (var linFacTot in facturaCompra.Totales)
                {
                    var FkCuenta = _db.TiposIva.Where(i => i.empresa == documento.Empresa && i.id == linFacTot.Fktiposiva)?.SingleOrDefault()?.fkcuentasivasoportado;
                    if (string.IsNullOrWhiteSpace(FkCuenta)) throw new ValidationException(string.Format("No se encuentra cuenta asociada a tipo iva {0}", linFacTot.Fktiposiva));

                    var linea = documento.Lineas.Where(l => l.Fkcuentas == FkCuenta).SingleOrDefault();
                    if (linea == null)
                    {
                        linea = new MovsLinModel();
                        linea.Id = documento.Lineas.Count + 1;
                        linea.Fkcuentas = FkCuenta;
                        linea.Esdebe = (short)(1 * EsDevolucion);
                        linea.Comentario = comentarioVentasEIva + " " + facturaCompra.Referencia + " " + facturaCompra.Nombrecliente;
                        documento.Lineas.Add(linea);
                    }

                    if(linFacTot.Cuotaiva != null)
                    {
                        linea.Importe += (decimal)Math.Abs(linFacTot.Cuotaiva.Value);
                    }
                    
                }

                //Proveedor
                if (string.IsNullOrWhiteSpace(facturaCompra.Fkproveedores)) throw new ValidationException(string.Format("No se encuentra cuenta asociada a proveedor {0}", facturaCompra.Fkproveedores));
                var lineaProv = new MovsLinModel();
                lineaProv.Id = documento.Lineas.Count + 1;
                lineaProv.Fkcuentas = facturaCompra.Fkproveedores;
                lineaProv.Esdebe = (short)(-1 * EsDevolucion);
                lineaProv.Importe = (decimal)Math.Abs(facturaCompra.Importetotaldoc.Value);
                lineaProv.Comentario = comentarioFacturaCompra + " " + facturaCompra.Referencia;
                documento.Lineas.Add(lineaProv);

                //retención
                if (!string.IsNullOrEmpty(facturaCompra.Fktiposretenciones))
                {
                    foreach (var linFacTot in facturaCompra.Totales)
                    {
                        var FkCuenta = _db.Tiposretenciones.Where(i => i.empresa == documento.Empresa && i.id == facturaCompra.Fktiposretenciones)?.SingleOrDefault()?.fkcuentasabono;
                        if (string.IsNullOrWhiteSpace(FkCuenta)) throw new ValidationException(string.Format("No se encuentra cuenta asociada a tipo retención {0}", facturaCompra.Fktiposretenciones));
                        var linea = documento.Lineas.Where(l => l.Fkcuentas == FkCuenta).SingleOrDefault();
                        if (linea == null)
                        {
                            linea = new MovsLinModel();
                            linea.Id = documento.Lineas.Count + 1;
                            linea.Fkcuentas = FkCuenta;
                            linea.Esdebe = (short)(-1 * EsDevolucion);
                            linea.Comentario = comentarioRetencion + " " + facturaCompra.Referencia;
                            documento.Lineas.Add(linea);
                        }

                        if(linFacTot.Importeretencion != null)
                        {
                            linea.Importe += (decimal)Math.Abs(linFacTot.Importeretencion.Value);
                        }                    
                    }
                }

                var totaldebe = Math.Round(documento.Lineas.Where(f => f.Esdebe == 1).Sum(f => f.Importe), documento.Decimalesmonedas);
                var totalhaber = Math.Round(documento.Lineas.Where(f => f.Esdebe == -1).Sum(f => f.Importe), documento.Decimalesmonedas);

                if (totaldebe - totalhaber != 0)
                    throw new ValidationException(RMovs.ErrorDescuadreDebeHaber);

                //Nuevos Vencimientos y Pagos
                var vencimientosService = new VencimientosService(_context);

                //Vencimientos
                foreach (var vencimiento in facturaCompra.Vencimientos)
                {
                    var vencimientosModel = new VencimientosModel(_context);
                    vencimientosModel.Empresa = Empresa; //Empresa
                    vencimientosModel.Fkseriescontables = _db.SeriesContables.Where(f => f.empresa == Empresa && f.tipodocumento == "PRP").Select(f => f.id).SingleOrDefault();
                    vencimientosModel.Situacion = _db.SituacionesTesoreria.Where(f => f.valorinicialpagos == true).Select(f => f.cod).SingleOrDefault();
                    vencimientosModel.Traza = facturaCompra.Referencia; //Traza//Referencia Factura
                    vencimientosModel.Tipo = TipoVencimiento.Pagos;
                    vencimientosModel.Origen = TipoOrigen.FacturaCompra;
                    vencimientosModel.Usuario = _context.Usuario;
                    vencimientosModel.Fkcuentas = facturaCompra.Fkproveedores;
                    vencimientosModel.Fechacreacion = DateTime.Now;
                    vencimientosModel.Fechafactura = facturaCompra.Fechadocumento.Value;
                    vencimientosModel.Fecharegistrofactura = _db.FacturasCompras.Where(f => f.empresa == Empresa && f.id == facturaCompra.Id).Select(f => f.fechaalta).SingleOrDefault();
                    vencimientosModel.Fechavencimiento = vencimiento.Fechavencimiento.Value;
                    vencimientosModel.Monedabase = _db.Empresas.Where(f => f.id == Empresa).Select(f => f.fkMonedabase.Value).SingleOrDefault();
                    vencimientosModel.Monedagiro = facturaCompra.Fkmonedas.Value;
                    vencimientosModel.Importegiro = vencimiento.Importevencimiento;
                    //vencimientosModel.Importefactura = Convert.ToDecimal(facturaCompra.Importetotaldoc.Value);
                    vencimientosModel.Monedafactura = facturaCompra.Fkmonedas.Value;
                    vencimientosModel.Mandato = facturaCompra.Fkbancosmandatos;
                    vencimientosModel.Estado = Marfil.Dom.Persistencia.Model.Documentos.CobrosYPagos.TipoEstado.Inicial;
                    vencimientosModel.Fkformaspago = facturaCompra.Fkformaspago;
                    vencimientosModel.Fkcuentatesoreria = facturaCompra.Fkcuentastesoreria;

                    vencimientosService.create(vencimientosModel);
                }
            }

            using (var trans = TransactionScopeBuilder.CreateTransactionObject())
            {
                create(documento);
                var serviceFactura = FService.Instance.GetService(factura.GetType(), _context);
                if (factura is FacturasModel)
                {
                    var facturaVenta = factura as FacturasModel;
                    facturaVenta.Fkasiento = documento.Id;
                    serviceFactura.edit(facturaVenta);
                }
                else
                {
                    var facturaCompra = factura as FacturasComprasModel;
                    facturaCompra.Fkasiento = documento.Id;
                    serviceFactura.edit(facturaCompra);
                }

                trans.Complete();
            }


            return documento;
        }
        #endregion


        #region Helper

        //public IEnumerable<CuentasBusqueda> GetCuentas(TiposCuentas tipo)
        //{
        //    return _db.Database.SqlQuery<CuentasBusqueda>(string.Format(SelectCuentasTerceros, (int)tipo, Empresa, tipo)).ToList();
        //}



        //public IEnumerable<TablasVariasGeneralModel> GetListComentariosAsientos()
        //{
        //    var service = new TablasVariasService(_context, _db);
        //    var comentariosasientos = service.GetTablasVariasByCode(70);
        //    return comentariosasientos.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        //}


        //private struct StLote
        //{
        //    public string Lote { get; set; }
        //    public int Numero { get; set; }
        //}





        #endregion





        #region Anterior/Siguiente



        public override string LastRegister()
        {
            var keyNames = GetprimarykeyColumns();
            var enumerable = keyNames as string[] ?? keyNames.ToArray();
            
            var query = _db.Movs.Where(f => f.empresa == Empresa);

            var flagFirst = true;
            IOrderedQueryable<Movs> orderedQuery = null;
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
                            var obj = _db.Set<Movs>().Find(vector);
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

            sb.AppendFormat("select top 1 * from Movs where ");
            var flagFirst = true;
            for (var i = 0; i < keyNames.Count(); i++)
            {
                if (!flagFirst)
                    sb.Append(" AND ");

                if (i == keyNames.Count() - 1)
                    sb.AppendFormat("{0}{1}@{0}", keyNames[i], tipo == TipoSelect.Next ? ">" : "<");
                else
                {
                    sb.AppendFormat("{0}=@{0}", keyNames[i]);
                    flagFirst = false;
                }
            }

            if (tipo == TipoSelect.Previous)
                sb.AppendFormat(" order by {0} desc", string.Join(",", keyNames));

            return sb.ToString();
        }

        public override string FirstRegister()
        {
            
            var obj = _db.Set<Movs>().FirstOrDefault(f => f.empresa == Empresa );
            if (obj == null) return string.Empty;

            var modelObj = _converterModel.GetModelView(obj);
            return modelObj.GetPrimaryKey();
        }

        #endregion


    }
}
