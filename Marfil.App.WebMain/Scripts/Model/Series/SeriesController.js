app.controller('SeriesCtrl', ['$scope', '$rootScope', '$http', '$interval', '$window', '$timeout', function ($scope, $rootScope, $http, $interval, $window, $timeout) {

    $scope.tipodocumento = "";
    $scope.codigo = "";
    $scope.selected = "";
    $scope.urlApiSeries = "";
    $scope.items = [{Id:"",Descripcion:""}];
    $scope.fkserieasociadatipodocumento = "";
    $scope.urlDocumentoAsociado = "";

    $scope.init = function (codigo,tipodocumento,urlApiSeries,serieasociada,urlDocumentoAsociado){
        $scope.urlDocumentoAsociado = urlDocumentoAsociado;
        $scope.urlApiSeries = urlApiSeries;
        $scope.tipodocumento = tipodocumento;
        $scope.codigo = codigo;
        $scope.selected = serieasociada;
    }

    $("[name='Entradasvarias']").change(function () {
        $("[name='Fkseriesasociada']").prop('disabled', this.checked);
        
        if (this.checked) {
            $("[name='Fkseriesasociada']").val("");
            $("[name='Salidasvarias']").prop('checked', !this.checked);
        }
    });

    $("[name='Salidasvarias']").change(function () {
        $("[name='Fkseriesasociada']").prop('disabled', this.checked);
        
        if (this.checked) {
            $("[name='Fkseriesasociada']").val("");
            $("[name='Entradasvarias']").prop('checked', !this.checked);
        }
    });

    $scope.calcularSeries = function () {
        if (!$scope.fkserieasociadatipodocumento) {
            $scope.items = [{ Id: "", Descripcion: "" }];
            return;
        }
        $http.get($scope.urlApiSeries + "/" + $scope.fkserieasociadatipodocumento)
            .success(function (response) {
                if (response.values.length > 0) {
                    $scope.items = response.values;
                    $scope.items.splice(0, 0, { Id: "", Descripcion: "" });
                } else {
                    $scope.items = [{ Id: "", Descripcion: "" }];
                }
                $scope.$apply();
            }).error(function(data, status, headers, config) {
                //alert(status);
            });
    }

    $scope.getDocumentoAsociado = function() {
        $http.get($scope.urlDocumentoAsociado + "/" + $scope.tipodocumento)
                    .success(function (response) {
                        $scope.fkserieasociadatipodocumento = response;

                    }).error(function (data, status, headers, config) {
                        //alert(status);
                    });
    }
    $scope.calcularCoeficiente = function (espesor) {
        Globalize.cultureSelector = $("meta[name='accept-language']").prop("content");
        return (((($scope.grosor) * 100) + espesor) / (2.0 + espesor)).toFixed(3);
    }

    $scope.$watch('tipodocumento', function (newVal, oldVal) {
        $scope.codigo = $scope.tipodocumento;
        $scope.getDocumentoAsociado();
    }, true);

    $scope.$watch('fkserieasociadatipodocumento', function (newVal, oldVal) {
        $scope.calcularSeries();
    }, true);

    var rellenacod = function (longitud, tipo, codigo) {
        var frellenacod = new FRellenacod();
        var rellenacodService = frellenacod.CreateRellenacod(longitud, tipo);
        return rellenacodService.Formatea(codigo);
    }

    $scope.existePk = function (longitud, tipo, urlApi, urlRedirect) {
        $scope.codigo = rellenacod(longitud, tipo, $scope.codigo);
        if (urlApi)
            $http.get(urlApi + "/" + $scope.tipodocumento + "-" + $scope.codigo)
                .success(function (response) {
                    if (response.Existe) {
                        var resultValue;
                        bootbox.confirm(Messages.EditarRegistroExistente, function (result) {
                            if (result) {
                                window.location = urlRedirect + "/" + $scope.tipodocumento + "-" + $scope.codigo;
                            } else {
                                $timeout(function () {
                                    var element = window.document.getElementById("idcontrol");
                                    if (element)
                                        element.focus();
                                    location.reload(true);
                                });
                            }
                        });


                    }
                }).error(function (data, status, headers, config) {
                    alert(status);
                });
    }
}]);