using System;
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
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Newtonsoft.Json;

namespace Marfil.App.WebMain.Controllers
{
    public class SeriesApiController : ApiBaseController
    {
        public SeriesApiController(IContextService context) : base(context)
        {
        }

        public HttpResponseMessage Get()
        {
            
            using (var service = FService.Instance.GetService(typeof(SeriesModel),ContextService) )
            {
                var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                var tipodocumento = nvc["tipodocumento"];
                var vector = service.getAll().Select(f => (SeriesModel) f).Where(f=>f.Bloqueado==false && (string.IsNullOrEmpty(f.Fkejercicios) || f.Fkejercicios== ContextService.Ejercicio));

                if (!string.IsNullOrEmpty(tipodocumento))
                {
                    vector = vector.Where(f => f.Tipodocumento == tipodocumento);
                }

                //Rai -- necesitamos saber cuales son los roles en los que está el usuario en este momento
                //ya que en los documentos venta y compra se filtran los tipos de series por usuario
                var serviceusuarios = FService.Instance.GetService(typeof(RolesModel), ContextService);

                var roles = serviceusuarios.getAll().Select(f => (RolesModel)f).ToList();
                List<RolesModel> rolesUsuario = new List<RolesModel>();

                //Recorremos todos los roles
                foreach (var rol in roles)
                {
                    //Miramos en el usuario del grupo de usuarios de ese rol
                    foreach (var usuario in rol.Usuarios.usuarios)
                    {
                        if (usuario.usuario == ContextService.Usuario)
                        {
                            rolesUsuario.Add(rol);
                        }
                    }
                }

                //Rai -- Si el usuario está en algún rol (debe estar si o si)
                List<SeriesModel> series = new List<SeriesModel>();

                if (rolesUsuario.Any())
                {
                    foreach(var serie in vector)
                    {
                        //Series sin grupos de usuario se muestran
                        if(String.IsNullOrEmpty(serie.Fkgruposusuarios))
                        {
                            series.Add(serie);
                        }

                        else
                        {
                            //Si tienen grupo de usuario, solo las que tienen el mismo grupo del usuario
                            if(rolesUsuario.Any(x => x.Id.ToString() == serie.Fkgruposusuarios))
                            {
                                series.Add(serie);
                            }
                        }
                    }
                }

                //Administrador
                else
                {
                    series.AddRange(vector);
                }

                var result = new ResultBusquedas<SeriesModel>()
                {
                    values = series,
                    columns = new[]
                    {
                        new ColumnDefinition() { field = "Id", displayName = "Id", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }},
                        new ColumnDefinition() { field = "Descripcion", displayName = "Descripción", visible = true },
                        new ColumnDefinition() { field = "Fkejerciciosdescripcion", displayName = "Ejercicio", visible = true },
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
            
            using (var service = FService.Instance.GetService(typeof(SeriesModel),ContextService))
            {
                var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                var tipodocumento = nvc["tipodocumento"];
                var list = service.get(tipodocumento + "-" + id) as SeriesModel;

                //Rai -- necesitamos saber cuales son los roles en los que está el usuario en este momento
                //ya que en los documentos venta y compra se filtran los tipos de series por usuario
                var serviceusuarios = FService.Instance.GetService(typeof(RolesModel), ContextService);

                var roles = serviceusuarios.getAll().Select(f => (RolesModel)f).ToList();
                List<RolesModel> rolesUsuario = new List<RolesModel>();

                //Recorremos todos los roles
                foreach (var rol in roles)
                {
                    //Miramos en el usuario del grupo de usuarios de ese rol
                    foreach (var usuario in rol.Usuarios.usuarios)
                    {
                        if (usuario.usuario == ContextService.Usuario)
                        {
                            rolesUsuario.Add(rol);
                        }
                    }
                }

                bool serie = false;

                //Roles del user
                if (rolesUsuario.Any())
                {
                    if (String.IsNullOrEmpty(list.Fkgruposusuarios))
                    {
                        serie = true;
                    }
                    else
                    {
                        //Si tienen grupo de usuario, solo las que tienen el mismo grupo del usuario
                        if (rolesUsuario.Any(x => x.Id.ToString() == list.Fkgruposusuarios))
                        {
                            serie = true;
                        }
                    }
                }

                //Administrador
                else
                {
                    serie = true;
                }

                if (serie)
                {
                    if (!list.Bloqueado && ((string.IsNullOrEmpty(list.Fkejercicios) || list.Fkejercicios == ContextService.Ejercicio)))
                    {
                        var response = Request.CreateResponse(HttpStatusCode.OK);
                        response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                            "application/json");
                        return response;
                    }
                    else
                    {
                        var response = Request.CreateResponse(HttpStatusCode.Conflict);

                        return response;
                    }
                }

                return Request.CreateResponse(HttpStatusCode.Conflict);
            }
        }
    }
}
