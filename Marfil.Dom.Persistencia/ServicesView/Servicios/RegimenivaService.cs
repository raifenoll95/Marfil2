using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Iva;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IRegimenivaService
    {

    }

    public class RegimenivaService : GestionService<RegimenIvaModel, RegimenIva>, IRegimenivaService
    {


        #region CTR

        public RegimenivaService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var obj = new RegimenIvaModel();
            var excluded =
                obj.getProperties()
                    .Where(f => f.property.Name != "Descripcion" && f.property.Name != "Id")
                    .Select(f => f.property.Name).ToList();
            model.ExcludedColumns = excluded;

            return model;
        }

        public bool GetOperacionUE(string empresa, string regimen)
        {
            return _db.RegimenIva.Where(f => f.empresa == empresa && f.id == regimen).FirstOrDefault().operacionue.Value;
        }

        public bool GetExportacion(string empresa, string regimen)
        {
            return _db.RegimenIva.Where(f => f.empresa == empresa && f.id == regimen).FirstOrDefault().exportacion.Value;
        }

        public bool GetBienInversion(string empresa, string tipofactura)
        {
            var tipoparse = int.Parse(tipofactura);
            return _db.TiposFacturas.Where(f => f.empresa == empresa && f.id == tipoparse).FirstOrDefault().bieninversion.Value;
        }

        public bool EsSujetaNoExenta(string empresa, string regimen)
        {
            var exento = _db.RegimenIva.Where(f => f.empresa == empresa && f.id == regimen).FirstOrDefault().exentotasa.Value;
            var nosujeto = _db.RegimenIva.Where(f => f.empresa == empresa && f.id == regimen).FirstOrDefault().operacionesnosujetas.Value;

            return !exento && !nosujeto ? true : false;
        }

        public bool EsSujetaExenta(string empresa, string regimen)
        {
            var exento = _db.RegimenIva.Where(f => f.empresa == empresa && f.id == regimen).FirstOrDefault().exentotasa.Value;
            var nosujeto = _db.RegimenIva.Where(f => f.empresa == empresa && f.id == regimen).FirstOrDefault().operacionesnosujetas.Value;

            return exento && !nosujeto ? true : false;
        }

        public bool EsNoSujeta(string empresa, string regimen)
        {
            //var exento = _db.RegimenIva.Where(f => f.empresa == empresa && f.id == regimen).FirstOrDefault().exentotasa.Value;
            var nosujeto = _db.RegimenIva.Where(f => f.empresa == empresa && f.id == regimen).FirstOrDefault().operacionesnosujetas.Value;

            return nosujeto ? true : false;
        }

        public bool EsInvSujetoPasivo(string empresa, string regimen)
        {
            var invsujetopasivo = _db.RegimenIva.Where(f => f.empresa == empresa && f.id == regimen).FirstOrDefault().inversionsujetopasivo.Value;

            return invsujetopasivo;
        }

        public string GetClaveTipoFacturaEmitida(string regimen)
        {
            var clave = _db.RegimenIva.Where(f => f.empresa == Empresa && f.id == regimen).SingleOrDefault().tipofacturaemitida;

            return string.IsNullOrEmpty(clave) ? "F1" : clave;//Por defecto F1
        }

        public string GetRegimenEspecialEmitida(string regimen)
        {
            var regimenEspecial = _db.RegimenIva.Where(f => f.empresa == Empresa && f.id == regimen).SingleOrDefault().regimenespecialemitida;

            return string.IsNullOrEmpty(regimenEspecial) ? "01" : regimenEspecial;//Por defecto 01       
        }
    }
}
