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

    static MaxLength(valor: any, maxLength: number) {
        return valor.length <= maxLength;
    }

    static MinLength(valor: any, minLength: number) {
        return valor.length >= minLength;
    }

    static ShowErrorMessage(mensaje: string) {
        document.getElementById("errors").innerHTML = '<div id="mensaje_error" class="alert alert-warning boxdialog">' +
            '<button type= "button" class="close" onclick= "$(\'#mensaje_error\').fadeOut()" data- dismiss="alert" aria- hidden="true" >&times; </button>' +
            '<strong> <i class="fa fa-warning" > </i> Atenci&oacute;n!</strong> ' + mensaje + '</div>';
    }
}