using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Inf.ResourcesGlobalization.Textos.Entidades;
using Marfil.Dom.Persistencia.Model.CRM;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Validation;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Inf.Genericos.Helper;
using System.Data.Entity.Migrations;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{

    public interface IPeticionesAsincronasService
    {

    }


    public class PeticionesAsincronasService : GestionService<PeticionesAsincronasModel, PeticionesAsincronas>, IPeticionesAsincronasService
    {
        #region CTR

        public PeticionesAsincronasService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion


        #region List Index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var st = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var estadosService = new EstadosService(_context, _db);
            st.List = st.List.OfType<PeticionesAsincronasModel>().OrderByDescending(f => f.Fecha).ThenByDescending(f => f.Id);
            var propiedadesVisibles = new[] { "Id", "Usuario", "Fecha", "Estado", "Tipo" };
            var propiedades = Helpers.Helper.getProperties<PeticionesAsincronasModel>();
            st.PrimaryColumnns = new[] { "Id" };
            st.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();            
                                
            return st;
        }

        //public override string GetSelectPrincipal()
        //{
        //    var result = new StringBuilder();

        //    result.Append("SELECT o.*, c.descripcion AS [Descripciontercero]");
        //    result.Append(" FROM Oportunidades as o");
        //    result.Append(" LEFT JOIN Cuentas AS c ON c.empresa = o.empresa AND c.id = o.fkempresa");
        //    result.AppendFormat(" where o.empresa ='{0}' ", _context.Empresa);
        //    return result.ToString();
        //}

        #endregion

        public int NextId()
        {
            return _db.PeticionesAsincronas.Any() ? _db.PeticionesAsincronas.Max(f => f.id) + 1 : 1;
        }

        public void CambiarEstado(EstadoPeticion estado, int idPeticion, string mensaje="")
        {
            var item = _db.PeticionesAsincronas.Where(f => f.empresa == _context.Empresa && f.id == idPeticion).SingleOrDefault();

            item.estado = (int)estado;
            item.resultado = mensaje;

            _db.PeticionesAsincronas.AddOrUpdate(item);
            _db.SaveChanges();
        }

    }

}
