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

namespace Marfil.Dom.Persistencia.Model
{
    
    public class ImportarModel
    {
        #region Properties

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

        #endregion

        #region CTR

        public ImportarModel()
        {
            
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
