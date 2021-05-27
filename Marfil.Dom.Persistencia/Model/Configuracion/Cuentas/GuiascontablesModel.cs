using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Marfil.Dom.ControlsUI.Bloqueo;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;
using RGuiascontables = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Guiascontables;
namespace Marfil.Dom.Persistencia.Model.Configuracion.Cuentas
{
    [Serializable]
    public class GuiascontablesLinModel
    {
        #region Properties
        [Required]
        public string Empresa { get; set; }

        [Display(ResourceType = typeof(RGuiascontables), Name = "Id")]
        [Required]
        public string Fkguiascontables { get; set; }

        [Required]
        public int Id { get; set; }
        [Required]
        [Display(ResourceType = typeof(RGuiascontables), Name = "Fkregimeniva")]
        public string Fkregimeniva { get; set; }

        [Display(ResourceType = typeof(RGuiascontables), Name = "Fkcuentascompras")]
        public string Fkcuentascompras { get; set; }

        [Display(ResourceType = typeof(RGuiascontables), Name = "Fkcuentasventas")]
        public string Fkcuentasventas { get; set; }

        [Display(ResourceType = typeof(RGuiascontables), Name = "Fkcuentasdevolucioncompras")]
        public string Fkcuentasdevolucioncompras { get; set; }

        [Display(ResourceType = typeof(RGuiascontables), Name = "Fkcuentasdevolucionventas")]
        public string Fkcuentasdevolucionventas { get; set; }
        
        #endregion

        public GuiascontablesLinModel()
        {
            Id = 0;
        }
    }

    public class GuiascontablesModel : BaseModel<GuiascontablesModel, Guiascontables>
    {
       

        #region Properties

        [Required]
        public string Empresa { get; set; }

        [MaxLength(12,ErrorMessageResourceType = typeof(Unobtrusive),ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RGuiascontables),Name="Id")]
        [Required]
        public string Id { get; set; }

        [Required]
        [MaxLength(40, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RGuiascontables), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(RGuiascontables), Name = "Defecto")]
        public bool Defecto { set; get; }

        private List<GuiascontablesLinModel> _lineas = new List<GuiascontablesLinModel>();
        public List<GuiascontablesLinModel> Lineas
        {
            get { return _lineas; }
            set { _lineas = value.ToList(); }
        }

        [Display(ResourceType = typeof(RGuiascontables), Name = "Fkcuentascompras")]
        public string Fkcuentascompras { get; set; }

        [Display(ResourceType = typeof(RGuiascontables), Name = "Fkcuentasventas")]
        public string Fkcuentasventas { get; set; }

        [Display(ResourceType = typeof(RGuiascontables), Name = "Fkcuentasdevolucioncompras")]
        public string Fkcuentasdevolucioncompras { get; set; }

        [Display(ResourceType = typeof(RGuiascontables), Name = "Fkcuentasdevolucionventas")]
        public string Fkcuentasdevolucionventas { get; set; }

        #endregion

        #region CTR

        public GuiascontablesModel()
        {
           
        }

        public GuiascontablesModel(IContextService context) : base(context)
        {
           
        }

        #endregion

        public override string DisplayName => RGuiascontables.TituloEntidad;

        public override object generateId(string id)
        {
            return id;
        }
    }
}
