using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

    //public class FacturasDuaValidation : ValidationAttribute 
    //{
    //    public IContextService context;

    //    public FacturasDuaValidation() : base("Los primeros dos caracteres del número de DUA {0} deben ser dígitos y los dos siguientes el código del país de exportación") {
    //    }

    //    public FacturasDuaValidation(FacturasDuaValidation dua) : base("Los primeros dos caracteres del número de DUA {0} deben ser dígitos y los dos siguientes el código del país de exportación")
    //    {
    //        this.context = dua.context;
    //    }

    //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    {
    //        var digitos = value.ToString().Substring(0, 2);
    //        var codpais = value.ToString().Substring(2, 4);

    //        var tablasvariasService = new TablasVariasService(context, MarfilEntities.ConnectToSqlServer(context.BaseDatos));
    //        var paises = tablasvariasService.GetListPaises().Select(f => f.Valor).ToList();

    //        int n;

    //        if(paises.Any(f => f == codpais) && int.TryParse(digitos, out n))
    //        {
    //            return ValidationResult.Success;
    //        }

    //        var errorMessage = FormatErrorMessage(validationContext.DisplayName);
    //        return new ValidationResult(errorMessage);
    //    }
    //}

