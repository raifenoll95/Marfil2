using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.ControlsUI.Login;
using Marfil.Dom.ControlsUI.Login.Ficheros;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;
using Constantes = Marfil.App.WebMain.Misc.Constantes;

namespace Marfil.App.Web.Controllers
{
    public class LoginController : Controller
    {
        #region Members

        private readonly ILoginService _loginService;
        private readonly ILoginViewService _loginViewService;
        private readonly string _dominio;
        #endregion

        #region CTR

        public LoginController(ILoginService loginService, ILoginViewService loginviewService)
        {
            _loginService = loginService;
            _loginViewService = loginviewService;
            _dominio = System.Web.HttpContext.Current.Request.Url.DnsSafeHost;
        }

        #endregion

        // GET: Login
        [AllowAnonymous]
        public ActionResult Index(string id)
        {
        
            var cadenaRecordarme = WebHelper.ReadCookie("login");
            var model = new LoginModel()
            {
                ReturUrl = Request["ReturnUrl"],
                Usuario = string.IsNullOrEmpty(cadenaRecordarme) ? string.Empty : cadenaRecordarme.Split('#')[0],
                //BaseDatos = loginviewmodel?.Basedatos, //string.IsNullOrEmpty(cadenaRecordarme) ? string.Empty : cadenaRecordarme.Split('#')[1],
                Recordarme = !string.IsNullOrEmpty(cadenaRecordarme)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Index(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var context = new ContextLogin();
                    var licenciaModel = new LicenciasaplicacionService(_dominio);
                    context.BaseDatos = licenciaModel.Basedatos;
                    context.Azureblob = licenciaModel.Azureblob;
                    using (var startupService = new StartupService(context, context.BaseDatos))
                    {
                        if (startupService.ExisteAdmin()) 
                        {
                            HttpCookie securityCookie;
                            if (_loginService.Login(_dominio, model.Usuario, model.Contraseña,  out securityCookie))
                            {
                                //ensure != null
                                securityCookie.Expires = DateTime.Now.AddMinutes(120);
                                Response.Cookies.Add(securityCookie);
                                if (model.Recordarme)
                                    WebHelper.CreateCookie("login", $"{model.Usuario}#{licenciaModel.Basedatos}");
                                else
                                    WebHelper.RemoveCookie();

                                return RedirectToLocal(model.ReturUrl);
                            }

                            ModelState.AddModelError("", "Usuario, contraseña o base de datos incorrectas");
                        }
                        else
                        {

                            return RedirectToAction("Admin", "Startup", new {id = licenciaModel.Basedatos});
                        }
                    }
                }
                catch (LicenciaException  ex)
                {
                    ModelState.AddModelError("", ex.Message); //añadir lenguaje settings
                }
                catch (UsuarioactivoException ex)
                {
                    ModelState.AddModelError("", ex.Message); //añadir lenguaje settings
                    
                }
                catch (UsuarioensuoException ex)
                {
                    model.Usuariobloqueado = true;
                }

                //Rai
                catch (CambiarEmpresaException ex)
                {
                    ModelState.AddModelError("", ex.Message); //añadir lenguaje settings
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Usuario, contraseña o base de datos incorrectas");//añadir lenguaje settings
                }
                
                    
            }

            return View(model);
        }

        public ActionResult LogOff()
        {
            _loginService.Logout(System.Web.HttpContext.Current.User as ICustomPrincipal);
            FormsAuthentication.SignOut();
            Session.Abandon();
           
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForzarDesconexion(string usuario)
        {
            try
            {
                var licenciaModel = new LicenciasaplicacionService(_dominio);
                _loginService.Forzardesconexion(licenciaModel.Basedatos, usuario);

                TempData[Constantes.VariableMensajeExito] = General.MensajeExitoOperacionForzarDesconexion;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("",ex.Message);
            }
            

            return RedirectToAction("Index");
        }

        #region Helper

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

      

        public string GetCodigo()
        {
            return string.IsNullOrEmpty(HttpContext.Request.Params["id"]) ?
                System.Web.HttpContext.Current.Request.Url.DnsSafeHost:
            HttpContext.Request.Params["id"];
        }

        #endregion
    }
}