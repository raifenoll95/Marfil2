using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;

using Newtonsoft.Json;
using Marfil.Dom.ControlsUI.Busquedas;

namespace Marfil.Sam.ControlsUI.Controllers
{
    public class TestApiController : ApiController
    {
        public class TestResult
        {
            public int id { get; set; }
            public string nombre { get; set; }
        }

        private IEnumerable<TestResult> _vector = new[]
        {new TestResult() {id = 1, nombre = "Prueba1"}, new TestResult() {id = 2, nombre = "Prueba2"}};
        // GET api/<controller>
        public HttpResponseMessage Get()
        {
            var items = new ResultBusquedas<TestResult>
            {
                columns =new [] {new ColumnDefinition() { field = "nombre",displayName = "Nombre", visible=true}  },
                values = _vector
            };
            
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(JsonConvert.SerializeObject(items), Encoding.UTF8, "application/json");
            return response;
        }

        // GET api/<controller>/5
        public HttpResponseMessage Get(int id)
        {
           
           
            

            var response = Request.CreateResponse(HttpStatusCode.NotAcceptable);
            if (_vector.Any(f => f.id == id))
            {
                var result = new ResultBusquedas<TestResult>
                {
                    columns = new[] { new ColumnDefinition() { field = "nombre", displayName = "Nombre", visible = true } },
                    values = new[] { _vector.First(f => f.id == id) }
                };
                response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json");
            }
            
            return response;
            
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}