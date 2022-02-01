using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.ResourcesGlobalization.Textos.Entidades;
using ValidationException = Marfil.Dom.Persistencia.Helpers.ValidationException;
using RTablas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.TablasVarias;
namespace Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados
{
    public class TablasVariasGeneralModel : IModelView, ICanValidate
    {
        [XmlIgnore]
        public IContextService Context { get; set; }

        #region CTR

        public TablasVariasGeneralModel()
        {

        }

        #endregion

        #region Properties

        [Key]
        [Required]
        [Display(ResourceType = typeof(RTablas), Name = "Valor")]
        [MaxLength(3)]
        public string Valor { get; set; }

        /*[Display(ResourceType = typeof(RTablas), Name = "Codigo")]
        public string Codigo { get; set; }*/

        [Required]
        [CustomDisplayDescription(typeof(RTablas), "Descripcion", CustomDisplayDescriptionAttribute.TipoDescripcion.Principal)]
        public string Descripcion { get; set; }
        
        [CustomDisplayDescription(typeof(RTablas), "Descripcion", CustomDisplayDescriptionAttribute.TipoDescripcion.Secundaria)]
        public string Descripcion2 { get; set; }

        #endregion

        #region Base

        public IEnumerable<ViewProperty> getProperties()
        {
            var listNames =
                GetType()
                    .GetProperties()
                    .Select(f => f.Name)
                    .Except(typeof (IModelView).GetProperties().Select(h => h.Name))
                    .Except(typeof (ICanDisplayName).GetProperties().Select(f => f.Name))
                    .Except(typeof (IModelViewExtension).GetProperties().Select(h => h.Name));
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


            if (containerCollection.Count(f => ((TablasVariasGeneralModel) f).Valor == Valor) > 1)
            {
                throw new ValidationException("El valor " + Valor + " está duplicado");
            }

            return true;
        }

        #endregion
    }
}
