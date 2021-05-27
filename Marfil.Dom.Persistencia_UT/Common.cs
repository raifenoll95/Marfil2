using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


using System.IO;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Transactions;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.Dom.Persistencia_UT
{
    public class Common
    {
        #region configure

        public static void Configure(string name)
        {
            var script = File.ReadAllText(Path.Combine(".\\App_data\\marfil.sql"))
                .Replace("[MARFILNAME]", "[" + name + "]");

            var tempName = Path.GetTempFileName().Replace("tmp","sql");
            File.WriteAllText(tempName,script);
            var pi = new ProcessStartInfo("sqlcmd", @"-S 192.168.223.210 -U sa -P Tot.2020 -i " + tempName);
            pi.UseShellExecute = false;
            pi.RedirectStandardError = true;
            var p =Process.Start(pi);
            var error = p.StandardError.ReadToEnd();
            p.WaitForExit();

            File.Delete(tempName);

        }

        public static void Remove(string name)
        {
            var stringconnection = MarfilEntities.ConnectToSqlServer("master").Database.Connection.ConnectionString;
            using (var con = new SqlConnection(stringconnection))
            {
                using (var cmd = new SqlCommand("", con))
                {
                    con.Open();
                    cmd.CommandText = "ALTER DATABASE [" + name + "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "drop database [" + name + "]";
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }


        }

        public static void LaunchScript(string filename, string database)
        {
            var fi=new FileInfo(filename);
            var pi = new ProcessStartInfo("sqlcmd.exe", @"-S 192.168.223.210 -U sa -P Tot.2020 -d "+ database +" -i \"" + fi.FullName+"\"")
            {
                UseShellExecute = false,
                RedirectStandardError = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            var p = Process.Start(pi);
            var error = p.StandardError.ReadToEnd();
            p.WaitForExit();

            
        }
        #endregion

    }
}
