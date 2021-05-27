using System;
using System.Data.Entity.Migrations;
using System.Linq;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Preferencias
{
    public class PreferenciasUsuarioService : IDisposable
    {
        #region members

        private readonly MarfilEntities _db;

        #endregion

        #region CTR

        public PreferenciasUsuarioService(MarfilEntities db)
        {
            _db = db;
        }

        #endregion

        public object GePreferencia(TiposPreferencias tipopreferencia, Guid usuario,string id, string name)
        {
            var preferencia = _db.PreferenciasUsuario.SingleOrDefault(f => f.fkUsuario == usuario && f.tipo == (int)tipopreferencia && f.id==id && f.nombre==name);
            if (preferencia != null)
            {
                return FPreferenciasUsuario.GetPreferencia(tipopreferencia, preferencia.xml);

            }


            return null;
        }

        public void SetPreferencia(TiposPreferencias tipopreferencia, Guid usuario,string id, string name, object preferencia)
        {
            var item = _db.PreferenciasUsuario.SingleOrDefault(f => f.fkUsuario == usuario && f.tipo == (int)tipopreferencia && f.id==id && f.nombre==name) ??
                       _db.PreferenciasUsuario.Create();

            item.fkUsuario = usuario;
            item.tipo = (int)tipopreferencia;
            item.id = id;
            item.nombre = name;

            item.xml = FPreferenciasUsuario.GetXmlPreferencia(tipopreferencia, preferencia);

            _db.PreferenciasUsuario.AddOrUpdate(item);
            _db.SaveChanges();
        }

        #region Documentos Impresion
        public object GetDocumentosImpresionMantenimiento(Guid usuario, string id, string name)
        {
            //usuario
            var preferencia = _db.PreferenciasUsuario.SingleOrDefault(f =>  f.fkUsuario == usuario && f.tipo == (int)TiposPreferencias.DocumentoImpresionDefecto && f.id == id && f.nombre == name);
            if (preferencia != null)
            {
                return FPreferenciasUsuario.GetPreferencia(TiposPreferencias.DocumentoImpresionDefecto, preferencia.xml);
            }

            //admin
            preferencia = _db.PreferenciasUsuario.SingleOrDefault(f =>  f.fkUsuario == Guid.Empty && f.tipo == (int)TiposPreferencias.DocumentoImpresionDefecto && f.id == id && f.nombre == name);
            if (preferencia != null)
            {
                return FPreferenciasUsuario.GetPreferencia(TiposPreferencias.DocumentoImpresionDefecto, preferencia.xml);
            }

            return null;
        }
        #endregion


        #region Documentos Contables Impresion
        public object GetDocumentosContablesImpresionMantenimiento(Guid usuario, string id, string name)
        {
            //usuario
            var preferencia = _db.PreferenciasUsuario.SingleOrDefault
                            (f => f.fkUsuario == usuario 
                                && f.tipo == (int)TiposPreferencias.DiarioContableImpresionDefecto 
                                && f.id == id && f.nombre == name);

            if (preferencia != null)
            {
                return FPreferenciasUsuario.GetPreferencia(TiposPreferencias.DiarioContableImpresionDefecto, preferencia.xml);
            }

            //admin
            preferencia = _db.PreferenciasUsuario.SingleOrDefault
                        (f => f.fkUsuario == Guid.Empty 
                            && f.tipo == (int)TiposPreferencias.DiarioContableImpresionDefecto 
                            && f.id == id && f.nombre == name);

            if (preferencia != null)
            {
                return FPreferenciasUsuario.GetPreferencia(TiposPreferencias.DocumentoImpresionDefecto, preferencia.xml);
            }

            return null;
        }
        #endregion Documentos Contables Impresion

        public void Dispose()
        {

        }
    }
}
