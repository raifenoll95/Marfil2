using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{

    public class SituacionesTesoreriaService : GestionService<SituacionesTesoreriaModel, SituacionesTesoreria>
    {
        #region CTR

        public SituacionesTesoreriaService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var propiedadesVisibles = new[] { "Cod", "Descripcion", "Descripcion2", "Valorinicialcobros", "Valorinicialpagos", "Prevision" };
            var propiedades = Helpers.Helper.getProperties<SituacionesTesoreriaModel>();
            model.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            model.PrimaryColumnns = new[] { "Cod" };
            return model;
        }

        public string getCobro()
        {
            string cobro = "";

            if(_db.SituacionesTesoreria.Any(f => f.valorinicialcobros == true))
            {
                cobro = "cobro";
            }

            return cobro;
        }

        public string getPago()
        {
            string pago = "";

            if (_db.SituacionesTesoreria.Any(f => f.valorinicialpagos == true))
            {
                pago = "pago";
            }

            return pago;
        }

        public IEnumerable<SituacionesTesoreriaModel> GetListSituaciones()
        {
            var situacionesDB = _db.SituacionesTesoreria.ToList();
            List<SituacionesTesoreriaModel> situaciones = new List<SituacionesTesoreriaModel>();

            foreach(var situacion in situacionesDB)
            {
                situaciones.Add(new SituacionesTesoreriaModel
                {
                    Cod = situacion.cod,
                    Descripcion = situacion.descripcion,
                    Descripcion2 = situacion.descripcion2,
                    Valorinicialcobros = situacion.valorinicialcobros.Value,
                    Valorinicialpagos = situacion.valorinicialpagos.Value,
                    Prevision = (TipoPrevision)situacion.prevision,
                    Editable = situacion.editable.Value,
                    Remesable = situacion.remesable.Value,
                    Riesgo = (TipoRiesgo)situacion.riesgo
                });
            }

            return situaciones;
        }

        public SituacionesTesoreriaModel getSituacion(string circuito)
        {
            var situacionfinal = _db.CircuitosTesoreriaCobros.Where(f => f.empresa == Empresa && f.id.ToString() == circuito).
                Select(f => f.situacionfinal).SingleOrDefault().ToString();

            return get(situacionfinal) as SituacionesTesoreriaModel;
        }
    }
}
