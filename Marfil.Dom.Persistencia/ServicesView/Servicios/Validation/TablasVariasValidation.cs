using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Converter;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    class TablasVariasValidation : BaseValidation<Tablasvarias>
    {
        public TablasVariasValidation(IContextService context, MarfilEntities db) : base(context,db)
        {
        }

        public override bool ValidarGrabar(Tablasvarias model)
        {
            var serializerType = typeof(TablasVariasLinConverterService<>);
            var genericType = serializerType.MakeGenericType(Helper.GetTypeFromFullName(model.clase));
            var serializer = Activator.CreateInstance(genericType,new object[] { Context, _db });
            var methodInfo = genericType.GetMethod("GetModelView");

            var lineas = new List<object>();
            foreach (var item in model.TablasvariasLin)
            {
                (lineas as List<object>).Add(methodInfo.Invoke(serializer, new[] { item }));
            }

            foreach (var item in lineas)
            {
                var validation = item as ICanValidate;
                validation.ValidateModel(lineas);
            }

            return true;
        }

        public override bool ValidarBorrar(Tablasvarias model)
        {
            return false;//No se pueden borrar las tablas varias
        }
    }
}
