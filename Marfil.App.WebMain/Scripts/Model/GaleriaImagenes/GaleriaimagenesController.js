app.controller('GaleriaimagenesCtrl', ['$scope', '$rootScope', '$http', '$interval', function ($scope, $rootScope, $http, $interval) {

    $scope.Empresa = "";
    $scope.Parentfolder = "";
    $scope.Ficheros = [];
    $scope.Urlficheroslist = "";
    $scope.Urlficherosadd = "";
    $scope.Urlficherosremove = "";
    $scope.Subiendoarchivo = false;

    //Empresa, carpeta del padre, url listado, añadir y eliminar
    $scope.init = function (empresa, parentfolder, urlficheroslist, urlficherosadd, urlficherosremove) {

        $scope.Empresa = empresa;
        $scope.Parentfolder = parentfolder;
        $scope.Urlficheroslist = urlficheroslist;
        $scope.Urlficherosadd = urlficherosadd;
        $scope.Urlficherosremove = urlficherosremove;

        $scope.CargarFicheros();
    }

    $scope.CargarFicheros = function (item) {

        $http({
            url: $scope.Urlficheroslist,
            method: "POST",
            params: { id: $scope.Parentfolder }
        })
        .success(function (response) {
            $scope.Ficheros = response;
        }).error(function (data, status, headers, config) {
            $scope.Ficheros = [];
        });

    }

    $scope.EliminarFicheros = function (item) {

        console.log("elimina ficheros");

        bootbox.confirm(Messages.EliminarRegistro, function (result) {
            if (result) {
                $http({
                    url: $scope.Urlficherosremove,
                    method: "POST",
                    params: { id: item.Id }
                })
         .success(function (response) {
             var index = $scope.Ficheros.indexOf(item);
             if (index >= 0)
                 $scope.Ficheros.splice(index, 1);
         }).error(function (data, status, headers, config) {

         });
            }
        });


    }

    $scope.AñadirFicheros = function (input, DirectorioId) {
        $scope.Subiendoarchivo = true;
        var request = new XMLHttpRequest();
        var nuevoFormulario = new FormData();
        
        for (var x = 0; x < input.files.length; x++) {
            nuevoFormulario.append("file[]", input.files[x]);
        }
        nuevoFormulario.append('directorioid', DirectorioId);
        request.onreadystatechange = function () {
            if (request.readyState == 4 && request.status == 200) {
                $scope.CargarFicheros();
                $scope.Subiendoarchivo = false;
            } else if (request.readyState == 400 || request.readyState == 500) {
                $scope.Subiendoarchivo = false;
            }
        }
        request.open("POST", $scope.Urlficherosadd);
        request.send(nuevoFormulario);
    }

    $scope.Preventclick = function (item, event) {
        if (!item.Esimagen)
            event.stopPropagation();
    }

    $scope.Getsrc = function (url, item) {

        console.log("item galeria: " + item);

        if (item.Esimagen)
            return url + item.Id;
        return item.Imagenauxiliar;
    }

}]);