app.directive('changeOnBlur', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, elm, attrs, ngModelCtrl) {
            if (attrs.type === 'radio' || attrs.type === 'checkbox')
                return;

            var expressionToCall = attrs.changeOnBlur;

            var oldValue = null;
            
            elm.bind('focus', function() {
                //scope.$apply(function () {
                oldValue = elm.val();
                //});
            });
            elm.bind('blur', function () {
                //scope.$apply(function () {
                    var newValue = elm.val();
                    if (newValue !== oldValue) {
                        scope.$eval(expressionToCall);
                    }
                    
                //});
            });
        }
    };
});

app.controller('CampoverificacionController', [
    '$scope', '$rootScope', '$http', '$interval', 'uiGridConstants', '$q', '$timeout', function ($scope, $rootScope, $http, $interval, uiGridConstants, $q, $timeout) {

        var rellenacod = function (longitud, tipo, codigo) {
            var frellenacod = new FRellenacod();
            var rellenacodService = frellenacod.CreateRellenacod(longitud, tipo);
            return rellenacodService.Formatea(codigo);
        }

        //members
        $scope.loading = false;
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
        $scope.urledicion ="";
        $scope.tipocuenta = undefined;
        $scope.isValid = function() {
            var result = $q.defer();
            $http.get( $scope.url, {params: {id: $scope.valor }}).
            then(function (name) {
                    result.resolve('OK');
                }, function (name) {
                    result.reject('Error');
                });
            
            return result.promise;
        };

        var filter = function (value) {
            if (!value || value === "")
                return true;
            var retrievedObject = sessionStorage.getItem($scope.currentCache);
            var list = JSON.parse(retrievedObject);
            for (var i = 0; i < list.length; i++) {
                if (list[i][$scope.campoIdentificador] == value) {
                    return true;
                }
            }
            return false;
        }

        $scope.longitud = 0;
        $scope.tipo = "";
        $scope.controlesasociados = "";
        $scope.primeracarga = true;
        $scope.columnabloqueados = "";
        $scope.bloqueado = false;
        $scope.Titulo = "";
        $scope.init = function (longitud, tipo, url, controlAsociado, campoIdentificador, campoDescripcion, modalSearch, valor, params, urlverificacion, urledicion, tipocuenta, controlesasociados, columnabloqueados,titulo) {
            $scope.bloqueado = false;
            $scope.columnabloqueados = columnabloqueados;
            $scope.primeracarga = true;
            $scope.controlesasociados = controlesasociados;
            $scope.longitud = longitud;
            $scope.tipo = tipo;
            $scope.url = url;
            $scope.controlAsociado = controlAsociado;
            $scope.campoIdentificador = campoIdentificador;
            $scope.campoDescripcion = campoDescripcion;
            $scope.modalSearch = modalSearch;
            $scope.params = params;
            $scope.urlverificacion = urlverificacion;
            $scope.urledicion = urledicion;
            $scope.tipocuenta = tipocuenta;
            $scope.Titulo = titulo;

            if ($scope.controlesasociados) {
                var nobj = {};
                if ($scope.params)
                    nobj=JSON.parse($scope.params);
                var controles = JSON.parse($scope.controlesasociados);
               
                for (var i = 0; i < controles.length; i++) {
                    var obj = controles[i];
                    $.each(obj, function (k, v) {
                        nobj[k] = $("#" + v).val();
                    });
                }

                $scope.params = JSON.stringify(nobj);
            }

            var now = new Date();
            $scope.currentCache = controlAsociado + "-" + $scope.campoIdentificador + "-" + campoDescripcion;
            eventAggregator.RegisterEvent(controlAsociado + "-Buscar", function(message) {
                $scope.valor = message;
                $scope.lostFocus();
            });

            eventAggregator.RegisterEvent(controlAsociado + "-Buscarmultiple", function (message) {
                
                $scope.SendValue(message);
            });


            //Llamado desde el BusquedasController.js. Recibe el componente(fkarticulos, fkseries, fkoperarios, Loteentrega) Y EL valor (el articulo, serie, lote), etc.
            eventAggregator.RegisterEvent(controlAsociado + "-Buscarfocus", function (message) {
                console.log("control asociado: " + controlAsociado + " " + message);
                $scope.valor = message;
                var element = window.document.getElementById($scope.controlAsociado);

                //Tesoreria
                if (controlAsociado == "Fkcuentatesoreria") {
                    var cvelement = window.document.getElementById("cv-" + $scope.controlAsociado + "-descripcion");
                }
                //

                if (element) {
                    element.value = message;

                    //Tesoreria
                    if (cvelement) {
                        var cuenta = message;
                        $http.get("/Cuentastesoreria/obtenerCuentaTesoreria" + "?cuenta=" + cuenta).success(function (data) {
                            var modelo = JSON.parse(data);
                            cvelement.textContent = modelo.Descripcion;
                        });
                    }
                    //

                    element.focus();
                }
                $scope.lostFocus(); 
            });

            

            if (valor && valor !== "") {
                $scope.valor = valor;
                $scope.lostFocus();
            } else {
                $scope.primeracarga = false;
            }
            
            if (sessionStorage[$scope.currentCache] == null) {
                if ($scope.params) {
                    var nobj = JSON.parse($scope.params);
                    
                    $http({
                        url: $scope.url,
                        method: "GET",
                        params: nobj
                    }).then(function(response) {
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

        $scope.keydown=function(eve) {
            if (eve.which === 66 && eve.ctrlKey) {
                $scope.buscar();
                eve.preventDefault();
            }
            else if (eve.which === 66 && eve.ctrlKey) {
                $scope.lostFocus();
                eve.preventDefault();
            }
        }

        $scope.buscar = function () {

            if ($scope.controlesasociados) {
                var objaux = JSON.parse($scope.params);
                var controles = JSON.parse($scope.controlesasociados);
                for (var i = 0; i < controles.length; i++) {
                    var item = controles[i];
                    $.each(item, function (k, v) {
                        objaux[k] = $("#" + v).val();
                    });
                }

                $scope.params = JSON.stringify(objaux);
            }
            
            var obj = {
                campoIdentificador: $scope.campoIdentificador,
                IdComponenteasociado: $scope.controlAsociado,
                IdFormulariomodal: $scope.modalSearch,
                Url: $scope.url,
                Titulo: $scope.Titulo,
                Params: $scope.params
            };
            $rootScope.$emit("CallFormSearch", { obj:obj });
        }

        $scope.verificarPk = function () {

            if ($scope.urlverificacion != "" && $scope.urledicion != "") {
                var obj = { id: $scope.valor };
                if ($scope.params) {
                    var nobj = JSON.parse($scope.params);
                    nobj.id = $scope.valor;
                    obj = nobj;
                }
                
                $http({
                    url: $scope.urlverificacion,
                    method: "GET",
                    params: obj
                }).success(function(response) {
                    if (response.Existe) {
                        var resultValue;
                        bootbox.confirm(Messages.EditarRegistroExistente, function(result) {
                            if (result) {
                                window.location = $scope.urledicion + "/" + $scope.valor;
                            } else {
                                $timeout(function() {
                                    var element = window.document.getElementById($scope.controlAsociado);
                                    if (element)
                                        element.focus();
                                    $scope.valor = "";
                                    $scope.descripcion = "";
                                    eventAggregator.Publish($scope.controlAsociado + "-cv", "");
                                });
                            }
                        });
                    }
                });

                
            }

            
        }
        $scope.SendValue = function (parametros) {

            parametros.primeracarga = $scope.primeracarga;
            $scope.primeracarga=false;
            $scope.loading = true;
            mainControllerAggregator.RegisterElement(String.format("{0}{1}{2}", $scope.campoIdentificador, $scope.campoDescripcion, $scope.controlAsociado));
            $http({
                url: $scope.url, method: "GET", params: parametros
            }).success(function (response) {
                $scope.loading = false;
                mainControllerAggregator.DisposeElement(String.format("{0}{1}{2}", $scope.campoIdentificador, $scope.campoDescripcion, $scope.controlAsociado));
                if ($scope.columnabloqueados !== "") {
                    $scope.bloqueado = response[$scope.columnabloqueados];
                }

                $scope.valor = response[$scope.campoIdentificador];
                $scope.descripcion = response[$scope.campoDescripcion];
                $scope.verificarPk();
                eventAggregator.Publish($scope.controlAsociado, $scope.valor);
                eventAggregator.Publish($scope.controlAsociado + "-cv", response);


            }).error(function (data, status, headers, config) {
                mainControllerAggregator.DisposeElement(String.format("{0}{1}{2}", $scope.campoIdentificador, $scope.campoDescripcion, $scope.controlAsociado));
                $scope.bloqueado = false;
                $scope.loading = false;
                var element = document.getElementById($scope.controlAsociado);
                $scope.valor = "";
                $scope.descripcion = "";
                
                if (element) {
                    element.value = "";
                    element.focus();
                    
                }
               
                eventAggregator.Publish($scope.controlAsociado, $scope.valor);
                eventAggregator.Publish($scope.controlAsociado + "-cv", null);
            });
        }

        $scope.lostFocus = function () {
            
            if ($scope.valor === "") {
                $scope.primeracarga = false;
                $scope.valor = "";
                $scope.descripcion = "";
                $scope.bloqueado = false;
                eventAggregator.Publish($scope.controlAsociado, "");
                return;
            }

            if ($scope.tipo !== "")
                $scope.valor = rellenacod($scope.longitud, $scope.tipo, $scope.valor);
           
            var obj = { id: $scope.valor };
            if ($scope.params) {
                var nobj = JSON.parse($scope.params);
                nobj.id = $scope.valor;
                obj = nobj;
            }

            if ($scope.controlesasociados) {
                var controles = JSON.parse($scope.controlesasociados);
                for (var i = 0; i < controles.length; i++) {
                    var item = controles[i];
                    $.each(item, function (k, v) {
                        obj[k] = $("#" + v).val();
                    });
                }
            }

            $scope.SendValue(obj);

        }



    }]);

app.controller('BotonaltaenlazadaCtrl', [
'$scope', '$rootScope','$compile', function ($scope, $rootScope,$compile) {
    $scope.nuevo = function (campoIdentificador, IdComponenteasociado, IdFormulariomodal, Url) {
        $("#" + IdFormulariomodal).modal();
        $("#" + IdFormulariomodal + "_contenido").html("<span>Loading</span>");
        $("#" + IdFormulariomodal + "_contenido").load(Url);
       
    }
}
]);