app.controller('CampoidentificadorController', [
    '$scope', '$rootScope', '$http', '$interval', 'uiGridConstants', '$q', '$timeout', function ($scope, $rootScope, $http, $interval, uiGridConstants, $q, $timeout) {

        //members
        $scope.loading = false;
        $scope.editarenpagina = false;
        $scope.nextitemchar = "*";
        $scope.mascara = "";
        $scope.currentCache = "";
        $scope.url = "";
        $scope.controlAsociado = "";
        $scope.campoIdentificador = "";
        $scope.campoDescripcion = "";
        $scope.modalSearch = "";
        $scope.valor = "";
        $scope.descripcion = "";
        $scope.params = "";
        $scope.urlverificacion = "";
        $scope.urledicion = "";
        $scope.tipocuenta = undefined;
        $scope.rellenacod = undefined;
        $scope.permiteexistentes = true;
        $scope.isValid = function () {
            var result = $q.defer();
            $http.get($scope.url, { params: { id: $scope.valor } }).
            then(function (name) {
                result.resolve('OK');
            }, function (name) {
                result.reject('Error');
            });

            return result.promise;
        };


        $scope.checkenter = function(evt) {
            if (evt.which === 13) {
                evt.preventDefault();
                evt.stopPropagation();
                return false;
            }
        };

        $scope.init = function (url, controlAsociado, campoIdentificador, campoDescripcion, modalSearch, valor, params, urlverificacion, urledicion, tipocuenta, mascara, editarenpagina, rellenacod,permiteexistentes) {
            $scope.permiteexistentes = permiteexistentes;
            $scope.rellenacod = rellenacod;
            $scope.editarenpagina = editarenpagina;
            $scope.mascara = mascara;
            $scope.url = url;
            $scope.controlAsociado = controlAsociado;
            $scope.campoIdentificador = campoIdentificador;
            $scope.campoDescripcion = campoDescripcion;
            $scope.modalSearch = modalSearch;
            $scope.params = params;
            $scope.urlverificacion = urlverificacion;
            $scope.urledicion = urledicion;
            $scope.tipocuenta = tipocuenta;
            var now = new Date();
            $scope.currentCache = controlAsociado + "-" + $scope.campoIdentificador + "-" + campoDescripcion;
            eventAggregator.RegisterEvent(controlAsociado + "-Buscar", function (message) {
                $scope.valor = message;
                $scope.lostFocus();
            });

            if (valor && valor !== "") {
                $scope.valor = valor;
                $scope.lostFocus();
            }

            if (sessionStorage[$scope.currentCache] == null) {
                if ($scope.params) {
                    var nobj = JSON.parse($scope.params);

                    $http({
                        url: $scope.url,
                        method: "GET",
                        params: nobj
                    }).then(function (response) {
                        sessionStorage.setItem($scope.currentCache, JSON.stringify(response.data.values));

                    });
                } else {
                    $http({
                        url: $scope.url,
                        method: "GET"
                    }).then(function (response) {
                        sessionStorage.setItem($scope.currentCache, JSON.stringify(response.data.values));

                    });
                }

            }

        };

        $scope.keydown = function (eve) {

            if (eve.which === 113) {
                $scope.buscar();
            }
            else if (eve.which === 13) {
                $scope.lostFocus();
                eve.preventDefault();
            }
        }

        $scope.buscar = function () {
            var obj = {
                campoIdentificador: $scope.campoIdentificador,
                IdComponenteasociado: $scope.controlAsociado,
                IdFormulariomodal: $scope.modalSearch,
                Url: $scope.url,
                Params: $scope.params
            };
            $rootScope.$emit("CallFormSearch", { obj: obj });
        }

        $scope.verificarPk = function () {
           
            if ($scope.editarenpagina) {

                if ($scope.urlverificacion != "" && $scope.urledicion != "") {
                    var obj = { id: $scope.valor };
                    if ($scope.params) {
                        var nobj = JSON.parse($scope.params);
                        obj.tipocuenta = nobj.tipocuenta;
                    }

                    $http({
                        url: $scope.urlverificacion,
                        method: "GET",
                        params: obj
                    }).success(function (response) {
                        if (!response.Valido || !$scope.permiteexistentes) {
                            var element = document.getElementById($scope.controlAsociado);
                            if (element)
                                element.focus();
                            $scope.valor = "";
                            $scope.descripcion = "";
                        }
                        else if (response.Existe) {
                            var resultValue;
                            bootbox.confirm(Messages.EditarRegistroExistente, function (result) {
                                if (result) {
                                    window.location = $scope.urledicion + "/" + $scope.valor;
                                }
                                else {
                                    location.reload(true);
                                }
                            });
                        }
                    });

                }
            }
        }

        $scope.getSiguienteNumero = function () {
            var obj = { id: $scope.valor };
            if ($scope.params) {
                var nobj = JSON.parse($scope.params);
                obj.tipocuenta = nobj.tipocuenta;
            }
            $http({
                url: $scope.url, method: "GET", params: obj
            }).success(function (response) {
                $scope.valor = response[$scope.campoIdentificador];
                $scope.descripcion = "[" + Messages.NuevoRegistro + "]";
                eventAggregator.Publish($scope.controlAsociado, $scope.valor);
            }).error(function (data, status, headers, config) {

                var element = document.getElementById($scope.controlAsociado);
                if (element)
                    element.focus();
                $scope.valor = "";
                $scope.descripcion = "";
                eventAggregator.Publish($scope.controlAsociado, $scope.valor);
                eventAggregator.Publish($scope.controlAsociado + "-ci", null);
            });
        }


        $scope.lostFocus = function () {
          
            if ($scope.valor === "") {
                $scope.valor = "";
                $scope.descripcion = "";
                eventAggregator.Publish($scope.controlAsociado, "");
                eventAggregator.Publish($scope.controlAsociado + "-ci", null);
                return;
            }
            $scope.loading = true;
            if (rellenacod) {
               if(!rellenacod($scope))
                {
                   eventAggregator.Publish($scope.controlAsociado, $scope.valor);
                   $scope.loading = false;
                    return;
                }
            }

            var obj = { id: $scope.valor };
            if ($scope.params) {
                var nobj = JSON.parse($scope.params);
                obj.tipocuenta = nobj.tipocuenta;
            }
            $http({
                url: $scope.url, method: "GET", params: obj
            }).success(function (response) {
                $scope.loading = false;
                if (!$scope.permiteexistentes) {
                    $scope.valor = "";
                    $scope.descripcion = "";
                    var element = document.getElementById($scope.controlAsociado);
                    if (element)
                        element.focus();
                    eventAggregator.Publish($scope.controlAsociado, $scope.valor);
                    eventAggregator.Publish($scope.controlAsociado + "-ci", null);
                } else {
                   
                    $scope.valor = response[$scope.campoIdentificador];
                    $scope.descripcion = response[$scope.campoDescripcion];
                    $scope.verificarPk();
                    eventAggregator.Publish($scope.controlAsociado, $scope.valor);
                    eventAggregator.Publish($scope.controlAsociado + "-ci", response);
                }
                
                
            }).error(function (data, status, headers, config) {
             $scope.loading = false;
                //var element = document.getElementById($scope.controlAsociado);
                //if (element)
                //    element.focus();
                //$scope.valor = "";

                $scope.verificarPk();
                $scope.descripcion = "["+ Messages.NuevoRegistro +"]";
                eventAggregator.Publish($scope.controlAsociado, $scope.valor);
                eventAggregator.Publish($scope.controlAsociado + "-ci", null);
            });
        }



    }]);