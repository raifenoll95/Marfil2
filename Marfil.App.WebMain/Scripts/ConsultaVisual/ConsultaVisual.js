//Raimundo Fenoll Albaladejo. Consulta Visual Controladores Script

//Almacenes Ctrl
app.controller('AlmacenesCtrl', ['$scope', '$rootScope', '$http', '$location', '$window', '$timeout', function ($scope, $rootScope, $http, $location, $window, $timeout) {

    //Atributos de momento
    $scope.myobj;
    $scope.almacenes;
    $scope.almacenesAux;
    $scope.urlImageAux;
    $scope.AlmacenSeleccionado;
    $scope.AlmacenDescripcion;
    $scope.AlmacenId;
    $scope.urlToFamilias;
    $scope.urlToGetAlmacenes;
    $scope.modelo;
    $scope.ordena;

    function sensitiveCompare(input, search) {
        return ('' + input).indexOf('' + search) > -1;
    }

    $scope.init = function (urlImagenDefecto, urlfamilias, urlalmacenes) {
        $scope.urlImageAux = urlImagenDefecto;
        $scope.urlToFamilias = urlfamilias;
        $scope.urlToGetAlmacenes = urlalmacenes;
        comprobarAlmacenesBD();    
    }

    $scope.compruebaCheck = function (almacen) {
        if ($scope.ordena == true) {
            console.log("si");
            return -almacen.numFamilias;
        }
    }

    //obtener almacenes
    function comprobarAlmacenesBD() {

        $scope.modelo = modeloalmacen;

        $http.get($scope.urlToGetAlmacenes + "?seleccion=" + JSON.stringify($scope.modelo)).success(function (data) {
            $scope.almacenes = JSON.parse(data);
            $scope.almacenesAux = JSON.parse(data);
        }).error(function (error) {
            console.log(error);
        });
    }

    //Seleccionar Almacen
    $scope.seleccionar = function(id,desc) {
        modeloalmacen.idAlmacen = id;
        modeloalmacen.DescAlmacen = desc;
        $scope.modelo = modeloalmacen;
        almacenes = $scope;
        window.location.href = $scope.urlToFamilias + "?seleccion=" + JSON.stringify($scope.modelo);
    }

    $scope.Getsrc = function (url, item) {

        console.log("url: " + url);
        console.log("id: " + item[0]);

        if (Array.isArray(item)) {

            if (item.length > 0) {
                console.log("id: " + item[0].Id);
                return url + item[0].Id;
            }
            else {
                return $scope.urlImageAux;
                console.log("img aux: " + $scope.urlImageAux);
            }
        }
    }
}]);


//Familias Ctrl
app.controller('FamiliasCtrl', ['$scope', '$rootScope', '$http', '$location', '$window', '$timeout', function ($scope, $rootScope, $http, $location, $window, $timeout) {

    //Atributos de momento
    $scope.idAlmacen;
    $scope.familias;
    $scope.DescripcionAlmacen;
    $scope.myobj;
    $scope.urlToGruposMateriales;
    $scope.urlToGetFamilias;
    $scope.modelo;
    $scope.ordena;
    $scope.urlImageAux;
   

    //init
    $scope.init = function (urlImagenDefecto,urlToMateriales, urlToGetFamilias) {
        $scope.urlImageAux = urlImagenDefecto;
        $scope.urlToGruposMateriales = urlToMateriales;
        $scope.urlToGetFamilias = urlToGetFamilias;
        comprobarFamiliasBD();
    }

    function sensitiveCompare(input, search) {
        return ('' + input).indexOf('' + search) > -1;
    }

    //Familias asociadas a ese almacen
    function comprobarFamiliasBD() {

        $scope.modelo = modelofamilias;

        var numCuenta = $.val//numerocuentacliente

        $http.get($scope.urlToGetFamilias + "?seleccion=" + JSON.stringify($scope.modelo)).success(function (data) {
            $scope.familias = JSON.parse(data);
        }).error(function (error) {
            console.log(error);
        });
    }

    //Seleccionar familia
    $scope.seleccionar = function (id,desc) {
        modelofamilias.idFamilia = id;
        modelofamilias.DescFamilia = desc;
        $scope.modelo = modelofamilias;
        window.location.href = $scope.urlToGruposMateriales + "?seleccion=" + JSON.stringify($scope.modelo);
    }

    $scope.Getsrc = function (url, item) {

        console.log("url: " + url);
        console.log("id: " + item[0]);

        if (Array.isArray(item)) {

            if (item.length > 0) {
                console.log("id: " + item[0].Id);
                return url + item[0].Id;
            }
            else {
                return $scope.urlImageAux;
                console.log("img aux: " + $scope.urlImageAux);
            }
        }
    }
}]);

//Grupo Materiales Ctrl
app.controller('GrupoMaterialesCtrl', ['$scope', '$rootScope', '$http', '$location', '$window', '$timeout', function ($scope, $rootScope, $http, $location, $window, $timeout) {

    //Atributos de momento
    $scope.myobj;
    $scope.FamiliaSeleccionada;
    $scope.DescripcionFamilia;
    $scope.grupoMateriales;
    $scope.urlImageAux;
    $scope.urlToMateriales;
    $scope.urlToGetMateriales;
    $scope.modelo;

    //init
    $scope.init = function (urlImagenDefecto, urlToProductos, urlToGetMateriales) {
        $scope.urlImageAux = urlImagenDefecto;
        $scope.urlToMateriales = urlToProductos;
        $scope.urlToGetMateriales = urlToGetMateriales;
        comprobarGruposMaterialesBD();
    }

    function sensitiveCompare(input, search) {
        return ('' + input).indexOf('' + search) > -1;
    }

    
    $scope.Getsrc = function (url, ficheros) {

        console.log("url: " + url);

        if (Array.isArray(ficheros)) {

            if (ficheros.length > 0) {
                return url + ficheros[0].Id;
                console.log("si");
            }          
            else {
                return $scope.urlImageAux;
                console.log("img aux: " + $scope.urlImageAux);
            }
        }
    }

    //Grupos de materiales
    function comprobarGruposMaterialesBD() {

        $scope.modelo = modelomateriales;

        $http.get($scope.urlToGetMateriales + "?seleccion=" + JSON.stringify($scope.modelo)).success(function (data) {
            $scope.grupoMateriales = JSON.parse(data);
        }).error(function (error) {
            console.log(error);
        });
    }

    //Seleccionar Almacen
    $scope.seleccionar = function (id,desc) {
        modelomateriales.idGrupoMateriales = id;
        modelomateriales.DescGrupoMateriales = desc;
        $scope.modelo = modelomateriales;
        window.location.href = $scope.urlToMateriales + "?seleccion=" + JSON.stringify($scope.modelo);
    }
}]);


//Productos Ctrl
app.controller('MaterialesCtrl', ['$scope', '$http', '$location', '$window', '$timeout', function ($scope, $http, $location, $window, $timeout) {

    //Atributos de momento
    $scope.myobj;
    $scope.GrupoMaterialSeleccionado;
    $scope.DescripcionGrupo;
    $scope.productos;
    $scope.urlImageAux;
    $scope.urlToLotes;
    $scope.urlToGetProductos;
    $scope.modelo;
    $scope.ordena;

    //init
    $scope.init = function (urlImagenDefecto, urlToProducto, urlToGetProductos) {
        $scope.urlImageAux = urlImagenDefecto;
        $scope.urlToLotes = urlToProducto;
        $scope.urlToGetProductos = urlToGetProductos;
        comprobarProductos();
    }

    function sensitiveCompare(input, search) {
        return ('' + input).indexOf('' + search) > -1;
    }

    $scope.compruebaCheck = function (articulos) {
        if ($scope.ordena == true) {
            console.log("si");
            return -articulos.numLotes;
        }
    }

    $scope.Getsrc = function (url, item) {

        console.log("url: " + url);
        console.log("id: " + item[0]);

        if (Array.isArray(item)) {

            if (item.length > 0) {
                console.log("id: " + item[0].Id);
                return url + item[0].Id;
            }
            else {
                return $scope.urlImageAux;
                console.log("img aux: " + $scope.urlImageAux);
            }
        }
    }

    //Comprobamos los productos
    function comprobarProductos() {

        $scope.modelo = modeloproductos;

        $http.get($scope.urlToGetProductos + "?seleccion=" + JSON.stringify($scope.modelo)).success(function (data) {
            $scope.productos = JSON.parse(data);
            console.log("data: " + data);
        }).error(function (error) {
            console.log(error);
        });
    }

    //Seleccionar articulos
    $scope.seleccionar = function (id, desc) {
        console.log("productos es: " + $scope.productos);
        modeloproductos.idMaterial = id;
        modeloproductos.DescMaterial = desc;
        $scope.modelo = modeloproductos;
        window.location.href = $scope.urlToLotes + "?seleccion=" + JSON.stringify($scope.modelo);
    }
}]);


//Articulos Ctrl
app.controller('LotesCtrl', ['$scope', '$http', '$location', '$window', '$timeout', function ($scope, $http, $location, $window, $timeout) {

    //Atributos de momento
    $scope.myobj;
    $scope.ArticuloSeleccionado;
    $scope.DescripcionArticulo;
    $scope.articulos;
    $scope.modelo;
    $scope.url;

    //init
    $scope.init = function (urlImagenDefecto,urlToGetLotes) {

        $scope.urlImageAux = urlImagenDefecto;
        $scope.url = urlToGetLotes;
        comprobarArticulos();
    }

    function sensitiveCompare(input, search) {
        return ('' + input).indexOf('' + search) > -1;
    }

    $scope.Getsrc = function (url, item) {

        console.log("url: " + url);

        if (Array.isArray(item)) {

            if (item.length > 0) {
                return url + item[0].Id;
            }
            else {
                return $scope.urlImageAux;
                console.log("img aux: " + $scope.urlImageAux);
            }
        }
    }

    //Comprobamos los productos
    function comprobarArticulos() {

        $scope.modelo = modeloproducto;

        $http.get($scope.url + "?seleccion=" + JSON.stringify($scope.modelo)).success(function (data) {
            $scope.articulos = JSON.parse(data);
        }).error(function (error) {
            console.log(error);
        });
    }
}]);