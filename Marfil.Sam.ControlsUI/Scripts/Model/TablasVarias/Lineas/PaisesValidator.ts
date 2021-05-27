class PaisesValidator implements IValidator
    {
    IsValidObject(data, obj) {

        if (!Funciones.IsRequired(obj.CodigoIsoAlfa2))
            throw "El campo Código Iso Alfa 2 es obligatorio";
        if (!Funciones.IsRequired(obj.CodigoIsoAlfa3))
            throw "El campo Código Iso Alfa 3 es obligatorio";
        if (!Funciones.IsRequired(obj.CodigoIsoNumerico))
            throw "El campo Código Iso Numérico es obligatorio";


        if (!Funciones.MaxLength(obj.CodigoIsoAlfa2,2))
            throw "La longitud de Código Iso Alfa 2 es: 2";
        if (!Funciones.MaxLength(obj.CodigoIsoAlfa3,3))
            throw "La longitud de Código Iso Alfa 3 es: 3";
        if (!Funciones.MaxLength(obj.CodigoIsoNumerico,3))
            throw "La longitud de Código Iso Numerico es: 3";

        if (!Funciones.MinLength(obj.CodigoIsoAlfa2, 2))
            throw "La longitud de Código Iso Alfa 2 es: 2";
        if (!Funciones.MinLength(obj.CodigoIsoAlfa3, 3))
            throw "La longitud de Código Iso Alfa 3 es: 3";
        if (!Funciones.MinLength(obj.CodigoIsoNumerico, 3))
            throw "La longitud de Código Iso Numerico es: 3";

        if (!Funciones.IsUnique(data, obj.CodigoIsoAlfa2, "CodigoIsoAlfa2"))
            throw "El Código Iso Alfa 2 de ser único";
        if (!Funciones.IsUnique(data, obj.CodigoIsoAlfa3, "CodigoIsoAlfa3"))
            throw "El Código Iso Alfa 3 de ser único";
        if (!Funciones.IsUnique(data, obj.CodigoIsoNumerico, "CodigoIsoNumerico"))
            throw "El Código Iso Numerico debe ser único";


        return true;
    }
}