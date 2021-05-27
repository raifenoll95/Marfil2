using System;
using System.ComponentModel.DataAnnotations;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Model.Interfaces;
using RPedidosCompras=Marfil.Inf.ResourcesGlobalization.Textos.Entidades.PedidosCompras;
namespace Marfil.Dom.Persistencia.Model.Documentos.PedidosCompras
{
    public class ToolbarAsistentePedidosComprasModel: ToolbarModel
    {
        public ToolbarAsistentePedidosComprasModel()
        {
            Operacion = TipoOperacion.Custom;
            Titulo = RPedidosCompras.Generarpedidoscompras;
        }

        public override string GetCustomTexto()
        {
            return RPedidosCompras.AsistentePedidosCompras;
        }
    }

    public class AsistentePedidosComprasModel: IToolbar
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

        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.Clientes), Name = "TituloEntidadSingular")]
        public string Fkclientes { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Fkpedidoinicio")]
        public string Fkpedidoinicio { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Fpedidofin")]
        public string Fkpedidofin { get; set; }

        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.Series), Name = "TituloEntidadSingular")]
        public string Fkseriepedido { get; set; }

        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.AlbaranesCompras), Name = "Fkproveedores")]
        public string Fkproveedores { get; set; }

        [Required]
        [Display(ResourceType = typeof(RPedidosCompras), Name = "Fechadocumento")]
        public DateTime Fechapedido { get; set; }

        #endregion

        public AsistentePedidosComprasModel()
        {
            _toolbar = new ToolbarAsistentePedidosComprasModel();
        }
    }
}
