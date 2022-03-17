$(this).ready(function () {
    var descripcionOrden = [];
    var descripcion2Orden = [];
    var descripcionabreviadaOrden = [];
    var familia;
    var materiales;
    var caracteristicas;
    var grosores;
    var acabados;
    var clasificacionArticulo;
    var SumaLargosComponentes;

    $("[name='Labor']").change(function () {
        $("[name='Fklabores']").prop('disabled', !this.checked);
        if (!this.checked) {
            $("[name='Fklabores']").val("");
        }
    });


    //registrar eventos
    eventAggregator.RegisterEvent("Familia-cv", function (message) {

        descripcionOrden = [];
        descripcion2Orden = [];
        descripcionabreviadaOrden = [];

        console.log(message);

        if (message) {

            CalculaCodigo();
            familia = message;

            descripcionOrden = familia.Descripcion1generada ? familia.Descripcion1generada.split(",") : [];
            descripcion2Orden = familia.Descripcion2generada ? familia.Descripcion2generada.split(",") : [];
            descripcionabreviadaOrden = familia.Descripcionabreviadagenerada ? familia.Descripcionabreviadagenerada.split(',') : [];

            clasificacionArticulo = familia.Clasificacion;
            console.log(clasificacionArticulo);
            
            eventAggregator.Publish("Fkgruposiva-Buscar", message.Fkgruposiva ? message.Fkgruposiva : "");
            eventAggregator.Publish("Fkguiascontables-Buscar", message.Fkguiascontables ? message.Fkguiascontables : "");
            eventAggregator.Publish("Fkunidades-Buscar", message.Fkunidadesmedida ? message.Fkunidadesmedida : "");
            eventAggregator.Publish("Fkcontadores-Buscar", message.Fkcontador ? message.Fkcontador : "");

            $("[name='Gestionarcaducidad']").prop('checked', message.Gestionarcaducidad);
            /*$("[name='Existenciasminimasmetros']").val(message.Existenciasminimasmetros);
            $("[name='Existenciasmaximasmetros']").val(message.Existenciasmaximasmetros);
            $("[name='Existenciasminimasunidades']").val(message.Existenciasminimasunidades);
            $("[name='Existenciasmaximasunidades']").val(message.Existenciasmaximasunidades);*/
            $("[name='Stockseguridad']").val(message.Stockseguridad);
            $("[name='Stockmaximo']").val(message.Stockmaximo);
            $("[name='Stockminimo']").val(message.Stockminimo);
            $("[name='Categoria']").val(message.Categoria);
            $("[name='Gestionstock']").prop('checked', message.Gestionstock);
            $("#Tipogestionlotes").val(message.Tipogestionlotes);
            $("[name='Stocknegativoautorizado']").prop('checked', message.Stocknegativoautorizado);
            $("[name='Lotefraccionable']").prop('checked', message.Lotefraccionable);
            $("[name='Web']").prop('checked', message.Web);
            $("[name='Gestionstock']").prop('disabled', message.Tipofamilia > 0);
            $("#Tipogestionlotes").prop('disabled', message.Tipofamilia > 0);
            $("[name='Stocknegativoautorizado']").prop('disabled', message.Tipofamilia > 0);
            $("[name='Editarancho']").prop('checked', message.Editarancho);
            $("[name='Editarlargo']").prop('checked', message.Editarlargo);
            $("[name='Editargrueso']").prop('checked', message.Editargrueso);
            $("[name='Clasificacion']").val(message.Clasificacion);
            $("[name='Articulonegocio']").prop('checked', message.Articulonegocio);
            
            var permitecomentario = !message.Gestionstock && message.Tipofamilia == 3;
            $("[name='Articulocomentariovista']").prop('disabled', !permitecomentario);
            
            CalculaTodasDescripciones(descripcionOrden, descripcion2Orden, descripcionabreviadaOrden, familia, materiales, caracteristicas, grosores, acabados);
        }
    });

    eventAggregator.RegisterEvent("Materiales-cv", function (message) {
        if (message) {
            CalculaCodigo();
            materiales = message;
            $("#Fkgruposmateriales").val(message.Fkgruposmateriales);
            CalculaTodasDescripciones(descripcionOrden, descripcion2Orden, descripcionabreviadaOrden, familia, materiales, caracteristicas, grosores, acabados);
        }
    });

    eventAggregator.RegisterEvent("Caracteristicas-cv", function (message) {
        if (message)
            CalculaCodigo();
        caracteristicas = message;
        CalculaTodasDescripciones(descripcionOrden, descripcion2Orden, descripcionabreviadaOrden, familia, materiales, caracteristicas, grosores, acabados);
    });

    eventAggregator.RegisterEvent("Grosores-cv", function (message) {
        if (message) {
            CalculaCodigo();
            grosores = message;
            $("[name='Grueso']").val(message.Grosor);
            CalculaTodasDescripciones(descripcionOrden, descripcion2Orden, descripcionabreviadaOrden, familia, materiales, caracteristicas, grosores, acabados);
        }

    });

    eventAggregator.RegisterEvent("Acabados-cv", function (message) {
        if (message) {
            CalculaCodigo();
            acabados = message;
            CalculaTodasDescripciones(descripcionOrden, descripcion2Orden, descripcionabreviadaOrden, familia, materiales, caracteristicas, grosores, acabados);
        }

    });

});


var ResetCodigo = function () {
    $("#Id").val("");
}

var CalculaCodigo = function () {
    var codigo = $("#Familia").val()
        + $("#Materiales").val()
        + $("#Caracteristicas").val()
        + $("#Grosores").val()
        + $("#Acabados").val()
        + $("[name='Codigolibre']").val();
    $("#Id").val(codigo);
    eventAggregator.Publish("CodigoArticulo",codigo);
}

var CalculaTodasDescripciones = function (vector1, vector2, vector3, familia, materiales, caracteristicas, grosores, acabados) {
    $("#Descripcion").val(CalculaDescripcion(vector1, "Descripcion", familia, materiales, caracteristicas, grosores, acabados));
    $("#Descripcion2").val(CalculaDescripcion(vector2, "Descripcion2", familia, materiales, caracteristicas, grosores, acabados));
    $("#Descripcionabreviada").val(CalculaDescripcion(vector3, "Descripcionabreviada", familia, materiales, caracteristicas, grosores, acabados));
}

var CalculaDescripcion = function (vector, nameproperty,familia,materiales,caracteristicas,grosores,acabados) {
    var descripcion = "";
    for (var i = 0; i < vector.length; i++) {

        if (vector[i] === "0") {
            if (familia)
                descripcion += familia[nameproperty] + " ";
        }
        else if (vector[i] === "1" ) {
            if (materiales)
                descripcion += materiales[nameproperty] + " ";
        }
        else if (vector[i] === "2") {
            if (caracteristicas)
                descripcion += caracteristicas[nameproperty] + " ";
        }
        else if (vector[i] === "3") {
            if (grosores)
                descripcion += grosores[nameproperty] + " ";
        }
        else if (vector[i] === "4" ) {
            if (acabados)
                descripcion += acabados[nameproperty] + " ";
        }
        else if (vector[i] === "5") {

        }

    }

    return descripcion;

}






app.controller('ArticulosCtrl', ['$scope', '$http', '$location', '$window', '$timeout', function ($scope, $http, $location, $window, $timeout) {
    $scope.Preciomateriaprima = 0.0;
    $scope.Porcentajemerma = 0.0;
    $scope.Costemateriaprima = 0.0;
    $scope.Costeelaboracionmateriaprima = 0.0;
    $scope.Costeportes = 0.0;
    $scope.Otroscostes = 0.0;
    $scope.Costefabricacion = 0.0;
    $scope.Costeindirecto = 0.0;
    $scope.Coste = 0.0;
    
    $scope.Validarmaterial = true;
    $scope.Validarcaracteristica = true;
    $scope.Validargrosor = true;
    $scope.Validaracabado = true;

    $scope.Tamañocodigoarticulo = 11;
    $scope.Tamañocodigofamilia = 2;
    $scope.Tamañocodigomaterial = 3;
    $scope.Tamañocodigocaracteristica = 2;
    $scope.Tamañocodigogrosor = 2;
    $scope.Tamañocodigoacabado = 2;
    $scope.Tamañocodigolibre = 0;

    $scope.Mostrarcodigolibre = false;
    $scope.Codigolibre = "";

    var CalculoCostemateriaprima = function () {
        $scope.Costemateriaprima = (parseFloat($scope.Preciomateriaprima) + (parseFloat($scope.Preciomateriaprima) * ($scope.Porcentajemerma / 100.0))).toFixed(2);
    }

    var CalculoCosteTotal =function() {
        $scope.Coste =( parseFloat($scope.Costemateriaprima)
            + parseFloat($scope.Costeelaboracionmateriaprima)
            + parseFloat($scope.Costeportes)
            + parseFloat($scope.Otroscostes)
            + parseFloat($scope.Costefabricacion)
            + parseFloat($scope.Costeindirecto)).toFixed(2);
    }

    $scope.init = function (Preciomateriaprima, Porcentajemerma, Costemateriaprima
        , Costeelaboracionmateriaprima, Costeportes, Otroscostes
        , Costefabricacion, Costeindirecto
        , Coste, Codigolibre,
        Validarmaterial, Validarcaracteristica, Validargrosor, Validaracabado, Mostrarcodigolibre) {
        $scope.Preciomateriaprima = Preciomateriaprima;
        $scope.Porcentajemerma = Porcentajemerma;
        $scope.Costemateriaprima = Costemateriaprima;
        $scope.Costeelaboracionmateriaprima = Costeelaboracionmateriaprima;
        $scope.Costeportes = Costeportes;
        $scope.Otroscostes = Otroscostes;
        $scope.Costefabricacion = Costefabricacion;
        $scope.Costeindirecto = Costeindirecto;
        $scope.Coste = Coste;
        $scope.Codigolibre = Codigolibre;
        $scope.Validarmaterial = Validarmaterial;
        $scope.Validarcaracteristica = Validarcaracteristica;
        $scope.Validargrosor = Validargrosor;
        $scope.Validaracabado = Validaracabado;
        $scope.Mostrarcodigolibre = Mostrarcodigolibre;
    }

    $scope.$watch('Preciomateriaprima', function () {
        CalculoCostemateriaprima();
    });

    $scope.$watch('Porcentajemerma', function () {
        CalculoCostemateriaprima();
    });

    $scope.$watch('Costemateriaprima', function () {
        CalculoCosteTotal();
    });

    $scope.$watch('Costeelaboracionmateriaprima', function () {
        CalculoCosteTotal();
    });

    $scope.$watch('Costeportes', function () {
        CalculoCosteTotal();
    });

    $scope.$watch('Otroscostes', function () {
        CalculoCosteTotal();
    });

    $scope.$watch('Costefabricacion', function () {
        CalculoCosteTotal();
    });

    $scope.$watch('Costeindirecto', function () {
        CalculoCosteTotal();
    });

    eventAggregator.RegisterEvent("Familia-cv", function(message) {
        
        $scope.Validarmaterial = message.Validarmaterial;
        $scope.Validarcaracteristica = message.Validarcaracteristica;
        $scope.Validargrosor = message.Validargrosor;
        $scope.Validaracabado = message.Validaracabado;
        $scope.Mostrarcodigolibre = message.Tipofamilia == "3";

        $scope.Tamañocodigolibre = $scope.Tamañocodigoarticulo - ($scope.Tamañocodigofamilia + 
            ($scope.Validarmaterial ? $scope.Tamañocodigomaterial : 0) +
            ($scope.Validarcaracteristica ? $scope.Tamañocodigocaracteristica : 0 )+
            ($scope.Validargrosor ? $scope.Tamañocodigogrosor : 0) +
            ($scope.Validaracabado ? $scope.Tamañocodigoacabado : 0 ));

    });

    $scope.ValidarCodigo =function() {
        var frellenaCod = new FRellenacod();
        var relleacodService = frellenaCod.CreateRellenacod($scope.Tamañocodigolibre, TipoRellenacod.Generico);
        $scope.Codigolibre = relleacodService.Formatea($scope.Codigolibre);


        var codigoTotal = "";
        codigoTotal = $("#Familia").val()
            + ($scope.Validarmaterial ? $("#Materiales").val() : "")
            + ($scope.Validarcaracteristica ? $("#Caracteristicas").val() : "")
            + ($scope.Validargrosor ? $("#Grosores").val() : "")
            + ($scope.Validaracabado ? $("#Acabados").val() : "")
            + ($scope.Mostrarcodigolibre ? $scope.Codigolibre : "");

        $("#Id").val(codigoTotal);

        eventAggregator.Publish("CodigoArticulo", codigoTotal);
    }

    

    

}]);