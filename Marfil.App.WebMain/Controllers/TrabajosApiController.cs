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
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Newtonsoft.Json;

namespace Marfil.App.WebMain.Controllers
{
    public class TrabajosApiController : ApiBaseController
    {
        public TrabajosApiController(IContextService context) : base(context)
        {
        }

        public HttpResponseMessage Get()
        {
            
            using (var service = FService.Instance.GetService(typeof(TrabajosModel),ContextService)  as TrabajosService)
            {
                var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                var tipotrabajo = nvc["tipotrabajo"];

                var vector = service.GetAll<TrabajosModel>();

                if (tipotrabajo != null && tipotrabajo == "0")
                {
                   vector = vector.Where(f => f.Tipotrabajo == TipoTrabajo.Aserrado);
                }
                else if(tipotrabajo != null && tipotrabajo == "1")
                {
                   vector = vector.Where(f => f.Tipotrabajo == TipoTrabajo.Elaborado);
                }        

                var result = new ResultBusquedas<TrabajosModel>()
                {
                    values = vector,
                    columns = new[]
                    {
                        new ColumnDefinition() { field = "Id", displayName = "Id", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }},
                        new ColumnDefinition() { field = "Descripcion", displayName = "Descripción", visible = true },
                        new ColumnDefinition() { field = "Acabadoinicialdescripcion", displayName = "Ac. Inicial", visible = true },
                        new ColumnDefinition() { field = "Acabadofinaldescripcion", displayName = "Ac. Final", visible = true },
                    }
                };


                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json");
                return response;
            }
        }

        // GET: api/Supercuentas/5
        public HttpResponseMessage Get(string id)
        {
            
            using (var service = FService.Instance.GetService(typeof(TrabajosModel),ContextService))
            {
                
                
                var list = service.get(id) as TrabajosModel;
                

                
                
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                        "application/json");
                    return response;
                
                


            }
        }
    }
}
