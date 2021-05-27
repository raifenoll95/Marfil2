using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Resources;

namespace Marfil.Dom.ControlsUI.Toolbar
{
    public enum TipoOperacion
    {
        Index,
        Alta,
        Baja,
        Editar,
        Ver,
        Custom
    }

    public interface IToolbaritem
    {
        
    }

    public interface IToolbarSeparatorModel : IToolbaritem
    {
        
    }

    public interface IToolbarComboModel : IToolbaritem
    {
        IEnumerable<ToolbarActionModel> Items { get; set; }
    }

    public interface IToolbarActionModel : IToolbaritem
    {
        string Texto { get; set; }
        string Icono { get; set; }
        string Url { get; set; }
        bool Desactivado { get; set; }
        bool OcultarTextoSiempre { get; set; }
        string Target { get; set; }
    }

    public class ToolbarSeparatorModel: IToolbarSeparatorModel
    {
        
    }

    public class ToolbarActionComboModel : ToolbarActionModel, IToolbarComboModel
    {
        public IEnumerable<ToolbarActionModel> Items { get; set; }
    }

    public class ToolbarActionModel: IToolbarActionModel
    {
        public string Texto { get; set; }
        public string Icono { get; set; }
        public string Url { get; set; }
        public bool Desactivado { get; set; }
        public bool OcultarTextoSiempre { get; set; }
        public string Target { get; set; }
    }

    public class ToolbarModel
    {
        private List<IToolbaritem> _acciones= new List<IToolbaritem>();
        private bool _customAction = false;
        public string Titulo { get; set; }
        public string OperacionTexto { get { return GetOperacionTexto(Operacion); } }
        public TipoOperacion Operacion { get; set; }

        public bool CustomAction
        {
            get { return _customAction; }
            set { _customAction = value; }
        }

        public string CustomActionName { get; set; }

        public IEnumerable<IToolbaritem> Acciones
        {
            get { return _acciones; }
            set { _acciones = value.ToList(); }
        }

        private string GetOperacionTexto(TipoOperacion operacion)
        {
            switch (operacion)
            {
                case TipoOperacion.Alta:
                    return General.LblCrear;
                case TipoOperacion.Baja:
                    return General.LblBorrar;
                case TipoOperacion.Editar:
                    return General.LblEditar;
                case TipoOperacion.Index:
                    return General.LblInicio;
                case TipoOperacion.Ver:
                    return General.LblVer;
                case TipoOperacion.Custom:
                    return GetCustomTexto();
                default:
                    return string.Empty;
            }
        }

        public virtual string GetCustomTexto()
        {
            return string.Empty;
        }

       
    }

    

  
}
