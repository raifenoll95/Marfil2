class RellenacodGeneric implements IRellenacodService {

    private _longitud: number;

    constructor(longitud: number) {
        this._longitud = longitud;
    }
    public Formatea(codigo: string): string {
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
    }

    private WithPoint(codigo: string): string {
        var vector = codigo.split('.');
        if (vector[1].length === 1 && vector[1] === "*")
            vector[1] = "";
        var totalZeros = this._longitud - (vector[0].length + vector[1].length);
        var zeros = "";
        for (var i = 0; i < totalZeros; i++)
            zeros += "0";
        return vector[0] + zeros + vector[1];
    }


    private GenericRellenacod(codigo: string): string {

        if (codigo === "")
            return codigo;
        var totalZeros = this._longitud - codigo.length;
        var zeros = "";
        for (var i = 0; i < totalZeros; i++)
            zeros += "0";
        var result = zeros + codigo;
        result = result.substring(0, this._longitud);
        return result;
    }
}