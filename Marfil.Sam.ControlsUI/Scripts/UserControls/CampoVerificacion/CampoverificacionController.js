app.controller('CampoverificacionController', [
    '$scope', '$rootScope', '$http', '$interval', 'uiGridConstants', function ($scope, $rootScope, $http, $interval, uiGridConstants) {

        //members
        $scope.url = "";
        $scope.controlAsociado = "";
        $scope.campoIdentificador = "";
        $scope.campoDescripcion = "";
        $scope.modalSearch = "";

        $scope.valor = "";
        $scope.descripcion = "";
        
        $scope.init = function (url, controlAsociado, campoIdentificador, campoDescripcion,modalSearch) {
            $scope.url = url;
            $scope.controlAsociado = controlAsociado;
            $scope.campoIdentificador = campoIdentificador;
            $scope.campoDescripcion = campoDescripcion;
            $scope.modalSearch = modalSearch;
            eventAggregator.RegisterEvent(controlAsociado + "-Buscar", function(message) {
                $scope.valor = message;
                $scope.lostFocus();
            });
        };

        $scope.keydown=function(eve) {
            if (eve.which === 113) {
                
                $scope.buscar();
            }
            
        }

        $scope.buscar = function () {
            var obj = {
                campoIdentificador: $scope.campoIdentificador,
                IdComponenteasociado: $scope.controlAsociado,
                IdFormulariomodal: $scope.modalSearch,
                Url: $scope.url
            }
            $rootScope.$emit("CallFormSearch", { obj });
        }

        $scope.lostFocus = function () {
            
            if ($scope.valor === "") {
                $scope.valor = "";
                $scope.descripcion = "";
                eventAggregator.Publish($scope.controlAsociado, "");
                return;
            }
            $http({ url: $scope.url,method:"GET",params: {id: $scope.valor }
        }).success(function (response) {
                $scope.valor = response.values[0][$scope.campoIdentificador];
                $scope.descripcion = response.values[0][$scope.campoDescripcion];
                eventAggregator.Publish($scope.controlAsociado, $scope.valor);
        }).error(function (data, status, headers, config) {
            
                var element = document.getElementById($scope.controlAsociado);
                if (element)
                    element.focus();
                $scope.valor = "";
                $scope.descripcion = "";
                eventAggregator.Publish($scope.controlAsociado, $scope.valor);
                });
        }



    }]);