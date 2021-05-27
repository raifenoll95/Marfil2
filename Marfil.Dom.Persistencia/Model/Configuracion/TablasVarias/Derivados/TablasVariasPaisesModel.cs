using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Marfil.Dom.ControlsUI.Busquedas;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;
using ValidationException= Marfil.Dom.Persistencia.Helpers.ValidationException;
namespace Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados
{
    [Serializable]
    public class TablasVariasPaisesModel :  IModelView, IGetColumnasGrid, ICanValidate
    {
        #region Properties
        [Key]
        [Required]
        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.TablasVarias), Name = "Valor")]
        [MaxLength(3)]
        public string Valor { get; set; }

        [Required]
        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.TablasVarias), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.TablasVarias), Name = "Descripcion2")]
        public string Descripcion2 { get; set; }

        [Required]
        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.TablasVarias), Name = "CodigoIsoAlfa2")]
        [MaxLength(2, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [MinLength(2, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MinLength")]
        public string CodigoIsoAlfa2 { get; set; }

        [Required]
        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.TablasVarias), Name = "CodigoIsoAlfa3")]
        [MaxLength(3, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [MinLength(3, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MinLength")]
        public string CodigoIsoAlfa3 { get; set; }

        [Required]
        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.TablasVarias), Name = "CodigoIsoNumerico")]
        [MaxLength(3, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [MinLength(3, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MinLength")]
        public string CodigoIsoNumerico { get; set; }

        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.TablasVarias), Name = "NifEuropeo")]
        [MaxLength(2, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        public string NifEuropeo { get; set; }

        #endregion

        #region Base

        public object generateId(string id)
        {
            return id;
        }

        public IEnumerable<ColumnDefinition> GetColumnDefinitions()
        {
            return new[]
            {
                new ColumnDefinition() {field = "Valor", displayName = "Código", visible = false},
                new ColumnDefinition() {field = "Descripcion", displayName = "Descripción", visible = true},
                new ColumnDefinition() {field = "Descripcion2", displayName = "Descripción (2)", visible = true},
                new ColumnDefinition() {field = "CodigoIsoAlfa2", displayName = "Cod. ISO Alfa 2", visible = true},
                new ColumnDefinition() {field = "CodigoIsoAlfa3", displayName = "Cod. ISO Alfa 3", visible = true},
                new ColumnDefinition() {field = "CodigoIsoNumerico", displayName = "Cod. Númerico", visible = true}
            };
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

        public object get(string propertyName)
        {
            return GetType().GetProperty(propertyName).GetValue(this);
        }

        public void set(string propertyName, object value)
        {
            GetType().GetProperty(propertyName).SetValue(this, value);
        }

        public void createNewPrimaryKey()
        {
            throw new NotImplementedException();
        }

        public string GetPrimaryKey()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Validation

        public bool ValidateModel(IEnumerable<object> containerCollection)
        {
            if (string.IsNullOrEmpty(Valor))
                throw new ValidationException("El campo Valor no puede estar vacío");

            if (string.IsNullOrEmpty(Descripcion))
                throw new ValidationException("El campo Descripción no puede estar vacío");


            if (containerCollection.Count(f => ((TablasVariasPaisesModel)f).Valor == Valor) > 1)
            {
                throw new ValidationException("El valor " + Valor + " está duplicado");
            }

            if (containerCollection.Count(f => ((TablasVariasPaisesModel)f).CodigoIsoAlfa2 == CodigoIsoAlfa2) > 1)
            {
                throw new ValidationException("El Codigo Iso Alfa2 " + CodigoIsoAlfa2 + " está duplicado");
            }

            if (containerCollection.Count(f => ((TablasVariasPaisesModel)f).CodigoIsoAlfa3 == CodigoIsoAlfa3) > 1)
            {
                throw new ValidationException("El Codigo Iso Alfa3 " + Valor + " está duplicado");
            }

            if (containerCollection.Count(f => ((TablasVariasPaisesModel)f).CodigoIsoNumerico == CodigoIsoNumerico) > 1)
            {
                throw new ValidationException("El Codigo Iso Numérico " + Valor + " está duplicado");
            }

            foreach (var item in containerCollection)
            {
                ((TablasVariasPaisesModel) item).CodigoIsoAlfa2 =
                    ((TablasVariasPaisesModel) item).CodigoIsoAlfa2.ToUpper();
                ((TablasVariasPaisesModel)item).CodigoIsoAlfa3 =
                    ((TablasVariasPaisesModel)item).CodigoIsoAlfa3.ToUpper();
                if(!string.IsNullOrEmpty(((TablasVariasPaisesModel)item).NifEuropeo))
                ((TablasVariasPaisesModel)item).NifEuropeo =
                    ((TablasVariasPaisesModel)item).NifEuropeo.ToUpper();
            }

            return true;
        }

        #endregion

        
    }
}
