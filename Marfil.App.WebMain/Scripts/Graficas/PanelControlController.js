app.controller('PanelControlCtrl', ['$scope', '$rootScope', '$http', '$interval',  '$timeout', function($scope, $rootScope, $http, $interval, $timeout) {

    $scope._urlorden = "";
    $scope._urlvisualizar = "";
    $scope._data = [];
    $scope.init = function (urlorden,urlvisualizar) {
        $scope._urlorden = urlorden;
        $scope._urlvisualizar = urlvisualizar;

        $(document).ready(function() {
            $("#mainbody").removeClass("col-md-offset-1 col-md-10");
            jQuery(function ($) {
                var panelList = $('#draggablePanelList');

                panelList.sortable({
                    // Only make the .panel-heading child elements support dragging.
                    // Omit this to make then entire <li>...</li> draggable.
                    handle: '.panel-heading',
                    update: function () {
                        var datos = [];
                        $('li', panelList).each(function (index, elem) {
                            var $listItem = $(elem);
                            datos.push({ Grafica: $listItem.attr("tag"), Indice: $listItem.index() });
                        });
                        $scope.Guardarpanel(datos);
                    }
                });
            });
        });

    }

    $scope.VisualizarGrafica = function(id, titulo) {
        $('#modalgraficas').on('shown.bs.modal', function () {
            $('#modalgraficas iframe').attr("src", $scope._urlvisualizar + '/?id=' + id);
            $('#titulografica').html(titulo);
        });
        $("#modalgraficas").modal('show');
    }

    $scope.Guardarpanel = function (datos) {
        
        waitingDialog.show('Guardando preferencias...', { dialogSize: 'sm' });
        $http({
            url: $scope._urlorden, method: "POST", params: { model: JSON.stringify(datos) }
        })
          .success(function (response) {

                waitingDialog.hide();
            }).error(function (data, status, headers, config) {

                $scope.Loading = false;
          });
    }

}]);