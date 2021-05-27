//Rai

app.controller('SeguimientosCtrl', ['$scope', '$http', '$location', '$window', '$timeout', function ($scope, $http, $location, $window, $timeout) {

    $scope.modeloProspecto;
    $scope.UrlEmpresa;
    $scope.UrlCuentas;
    $scope.UrlContacto;
    $scope.UrlToEdit
    $scope.tipo;
    $scope.UrlCrearCorreos;
    $scope.UrlCosteAccionCRM;

    $scope.init = function (urlEmpresa, urlCuentas, urlContacto, urlEdit, urlCorreos, urlCosteAccionCRM) {

        $scope.UrlEmpresa = urlEmpresa;
        $scope.UrlCuentas = urlCuentas;
        $scope.UrlContacto = urlContacto;
        $scope.UrlToEdit = urlEdit;
        $scope.UrlCrearCorreos = urlCorreos;
        $scope.UrlCosteAccionCRM = urlCosteAccionCRM;
    }

    //Recoge la accion CRM que devuelve el coste en ese ejercicio
    eventAggregator.RegisterEvent("_cargarCosteCRM", function (data) {
        $.get($scope.UrlCosteAccionCRM, { accion: data.accion, fechaejercicio: data.fechaejercicio }).success(function (result) {
            $("[name='Coste']").val(result);   
        }).error(function (jqXHR, textStatus, errorThrown) {
            console.log("error call obtener coste crm");
        });
    });

    //Recargar pantalla correo cuando se haya enviado
    eventAggregator.RegisterEvent("_recargarCorreos", function (msg) {
        var model = msg;
        var origen = $("[name='Origen']").val();
        //Aqui se pued enviar tambien el contenido!!! (model.Contenido)
        $.get($scope.UrlCrearCorreos, {asunto: model.Asunto, destinatario: model.Destinatario, id: model.Id, fkorigen : origen}).success(function (result) {
            window.location.href = $scope.UrlToEdit + "?id=" + msg.Id;
        }).error(function (jqXHR, textStatus, errorThrown) {
            console.log("error call recargar correos");
        });
    });

    eventAggregator.RegisterEvent("EnviarSeguimiento", function (message) {
        var model = new EmailModel();
        model.Tituloformulario = "Seguimientos:";
        model.Id = message;
        model.Tipo = TipoAccion.Seguimientos;
        model.Asunto = $("[name='Asunto']").val(); //Asunto
        model.Contenido = $("[name='Notas']").val(); //Contenido
        model.PertmiteCc = true;
        model.PertmiteBcc = true;
        model.Fkcuenta = $("[name='Fkempresa']").val();


        var referenciaDocumento = $("[name='Origen']").val();
        var empresa = $("[name='Fkempresa']").val();
        var contacto = $("[name='Fkcontacto']").val();
        var ficheroDefecto = new FicherosEmailModel();
        
        
        //Si no tiene un contacto, el destinatario sera el email de ese prospecto LO DE EMPRESA ES UN PROSPECTO.
        //CADA PROSPECTO TIENE UNA LISTA DE DIRECCIONES, Y DE CADA DIRECCION OBTENEMOS SU EMAIL
        if (contacto == null || contacto == "") {
            $.get($scope.UrlEmpresa + '/' + empresa, { primeracarga: 'true' }).success(function (result) {
                $scope.modeloProspecto = result;
                if (result != null) {
                    model.Destinatario = $scope.modeloProspecto.Email;
                }
                eventAggregator.Publish("Mostraremail", model);
            }).error(function (jqXHR, textStatus, errorThrown) {
                console.log("error call /Api/CuentasTiposClientesProspectosExclusiveApi");
            });
        }

        //Si no, sera el email del contacto
        else {
            $.get($scope.UrlCuentas + '/' + contacto).success(function (result) {
                $scope.tipo = result.Tiposcuentas;
            }).error(function (jqXHR, textStatus, errorThrown) {
                console.log("error call /Api/CuentasApi");
            });

            $.get($scope.UrlContacto + '/' + contacto, { fkentidad: empresa, tipotercero: $scope.tipo}).success(function (result) {
                $scope.modeloProspecto = result;
                if (result != null) {
                    model.Destinatario = $scope.modeloProspecto.Email;
                }
                eventAggregator.Publish("Mostraremail", model);
            }).error(function (jqXHR, textStatus, errorThrown) {
                console.log("error call /Api/ContactoContactosApi");
            });
        }       
    });
}]);



