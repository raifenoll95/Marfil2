app.controller('InventariosCtrl', ['$scope', '$http', '$location', '$window', '$timeout', function ($scope, $http, $location, $window, $timeout) {

    $scope.firstEditClientes = false;
    $scope.firstEditAgentes = false;
    $scope.firstEditComerciales = false;
    $scope.firstEditDireccionfacturacion = false;
    $scope.Loading = false;
    $scope.urlDescargaDefecto = "";
    $scope.urlcalcularinventario = "";
    $scope.Errorgeneral = "";
    $scope.init = function (urlDescargaDefecto,urlcalcularinventario)
    {
        $scope.urlDescargaDefecto = urlDescargaDefecto;
        $scope.urlcalcularinventario = urlcalcularinventario;
    }

    $scope.Calcularinventario = function () {
        

        if($scope.ValidarDatos())
            $scope.EnviarDatos();
    }

    $scope.ValidarDatos = function() {
        $scope.Errorgeneral = "";
        var result = true;
        var codigoalmacen = $("#Fkalmacenes").val();
        if (!codigoalmacen || codigoalmacen == "") {
            $scope.Errorgeneral = "El campo Almacén es obligatorio";
            result = false;
        }
        return result;
    }

    $scope.EnviarDatos = function () {
        var datos = {};
        datos.Fkalmacenes = $("#Fkalmacenes").val();
        datos.Tipodealmacenlote = $("#Tipodealmacenlote").val();
        datos.Fkalmaceneszonas = $("#Fkalmaceneszonas").val();
        datos.Fkarticulosdesde = $("#Fkarticulosdesde").val();
        datos.Fkarticuloshasta = $("#Fkarticuloshasta").val();
        datos.Fkfamiliamaterial = $("#Fkfamiliamaterial").val();
        datos.Fkfamiliaproductodesde = $("#Fkfamiliaproductodesde").val();
        datos.Fkfamiliaproductohasta = $("#Fkfamiliaproductohasta").val();
        datos.Fkmaterialdesde = $("#Fkmaterialdesde").val();
        datos.Fkmaterialhasta = $("#Fkmaterialhasta").val();
        datos.Fkcaracteristicadesde = $("#Fkcaracteristicadesde").val();
        datos.Fkcaracteristicahasta = $("#Fkcaracteristicahasta").val();
        datos.Fkgrosordesde = $("#Fkgrosordesde").val();
        datos.Fkgrosorhasta = $("#Fkgrosorhasta").val();
        datos.Fkacabadodesde = $("#Fkacabadodesde").val();
        datos.Fkacabadohasta = $("#Fkacabadohasta").val();
        $scope.Loading = true;
        var req = {
            method: 'POST',
            url: $scope.urlcalcularinventario,
            data: JSON.stringify(datos),
            contentType: 'application/json',
            dataType: 'json'
        }
        $http(req).success(function (response) {
            $scope.Loading = false;
            $('.nav-tabs a[href="#desglose"]').tab('show');

        }).error(function (data, status, headers, config) {
            $scope.Errorgeneral = data;
            $scope.Loading = false;
               });
    }
    
}]);