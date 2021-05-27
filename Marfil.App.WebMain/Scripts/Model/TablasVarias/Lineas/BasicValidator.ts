class BasicValidator implements IValidator {
    IsValidObject(data, obj) {

        if (!Funciones.IsRequired(obj.Valor))
            throw "El campo Valor es obligatorio";

        if (!Funciones.IsUnique(data, obj.Valor, "Valor"))
            throw "El campo Valor de ser único";

        if (!obj.Descripcion) {
            throw "El campo Descripción es obligatorio";
        }

        return true;
    }
}