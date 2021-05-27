using Marfil.App.WebMain.Services;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion.Empresa;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Inf.Genericos.Helper;
using System.Runtime.Remoting.Contexts;
using System.Web.Mvc;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using System.Linq;
using Newtonsoft.Json;
using Marfil.Dom.Persistencia.Model;

namespace Marfil.App.WebMain.Controllers
{
    public class HelperViewController : GenericController<ArticulosModel>
    {
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "articulos";
            var permisos = appService.GetPermisosMenu("articulos");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        public HelperViewController(IContextService context) : base(context)
        {
        }

        //Nos devuelve la longitud que permite las cuentas de la empresa
        public JsonResult comprobarNumeroDigitos()
        {
            var modelview = Helper.fModel.GetModel<EmpresaModel>(ContextService) as EmpresaModel;
            int digitos = Funciones.Qint(modelview.DigitosCuentas).Value;
            var data = JsonConvert.SerializeObject(digitos, Formatting.Indented);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //Comprobamos si el input de la cuenta introducida existe o no
        public JsonResult existeCuenta(string input)
        {
            var cuentasService = FService.Instance.GetService(typeof(CuentasModel), ContextService) as CuentasService;
            var cuentas = cuentasService.GetCuentasTercerosArticulos();
            var data = "";
            if(cuentas.Any(f => f.Id == input))
            {
                var cuenta = cuentas.Single(f => f.Id == input) as CuentasModel;
                data = JsonConvert.SerializeObject(cuenta, Formatting.Indented);          
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult existeCuenta4Movs(string input)
        {
            var cuentasService = FService.Instance.GetService(typeof(CuentasModel), ContextService) as CuentasService;
            var cuentas = cuentasService.GetCuentasContablesNivel(0);
            var data = "";
            if (cuentas.Any(f => f.Id == input))
            {
                var cuenta = cuentas.Single(f => f.Id == input) as CuentasModel;
                data = JsonConvert.SerializeObject(cuenta, Formatting.Indented);
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //Devuelve la direccion asociada a ese numero de cuenta
        public JsonResult existeCuenta4Campanyas(string input)
        {
            var direccionesService = FService.Instance.GetService(typeof(DireccionesLinModel), ContextService) as DireccionesService;
            var direccion = direccionesService.getDireccion(input);
            var data = JsonConvert.SerializeObject(direccion, Formatting.Indented);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}