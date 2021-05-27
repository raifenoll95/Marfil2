/// <references path="NifValidators/NifValidationService.ts" />
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
    Funciones.IsNumeric = function (valor) {
        return !isNaN(valor);
    };
    Funciones.MaxLength = function (valor, maxLength) {
        return valor.length <= maxLength;
    };
    Funciones.MinLength = function (valor, minLength) {
        return valor.length >= minLength;
    };
    Funciones.RellenaCeros = function (number, width, lado) {
        if (lado === void 0) { lado = 0; }
        return this.RellenarConCaracter(number, width, '0', lado);
    };
    Funciones.RellenarConCaracter = function (number, width, caracter, lado) {
        if (lado === void 0) { lado = 0; }
        width -= number.toString().length;
        if (width > 0) {
            if (lado === 0) {
                return new Array(width + (/\./.test(number) ? 2 : 1)).join(caracter) + number;
            }
            else {
                return number + (new Array(width + (/\./.test(number) ? 2 : 1)).join(caracter));
            }
        }
        return number + ""; // always return a string
    };
    Funciones.ShowErrorMessage = function (mensaje) {
        document.getElementById("errors").innerHTML = '<div id="mensaje_error" class="alert alert-warning boxdialog">' +
            '<button type= "button" class="close" onclick= "$(\'#mensaje_error\').fadeOut()" data- dismiss="alert" aria- hidden="true" >&times; </button>' +
            '<strong> <i class="fa fa-warning" > </i> Atenci&oacute;n!</strong> ' + mensaje + '</div>';
    };
    Funciones.ShowSuccessMessage = function (mensaje) {
        document.getElementById("success").innerHTML = '<div id="mensaje_exito" class="alert alert-success boxdialog">' +
            ' <button type="button" class="close" onclick="$(\'#mensaje_exito\').fadeOut()" data-dismiss="alert" aria-hidden="true">&times;</button>' +
            '<strong><i class="fa fa-tick"></i> Bien!</strong> ' + mensaje + '</div>';
    };
    Funciones.ValidarDni = function (pais, dni) {
        var fValidatorService = new FNifValidatorService();
        var validatorService = fValidatorService.CreateNifValidator(pais);
        return validatorService.Validate(dni);
    };
    Funciones.RedondearNumerico = function (numero, decimales) {
        var aux = numero * Math.pow(10, decimales);
        aux = Math.round(aux);
        return aux / Math.pow(10, decimales);
    };
    Funciones.RedondearGlobalize = function (numero, decimales) {
        return (numero).toLocaleString(undefined, { minimumFractionDigits: decimales, maximumFractionDigits: decimales });
    };
    Funciones.Redondear = function (numero, decimales) {
        var aux = numero * Math.pow(10, decimales);
        aux = Math.round(aux);
        aux = aux / Math.pow(10, decimales);
        return aux.toFixed(decimales);
    };
    Funciones.Guid = function () {
        return this.s4() + this.s4() + '-' + this.s4() + '-' + this.s4() + '-' +
            this.s4() + '-' + this.s4() + this.s4() + this.s4();
    };
    Funciones.s4 = function () {
        return Math.floor((1 + Math.random()) * 0x10000)
            .toString(16)
            .substring(1);
    };
    return Funciones;
}());
//# sourceMappingURL=Funciones.js.map