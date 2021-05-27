class RellenacodLetters implements IRellenacodService {

    private _longitud: number;

    constructor(longitud: number) {
        this._longitud = longitud;
    }
    public Formatea(codigo: string): string {
        codigo = codigo.replace("+", "");
        codigo = codigo.toUpperCase();
        var longitudtotal = this._longitud == 0 ? codigo.length : this._longitud;
        codigo = codigo.substring(0, longitudtotal);
        return codigo;
    }
}