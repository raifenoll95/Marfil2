using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Marfil.App.WebMain.Services;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using Marfil.Dom.Persistencia.Model.Diseñador;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Preferencias;
using Marfil.Inf.Genericos.Helper;
using ConstantesLogin= Marfil.Dom.ControlsUI.Login.Constantes;
namespace Marfil.App.WebMain.Misc
{
    
    public class WebHelper
    {
        public static bool DevexpressAA
        {
            get
            {
                return Funciones.Qbool(HttpContext.Current.Request.Cookies[ConstantesLogin.COOKIEAA]?.Value??"false");
                //return Funciones.Qbool(ConfigurationManager.AppSettings["DevexpressAA"]);
            }
        }
        public static IEnumerable<TablasVariasTiposNif> TiposNif
        {
            get
            {
                if (HttpContext.Current.Session["TiposNif"]==null)
                {
                    var user = HttpContext.Current.User as ICustomPrincipal;
                    var context = new ContextService()
                    {
                        BaseDatos = user.BaseDatos,
                        Empresa = user.BaseDatos
                    };
                    var appService=new ApplicationHelper(context);
                    HttpContext.Current.Session["TiposNif"] = appService.GetListTiposNif();
                }

                return (IEnumerable<TablasVariasTiposNif>)HttpContext.Current.Session["TiposNif"];
            }
        }

        public static ILogService Log { get; set; }

        private const string CookieName = "Totware";

        public static IContextService ContextService
        {
            get { return new ContextService(); }
        }
        #region Cookies

        public static void CreateCookie(string name, string value)
        {
            HttpCookie myCookie = new HttpCookie(CookieName);

            //Add key-values in the cookie
            myCookie.Values.Add(name, value);

            //set cookie expiry date-time. Made it to last for next 12 hours.
            myCookie.Expires = DateTime.Now.AddMonths(1);

            //Most important, write the cookie to client.
            HttpContext.Current.Response.Cookies.Add(myCookie);
        }

        public static string ReadCookie(string name)
        {
            //Assuming user comes back after several hours. several < 12.
            //Read the cookie from Request.
            HttpCookie myCookie = HttpContext.Current.Request.Cookies[CookieName];
            if (myCookie != null)
            {
                //ok - cookie is found.
                //Gracefully check if the cookie has the key-value as expected.
                if (!string.IsNullOrEmpty(myCookie.Values[name]))
                {
                    return myCookie.Values[name];

                }
            }

            return string.Empty;
        }

        public static void RemoveCookie()
        {
            var myCookie = HttpContext.Current.Request.Cookies[CookieName];
            if (myCookie != null)
            {
                myCookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Add(myCookie);
            }
        }

        #endregion

        public static bool IsSuperadmin()
        {
            var user = HttpContext.Current.User as ICustomPrincipal;
            if (user != null)
            {
                return user.Usuario == ApplicationHelper.UsuariosAdministrador;
            }
            return false;
        }

        public static ApplicationHelper GetApplicationHelper()
        {
            return new ApplicationHelper(new ContextService());
        }


        public static DocumentosBotonImprimirModel GetListFormatos(TipoDocumentoImpresion tipo,string referencia)
        {
            var user = HttpContext.Current.User as ICustomPrincipal;
            using (var db = MarfilEntities.ConnectToSqlServer(user.BaseDatos))
            {
                var servicePreferencias = new PreferenciasUsuarioService(db);
                var doc = servicePreferencias.GePreferencia(TiposPreferencias.DocumentoImpresionDefecto, user.Id, tipo.ToString(), "Defecto") as PreferenciaDocumentoImpresionDefecto;
                var service = new DocumentosUsuarioService(db);
                {
                    var lst =
                        service.GetDocumentos(tipo, user.Id)
                            .Where(f => f.Tiporeport == TipoReport.Report);
                    return new DocumentosBotonImprimirModel()
                    {
                        Tipo = tipo,
                        Lineas = lst.Select(f => f.Nombre),
                        Primarykey = referencia,
                        Defecto = doc?.Name
                    };
                }
            }

        }

    }
}