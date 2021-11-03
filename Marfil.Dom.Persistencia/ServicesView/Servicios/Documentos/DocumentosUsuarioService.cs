using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using DevExpress.Utils.Text;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Diseñador;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Preferencias;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos
{
    public class DocumentosUsuarioService:IDisposable
    {
        #region members
       
        private const char Separator = ';';
        private readonly MarfilEntities _db;
        public IContextService _context;
        private string _empresa;
        public string Empresa
        {
            get { return _empresa; }
        }

        #endregion

        #region CTR

        public DocumentosUsuarioService(IContextService context, MarfilEntities db)
        {
            _db = db;
            _context = context;
            _empresa = _context.Empresa;
        }
        public DocumentosUsuarioService(string empresa, MarfilEntities db)
        {
            _db = db;
            _empresa = empresa;
        }

        #endregion

        public IEnumerable<DocumentosModel> GetDocumentosParaImprimir(TipoDocumentoImpresion tipoDocumento, Guid usuario)
        {
            var servicePreferencias = new PreferenciasUsuarioService(_db);
            var doc = servicePreferencias.GePreferencia(TiposPreferencias.DocumentoImpresionDefecto, usuario, tipoDocumento.ToString(), "Defecto") as PreferenciaDocumentoImpresionDefecto;
            var defectoName = doc != null ? doc.Name : string.Empty;
            return _db.DocumentosUsuario.Where(f => f.empresa == Empresa && (f.fkusuario == usuario || f.fkusuario==Guid.Empty) && f.tipo == (int)tipoDocumento).ToList().Select(f => new DocumentosModel() { Defecto = defectoName == f.nombre, Tipo = tipoDocumento, CustomId = CreateCustomId(tipoDocumento, f.fkusuario, f.nombre), Nombre = f.nombre, Usuario = f.fkusuario == Guid.Empty ? "Admin" : _db.Usuarios.Single(j => j.id == f.fkusuario).usuario, Tipoprivacidad = (TipoPrivacidadDocumento)f.tipoprivacidad, Tiporeport = (TipoReport)f.tiporeport }).ToList().OrderByDescending(f => f.Defecto);
        }

        public IEnumerable<DocumentosModel> GetDocumentos(TipoDocumentoImpresion tipoDocumento, Guid usuario)
        {
            var servicePreferencias = new PreferenciasUsuarioService(_db);
            var doc = servicePreferencias.GePreferencia(TiposPreferencias.DocumentoImpresionDefecto, usuario, tipoDocumento.ToString(), "Defecto") as PreferenciaDocumentoImpresionDefecto;
            var defectoName = doc != null ? doc.Name : string.Empty;
            return _db.DocumentosUsuario.Where
                (f => f.empresa == Empresa && ((f.fkusuario == usuario && f.tipo == (int)tipoDocumento) || (f.tipoprivacidad == 0 && f.tipo == (int)tipoDocumento)))
                .ToList().Select(f=>new DocumentosModel() { Defecto= defectoName == f.nombre,  Tipo = tipoDocumento,CustomId = CreateCustomId(tipoDocumento,f.fkusuario,f.nombre),Nombre=f.nombre,Usuario=f.fkusuario == Guid.Empty ? "Admin": _db.Usuarios.Single(j=>j.id==f.fkusuario).usuario,Tipoprivacidad = (TipoPrivacidadDocumento)f.tipoprivacidad,Tiporeport = (TipoReport)f.tiporeport}).ToList().OrderByDescending(f=>f.Defecto);
        }

        public bool ExisteDocumento(TipoDocumentoImpresion tipoDocumento, Guid usuario, string name, TipoPrivacidadDocumento privacidad = TipoPrivacidadDocumento.Publico)
        {
            return                 _db.DocumentosUsuario.Any(f => f.empresa == Empresa && f.fkusuario == usuario && f.tipo == (int) tipoDocumento && f.nombre == name && f.tipoprivacidad == (int)privacidad);
        }

        public DocumentosModel GetDocumento(TipoDocumentoImpresion tipoDocumento, Guid usuario, string name)
        {
            var servicePreferencias = new PreferenciasUsuarioService(_db);
            var doc = servicePreferencias.GePreferencia(TiposPreferencias.DocumentoImpresionDefecto, usuario, tipoDocumento.ToString(), "Defecto") as PreferenciaDocumentoImpresionDefecto;
            var defectoName = doc != null ? doc.Name : string.Empty;
            var documento = _db.DocumentosUsuario.SingleOrDefault(f => f.empresa == Empresa && (f.fkusuario == usuario || f.fkusuario == Guid.Empty) && f.tipo == (int)tipoDocumento && f.nombre == name);
            if (documento != null)
            {
                return new DocumentosModel()
                {
                    Empresa = Empresa,
                    Defecto = defectoName == documento.nombre,
                    Tipo = tipoDocumento,
                    CustomId = CreateCustomId(tipoDocumento, documento.fkusuario, documento.nombre),
                    Nombre = documento.nombre,
                    Usuario = documento.fkusuario == Guid.Empty ? "Admin" : _db.Usuarios.Single(j => j.id == documento.fkusuario).usuario,
                    Datos = documento.datos,
                    Tipoprivacidad = (TipoPrivacidadDocumento)documento.tipoprivacidad,
                    Tiporeport = (TipoReport)documento.tiporeport
                };
            }
            return null;
        }

        public DocumentosModel GetDocumentoParaImprimir(TipoDocumentoImpresion tipoDocumento, Guid usuario, string name)
        {
            var documento = _db.DocumentosUsuario.SingleOrDefault(f => f.empresa == Empresa && ( f.fkusuario == usuario ||f.fkusuario==Guid.Empty )&& f.tipo == (int)tipoDocumento && f.nombre == name);
            if (documento != null)
            {
                return new DocumentosModel()
                {
                    Empresa = Empresa,
                    Tipo = tipoDocumento,
                    CustomId = CreateCustomId(tipoDocumento, documento.fkusuario, documento.nombre),
                    Nombre = documento.nombre,
                    Usuario = documento.fkusuario == Guid.Empty ? "Admin" : _db.Usuarios.Single(j => j.id == documento.fkusuario).usuario,
                    Datos = documento.datos,
                    Tipoprivacidad = (TipoPrivacidadDocumento)documento.tipoprivacidad,
                    Tiporeport = (TipoReport)documento.tiporeport
                };
            }
            return null;
        }

        public void SetPreferencia(TipoDocumentoImpresion tipoDocumento, Guid usuario,TipoPrivacidadDocumento tipoprivacidad,TipoReport tiporeport, string name, byte[] report,bool defecto=false)
        {

            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var item = _db.DocumentosUsuario.SingleOrDefault(f => f.empresa == Empresa && f.fkusuario == usuario && f.tipo == (int)tipoDocumento && f.nombre == name) ??
                           _db.DocumentosUsuario.Create();

                item.empresa = Empresa;
                item.fkusuario = usuario;
                item.tipo = (int)tipoDocumento;
                item.nombre = name;
                item.datos = report;
                item.tipoprivacidad = (int)tipoprivacidad;
                item.tiporeport = (int)tiporeport;
                

                _db.DocumentosUsuario.AddOrUpdate(item);
                _db.SaveChanges();

                if (defecto && tiporeport==TipoReport.Report)
                {
                    var service = new PreferenciasUsuarioService(_db);
                    service.SetPreferencia(TiposPreferencias.DocumentoImpresionDefecto, usuario, tipoDocumento.ToString(), "Defecto", new PreferenciaDocumentoImpresionDefecto() { Name = name, Tipodocumento = tipoDocumento, Usuario = usuario });
                }
                

                tran.Complete();
            }


        }

        public void RemovePreferencia(TipoDocumentoImpresion tipoDocumento, Guid usuario, string name)
        {
            var documento =
                _db.DocumentosUsuario.SingleOrDefault(
                    f => f.empresa == Empresa && f.fkusuario == usuario && f.tipo == (int) tipoDocumento && f.nombre == name);

            var service = new PreferenciasUsuarioService(_db);
            var doc=service.GePreferencia(TiposPreferencias.DocumentoImpresionDefecto, usuario, tipoDocumento.ToString(), "Defecto");
            if (doc != null)
            {
                if (documento != null)
                {
                    var docObj = doc as PreferenciaDocumentoImpresionDefecto;
                    if (docObj.Name == documento.nombre)
                    {
                        throw new ValidationException("No se puede eliminar el documento por defecto");
                    }

                    _db.DocumentosUsuario.Remove(documento);
                    _db.SaveChanges();
                }
            }
            
        }

        public static string CreateCustomId(TipoDocumentoImpresion tipoDocumento, Guid usuario, string name)
        {
            return string.Format("{0}{3}{1}{3}{2}", (int) tipoDocumento, usuario, name, Separator);
        }

        public static void GetFromCustomId(string customId, out TipoDocumentoImpresion tipo, out Guid usuario, out string name)
        {
            var vector = customId.Split(Separator);
            var nombre = "";

            //este truño es por si el nombre tiene guiones
            var flag = false;
            for(var i=2;i< vector.Length;i++)
            {
                if (flag)
                    nombre += Separator;

                nombre += vector[i];

                flag = true;
            }

            tipo = (TipoDocumentoImpresion)Enum.Parse(typeof (TipoDocumentoImpresion), vector[0]);
            usuario = new Guid(vector[1]);
            name = nombre;
        }

        public void Dispose()
        {

        }
    }
}
