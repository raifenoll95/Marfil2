using DevExpress.DataAccess.ConnectionParameters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DevExpress.DataAccess.Sql;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.Dom.Persistencia.Model.Documentos.Transformacioneslotesnave
{
    

    class TransformacioneslotesnaveReport: IReport
    {
        public SqlDataSource DataSource { get; private set; }

        public TransformacioneslotesnaveReport(IContextService user,string primarykey)
        {
            
            
                var server = ConfigurationManager.AppSettings["Server"];
                var usuario = ConfigurationManager.AppSettings["User"];
                var password = ConfigurationManager.AppSettings["Password"];
                DataSource = new SqlDataSource("Report", new MsSqlConnectionParameters(server, user.BaseDatos, usuario, password,MsSqlAuthorizationType.SqlServer));
                DataSource.Name = "Report";
                var mainQuery = new CustomSqlQuery("Transformacioneslotesnave", "SELECT *,'' as [CodigoTrabajo],'' as [Trabajo],'' as [CodigoAcabadoInicial],'' as [CodigoAcabadoFinal],'' as [AcabadoInicial],'' as [AcabadoFinal] FROM [Transformacioneslotesnave] ");
                if (!string.IsNullOrEmpty(primarykey))
                {
                    mainQuery.Parameters.Add(new QueryParameter("empresa", typeof(string), user.Empresa));
                    mainQuery.Parameters.Add(new QueryParameter("referencia",typeof(string),primarykey));
                    mainQuery.Sql = "SELECT t.*,tr.id as [CodigoTrabajo],tr.descripcion as [Trabajo],tr.fkacabadoinicial as [CodigoAcabadoInicial],tr.fkacabadofinal as [CodigoAcabadoFinal],aini.descripcion as [AcabadoInicial],afin.descripcion as [AcabadoFinal]  FROM [Transformacioneslotesnave] as t " +
                    " inner join trabajos as tr on tr.empresa= t.empresa and tr.id=t.fktrabajos " +
                                    " left join acabados as aini on aini.id = tr.fkacabadoinicial " +
                                    " left join acabados as afin on aini.id = tr.fkacabadofinal " +
                                    " where t.empresa=@empresa and t.referencia=@referencia";
                }
           
            DataSource.Queries.Add(new CustomSqlQuery("empresa", "SELECT e.*,d.direccion as [Direccionempresa],d.poblacion as [Poblacionempresa],d.cp as [Cpempresa],d.telefono as [Telefonoempresa] FROM [Empresas] as e left join direcciones as d on d.empresa=e.id and d.tipotercero=-1 and d.fkentidad=e.id"));
            DataSource.Queries.Add(mainQuery);
           

            DataSource.Queries.Add(new CustomSqlQuery("Transformacioneslotesnavelin", "SELECT pr.*,u.textocorto as [Unidadesdescripcion] FROM [TransformacioneslotesnaveLin] as pr" +
                     " inner join Familiasproductos as fp on fp.empresa=pr.empresa and fp.id=substring(pr.fkarticulos,0,3) " +
                     " left join unidades as u on fp.fkunidadesmedida=u.id"));
            // Create a master-detail relation between the queries.

            DataSource.Relations.Add("Transformacioneslotesnave", "Transformacioneslotesnavelin", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("id", "fkTransformacioneslotes")});


            DataSource.Relations.Add("Transformacioneslotesnave", "empresa", new[] {
                    new RelationColumnInfo("empresa", "id")});


            DataSource.Relations.Add("Transformacioneslotesnave", "Formaspago", new[] {
                    new RelationColumnInfo("fkformaspago", "id")});

            DataSource.RebuildResultSchema();
                
            
        }
    }
}
