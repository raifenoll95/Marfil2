using Marfil.Dom.ControlsUI.Busquedas;
using Marfil.Dom.Persistencia.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.ComponentModel.DataAnnotations;
using Resources;
using ValidationException = Marfil.Dom.Persistencia.Helpers.ValidationException;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados
{
    public class TablasVariasPropAdeudoModel : IModelView, IGetColumnasGrid, ICanValidate
    {
        #region Properties
        [Key]
        [Required]
        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.TablasVarias), Name = "Valor")]
        [MaxLength(3)]
        public string Valor { get; set; }

        [Required]
        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.TablasVarias), Name = "Codigo")]
        [MaxLength(4, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [MinLength(4, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MinLength")]
        public string Codigo { get; set; }

        [Required]
        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.TablasVarias), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Required]
        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.TablasVarias), Name = "Descripcion2")]
        public string Descripcion2 { get; set; }


        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.TablasVarias), Name = "Clasificacion")]
        public string Clasificacion { get; set; }

        #endregion

        public void createNewPrimaryKey()
        {
            throw new NotImplementedException();
        }

        public object get(string propertyName)
        {
            return GetType().GetProperty(propertyName).GetValue(this);
        }

        public IEnumerable<ColumnDefinition> GetColumnDefinitions()
        {
            return new[]
{
                new ColumnDefinition() {field = "Clave", displayName = "Clave", visible = true},
                new ColumnDefinition() {field = "Código", displayName = "Código", visible = true},
                new ColumnDefinition() {field = "Descripcion", displayName = "Descripción", visible = true},
                new ColumnDefinition() {field = "Descripcion2", displayName = "Descripción Idioma", visible = true},
                new ColumnDefinition() {field = "Clasificacion", displayName = "Clasificación", visible = true}
            };
        }

        public string GetPrimaryKey()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ViewProperty> getProperties()
        {
            var listNames = GetType().GetProperties().Select(f => f.Name).Except(typeof(IModelView).GetProperties().Select(h => h.Name)).Except(typeof(ICanDisplayName).GetProperties().Select(f => f.Name)).Except(typeof(IModelViewExtension).GetProperties().Select(h => h.Name));
            var properties = GetType().GetProperties().Where(f => listNames.Any(h => h == f.Name));

            return properties.Select(item => new ViewProperty
            {
                property = item,
                attributes = item.GetCustomAttributes(true)
            }).ToList();
        }

        public void set(string propertyName, object value)
        {
            GetType().GetProperty(propertyName).SetValue(this, value);
        }

        public bool ValidateModel(IEnumerable<object> containerCollection)
        {
            if (string.IsNullOrEmpty(Valor))
                throw new ValidationException("El campo Valor no puede estar vacío");

            if (string.IsNullOrEmpty(Descripcion))
                throw new ValidationException("El campo Descripción no puede estar vacío");


            if (containerCollection.Count(f => ((TablasVariasPropAdeudoModel)f).Valor == Valor) > 1)
            {
                throw new ValidationException("El valor " + Valor + " está duplicado");
            }

            foreach (var item in containerCollection)
            {
                ((TablasVariasPropAdeudoModel)item).Clasificacion =
                    ((TablasVariasPropAdeudoModel)item).Clasificacion.ToUpper();
            }

            return true;
        }
    }
}
