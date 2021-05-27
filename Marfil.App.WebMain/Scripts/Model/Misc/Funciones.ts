/// <references path="NifValidators/NifValidationService.ts" />
class Funciones {

    static IsUnique(data: any, valor: any, columna: any) {
        var result = data.filter(function (value, index, ar) {
            return value[columna] == valor;
        });

        return result.length <= 1;
    }

    static IsRequired(valor: any) {
        return valor;
    }

    static IsNumeric(valor: any) {
        return !isNaN(valor);
    }

    static MaxLength(valor: any, maxLength: number) {
        return valor.length <= maxLength;
    }

    static MinLength(valor: any, minLength: number) {
        return valor.length >= minLength;
    }

    static RellenaCeros(number: any, width: number, lado: number = 0) {

        return this.RellenarConCaracter(number, width, '0', lado);
    }

    static RellenarConCaracter(number: any, width: number, caracter: any, lado: number = 0) {

        width -= number.toString().length;
        if (width > 0) {
            if (lado === 0) { //izquierda
                return new Array(width + (/\./.test(number) ? 2 : 1)).join(caracter) + number;
            } else {
                return number + (new Array(width + (/\./.test(number) ? 2 : 1)).join(caracter));
            }
        }


        return number + ""; // always return a string
    }

    static ShowErrorMessage(mensaje: string) {
        document.getElementById("errors").innerHTML = '<div id="mensaje_error" class="alert alert-warning boxdialog">' +
            '<button type= "button" class="close" onclick= "$(\'#mensaje_error\').fadeOut()" data- dismiss="alert" aria- hidden="true" >&times; </button>' +
            '<strong> <i class="fa fa-warning" > </i> Atenci&oacute;n!</strong> ' + mensaje + '</div>';
    }

    static ShowSuccessMessage(mensaje: string) {
        document.getElementById("success").innerHTML = '<div id="mensaje_exito" class="alert alert-success boxdialog">' +
            ' <button type="button" class="close" onclick="$(\'#mensaje_exito\').fadeOut()" data-dismiss="alert" aria-hidden="true">&times;</button>' +
            '<strong><i class="fa fa-tick"></i> Bien!</strong> ' + mensaje + '</div>';
    }

    static ValidarDni(pais: string, dni: string) {
        var fValidatorService = new FNifValidatorService();
        var validatorService = fValidatorService.CreateNifValidator(pais) as INifValidationService;
        return validatorService.Validate(dni);
    }

    static RedondearNumerico(numero: number, decimales: number): number {
        var aux = numero * Math.pow(10, decimales);
        aux = Math.round(aux);
        return aux / Math.pow(10, decimales);
    }

    static RedondearGlobalize(numero: number, decimales: number): any {
        
        return (numero).toLocaleString(undefined, { minimumFractionDigits: decimales, maximumFractionDigits: decimales});
       
    }

    static Redondear(numero: number, decimales: number): any {
        var aux = numero * Math.pow(10, decimales);
        aux = Math.round(aux);
        aux = aux / Math.pow(10, decimales);
        return aux.toFixed(decimales);
    }

    static Guid(): any {
        return this.s4() + this.s4() + '-' + this.s4() + '-' + this.s4() + '-' +
            this.s4() + '-' + this.s4() + this.s4() + this.s4();
    }

    private static s4(): any {
        return Math.floor((1 + Math.random()) * 0x10000)
            .toString(16)
            .substring(1);
    }
}