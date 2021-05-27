

app.controller('EmailCtrl', ['$scope', '$http', '$location', '$window', '$timeout', function ($scope, $http, $location, $window, $timeout) {

    $scope.Urlimagenesemail = "";
    $scope.Subiendoarchivo = false;
    $scope.Model = new EmailModel();
    $scope.MostrarCc = false;
    $scope.MostrarBcc = false;
    $scope.Enviando = false;
    $scope.Url = "";
    $scope.ResultadoEnvio = 2;
    $scope.Urlbuscarcontacto = "";
    $scope.UrlToEdit;

    $scope.init = function (urlEdit, url, urlenviararchivo, urlbuscarcontacto) {

        $scope.UrlToEdit = urlEdit;
        $scope.Url = url;
        $scope.Urlimagenesemail = urlenviararchivo;
        $scope.Urlbuscarcontacto = urlbuscarcontacto;

    }

    $scope.BuscarDireccion = function () {
        $scope.BuscarDireccionInternal("Destinatario");
    }

    $scope.BuscarDireccionCC = function () {
        $scope.BuscarDireccionInternal("CC");

    };

    $scope.BuscarDireccionBCC = function () {
        $scope.BuscarDireccionInternal("BCC");

    };

    $scope.BuscarDireccionInternal = function (componeteasociado) {
        var obj = {
            campoIdentificador: "Email",
            IdComponenteasociado: componeteasociado,
            IdFormulariomodal: "BusquedaGlobal",
            Url: $scope.Urlbuscarcontacto,
            Titulo: "Busqueda de Contacto",
            Params: "{\"ocultaremailvacio\":true,\"fkentidad\":\"" + $scope.Model.Fkcuenta + "\" }"

        };

        eventAggregator.Publish("_lanzarbusqueda", obj);
    }

    //Buscar Focus Destinatario
    eventAggregator.RegisterEvent("Destinatario-Buscarfocus", function (msg) {

        if ($scope.Model.Destinatario != null) {
            $scope.Model.Destinatario = "";
        }

        if ($scope.Model.Destinatario && $scope.Model.Destinatario != "")
            $scope.Model.Destinatario = String.format("{0};",$scope.Model.Destinatario);
        $scope.Model.Destinatario = String.format("{0}{1}", $scope.Model.Destinatario,msg);
    });

    eventAggregator.RegisterEvent("BCC-Buscarfocus", function (msg) {
        if ($scope.Model.DestinatarioBcc && $scope.Model.DestinatarioBcc != "")
            $scope.Model.DestinatarioBcc = String.format("{0};", $scope.Model.DestinatarioBcc);
        $scope.Model.DestinatarioBcc = String.format("{0}{1}", $scope.Model.DestinatarioBcc, msg);
    });

    eventAggregator.RegisterEvent("CC-Buscarfocus", function (msg) {
        if ($scope.Model.DestinatarioCc && $scope.Model.DestinatarioCc != "")
            $scope.Model.DestinatarioCc = String.format("{0};", $scope.Model.DestinatarioCc);
        $scope.Model.DestinatarioCc = String.format("{0}{1}", $scope.Model.DestinatarioCc, msg);
    });



    var enviarArchivo = function (input) {
        $scope.Subiendoarchivo = true;
        var request = new XMLHttpRequest();
        var nuevoFormulario = new FormData();
        nuevoFormulario.append('file', input.files[0]);
        request.onreadystatechange = function () {
            if (request.readyState == 4 && request.status == 200) {
                var ficheroDefecto = new FicherosEmailModel();
                ficheroDefecto.Nombre = String.format(input.files[0].name);
                ficheroDefecto.Tipo = TipoFicherosEmail.Local;
                ficheroDefecto.Url = JSON.parse(request.response).filename;
                $scope.Model.Ficheros.push(ficheroDefecto);
                $scope.Subiendoarchivo = false;
                $scope.$apply();
            } else if (request.readyState == 400 || request.readyState == 500) {
                $scope.Subiendoarchivo = false;
            }
        }
        request.open("POST", $scope.Urlimagenesemail);
        request.send(nuevoFormulario);
        $scope.$apply();
    }

    eventAggregator.RegisterEvent("Mostraremail", function (message) {
        $scope.Model = message;
        $scope.Enviando = false;
        $scope.ResultadoEnvio = 2;
        $scope.Subiendoarchivo = false;
        $("#editor1").html($scope.Model.Contenido);
        $scope.$apply();
        $("#enviaremail").modal();
    });

    $scope.seleccionarArchivo = function (input) {
        if (input.files && input.files.length > 0) {
            enviarArchivo(input);
        }
    }



    $scope.removeElement = function (item) {
        var index = $scope.Model.Ficheros.indexOf(item);
        if (index > -1) {
            $scope.Model.Ficheros.splice(index, 1);
        }

    }

    $scope.validarEnvio = function () {
        var result = true;

        if ($scope.Model.Destinatario == "") {
            $scope.Model.Destinatarioerror = "Es obligatorio indicar un destinatario";
            result = false;
        }

        if ($scope.Model.Asunto == "") {
            $scope.Model.Asuntoerror = "Es obligatorio indicar el asunto del email";
            result = false;
        }

        return result;
    }

    //Aqui se va a enviar el correo electronico
    $scope.Send = function () {
        if ($scope.validarEnvio()) {
            $scope.Enviando = true;
            $scope.ResultadoEnvio = 2;
            $scope.Model.Contenido = $("#editor1").html();

            $.post($scope.Url, $scope.Model).success(function (message) {

                $("#enviaremail").modal('hide');
                $scope.Enviando = false;
                messagesService.show(TipoMensaje.Exito, "Bien!", "Email enviado correctamente");
                $scope.$apply();

                //SEGUIMIENTOS
                if ($scope.Model.Tipo == 0) {
                    eventAggregator.Publish("_recargarCorreos", $scope.Model);
                }        

            }).error(function (jqXHR, textStatus, errorThrown) {
                $scope.Enviando = false;
                $scope.ResultadoEnvio = 0;
                $scope.$apply();
            });
        }
    }

}]);