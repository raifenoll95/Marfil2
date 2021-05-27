using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Marfil.App.WebMain.Misc
{
    public static class GlobalizationHelpers
    {
        /// <summary>
        /// Taken from Scott Hanselman's blog post: http://www.hanselman.com/blog/GlobalizationInternationalizationAndLocalizationInASPNETMVC3JavaScriptAndJQueryPart1.aspx
        /// </summary>
        /// <typeparam name="t"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <returns></returns>
        public static IHtmlString MetaAcceptLanguage<t>(this HtmlHelper<t> htmlHelper)
        {
            var acceptLanguage = HttpUtility.HtmlAttributeEncode(Thread.CurrentThread.CurrentUICulture.ToString());
            return new HtmlString(string.Format("<meta name=\"accept-language\" content=\"{0}\" />", acceptLanguage));
        }

        /// <summary>
        /// Return the JavaScript bundle for this users culture
        /// </summary>
        /// <typeparam name="t"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <returns>a culture bundle that looks something like this: "~/js-culture.en-GB"</returns>
        public static string JsCultureBundle<t>(this HtmlHelper<t> htmlHelper)
        {
            return "~/js-culture." + Thread.CurrentThread.CurrentUICulture.ToString();
        }
    }
}