enum TipoFicherosEmail {
    Local,
    Url
}

enum TipoAccion {
    Seguimientos,
    Pedidos,
    Presupuestos,
    RecepcionesStock,
    AlbaranesCompras
}

class FicherosEmailModel {
    Tipo: TipoFicherosEmail;
    Nombre: string;
    Url:string;
}

class EmailModel {
    Id: number;
    Tipo: TipoAccion;
    Fkcuenta: string;
    Tituloformulario: string;
    Destinatario: string;
    Destinatarioerror: string;

    PermiteCc: boolean;
    PermiteBcc: boolean;

    Asunto: string;
    Asuntoerror: string;

    Remitente: string;
    Remitenteerror: string;

    DestinatarioCc: string;
    DestinatarioCcerror: string;

    DestinatarioBcc: string;
    DestinatarioBccerror: string;
    
    Contenido: string;

    Ficheros: FicherosEmailModel[];

    EmailModel() {

        this.DestinatarioCc = "";
        this.DestinatarioBcc = "";
        this.Destinatario = "";
        this.Ficheros = [];
    }

}