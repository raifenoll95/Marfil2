using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos.Helper;
using Resources;

namespace Marfil.App.WebMain.Controllers
{
    [Authorize]
    public class UsuariosController : GenericController<UsuariosModel>
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

        public UsuariosController(IContextService context) : base(context)
        {

        }

        #endregion

        public override ActionResult CreateOperacion(UsuariosModel model)
        {

            if (HttpContext.Request.Files.Count > 0)
            {
                if (HttpContext.Request.Files["Firma"] != null)
                {
                    using (var ms = HttpContext.Request.Files["Firma"].InputStream)
                    {
                        using (var sr = new StreamReader(ms))
                        {
                            model.Contenidofirma = sr.ReadToEnd();
                        }
                    }
                }
                
            }

            return base.CreateOperacion(model);
        }

        public override ActionResult EditOperacion(UsuariosModel model)
        {
            if (HttpContext.Request.Files.Count > 0)
            {
                if (HttpContext.Request.Files["Firma"] != null)
                {
                    using (var ms = HttpContext.Request.Files["Firma"].InputStream)
                    {
                        using (var sr = new StreamReader(ms))
                        {
                            model.Contenidofirma = sr.ReadToEnd();
                        }
                    }
                }

            }
            return base.EditOperacion(model);
        }

      

        protected override IEnumerable<IToolbaritem> IndexToolbar()
        {
            var result = base.IndexToolbar().ToList();

            

            if (ContextService.Id == Guid.Empty)
            {
               result.Add(new ToolbarSeparatorModel());

                result.Add(new ToolbarActionModel()
                {
                    Icono = "fa fa-pencil",
                    Texto="Editar usuario administrador",
                    Url = Url.Action("Edit",new {id= ContextService.Id} )

                });
            }

            return result;
        }
    }



    
}
