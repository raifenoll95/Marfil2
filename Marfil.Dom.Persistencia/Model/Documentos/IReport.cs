using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.DataAccess.Sql;

namespace Marfil.Dom.Persistencia.Model.Documentos
{
    public struct ReportInfo
    {
        public string Usuario { get; set; }
        public int Tipo { get; set; }
    }

    public interface IReport
    {
        SqlDataSource DataSource { get; }
    }
}
