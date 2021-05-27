using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Ficheros;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos.Helper;


using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public struct StFicherosDocumentos
    {
        public string Nombre { get; set; }
        public Stream Datos { get; set; }
    }

    public interface IFicherosService
    {

    }

    public class FicherosService : GestionService<FicherosGaleriaModel, Ficheros>, IFicherosService
    {
        // Azure CluodStorageAccount
        // Por falta de tiempo se ha declarado variable constante
        //  Pendiente mejora para poner en configuración  no visible o codificada
        private const string strCloudStorageAccount = "DefaultEndpointsProtocol=https;AccountName=marfilnetstorage;AccountKey=u1biAKe2dsaciMkYc4/r3Exp/sD0zxlfdhP0Tu8Vq/gDIZCgpkhBopBSel/XPbCwYfwClSQtqUvV85rAYWo/FQ==;EndpointSuffix=core.windows.net";

        #region CTR

        public FicherosService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        #region Api

        public bool ExisteFichero(string fullname)
        {
            return _db.Ficheros.Any(f => f.ruta == fullname && f.empresa==Empresa);
        }

        public FicherosGaleriaModel GetFichero(string fullname)
        {
            if(ExisteFichero(fullname))
                return _converterModel.GetModelView(_db.Ficheros.Where(f => f.ruta == fullname && f.empresa == Empresa).FirstOrDefault()) as FicherosGaleriaModel;
            return null;
        }

        public IEnumerable<FicherosGaleriaModel> GetFicherosDeCarpetaId(Guid idcarpeta)
        {
            return _db.Ficheros.Where(f => f.fkcarpetas == idcarpeta && f.empresa == Empresa).ToList().Select(f=>_converterModel.GetModelView(f) as FicherosGaleriaModel);
        }

        public void DeleteFichero(string id,string rootpath)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var ficheroModel = get(id) as FicherosGaleriaModel;
                var fi =new FileInfo(ficheroModel.Nombre);

                delete(ficheroModel);

                File.Delete(Path.Combine(rootpath, string.Format("{0}{1}", ficheroModel.Id, fi.Extension)));
                
                tran.Complete();
            }
        }

        #endregion

        #region Azure

        public void DeleteFicheroAzure(string id, string empresa, string azureblob)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                string myBlobContainerReferenceKey = azureblob + empresa;
                var ficheroModel = get(id) as FicherosGaleriaModel;
                var fi = new FileInfo(ficheroModel.Nombre);

                delete(ficheroModel);

                var StorageAccount = CloudStorageAccount.Parse(strCloudStorageAccount);

                CloudBlobClient BlobClient = StorageAccount.CreateCloudBlobClient();
                CloudBlobContainer Content = BlobClient.GetContainerReference(myBlobContainerReferenceKey);
                CloudBlockBlob blockBlob = Content.GetBlockBlobReference(string.Format("{0}{1}", ficheroModel.Id, fi.Extension));
                blockBlob.Delete();

                tran.Complete();
            }
        }


        public Byte[] ReadAllBytesAzure(string empresa,string azureblob,Guid id, string extension)
        {
            string myBlobContainerReferenceKey = azureblob + empresa;

            var StorageAccount = CloudStorageAccount.Parse(strCloudStorageAccount);

            CloudBlobClient BlobClient = StorageAccount.CreateCloudBlobClient();
            CloudBlobContainer Content = BlobClient.GetContainerReference(myBlobContainerReferenceKey);
            CloudBlockBlob blockBlob = Content.GetBlockBlobReference(string.Format("{0}{1}", id, extension));

            blockBlob.FetchAttributes();
            long fileByteLength = blockBlob.Properties.Length;
            Byte[] myByteArray = new Byte[fileByteLength];
            blockBlob.DownloadToByteArray(myByteArray, 0);

            return myByteArray;
        }

        public Stream MemoryStreamAzure(string empresa, string azureblob, Guid id, string extension)
        {
            string myBlobContainerReferenceKey = azureblob + empresa;
            Stream outPutStream=  new MemoryStream();

            var StorageAccount = CloudStorageAccount.Parse(strCloudStorageAccount);

            CloudBlobClient BlobClient = StorageAccount.CreateCloudBlobClient();
            CloudBlobContainer Content = BlobClient.GetContainerReference(myBlobContainerReferenceKey);
            CloudBlockBlob blockBlob = Content.GetBlockBlobReference(string.Format("{0}{1}", id, extension));

           blockBlob.FetchAttributes();


            int bufferLength = 1 * 1024 * 1024;//1 MB chunk
            long blobRemainingLength = blockBlob.Properties.Length;
            Queue<KeyValuePair<long, long>> queues = new Queue<KeyValuePair<long, long>>();
            long offset = 0;
            while (blobRemainingLength > 0)
            {
                long chunkLength = (long)Math.Min(bufferLength, blobRemainingLength);
                queues.Enqueue(new KeyValuePair<long, long>(offset, chunkLength));
                offset += chunkLength;
                blobRemainingLength -= chunkLength;
            }
            Parallel.ForEach(queues,
                new ParallelOptions()
                {
                //Gets or sets the maximum number of concurrent tasks
                MaxDegreeOfParallelism = 10
                }, (queue) =>
                {
                    using (var ms = new MemoryStream())
                    {
                        blockBlob.DownloadRangeToStream(ms, queue.Key, queue.Value);
                        lock (outPutStream)
                        {
                            outPutStream.Position = queue.Key;
                            var bytes = ms.ToArray();
                            outPutStream.Write(bytes, 0, bytes.Length);
                        }
                    }
                });
            return outPutStream;

        }

        public void AgregarFicheroAzure(string empresa, string azureblob,string idfichero,string extension,byte[] data)
        {
            string myBlobContainerReferenceKey = azureblob + empresa;

            var StorageAccount = CloudStorageAccount.Parse(strCloudStorageAccount);

            CloudBlobClient BlobClient = StorageAccount.CreateCloudBlobClient();
            CloudBlobContainer Content = BlobClient.GetContainerReference(myBlobContainerReferenceKey);
            CloudBlockBlob blockBlob = Content.GetBlockBlobReference(string.Format("{0}{1}", idfichero, extension));

            using (var stream = new MemoryStream(data, writable: false))
            {
                blockBlob.UploadFromStream(stream);
            }

        }
        public void AgregarFicherosAzure(CarpetasModel carpetaModel, List<StFicherosDocumentos> ficheros, string rootPath,string empresa, string azureblob)
        {
            string myBlobContainerReferenceKey = azureblob + empresa;
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                foreach (var item in ficheros)
                {
                    var fi = new FileInfo(Path.Combine(rootPath, item.Nombre));
                    var idfichero = Guid.NewGuid();
                    create(new FicherosGaleriaModel()
                    {
                        Id = idfichero,
                        Nombre = item.Nombre,
                        Ruta = Path.Combine(carpetaModel.Ruta, fi.Name),
                        Tipo = fi.Extension,
                        Fkcarpetas = carpetaModel.Id,
                        Empresa = Empresa
                    });
                    fi.Directory.Create();

                    var StorageAccount = CloudStorageAccount.Parse(strCloudStorageAccount);

                    CloudBlobClient BlobClient = StorageAccount.CreateCloudBlobClient();
                    CloudBlobContainer Content = BlobClient.GetContainerReference(myBlobContainerReferenceKey);
                    CloudBlockBlob blockBlob = Content.GetBlockBlobReference(string.Format("{0}{1}", idfichero, fi.Extension));

                    item.Datos.Position = 0;
                    blockBlob.UploadFromStream(item.Datos);

                }

                tran.Complete();
            }
        }


        #endregion Azure


        public void AgregarFicheros(CarpetasModel carpetaModel, List<StFicherosDocumentos> ficheros, string rootPath)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                foreach (var item in ficheros)
                {
                    var fi = new FileInfo(Path.Combine(rootPath, item.Nombre));
                    var idfichero = Guid.NewGuid();
                    create(new FicherosGaleriaModel()
                    {
                        Id = idfichero,
                        Nombre = item.Nombre,
                        Ruta = Path.Combine(carpetaModel.Ruta, fi.Name),
                        Tipo = fi.Extension,
                        Fkcarpetas = carpetaModel.Id,
                        Empresa = Empresa
                    });
                    fi.Directory.Create();
                    File.WriteAllBytes(Path.Combine(rootPath, string.Format("{0}{1}", idfichero, fi.Extension)), Funciones.ReadAllBytes(item.Datos));
                }

                tran.Complete();
            }
        }
    }
}
