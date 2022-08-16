using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Inf.Genericos.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public class RegistroIvaRepercutidoService : GestionService<RegistroIvaRepercutidoModel, RegistroIVARepercutido>
    {

        #region CONSTRUCTOR
        public RegistroIvaRepercutidoService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }
        #endregion

        #region List index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var propiedadesVisibles = new[] { "Origendoc", "Id", "Fechafactura", "Periodo", "Referencia", "Tipofactura", "Cuentacliente", "Nombre Cliente" };
            var propiedades = Helpers.Helper.getProperties<RegistroIvaRepercutidoModel>();
            model.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            model.PrimaryColumnns = new[] { "Id" };

            return model;
        }

        public override string GetSelectPrincipal()
        {
            var result = new StringBuilder();
            result.Append(" select r.origendoc, r.id, r.fechafactura, r.periodo, r.referencia, r.tipofactura, r.cuentacliente, c.descripcion as [Nombre Cliente] from RegistroIVARepercutido r, Cuentas c ");
            result.AppendFormat(" where r.empresa = c.empresa and r.cuentacliente = c.id and r.empresa ='{0}' ", _context.Empresa);

            return result.ToString();
        }

        #endregion

        public override void create(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as RegistroIvaRepercutidoModel;

                base.create(model);

                _db.SaveChanges();
                tran.Complete();
            }

        }

        public override void edit(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var original = get(Funciones.Qnull(obj.get("id"))) as RegistroIvaRepercutidoModel;
                var editado = obj as RegistroIvaRepercutidoModel;


                base.edit(obj);
                _db.SaveChanges();
                tran.Complete();

            }
        }

        public override void delete(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                base.delete(obj);
                _db.SaveChanges();
                tran.Complete();
            }

        }
    }
}
