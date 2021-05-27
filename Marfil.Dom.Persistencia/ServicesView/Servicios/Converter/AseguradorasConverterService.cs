using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Terceros;


namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class AseguradorasConverterService : BaseConverterModel<AseguradorasModel, Aseguradoras>
    {
        

        #region CTR

        public AseguradorasConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
            
        }

        #endregion

        #region Api

        public override IEnumerable<IModelView> GetAll()
        {
            var fservice=FService.Instance;
            var cuentasService = fservice.GetService(typeof (CuentasModel), Context, _db);
            var list = _db.Aseguradoras.Where(f => f.empresa == Empresa).ToList();
           
                    
            var result = new List<AseguradorasModel>();
            foreach (var item in list)
            {
                var newitem = GetModelView(item) as AseguradorasModel;
                newitem.Cuentas = cuentasService.get(item.fkcuentas) as CuentasModel;
                result.Add(newitem);
            }

            return result;
        }

        public override bool Exists(string id)
        {
            return _db.Set<Aseguradoras>().Any(f => f.fkcuentas == id && f.empresa == Empresa);
        }

        public override IModelView CreateView(string id)
        {
            var obj = _db.Set<Aseguradoras>().Single(f => f.fkcuentas == id && f.empresa == Empresa);
            var result=  GetModelView(obj) as AseguradorasModel;
            var fService=FService.Instance;
            var fModel =new FModel();
            var cuentasService = fService.GetService(typeof (CuentasModel), Context, _db);
            var direccionesService = fService.GetService(typeof (DireccionesLinModel), Context, _db) as DireccionesService;
            var contactosService = fService.GetService(typeof(ContactosLinModel), Context, _db) as ContactosService;
            var bancosmandatosService = fService.GetService(typeof(BancosMandatosLinModel), Context, _db) as BancosMandatosService;

            result.Cuentas = cuentasService.get(result.Fkcuentas) as CuentasModel;
            result.Direcciones=fModel.GetModel<DireccionesModel>(Context);
            result.Direcciones.Direcciones = direccionesService.GetDirecciones(Empresa, TiposCuentas.Aseguradoras, obj.fkcuentas);
            result.Direcciones.Id = Guid.NewGuid();
            result.Direcciones.Tipotercero = (int)TiposCuentas.Aseguradoras;

            result.Contactos = fModel.GetModel<ContactosModel>(Context);
            result.Contactos.Contactos = contactosService.GetContactos(TiposCuentas.Aseguradoras, obj.fkcuentas);
            result.Contactos.Id = Guid.NewGuid();
            result.Contactos.Tipotercero = (int)TiposCuentas.Aseguradoras;
            result.Contactos.Direcciones = result.Direcciones;

            result.BancosMandatos = fModel.GetModel<BancosMandatosModel>(Context);
            result.BancosMandatos.BancosMandatos = bancosmandatosService.GetBancosMandatos(obj.fkcuentas);
            

            return result;
        }

        public override Aseguradoras EditPersitance(IModelView obj)
        {
            var viewmodel = obj as AseguradorasModel;
            var result = _db.Aseguradoras.Single(f => f.fkcuentas == viewmodel.Fkcuentas && f.empresa == viewmodel.Empresa);

            foreach (var item in result.GetType().GetProperties())
            {
                item.SetValue(result, viewmodel.get(item.Name));
            }

            return result;
        }

        /*public override IModelView GetModelView(Aseguradoras obj)
        {
            var result = new AseguradorasModel();
            foreach (var item in result.GetType().GetProperties())
            {
                item.SetValue(result, viewmodel.get(item.Name));
            }
            return result;
        }*/

        #endregion
    }
}
