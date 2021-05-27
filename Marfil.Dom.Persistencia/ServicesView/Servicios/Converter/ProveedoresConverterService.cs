using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class ProveedoresConverterService : BaseConverterModel<ProveedoresModel, Proveedores>
    {
        

        #region CTR

        public ProveedoresConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
            
        }

        #endregion

        #region Api

        public override IEnumerable<IModelView> GetAll()
        {
            var fservice = FService.Instance;
            var cuentasService = fservice.GetService(typeof(CuentasModel), Context, _db);
            var list = _db.Proveedores.Where(f => f.empresa == Empresa).ToList();


            var result = new List<ProveedoresModel>();
            var fModel = new FModel();
            var fService = FService.Instance;
            var direccionesService = fService.GetService(typeof(DireccionesLinModel), Context, _db) as DireccionesService;
            foreach (var item in list)
            {
                var newitem = GetModelView(item) as ProveedoresModel;
                newitem.Cuentas = cuentasService.get(item.fkcuentas) as CuentasModel;
                newitem.Direcciones = fModel.GetModel<DireccionesModel>(Context);
                newitem.Direcciones.Direcciones = direccionesService.GetDirecciones(Empresa, TiposCuentas.Proveedores, newitem.Fkcuentas);
                newitem.Direcciones.Id = Guid.NewGuid();
                newitem.Direcciones.Tipotercero = (int)TiposCuentas.Proveedores;
                result.Add(newitem);
            }

            return result;
        }

        public override bool Exists(string id)
        {
            return _db.Set<Proveedores>().Any(f => f.fkcuentas == id && f.empresa == Empresa);
        }

        public override IModelView CreateView(string id)
        {
            var obj = _db.Set<Proveedores>().Single(f => f.fkcuentas == id && f.empresa == Empresa);
            var result = GetModelView(obj) as ProveedoresModel;
            var fService = FService.Instance;
            var fModel = new FModel();
            var cuentasService = fService.GetService(typeof(CuentasModel), Context, _db);
            var direccionesService = fService.GetService(typeof(DireccionesLinModel), Context, _db) as DireccionesService;
            var contactosService = fService.GetService(typeof(ContactosLinModel), Context, _db) as ContactosService;
            var bancosmandatosService = fService.GetService(typeof(BancosMandatosLinModel), Context, _db) as BancosMandatosService;

            result.Cuentas = cuentasService.get(result.Fkcuentas) as CuentasModel;
            result.Direcciones = fModel.GetModel<DireccionesModel>(Context);
            result.Direcciones.Direcciones = direccionesService.GetDirecciones(Empresa, TiposCuentas.Proveedores, obj.fkcuentas);
            result.Direcciones.Id = Guid.NewGuid();
            result.Direcciones.Tipotercero = (int)TiposCuentas.Proveedores;

            result.Contactos = fModel.GetModel<ContactosModel>(Context);
            result.Contactos.Contactos = contactosService.GetContactos(TiposCuentas.Proveedores, obj.fkcuentas);
            result.Contactos.Id = Guid.NewGuid();
            result.Contactos.Tipotercero = (int)TiposCuentas.Proveedores;
            result.Contactos.Direcciones = result.Direcciones;

            result.BancosMandatos = fModel.GetModel<BancosMandatosModel>(Context);
            result.BancosMandatos.BancosMandatos = bancosmandatosService.GetBancosMandatos(obj.fkcuentas);
            var appService=new ApplicationHelper(Context);
            result.LstTarifas = appService.GetTarifasCuenta(TipoFlujo.Compra, id, Empresa).Select(f => new SelectListItem() { Value = f.Id, Text = f.Descripcion });

            return result;
        }

        public override Proveedores CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as ProveedoresModel;
            var result = _db.Proveedores.Create();

            foreach (var item in result.GetType().GetProperties())
            {
                if (typeof(ProveedoresModel).GetProperties().Any(f => f.Name.ToLower() == item.Name.ToLower()))
                    item.SetValue(result, viewmodel.get(item.Name));
            }

            if(viewmodel.Tipodeportes != null)
            {
                result.tipoportes = (int)viewmodel.Tipodeportes;
            }

            else
            {
                result.tipoportes = null;
            }

            result.fkpuertosfkpaises = viewmodel.Fkpuertos.Fkpaises;
            result.fkpuertosid = viewmodel.Fkpuertos.Id;

            return result;
        }

        public override Proveedores EditPersitance(IModelView obj)
        {
            var viewmodel = obj as ProveedoresModel;
            var result = _db.Proveedores.Single(f => f.fkcuentas == viewmodel.Fkcuentas && f.empresa == viewmodel.Empresa);

            foreach (var item in result.GetType().GetProperties())
            {
                if(typeof(ProveedoresModel).GetProperties().Any(f=>f.Name.ToLower()== item.Name.ToLower()))
                    item.SetValue(result, viewmodel.get(item.Name));
            }

            if (viewmodel.Tipodeportes != null)
            {
                result.tipoportes = (int)viewmodel.Tipodeportes;
            }

            else
            {
                result.tipoportes = null;
            }

            result.fkpuertosfkpaises = viewmodel.Fkpuertos.Fkpaises;
            result.fkpuertosid = viewmodel.Fkpuertos.Id;
            
            return result;
        }

        public override IModelView GetModelView(Proveedores obj)
        {
            var result= base.GetModelView(obj) as ProveedoresModel;

            result.Fkpuertos.Fkpaises = obj.fkpuertosfkpaises;
            result.Fkpuertos.Id = obj.fkpuertosid;

            if(obj.tipoportes != null)
            {
                result.Tipodeportes = (Tipoportes)obj.tipoportes;
            }

            return result;
        }

        #endregion
    }
}
