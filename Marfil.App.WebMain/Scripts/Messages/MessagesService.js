var TipoMensaje;
(function (TipoMensaje) {
    TipoMensaje[TipoMensaje["Informacion"] = 0] = "Informacion";
    TipoMensaje[TipoMensaje["Warning"] = 1] = "Warning";
    TipoMensaje[TipoMensaje["Error"] = 2] = "Error";
    TipoMensaje[TipoMensaje["Exito"] = 3] = "Exito";
})(TipoMensaje || (TipoMensaje = {}));
var MessagesService = (function () {
    function MessagesService(servicio) {
        this._servicio = servicio;
    }
    MessagesService.prototype.show = function (tipo, titulo, contenido, image) {
        if (image === void 0) { image = ""; }
        var classname;
        if (tipo === TipoMensaje.Informacion) {
            classname = "gritter-info gritter-light";
        }
        else if (tipo === TipoMensaje.Exito) {
            classname = "gritter-success gritter-light";
        }
        else if (tipo === TipoMensaje.Warning) {
            classname = "gritter-warning gritter-light";
        }
        else {
            classname = "gritter-error gritter-light";
        }
        this._servicio.add({
            title: titulo,
            text: contenido,
            class_name: classname,
            image: image
        });
    };
    return MessagesService;
}());
//# sourceMappingURL=MessagesService.js.map