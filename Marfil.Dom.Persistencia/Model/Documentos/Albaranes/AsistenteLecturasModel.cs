using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RAlbaranes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Albaranes;

namespace Marfil.Dom.Persistencia.Model.Documentos.Albaranes
{
    public class ToolbarAsistenteLecturasModel : ToolbarModel
    {
        public ToolbarAsistenteLecturasModel()
        {
            Operacion = TipoOperacion.Custom;
            Titulo = RAlbaranes.GenerarLecturas;
        }

        public override string GetCustomTexto()
        {
            return RAlbaranes.AsistenteLecturas;
        }
    }
    public class AsistenteLecturasModel : IToolbar
    {
        #region Members

        private ToolbarModel _toolbar;

        #endregion

        public ToolbarModel Toolbar
        {
            get { return _toolbar; }
            set { _toolbar = value; }
        }

        public AsistenteLecturasModel()
        {

        }
        public AsistenteLecturasModel(IContextService context)
        {
            _toolbar = new ToolbarAsistenteLecturasModel();
        }
    }
}
