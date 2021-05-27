using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.Helpers
{
    public class ToolbarExportar : ToolbarModel
    {
        public ToolbarExportar()
        {
            Operacion = TipoOperacion.Custom;
            Titulo = General.LblExportarClassic;
        }

        public override string GetCustomTexto()
        {
            return General.LblExportarClassic;
        }
    }

    public class Exportar: IToolbar
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

        #endregion

        public Exportar()
        {
            _toolbar = new ToolbarExportar();
        }

    }
}
