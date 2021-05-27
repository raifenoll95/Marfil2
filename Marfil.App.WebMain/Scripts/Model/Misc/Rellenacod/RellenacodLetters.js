var RellenacodLetters = (function () {
    function RellenacodLetters(longitud) {
        this._longitud = longitud;
    }
    RellenacodLetters.prototype.Formatea = function (codigo) {
        codigo = codigo.replace("+", "");
        codigo = codigo.toUpperCase();
        var longitudtotal = this._longitud == 0 ? codigo.length : this._longitud;
        codigo = codigo.substring(0, longitudtotal);
        return codigo;
    };
    return RellenacodLetters;
}());
//# sourceMappingURL=RellenacodLetters.js.map