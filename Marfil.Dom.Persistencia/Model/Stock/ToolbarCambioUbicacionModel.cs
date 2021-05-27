using Marfil.Dom.ControlsUI.Toolbar;
using RMovimientosalmacen = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Movimientosalmacen;
using RStock = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Stock;
namespace Marfil.Dom.Persistencia.Model.Stock
{
    internal class ToolbarCambioUbicacionModel : ToolbarModel
    {
        public ToolbarCambioUbicacionModel()
        {
            Operacion = TipoOperacion.Custom;
            Titulo = RMovimientosalmacen.CambioUbicacion;
            CustomAction = true;
            CustomActionName = "CambioUbicacion";
        }

        public override string GetCustomTexto()
        {
            return RMovimientosalmacen.RealizarCambioUbicacion;
        }
    }
}