using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Ficheros;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface ICarpetasService
    {

    }

    public class CarpetasService : GestionService<CarpetasModel, Carpetas>, ICarpetasService
    {
        #region CTR

        public CarpetasService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        #region Api

        public IEnumerable<CarpetasModel> GetSubcarpetasDeCarpetaId(string fullname)
        {

            var carpetaid = _db.Carpetas.Single(f => f.ruta == fullname && f.empresa == Empresa);
            return _db.Carpetas.Where(f => f.fkcarpetas == carpetaid.id && f.empresa == Empresa).ToList().Select(f=>_converterModel.GetModelView(f) as CarpetasModel);
        }

        public bool ExisteCarpeta(string fullname)
        {
            return _db.Carpetas.Any(f => f.ruta == fullname && f.empresa == Empresa);
        }

        public CarpetasModel GenerarCarpetaAsociada(string tipo, string ejercicio, string referencia)
        {
            var carpetasService = FService.Instance.GetService(typeof(CarpetasModel), _context, _db) as CarpetasService;

            var rutaPadre = Path.Combine(ConfigurationManager.AppSettings["FileManagerNodoRaiz"],
                tipo);

            var rutaEjercicio = Path.Combine(ConfigurationManager.AppSettings["FileManagerNodoRaiz"],
                tipo, ejercicio);

            var guidRoot = Guid.NewGuid();
            if (!carpetasService.ExisteCarpeta(rutaPadre))
            {
                carpetasService.create(new CarpetasModel()
                {
                    Empresa = Empresa,
                    Nombre = tipo,
                    Id = guidRoot,
                    Ruta = rutaPadre,
                    Fkcarpetas = Guid.Empty

                });
            }
            else
                guidRoot = carpetasService.GetCarpeta(rutaPadre).Id;

            var guidEjercicio = Guid.NewGuid();
            if (!carpetasService.ExisteCarpeta(rutaEjercicio))
            {
                carpetasService.create(new CarpetasModel()
                {
                    Empresa = Empresa,
                    Nombre = ejercicio,
                    Id = guidEjercicio,
                    Ruta = rutaEjercicio,
                    Fkcarpetas = guidRoot

                });
            }
            else
                guidEjercicio = carpetasService.GetCarpeta(rutaEjercicio).Id;

            var nuevaCarpeta = new CarpetasModel()
            {
                Empresa = Empresa,
                Nombre = referencia,
                Id = Guid.NewGuid(),
                Ruta = carpetasService.GenerateRutaCarpeta(tipo, ejercicio, referencia),
                Fkcarpetas = guidEjercicio

            };
            carpetasService.create(nuevaCarpeta);

            return nuevaCarpeta;
        }

        public CarpetasModel GenerarCarpetaAsociadaDocumento(TipoDocumentos tipo,string ejercicio,string referencia)
        {

            return GenerarCarpetaAsociada(Funciones.GetEnumByStringValueAttribute(tipo),
                ejercicio,
                referencia);
        }

        #endregion

        public string GenerateRutaCarpeta(string tipo, string ejercicio, string referencia)
        {
            return Path.Combine(ConfigurationManager.AppSettings["FileManagerNodoRaiz"], tipo, ejercicio, referencia);
        }

        public string GenerateRutaCarpeta(TipoDocumentos tipo, string ejercicio, string referencia)
        {
            return GenerateRutaCarpeta(Funciones.GetEnumByStringValueAttribute(tipo), ejercicio, referencia);
        }

        public CarpetasModel GetCarpeta(string fullname)
        {
            return
                _converterModel.GetModelView(_db.Carpetas.Single(f => f.ruta == fullname && f.empresa == Empresa)) as
                    CarpetasModel;

        }
    }
}
