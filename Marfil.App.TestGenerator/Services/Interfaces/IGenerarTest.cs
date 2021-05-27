using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Interfaces;

namespace Marfil.App.TestGenerator.Services.Interfaces
{
    internal interface IGenerarTest
    {
        string GenerarTest<T>(T model);
    }
}
