using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Terceros;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class OperariosConverterService : BaseConverterModel<OperariosModel, Operarios>
    {
       

        #region CTR

        public OperariosConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        #endregion

        #region Api

        public override IEnumerable<IModelView> GetAll()
        {
            var fservice = FService.Instance;
            var cuentasService = fservice.GetService(typeof(CuentasModel), Context, _db);
            var list = _db.Operarios.Where(f => f.empresa == Empresa).ToList();


            var result = new List<OperariosModel>();
            var fModel = new FModel();
            var fService = FService.Instance;
            var direccionesService = fService.GetService(typeof(DireccionesLinModel), Context, _db) as DireccionesService;
            foreach (var item in list)
            {
                var newitem = GetModelView(item) as OperariosModel;

                newitem.Cuentas = cuentasService.get(item.fkcuentas) as CuentasModel;
                newitem.Direcciones = fModel.GetModel<DireccionesModel>(Context);
                newitem.Direcciones.Direcciones = direccionesService.GetDirecciones(Empresa, TiposCuentas.Operarios, newitem.Fkcuentas);
                newitem.Direcciones.Id = Guid.NewGuid();
                newitem.Direcciones.Tipotercero = (int)TiposCuentas.Operarios;
                result.Add(newitem);
            }

            return result;
        }

        public override bool Exists(string id)
        {
            return _db.Set<Operarios>().Any(f => f.fkcuentas == id && f.empresa == Empresa);
        }

        public override IModelView CreateView(string id)
        {
            var obj = _db.Set<Operarios>().Single(f => f.fkcuentas == id && f.empresa == Empresa);
            var result = GetModelView(obj) as OperariosModel;
            var fService = FService.Instance;
            var fModel = new FModel();
            var cuentasService = fService.GetService(typeof(CuentasModel), Context, _db);
            var direccionesService = fService.GetService(typeof(DireccionesLinModel), Context, _db) as DireccionesService;
            
            var bancosmandatosService = fService.GetService(typeof(BancosMandatosLinModel), Context, _db) as BancosMandatosService;

            result.Cuentas = cuentasService.get(result.Fkcuentas) as CuentasModel;
            result.Direcciones = fModel.GetModel<DireccionesModel>(Context);
            result.Direcciones.Direcciones = direccionesService.GetDirecciones(Empresa, TiposCuentas.Operarios, obj.fkcuentas);
            result.Direcciones.Id = Guid.NewGuid();
            result.Direcciones.Tipotercero = (int)TiposCuentas.Operarios;

            result.BancosMandatos = fModel.GetModel<BancosMandatosModel>(Context);
            result.BancosMandatos.BancosMandatos = bancosmandatosService.GetBancosMandatos(obj.fkcuentas);


            return result;
        }

        public override Operarios CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as OperariosModel;
            var result = _db.Operarios.Create();

            foreach (var item in result.GetType().GetProperties())
            {
                if (typeof(OperariosModel).GetProperties().Any(f => f.Name.ToLower() == item.Name.ToLower()))
                    item.SetValue(result, viewmodel.get(item.Name));
            }

            return result;
        }

        public override Operarios EditPersitance(IModelView obj)
        {
            var viewmodel = obj as OperariosModel;
            var result = _db.Operarios.Single(f => f.fkcuentas == viewmodel.Fkcuentas && f.empresa == viewmodel.Empresa);

            foreach (var item in result.GetType().GetProperties())
            {
                if (typeof(OperariosModel).GetProperties().Any(f => f.Name.ToLower() == item.Name.ToLower()))
                    item.SetValue(result, viewmodel.get(item.Name));
            }

            return result;
        }

        #endregion
    }
}
