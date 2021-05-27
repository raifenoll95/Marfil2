using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Converter;
using Marfil.Inf.Genericos.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class ComercialesConverterService : BaseConverterModel<ComercialesModel, Comerciales>
    {
        

        #region CTR

        public ComercialesConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
            
        }

        #endregion

        #region Api

        public override IEnumerable<IModelView> GetAll()
        {
            var fservice = FService.Instance;
            var cuentasService = fservice.GetService(typeof(CuentasModel), Context, _db);
            var list = _db.Comerciales.Where(f => f.empresa == Empresa).ToList();


            var result = new List<ComercialesModel>();
            foreach (var item in list)
            {
                var newitem = GetModelView(item) as ComercialesModel;
                newitem.Cuentas = cuentasService.get(item.fkcuentas) as CuentasModel;
                result.Add(newitem);
            }

            return result;
        }

        public override bool Exists(string id)
        {
            return _db.Set<Comerciales>().Any(f => f.fkcuentas == id && f.empresa == Empresa);
        }

        public override IModelView CreateView(string id)
        {
            var obj = _db.Set<Comerciales>().Single(f => f.fkcuentas == id && f.empresa == Empresa);
            var result = GetModelView(obj) as ComercialesModel;
            var fService = FService.Instance;
            var fModel = new FModel();
            var cuentasService = fService.GetService(typeof(CuentasModel), Context, _db);
            var direccionesService = fService.GetService(typeof(DireccionesLinModel), Context, _db) as DireccionesService;
            var bancosmandatosService = fService.GetService(typeof(BancosMandatosLinModel), Context, _db) as BancosMandatosService;

            result.Cuentas = cuentasService.get(result.Fkcuentas) as CuentasModel;
            result.Direcciones = fModel.GetModel<DireccionesModel>(Context);
            result.Direcciones.Direcciones = direccionesService.GetDirecciones(Empresa, TiposCuentas.Comerciales, obj.fkcuentas);
            result.Direcciones.Id = Guid.NewGuid();
            result.Direcciones.Tipotercero = (int)TiposCuentas.Comerciales;

            result.BancosMandatos = fModel.GetModel<BancosMandatosModel>(Context);
            result.BancosMandatos.BancosMandatos = bancosmandatosService.GetBancosMandatos(obj.fkcuentas);


            return result;
        }

        public override Comerciales CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as ComercialesModel;
            var result = _db.Comerciales.Create();
            result.empresa = viewmodel.Empresa;
            result.fkcuentas = viewmodel.Fkcuentas;
            result.fkformapago = Funciones.Qint(viewmodel.Fkformapago);
            result.comisionporm2 = viewmodel.Comisionporm2;
            result.comisionporm3 = viewmodel.Comisionporm3;
            result.fkregimeniva = viewmodel.Fkregimeniva;
            result.fktipoirpf = viewmodel.Fktipoirpf;
            result.porcentajecomision = viewmodel.Porcentajecomision;
            result.porcentajedecrementosobreptb = viewmodel.Porcentajedecrementosobreptb;
            result.porcentajeincrementosobreptb = viewmodel.Porcentajeincrementosobreptb;
            result.primadecrementosobreptb = viewmodel.Primadecrementosobreptb;
            result.primaincrementosobreptb = viewmodel.Primaincrementosobreptb;
            
            return result;
        }

        public override Comerciales EditPersitance(IModelView obj)
        {
            var viewmodel = obj as ComercialesModel;
            var result = _db.Comerciales.Single(f => f.fkcuentas == viewmodel.Fkcuentas && f.empresa == viewmodel.Empresa);

            result.empresa = viewmodel.Empresa;
            result.fkcuentas = viewmodel.Fkcuentas;
            result.fkformapago = Funciones.Qint(viewmodel.Fkformapago);
            result.comisionporm2 = viewmodel.Comisionporm2;
            result.comisionporm3 = viewmodel.Comisionporm3;
            result.fkregimeniva = viewmodel.Fkregimeniva;
            result.fktipoirpf = viewmodel.Fktipoirpf;
            result.porcentajecomision = viewmodel.Porcentajecomision;
            result.porcentajedecrementosobreptb = viewmodel.Porcentajedecrementosobreptb;
            result.porcentajeincrementosobreptb = viewmodel.Porcentajeincrementosobreptb;
            result.primadecrementosobreptb = viewmodel.Primadecrementosobreptb;
            result.primaincrementosobreptb = viewmodel.Primaincrementosobreptb;

            return result;
        }

        public override IModelView GetModelView(Comerciales obj)
        {
            return new ComercialesModel()
            {
                Empresa= obj.empresa,
                Fkcuentas = obj.fkcuentas,
                Fkformapago = obj.fkformapago.HasValue? obj.fkformapago.Value.ToString() : string.Empty,
                Fktipoirpf = obj.fktipoirpf,
                Fkregimeniva = obj.fkregimeniva,
                Porcentajecomision = obj.porcentajecomision,
                Comisionporm2 = obj.comisionporm2,
                Comisionporm3 = obj.comisionporm3,
                Porcentajedecrementosobreptb= obj.porcentajedecrementosobreptb,
                Porcentajeincrementosobreptb = obj.porcentajeincrementosobreptb,
                Primadecrementosobreptb = obj.primadecrementosobreptb,
                Primaincrementosobreptb = obj.primaincrementosobreptb
            };
        }

        #endregion
    }
}

