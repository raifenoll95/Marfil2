using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Marfil.Dom.ControlsUI.Ayuda;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Converter;
using Marfil.Dom.Persistencia.Model.Configuracion.Empresa;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Login;
using Marfil.Inf.Genericos;
using Marfil.Inf.Genericos.Helper;
using Marfil.Inf.Genericos.NifValidators;
using Marfil.Inf.ResourcesGlobalization.Textos.MenuAplicacion;
using Resources;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Contabilidad;
using Marfil.Dom.Persistencia.Model.Contabilidad;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;

namespace Marfil.Dom.Persistencia.Helpers
{
    public class ApplicationHelper
    {
        public const int EspacioOrdenLineas = 20;
        public const int ALMACENDIRECCIONINT = 99;
        private readonly IContextService _context;
        #region CTR

        public ApplicationHelper(IContextService context)
        {
            _context = context;
        }

        #endregion

        #region Nif

        public ICheckPaisEuropeo CheckPaisEuropeoService(string codigopais)
        {
            return new CheckPaisEuropeo(_context, codigopais);
        }

        #endregion

        #region Permisions

        public struct stPermisos
        {
            public bool IsActivado { get; set; }
            public bool CanCrear { get; set; }
            public bool CanModificar { get; set; }
            public bool CanEliminar { get; set; }
            public bool CanBloquear { get; set; }
        }

        private string GetMenuFile()
        {
            var result = "MenuAplicacionPyme.xml";

            switch (_context.Tipolicencia)
            {
                case TipoLicencia.Avanzado:
                    result = "MenuAplicacionAvanzado.xml";
                    break;
                case TipoLicencia.Profesional:
                    result = "MenuAplicacionProfesional.xml";
                    break;
            }

            return result;
        }

        public stPermisos GetPermisosMenu(string menu)
        {
            var result = new stPermisos()
            {
                IsActivado = true,
                CanCrear = true,
                CanEliminar = true,
                CanModificar = true,
                CanBloquear = true
            };
            var user = _context;
            if (user.RoleId != Guid.Empty)
            {
                using (
                    var db = MarfilEntities.ConnectToSqlServer(_context.BaseDatos))
                {
                    var role = db.Roles.Find(user.RoleId);
                    var xml = role.permisos;
                    if (!string.IsNullOrEmpty(xml))
                    {
                        var service = new RolesConverterService(_context,db);
                        var rolemodel = service.CreateView(user.RoleId.ToString()) as RolesModel;
                        GetPermisos(rolemodel.Permisos.Items, menu, ref result);
                    }
                }

            }
            else
            {
                var xmlFile = _context.ServerMapPath(string.Format("~/App_Data/{0}", GetMenuFile()));
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlFile);

                var node = xmlDoc.SelectNodes("//item[@name='" + menu + "']");
                foreach (XmlNode item in node)
                {
                    result.CanCrear = Funciones.Qbool(item.Attributes["allowCreate"]?.Value ?? "true");
                    result.CanEliminar = Funciones.Qbool(item.Attributes["allowDelete"]?.Value ?? "true");
                    result.CanBloquear = Funciones.Qbool(item.Attributes["allowBlock"]?.Value ?? "true");
                    result.CanModificar = Funciones.Qbool(item.Attributes["allowUpdate"]?.Value ?? "true");
                }

            }


            return result;
        }

        private bool GetPermisos(IEnumerable<PermisosItemModel> permisos, string menu, ref stPermisos result)
        {
            foreach (var item in permisos)
            {
                if (item.Nombre.ToLower() == menu.ToLower())
                {
                    result.IsActivado = item.Visible;
                    result.CanEliminar = item.Eliminar;
                    result.CanCrear = item.Crear;
                    result.CanModificar = item.Modificar;
                    result.CanBloquear = item.Bloquear;
                    return true;
                }
            }

            foreach (var item in permisos)
            {
                if (item.Permisos.Any())
                {
                    if (GetPermisos(item.Permisos, menu, ref result))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion

        #region read App menu

        public IEnumerable<MenuItemsAplicacionModel> getMenuAplicacionModels(bool reglasAA)
        {


            var xmlFile = _context.ServerMapPath(string.Format("~/App_Data/{0}", GetMenuFile()));
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFile);

            var list = xmlDoc.SelectNodes("xml/item");

            var result = (from XmlNode item in list where !(Funciones.Qbool(item.Attributes["excluirConAA"]?.Value ?? "false") && reglasAA) select readItem(item, reglasAA)).ToList();

            return result;



        }

        public IEnumerable<MenuItemJavascriptModel> CreateMenuAplicacionJavascriptUsuario(bool reglasAA)
        {
            var menu = getMenuAplicacionModels(reglasAA);
            
            if (_context.RoleId != Guid.Empty)
            {
                using (var db = MarfilEntities.ConnectToSqlServer(_context.BaseDatos))
                {
                    var service = new RolesConverterService(_context,db);
                    var role = service.CreateView(_context.RoleId.ToString()) as RolesModel;
                    var permisos = role.Permisos.Items.Where(f => f.Visible == false);
                    foreach (var item in permisos)
                    {
                        RemoveItemMenu(menu, item.Nombre);
                    }

                    List<MenuItemsAplicacionModel> _aux = menu.Where(item => !item.items.Any() && string.IsNullOrEmpty(item.url)).ToList();
                    List<MenuItemsAplicacionModel> menuLst = menu as List<MenuItemsAplicacionModel>;
                    foreach (var item in _aux)
                        menuLst.Remove(item);

                }
            }


            var result = CreateSuperadminmenu(CreateMenuJavascript(menu.ToList()).ToList());
            

            return result;

        }

        private IEnumerable<MenuItemJavascriptModel> CreateMenuJavascript(IEnumerable<MenuItemsAplicacionModel> menu)
        {
            var result = new List<MenuItemJavascriptModel>();
            var helper = _context.GetUrlHelper();

            foreach (var item in menu)
            {

                var newItem = new MenuItemJavascriptModel();
                newItem.isseparator = item.isSeparator;
                newItem.icono = item.icono ?? string.Empty;
                newItem.text = item.texto;
                newItem.target = item.target;
                newItem.link = !string.IsNullOrEmpty(item.url) ? helper.Content(item.url) : string.Empty;
                if (item.items.Any())
                    newItem.items = CreateMenuJavascript(item.items);

                result.Add(newItem);


            }

            return result;
        }

        private IEnumerable<MenuItemJavascriptModel> CreateSuperadminmenu(
           IList<MenuItemJavascriptModel> menu)
        {
            var helper = _context.GetUrlHelper();
            
            if (_context.Id == Guid.Empty)
            {
                var configuracionItem = new MenuItemJavascriptModel()
                {
                    text = General.LblMenuSuperusuario,
                    link = "",
                    icono = "fa fa-user-plus"
                };

                //seguridad
                var configuracionSeguridadItem = new MenuItemJavascriptModel()
                {
                    text = General.LblSeguridad,
                    link = "",
                    icono = "fa fa-lock"
                };

                var configuracionSeguridadUsuariosItem = new MenuItemJavascriptModel()
                {
                    text = General.LblGestionUsuarios,
                    link = helper.Content("~/Usuarios/Index"),
                    icono = "fa fa-user"
                };

                var configuracionSeguridadGruposItem = new MenuItemJavascriptModel()
                {
                    text = General.LblGestionGrupos,
                    link = helper.Content("~/Grupos/Index"),
                    icono = "fa fa-users"
                };

                configuracionSeguridadItem.items = new[]
                {configuracionSeguridadUsuariosItem, configuracionSeguridadGruposItem};

                //end seguridad

                //Configuracion aplicacion

                var configuracionAplicacionItem = new MenuItemJavascriptModel()
                {
                    text = General.LblConfiguracion,
                    link = helper.Content("~/Configuracion/Index"),
                    icono = "fa fa-cogs"
                };

                //end Configuracion aplicacion

                //Configuracion empresas

                var configuracionEmpresasItem = new MenuItemJavascriptModel()
                {
                    text = General.LblEmpresas,
                    link = helper.Content("~/Empresas/Index"),
                    icono = "fa fa-university"
                };

                //end Configuracion empresas


                //Gestión de ejercicio

                var ejercicioItem = new MenuItemJavascriptModel()
                {
                    text = General.LblEjercicio,
                    link = helper.Content("~/Ejercicios/Index"),
                    icono = "fa fa-calculator"
                };

                //end Gestión de ejercicio

                #region Contabilidad

                var contabilidadItem = new MenuItemJavascriptModel()
                {
                    text = General.LblContabilidad,
                    link = "",
                    icono = "fa fa-calculator"
                };

                var contabilidadImportarItem = new MenuItemJavascriptModel()
                {
                    text = General.LblImportar,
                    link = "",
                    icono = "fa fa-cogs"
                };

                var contabilidadImportarPlanItem = new MenuItemJavascriptModel()
                {
                    text = General.LblPlanContable,
                    link = helper.Content("~/Cuentas/AsistenteCuentas"),
                    icono = "fa fa-user"
                };

                var contabilidadImportarDiarioItem = new MenuItemJavascriptModel()
                {
                    text = General.LblDiarioContable,
                    link = helper.Content("~/Movs/AsistenteMovs"),
                    icono = "fa fa-user"
                };

                var contabilidadSeccionesAnaliticasItem = new MenuItemJavascriptModel()
                {
                    text = General.LblGenerarSecciones,
                    link = helper.Content("~/SeccionesAnaliticas/AsistenteSeccionesAnaliticas"),
                    icono = "fa fa-user"
                };


                contabilidadItem.items = new[]
                {contabilidadImportarPlanItem, contabilidadImportarDiarioItem, contabilidadSeccionesAnaliticasItem};


                #endregion Contabilidad

                var importarStockItem = new MenuItemJavascriptModel()
                {
                    text = General.LblImportar2,
                    link = "",
                    icono = "fa fa-industry"
                };

                var importarArticulos = new MenuItemJavascriptModel()
                {
                    text = General.LblImportarArticulos,
                    link = helper.Content("~/Importar/ImportarArticulos"),
                    icono = "fa fa-star"
                };

                var importarStock = new MenuItemJavascriptModel()
                {
                    text = General.LblImportarStock,
                    link = helper.Content("~/Importar/ImportarStock"),
                    icono = "fa fa-star"
                };

                importarStockItem.items = new[]
                {importarArticulos, importarStock};

                var exportarItem = new MenuItemJavascriptModel()
                {
                    text = General.LblExportarClassic,
                    link = helper.Content("~/Exportar/Exportar"),
                    icono = "fa fa-user"
                };

                var peticionesAsincronas = new MenuItemJavascriptModel()
                {
                    text = General.LblPeticionesAsincronas,
                    link = helper.Content("~/PeticionesAsincronas/Index"),
                    icono = "fa fa-search"
                };

                configuracionItem.items = new[]
                {configuracionSeguridadItem, configuracionAplicacionItem, configuracionEmpresasItem, ejercicioItem, contabilidadItem, importarStockItem, exportarItem, peticionesAsincronas};

                menu.Insert(0, configuracionItem);
            }

            return menu;
        }
        private void RemoveItemMenu(IEnumerable<MenuItemsAplicacionModel> menu, string name)
        {
            MenuItemsAplicacionModel found = null;
            foreach (var item in menu)
            {
                if (item.name == name)
                {
                    found = item;
                }
                else RemoveItemMenu(item.items, name);
            }

            if (found != null)
            {
                var menuList = menu as IList<MenuItemsAplicacionModel>;
                menuList.Remove(found);
            }
        }

        private MenuItemsAplicacionModel readItem(XmlNode node, bool reglasAA)
        {
            var result = new MenuItemsAplicacionModel
            {
                isSeparator = node.Attributes["isseparator"] != null ? bool.Parse(node.Attributes["isseparator"].Value) : false,
                icono = node.Attributes["icono"]?.Value,
                name = node.Attributes["name"]?.Value,
                target = node.Attributes["target"]?.Value,
                url = node.Attributes["url"]?.Value,
                texto = node.Attributes["name"] != null ? Menuaplicacion.ResourceManager.GetString(node.Attributes["name"].Value) : string.Empty,
                AllowDelete = node.Attributes["allowDelete"] != null ? bool.Parse(node.Attributes["allowDelete"].Value) : true,
                AllowCreate = node.Attributes["allowCreate"] != null ? bool.Parse(node.Attributes["allowCreate"].Value) : true,
                AllowUpdate = node.Attributes["allowUpdate"] != null ? bool.Parse(node.Attributes["allowUpdate"].Value) : true,
                AllowBlock = node.Attributes["allowBlock"] != null ? bool.Parse(node.Attributes["allowBlock"].Value) : false,

                items = node.HasChildNodes
                    ? (from XmlNode item in node.SelectNodes("item") where !(Funciones.Qbool(item.Attributes["excluirConAA"]?.Value ?? "false") && reglasAA) select readItem(item, reglasAA)).ToList()
                    : Enumerable.Empty<MenuItemsAplicacionModel>()
            };

            return result;
        }

        #endregion

        #region Ayuda

        public AyudaModel GetAyudaModel()
        {
            //TODO EL: Guardar en cache
            var serializer = new Serializer<AyudaModel>();
            var fichero = _context.ServerMapPath(Path.Combine("~/App_data/configuracionayuda.xml"));
            if (File.Exists(fichero))
                return serializer.SetXml(File.ReadAllText(fichero));

            return new AyudaModel();

        }

        #endregion

        #region Configuracion

        internal bool ValidarEstado(string fkestado, MarfilEntities db, out string errormessage)
        {
            var result = true;
            errormessage = string.Empty;
            var vector = fkestado.Split('-');
            var documento = Funciones.Qint(vector[0]) ?? 99;
            var id = vector[1];
            var estadoObj = db.Estados.Single(f => f.documento == documento && f.id == id);
            var tipoestado = (TipoEstado)estadoObj.tipoestado;

            if (tipoestado == TipoEstado.Finalizado || tipoestado == TipoEstado.Anulado ||
                tipoestado == TipoEstado.Caducado)
            {
                errormessage = string.Format(General.ErrorModificacionNoValidaPorElEstadoActual, estadoObj.descripcion);
                result = false;


            }

            return result;
        }

        public  double Espesorfleje(MarfilEntities db = null)
        {

            var service = db == null ? new ConfiguracionService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)) : new ConfiguracionService(_context,db);
            return service.GetModel().Espesorfleje;


        }

        public  double Espesordisco(MarfilEntities db = null)
        {
            var service = db == null ? new ConfiguracionService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)) : new ConfiguracionService(_context,db);
            return service.GetModel().Espesordisco;
        }

        public  ConfiguracionModel GetConfiguracion(MarfilEntities db = null)
        {
            var service = db == null ? new ConfiguracionService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)) : new ConfiguracionService(_context,db);
            return service.GetModel();
        }

        public static string IdiomaPrincipal
        {
            get
            {
                try
                {
                    var user = HttpContext.Current.User as ICustomPrincipal;
                    var context = new LoginContextService(user.Empresa, user.BaseDatos);
                    var appService= new ApplicationHelper(context);
                    using (var service = new ConfiguracionService(context, MarfilEntities.ConnectToSqlServer(context.BaseDatos)))
                    {
                        var idioma = service.GetModel().Fkidioma1;
                        if (!string.IsNullOrEmpty(idioma))
                        {
                            var idiomas = appService.GetListIdiomas();
                            return idiomas.Single(f => f.Valor == idioma).Descripcion;
                        }
                    }
                }
                catch (Exception)
                {


                }


                return string.Empty;
            }
        }

        public static string IdiomaSecundario
        {
            get
            {
                try
                {
                    var user = HttpContext.Current.User as ICustomPrincipal;
                    var context = new LoginContextService(user.Empresa, user.BaseDatos);
                    var appService = new ApplicationHelper(context);
                    using (var service = new ConfiguracionService(context, MarfilEntities.ConnectToSqlServer(context.BaseDatos)))
                    {
                        var idioma = service.GetModel().Fkidioma2;
                        if (!string.IsNullOrEmpty(idioma))
                        {
                            var idiomas = appService.GetListIdiomas();
                            return idiomas.Single(f => f.Valor == idioma).Descripcion;
                        }
                    }
                }
                catch (Exception)
                {


                }


                return "2";
            }

        }

        public const string UsuariosAdministrador = "admin";
        public const string EmpresaMock = "mock";

        public  string GetPaisesDefecto()
        {
            
            if (_context != null)
            {
                var empresa = _context.Empresa;
                if (empresa != EmpresaMock)
                {
                    var obj = GetCurrentEmpresa();
                    return obj.Fkpais;
                }
            }

            return string.Empty;
        }

        #region CuentasEntradasySalidasVarias

        public string GetCuentaSalidasVariasAlmacen()
        {
            if (_context != null)
            {
                var empresa = _context.Empresa;
                if (empresa != EmpresaMock)
                {
                    var obj = GetCurrentEmpresa();
                    return obj.FkCuentaSalidasVariasAlmacen;
                }
            }
            return string.Empty;
        }
        public string GetCuentaEntradasVariasAlmacen()
        {
            if (_context != null)
            {
                var empresa = _context.Empresa;
                if (empresa != EmpresaMock)
                {
                    var obj = GetCurrentEmpresa();
                    return obj.FkCuentaEntradasVariasAlmacen;
                }
            }
            return string.Empty;
        }
        #endregion

        #region EmpresaModel
        public EmpresaModel GetCurrentEmpresa(string customempresa = "")
        {
            var empresa = customempresa;
            if (string.IsNullOrEmpty(customempresa))
            {
                
                if (_context != null)
                    empresa = _context.Empresa;
                else return null;
            }

            if (empresa != EmpresaMock)
            {
                if (_context.IsObjectDictionaryEnabled())
                {
                    if (_context.GetObject("_empresa_" + empresa) == null)
                    {
                        using (var service = FService.Instance.GetService(typeof(EmpresaModel), _context))
                        {
                           _context.SetObject("_empresa_" + empresa, service.get(empresa) as EmpresaModel);
                        }
                    }

                    return _context.GetObject("_empresa_" + empresa) as EmpresaModel;

                }
                else
                {
                    using (var service = FService.Instance.GetService(typeof(EmpresaModel), _context))
                    {
                        return service.get(empresa) as EmpresaModel;
                    }
                }



            }



            return null;
        }

        #endregion
        public EjerciciosModel GetCurrentEjercicio()
        {
            if (_context == null) return null;
            var empresa = _context.Empresa;
            var ejercicio = _context.Ejercicio;
            if (string.IsNullOrEmpty(ejercicio)) return null;
            var serviceEjercicio = FService.Instance.GetService(typeof(EjerciciosModel), _context) as EjerciciosService;
            if (_context.IsObjectDictionaryEnabled())
            {

                if (_context.GetObject("_ejercicio_" + ejercicio) == null)
                {
                    using (var service = FService.Instance.GetService(typeof(EjerciciosModel), _context) as EjerciciosService)
                    {
                        _context.SetObject("_ejercicio_" + ejercicio, service.get(ejercicio ?? serviceEjercicio.GetEjercicioDefecto(_context.Id, MarfilEntities.ConnectToSqlServer(_context.BaseDatos), _context.Empresa)) as EjerciciosModel);
                       
                    }
                }
                return _context.GetObject("_ejercicio_" + ejercicio) as EjerciciosModel;
                

            }
            else
            {
                using (var service = FService.Instance.GetService(typeof(EjerciciosModel), _context) as EjerciciosService)
                {
                    return service.get(ejercicio ?? serviceEjercicio.GetEjercicioDefecto(_context.Id, null, empresa)) as EjerciciosModel;
                }
            }

        }

        public string GetFiltroAcumulador(){

            using (var service = FService.Instance.GetService(typeof(GuiasBalancesModel), _context) as GuiasBalancesService)
            {
                return service.GetFiltroAcumulador();
            }

        }

        public  AlmacenesModel GetCurrentAlmacen()
        {
            
            if (_context == null) return null;
            var empresa = _context.Empresa;
            var almacen = _context.Fkalmacen;
            if (string.IsNullOrEmpty(almacen)) return new AlmacenesModel();
            var serviceAlmacen = FService.Instance.GetService(typeof(AlmacenesModel), _context) as AlmacenesService;
            if (_context.IsObjectDictionaryEnabled())
            {

                if (_context.GetObject("_almacen_" + almacen) == null)
                {
                    _context.SetObject("_almacen_" + almacen, serviceAlmacen.get(string.IsNullOrEmpty(almacen) ? serviceAlmacen.GetAlmacenDefecto(_context.Id, MarfilEntities.ConnectToSqlServer(_context.BaseDatos), _context.Empresa) : almacen) as AlmacenesModel);
                }
                return _context.GetObject("_almacen_" + almacen) as AlmacenesModel;
                

            }


            return serviceAlmacen.get(string.IsNullOrEmpty(almacen) ? serviceAlmacen.GetAlmacenDefecto(_context.Id, null, empresa) : almacen) as AlmacenesModel;



        }

        public DateTime? GetRecalculo()
        {
            using (var service = new SaldosAcumuladosPeriodosService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                var usuario = _context.Usuario;
                var ejercicio = int.Parse(_context.Ejercicio);
                return service.GetRecalculo(usuario,ejercicio);
            }
        }

        public FiltrosAcumulador GetFiltros()
        {
            using (var service = new SaldosAcumuladosPeriodosService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                var usuario = _context.Usuario;
                var ejercicio = int.Parse(_context.Ejercicio);
                return service.GetFiltros(usuario,ejercicio);
            }
        }

        #region MonedasModel
        public MonedasModel GetMonedaBase()
        {
            
            if (_context != null)
            {
                var empresa = _context.Empresa;
                if (empresa != EmpresaMock)
                {
                    var model = GetCurrentEmpresa();
                    using (var serviceMonedas = new MonedasService(_context,MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
                    {
                        var monedamodel = serviceMonedas.get(model.FkMonedaBase) as MonedasModel;
                        return monedamodel;
                    }

                }
            }

            return null;
        }

        public MonedasModel GetMonedaAdicional()
        {
            
            if (_context != null)
            {
                var empresa = _context.Empresa;
                if (empresa != EmpresaMock)
                {
                    var model = GetCurrentEmpresa();
                    if (!string.IsNullOrEmpty(model.FkMonedaAdicional))
                        using (var serviceMonedas = new MonedasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
                        {

                            var monedamodel = serviceMonedas.get(model.FkMonedaAdicional) as MonedasModel;
                            return monedamodel;
                        }

                }
            }

            return null;
        }
        #endregion
        #region GetList
        public IEnumerable<TarifasbaseModel> GetListTarifasBase(TipoFlujo flujo, MarfilEntities db = null)
        {
            var service = db == null ? new TarifasbaseService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)) : new TarifasbaseService(_context, db);
            return service.getAll().OfType<TarifasbaseModel>().Where(f => f.Tipoflujo == flujo);
        }
        public IEnumerable<TablasVariasGeneralModel> GetListOperaciones340()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListOperaciones340();
            }
        }
        public IEnumerable<TablasVariasMotivoBloqueoCuentasModel> GetListMotivosBloqueo()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListMotivosBloqueo();
            }
        }

        public IEnumerable<TablasVariasGeneralModel> GetListTiposEmpresas()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListTiposEmpresas();
            }
        }

        public IEnumerable<TablasVariasPaisesModel> GetListPaises()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListPaises();
            }
        }

        public IEnumerable<ProvinciasModel> GetListProvincias(string pais)
        {
            using (var service = new ProvinciasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetProvinciasPais(pais);
            }
        }

        //Municipios
        public IEnumerable<MunicipiosModel> GetListMunicipios(string provincia)
        {
            using (var service = new MunicipiosService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetMunicipiosProvincia(provincia);
            }
        }

        public IEnumerable<BancosModel> GetListBancos()
        {
            using (var service = new BancosService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetBancos();
            }
        }
        public IEnumerable<AlbaranesComprasModel> GetListAlbaranesImportar()
        {
            using (var service = new AlbaranesComprasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetAlbaranesImportar();
            }
        }
        public IEnumerable<TarifasModel> GetListTarifas()
        {
            using (var service = new TarifasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetTarifas();
            }
        }
        public double GetTotalRemesa(string referencia)
        {
            using (var service = new RemesasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetTotalRemesa(referencia);
            }
        }

        public string GetNombreCuenta(string cuenta)
        {
            using (var service = new RemesasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetNombreCuenta(cuenta);
            }
        }

        public IEnumerable<Marfil.Dom.Persistencia.Model.Documentos.CobrosYPagos.RemesasModel> GetDesglose(string referencia)
        {
            using (var service = new RemesasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetRemesas(referencia);
            }
        }

        public IEnumerable<CuadernosBancariosModel> GetListCuadernos(string tipoVencimiento)
        {
            using (var service = new CuadernosBancariosServices(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetCuadernos(tipoVencimiento);
            }
        }

        public IEnumerable<MapeoRemesasModel> GetEtiquetasCuadernos()
        {
            using (var service = new MapeoRemesasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetEtiquetasCampos();
            }
        }
        public IEnumerable<TablasVariasGeneralModel> GetListTiposVias()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListTiposVias();
            }
        }

        public IEnumerable<TablasVariasGeneralModel> GetListGruposFormasPago()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListGruposFormasPago();
            }
        }

        public IEnumerable<SituacionesTesoreriaModel> GetListSituaciones()
        {
            using (var service = new SituacionesTesoreriaService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListSituaciones();
            }
        }

        public IEnumerable<TablasVariasGeneralModel> GetListTiposUbicacion()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListTiposUbicacion();
            }
        }
        public IEnumerable<TablasVariasGeneralModel> GetListTiposDireccion()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListTiposDireccion();
            }
        }

        public IEnumerable<TablasVariasGeneralModel> GetListTiposContacto()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListTiposContacto();
            }
        }


        public IEnumerable<TablasVariasGeneralModel> GetListModosContacto()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListModosContacto();
            }
        }

        public IEnumerable<TablasVariasCargosEmpresaModel> GetListCargosEmpresa()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListCargosEmpresa();
            }
        }

        public IEnumerable<TablasVariasGeneralModel> GetListCalificacionComercial()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListCalificacionComercial();
            }
        }

        public IEnumerable<TablasVariasGeneralModel> GetListTipograno()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListTipograno();
            }
        }

        public IEnumerable<TablasVariasGeneralModel> GetListTono()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListTono();
            }
        }

        public IEnumerable<TablasVariasTiposAlbaranesModel> GetListTiposAlbaranes()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListTiposAlbaranes();
            }
        }


        public IEnumerable<TablasVariasGeneralModel> GetListTiposObras()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListTiposObras();
            }
        }

        public IEnumerable<TablasVariasGeneralModel> GetListCanales()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListCanales();
            }
        }

        public IEnumerable<TablasVariasGeneralModel> GetListMotivosDevolucion()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListMotivosDevolucion();
            }
        }

        public IEnumerable<TablasVariasGeneralModel> GetListMotivosRectificacion()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListMotivosDevolucion();
            }
        }

        public IEnumerable<TablasVariasGeneralModel> GetListTipoTransporte()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListTipoTransporte();
            }
        }



        public IEnumerable<TablasVariasGeneralModel> GetListIdiomas()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListIdiomas();
            }
        }

        public IEnumerable<TablasVariasGeneralModel> GetListEstadoCivil()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListEstadoCivil();
            }
        }

        public IEnumerable<TablasVariasGeneralModel> GetListFamiliaMateriales()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListFamiliaMateriales();
            }
        }

        public IEnumerable<FamiliasproductosModel> GetListFamiliaArticulos()
        {
            using (var service = new FamiliasproductosService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListFamiliaArticulos();
            }
        }

        public IEnumerable<AgentesModel> GetListAgentes()
        {
            using (var service = new AgentesService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListAgentes();
            }
        }

        /*
        public IEnumerable<TablasVariasGeneralModel> GetListGrupoMateriales()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListGrupoMateriales();
            }
        }
        */

        public bool GetActualizarCuenta(int circuito)
        {
            using (var service = new CircuitosTesoreriaCobrosService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetActualizarCuenta(circuito);
            }
        }

        public bool GetSolicitarDatos(int circuito)
        {
            using (var service = new CircuitosTesoreriaCobrosService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetSolicitarDatos(circuito);
            }
        }

        public IEnumerable<TablasVariasGeneralModel> GetListLabores()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListLabores();
            }
        }

        public IEnumerable<TablasVariasGeneralModel> GetListSeccionesProduccion()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListSeccionesProduccion();
            }
        }

        public IEnumerable<TablasVariasGeneralModel> GetListFamiliaProveedor()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListFamiliaProveedor();
            }
        }

        public IEnumerable<TablasVariasGeneralModel> GetListZonaClienteProveedor()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListZonaClienteProveedor();
            }
        }

        public IEnumerable<TablasVariasGeneralModel> GetListTiposDocumentos()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetTiposDocumentos();
            }
        }

        //Rai
        public IEnumerable<RolesModel> GetGruposUsuarios()
        {
            using (var service = new GruposService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                var grupos = service.getAll().Select(f => (RolesModel)f);
                return grupos;
            }
        }
         

        public IEnumerable<TablasVariasGeneralModel> GetListTiposDocumentosContables()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetTiposDocumentosContables();
            }
        }


        public IEnumerable<TablasVariasGeneralModel> GetListUnidadNegocio()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListUnidadNegocio();
            }
        }


        public IEnumerable<TablasVariasGeneralModel> GetListIncoterm()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListIncoterm();
            }
        }


        public IEnumerable<TablasVariasGeneralModel> GetListSituacionComision()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListSituacionComision();
            }
        }

        public IEnumerable<TipoInforme> GetListTipoInforme()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListTipoInforme();
            }
        }
        public IEnumerable<TipoGuia> GetListTipoGuia()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListTipoGuia();
            }
        }

        public IEnumerable<SelectListItem> GetListTipoGuiaCTPG()
        {
            List<SelectListItem> List = new List<SelectListItem>();

            List.Add(new SelectListItem() { Value = "1", Text = "Abreviada" });
            List.Add(new SelectListItem() { Value = "3", Text = "COOP_ABREVIA" });
            List.Add(new SelectListItem() { Value = "4", Text = "COOP_NORMAL" });
            List.Add(new SelectListItem() { Value = "5", Text = "NORMAL" });
            List.Add(new SelectListItem() { Value = "6", Text = "PYME" });

            return List;
        }

        public IEnumerable<SelectListItem> GetListTipoGuiaPG()
        {
            List<SelectListItem> List = new List<SelectListItem>();

            List.Add(new SelectListItem() { Value = "0", Text = "Abreviada" });
            List.Add(new SelectListItem() { Value = "2", Text = "COOP_ABREVIA" });
            List.Add(new SelectListItem() { Value = "3", Text = "COOP_NORMAL" });
            List.Add(new SelectListItem() { Value = "4", Text = "NORMAL" });
            List.Add(new SelectListItem() { Value = "5", Text = "PYME" });

            return List;
        }

        public IEnumerable<SelectListItem> GetListTipoGuiaBALCA()
        {
            List<SelectListItem> List = new List<SelectListItem>();

            List.Add(new SelectListItem() { Value = "1", Text = "Abreviado" });
            List.Add(new SelectListItem() { Value = "2", Text = "COOP_ABREVIA" });
            List.Add(new SelectListItem() { Value = "3", Text = "COOP_NORMAL" });
            List.Add(new SelectListItem() { Value = "4", Text = "NORMAL" });
            List.Add(new SelectListItem() { Value = "5", Text = "PYME" });

            return List;
        }


        public IEnumerable<SelectListItem> GetListActpas()
        {
            List<SelectListItem>  ListActpas = new List<SelectListItem>();

            ListActpas.Add(new SelectListItem() { Value = "A", Text = "Activo" });
            ListActpas.Add(new SelectListItem() { Value = "P", Text = "Pasivo" });

            return ListActpas;
        }

        public IEnumerable<SelectListItem> GetListDetfor()
        {
            List<SelectListItem> ListFormula = new List<SelectListItem>();

            ListFormula.Add(new SelectListItem() { Value = "D", Text = "Detalle" });
            ListFormula.Add(new SelectListItem() { Value = "T", Text = "Título" });
            ListFormula.Add(new SelectListItem() { Value = "F", Text = "Fórmula" });
            ListFormula.Add(new SelectListItem() { Value = "G", Text = "Gran Total" });
            ListFormula.Add(new SelectListItem() { Value = "Z", Text = "Gran Título" });

            return ListFormula;
        }

        public bool ExistenCuentasNoAsignadas()
        {
            var cuentas = false;
            using (var service = new GuiasBalancesService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                cuentas = service.HayCuentasNoAsignadas();
            }

            return cuentas;
        }

        public string TextRecalculoPYG(FiltrosAcumulador filtrosAcumulador)
        {
            var text = "";
            using (var service = new GuiasBalancesService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                text = service.TextRecalculoPYG(filtrosAcumulador);
            }

            return text;
        }

        public IEnumerable<TablasVariasGeneralModel> GetListGrupoIncidencias()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListGrupoIncidencias();
            }
        }


        public IEnumerable<TablasVariasGeneralModel> GetListTiposContrato()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListTiposContrato();
            }
        }


        public IEnumerable<TablasVariasTiposNif> GetListTiposNif(MarfilEntities db = null)
        {
            var service = db == null ? new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)) : new TablasVariasService(_context,db);
            var result = service.GetListTiposNif();

            if (db == null)
                service.Dispose();

            return result;
        }

        public IEnumerable<TablasVariasGeneralModel> GetListCanalContable()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListCanalContable();
            }
        }

        
        public IEnumerable<TablasVariasGeneralModel> GetListTipoFactura()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListTipoFactura();
            }
        }

        public IEnumerable<TablasVariasGeneralModel> GetListRegimenEspecialEmitidas()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListRegimenEspecialEmitidas();
            }
        }



        public IEnumerable<TablasVariasGeneralModel> GetListRegimenEspecialRecibidas()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListRegimenEspecialRecibidas();
            }
        }


        public IEnumerable<TablasVariasGeneralModel> GetListGrupoSecciones()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListGrupoSecciones();
            }
        }

        #region Contabilidad
        public IEnumerable<TablasVariasGeneralModel> GetListDescripcionAsientos()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListDescripcionAsientos();
            }
        }

        public IEnumerable<TablasVariasTiposAsientosModel> GetListTiposAsientos()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListTiposAsientos();
            }
        }

        public IEnumerable<TablasVariasGeneralModel> GetListCanalesContables()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListCanalContable();
            }
        }

        public IEnumerable<TablasVariasGeneralModel> GetListComentariosAsientos()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListComentariosAsientos();
            }
        }

        #endregion Contabilidad

        public IEnumerable<TablasVariasGeneralModel> GetListAcciones()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListAcciones();
            }
        }

        public IEnumerable<TablasVariasGeneralModel> GetListReacciones()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListReacciones();
            }
        }

        public IEnumerable<TablasVariasGeneralModel> GetListMargenTiempo()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetListMargenTiempo();
            }
        }
        #endregion
        #endregion

        #region Empresas

        //Rai
        public IEnumerable<Tuple<string, string>> GetEmpresas()
        {
            var user = _context;
            if (user != null)
            {
                var empresa = user.Empresa;
                if (empresa != EmpresaMock)
                    using (var service = FService.Instance.GetService(typeof(EmpresaModel), _context))
                    {
                        var serviceusuarios = FService.Instance.GetService(typeof(UsuariosModel), _context);
                        List<Tuple<string, string>> empresas = new List<Tuple<string, string>>();

                        if (_context.Usuario == UsuariosAdministrador)
                        {
                            empresas = service.getAll().Select(f => new Tuple<string, string>(((EmpresaModel)f).Id, ((EmpresaModel)f).Nombre)).ToList();
                        }

                        else
                        {
                            var usuario = serviceusuarios.getAll().Where(f => ((UsuariosModel)f).Usuario == _context.Usuario).SingleOrDefault() as UsuariosModel;
                            var nivelusuario = usuario.Nivel;
                            var list = service.getAll();
                            foreach (var item in list)
                            {
                                if (Int32.Parse(((EmpresaModel)item).Nivel) <= nivelusuario)
                                {
                                    empresas.Add(new Tuple<string, string>(((EmpresaModel)item).Id, ((EmpresaModel)item).Nombre));
                                }
                            }
                        } 

                        return empresas;
                    }
            }

            return Enumerable.Empty<Tuple<string, string>>();
        }

        #endregion

        #region Ejercicios

        public  IEnumerable<Tuple<string, string>> GetEjercicios()
        {
            var user = _context;
            if (user != null)
            {
                var empresa = user.Empresa;
                if (empresa != EmpresaMock)
                    using (var service = FService.Instance.GetService(typeof(EjerciciosModel), _context) as EjerciciosService)
                    {
                        return service.getAll().Select(f => new Tuple<string, string>(((EjerciciosModel)f).Id.ToString(), ((EjerciciosModel)f).Descripcion));

                    }
            }

            return Enumerable.Empty<Tuple<string, string>>();
        }

        public IEnumerable<string> GetEjercicio()
        {
            using (var service = FService.Instance.GetService(typeof(EjerciciosModel), _context) as EjerciciosService)
            {
                return service.GetEjercicio(_context.Ejercicio);
            }
        }


        #endregion

        #region Configuraciones niveles cuentas

        public  int DigitosCuentas(string empresa = "", MarfilEntities db = null)
        {
            if (string.IsNullOrEmpty(empresa))
            {
                var user = _context;
                if (user != null)
                {
                    empresa = user.Empresa;
                }
            }

            if (empresa != EmpresaMock)
            {
                var service = db == null
                    ? FService.Instance.GetService(typeof(EmpresaModel), _context)
                    : FService.Instance.GetService(typeof(EmpresaModel), _context, db);
                var empresaModel = service.get(empresa) as EmpresaModel;
                return Funciones.Qint(empresaModel.DigitosCuentas).Value;
            }

            return 0;
            //throw new Exception("No hay digitos de cuentas configurados en la empresa");
        }

        public  int NivelesCuentas(string empresa = "", MarfilEntities db = null)
        {
            if (string.IsNullOrEmpty(empresa))
            {
                var user = _context;
                if (user != null)
                {
                    empresa = user.Empresa;
                }
            }


            if (empresa != EmpresaMock)
            {
                var service = db == null ? FService.Instance.GetService(typeof(EmpresaModel), _context) : FService.Instance.GetService(typeof(EmpresaModel), _context, db);

                var empresaModel = service.get(empresa) as EmpresaModel;
                return Funciones.Qint(empresaModel.NivelCuentas).Value;
            }



            throw new Exception("No hay niveles de cuentas configurados en la empresa");


        }

        #endregion

        #region Articulos

        public IEnumerable<TarifasSistemaArticulosViewModel> GetTarifasSistema(TipoFlujo tipoflujo, bool soloactivados = true, string empresa = "")
        {
            if (string.IsNullOrEmpty(empresa))
            {
                var user = _context;
                if (user != null)
                    empresa = user.Empresa;
            }

            using (var service = FService.Instance.GetService(typeof(TarifasModel), _context))
            {
                var vector = service.getAll()
                    .OfType<TarifasModel>()
                    .Where(f => f.Tipotarifa == TipoTarifa.Sistema && f.Empresa == empresa && f.Tipoflujo == tipoflujo);

                if (soloactivados)
                    vector = vector.Where(f => f.Bloqueado == false);

                return vector.Select(f => new TarifasSistemaArticulosViewModel() { Id = f.Id, Descripcion = f.Descripcion, Precio = 0, Obligatorio = f.Precioobligatorio });


            }
        }

        public IEnumerable<TarifasEspecificasArticulosViewModel> GetTarifasEspecificas(TipoFlujo tipoflujo, string articulo, string empresa = "")
        {
            if (string.IsNullOrEmpty(empresa))
            {
                var user = _context;
                if (user != null)
                    empresa = user.Empresa;
            }

            using (var service = FService.Instance.GetService(typeof(TarifasModel), _context) as TarifasService)
            {
                return service.GetTarifasEspecificas(tipoflujo, articulo, empresa);
            }
        }

        public IEnumerable<TarifasModel> GetTarifasCuenta(TipoFlujo tipo, string cuenta, string empresa = "")
        {
            if (string.IsNullOrEmpty(empresa))
            {
                var user = _context;
                if (user != null)
                    empresa = user.Empresa;
            }

            using (var service = FService.Instance.GetService(typeof(TarifasModel), _context) as TarifasService)
            {
                return service.getAll().OfType<TarifasModel>().Where(f => f.Bloqueado == false && f.Tipoflujo == tipo && (string.IsNullOrEmpty(f.Fkcuentas) || f.Fkcuentas == cuenta));
            }
        }

        #endregion

        #region Criterios agrupacion

        public  IEnumerable<CriteriosagrupacionModel> GetListCriteriosagrupacion()
        {
            var user = _context;
            if (user != null)
            {
                var empresa = user.Empresa;
                if (empresa != EmpresaMock)
                    using (var service = FService.Instance.GetService(typeof(CriteriosagrupacionModel), _context) as CriteriosagrupacionService)
                    {
                        return service.getAll().Select(f => (CriteriosagrupacionModel)f);

                    }
            }

            return Enumerable.Empty<CriteriosagrupacionModel>();
        }

        #endregion

        #region Almacenes

        public  IEnumerable<Tuple<string, string>> GetAlmacenes()
        {
            var user = _context;
            if (user != null)
            {
                var empresa = user.Empresa;
                if (empresa != EmpresaMock)
                    using (var service = FService.Instance.GetService(typeof(AlmacenesModel), _context) as AlmacenesService)
                    {
                        return service.getAll().Select(f => new Tuple<string, string>(((AlmacenesModel)f).Id.ToString(), ((AlmacenesModel)f).Descripcion));

                    }
            }

            return Enumerable.Empty<Tuple<string, string>>();
        }

        public  IEnumerable<AlmacenesZonasModel> GetAlmacenesZonas()
        {
            var user = _context;
            if (user != null)
            {
                var empresa = user.Empresa;
                if (empresa != EmpresaMock)
                    using (var service = FService.Instance.GetService(typeof(AlmacenesModel), _context) as AlmacenesService)
                    {
                        var almacenModel = service.get(user.Fkalmacen) as AlmacenesModel;
                        return almacenModel.Lineas;
                    }
            }

            return Enumerable.Empty<AlmacenesZonasModel>();
        }

        #endregion

        #region Listados

        public IEnumerable<MenuItemJavascriptModel> GetListadosMenus()
        {

            var user = _context;
            IEnumerable<PermisosItemModel> permisos = new List<PermisosItemModel>();
            if (user.RoleId != Guid.Empty)
            {
                using (var db = MarfilEntities.ConnectToSqlServer(_context.BaseDatos))
                {
                    var service = new RolesConverterService(_context,db);
                    var role = service.CreateView(user.RoleId.ToString()) as RolesModel;
                    permisos = role.Permisos.Items.Where(f => f.Visible == false);
                }
            }

            var xmlFile = _context.ServerMapPath(string.Format("~/App_Data/{0}", GetMenuFile()));
            var xmlDoc = new XmlDocument();
            //xmlDoc.Load(xmlFile);
            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.IgnoreComments = true;
            using (XmlReader reader = XmlReader.Create(xmlFile, readerSettings))
            {
                xmlDoc.Load(reader);
            }

            var list = xmlDoc.SelectNodes(@"//*[@name='listado']");
            var result = new List<MenuItemJavascriptModel>();
            foreach (XmlNode padre in list)
            {
                var textopadre = "";
                if (padre.ParentNode != null)
                {
                    textopadre = padre.ParentNode.Attributes["name"] != null
                   ? Menuaplicacion.ResourceManager.GetString(padre.ParentNode.Attributes["name"].Value)
                   : string.Empty;

                    if (padre.ParentNode.ParentNode != null)
                    {

                        var nodoraiz = padre.ParentNode.ParentNode;
                            var raiztextopadre = nodoraiz.Attributes["name"] != null
                   ? Menuaplicacion.ResourceManager.GetString(nodoraiz.Attributes["name"].Value)+". "
                   : string.Empty;
                        textopadre = string.Format("{0} {1}", raiztextopadre, textopadre);
                    }
                }

                result.Add(new MenuItemJavascriptModel()
                {
                    link = "",
                    text = textopadre,
                    isseparator = true
                });

                foreach (XmlNode node in padre.ChildNodes)
                {
                    if (!permisos.Any(f => f.Nombre == node.Attributes["name"].Value))
                    {
                        var url = (node.Attributes["url"]?.Value ?? string.Empty).Split('/').Last();
                        var texto = node.Attributes["name"] != null
                            ? Menuaplicacion.ResourceManager.GetString(node.Attributes["name"].Value)
                            : string.Empty;
                        texto = string.Format("{0}: {1}", textopadre, texto);

                        result.Add(new MenuItemJavascriptModel()
                        {
                            link = url,
                            text = texto
                        });
                    }
                    
                }
               


            }

            return result;
        }
        #endregion

        #region crm2130

        public IEnumerable<TablasVariasGeneralModel> GetCRMValores()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetCRMValores();
            }
        }

        public IEnumerable<TablasVariasGeneralModel> GetClasificacionArticulos()
        {
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                return service.GetClasificacionArticulos();
            }
        }

        #endregion




    }
}
