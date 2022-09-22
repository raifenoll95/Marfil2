using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.Model
{
    public class LecturasModel : BaseModel<LecturasModel, Lecturas>
    {

        #region CTR

        public LecturasModel()
        {

        }

        public LecturasModel(IContextService context) : base(context)
        {

        }

        #endregion

        public string Identificador { get; set; }

        public DateTime Fecha { get; set; }

        public string Hora { get; set; }

        public string Lote { get; set; }

        public int Cantidad { get; set; }

        public Guid Usuario { get; set; }

        public bool Insertado { get; set; }

        public override string DisplayName => "Lecturas";

        public override object generateId(string id)
        {
            return Identificador;
        }
    }

    public class LecturasAsistenteModel : LecturasModel
    {
        public string Fechaformat { 
            get { return Fecha.ToString("dd/MM/yyyy"); }
        }
        public int Numregistros { get; set; }

    }
}
