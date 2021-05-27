var RellenacodLettersSimple = (function () {
    function RellenacodLettersSimple(longitud) {
        this._longitud = longitud;
    }
    RellenacodLettersSimple.prototype.Formatea = function (codigo) {
        codigo = codigo.toUpperCase();
        var longitudtotal = this._longitud == 0 ? codigo.length : this._longitud;
        codigo = codigo.substring(0, longitudtotal);
        return codigo;
    };
    return RellenacodLettersSimple;
}());
//# sourceMappingURL=RellenacodLettersSimple.js.map