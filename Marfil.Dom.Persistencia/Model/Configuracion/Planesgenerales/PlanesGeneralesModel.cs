using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.Dom.Persistencia.Model.Configuracion.Planesgenerales
{
    public class PlanesGeneralesModel : BaseModel<PlanesGeneralesModel, Persistencia.Planesgenerales>
    {
        #region Properties

        public Guid Id { get; set; }

        public string Nombre { get; set; }

        public string Fichero { get; set; }

        public bool Defecto { get; set; }

        #endregion

        #region CTR

        public PlanesGeneralesModel()
        {
            Id = Guid.NewGuid();
        }

        public PlanesGeneralesModel(IContextService context) : base(context)
        {
            Id = Guid.NewGuid();
        }

        #endregion



        public override object generateId(string id)
        {
            return new Guid(id);
        }

        public override string DisplayName => "Planes generales";
    }
}
