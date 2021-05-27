enum TipoMensaje {
    Informacion,
    Warning,
    Error,
    Exito
}

class MessagesService {

    private _servicio: any;

    constructor(servicio) {
        this._servicio = servicio;
    }

    show(tipo: TipoMensaje, titulo: string, contenido: string,image:string = "") {

        var classname;
        if (tipo === TipoMensaje.Informacion) {
            classname = "gritter-info gritter-light";
        }
        else if (tipo === TipoMensaje.Exito) {
            classname = "gritter-success gritter-light";
        }
        else if (tipo === TipoMensaje.Warning) {
            classname = "gritter-warning gritter-light";
        } else {
            classname = "gritter-error gritter-light";
        }

        this._servicio.add({
            title: titulo,
            text: contenido,
            class_name: classname,
            image: image
        });
    }
}

