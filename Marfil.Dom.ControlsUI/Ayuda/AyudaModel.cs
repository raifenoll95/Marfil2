using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.ControlsUI.Ayuda
{
    [Serializable]
    public class AyudaModel
    {
        #region Member

        private List<AyudaItemModel> _list = new List<AyudaItemModel>();

        #endregion

        #region PRoperties

        public List<AyudaItemModel> List
        {
            get { return _list; }
            set { _list = value; }
        }

        #endregion
    }

    [Serializable]
    public class AyudaItemModel
    {

        private string _currentError=string.Empty;
        [Required]
        public string Controller { get; set; }
        public string Action { get; set; }
        [Required]
        public string Url { get; set; }

        
    }
}
