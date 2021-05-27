class BasicValidator implements IValidator {
    IsValidObject(data, obj) {
        if (!obj.Descripcion) {
            throw "El campo Descripción es obligatorio";
        }

       
        

        return true;
    }
}