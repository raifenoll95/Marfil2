using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RGuiasBalances = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.GuiasBalances;
using RGuiasBLineas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.GuiasBalancesLineas;


namespace Marfil.Dom.Persistencia.Model.Contabilidad
{
    public class GuiasBalancesLineasModel
    {
        public enum TipoInformeE
        {
            [StringValue(typeof(RGuiasBalances), "Balca")]
            Balca,
            [StringValue(typeof(RGuiasBalances), "CTAPG")]
            Ctapg,
            [StringValue(typeof(RGuiasBalances), "CPGAN")]
            Cpgan,
            [StringValue(typeof(RGuiasBalances), "CPGFU")]
            Cpgfu
        }

        public enum TipoGuiaE
        {
            [StringValue(typeof(RGuiasBalances), "TipoGuia1")]
            Abreviada,
            [StringValue(typeof(RGuiasBalances), "TipoGuia2")]
            Abreviado,
            [StringValue(typeof(RGuiasBalances), "TipoGuia3")]
            COOP_ABREVIA,
            [StringValue(typeof(RGuiasBalances), "TipoGuia4")]
            COOP_NORMAL,
            [StringValue(typeof(RGuiasBalances), "TipoGuia5")]
            NORMAL,
            [StringValue(typeof(RGuiasBalances), "TipoGuia6")]
            PYME,
            [StringValue(typeof(RGuiasBalances), "TipoGuia7")]
            ESTANDAR,
            [StringValue(typeof(RGuiasBalances), "TipoGuia8")]
            INFGESTI
        }

        #region Propiedades
        [Display(ResourceType = typeof(RGuiasBLineas), Name = "Id")]
        public int Id { get; set; }
        
        [Display(ResourceType = typeof(RGuiasBLineas), Name = "Informe")]
        public TipoInformeE InformeId { get; set; }

        [Display(ResourceType = typeof(RGuiasBLineas), Name = "Guia")]
        public TipoGuiaE GuiaId { get; set; }

        [Display(ResourceType = typeof(RGuiasBLineas), Name = "GuiasBalancesId")]
        public int GuiasBalancesId { get; set; }

        [Display(ResourceType = typeof(RGuiasBLineas), Name = "Orden")]
        public string Orden { get; set; }

        [Display(ResourceType = typeof(RGuiasBLineas), Name = "Cuenta")]
        public string Cuenta { get; set; }

        [Display(ResourceType = typeof(RGuiasBLineas), Name = "Signo")]
        public string Signo { get; set; }

        [Display(ResourceType = typeof(RGuiasBLineas), Name = "Signoea")]
        public string Signoea { get; set; }
        #endregion
    }

    public class GuiasBalancesModel : BaseModel<GuiasBalancesModel, GuiasBalances>
    {
        public enum TipoInformeE
        {
            [StringValue(typeof(RGuiasBalances), "Balca")]
            Balca,
            [StringValue(typeof(RGuiasBalances), "CTAPG")]
            Ctapg,
            [StringValue(typeof(RGuiasBalances), "CPGAN")]
            Cpgan,
            [StringValue(typeof(RGuiasBalances), "CPGFU")]
            Cpgfu
        }

        public enum TipoGuiaE
        {
            [StringValue(typeof(RGuiasBalances), "TipoGuia1")]
            Abreviada,
            [StringValue(typeof(RGuiasBalances), "TipoGuia2")]
            Abreviado,
            [StringValue(typeof(RGuiasBalances), "TipoGuia3")]
            COOP_ABREVIA,
            [StringValue(typeof(RGuiasBalances), "TipoGuia4")]
            COOP_NORMAL,
            [StringValue(typeof(RGuiasBalances), "TipoGuia5")]
            NORMAL,
            [StringValue(typeof(RGuiasBalances), "TipoGuia6")]
            PYME,
            [StringValue(typeof(RGuiasBalances), "TipoGuia7")]
            ESTANDAR,
            [StringValue(typeof(RGuiasBalances), "TipoGuia8")]
            INFGESTI
        }

        public GuiasBalancesModel()
        {

        }
        public GuiasBalancesModel(IContextService context) : base(context)
        {
        }

        private List<GuiasBalancesLineasModel> _lineas = new List<GuiasBalancesLineasModel>();

        public override string DisplayName => "Guias Balances";

        public int Id { get; set; }

        [Display(ResourceType = typeof(RGuiasBalances), Name = "Informe")]

        public TipoInformeE InformeId { get; set; }

        [Display(ResourceType = typeof(RGuiasBalances), Name = "Guia")]
        public TipoGuiaE GuiaId { get; set; }

        /*[Display(ResourceType = typeof(RGuiasBalances), Name = "Informe")]
        public TipoInforme TipoInforme { get; set; }

        [Display(ResourceType = typeof(RGuiasBalances), Name = "Guia")]
        public TipoGuia TipoGuia { get; set; }

        [Display(ResourceType = typeof(RGuiasBalances), Name = "Informe")]
        public TipoInformeE TipoInformeE { get; set; }

        [Display(ResourceType = typeof(RGuiasBalances), Name = "Guia")]
        public TipoGuiaE TipoGuiaE { get; set; }*/

        [Display(ResourceType = typeof(RGuiasBalances), Name = "TextoGrupo")]
        public string TextoGrupo { get; set; }

        [Display(ResourceType = typeof(RGuiasBalances), Name = "Orden")]
        public string Orden { get; set; }

        [Display(ResourceType = typeof(RGuiasBalances), Name = "Actpas")]
        public string Actpas { get; set; }

        [Display(ResourceType = typeof(RGuiasBalances), Name = "Detfor")]
        public string Detfor { get; set; }

        [Display(ResourceType = typeof(RGuiasBalances), Name = "Formula")]
        public string Formula { get; set; }

        [Display(ResourceType = typeof(RGuiasBalances), Name = "RegDig")]
        public string RegDig { get; set; }

        [Display(ResourceType = typeof(RGuiasBalances), Name = "Descrip")]
        public string Descrip { get; set; }

        [Display(ResourceType = typeof(RGuiasBalances), Name = "Listado")]
        public string Listado { get; set; }

        public List<GuiasBalancesLineasModel> Lineas
        {
            get { return _lineas; }
            set { _lineas = value; }
        }

        public override object generateId(string id)
        {
            return int.Parse(id);
        }
    }
}
