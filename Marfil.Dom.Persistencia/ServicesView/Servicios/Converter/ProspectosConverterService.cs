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
using Marfil.Dom.Persistencia.Model.CRM;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class ProspectosConverterService : BaseConverterModel<ProspectosModel, Prospectos>
    {
        

        #region CTR

        public ProspectosConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
            
        }

        #endregion

        #region Api

        public override IEnumerable<IModelView> GetAll()
        {
            var fservice = FService.Instance;
            var cuentasService = fservice.GetService(typeof(CuentasModel), Context, _db);
            var list = _db.Prospectos.Where(f => f.empresa == Empresa).ToList();


            var result = new List<ProspectosModel>();
            var fModel = new FModel();
            var fService = FService.Instance;
            var direccionesService = fService.GetService(typeof(DireccionesLinModel), Context, _db) as DireccionesService;
            foreach (var item in list)
            {
                var newitem = GetModelView(item) as ProspectosModel;
                newitem.Cuentas = cuentasService.get(item.fkcuentas) as CuentasModel;
                newitem.Direcciones = fModel.GetModel<DireccionesModel>(Context);
                newitem.Direcciones.Direcciones = direccionesService.GetDirecciones(Empresa, TiposCuentas.Prospectos, newitem.Fkcuentas);
                newitem.Direcciones.Id = Guid.NewGuid();
                newitem.Direcciones.Tipotercero = (int)TiposCuentas.Prospectos;
                result.Add(newitem);
            }

            return result;
        }

        public override bool Exists(string id)
        {
            return _db.Set<Prospectos>().Any(f => f.fkcuentas == id && f.empresa == Empresa);
        }

        public override IModelView CreateView(string id)
        {
            var obj = _db.Set<Prospectos>().Single(f => f.fkcuentas == id && f.empresa == Empresa);
            var result = GetModelView(obj) as ProspectosModel;
            var fService = FService.Instance;
            var fModel = new FModel();
            var cuentasService = fService.GetService(typeof(CuentasModel), Context, _db);
            var direccionesService = fService.GetService(typeof(DireccionesLinModel), Context, _db) as DireccionesService;
            var contactosService = fService.GetService(typeof(ContactosLinModel), Context, _db) as ContactosService;
            var oportunidadesService = fService.GetService(typeof(OportunidadesModel), Context, _db) as OportunidadesService;

            result.Cuentas = cuentasService.get(result.Fkcuentas) as CuentasModel;
            result.Direcciones = fModel.GetModel<DireccionesModel>(Context);
            result.Direcciones.Direcciones = direccionesService.GetDirecciones(Empresa, TiposCuentas.Prospectos, obj.fkcuentas);
            result.Direcciones.Id = Guid.NewGuid();
            result.Direcciones.Tipotercero = (int)TiposCuentas.Prospectos;

            result.Contactos = fModel.GetModel<ContactosModel>(Context);
            result.Contactos.Contactos = contactosService.GetContactos(TiposCuentas.Prospectos, obj.fkcuentas);
            result.Contactos.Id = Guid.NewGuid();
            result.Contactos.Tipotercero = (int)TiposCuentas.Prospectos;
            result.Contactos.Direcciones = result.Direcciones;

            result.Oportunidades = oportunidadesService.getOportunidaesCliente(result.Fkcuentas).ToList();

            var appService = new ApplicationHelper(Context);
            result.LstTarifas = appService.GetTarifasCuenta(TipoFlujo.Venta, id,Empresa).Select(f => new SelectListItem() { Value = f.Id, Text = f.Descripcion });

            return result;
        }

        public override Prospectos CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as ProspectosModel;
            var result = _db.Prospectos.Create();

            foreach (var item in result.GetType().GetProperties())
            {
                if (typeof(ProspectosModel).GetProperties().Any(f => f.Name.ToLower() == item.Name.ToLower()))
                    item.SetValue(result, viewmodel.get(item.Name));
            }
         
            result.fkpuertosfkpaises = viewmodel.Fkpuertos.Fkpaises;
            result.fkpuertosid = viewmodel.Fkpuertos.Id;

            return result;
        }

        public override Prospectos EditPersitance(IModelView obj)
        {
            var viewmodel = obj as ProspectosModel;
            var result = _db.Prospectos.Single(f => f.fkcuentas == viewmodel.Fkcuentas && f.empresa == viewmodel.Empresa);

            foreach (var item in result.GetType().GetProperties())
            {
                if(typeof(ProspectosModel).GetProperties().Any(f=>f.Name.ToLower()== item.Name.ToLower()))
                    item.SetValue(result, viewmodel.get(item.Name));
            }
            
            result.fkpuertosfkpaises = viewmodel.Fkpuertos.Fkpaises;
            result.fkpuertosid = viewmodel.Fkpuertos.Id;
            
            return result;
        }

        public override IModelView GetModelView(Prospectos obj)
        {
            var result= base.GetModelView(obj) as ProspectosModel;
            result.Fkidiomas = obj.fkidiomas;
            result.Fkpuertos.Fkpaises = obj.fkpuertosfkpaises;
            result.Fkpuertos.Id = obj.fkpuertosid;
            
            return result;
        }

        #endregion
    }
}
