using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.ControlsUI.Busquedas;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Documentos;
using Marfil.Dom.Persistencia.Model.Documentos.Albaranes;
using Marfil.Dom.Persistencia.Model.Documentos.Facturas;
using Marfil.Dom.Persistencia.Model.Documentos.Inventarios;
using Marfil.Dom.Persistencia.Model.Documentos.Pedidos;
using Marfil.Dom.Persistencia.Model.Documentos.Presupuestos;
using Marfil.Dom.Persistencia.Model.Documentos.Reservasstock;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Mobile;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos.Helper;
using Newtonsoft.Json;



namespace Marfil.App.WebMain.Controllers
{
    public class StDocumentos : BaseParam, IDocumentosFiltros
    {
        public string Id { get; set; }
        public string Filtros { get; set; }
        public string Pagina { get; set; }
        public string RegistrosPagina { get; set; }
        public string Tipodocumento { get; set; }
    }

    public class StDocumentoVenta
    {
        public string Tipodocumento { get; set; }
        public string Referencia { get; set; }
    }

    public class StDocumentosVentasLineas
    {
        public string Tipodocumento { get; set; }
        public string Referencia { get; set; }
        public string Lote { get; set; }
        public string Operacion { get; set; }//Añadir :0, Elminar: 1

    }

    public class MobileDocumentosVentasApiController : BasicAuthHttpModule
    {
        public MobileDocumentosVentasApiController(ILoginService service) : base(service)
        {
        }

        #region Buscar documentos 

        [System.Web.Mvc.HttpPost]
        public ActionResult Buscar(StDocumentos model)
        {
            using (var service = GetService(model.Tipodocumento))
            {

                int registrosTotales = 0;
                IEnumerable<DocumentosBusqueda> items = null;
                try
                {

                    items = service.Buscar(model, out registrosTotales);
                }
                catch (Exception ex)
                {
                    return Json(new ErrorJson(ex.Message));
                }

                var result = new ResultBusquedasPaginados<DocumentosBusqueda>()
                {
                    values = items,
                    columns = new[]
                    {
                        new ColumnDefinition() { field = "Referencia", displayName = "Referencia", visible = true},
                        new ColumnDefinition() { field = "Fecha", displayName = "Fecha", visible = true },
                        new ColumnDefinition() { field = "Fkclientes", displayName = "Cliente", visible = true },
                        new ColumnDefinition() { field = "Nombrecliente", displayName = "Nombre", visible = true },
                        new ColumnDefinition() { field = "Estado", displayName = "Estado", visible = true },
                        new ColumnDefinition() { field = "Basecadena", displayName = "Base", visible = true }
                    },
                    RegistrosTotales = registrosTotales,
                    PaginaActual = Funciones.Qint(model.Pagina) ?? 1,
                };

                return Json(result);
            }
        }

        private IBuscarDocumento GetService(string tipo)
        {
            if (tipo == "0")
            {
                return FService.Instance.GetService(typeof (PresupuestosModel), ContextService,MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos)) as IBuscarDocumento;
            }
            if (tipo == "1")
            {
                return FService.Instance.GetService(typeof(PedidosModel), ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos)) as IBuscarDocumento;
            }
            if (tipo == "2")
            {
                return FService.Instance.GetService(typeof(AlbaranesModel), ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos)) as IBuscarDocumento;
            }
            if (tipo == "3")
            {
                return FService.Instance.GetService(typeof(FacturasModel), ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos)) as IBuscarDocumento;
            }
            if (tipo == "4")
            {
                return FService.Instance.GetService(typeof(EntregasStockModel), ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos)) as IBuscarDocumento;
            }
            if (tipo == "5")
            {
                return FService.Instance.GetService(typeof(ReservasstockModel), ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos)) as IBuscarDocumento;
            }
            if (tipo == "6")
            {
                return FService.Instance.GetService(typeof(InventariosModel), ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos)) as IBuscarDocumento;
            }
            throw new Exception("Tipo no implementado");
        }

        #endregion

        #region Buscar un documento

        [System.Web.Mvc.HttpPost]
        public ActionResult GetDocumento(StDocumentoVenta model)
        {
            using (var service = GetService(model.Tipodocumento))
            {
                try
                {
                    var list = service.BuscarDocumento(model.Referencia);
                    return Json(list);
                }
                catch (Exception ex)
                {
                    return Json(new ErrorJson(ex.Message));
                }
            }
        }


        #endregion

        #region Agregar linea

        [System.Web.Mvc.HttpPost]
        public ActionResult AgregarLinea(StDocumentosVentasLineas model)
        {
            using (var service = GetService(model.Tipodocumento) )
            {
                try
                {
                    if (service is InventariosService)
                    {
                        var agregarService = service as InventariosService;
                        var list = model.Operacion == "0" ? agregarService.AgregarLinea(model.Referencia, model.Lote) : agregarService.EliminarLinea(model.Referencia, model.Lote);
                        return Json(list);
                    }
                    else
                    {
                        var agregarService = service as IAgregarLineaDocumentoMovile;
                        var list = model.Operacion == "0" ? agregarService.AgregarLinea(model.Referencia, model.Lote) : agregarService.EliminarLinea(model.Referencia, model.Lote);
                        return Json(list);
                    }
                    
                }
                catch (Exception ex)
                {
                    var result = new AgregarLineaDocumentosModel() { Error = ex.Message };
                    return Json(result);
                }
            }
        }

        #endregion

        #region Buscar Reserva/Entrega/Inventarios

        [System.Web.Mvc.HttpPost]
        public ActionResult BuscarDocumentoVentaProduccion(StDocumentoVenta model)
        {
            using (var service = GetService(model.Tipodocumento) as IGestionService)
            {
                try
                {   
                    if (service is ReservasstockService)
                    {
                        var servicereservas = service as ReservasstockService;
                        var list = servicereservas.GetByReferencia(model.Referencia);
                        list = servicereservas.get(list.Id.ToString()) as ReservasstockModel;
                        var result = new AgregarLineaDocumentosModel()
                        {
                            Fecha =list.Fechadocumentocadena,
                            Referencia = list.Referencia,
                            Lineas=list.Lineas.Select(f=>new AgregarLineaDocumentosLinModel()
                            {
                                Fkarticulos = f.Fkarticulos,
                                Descripcion = f.Descripcion,
                                Lote = f.Lote,
                                Cantidad = f.Cantidad.ToString(),
                                Ancho = f.SAncho,
                                Largo = f.SLargo,
                                Grueso = f.SGrueso,
                                Metros = f.SMetros

                                

                            }).ToList()
                        };
                        return Json(result);
                    }
                    else if (service is EntregasService)
                    {
                        var servicereservas = service as EntregasService;
                        var list = servicereservas.GetByReferencia(model.Referencia);
                        list = servicereservas.get(list.Id.ToString()) as AlbaranesModel;
                        var result = new AgregarLineaDocumentosModel()
                        {
                            Fecha = list.Fechadocumentocadena,
                            Referencia = list.Referencia,
                            Lineas = list.Lineas.Select(f => new AgregarLineaDocumentosLinModel()
                            {
                                Fkarticulos = f.Fkarticulos,
                                Descripcion = f.Descripcion,
                                Lote = f.Lote,
                                Cantidad = f.Cantidad.ToString(),
                                Ancho = f.SAncho,
                                Largo = f.SLargo,
                                Grueso = f.SGrueso,
                                Metros = f.SMetros
                            }).ToList()
                        };
                        return Json(result);
                    }
                    else if (service is InventariosService)
                    {
                        var servicereservas = service as InventariosService;
                        var list = servicereservas.GetByReferencia(model.Referencia);
                        list = servicereservas.get(list.Id.ToString()) as InventariosModel;
                        var result = new AgregarLineaInventariosDocumentosModel()
                        {
                            Fecha = list.Fechadocumentocadena,
                            Referencia = list.Referencia,
                            Lineas = list.Lineas.Select(f => new AgregarLineaInventariosDocumentosLinModel()
                            {
                                Fkarticulos = f.Fkarticulos,
                                Descripcion = f.Descripcion,
                                Lote = string.Format("{0}{1}",f.Lote,Funciones.RellenaCod(f.Loteid,3)),
                                Cantidad = f.Cantidad.ToString(),
                                Ancho = f.SAncho,
                                Largo = f.SLargo,
                                Grueso = f.SGrueso,
                                Metros = f.SMetros,
                                Estado = Funciones.GetEnumByStringValueAttribute(f.Estado),
                                Codigoestado = ((int)f.Estado).ToString()
                            }).ToList()
                        };
                        return Json(result);
                    }

                    return Json(new AgregarLineaDocumentosModel() { Error = "Operacion incorrecta" });
                }
                catch (Exception ex)
                {
                    return Json(new AgregarLineaDocumentosModel() { Error = ex.Message });
                }
            }
        }

        #endregion


    }
}
