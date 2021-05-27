using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.ControlsUI.Busquedas;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Mobile;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos.Helper;
using Newtonsoft.Json;
using Filter = Marfil.Dom.ControlsUI.Busquedas.Filter;


namespace Marfil.App.WebMain.Controllers
{
    public class StZonasAlmacenes 
    {
        public string Fkalmacen { get; set; }
        public string Id { get; set; }
    }

    public class MobileZonasAlmacenesApiController : BasicAuthHttpModule
    {
        public MobileZonasAlmacenesApiController(ILoginService service) : base(service)
        {
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult ZonasAlmacen(StZonasAlmacenes model)
        {
            if (string.IsNullOrEmpty(model.Id))
            {
                return CrearListado(model);
            }

            return CrearZonaAlmacen(model);
        }


        private ActionResult CrearListado(StZonasAlmacenes model)
        {
            var almacen = model.Fkalmacen;
            using (var service = FService.Instance.GetService(typeof(AlmacenesModel), ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos)) as AlmacenesService)
            {
                var almacenModel = service.get(almacen) as AlmacenesModel;

                var list = almacenModel.Lineas;

                var result = new ResultBusquedas<AlmacenesZonasModel>()
                {
                    values = list,
                    columns = new[]
                    {
                        new ColumnDefinition() { field = "Id", displayName = "Id", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }},
                        new ColumnDefinition() { field = "Descripcion", displayName = "Descripción", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }},
                        new ColumnDefinition() { field = "Coordenadas", displayName = "Coordenadas", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }},
                    }
                };

                return Json(result);

            }
        }

        private ActionResult CrearZonaAlmacen(StZonasAlmacenes model)
        {
            using (var service = FService.Instance.GetService(typeof(AlmacenesModel), ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos)) as AlmacenesService)
            {
                var almacen = model.Fkalmacen;
                var list = service.get(almacen) as AlmacenesModel;
                var intId = Funciones.Qint(model.Id) ?? 0;

                return Json(list.Lineas.First(f => f.Id == intId));


            }
        }

        
    }
}
