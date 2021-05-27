var TipoFicherosEmail;
(function (TipoFicherosEmail) {
    TipoFicherosEmail[TipoFicherosEmail["Local"] = 0] = "Local";
    TipoFicherosEmail[TipoFicherosEmail["Url"] = 1] = "Url";
})(TipoFicherosEmail || (TipoFicherosEmail = {}));
var TipoAccion;
(function (TipoAccion) {
    TipoAccion[TipoAccion["Seguimientos"] = 0] = "Seguimientos";
    TipoAccion[TipoAccion["Pedidos"] = 1] = "Pedidos";
    TipoAccion[TipoAccion["Presupuestos"] = 2] = "Presupuestos";
    TipoAccion[TipoAccion["RecepcionesStock"] = 3] = "RecepcionesStock";
    TipoAccion[TipoAccion["AlbaranesCompras"] = 4] = "AlbaranesCompras";
})(TipoAccion || (TipoAccion = {}));
var FicherosEmailModel = (function () {
    function FicherosEmailModel() {
    }
    return FicherosEmailModel;
}());
var EmailModel = (function () {
    function EmailModel() {
    }
    EmailModel.prototype.EmailModel = function () {
        this.DestinatarioCc = "";
        this.DestinatarioBcc = "";
        this.Destinatario = "";
        this.Ficheros = [];
    };
    return EmailModel;
}());
//# sourceMappingURL=EmailModel.js.map