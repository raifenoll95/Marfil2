var RellenacodGeneric = (function () {
    function RellenacodGeneric(longitud) {
        this._longitud = longitud;
    }
    RellenacodGeneric.prototype.Formatea = function (codigo) {
        codigo = codigo.toUpperCase();
        codigo = codigo.replace("+", "");
        var regexPointMiddle = /^.+\..*$/;
        var regexPoint = /^.+\.$/;
        if (codigo.match(regexPointMiddle)) {
            return this.WithPoint(codigo);
        }
        else {
            return this.GenericRellenacod(codigo);
        }
    };
    RellenacodGeneric.prototype.WithPoint = function (codigo) {
        var vector = codigo.split('.');
        if (vector[1].length === 1 && vector[1] === "*")
            vector[1] = "";
        var totalZeros = this._longitud - (vector[0].length + vector[1].length);
        var zeros = "";
        for (var i = 0; i < totalZeros; i++)
            zeros += "0";
        return vector[0] + zeros + vector[1];
    };
    RellenacodGeneric.prototype.GenericRellenacod = function (codigo) {
        if (codigo === "")
            return codigo;
        var totalZeros = this._longitud - codigo.length;
        var zeros = "";
        for (var i = 0; i < totalZeros; i++)
            zeros += "0";
        var result = zeros + codigo;
        result = result.substring(0, this._longitud);
        return result;
    };
    return RellenacodGeneric;
}());
//# sourceMappingURL=RellenacodGeneric.js.map