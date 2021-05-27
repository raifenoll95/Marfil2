using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
//using Marfil.Dom.Persistencia.Model.Documentos.Albaranes;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos.Importar;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos.StateMachine;
using Marfil.Inf.Genericos.Helper;
using Marfil.Inf.ResourcesGlobalization.Textos.Entidades;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Resources;
using Marfil.Dom.Persistencia.Model.Contabilidad.Maes;
using System.IO;
using System.Data;

namespace Marfil.App.WebMain.Controllers
{
    public class MaesController : GenericController<MaesModel>
    {
        private const string session = "_maes_";
        //private const string sessiontotales = "_movstotales_";
        
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "maes";
            var permisos = appService.GetPermisosMenu("maes");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        #region CTR

        public MaesController(IContextService context) : base(context)
        {

        }

        #endregion

        protected override IGestionService createService(IModelView model)
        {

            var result = FService.Instance.GetService(typeof(MaesModel), ContextService) as MaesService;
            result.EjercicioId = ContextService.Ejercicio;
            return result;
        }
    }
}