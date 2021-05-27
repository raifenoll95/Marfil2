using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Newtonsoft.Json;

namespace Marfil.App.WebMain.Controllers
{
    public class FamiliasproductosController : GenericController<FamiliasproductosModel>
    {
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "familiasproductos";
            var permisos = appService.GetPermisosMenu("familiasproductos");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        #region CTR

        public FamiliasproductosController(IContextService context) : base(context)
        {

        }

        public ActionResult Validar(string familia)
        {
            var servicioFamilia = FService.Instance.GetService(typeof(FamiliasproductosModel), ContextService) as FamiliasproductosService;
            var familiaModel = servicioFamilia.get(familia) as FamiliasproductosModel;
            var dataModel = JsonConvert.SerializeObject(familiaModel, Formatting.Indented);
            return Json(dataModel, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}