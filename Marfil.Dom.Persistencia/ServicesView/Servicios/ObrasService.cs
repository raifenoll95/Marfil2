using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IObrasService
    {

    }

    public class ObrasService : GestionService<ObrasModel, Obras>, IObrasService
    {
        #region CTR

        public ObrasService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var st= base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var propiedadesVisibles = new[] { "Id","Fkclientes","Nombrecliente", "Nombreobra", "Fechainiciocadena", "Fktiposobra","Finalizada" };
            var propiedades = Helpers.Helper.getProperties<ObrasModel>();
            st.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            st.ColumnasCombo.Add("Fktiposobra", _appService.GetListTiposObras().Select(f => new Tuple<string, string>(f.Valor, f.Descripcion)));
            st.OrdenColumnas.Add("Id",0);
            st.OrdenColumnas.Add("Fkclientes", 1);
            st.OrdenColumnas.Add("Nombrecliente", 2);
            st.OrdenColumnas.Add("Nombreobra", 3);
            st.OrdenColumnas.Add("Fechainiciocadena", 4);
            st.OrdenColumnas.Add("Fktiposobra", 5);
            st.OrdenColumnas.Add("Finalizada", 6);
            return st;
        }

        public override string GetSelectPrincipal()
        {
            return string.Format("select o.*, c.descripcion as Nombrecliente from obras as o " +
                                 " inner join cuentas as c on c.empresa=o.empresa and c.id=o.fkclientes" +
                                 " where o.empresa='{0}'",Empresa);
        }

        public override void create(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as ObrasModel;
                model.Id= _db.Obras.Any(f => f.empresa == Empresa) ? _db.Obras.Where(f=>f.empresa==Empresa).Max(f => f.id) +1 : 1;

                base.create(obj);

                tran.Complete();
            }   
        }
    }
}

