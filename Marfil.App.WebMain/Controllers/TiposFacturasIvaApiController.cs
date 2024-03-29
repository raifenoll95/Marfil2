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
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Newtonsoft.Json;

namespace Marfil.App.WebMain.Controllers
{
    public class TiposFacturasIvaApiController : ApiBaseController
    {
        public TiposFacturasIvaApiController(IContextService context) : base(context)
        {
        }

        public HttpResponseMessage Get()
        {

            using (var service = FService.Instance.GetService(typeof(TiposFacturasIvaModel), ContextService))
            {
                var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                var tipotercero = nvc["tipotercero"]; // 0 - proveedores / acreedores , 1 - clientes

                var registros = service.getAll().Select(f => (TiposFacturasIvaModel)f).Where(f => f.Empresa == ContextService.Empresa);

                if (tipotercero == "0")
                {
                    registros = registros.Where(f => f.Tipocircuito == TipoFactura.Soportado);
                }
                else if (tipotercero == "1")
                {
                    registros = registros.Where(f => f.Tipocircuito == TipoFactura.Repercutido);
                }

                var result = new ResultBusquedas<TiposFacturasIvaModel>()
                {
                    values = registros,
                    columns = new[]
                    {
                        new ColumnDefinition() { field = "Id", displayName = "Id", visible = true},
                        new ColumnDefinition() { field = "Descripcion", displayName = "Descripción", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH } },
                    }
                };


                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json");
                return response;
            }
        }

        public HttpResponseMessage Get(string id)
        {

            using (var service = FService.Instance.GetService(typeof(TiposFacturasIvaModel), ContextService))
            {

                var list = service.get(id);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                    "application/json");
                return response;

            }
        }

    }
}