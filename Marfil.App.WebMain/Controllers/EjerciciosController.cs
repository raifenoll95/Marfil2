using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace Marfil.App.WebMain.Controllers
{
    public class EjerciciosController : GenericController<EjerciciosModel>
    {
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            
            IsActivado = ContextService.IsSuperAdmin;
            CanModificar = CanCrear = CanEliminar = IsActivado;
        }

        #region CTR

        public EjerciciosController(IContextService context) : base(context)
        {

        }

        #endregion

        public ActionResult obtenerEjercicio()
        {
            JavaScriptSerializer serializer1 = new JavaScriptSerializer();
            var id = ContextService.Ejercicio;
            var servicioEjecicios = FService.Instance.GetService(typeof(EjerciciosModel), ContextService) as EjerciciosService;
            var EjercicioModel = servicioEjecicios.get(id) as EjerciciosModel;
            EjercicioModel.DescSerieContable = servicioEjecicios.DescSerieContable(EjercicioModel.FkseriescontablesREM); 
            var data = JsonConvert.SerializeObject(EjercicioModel, Formatting.Indented);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}