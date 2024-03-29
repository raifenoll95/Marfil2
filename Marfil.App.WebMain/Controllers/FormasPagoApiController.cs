﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.ControlsUI.Busquedas;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Newtonsoft.Json;

namespace Marfil.App.WebMain.Controllers
{
    public class FormasPagoApiController : ApiBaseController
    {
        public FormasPagoApiController(IContextService context) : base(context)
        {
        }

        public HttpResponseMessage Get()
        {
            
            using (var service = new FormasPagoService(ContextService))
            {
                var listformaspago = appService.GetListGruposFormasPago();
                var result = new ResultBusquedas<FormasPagoModel>()
                {
                    values = service.getAll().Select(f=>(FormasPagoModel)f).ToList(),
                    columns = new[]
                    {
                        new ColumnDefinition() { field = "Id", displayName = "Id", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }},
                        new ColumnDefinition() { field = "Nombre", displayName = "Nombre", visible = true },
                        new ColumnDefinition() { field="Gruposformaspago", displayName = "Grupo",visible=true, filter= new Filter() { selectOptions = listformaspago.Select(f=>new ComboListItem() { value=f.Descripcion,label=f.Descripcion}).ToList(),type="select"} }
                    }
                };

                foreach (var item in result.values)
                {
                    item.Gruposformaspago =
                        listformaspago.FirstOrDefault(f => f.Valor == item.FkGruposformaspago)?.Descripcion;
                }

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json");
                return response;
            }
        }

        // GET: api/Supercuentas/5
        public HttpResponseMessage Get(string id)
        {
            
            using (var service = new FormasPagoService(ContextService))
            {

                var list = service.get(id) as FormasPagoModel;
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                    "application/json");
                return response;
               

            }
        }
    }
}
