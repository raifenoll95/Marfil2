using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.ControlsUI.NifCif;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.Empresa;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Xunit;

namespace Marfil.Dom.Persistencia_UT
{
  
    public class BaseFixture:IDisposable
    {
        private readonly string _dbName ;//cambiar a nombre fijo

        public string DbName
        {
            get { return _dbName;}
        }

        public IContextService Context { get; private set; }

        public BaseFixture()
        {
            _dbName = Guid.NewGuid().ToString();
            Context = new UTContextService(_dbName);
           Common.Configure(_dbName);

        }

        public void CrearDatosDefecto()
        {
            Common.LaunchScript(".\\App_data\\datos.sql", DbName);
        }

        public void Dispose()
        {
            Common.Remove(_dbName);
        }
        
    }
}
