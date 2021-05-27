using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.WebPages;
using Marfil.Inf.Genericos;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.App.WebMain.Misc
{
    public static class HtmlExtensions
    {
        public static IHtmlString EnumDropDownListForAngular<TEnum>(this HtmlHelper htmlHelper, string name, TEnum selectedValue,string model,string htmlAttributes = "")
        {
            var sb = new StringBuilder();

            var t = Nullable.GetUnderlyingType(typeof(TEnum)) ?? typeof(TEnum);

            IEnumerable<TEnum> values = Enum.GetValues(t)
                .Cast<TEnum>();

            var itemsString = "";

            if (Nullable.GetUnderlyingType(typeof(TEnum)) != null)
            {
                itemsString += string.Format("<option ng-value=\"{0}\"  ng-selected=\"{2} && {2}.length==0\">{1}</option>", "", "", model);
            }
            foreach (TEnum value in values)
            {
                itemsString += string.Format("<option ng-value=\"{0}\" ng-selected=\"'{0}' == {2}\">{1}</option>", Funciones.GetIntEnumByStringValueAttribute(value), Funciones.GetEnumByStringValueAttribute(value), model);
            }
            
            sb.AppendLine(string.Format("<select name=\"{0}\" class=\"form-control\" ng-model=\"{0}\" id=\"{2}\" {1}>", model, htmlAttributes,name));
            sb.AppendLine(itemsString);
            sb.AppendLine("</select>");
            sb.AppendLine(string.Format("<input type=\"hidden\" ng-model=\"{0}\" name=\"{0}\"  value=\"{{{{{1}}}}}\" />", name, model));
            
            return htmlHelper.Raw(sb.ToString());
        }

        public static MvcHtmlString EnumDropDownList<TEnum>(this HtmlHelper htmlHelper, string name, TEnum selectedValue,object htmlAttributes=null)
        {
            var t =Nullable.GetUnderlyingType(typeof (TEnum)) ?? typeof (TEnum);
            
            IEnumerable<TEnum> values = Enum.GetValues(t)
                .Cast<TEnum>();

            IEnumerable<SelectListItem> items =
                from value in values
                select new SelectListItem
                {
                    Text = Funciones.GetEnumByStringValueAttribute(value),
                    Value = Funciones.GetIntEnumByStringValueAttribute(value).ToString(),
                    Selected = (value.Equals(selectedValue))
                };

            var listItems = items.ToList();
            if (Nullable.GetUnderlyingType(typeof (TEnum)) != null)
            {
                listItems.Insert(0,new SelectListItem() { Text = "",Value = "", Selected = ("".Equals(selectedValue)) });
            }
            return htmlHelper.DropDownList(
                name,
                listItems,
                htmlAttributes
                );
        }

      

        public static string GetDisplayName<TModel, TProperty>(this TModel model, Expression<Func<TModel, TProperty>> expression)
        {

            Type type = typeof(TModel);

            MemberExpression memberExpression = (MemberExpression)expression.Body;
            string propertyName = ((memberExpression.Member is PropertyInfo) ? memberExpression.Member.Name : null);

            // First look into attributes on a type and it's parents
            DisplayAttribute attr;
            attr = (DisplayAttribute)type.GetProperty(propertyName).GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();

            // Look for [MetadataType] attribute in type hierarchy
            // http://stackoverflow.com/questions/1910532/attribute-isdefined-doesnt-see-attributes-applied-with-metadatatype-class
            if (attr == null)
            {
                MetadataTypeAttribute metadataType = (MetadataTypeAttribute)type.GetCustomAttributes(typeof(MetadataTypeAttribute), true).FirstOrDefault();
                if (metadataType != null)
                {
                    var property = metadataType.MetadataClassType.GetProperty(propertyName);
                    if (property != null)
                    {
                        attr = (DisplayAttribute)property.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault();
                    }
                }
            }
            return (attr != null) ? attr.Name : String.Empty;


        }

       public static void AddScript(this HtmlHelper htmlHelper,string url)
        {
            var key = string.Format("_script_{0}", url);
            if (!htmlHelper.ViewContext.HttpContext.Items.Contains(key))
                htmlHelper.ViewContext.HttpContext.Items[key] = "<script src=\"" + url + "\"></script>";
        }

        public static IHtmlString RenderScripts(this HtmlHelper htmlHelper)
        {
            var list = htmlHelper.ViewContext.HttpContext.Items.Keys.OfType<string>().Where(f => f.StartsWith("_script_")).OrderBy(f => f);
            foreach (var key in list)
            {
                var template = htmlHelper.ViewContext.HttpContext.Items[key] as string;
                if (!string.IsNullOrEmpty(template))
                {
                    htmlHelper.ViewContext.Writer.Write(template);
                }

            }
            return MvcHtmlString.Empty;
        }


       

        public static MvcHtmlString Css(this HtmlHelper htmlHelper, Func<object, HelperResult> template)
        {
            htmlHelper.ViewContext.HttpContext.Items["_css_" + htmlHelper.ViewContext.HttpContext.Items.Count + "_" + Guid.NewGuid()] = template;
            return MvcHtmlString.Empty;
        }

        public static void AddCss(this HtmlHelper htmlHelper, string url)
        {
            var key = string.Format("_css_{0}", url);
            if (!htmlHelper.ViewContext.HttpContext.Items.Contains(key))
                htmlHelper.ViewContext.HttpContext.Items[key] = "<link rel=\"stylesheet\" href=\"" + url + "\" />";
        }

        public static IHtmlString RenderCss(this HtmlHelper htmlHelper)
        {
            var list = htmlHelper.ViewContext.HttpContext.Items.Keys.OfType<string>().Where(f => f.StartsWith("_css_")).OrderBy(f => f);

            foreach (var key in list)
            {
                var template = htmlHelper.ViewContext.HttpContext.Items[key] as Func<object, HelperResult>;
                if (template != null)
                {
                    htmlHelper.ViewContext.Writer.Write(template(null));
                }
                else
                {
                    var template2 = htmlHelper.ViewContext.HttpContext.Items[key] as string;
                    if (!string.IsNullOrEmpty(template2))
                    {
                        htmlHelper.ViewContext.Writer.Write(template2);
                    }
                }



            }
            return MvcHtmlString.Empty;
        }
    }
}