using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Iva;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class TiposIvaConverterService : BaseConverterModel<TiposIvaModel, TiposIva>
    {
        

        public TiposIvaConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
           
        }

        public override IEnumerable<IModelView> GetAll()
        {
            var list = _db.Set<TiposIva>().Where(f => f.empresa == Empresa).ToList();

            var result = new List<TiposIvaModel>();
            foreach (var item in list)
            {
                result.Add(GetModelView(item) as TiposIvaModel);
            }

            return result;
        }

        public override bool Exists(string id)
        {
            return _db.Set<TiposIva>().Any(f => f.id == id && f.empresa == Empresa);
        }

        public override IModelView CreateView(string id)
        {
            var obj = _db.Set<TiposIva>().Single(f => f.id == id && f.empresa == Empresa);
            return GetModelView(obj);
        }

        public override TiposIva CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as TiposIvaModel;
            var result = _db.Set<TiposIva>().Create();
            result.empresa = Empresa;
            result.id = viewmodel.Id;
            result.nombre = viewmodel.Nombre;
            result.porcentajeiva = viewmodel.PorcentajeIva;
            result.porcentajerecargoequivalente = viewmodel.PorcentajeRecargoEquivalencia;
            result.fkcuentasivasoportado = viewmodel.CtaIvaSoportado;
            result.fkcuentasivarepercutido = viewmodel.CtaIvaRepercutido;
            result.fkcuentasivanodeducible = viewmodel.CtaIvaNoDeducible;
            result.fkcuentasrecargoequivalenciarepercutido = viewmodel.CtaRecargoEquivalenciaRepercutido;
            result.fkcuentasivasoportadocriteriocaja = viewmodel.CtaIvaSoportadoCriterioCaja;
            result.fkcuentasivarepercutidocriteriocaja = viewmodel.CtaIvaRepercutidoCriterioCaja;
            result.fkcuentasrecargoequivalenciarepercutidocriteriocaja = viewmodel.CtaRecargoEquivalenciaRepercutidoCriterioCaja;
            result.ivadeducible = viewmodel.IvaDeducible;
            result.ivasoportado = viewmodel.IvaSoportado;
            result.porcentajededucible = viewmodel.PorcentajeDeducible;
            return result;
        }

        public override TiposIva EditPersitance(IModelView obj)
        {
            var viewmodel = obj as TiposIvaModel;
            var result = _db.Set<TiposIva>().Single(f => f.id == viewmodel.Id && f.empresa == viewmodel.Empresa);
            result.empresa = Empresa;
            result.id = viewmodel.Id;
            result.nombre = viewmodel.Nombre;
            result.porcentajeiva = viewmodel.PorcentajeIva;
            result.porcentajerecargoequivalente = viewmodel.PorcentajeRecargoEquivalencia;
            result.fkcuentasivasoportado = viewmodel.CtaIvaSoportado;
            result.fkcuentasivarepercutido = viewmodel.CtaIvaRepercutido;
            result.fkcuentasivanodeducible = viewmodel.CtaIvaNoDeducible;
            result.fkcuentasrecargoequivalenciarepercutido = viewmodel.CtaRecargoEquivalenciaRepercutido;
            result.fkcuentasivasoportadocriteriocaja = viewmodel.CtaIvaSoportadoCriterioCaja;
            result.fkcuentasivarepercutidocriteriocaja = viewmodel.CtaIvaRepercutidoCriterioCaja;
            result.fkcuentasrecargoequivalenciarepercutidocriteriocaja = viewmodel.CtaRecargoEquivalenciaRepercutidoCriterioCaja;
            result.ivadeducible = viewmodel.IvaDeducible;
            result.ivasoportado = viewmodel.IvaSoportado;
            result.porcentajededucible = viewmodel.PorcentajeDeducible;
            return result;
        }

        public override IModelView GetModelView(TiposIva obj)
        {
            var result = new TiposIvaModel
            {
                Empresa = Empresa,
                Id = obj.id,
                Nombre = obj.nombre,
                PorcentajeIva = obj.porcentajeiva ?? 0,
                PorcentajeRecargoEquivalencia = obj.porcentajerecargoequivalente ?? 0,
                CtaIvaSoportado = obj.fkcuentasivasoportado,
                CtaIvaRepercutido = obj.fkcuentasivarepercutido,
                CtaIvaNoDeducible = obj.fkcuentasivanodeducible,
                CtaRecargoEquivalenciaRepercutido = obj.fkcuentasrecargoequivalenciarepercutido,
                CtaIvaSoportadoCriterioCaja = obj.fkcuentasivasoportadocriteriocaja,
                CtaIvaRepercutidoCriterioCaja = obj.fkcuentasivarepercutidocriteriocaja,
                CtaRecargoEquivalenciaRepercutidoCriterioCaja = obj.fkcuentasrecargoequivalenciarepercutidocriteriocaja,
                IvaDeducible = obj.ivadeducible ?? false,
                IvaSoportado = obj.ivasoportado ?? false,
                PorcentajeDeducible = obj.porcentajededucible ?? 0
            };

            return result;
        }
    }
}
