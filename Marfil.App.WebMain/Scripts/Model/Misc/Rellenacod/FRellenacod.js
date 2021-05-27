var TipoRellenacod;
(function (TipoRellenacod) {
    TipoRellenacod[TipoRellenacod["Generico"] = 0] = "Generico";
    TipoRellenacod[TipoRellenacod["Letras"] = 1] = "Letras";
    TipoRellenacod[TipoRellenacod["LetrasSimple"] = 2] = "LetrasSimple";
})(TipoRellenacod || (TipoRellenacod = {}));
var FRellenacod = (function () {
    function FRellenacod() {
    }
    FRellenacod.prototype.CreateRellenacod = function (longitud, tipo) {
        if (tipo == TipoRellenacod.Generico) {
            return new RellenacodGeneric(longitud);
        }
        else if (tipo == TipoRellenacod.Letras) {
            return new RellenacodLetters(longitud);
        }
        else if (tipo == TipoRellenacod.LetrasSimple) {
            return new RellenacodLettersSimple(longitud);
        }
        return new RellenacodGeneric(longitud);
    };
    return FRellenacod;
}());
//# sourceMappingURL=FRellenacod.js.map