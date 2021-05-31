using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.Inmueble;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public class InmueblesService : GestionService<InmueblesModel, Inmuebles>
    {

        #region CTR

        public InmueblesService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        #region ListIndexModel

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var propiedadesVisibles = new[] { "Id", "Descripcion", "DescripcionProvincia", "FkMunicipioNom", "RefCatastral" };
            var propiedades = Helpers.Helper.getProperties<InmueblesModel>();

            //model.PrimaryColumnns = new[] { "id" };
            model.ExcludedColumns = propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            model.OrdenColumnas.Add("Id",0);
            model.OrdenColumnas.Add("Descripcion", 1);
            model.OrdenColumnas.Add("DescripcionProvincia", 2);
            model.OrdenColumnas.Add("DescripcionMunicipio", 3);
            model.OrdenColumnas.Add("RefCatastral", 4);
            
            return model;
        }

        public override string GetSelectPrincipal()
        {
            return string.Format("select i.*,pa.descripcion as DescripcionPais, pr.nombre as DescripcionProvincia, mu.nombre as DescripcionMunicipio from inmuebles as i " +
                   " inner join paises as pa on pa.valor=i.fkPais" +
                   " inner join provincias as pr on pr.id=i.fkProvinciaCod " +
                   " inner join municipios as mu on mu.cod=i.fkMunicipioCod and mu.codigoprovincia=i.fkProvinciaCod" + 
                   " where i.empresa='{0}'", Empresa);
        }

        #endregion


    }
}
