app.controller('tarifasCtrl', ['$scope', '$http', '$location', '$window', '$timeout', function ($scope, $http, $location, $window, $timeout) {

    $scope.tipotarifa = "";
    $scope.asignaralcrear = false;
    $scope.precioobligatorio = false;
    $scope.tipoflujo = 0;
    $scope.urltarifaapi = "";
    $scope.tarifasbase = [];
    $scope.precioautomaticobase = "";
    $scope.init = function (tipotarifa,precioobligatorio, asignaralcrear, tipoflujo, urltarifaapi, precioautomaticobase) {
        $scope.tipotarifa = tipotarifa;
        $scope.asignaralcrear = asignaralcrear;
        $scope.precioobligatorio = precioobligatorio;
        $scope.tipoflujo = tipoflujo;
        $scope.urltarifaapi = urltarifaapi;
        $scope.precioautomaticobase = precioautomaticobase === "" ? "---" : precioautomaticobase;
        $("#Tipotarifa").prop('readonly', $scope.tipotarifa === "0");
        if ($scope.asignaralcrear)
            $("#tabcalculoprecio").show();
        else
            $("#tabcalculoprecio").hide();
    }

    $scope.$watch("tipoflujo", function () {
        $scope.CalcularTarifasBase();
    });

    $scope.$watch("tipotarifa", function () {
        if ($scope.tipotarifa === "0") {
            $scope.asignaralcrear = true;
            $scope.precioobligatorio = true;
        }
        $("#Fkcuentas").attr('disabled', $scope.tipotarifa === "0");
        $("#btnbuscarFkcuentas").attr('disabled', $scope.tipotarifa === "0");
        $("#Fkmonedas").attr('readonly', $scope.tipotarifa === "0");
        $("#btnbuscarFkmonedas").attr('disabled', $scope.tipotarifa === "0");
        $("[name='Validohasta']").attr('disabled', $scope.tipotarifa === "0");
        $("[name='Validodesde']").attr('disabled', $scope.tipotarifa === "0");

    });

    $scope.$watch("asignaralcrear", function () {
        $("#tabcalculoprecio").attr('disabled', !$scope.asignaralcrear);
        $(".creacionprecio").attr("disabled", !$scope.asignaralcrear);
        $("#Precioautomaticofkfamiliasproductosdesde").attr("disabled", !$scope.asignaralcrear);
        $("#Precioautomaticofkfamiliasproductoshasta").attr("disabled", !$scope.asignaralcrear);
        $("#Precioautomaticofkmaterialesdesde").attr("disabled", !$scope.asignaralcrear);
        $("#Precioautomaticofkmaterialeshasta").attr("disabled", !$scope.asignaralcrear);

        $("#btnbuscarPrecioautomaticofkfamiliasproductosdesde").attr("disabled", !$scope.asignaralcrear);
        $("#btnbuscarPrecioautomaticofkfamiliasproductoshasta").attr("disabled", !$scope.asignaralcrear);
        $("#btnbuscarPrecioautomaticofkmaterialesdesde").attr("disabled", !$scope.asignaralcrear);
        $("#btnbuscarPrecioautomaticofkmaterialeshasta").attr("disabled", !$scope.asignaralcrear);
    });

    $scope.CalcularTarifasBase = function () {
        $http.get($scope.urltarifaapi + "/" + $scope.tipoflujo)
           .success(function (response) {
               $scope.tarifasbase = response;
               $scope.tarifasbase.splice(0, 0, { Id: "---", Descripcion: "Coste de adquisición" });
                $scope.$apply();
            }).error(function (data, status, headers, config) {
               $scope.tarifasbase = [];
               $scope.tarifasbase.splice(0, 0, { Id: "---", Descripcion: "Coste de adquisición" });
               $scope.$apply();
           });
    }

}]);

var exportar = function (formato) {
    $("#formatoexportacion").val(formato);
    $("#exportartarifas").submit();
}



