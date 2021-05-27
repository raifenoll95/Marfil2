using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Configuracion;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IMaterialesService
    {

    }

    public class MaterialesService : GestionService<MaterialesModel, Materiales>, IMaterialesService
    {
        #region CTR

        public MaterialesService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        #region List Index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var propiedadesVisibles = new[] {"Id", "Descripcion", "Gruposmateriales", "Dureza","Familiamateriales" };

            var propiedades = Helpers.Helper.getProperties<MaterialesModel>();
            model.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();

           //model.ColumnasCombo.Add("Gruposmateriales", _appService.GetListGrupoMateriales().Select(f=>new Tuple<string, string>(f.Descripcion,f.Descripcion)));
           model.ColumnasCombo.Add("Familiamateriales", _appService.GetListFamiliaMateriales().Select(f => new Tuple<string, string>(f.Descripcion, f.Descripcion)));

            return model;
        }

        /*
        public override string GetSelectPrincipal()
        {
            return string.Format("select m.id,m.descripcion,m.dureza,fm.descripcion as Familiamateriales,gm.descripcion as Gruposmateriales from materiales as m " +
                                 " left join Familiamateriales as fm on fm.valor=m.fkfamiliamateriales " +
                                 " left join Gruposmateriales as gm on gm.valor =m.fkgruposmateriales " +
                   " where m.empresa='{0}'",Empresa);
        }
        */

        public override string GetSelectPrincipal()
        {
            return string.Format("select m.id,m.descripcion,m.dureza,fm.descripcion as Familiamateriales,gm.descripcion as Gruposmateriales from materiales as m " +
                                 " left join Familiamateriales as fm on fm.valor=m.fkfamiliamateriales " +
                                 " left join GrupoMateriales as gm on gm.empresa = m.empresa and gm.cod =m.fkgruposmateriales " +
                   " where m.empresa='{0}'", Empresa);
        }


        public override void create(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as MaterialesModel;
                DocumentosHelpers.GenerarCarpetaAsociada(model, TipoDocumentos.Materiales, _context, _db);
                base.create(obj);
                tran.Complete();
            }
        }

        #endregion
    }
}
