using System;
using System.Linq;
using System.Web.Mvc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView;
using Resources;
using System.IO;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using System.Text;
using System.Data;
using System.Web.Hosting;
using System.Threading.Tasks;
using System.Threading;
using Marfil.Dom.Persistencia;
using System.Collections.Generic;
using Marfil.Inf.Genericos.Helper;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.Model.Interfaces;

namespace Marfil.App.WebMain.Controllers
{
    public class ImportarController : GenericController<ImportarModel>
    {
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            //IsActivado = ContextService.IsSuperAdmin;
            //CanCrear = false;
            //CanModificar = false;
            //CanEliminar = false;
            MenuName = "importar";
            var permisos = appService.GetPermisosMenu("importar");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        #region CTR

        public ImportarController(IContextService context) : base(context)
        {

        }

        #endregion

        #region ImportarStock

        public ActionResult ImportarStock()
        {
            ImportarModel model = new ImportarModel();

            using (var db = MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos))
            {
                model.Serie = db.Series.Where(f => f.empresa == Empresa && f.entradasvarias == true)
                            .Select(f => new SelectListItem() { Value = f.id, Text = f.descripcion }).ToList();
            }

            model.TipoLote = new List<SelectListItem> {
                new SelectListItem { Text = Enum.GetName(typeof(TipoAlmacenlote), TipoAlmacenlote.Mercaderia), Value = TipoAlmacenlote.Mercaderia.ToString() },
                new SelectListItem { Text = Enum.GetName(typeof(TipoAlmacenlote), TipoAlmacenlote.Propio), Value = TipoAlmacenlote.Propio.ToString() },
                new SelectListItem { Text = Enum.GetName(typeof(TipoAlmacenlote), TipoAlmacenlote.Gestionado), Value = TipoAlmacenlote.Gestionado.ToString() }
            };


            return View("ImportarStock", model);
        }

        [HttpPost]
        public ActionResult ImportarStock(ImportarModel model)
        {
            var idPeticion = 0;
            var file = model.Fichero;
            char delimitador = model.Delimitador.ToCharArray()[0];
            int albaran = model.Albaran != null ? int.Parse(model.Albaran) : 0;
            string serie = model.SelectedId;
            //int tipoLote = Funciones.Qint(model.SelectedIdTipoAlmacenLote) ?? 0;

            using (var db = MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos))
            {
                model.Serie = db.Series.Where(f => f.empresa == Empresa && f.entradasvarias == true)
                            .Select(f => new SelectListItem() { Value = f.id, Text = f.descripcion }).ToList();
            }

            model.TipoLote = new List<SelectListItem> {
                new SelectListItem { Text = Enum.GetName(typeof(TipoAlmacenlote), TipoAlmacenlote.Mercaderia), Value = TipoAlmacenlote.Mercaderia.ToString() },
                new SelectListItem { Text = Enum.GetName(typeof(TipoAlmacenlote), TipoAlmacenlote.Propio), Value = TipoAlmacenlote.Propio.ToString() },
                new SelectListItem { Text = Enum.GetName(typeof(TipoAlmacenlote), TipoAlmacenlote.Gestionado), Value = TipoAlmacenlote.Gestionado.ToString() }
            };

            if (ModelState.IsValid)
            {
                if (albaran != 0)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        if (file.FileName.ToLower().EndsWith(".csv"))
                        {
                            var service = FService.Instance.GetService(typeof(RecepcionesStockModel), ContextService) as RecepcionStockService;
                            StreamReader sr = new StreamReader(file.InputStream, Encoding.UTF8);
                            StringBuilder sb = new StringBuilder();
                            DataTable dt = new DataTable();
                            DataRow dr;
                            string s;
                            int j = 0;

                            dt.Columns.Add("CodArticulo");
                            dt.Columns.Add("Lote");
                            dt.Columns.Add("Tabla");
                            dt.Columns.Add("Cantidad");
                            dt.Columns.Add("Largo");
                            dt.Columns.Add("Ancho");
                            dt.Columns.Add("Grueso");
                            dt.Columns.Add("Metros");
                            dt.Columns.Add("Precio");

                            while (!sr.EndOfStream)
                            {
                                while ((s = sr.ReadLine()) != null)
                                {
                                    //Ignorar cabecera                    
                                    if (j > 0 || !model.Cabecera)
                                    {
                                        string[] str = s.Split(delimitador);
                                        dr = dt.NewRow();

                                        for (int i = 0; i < dt.Columns.Count; i++)
                                        {
                                            try
                                            {
                                                dr[dt.Columns[i]] = str[i].Replace("\"", string.Empty).ToString() ?? string.Empty;
                                            }
                                            catch (Exception ex)
                                            {
                                                ModelState.AddModelError("File", General.ErrorDelimitadorFormato);
                                                return View("ImportarStock", model);
                                            }
                                        }
                                        dt.Rows.Add(dr);
                                    }
                                    j++;
                                }
                            }
                            try
                            {
                                idPeticion = service.CrearPeticionImportacion(ContextService);
                                HostingEnvironment.QueueBackgroundWorkItem(async token => await GetAsync(dt, albaran, serie, idPeticion, token));
                                //service.Importar(dt, model.Seriecontable.ToString(), ContextService);
                                sr.Close();
                            }
                            catch (ValidationException ex)
                            {
                                if (string.IsNullOrEmpty(ex.Message))
                                {
                                    TempData["Errors"] = null;
                                }
                                else
                                {
                                    TempData["Errors"] = ex.Message;
                                }
                            }

                            //TempData["Success"] = "Importado correctamente!";
                            TempData["Success"] = "Ejecutando, proceso con id = " + idPeticion.ToString() + ", para comprobar su ejecución ir al menú de peticiones asíncronas";
                            return RedirectToAction("ImportarStock", "Importar");
                        }
                        else
                        {
                            ModelState.AddModelError("File", General.ErrorFormatoFichero);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("File", General.ErrorFichero);
                    }
                }
                else
                {
                    ModelState.AddModelError("File", "El albarán indicado no es correcto");
                }
            }

            return View("ImportarStock", model);
        }

        private async Task GetAsync(DataTable dt, int albaran, string serie, int idPeticion, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var service = FService.Instance.GetService(typeof(RecepcionesStockModel), ContextService) as RecepcionStockService)
                {
                    await Task.Run(() => service.Importar(dt, albaran, serie, idPeticion, ContextService));
                    return;
                }

            }
            catch (TaskCanceledException tce)
            {

            }
            catch (Exception ex)
            {
                using (var service = FService.Instance.GetService(typeof(PeticionesAsincronasModel), ContextService) as PeticionesAsincronasService)
                {
                    service.CambiarEstado(EstadoPeticion.Error, idPeticion, ex.Message);
                }
            }
        }

        #endregion

        #region ImportarArticulos

        public ActionResult ImportarArticulos()
        {
            ImportarModel model = new ImportarModel();

            //using (var db = MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos))
            //{
            //    model.Serie = db.Series.Where(f => f.empresa == Empresa && f.entradasvarias == true)
            //                .Select(f => new SelectListItem() { Value = f.id, Text = f.descripcion }).ToList();
            //}

            return View("ImportarArticulos", model);
        }

        [HttpPost]
        public ActionResult ImportarArticulos(ImportarModel model)
        {
            var idPeticion = 0;
            var file = model.Fichero;
            char delimitador = model.Delimitador.ToCharArray()[0];                        

            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                    if (file.FileName.ToLower().EndsWith(".csv"))
                    {
                        var service = FService.Instance.GetService(typeof(ArticulosModel), ContextService) as ArticulosService;
                        StreamReader sr = new StreamReader(file.InputStream, Encoding.UTF8);
                        StringBuilder sb = new StringBuilder();
                        DataTable dt = new DataTable();
                        DataRow dr;
                        string s;
                        int j = 0;

                        dt.Columns.Add("TiempoFab");
                        dt.Columns.Add("CodArticulo");                        
                        dt.Columns.Add("Descripcion");
                        dt.Columns.Add("Descripcion2");
                        dt.Columns.Add("PartidaArancelaria");
                        dt.Columns.Add("Largo");
                        dt.Columns.Add("Ancho");
                        dt.Columns.Add("Grueso");
                        dt.Columns.Add("CosteMateriaPrima");
                        dt.Columns.Add("PorcentajeMerma");
                        dt.Columns.Add("CosteElaboracionMateriaPrima");
                        dt.Columns.Add("CosteFabricacion");
                        dt.Columns.Add("OtrosCostes");
                        dt.Columns.Add("CostesPortes");
                        dt.Columns.Add("CosteIndirecto");
                        dt.Columns.Add("Coste");
                        dt.Columns.Add("PVenta1");
                        dt.Columns.Add("PVenta2");
                        dt.Columns.Add("PVenta3");
                        dt.Columns.Add("UnidadMedida");//No se usa
                        dt.Columns.Add("KilosUd");
                        dt.Columns.Add("Rendimiento");
                        dt.Columns.Add("CodContable");//No se usa
                        dt.Columns.Add("ExistenciasMinimasMetros");
                        dt.Columns.Add("ExistenciasMaximasMetros");
                        dt.Columns.Add("Web");
                        //dt.Columns.Add("Notas");
                        dt.Columns.Add("PiezasCaja");
                        dt.Columns.Add("PrecioMinimoVenta");
                        dt.Columns.Add("ExcluirComisiones");
                        dt.Columns.Add("Labor");
                        dt.Columns.Add("MedidaLibre");
                        dt.Columns.Add("LoteUnico");//No se usa
                        dt.Columns.Add("Lote");//No se usa
                        //dt.Columns.Add("Relac");//No se usa
                        dt.Columns.Add("NomLabor");//No se usa
                        dt.Columns.Add("ExistenciasMinimasUnidades");
                        dt.Columns.Add("ExistenciasMaximasUnidades");
                        dt.Columns.Add("Stock");//No se usa
                        dt.Columns.Add("EsqFab");//No se usa
                        dt.Columns.Add("Grupo");//No se usa
                        dt.Columns.Add("ExentoRetencion");
                        dt.Columns.Add("FactprComp");//No se usa
                        dt.Columns.Add("Ean13");

                        while (!sr.EndOfStream)
                        {
                            while ((s = sr.ReadLine()) != null)
                            {
                                //Ignorar cabecera                    
                                if (j > 0 || !model.Cabecera)
                                {
                                    string[] str = s.Split(delimitador);
                                    dr = dt.NewRow();

                                    for (int i = 0; i < dt.Columns.Count; i++)
                                    {
                                        try
                                        {
                                            dr[dt.Columns[i]] = str[i].Replace("\"", string.Empty).ToString() ?? string.Empty;
                                        }
                                        catch (Exception ex)
                                        {
                                            ModelState.AddModelError("File", General.ErrorDelimitadorFormato);
                                            return View("ImportarArticulos", model);
                                        }
                                    }
                                    dt.Rows.Add(dr);
                                }
                                j++;
                            }
                        }
                        try
                        {
                            idPeticion = service.CrearPeticionImportacion(ContextService);
                            HostingEnvironment.QueueBackgroundWorkItem(async token => await GetAsyncArticulos(dt, idPeticion, token, model));
                            //service.Importar(dt, model.Seriecontable.ToString(), ContextService);
                            sr.Close();
                        }
                        catch (ValidationException ex)
                        {
                            if (string.IsNullOrEmpty(ex.Message))
                            {
                                TempData["Errors"] = null;
                            }
                            else
                            {
                                TempData["Errors"] = ex.Message;
                            }
                        }

                        //TempData["Success"] = "Importado correctamente!";
                        TempData["Success"] = "Ejecutando, proceso con id = " + idPeticion.ToString() + ", para comprobar su ejecución ir al menú de peticiones asíncronas";
                        return RedirectToAction("ImportarArticulos", "Importar");
                    }
                    else
                    {
                        ModelState.AddModelError("File", General.ErrorFormatoFichero);
                    }
                }
                else
                {
                    ModelState.AddModelError("File", General.ErrorFichero);
                }
            }

            return View("ImportarArticulos", model);
        }

        private async Task GetAsyncArticulos(DataTable dt, int idPeticion, CancellationToken cancellationToken, ImportarModel model)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var service = FService.Instance.GetService(typeof(ArticulosModel), ContextService) as ArticulosService)
                {
                    await Task.Run(() => service.Importar(dt, idPeticion, ContextService, model));
                    return;
                }

            }
            catch (TaskCanceledException tce)
            {

            }
            catch (Exception ex)
            {
                using (var service = FService.Instance.GetService(typeof(PeticionesAsincronasModel), ContextService) as PeticionesAsincronasService)
                {
                    service.CambiarEstado(EstadoPeticion.Error, idPeticion, ex.Message);
                }
            }
        }

        #endregion

        #region ImportarTerceros

        public ActionResult ImportarTerceros()
        {
            ImportarModel model = new ImportarModel();

            //using (var db = MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos))
            //{
            //    model.Serie = db.Series.Where(f => f.empresa == Empresa && f.entradasvarias == true)
            //                .Select(f => new SelectListItem() { Value = f.id, Text = f.descripcion }).ToList();
            //}

            //Ayuda
            var aux = model as IToolbar;
            aux.Toolbar.Acciones = HelpItem();
            return View("ImportarTerceros", model);
        }

        [HttpPost]
        public ActionResult ImportarTerceros(ImportarModel model)
        {
            var idPeticion = 0;
            var idPeticion2 = 0;
            var idPeticion3 = 0;
            var file = model.Fichero;
            char delimitador = model.Delimitador.ToCharArray()[0];

            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                    if (file.FileName.ToLower().EndsWith(".csv"))
                    {
                        var clienteService = FService.Instance.GetService(typeof(ClientesModel), ContextService) as ClientesService;
                        var proveedorService = FService.Instance.GetService(typeof(ProveedoresModel), ContextService) as ProveedoresService;
                        var acreedorService = FService.Instance.GetService(typeof(AcreedoresModel), ContextService) as AcreedoresService;
                        StreamReader sr = new StreamReader(file.InputStream, Encoding.GetEncoding(1252));
                        StringBuilder sb = new StringBuilder();
                        DataTable dt = new DataTable();
                        DataTable clientes = new DataTable();
                        DataTable proveedores = new DataTable();
                        DataTable acreedores = new DataTable();
                        DataRow dr;
                        string s;
                        int j = 0;

                        dt.Columns.Add("codigo");
                        dt.Columns.Add("provincia");
                        dt.Columns.Add("nombre");
                        dt.Columns.Add("nombre2");
                        dt.Columns.Add("direccion");
                        dt.Columns.Add("direc2");
                        dt.Columns.Add("poblacion");
                        dt.Columns.Add("contacto");
                        dt.Columns.Add("codpostal");
                        dt.Columns.Add("nprovincia");
                        dt.Columns.Add("paisiso");
                        dt.Columns.Add("tiponif");
                        dt.Columns.Add("nif");
                        dt.Columns.Add("idfiscal");
                        dt.Columns.Add("patente");
                        dt.Columns.Add("vacac1");
                        dt.Columns.Add("vacac2");
                        dt.Columns.Add("telefono");
                        dt.Columns.Add("movil");
                        dt.Columns.Add("telexfax");
                        dt.Columns.Add("diapago1");
                        dt.Columns.Add("diapago2");
                        dt.Columns.Add("dtopp");
                        dt.Columns.Add("dtocial");
                        dt.Columns.Add("tipoope");
                        dt.Columns.Add("forpago");
                        dt.Columns.Add("codagte");
                        dt.Columns.Add("codcomer");
                        dt.Columns.Add("czona");
                        dt.Columns.Add("codinco");
                        dt.Columns.Add("nptda");
                        dt.Columns.Add("portes");
                        dt.Columns.Add("ctransp");
                        dt.Columns.Add("portesm2");
                        dt.Columns.Add("banconom");
                        dt.Columns.Add("bancodir");
                        dt.Columns.Add("bancodi2");
                        dt.Columns.Add("bancopob");
                        dt.Columns.Add("iban");
                        dt.Columns.Add("bic");
                        dt.Columns.Add("notas");
                        dt.Columns.Add("acumulado");
                        dt.Columns.Add("clistat");
                        dt.Columns.Add("marcalis");
                        dt.Columns.Add("ciaseg");
                        dt.Columns.Add("codcseg");
                        dt.Columns.Add("ciasupl");
                        dt.Columns.Add("riescemp");
                        dt.Columns.Add("riessol");
                        dt.Columns.Add("riesccia");
                        dt.Columns.Add("riescom");
                        dt.Columns.Add("riespol");
                        dt.Columns.Add("riesfpag");
                        dt.Columns.Add("riesdia");
                        dt.Columns.Add("riesact");
                        dt.Columns.Add("fclasif");
                        dt.Columns.Add("fefecto");
                        dt.Columns.Add("fultsol");
                        dt.Columns.Add("fvalidez");
                        dt.Columns.Add("cobrador");
                        dt.Columns.Add("moneda");
                        dt.Columns.Add("criteriva");
                        dt.Columns.Add("iva");
                        dt.Columns.Add("rec");
                        dt.Columns.Add("tiporet");
                        dt.Columns.Add("retencion");
                        dt.Columns.Add("rsocial");
                        dt.Columns.Add("email");
                        dt.Columns.Add("web");
                        //dt.Columns.Add("relac");
                        dt.Columns.Add("diascomimp");
                        dt.Columns.Add("diasaplpag");
                        dt.Columns.Add("tarifa");
                        dt.Columns.Add("codtarifa");
                        dt.Columns.Add("codtarifac");
                        dt.Columns.Add("precioc");
                        dt.Columns.Add("preciov");
                        dt.Columns.Add("codex");
                        dt.Columns.Add("codecrm");
                        dt.Columns.Add("crm_pen");
                        dt.Columns.Add("noteidcrm1");
                        dt.Columns.Add("tipoter");
                        dt.Columns.Add("cligrupo");
                        dt.Columns.Add("riesgrupo");
                        dt.Columns.Add("penvto");
                        dt.Columns.Add("penvto2");
                        dt.Columns.Add("norecupe");
                        dt.Columns.Add("norecupe2");
                        dt.Columns.Add("penfac");
                        dt.Columns.Add("impagado");
                        dt.Columns.Add("totalrisk");
                        dt.Columns.Add("pmc");
                        dt.Columns.Add("bloqueado");
                        dt.Columns.Add("motbloqueo");
                        dt.Columns.Add("litigio");
                        dt.Columns.Add("tipocli");
                        dt.Columns.Add("lang");
                        dt.Columns.Add("ncopiasfac");
                        dt.Columns.Add("efactura");
                        dt.Columns.Add("fechaalta");
                        //copiamos el dt general al resto
                        clientes = dt.Copy();
                        proveedores = dt.Copy();
                        acreedores = dt.Copy();

                        while (!sr.EndOfStream)
                        {
                            while ((s = sr.ReadLine()) != null)
                            {
                                //Ignorar cabecera                    
                                if (j > 0 || !model.Cabecera)
                                {
                                    string[] str = s.Split(delimitador);
                                    dr = dt.NewRow();

                                    for (int i = 0; i < dt.Columns.Count; i++)
                                    {
                                        try
                                        {
                                            dr[dt.Columns[i]] = str[i].Replace("\"", string.Empty).ToString() ?? string.Empty;
                                        }
                                        catch (Exception ex)
                                        {
                                            ModelState.AddModelError("File", General.ErrorDelimitadorFormato);
                                            return View("ImportarTerceros", model);
                                        }
                                    }
                                    dt.Rows.Add(dr);
                                }
                                j++;
                            }
                        }
                        try
                        {
                            var tipo = 0;
                            var service = new TiposcuentasService(ContextService);
                            //obtenemos los diferentes clientes, proveedores y acreedores
                            foreach (DataRow row in dt.Rows)
                            {
                                var digitoscuenta = row["codigo"].ToString().Substring(0, 4);
                                var existeTipoCuenta = service.ExisteTipoCuenta(digitoscuenta);
                                if (existeTipoCuenta == true)
                                {
                                    tipo = service.GetTipoCuenta(digitoscuenta);
                                }

                                switch (tipo)
                                {
                                    case 0:
                                        clientes.ImportRow(row);
                                        break;
                                    case 1:
                                        proveedores.ImportRow(row);
                                        break;
                                    case 2:
                                        acreedores.ImportRow(row);
                                        break;
                                }
                            }

                            idPeticion = clienteService.CrearPeticionImportacion(ContextService);
                            HostingEnvironment.QueueBackgroundWorkItem(async token => await GetAsyncClientes(clientes, idPeticion, token, model));
                            idPeticion2 = proveedorService.CrearPeticionImportacion(ContextService);
                            HostingEnvironment.QueueBackgroundWorkItem(async token => await GetAsyncProveedores(proveedores, idPeticion2, token, model));
                            idPeticion3 = acreedorService.CrearPeticionImportacion(ContextService);
                            HostingEnvironment.QueueBackgroundWorkItem(async token => await GetAsyncAcreedores(acreedores, idPeticion3, token, model));
                            sr.Close();
                        }
                        catch (ValidationException ex)
                        {
                            if (string.IsNullOrEmpty(ex.Message))
                            {
                                TempData["Errors"] = null;
                            }
                            else
                            {
                                TempData["Errors"] = ex.Message;
                            }
                        }

                        var totalregistros = dt.Rows.Count;
                        var totalregistrosimportados = clientes.Rows.Count + proveedores.Rows.Count + acreedores.Rows.Count;
                        if (totalregistros != totalregistrosimportados)
                        {
                            TempData["Success"] = "Ejecutando, proceso con id = " + idPeticion.ToString() + " | " + idPeticion2.ToString() + " | " + idPeticion3.ToString() + ", para comprobar su ejecución ir al menú de peticiones asíncronas. Existen registros que no se van a importar por no coincidir con el tipo de cuenta de clientes, proveedores o acreedores. Revíselo una vez finalizadas las peticiones";
                        }
                        else
                        {
                            TempData["Success"] = "Ejecutando, proceso con id = " + idPeticion.ToString() + " | " + idPeticion2.ToString() + " | " + idPeticion3.ToString() + ", para comprobar su ejecución ir al menú de peticiones asíncronas";
                        }
                        //TempData["Success"] = "Importado correctamente!";
                        
                        return RedirectToAction("ImportarTerceros", "Importar");
                    }
                    else
                    {
                        ModelState.AddModelError("File", General.ErrorFormatoFichero);
                    }
                }
                else
                {
                    ModelState.AddModelError("File", General.ErrorFichero);
                }
            }

            return View("ImportarTerceros", model);
        }

        private async Task GetAsyncAcreedores(DataTable dt, int idPeticion, CancellationToken cancellationToken, ImportarModel model)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var service = FService.Instance.GetService(typeof(AcreedoresModel), ContextService) as AcreedoresService)
                {
                    await Task.Run(() => service.Importar(dt, idPeticion, ContextService, model));
                    return;
                }

            }
            catch (TaskCanceledException tce)
            {

            }
            catch (Exception ex)
            {
                using (var service = FService.Instance.GetService(typeof(PeticionesAsincronasModel), ContextService) as PeticionesAsincronasService)
                {
                    service.CambiarEstado(EstadoPeticion.Error, idPeticion, ex.Message);
                }
            }
        }

        private async Task GetAsyncClientes(DataTable dt, int idPeticion, CancellationToken cancellationToken, ImportarModel model)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var service = FService.Instance.GetService(typeof(ClientesModel), ContextService) as ClientesService)
                {
                    await Task.Run(() => service.Importar(dt, idPeticion, ContextService, model));
                    return;
                }

            }
            catch (TaskCanceledException tce)
            {

            }
            catch (Exception ex)
            {
                using (var service = FService.Instance.GetService(typeof(PeticionesAsincronasModel), ContextService) as PeticionesAsincronasService)
                {
                    service.CambiarEstado(EstadoPeticion.Error, idPeticion, ex.Message);
                }
            }
        }

        private async Task GetAsyncProveedores(DataTable dt, int idPeticion, CancellationToken cancellationToken, ImportarModel model)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var service = FService.Instance.GetService(typeof(ProveedoresModel), ContextService) as ProveedoresService)
                {
                    await Task.Run(() => service.Importar(dt, idPeticion, ContextService, model));
                    return;
                }

            }
            catch (TaskCanceledException tce)
            {

            }
            catch (Exception ex)
            {
                using (var service = FService.Instance.GetService(typeof(PeticionesAsincronasModel), ContextService) as PeticionesAsincronasService)
                {
                    service.CambiarEstado(EstadoPeticion.Error, idPeticion, ex.Message);
                }
            }
        }

        #endregion
    }
}