var Funciones = (function () {
    function Funciones() {
    }
    Funciones.IsUnique = function (data, valor, columna) {
        var result = data.filter(function (value, index, ar) {
            return value[columna] == valor;
        });
        return result.length <= 1;
    };
    Funciones.IsRequired = function (valor) {
        return valor;
    };
    Funciones.MaxLength = function (valor, maxLength) {
        return valor.length <= maxLength;
    };
    Funciones.MinLength = function (valor, minLength) {
        return valor.length >= minLength;
    };
    Funciones.ShowErrorMessage = function (mensaje) {
        document.getElementById("errors").innerHTML = '<div id="mensaje_error" class="alert alert-warning boxdialog">' +
            '<button type= "button" class="close" onclick= "$(\'#mensaje_error\').fadeOut()" data- dismiss="alert" aria- hidden="true" >&times; </button>' +
            '<strong> <i class="fa fa-warning" > </i> Atenci&oacute;n!</strong> ' + mensaje + '</div>';
    };
    return Funciones;
}());
//# sourceMappingURL=Funciones.js.map