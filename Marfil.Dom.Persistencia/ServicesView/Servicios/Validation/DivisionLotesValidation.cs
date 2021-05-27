using Marfil.Dom.Persistencia.Helpers;
using Marfil.Inf.Genericos.Helper;
using Resources;
using System;
using System.Linq;
using RDivisionLotes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.DivisionLotes;
using RAlbaranesCompras = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.AlbaranesCompras;
using RAlbaranes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Albaranes;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Documentos.DivisionLotes;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class DivisionLotesValidation : BaseValidation<DivisionLotes>
    {
        public bool CambiarEstado { get; set; }
        public string EjercicioId { get; set; }

        #region CONSTRUCTOR
        public DivisionLotesValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        #endregion
        

        public override bool ValidarGrabar(DivisionLotes model)
        {
            if (!CambiarEstado)
            {
                ValidarCabecera(model);
                ValidarLineas(model);
                return base.ValidarGrabar(model);
            }
            else
            {
                ValidarCabecera(model);
                ValidarLineas(model);
            }

            return true;

        }

        //Solo se permite 1 salida
        public void ValidarSalidas(DivisionLotesModel model)
        {
            if(model.LineasSalida.Count>1)
            {
                model.LineasSalida.Clear();
                model.LineasEntrada.Clear();
                throw new ValidationException("Sólo se puede generar división de lotes para una salida");
            }
        }

        //Validamos las medidas de las entradas respecto a la salida
        public void ValidarEntradas(DivisionLotesModel model)
        {

            //Obtenemos los valores de las salidas inicialmente
            var largoSalida = model.LineasSalida.First().Largo;
            var anchoSalida = model.LineasSalida.First().Ancho;
            var gruesoSalida = model.LineasSalida.First().Grueso;
            var cantidadSalida = model.LineasSalida.First().Cantidad;
            var metrosSalida = model.LineasSalida.First().Metros;

            bool largoModificado = false;
            bool anchoModificado = false;
            bool gruesoModificado = false;
            bool cantidadModificada = false;

            foreach (var entrada in model.LineasEntrada)
            {
                if (entrada.Largo != largoSalida)
                    largoModificado = true;

                if (entrada.Ancho != anchoSalida)
                    anchoModificado = true;

                if (entrada.Grueso != gruesoSalida)
                    gruesoModificado = true;

                if (entrada.Cantidad != cantidadSalida)
                    cantidadModificada = true;
            }

            if (!largoModificado && !anchoModificado && !gruesoModificado)
            {
                if(!cantidadModificada)
                {
                    throw new ValidationException("No se han modificado las entradas");
                }
                
                if(cantidadModificada && (cantidadSalida != model.LineasEntrada.Sum(a => a.Cantidad.Value)))
                {
                    throw new ValidationException("La cantidad de las entradas no corresponde con la cantidad de la salida");
                } 
            }

            //Si modificamos el largo, ancho y grueso y ademas los metros cuadrado
            if(largoModificado || anchoModificado || gruesoModificado)
            {

                //La suma de los metros cuadrados (tabla) o cubicos (bloque) de las entradas no pueden ser superior a los de la salida 
                if(model.LineasEntrada.Sum(a=> a.Metros.Value) > metrosSalida)
                {
                    throw new ValidationException("Los metros de las entradas son superiores a la salida");
                }
            }
        }


        //VALIDAMOS LA PESTAÑA DEL GENERAL
        private void ValidarCabecera(DivisionLotes model)
        {

            //EL DOCUMENTO NO PUEDE SER NULO
            if (model.fechadocumento == null)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Fechadocumento));

            //EL ALMACEN NO PUEDE SER NULO (POR DEFECTO SE PONE A ALMACEN GENERAL) 
            if (string.IsNullOrEmpty(model.fkalmacen))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Fkalmacen));

            //DEBE HABER UN OPERARIO QUE REALICE LA OPERACION DE LA DIVISION 
            if (string.IsNullOrEmpty(model.fkoperarios))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Fkalmacen));

            //SI HAY ALGUNA ZONA QUE NO PERTENECE A NINGUN ALMACEN
            if (!string.IsNullOrEmpty(model.fkzonas))
            {
                var idInt = Funciones.Qint(model.fkzonas) ?? 0;
                if (!_db.AlmacenesZona.Any(
                    f => f.empresa == model.empresa && f.fkalmacenes == model.fkalmacen && f.id == idInt))
                    throw new ValidationException(string.Format(RAlbaranes.ErrorZonaAlmacen, model.fkzonas, model.fkalmacen));

            }

            model.integridadreferencialflag = Guid.NewGuid();
        }

        private void ValidarLineas(DivisionLotes model)
        {
            /*
            //TIENE QUE HABER ALGO EN LA SALIDA
            if (!model.DivisionLotessalidalin.Any())
                throw new ValidationException(string.Format(RDivisionLotes.ErrorLineasObligatorias, RDivisionLotes.Salida));

            //EL ESTADO NO PUEDE SER FINALIZADO
            var estado = _db.Estados.Single(f => f.documento + "-" + f.id == model.fkestados);
            if (!model.DivisionLotessalidalin.Any() && estado.tipoestado == (int)TipoEstado.Finalizado)
                throw new ValidationException(string.Format(RDivisionLotes.ErrorLineasObligatorias, RDivisionLotes.Entrada));
                */


            foreach (var item in model.DivisionLotessalidalin)
            {

                //DEBE HABER ARTICULOS
                if (string.IsNullOrEmpty(item.fkarticulos))
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Fkarticulos));
                
                //TIENE QUE HABER UNA CANTIDAD
                if (!item.cantidad.HasValue)
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Cantidad));

                //TIENE QUE TENER UN VALOR
                if (!item.metros.HasValue)
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Metros));

                //COMPROBAR SI ESE LOTE ES FRACCIONABLE
                var numeroFamiliaArticulo = item.fkarticulos.Substring(0, 2);
                var fraccionable = _db.Familiasproductos.Where(f => f.empresa == item.empresa && f.id == numeroFamiliaArticulo).Select(f => f.lotefraccionable).ToString();

                if (fraccionable=="1")
                {

                }

                else
                {

                }

                
            }

            String excepcionesGeneradas = "";

            //VERIFICAMOS LAS LINEAS DE LAS TRANSFORMACIONES DE ENTRADAS SON CORRECTAS
            foreach (var item in model.DivisionLotesentradalin)
            {

                bool hayEnStock = _db.Stockactual.Any(f => f.empresa == model.empresa && f.lote == item.lote && f.loteid == item.id.ToString());
                bool hayEnStockHistorico = _db.Stockhistorico.Any(f => f.empresa == model.empresa && f.lote == item.lote && f.loteid == item.id.ToString());

                if (hayEnStock)
                    excepcionesGeneradas += string.Format("El Lote: {0}.{1} ya existe en el Stock<br />", item.lote, item.id);

                if (!hayEnStock && hayEnStockHistorico)
                    excepcionesGeneradas += string.Format("El Lote: {0}.{1} ya ha sido Vendido<br />", item.lote, item.id);

                if (string.IsNullOrEmpty(item.fkarticulos))
                    excepcionesGeneradas += string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Fkarticulos);

                if (!item.cantidad.HasValue)
                    excepcionesGeneradas += string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Cantidad);

                if (!item.metros.HasValue)
                    excepcionesGeneradas += string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Metros);
            }
        }
    }
}
