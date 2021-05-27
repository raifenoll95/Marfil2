using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace Marfil.Dom.Persistencia.Helpers
{
    public interface ILogService
    {
        void AddLog(Exception message);
    }
    public class LogService:ILogService
    {
        private readonly ILog _log = LogManager.GetLogger("MarfilLog");

        public LogService()
        {
            log4net.Config.XmlConfigurator.Configure();
        }
        public void AddLog(Exception message)
        {
            var sb=new StringBuilder();
            sb.AppendLine(message.Message);
            sb.AppendLine("------- Stack trace --------");
            sb.AppendLine(message.StackTrace);
            sb.AppendLine(message.TargetSite?.Name);
            if (message.InnerException != null)
            {
                sb.AppendLine("------- Inner exception --------");
                sb.AppendLine(message.InnerException?.Message);
                sb.AppendLine(message.InnerException?.StackTrace);
            }

            
            _log.Error(sb.ToString());
        }
    }
}
