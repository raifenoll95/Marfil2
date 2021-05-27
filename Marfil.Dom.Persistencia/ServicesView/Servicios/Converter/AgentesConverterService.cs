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
    internal class AgentesConverterService : BaseConverterModel<AgentesModel, Agentes>
    {
        

        #region CTR

        public AgentesConverterService(IContextService context, MarfilEntities db) : base(context,db)
        {
            
        }

        #endregion

        #region Api

        public override IEnumerable<IModelView> GetAll()
        {
            var fservice = FService.Instance;
            var cuentasService = fservice.GetService(typeof(CuentasModel), Context, _db);
            var list = _db.Agentes.Where(f => f.empresa == Empresa).ToList();


            var result = new List<AgentesModel>();
            foreach (var item in list)
            {
                var newitem = GetModelView(item) as AgentesModel;
                newitem.Cuentas = cuentasService.get(item.fkcuentas) as CuentasModel;
                result.Add(newitem);
            }

            return result;
        }

        public override bool Exists(string id)
        {
            return _db.Set<Agentes>().Any(f => f.fkcuentas == id && f.empresa == Empresa);
        }

        public override IModelView CreateView(string id)
        {
            var obj = _db.Set<Agentes>().Single(f => f.fkcuentas == id && f.empresa == Empresa);
            var result = GetModelView(obj) as AgentesModel;
            var fService = FService.Instance;
            var fModel = new FModel();
            var cuentasService = fService.GetService(typeof(CuentasModel), Context, _db);
            var direccionesService = fService.GetService(typeof(DireccionesLinModel), Context, _db) as DireccionesService;
            var bancosmandatosService = fService.GetService(typeof(BancosMandatosLinModel), Context, _db) as BancosMandatosService;

            result.Cuentas = cuentasService.get(result.Fkcuentas) as CuentasModel;
            result.Direcciones = fModel.GetModel<DireccionesModel>(Context);
            result.Direcciones.Direcciones = direccionesService.GetDirecciones(Empresa, TiposCuentas.Agentes, obj.fkcuentas);
            result.Direcciones.Id = Guid.NewGuid();
            result.Direcciones.Tipotercero = (int)TiposCuentas.Agentes;

            result.BancosMandatos = fModel.GetModel<BancosMandatosModel>(Context);
            result.BancosMandatos.BancosMandatos = bancosmandatosService.GetBancosMandatos(obj.fkcuentas);


            return result;
        }

        public override Agentes CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as AgentesModel;
            var result = _db.Agentes.Create();
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

        public override Agentes EditPersitance(IModelView obj)
        {
            var viewmodel = obj as AgentesModel;
            var result = _db.Agentes.Single(f => f.fkcuentas == viewmodel.Fkcuentas && f.empresa == viewmodel.Empresa);

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

        public override IModelView GetModelView(Agentes obj)
        {
            return new AgentesModel()
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

