using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Iva;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public class TiposFacturasIvaService : GestionService<TiposFacturasIvaModel, TiposFacturas>
    {
        #region CONSTRUCTOR
        public TiposFacturasIvaService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }
        #endregion

        #region List index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var propiedadesVisibles = new[] { "Tipocircuito", "Regimeniva", "Descripcion" };
            var propiedades = Helpers.Helper.getProperties<TiposFacturasIvaModel>();
            model.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            model.PrimaryColumnns = new[] { "Id" };

            return model;
        }

        public override string GetSelectPrincipal()
        {
            var result = new StringBuilder();
            result.Append(" select * from TiposFacturas ");
            result.AppendFormat(" where empresa ='{0}' ", _context.Empresa);

            return result.ToString();
        }

        public string GetTipoFacturaDefectoRepercutido()
        {
            return _db.TiposFacturas.Where(f => f.empresa == _context.Empresa && f.tipocircuito == 1 && f.tipofacturadefecto == true).FirstOrDefault().id.ToString();
        }

        public string GetTipoFacturaDefectoSoportado()
        {
            return _db.TiposFacturas.Where(f => f.empresa == _context.Empresa && f.tipocircuito == 0 && f.tipofacturadefecto == true).FirstOrDefault().id.ToString();
        }

        public string GetTipoFacturaClientes(string codTercero)
        {
            return _db.Clientes.Where(f => f.empresa == _context.Empresa && f.fkcuentas == codTercero).FirstOrDefault().fktipofactura;
        }

        public string GetTipoFacturaProveedores(string codTercero)
        {
            return _db.Proveedores.Where(f => f.empresa == _context.Empresa && f.fkcuentas == codTercero).FirstOrDefault().fktipofactura;
        }

        #endregion
    }
}
