using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.Model
{
    public class MenuItemJavascriptModel
    {
        private string _icono="";

        public string icono
        {
            get { return _icono; }
            set { _icono = value; }
        }
        public bool isseparator { get; set; }
        public string text { get; set; }
        public string link { get; set; }
        public IEnumerable<MenuItemJavascriptModel> items { get; set; }
        public string target { get; set; }
    }
    public class MenuItemsAplicacionModel
    {
        #region Members

        private bool _allowDelete = true;
        private bool _allowCreate =  true;
        private bool _allowUpdate = true;
        private bool _allowBlock = true;

        #endregion

        #region Properties
        public bool isSeparator { get; set; }
        public string name { get; set; }
        public string texto { get; set; }
        public string url { get; set; }
        public string icono { get; set; }
        public string target { get; set; }
        public bool AllowDelete
        {
            get { return _allowDelete; }
            set { _allowDelete = value; }
        }
        public bool AllowCreate
        {
            get { return _allowCreate; }
            set { _allowCreate = value; }
        }
        public bool AllowUpdate
        {
            get { return _allowUpdate; }
            set { _allowUpdate = value; }
        }

        public bool AllowBlock
        {
            get { return _allowBlock; }
            set { _allowBlock = value; }
        }

        public IEnumerable<MenuItemsAplicacionModel> items { get; set; }

        #endregion

    }
}
