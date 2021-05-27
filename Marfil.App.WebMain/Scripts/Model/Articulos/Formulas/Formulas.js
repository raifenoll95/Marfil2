var FormulaSuperficie = (function () {
    function FormulaSuperficie() {
    }
    FormulaSuperficie.prototype.calculate = function (cantidad, largo, ancho, grueso, metros, decimales) {
        var operador = decimales <= 0 ? 1 : Math.pow(10, decimales);
        return (Math.round(cantidad * largo * ancho * operador) / operador);
    };
    return FormulaSuperficie;
}());
var FormulaVolumen = (function () {
    function FormulaVolumen() {
    }
    FormulaVolumen.prototype.calculate = function (cantidad, largo, ancho, grueso, metros, decimales) {
        var operador = decimales <= 0 ? 1 : Math.pow(10, decimales);
        return (Math.round(cantidad * largo * ancho * grueso * operador) / operador);
    };
    return FormulaVolumen;
}());
var FormulaLinear = (function () {
    function FormulaLinear() {
    }
    FormulaLinear.prototype.calculate = function (cantidad, largo, ancho, grueso, metros, decimales) {
        var operador = decimales <= 0 ? 1 : Math.pow(10, decimales);
        return (Math.round(cantidad * largo * operador) / operador);
    };
    return FormulaLinear;
}());
var FormulaCantidad = (function () {
    function FormulaCantidad() {
    }
    FormulaCantidad.prototype.calculate = function (cantidad, largo, ancho, grueso, metros, decimales) {
        var operador = decimales <= 0 ? 1 : Math.pow(10, decimales);
        return (Math.round(cantidad * operador) / operador);
    };
    return FormulaCantidad;
}());
var FormulaTotal = (function () {
    function FormulaTotal() {
    }
    FormulaTotal.prototype.calculate = function (cantidad, largo, ancho, grueso, metros, decimales) {
        var operador = decimales <= 0 ? 1 : Math.pow(10, decimales);
        return (Math.round(metros * operador) / operador);
    };
    return FormulaTotal;
}());
//# sourceMappingURL=Formulas.js.map