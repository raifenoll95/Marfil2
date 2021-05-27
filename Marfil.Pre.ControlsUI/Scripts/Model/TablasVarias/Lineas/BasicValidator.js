var BasicValidator = (function () {
    function BasicValidator() {
    }
    BasicValidator.prototype.IsValidObject = function (data, obj) {
        if (!obj.Descripcion) {
            throw "El campo Descripción es obligatorio";
        }
        return true;
    };
    return BasicValidator;
}());
//# sourceMappingURL=BasicValidator.js.map