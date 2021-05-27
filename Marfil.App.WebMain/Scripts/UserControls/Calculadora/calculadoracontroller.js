app.controller('CaluladoraCtrl', ['$scope', '$rootScope', '$http', '$interval', 'uiGridConstants', '$timeout', function ($scope, $rootScope, $http, $interval, $uiGridConstants, $timeout) {
    $scope.Model = {};
    $scope.Model.Entrada = undefined;
    $scope.Model.Cantidad = 0;
    $scope.Model.Tipo = "0";
    $scope.Model.Mostrarcantidad = false;
    $scope.FormulaSuperficie = 0;
    $scope.Largo = 0;
    $scope.Ancho = 0;
    $scope.Grueso = 0;
    $scope.Cajas = 0;
    $scope.ControlCantidad = null;

    $scope.inicio = function() {
        $('#modalcalculadora').on('shown.bs.modal', function () {
            $scope.Model.Entrada = undefined;
            $scope.Model.Cantidad = undefined;
            $scope.Model.Tipo = "0";
            $scope.Model.Mostrarcantidad = false;
            $scope.$apply();
            $("#Entrada").focus();
        });
    }
    eventAggregator.RegisterEvent("calculadora", function (msg) {
        $scope.FormulaSuperficie = msg.FormulaSuperficie;
        $scope.Cajas = msg.Cajas;
        $scope.ControlCantidad = msg.ControlCantidad;
        $scope.Largo = msg.Largo;
        $scope.Ancho = msg.Ancho;
        $scope.Grueso = msg.Grueso;
        $scope.Decimalesmedidas = msg.Decimales;
        $scope.TotalMetros = FFormulasService.CreateFormula(msg.FormulaSuperficie).calculate(1, msg.Largo, msg.Ancho, msg.Grueso,msg.Metros, msg.Decimales);
        $("#modalcalculadora").modal('show');
    });
    
    $scope.Aceptar = function () {
        if (!isNaN($scope.Model.Cantidad) && $scope.Model.Cantidad!==Infinity) {
            $scope.ControlCantidad.SetValue($scope.Model.Cantidad);
            OnSuperficieChanged();
            $("#modalcalculadora").modal('hide');
        }
    }

    $scope.$watch("Model.Entrada", function (value, old) {
        $scope.CalcularCantidad();
    });

    $scope.$watch("Model.Tipo", function (value, old) {
        $scope.CalcularCantidad();
    });

    $scope.CalcularCantidad = function () {
        $scope.Model.Mostrarcantidad = false;
        if ($scope.Model.Entrada) {
            if ($scope.Model.Tipo == "0") //metros
            {
                $scope.Model.Cantidad = Math.round($scope.Model.Entrada / $scope.TotalMetros);
            } else//cajas
            {
                $scope.Model.Cantidad = $scope.Model.Entrada * $scope.Cajas;//redondeamos siempre al alza. Ej. 1.1 --> 2
            }

            $scope.Model.Mostrarcantidad = !isNaN($scope.Model.Cantidad) && $scope.Model.Cantidad !== Infinity;
        }
        
    }
}]);


