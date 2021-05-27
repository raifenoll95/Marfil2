using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using DevExpress.Web.Mvc;
using System.Net;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.ServicesView;
using Resources;
using Marfil.Dom.Persistencia.Model.CRM;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.Model.Configuracion;

namespace Marfil.App.WebMain.Controllers
{
    public class PeticionesAsincronasController : GenericController<PeticionesAsincronasModel>
    {                
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            //IsActivado = ContextService.IsSuperAdmin;
            //CanCrear = false;
            //CanModificar = false;
            //CanEliminar = false;
            MenuName = "peticiones";
            var permisos = appService.GetPermisosMenu("peticiones");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        #region CTR

        public PeticionesAsincronasController(IContextService context) : base(context)
        {

        }

        #endregion

    }
}