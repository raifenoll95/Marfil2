using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using System.Data;
using DevExpress.DataAccess.Sql;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface ISeccionesAnaliticas
    {

    }

    public class SeccionesanaliticasService:GestionService<SeccionesanaliticasModel, Seccionesanaliticas>, ISeccionesAnaliticas
    {
        #region CTR
        public SeccionesanaliticasService(IContextService context, MarfilEntities db = null): base (context,db)
        {

        }
        #endregion

        #region Listado principal

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var st = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var propiedadesVisibles = new[] { "Id", "Nombre", "Grupo" };
            var propiedades = Helpers.Helper.getProperties<SeccionesanaliticasModel>();
            st.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
           // st.ColumnasCombo.Add("grupo", _appService.GetListGrupoSecciones().Select(f => new Tuple<string, string>(f.Valor, f.Descripcion)));
            st.OrdenColumnas.Add("Id", 0);
            st.OrdenColumnas.Add("Nombre", 1);
            st.OrdenColumnas.Add("Grupo", 2);
            return st;
        }


        public override string GetSelectPrincipal()
        {
            return string.Format("select s.id,s.nombre,s.grupo from seccionesanaliticas as s " +
                                    " where s.empresa='{0}'", Empresa);
            //"select s.*, c.descripcion as Nombreseccion from seccionesanaliticas as s ");// +
            //" inner join cuentas as c on c.empresa=s.empresa and c.id=s.fkclientes" +
            //" where s.empresa='{0}'", Empresa);

            //return string.Format("select s.*,tvl from seccionesanaliticas as s " +
            //            //" left join tablasvarias as tv on tv.empresa=s.empresa and tv.id=s.grupo" +
            //            //" inner join tablasvariaslin as tvl on tvl.fkTablasvarias=tv.id" +
            //            " where s.empresa='{0}'", Empresa);

        }

        #endregion


        #region Importar

        public void Importar(DataTable dt, IContextService context)
        {
            string errores = "";            
            SeccionesanaliticasModel secc = new FModel().GetModel<SeccionesanaliticasModel>(context);

            foreach (DataRow row in dt.Rows)
            {
                secc.Id = row["CodSec"].ToString().PadLeft(4, '0').ToUpper();                
                secc.Nombre = row["Nombre"].ToString();               

                try
                {
                    create(secc);
                }
                catch (Exception ex)
                {
                    errores += row["CodSec"] + ";" + row["CodSec"] + ";" + ex.Message;
                }
            }

            throw new Exception(errores);
        }

        #endregion

    }
}
