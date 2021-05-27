using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Iva;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class RegimenIvaConverterService : BaseConverterModel<RegimenIvaModel, RegimenIva>
    {
        public RegimenIvaConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
            
        }

        public override IEnumerable<IModelView> GetAll()
        {
            var list = _db.Set<RegimenIva>().Where(f => f.empresa == Empresa).ToList();

            var result = new List<RegimenIvaModel>();
            foreach (var item in list)
            {
                result.Add(GetModelView(item) as RegimenIvaModel);
            }

            return result;
        }

        public override bool Exists(string id)
        {
            return _db.Set<RegimenIva>().Any(f => f.id == id && f.empresa == Empresa);
        }

        public override IModelView CreateView(string id)
        {
            var obj = _db.Set<RegimenIva>().Single(f => f.id == id && f.empresa == Empresa);
            return GetModelView(obj);
        }

        public override RegimenIva CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as RegimenIvaModel;
            var result = _db.Set<RegimenIva>().Create();
            result.empresa = Empresa;
            result.id = viewmodel.Id;
            result.descripcion = viewmodel.Descripcion;
            result.normal = viewmodel.Normal;
            result.recargo = viewmodel.Recargo;
            result.exportacion = viewmodel.Exportacion;
            result.exentotasa = viewmodel.ExentoTasa;
            result.operacionue = viewmodel.OperacionUe;
            result.inversionsujetopasivo = viewmodel.InversionSujetoPasivo;
            result.operacionesnosujetas = viewmodel.OperacionesNoSujetas;
            result.zonaespecial = viewmodel.ZonasEspeciales;
            result.canariasigic = viewmodel.CanariasIgic;
            result.extranjero = viewmodel.Extranjero;
            result.ivadiferido = viewmodel.IvaDiferido;
            result.ivaimportacion = viewmodel.IvaImportacion;
            result.incompatiblecriteriocaja = (int?)viewmodel.IncompatibleCriterioCaja;
            result.soportadorepercutidoambos = (int)viewmodel.SoportableRepercutidoAmbos;
            result.bieninversion = viewmodel.BienInversion;
            result.exentosventas = viewmodel.ExentoVentas;
            result.claveoperacion340 = viewmodel.ClaveOperacion340;
            result.incluirmodelo347 = viewmodel.Incluirmodelo347;
            result.tipofacturaemitida = viewmodel.TipoFacturaEmitida;
            result.tipofacturarecibida = viewmodel.TipoFacturaRecibida;
            result.regimenespecialemitida = viewmodel.RegimenEspecialEmitida;
            result.regimenespecialrecibida = viewmodel.RegimenEspecialRecibida;
            return result;
        }

        public override RegimenIva EditPersitance(IModelView obj)
        {
            var viewmodel = obj as RegimenIvaModel;
            var result = _db.Set<RegimenIva>().Single(f => f.id == viewmodel.Id && f.empresa == viewmodel.Empresa);
            result.empresa = Empresa;
            result.id = viewmodel.Id;
            result.descripcion = viewmodel.Descripcion;
            result.normal = viewmodel.Normal;
            result.recargo = viewmodel.Recargo;
            result.exportacion = viewmodel.Exportacion;
            result.exentotasa = viewmodel.ExentoTasa;
            result.operacionue = viewmodel.OperacionUe;
            result.inversionsujetopasivo = viewmodel.InversionSujetoPasivo;
            result.operacionesnosujetas = viewmodel.OperacionesNoSujetas;
            result.zonaespecial = viewmodel.ZonasEspeciales;
            result.canariasigic = viewmodel.CanariasIgic;
            result.extranjero = viewmodel.Extranjero;
            result.ivadiferido = viewmodel.IvaDiferido;
            result.ivaimportacion = viewmodel.IvaImportacion;
            result.incompatiblecriteriocaja = (int?)viewmodel.IncompatibleCriterioCaja;
            result.soportadorepercutidoambos = (int)viewmodel.SoportableRepercutidoAmbos;
            result.bieninversion = viewmodel.BienInversion;
            result.exentosventas = viewmodel.ExentoVentas;
            result.claveoperacion340 = viewmodel.ClaveOperacion340;
            result.incluirmodelo347 = viewmodel.Incluirmodelo347;
            result.tipofacturaemitida = viewmodel.TipoFacturaEmitida;
            result.tipofacturarecibida = viewmodel.TipoFacturaRecibida;
            result.regimenespecialemitida = viewmodel.RegimenEspecialEmitida;
            result.regimenespecialrecibida = viewmodel.RegimenEspecialRecibida;
            return result;
        }

        public override IModelView GetModelView(RegimenIva obj)
        {
            var result = new RegimenIvaModel
            {
                Empresa = Empresa,
                Id = obj.id,
                Descripcion = obj.descripcion,
                Normal = obj.normal ?? false,
                Recargo = obj.recargo ?? false,
                Exportacion = obj.exportacion ?? false,
                ExentoTasa = obj.exentotasa ?? false,
                OperacionUe = obj.operacionue ?? false,
                InversionSujetoPasivo = obj.inversionsujetopasivo ?? false,
                OperacionesNoSujetas = obj.operacionesnosujetas ?? false,
                ZonasEspeciales = obj.zonaespecial ?? false,
                CanariasIgic = obj.canariasigic ?? false,
                Extranjero = obj.extranjero ?? false,
                IvaDiferido = obj.ivadiferido ?? false,
                IvaImportacion = obj.ivaimportacion ?? false,
                IncompatibleCriterioCaja =(Sra?)obj.incompatiblecriteriocaja,
                SoportableRepercutidoAmbos = (Sra)(obj.soportadorepercutidoambos ?? (int)Sra.Ambos),
                BienInversion = obj.bieninversion ?? false,
                ExentoVentas = obj.exentosventas ?? false,
                ClaveOperacion340 = obj.claveoperacion340,
                Incluirmodelo347 = obj.incluirmodelo347 ?? false,
                TipoFacturaEmitida = obj.tipofacturaemitida,
                TipoFacturaRecibida=obj.tipofacturarecibida,
                RegimenEspecialEmitida=obj.regimenespecialemitida,
                RegimenEspecialRecibida=obj.regimenespecialrecibida
        };

            return result;
        }
    }
}

