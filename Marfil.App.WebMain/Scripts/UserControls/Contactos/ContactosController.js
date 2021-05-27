app.controller('ContactosCtrl', ['$scope', '$rootScope', '$http', '$interval',  function($scope, $rootScope, $http, $interval) {

    $scope.controlId = "";
    $scope.empresa = "";
    $scope.tipotercero = "";
    $scope.fkentidad = "";
    $scope.contactos = [];
    $scope.cargosempresas = [];

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
        nifciferror:""
}
    $scope.nuevo =
    {
        
        //descripcion
        Nombre: Messages.ContactoPrincipal,
        Fktipocontacto: "",
        Fkcargoempresa: "",
        Fkidioma: "",
        Fkid_direccion: "",
        Telefono: "",
        Telefonomovil: "",
        Fax: "",
        Email: "",
        Nifcif: "",
        Observaciones:""
    };
    
    $scope.clear = function() {
        $scope.nuevo =
    {
        Empresa:$scope.empresa,
        Tipotercero:$scope.tipotercero,
        Fkentidad:$scope.fkentidad,
        Id:--counter,
        Nombre: Messages.ContactoPrincipal,
        Fktipocontacto: "",
        Fkcargoempresa: "",
        Fkidioma: "",
        Fkid_direccion: "",
        Telefono: "",
        Telefonomovil: "",
        Fax: "",
        Email: "",
        Nifcif: "",
        Observaciones: ""
    };
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
         nifciferror: ""
     }
    }

    $scope.init = function (controlId, empresa, tipotercero, contactos,cargosempresas) {
        $scope.controlId = controlId;
        $scope.empresa = empresa;
        $scope.tipotercero = tipotercero;
        
        $scope.contactos = contactos;
        $scope.cargosempresas = cargosempresas;
        counter = contactos ? (contactos.length + 1) * -1 : -1;

        $("form").submit(function(event) {
            for (var i = 0; i < $scope.contactos.length; i++) {
                $scope.contactos[i].Fkentidad = $("#Fkcuentas").val();
            }
        });
    }

    $scope.nuevoRegistro = function () {
        $scope.clear();
        $("#" + $scope.controlId).modal();
    }
    $scope.setDefecto=function(item) {
        //establecemos un defecto
        if (item.Defecto) {
            for (var i = 0; i < $scope.contactos.length; i++) {
                if (!($scope.contactos[i].Empresa == item.Empresa &&
                    $scope.contactos[i].Tipotercero == item.Tipotercero &&
                    $scope.contactos[i].Fkentidad == item.Fkentidad &&
                    $scope.contactos[i].Id == item.Id)) {
                    return $scope.contactos[i].Defecto = false;
                }
            }
        } else {
            var existeDefecto = false;
            for (var i = 0; i < $scope.contactos.length; i++) {
                if ($scope.contactos[i].Defecto) {
                    existeDefecto = true;
                    break;
                }
            }

            if (!existeDefecto && $scope.contactos.length > 0)
                return $scope.contactos[0].Defecto = true;
            else if ($scope.contactos.length == 0) {
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

    $scope.nifobligatorio = function() {
        for (var i = 0; i < $scope.cargosempresas.length; i++) {
            if ($scope.cargosempresas[i].Valor == $scope.nuevo.Fkcargoempresa) {
                return $scope.cargosempresas[i].NifObligatorio;
            }
        }
        return false;
    }

    $scope.saveItem = function() {
        //todo validar
        if ($scope.validateItem($scope.nuevo)) {
            var item = $scope.searchItem($scope.nuevo.Empresa, $scope.nuevo.Tipotercero, $scope.nuevo.Fkentidad, $scope.nuevo.Id);
            if (item == undefined)
                $scope.contactos.push($scope.nuevo);
            else {
                var index = $scope.contactos.indexOf(item);
                if(index>=0)
                    $scope.contactos[index] = $scope.nuevo;
            }
            $("#" + $scope.controlId).modal('hide');
            $scope.clear();
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
                    var index = $scope.contactos.indexOf(item);
                    if (index > -1) {
                        $scope.$apply(function () {
                            var eraDefecto = item.Defecto;
                            $scope.contactos.splice(index, 1);
                            if (eraDefecto && $scope.contactos.length > 0) {
                                $scope.contactos[0].Defecto = true;
                            }
                            
                        });
                    }
                }
            });
    }

    $scope.searchItem = function(empresa, tipotercero, fkentidad, id) {
        for (var i = 0; i < $scope.contactos.length; i++) {
            if ($scope.contactos[i].Empresa == empresa &&
                $scope.contactos[i].Tipotercero == tipotercero &&
                $scope.contactos[i].Fkentidad == fkentidad &&
                $scope.contactos[i].Id == id) {
                return $scope.contactos[i];
            }
        }
    };

}]);
