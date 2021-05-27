class RellenacodDate implements IRellenacodService {

    private _longitud: number;

    constructor() {

    }
    public Formatea(codigo: string): string {
       // codigo = codigo.toUpperCase();
       // codigo = codigo.replace("+", "");

        //var 

        if (codigo.length === 10) {

            return codigo;
        }
        else {
            if (codigo.length === 8){
                return this.WithSlash(codigo);
            }

        }














        //var regex = /^\d{1,2}\/\d{1,2}\/\d{2,4}$/;
        
        //if (codigo.match(regex)) {
            
        //}
    }

    private WithSlash(codigo: string): string {


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
    }


    //private GenericRellenacod(codigo: string): string {

    //    if (codigo === "")
    //        return codigo;
    //    var totalZeros = this._longitud - codigo.length;
    //    var zeros = "";
    //    for (var i = 0; i < totalZeros; i++)
    //        zeros += "0";
    //    var result = zeros + codigo;
    //    result = result.substring(0, this._longitud);
    //    return result;
    //}
}