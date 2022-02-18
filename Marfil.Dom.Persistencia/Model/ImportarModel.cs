using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;
using Marfil.Inf.Genericos;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Dom.ControlsUI.Toolbar;

namespace Marfil.Dom.Persistencia.Model
{
    public class ToolbarImportarModel : ToolbarModel
    {
        public ToolbarImportarModel()
        {
            Operacion = TipoOperacion.Custom;
            Titulo = General.LblImportarClassic;
        }

        public override string GetCustomTexto()
        {
            return General.LblImportar2;
        }
    }
    public class ImportarModel : IToolbar
    {
        #region Members

        private ToolbarModel _toolbar;

        #endregion

        #region Properties

        public ToolbarModel Toolbar
        {
            get { return _toolbar; }
            set { _toolbar = value; }
        }

        [Display(ResourceType = typeof(General), Name = "LblFichero")]
        public HttpPostedFileBase Fichero { get; set; }

        [Display(ResourceType = typeof(General), Name = "LblDelimitador")]
        public string Delimitador { get; set; }

        [Display(ResourceType = typeof(General), Name = "LblCabecera")]
        public bool Cabecera { get; set; }

        [Display(ResourceType = typeof(General), Name = "LblSerie")]
        public IEnumerable<SelectListItem> Serie { get; set; }

        public string SelectedId { get; set; }

        [Display(ResourceType = typeof(General), Name = "LblTipoAlmacenLote")]
        public IEnumerable<SelectListItem> TipoLote { get; set; }

        public string SelectedIdTipoAlmacenLote { get; set; }

        [Display(ResourceType = typeof(General), Name = "LblAlbaranImportar")]
        public string Albaran { get; set; }

        [Display(ResourceType = typeof(General), Name = "LblTarifa1")]
        public string Tarifa1 { get; set; }

        [Display(ResourceType = typeof(General), Name = "LblTarifa2")]
        public string Tarifa2 { get; set; }

        [Display(ResourceType = typeof(General), Name = "LblTarifa3")]
        public string Tarifa3 { get; set; }

        [Display(ResourceType = typeof(General), Name = "LblFamilia")]
        public string Familia { get; set; }
        #endregion

        #region CTR

        public ImportarModel()
        {
            _toolbar = new ToolbarImportarModel();
        }

        //public PeticionesAsincronasModel(IContextService context) : base(context)
        //{

        //}

        #endregion

        //public override object generateId(string id)
        //{
        //    return new Guid(id);
        //}

        //public override void createNewPrimaryKey()
        //{
        //    primaryKey = getProperties().Where(f => f.property.Name == "Id").Select(f => f.property);
        //}

        //public override string DisplayName => RPeticiones.TituloEntidad;

    }
}
