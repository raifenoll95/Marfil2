using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.ControlsUI.Toolbar;

namespace Marfil.Dom.Persistencia.Model.Interfaces
{
    public interface IModelSearch
    {
        IEnumerable<ViewProperty> getProperties();
    }

    public struct ViewProperty
    {
        public PropertyInfo property { get; set; }
        public object[] attributes { get; set; }
    }

   public interface IModelView
    {
        //object Fkempresa { get; }

        IEnumerable<ViewProperty> getProperties();
        object get(string propertyName);
        void set(string propertyName, object value);
        void createNewPrimaryKey();
        string GetPrimaryKey();

    }

    public interface IModelViewExtension
    {
        [IgnoreDataMember]
        IEnumerable<PropertyInfo> primaryKey { get; }
        [IgnoreDataMember]
        Type persistencyType { get; }
        object generateId(string id);

    }

    public interface IToolbar
    {
        ToolbarModel Toolbar { get; set; }
    }
}
