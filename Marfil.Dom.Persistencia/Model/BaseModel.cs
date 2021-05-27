using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Marfil.Dom.ControlsUI.CampoVerificacion;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using static Marfil.Dom.Persistencia.Helpers.Helper;

namespace Marfil.Dom.Persistencia.Model
{
    [Serializable]
    public abstract class BaseModel<T, T2> : IModelView, IModelViewExtension, ICanDisplayName, IToolbar
    {
        private ToolbarModel _toolbar;

        #region Properties

        [XmlIgnore]
        [IgnoreDataMember]
        public IContextService Context { get; set; }

        [XmlIgnore]
        public virtual IEnumerable<PropertyInfo> primaryKey { get; protected set; }

        [XmlIgnore]
        public Type persistencyType
        {
            get { return typeof(T2); }
        }

        
        public abstract object generateId(string id);

        [XmlIgnore]
        public ToolbarModel Toolbar
        {
            get { return _toolbar; }
            set { _toolbar = value; }
        }

        #endregion

        #region CTR

        public BaseModel()
        {
            createNewPrimaryKey();

            _toolbar = new ToolbarModel()
            {
                Titulo = DisplayName
            };
        } 

        public BaseModel(IContextService context):this()
        {
            Context = context;
            
        }

        #endregion

        #region Funciones base
        
        public IEnumerable<ViewProperty> getProperties()
        {
            return getProperties<T>();
        }

        public object get(string propertyName)
        {
            var property = GetType().GetProperty(propertyName) ?? GetType().GetProperty(FirstCharToUpper(propertyName));
            return property?.GetValue(this, null);
        }

        public void set(string propertyName, object value)
        {
            var property = GetType().GetProperty(propertyName) ?? GetType().GetProperty(FirstCharToUpper(propertyName));
            property?.SetValue(this, value);
        }

        public virtual void createNewPrimaryKey()
        {
            primaryKey = getProperties().Where(f => f.property.Name.ToLower() == "id").ToList().Select(f => f.property);
        }
        
        public virtual string GetPrimaryKey()
        {
            return get(primaryKey.First().Name)?.ToString();
        }

        internal virtual CampoverificacionModel GetCampoverificacionModel()
        {
            return null;
        }

        #endregion

        [XmlIgnore]
        public abstract string DisplayName { get; }
    }
}
