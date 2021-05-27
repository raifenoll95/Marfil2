app.controller('cuentasCtrl', ['$scope', '$http','$location','$window','$timeout', function ($scope, $http,$location,$window,$timeout) {
    $scope.maxLevel = 4;
    $scope.maxDigitos = 8;

    var operation = "";
    $scope.codigo = "";
    $scope.level = 0;
    $scope.$watch('codigo', function (newVal, oldVal) {
        $scope.level = newVal.length > $scope.maxLevel ? 0 : newVal.length;//TODO EL: Review maximum level, is parametrized
       
    }, true);

    $scope.getLevel = function(codigo) {
        return codigo.length > $scope.maxLevel ? 0 : codigo.length;
    }

    $scope.cloneDescripcion = function() {
        if ($("#Descripcion2").val() == "") {
            $("#Descripcion2").val($("#Descripcion").val());
        }
    };
    $scope.onItemFocus = function(obj) {
        if (!obj.Descripcion2||obj.Descripcion2=="")
            obj.Descripcion2 = obj.Descripcion;
    };

    $scope.currenttiposcuentas = "";
    $scope.supercuentas = [];
    $scope.tiposcuentas = [];
    $scope.urltiposcuentas = "";

    $scope.loadTiposcuentas = function () {
        if ($scope.codigo && $scope.scope !== "" && $scope.getLevel($scope.codigo) === 0 && $scope.urltiposcuentas!=="") {
            $http.get($scope.urltiposcuentas + "/" + $scope.codigo)
           .success(function (response) {
               $scope.tiposcuentas = response;
               if($scope.tiposcuentas.length===0)
                   $scope.tiposcuentas.splice(0, 0, { Value: "", Text: Messages.CuentaGeneral});

               if ($scope.currenttiposcuentas === "") {
                   $scope.currenttiposcuentas = $scope.tiposcuentas[0].Value.toString();
               }

                }).error(function (data, status, headers, config) {
               alert(status);
           });
        }
        
    }

    
    var validate = function (valor) {

        if (valor.length > $scope.maxDigitos)
            return false;

        if (valor.match("^[0-9]*\\.[0-9]*")) {
            vector = valor.split('.');
            if (vector.length === 2) {
                var totalZeros = $scope.maxDigitos - (vector[0].length + vector[1].length);
                var zeros = "";
                for (var i = 0; i < totalZeros; i++)
                    zeros += "0";
                $scope.codigo = vector[0] + zeros + vector[1];

            } else
                return false;
        } else if(valor.match("^[0-9]+")){
            var totalZeros = $scope.maxDigitos - valor.length;
            var zeros = "";
            for (var i = 0; i < totalZeros; i++)
                zeros += "0";
            $scope.codigo = valor + zeros;
        }

        return true;
    }
    $scope.clean=function() {
        
      
            $scope.codigo = null;
       
    }
    $scope.VerificarPK = function (urlSupercuentas, urlCuenta, urlRedirect) {

        if ($scope.codigo == "" || validate($scope.codigo) === false) {
            $scope.codigo = "";
            $("#id").focus();
            return;
        }
        $scope.loadTiposcuentas();
        $http.get(urlCuenta + "/" + $scope.codigo)
            .success(function (response) {
                if (response.Existe) {
                    var resultValue;
                    bootbox.confirm(Messages.EditarRegistroExistente, function (result) {
                        if (result) {
                            window.location = urlRedirect + "/" + $scope.codigo;
                        } else {
                            $timeout(function () {
                                var element = window.document.getElementById("id");
                                if (element)
                                    element.focus();
                                $scope.codigo = "";
                            }); 
                        }
                    });

                    
                }
                else {
                    $scope.getsupercuentas(urlSupercuentas);
                }

            }).error(function (data, status, headers, config) {
                alert(status);
            });

    }

    $scope.getsupercuentas = function (url) {
        if ($scope.codigo == "") {
            $scope.codigo = "";
            $("#id").focus();
            return;
        }

        //cargar super cuentas

        $http.get(url + "/" + $scope.codigo)
            .success(function (response) {
                $scope.supercuentas = response;

            }).error(function (data, status, headers, config) {
                $scope.supercuentas = [];
            });
    }

    $scope.init = function (value, url,urltiposcuentas, tipocuenta,maxlevel,maxdigitos) {
        $scope.codigo = value;
        $scope.currenttiposcuentas = tipocuenta;
        $scope.getsupercuentas(url);
        $scope.urltiposcuentas = urltiposcuentas;
        $scope.maxLevel = maxlevel;
        $scope.maxDigitos = maxdigitos;
        if (value && value !== "") {
            $scope.loadTiposcuentas();
        }
    }



}]);

var redirect = function (url, nombreentidad) {

    var mensaje = String.format(Messages.EditarTerceroExistente, nombreentidad);
    bootbox.confirm(mensaje, function (result) {
        if (result) {
            window.location = url;
        } else {
            window.location.reload();
        }
    });
}

function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode != 46 && charCode > 31
      && (charCode < 48 || charCode > 57))
        return false;

    return true;
}

function desbloquear(id) {
    bootbox.confirm({
						message: "Va a desbloquear la cuenta " + id + ",¿Desea continuar?",
						buttons: {
						  confirm: {
							 label: "Sí",
							 className: "btn-primary btn-sm",
						  },
						  cancel: {
							 label: "No",
							 className: "btn-sm",
						  }
						},
						callback: function(result) {
						    if (result)
						        alert("desbloqueando");
						}
					  }
					);
}

