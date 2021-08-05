using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Contabilidad;
using Marfil.Dom.Persistencia.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public class CuadernosBancariosServices : GestionService<CuadernosBancariosModel, CuadernosBancarios>
    {
        #region CTR

        public CuadernosBancariosServices(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        #region ListIndexModel

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var propiedadesVisibles = new[] { "Clave", "Descripcion", "Banco", "Formato"};
            var propiedades = Helpers.Helper.getProperties<CuadernosBancariosModel>();

            //model.PrimaryColumnns = new[] { "id" };
            model.ExcludedColumns = propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            model.OrdenColumnas.Add("Clave", 0);
            model.OrdenColumnas.Add("Descripcion", 1);
            model.OrdenColumnas.Add("Banco", 2);
            model.OrdenColumnas.Add("Formato", 3);

            return model;
        }

        public override string GetSelectPrincipal()
        {
            return string.Format("select * " +
                "from CuadernosBancarios " +
                "where empresa='{0}'", Empresa);
        }

        #endregion

        public IEnumerable<CuadernosBancariosModel> GetCuadernos()
        {
            return _db.Set<CuadernosBancarios>().ToList().Select(f => _converterModel.GetModelView(f) as CuadernosBancariosModel);
        }

        public string DeleteLin(string id)
        {
            var intid = int.Parse(id);
            var lin = _db.CuadernosBancariosLin.Where(f => f.id == intid).FirstOrDefault();

            if (lin != null)
            {
                var registro = _db.CuadernosBancariosLin.Where(f => f.id == intid).FirstOrDefault().registro;
                _db.CuadernosBancariosLin.Remove(lin);
                _db.SaveChanges();

                return registro;
            }
            else
            {
                return "";
            }

        }

        public void DeleteAllLin()
        {
            var lin = _db.CuadernosBancariosLin.Where(f => f.idCab == null).ToList();

            if (lin != null)
            {
                _db.CuadernosBancariosLin.RemoveRange(lin);
                _db.SaveChanges();

            }

        }
    }
}
