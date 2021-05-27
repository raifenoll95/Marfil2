using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.App.TestGenerator.Model;
using Marfil.App.TestGenerator.Services.Interfaces;
using Marfil.Dom.Persistencia.Model.Terceros;

namespace Marfil.App.TestGenerator.Services
{
   internal class FGenerarTest
   {
       private Dictionary<Type, IGenerarTest> _dictionary;

       private static FGenerarTest _instance;
       public static FGenerarTest Instance
       {
           get
            {

               if(_instance == null)
                    _instance=new FGenerarTest();
               return _instance;
           }
       }

       private FGenerarTest()
       {
           _dictionary= new Dictionary<Type, IGenerarTest>();
            _dictionary.Add(typeof(Clientes), new TestClientes());
       }

       public IGenerarTest Generar<T>()
       {
           return _dictionary[typeof(T)];
       }
    }
}
