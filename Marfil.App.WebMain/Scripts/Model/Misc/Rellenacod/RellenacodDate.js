var RellenacodDate = (function () {
    function RellenacodDate() {
    }
    RellenacodDate.prototype.Formatea = function (codigo) {
        // codigo = codigo.toUpperCase();
        // codigo = codigo.replace("+", "");
        //var 
        if (codigo.length === 10) {
            return codigo;
        }
        else {
            if (codigo.length === 8) {
                return this.WithSlash(codigo);
            }
        }
        //var regex = /^\d{1,2}\/\d{1,2}\/\d{2,4}$/;
        //if (codigo.match(regex)) {
        //}
    };
    RellenacodDate.prototype.WithSlash = function (codigo) {
        var mycodigo;
        mycodigo = codigo.substring(0, 2) + "/";
        mycodigo += codigo.substring(3, 2) + "/";
        mycodigo += codigo.substring(5, 4);
        return mycodigo;
        //    var vector = codigo.split('.');
        //    if (vector[1].length === 1 && vector[1] === "*")
        //        vector[1] = "";
        //    var totalZeros = this._longitud - (vector[0].length + vector[1].length);
        //    var zeros = "";
        //    for (var i = 0; i < totalZeros; i++)
        //        zeros += "0";
        //    return vector[0] + zeros + vector[1];
    };
    return RellenacodDate;
}());
//# sourceMappingURL=RellenacodDate.js.map