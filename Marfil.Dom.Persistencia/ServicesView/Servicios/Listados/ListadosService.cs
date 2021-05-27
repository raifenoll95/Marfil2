using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Marfil.Dom.Persistencia.Listados.Base;
using Marfil.Dom.Persistencia.Model;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Listados
{
    public class ListadosService
    {
        
        public ListadoResultado Listar(ListadosModel model)
        {
            var result = new ListadoResultado();
            result.TituloListado = model.TituloListado;
            result.IdListado = model.IdListado;
            result.WebIdListado = model.WebIdListado;
            result.Filtros = model.Condiciones;
            result.Listado = Query(model.Context,model.Select,model.ValoresParametros);
            result.ConfiguracionColumnas = model.ConfiguracionColumnas;
           return result;
        }

        

        private DataTable Query(IContextService  context,string select,Dictionary<string,object> parametros)
        {
            var result=new DataTable();
            var dbconnection = "";
            using (var db = MarfilEntities.ConnectToSqlServer(context.BaseDatos))
            {
                dbconnection = db.Database.Connection.ConnectionString;
            }
            using (var con = new SqlConnection(dbconnection))
            {
                using (var cmd = new SqlCommand(select, con))
                {
                    cmd.CommandTimeout = 500;
                    foreach (var item in parametros)
                    {
                        cmd.Parameters.AddWithValue(item.Key, item.Value);
                    }

                    using (var ad = new SqlDataAdapter(cmd))
                    {
                        ad.Fill(result);
                    }
                }
            }

           
            return result;
        }
    }
}
