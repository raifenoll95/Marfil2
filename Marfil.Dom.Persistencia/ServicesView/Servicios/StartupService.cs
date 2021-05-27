using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Configuracion.Empresa;
using Marfil.Dom.Persistencia.Model.Configuracion.Planesgenerales;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Startup;
using System.Transactions;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using Marfil.Inf.Genericos.Helper;
using System.Threading;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public class StartupService : IDisposable
    {
        #region Members

        private readonly string _database;
        private readonly MarfilEntities _db;
        private IContextService _context;

        private readonly string _azureblob;
        #endregion

        #region Properties

        public MarfilEntities Db
        { get { return _db; } }

        #endregion

        public StartupService(IContextService context,string database)
        {
            _database = database;
            _context = context;
            _db = MarfilEntities.ConnectToSqlServer(database);
            //_azureblob = context.Azureblob;
        }

        #region Admin

        public bool ExisteAdmin()
        {
            return _db.Administrador.Any();
        }

        public void CreateAdmin(string password)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                if (!_db.Administrador.Any())
                {
                    if (string.IsNullOrEmpty(password))
                        throw new ValidationException("El password no puede estar vacío");

                    var item = _db.Administrador.Create();
                    item.password = password;
                    _db.Administrador.Add(item);

                    var usuarioAministrador = _db.Usuarios.Create();
                    usuarioAministrador.id= Guid.Empty;
                    usuarioAministrador.usuario = ApplicationHelper.UsuariosAdministrador;
                    usuarioAministrador.password = password;
                    usuarioAministrador.nombre= ApplicationHelper.UsuariosAdministrador;
                    _db.Usuarios.Add(usuarioAministrador);
                    _db.SaveChanges();
                    tran.Complete();
                }

                
            }
                

        }

        #endregion

        #region Datos defecto

        public IEnumerable<DatosDefectoItemModel> GetDatosDefecto()
        {
            var xmlFile = _context.ServerMapPath("~/App_Data/startupData.xml");
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFile);

            var nodeList = xmlDoc.SelectNodes("xml/item");

            return (from XmlNode node in nodeList
                    select new DatosDefectoItemModel()
                    {
                        Descripcion = node["descripcion"].InnerText,
                        Titulo = node["titulo"].InnerText,
                        Id = node.Attributes["id"].Value,
                        Fichero = node["data"].InnerText,
                        Selected = false,
                        Readonly = node.Attributes["readonly"] != null && bool.Parse(node.Attributes["readonly"].Value)
                    }).ToList();
        }

        public void CreateDatosDefecto(IEnumerable<DatosDefectoItemModel> entidades)
        {
            using (var service = new FStartup(_context,_db))
            {
                foreach (var item in entidades.Where(f => !string.IsNullOrEmpty(f.Id)))
                {
                   var itemService = service.CreateService(item.Id);
                    if (itemService is TablaVariaStartup)
                    {
                        var xmlFile = _context.ServerMapPath("~/App_Data/startupData.xml");
                        var xmlDoc = new XmlDocument();
                        xmlDoc.Load(xmlFile);
                        var tservice = itemService as TablaVariaStartup;
                        var node = xmlDoc.SelectSingleNode("xml/item[@id='tablasvarias']");
                        foreach (XmlNode fichero in node.SelectNodes("data"))
                        {
                            tservice.Id = fichero.Attributes["id"].Value;
                            tservice.Nombre = fichero.Attributes["nombre"].Value;
                            tservice.Clase = fichero.Attributes["clase"].Value;
                            tservice.Tipo = (TipoTablaVaria) Funciones.Qint(fichero.Attributes["tipo"].Value).Value;
                            tservice.NoEditable = fichero.Attributes["noeditable"].Value == "true";
                            tservice.CrearDatos(fichero.InnerText);
                        }
                    }
                    else
                    {
                        itemService.CrearDatos(item.Fichero);
                    }
                }
            }
        }       

        #endregion

        #region Empresa

        public void CreateEmpresa(EmpresaModel model)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                try
                {
                    using (var service = new EmpresasService(_context,_db))
                    {
                        service.create(model);
                    }
                    tran.Complete();
                }
                catch (Exception)
                {
                    throw;
                }

            }
        }

        

        #endregion

        public void Dispose()
        {
            _db?.Dispose();
        }
    }
}
