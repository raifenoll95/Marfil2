using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.App.TestGenerator.Model;
using Marfil.App.TestGenerator.Services.Interfaces;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Terceros;

namespace Marfil.App.TestGenerator.Services
{
   internal class TestClientes:IGenerarTest
    {
       

       public string GenerarTest<T>(T model)
       {
           var obj = model as Clientes;
            var sb = new StringBuilder();

            sb.AppendFormat(
                "{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16};{17};{18};{19};{20};{21};{22};{23};{24}",
                obj.Cuentas.Empresa, obj.Cuentas.Id, obj.Cuentas.Descripcion,
                obj.Cuentas.Descripcion2, obj.Cuentas.FkPais, obj.Cuentas.Nif.Nif,
                obj.Cuentas.Nif.TipoNif, obj.Fkidiomas, obj.Fkpuertos.Fkpaises, obj.Fkmonedas,
                obj.Fkregimeniva, (int)obj.Criterioiva, obj.Fkformaspago,
                obj.Descuentoprontopago, obj.Descuentocomercial, obj.Diafijopago1,
                obj.Diafijopago2, obj.Numerocopiasfactura, obj.Fktarifas,
                obj.Porcentajeriesgocomercial, obj.Porcentajeriesgopolitico,
                obj.Riesgoconcedidoempresa, obj.Riesgoaseguradora, obj.Riesgosolicitado,
                obj.Diascondecidos);
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
