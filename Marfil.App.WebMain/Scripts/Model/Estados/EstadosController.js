$(document).ready(function () {

  
    setEstado();

    $("[name='Tipoestado']").change(function () {
        setEstado();

    });


});

var setEstado = function() {
    if ($("[name='Tipoestado']").val() <= "1") {
        $("#permiteeditar").show();
        $("#nopermiteeditar").hide();
    } else {
        $("#permiteeditar").hide();
        $("#nopermiteeditar").show();
    }
}