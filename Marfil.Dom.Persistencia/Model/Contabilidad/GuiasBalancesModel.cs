using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RGuiasBalances = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.GuiasBalances;

namespace Marfil.Dom.Persistencia.Model.Contabilidad
{
    public enum TipoInformeE
    {
        [StringValue(typeof(RGuiasBalances), "Balca")]
        Balca = 1,
        [StringValue(typeof(RGuiasBalances), "CTAPG")]
        Ctapg = 2,
    }
    public enum TipoGuiaE
    {
        [StringValue(typeof(RGuiasBalances), "TipoGuia1")]
        Abreviada = 1,
        [StringValue(typeof(RGuiasBalances), "TipoGuia2")]
        Abreviado = 2,
        [StringValue(typeof(RGuiasBalances), "TipoGuia3")]
        COOP_ABREVIA = 3,
        [StringValue(typeof(RGuiasBalances), "TipoGuia4")]
        COOP_NORMAL = 4,
        [StringValue(typeof(RGuiasBalances), "TipoGuia5")]
        NORMAL = 5,
        [StringValue(typeof(RGuiasBalances), "TipoGuia6")]
        PYME = 6
    }

    //http://localhost:55459/GuiasBalances/Index
    public class GuiasBalancesModel : BaseModel<GuiasBalancesModel, GuiasBalances>
    {
        public override string DisplayName => "Guias Contables";

        public int Id { get; set; }

        [Display(ResourceType = typeof(RGuiasBalances), Name = "Informe")]

        public int InformeId { get; set; }

        [Display(ResourceType = typeof(RGuiasBalances), Name = "Guia")]
        public int GuiaId { get; set; }

        [Display(ResourceType = typeof(RGuiasBalances), Name = "Informe")]
        public TipoInforme TipoInforme { get; set; }

        [Display(ResourceType = typeof(RGuiasBalances), Name = "Guia")]
        public TipoGuia TipoGuia { get; set; }

        [Display(ResourceType = typeof(RGuiasBalances), Name = "Informe")]
        public TipoInformeE TipoInformeE { get; set; }

        [Display(ResourceType = typeof(RGuiasBalances), Name = "Guia")]
        public TipoGuiaE TipoGuiaE { get; set; }

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

        private List<GuiasBalancesLineasModel> _GuiasBalancesLineas;

        public List<GuiasBalancesLineasModel> GuiasBalancesLineas
        {
            get { return _GuiasBalancesLineas; }
            set { _GuiasBalancesLineas = value; }
        }

        private List<string> listFormula;
        public List<string> ListFormula
        {

            get { return listFormula; }
            set { listFormula = value; }
        }

        private List<string> listActpas;
        public List<string> ListActpas
        {
            get { return listActpas; }
            set { listActpas = value; }
        }

        public GuiasBalancesModel()
        {

        }
        public GuiasBalancesModel(IContextService context) : base(context)
        {
            GuiasBalancesLineas = new List<GuiasBalancesLineasModel>();
            ListFormula = new List<string>();
            ListActpas = new List<string>();

            ListActpas.Add("A");
            ListActpas.Add("P");

            ListFormula.Add("D");
            ListFormula.Add("T");
            ListFormula.Add("F");
            ListFormula.Add("G");
        }
        public override object generateId(string id)
        {
            return int.Parse(id);
        }
    }
}
