app.controller('DireccionesCtrl', ['$scope', '$rootScope', '$http', '$interval',  function($scope, $rootScope, $http, $interval) {

    $scope.controlId = "";
    $scope.empresa = "";
    $scope.tipotercero = "";
    $scope.fkentidad = "";
    $scope.fkpaisdefecto = "";
    $scope.urlprovincias = "";
    $scope.provincias = [];
    var counter = -1;
    //nuevo item
    $scope.errores =
    {
        descripcionerror: "",
        direccionerror: "",
        poblacionerror: "",
        cperror: "",
        personacontactoerror: "",
        telefonoerror: "",
        telefonomovilerror: "",
        faxerror: "",
        emailerror: "",
        weberror: "",
    }
    $scope.nuevo =
    {
        Defecto: false,
        //descripcion
        Descripcion: Messages.DireccionPrincipal,
        
        Fkpais: "049",
        Fkprovincia:"",
        Fktipovia: "CL",
        //direccion
        Direccion: "",
       
        //poblacion
        Poblacion: "",
       
        //cp
        Cp:"",
        
        //personacontacto
        Personacontacto: "",
        
        //telefono
        Telefono: "",
        
        Telefonomovil: "",
        
        //fax
        Fax: "",
        
        //email
        Email: "",
        
        //web
        Web: "",
        
        //notas
        Notas: ""
        

    };

    $scope.direcciones = [];
    
    $scope.clear = function() {
        $scope.nuevo =
    {
       
        Id:--counter,
        Defecto: false,
        //descripcion
        Descripcion: Messages.DireccionPrincipal,
        descripcionerror: "",
        Fktipodireccion:"",
        Fkpais: $scope.fkpaisdefecto,
        Fkprovincia: "",
        Fktipovia: "CL",
        //direccion
        Direccion: "",
        direccionerror: "",
        //poblacion
        Poblacion: "",
        poblacionerror: "",
        //cp
        Cp: "",
        cperror: "",
        //personacontacto
        Personacontacto: "",
        personacontactoerror: "",
        //telefono
        Telefono: "",
        telefonoerror: "",
        Telefonomovil: "",
        telefonomovilerror: "",
        //fax
        Fax: "",
        faxerror: "",
        //email
        Email: "",
        emailerror: "",
        //web
        Web: "",
        weberror: "",
        //notas
        Notas: ""
    };
        $scope.nuevo.Tipotercero = $scope.tipotercero;
    }

    $scope.init = function (controlId, empresa, tipotercero, direcciones, urlprovincias, fkpaisdefecto) {
        $scope.controlId = controlId;
        $scope.empresa = empresa;
        $scope.tipotercero = tipotercero;
        $scope.direcciones = direcciones;
        $scope.urlprovincias = urlprovincias;
        $scope.fkpaisdefecto = fkpaisdefecto;
        counter = direcciones ? (direcciones.length + 1) * -1 : -1;
        $("form").submit(function(event) {
            for (var i = 0; i < $scope.direcciones.length; i++) {
                $scope.direcciones[i].Fkentidad = $("#Fkcuentas").val();
            }
        });
    }

    $scope.nuevoRegistro = function () {
        $scope.clear();
        var paiscliente = $("[name='Cuentas.FkPais']").val();
        if (paiscliente && paiscliente != "") {
            $scope.nuevo.Fkpais = paiscliente;
        }
        $("#" + $scope.controlId).modal();
    }
    $scope.setDefecto=function(item) {
        //establecemos un defecto
        if (item.Defecto) {
            for (var i = 0; i < $scope.direcciones.length; i++) {
                if (!($scope.direcciones[i].Empresa == item.Empresa &&
                    $scope.direcciones[i].Tipotercero == item.Tipotercero &&
                    $scope.direcciones[i].Fkentidad == item.Fkentidad &&
                    $scope.direcciones[i].Id == item.Id)) {
                    return $scope.direcciones[i].Defecto = false;
                }
            }
        } else {
            var existeDefecto = false;
            for (var i = 0; i < $scope.direcciones.length; i++) {
                if ($scope.direcciones[i].Defecto) {
                    existeDefecto = true;
                    break;
                }
            }

            if (!existeDefecto && $scope.direcciones.length > 0)
                return $scope.direcciones[0].Defecto = true;
            else if ($scope.direcciones.length==0) {
                item.Defecto = true;
            }
        }
    }
    $scope.validateItem = function(item) {
        $scope.setDefecto(item);

        $scope.nuevo.telefonoerror = "";
        $scope.nuevo.telefonomovilerror = "";
        //aqui pondremos las reglas necesarias
        if ($scope.nuevo.Telefono && $scope.nuevo.Telefono.length > 15) {
            $scope.nuevo.telefonoerror = "La longitud máxima del teléfono es de 15";
            return false;
        }
        if ($scope.nuevo.Telefonomovil && $scope.nuevo.Telefonomovil.length > 15) {
            $scope.nuevo.telefonomovilerror = "La longitud máxima del móvil es de 15";
            return false;
        }
        return true;
    }

    $scope.saveItem = function() {
        //todo validar
        if ($scope.validateItem($scope.nuevo)) {
            var item = $scope.searchItem($scope.nuevo.Empresa, $scope.nuevo.Tipotercero, $scope.nuevo.Fkentidad, $scope.nuevo.Id);
            if (item == undefined)
                $scope.direcciones.push($scope.nuevo);
            else {
                var index = $scope.direcciones.indexOf(item);
                if(index>=0)
                $scope.direcciones[index] = $scope.nuevo;
            }
            $("#" + $scope.controlId).modal('hide');
        }
        
    }

    $scope.edit = function(empresa, tipotercero, fkentidad, id) {
        var item = $scope.searchItem(empresa, tipotercero, fkentidad, id);
        $scope.nuevo =  $.parseJSON(JSON.stringify(item));;
        $("#" + $scope.controlId).modal();
    }
    $scope.delete = function (empresa, tipotercero, fkentidad, id) {
       
            bootbox.confirm(Messages.EliminarRegistro, function (result) {
                if (result) {
                    var item = $scope.searchItem(empresa, tipotercero, fkentidad, id);
                    var index = $scope.direcciones.indexOf(item);
                    if (index > -1) {
                        $scope.$apply(function () {
                            var eraDefecto = item.Defecto;
                            $scope.direcciones.splice(index, 1);
                            if (eraDefecto && $scope.direcciones.length > 0) {
                                $scope.direcciones[0].Defecto = true;
                            }
                            
                        });
                    }
                }
            });
    }

    $scope.searchItem = function(empresa, tipotercero, fkentidad, id) {
        for (var i = 0; i < $scope.direcciones.length; i++) {
            if ($scope.direcciones[i].Empresa == empresa &&
                $scope.direcciones[i].Tipotercero == tipotercero &&
                $scope.direcciones[i].Fkentidad == fkentidad &&
                $scope.direcciones[i].Id == id) {
                return $scope.direcciones[i];
            }
        }
    };

    $scope.$watch('nuevo.Fkpais', function () {
        $http({
            url: $scope.urlprovincias,
            method: "GET",
            params: { codigopais: $scope.nuevo.Fkpais }
        })
          .success(function (response) {
                $scope.provincias = response.values;
          }).error(function (data, status, headers, config) {
                $scope.Fkprovincia = "";
                $scope.provincias = [];
          });
    });

}]);
