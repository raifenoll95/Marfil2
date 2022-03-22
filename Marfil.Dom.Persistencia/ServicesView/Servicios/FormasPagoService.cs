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
    public interface IFormasPagoService
    {

    }

    public class FormasPagoService : GestionService<FormasPagoModel, FormasPago>, IFormasPagoService
    {
        #region CTR

        public FormasPagoService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            model.ExcludedColumns = new[]
            {
               "Bloqueado","BloqueoModel", "Nombre2", "ImprimirVencimientoFacturas", "RecargoFinanciero", "Efectivo", "Remesable", "Mandato",
                "ExcluirFestivos", "ModoPago", "ListModosPago", "Lineas", "NumeroVencimientos","Gruposformaspago", "Docsventaimprimircuenta","Toolbar"
            };
            model.BloqueoColumn = "Bloqueado";
            using (var service = new TablasVariasService(_context,_db))
            {
                model.ColumnasCombo.Add("FkGruposformaspago", service.GetListGruposFormasPago().Select(f => new Tuple<string, string>(f.Valor, f.Descripcion)));
            }
            

            return model;
        }

        public int NextId()
        {
            return _db.FormasPago.Any() ? _db.FormasPago.Max(f => f.id) + 1 : 1;
        }

        public void Bloquear(string id, string motivo, string user, bool operacion)
        {
            var intId = Funciones.Qint(id).Value;
            var formapago = _db.FormasPago.Single(f => f.id == intId);
            formapago.fkMotivosbloqueo = motivo;
            formapago.fkUsuariobloqueo = new Guid(user);
            formapago.fechamodificacionbloqueo = DateTime.Now;
            formapago.bloqueada = operacion;
            _db.Entry(formapago).State = EntityState.Modified;
            _db.SaveChanges();
        }
    }
}
