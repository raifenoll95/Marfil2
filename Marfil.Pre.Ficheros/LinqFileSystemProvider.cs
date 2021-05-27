using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using DevExpress.Web;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Model.Ficheros;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos.Helper;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;






namespace Marfil.Pre.Ficheros
{
    ///TODO EL: Revisar esta web https://demos.devexpress.com/MVCxFileManagerAndUploadDemos/FileManager/AzureProvider
    /// Si queremos añadir un proveedor de servicios de azure
    public class LinqFileSystemProvider : FileSystemProviderBase
    {
        private readonly string _empresa;
        private readonly string _rutaBase;
        private readonly FicherosService _ficherosService;
        private readonly CarpetasService _carpetasService;

        private readonly string _azureblob;
        
        public LinqFileSystemProvider(string bd,IContextService context, HttpServerUtilityBase server) : base(ConfigurationManager.AppSettings["FileManagerNodoRaiz"])
        {
            _ficherosService = FService.Instance.GetService(typeof(FicherosGaleriaModel), context) as FicherosService;
            _carpetasService = FService.Instance.GetService(typeof(CarpetasModel), context) as CarpetasService;
            
            _rutaBase = GetRootFolder(server, context.Empresa, bd);
            _empresa = context.Empresa;

            _azureblob = context.Azureblob;
        }

        public static string GetRootFolder(HttpServerUtilityBase server,string empresa, string bd)
        {
            return server.MapPath(Path.Combine(ConfigurationManager.AppSettings["RootFolderFicheros"], bd,empresa));
        }

        public override IEnumerable<FileManagerFile> GetFiles(FileManagerFolder folder)
        {
            return
                _ficherosService.GetFicherosDeCarpetaId(string.IsNullOrEmpty(folder.Id) ? Guid.Empty : new Guid(folder.Id))
                    .OrderBy(f => f.Nombre).Select(f => new FileManagerFile(this, folder, f.Nombre, f.Id.ToString()));
        }
        public override IEnumerable<FileManagerFolder> GetFolders(FileManagerFolder folder)
        {
            
            var list=    _carpetasService.GetSubcarpetasDeCarpetaId(folder.FullName);

            return        list.OrderBy(f => f.Nombre).Select(f => new FileManagerFolder(this, folder, f.Nombre,f.Id.ToString()));
        }
        public override bool Exists(FileManagerFile file)
        {
            return _ficherosService.ExisteFichero(file.FullName);
        }
        public override bool Exists(FileManagerFolder folder)
        {
            return _carpetasService.ExisteCarpeta(folder.FullName);
        }

        
        public override Stream ReadFile(FileManagerFile file)
        {
            var ficheroObj = _ficherosService.get(file.Id) as FicherosGaleriaModel;
            var fi=new FileInfo(ficheroObj.Nombre);

            if (String.IsNullOrEmpty(_azureblob))
            {
                return new MemoryStream(File.ReadAllBytes(Path.Combine(_rutaBase, string.Format("{0}{1}", ficheroObj.Id, fi.Extension))));
            }
            else
            {
                return _ficherosService.MemoryStreamAzure(_empresa, _azureblob, ficheroObj.Id, fi.Extension);
            }
        }

        public override void CreateFolder(FileManagerFolder parent, string name)
        {
            _carpetasService.create(new CarpetasModel()
            {
                Nombre = name,
                Id = Guid.NewGuid(),
                Fkcarpetas = new Guid(string.IsNullOrEmpty(parent.Id) ? Guid.Empty.ToString() : parent.Id),
                Ruta=Path.Combine(parent.FullName,name),
                Empresa= _empresa
            });
        }

        public override void RenameFile(FileManagerFile file, string name)
        {
            var model = _ficherosService.get(file.Id) as FicherosGaleriaModel;
            model.Nombre = name;
            model.Ruta = Path.Combine(file.Folder?.FullName ?? string.Empty, name);
            _ficherosService.edit(model);
        }
        public override void RenameFolder(FileManagerFolder folder, string name)
        {
            var model = _carpetasService.get(folder.Id) as CarpetasModel;
            model.Nombre = name;
            model.Ruta = Path.Combine(folder.Parent?.FullName ?? string.Empty, name);
            _carpetasService.edit(model);
        }

        public override  void UploadFile(FileManagerFolder folder, string fileName, Stream content)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var fi = new FileInfo(Path.Combine(_rutaBase, fileName));
                var idfichero = Guid.NewGuid();
                _ficherosService.create(new FicherosGaleriaModel()
                {
                    Id = idfichero,
                    Nombre = fileName,
                    Ruta = Path.Combine(folder.FullName, fi.Name),
                    Tipo = fi.Extension,
                    Fkcarpetas = new Guid(folder.Id),
                    Empresa = _empresa
                });
                fi.Directory.Create();

                if (String.IsNullOrEmpty(_azureblob))
                {
                    File.WriteAllBytes(Path.Combine(_rutaBase, string.Format("{0}{1}", idfichero, fi.Extension)), Funciones.ReadAllBytes(content));
                }
                else
                {
                    _ficherosService.AgregarFicheroAzure(_empresa, _azureblob, fi.Name, fi.Extension, Funciones.ReadAllBytes(content));
                }

                tran.Complete();
            }

        }
        public override void DeleteFile(FileManagerFile file)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var fi = new FileInfo(file.FullName);

                _ficherosService.delete(_ficherosService.get(file.Id));
                File.Delete(Path.Combine(_rutaBase, string.Format("{0}{1}", file.Id, fi.Extension)));
                tran.Complete();
            }
        }
        public override void DeleteFolder(FileManagerFolder folder)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                _carpetasService.delete(_carpetasService.get(folder.Id));
                tran.Complete();
            }
        }

        
    }
}
