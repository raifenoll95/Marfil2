using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia
{
    public partial class MarfilEntities : DbContext
    {
        private MarfilEntities(string connectionString)
        : base(connectionString)
        {
        }

        public static MarfilEntities ConnectToSqlServer( string catalog, string host="", string user ="", string pass ="", bool winAuth = false)
        {
            host = string.IsNullOrEmpty(host) ? System.Configuration.ConfigurationManager.AppSettings["Server"] : host;
            user = string.IsNullOrEmpty(user) ? System.Configuration.ConfigurationManager.AppSettings["User"] : user;
            pass = string.IsNullOrEmpty(pass) ? System.Configuration.ConfigurationManager.AppSettings["Password"] : pass;
            var sqlBuilder = new SqlConnectionStringBuilder
            {
                DataSource = host,
                InitialCatalog = catalog,
                PersistSecurityInfo = true,
                IntegratedSecurity = winAuth,
                MultipleActiveResultSets = true,
                UserID = user,
                Password = pass,
                ConnectTimeout = 500                
            };

            // assumes a connectionString name in .config of MyDbEntities
            var entityConnectionStringBuilder = new EntityConnectionStringBuilder
            {
                Provider = "System.Data.SqlClient",
                ProviderConnectionString = sqlBuilder.ConnectionString ,
                Metadata = "res://*/MarfilModel.csdl|res://*/MarfilModel.ssdl|res://*/MarfilModel.msl"
            };
            return new MarfilEntities(entityConnectionStringBuilder.ConnectionString);
        }

        public DbSet Set(string name)
        {
            // you may need to fill in the namespace of your context
            return base.Set(Type.GetType(name));
        }
    }
}
