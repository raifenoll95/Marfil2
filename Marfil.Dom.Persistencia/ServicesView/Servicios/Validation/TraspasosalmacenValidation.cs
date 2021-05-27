using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Documentos.Albaranes;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Dom.Persistencia.Model.Documentos.Traspasosalmacen;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RAlbaranes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Albaranes;
using RTraspasosalmacen = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Traspasosalmacen;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
   
    internal class TraspasosalmacenValidation : BaseValidation<Traspasosalmacen>
    {
        public string EjercicioId { get; set; }
        public bool CambiarEstado { get; set; }
        public bool FlagActualizarCantidadesServidas { get; set; }
        public bool FlagActualizarCantidadesFacturadas { get; set; }
        public bool ModificarCostes { get; set; }

        public TraspasosalmacenValidation(IContextService context, MarfilEntities db) : base(context,db)
        {

        }

        #region Validar grabar

        public override bool ValidarGrabar(Traspasosalmacen model)
        {
            //Todo EL: verificar los albaranes de compra si se pueden modificar si está en alguna factura de compra
            //if (CambiarEstado)
            //    if (_db.FacturasLin.Any(f => f.fkalbaranesreferencia == model.referencia && f.empresa == model.empresa))
            //    throw new ValidationException(RTraspasosalmacen.ErrorAlbaranFacturado);

            if (!CambiarEstado)
            {
                if(!ModificarCostes)
                    ValidarEstado(model);
                ValidarCabecera(model);
                ValidarLineas(model);
                CalcularCostesadicionales(model);
            }
            
            return base.ValidarGrabar(model);
        }

       

        private void ValidarEstado(Traspasosalmacen model)
        {
            string message;
            if (!FlagActualizarCantidadesFacturadas)
            {
                if (!_appService.ValidarEstado(model.fkestados, _db, out message))
                    throw new ValidationException(message);
            }

            var estadosService = new EstadosService(Context,_db);
            var configuracionService = new ConfiguracionService(Context, _db);
            var configuracionModel = configuracionService.GetModel();
            var estadoactualObj = estadosService.get(model.fkestados) as EstadosModel;
            if (!string.IsNullOrEmpty(configuracionModel.Estadototal) && estadoactualObj.Tipoestado <= TipoEstado.Curso && model.TraspasosalmacenLin.Any() && model.TraspasosalmacenLin.All(f => (f.cantidad?? 0) != 0 && (f.cantidad ?? 0) - (f.cantidadpedida ?? 0) <= 0))
            {
                model.fkestados = configuracionModel.Estadotraspasosalmacentotal;
            }
            else if (!string.IsNullOrEmpty(configuracionModel.Estadoparcial) && estadoactualObj.Tipoestado <= TipoEstado.Curso &&
                     model.TraspasosalmacenLin.Any(f => (f.cantidadpedida ?? 0) > 0))
            {
                model.fkestados = configuracionModel.Estadoparcial;
            }
            else if (!string.IsNullOrEmpty(configuracionModel.Estadoinicial) && estadoactualObj.Tipoestado <= TipoEstado.Curso &&
                     model.TraspasosalmacenLin.Any(f => (f.cantidadpedida ?? 0) == 0))
            {
                model.fkestados = configuracionModel.Estadoalbaranesventasinicial;
            }
        }

        private bool ValidaRangoEjercicio(Traspasosalmacen model)
        {
            var result = true;
            var ejercicio = model.fkejercicio;
            var ejercicioobj = _db.Ejercicios.Single(f => f.empresa == model.empresa && f.id == ejercicio);
            var fechadocumento = model.fechadocumento.Value;
            return fechadocumento >= ejercicioobj.desde.Value && fechadocumento <= ejercicioobj.hasta.Value;
        }

        private void ValidarCabecera(Traspasosalmacen model)
        {
            if (model.fechadocumento == null)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RTraspasosalmacen.Fechadocumento));
            
            
            if (string.IsNullOrEmpty(model.fkalmacen))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RTraspasosalmacen.Fkalmacen));
            if (string.IsNullOrEmpty(model.fkalmacendestino))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RTraspasosalmacen.Fkalmacen));
            if(model.fkalmacen==model.fkalmacendestino)
                throw new ValidationException("Los almacenes deben ser distintos");

            if (!string.IsNullOrEmpty(model.fkzonas))
            {
                var idInt = Funciones.Qint(model.fkzonas) ?? 0;
                if (!_db.AlmacenesZona.Any(
                    f => f.empresa == model.empresa && f.fkalmacenes == model.fkalmacendestino && f.id == idInt))
                    throw new ValidationException(string.Format(RAlbaranes.ErrorZonaAlmacen,model.fkzonas,model.fkalmacen));
                
            }
            
            
            if (!FlagActualizarCantidadesFacturadas && !ValidaRangoEjercicio(model))
                throw new ValidationException(RTraspasosalmacen.ErrorFechaEjercicio);

            model.integridadreferenciaflag = Guid.NewGuid();

        }

        private void ValidarLineas(Traspasosalmacen model)
        {
            if (!model.TraspasosalmacenLin.Any() && model.TraspasosalmacenCostesadicionales.Any())
                throw new ValidationException(General.ErrorLineasObligatoriasCostes);

            var familiasProductosService = FService.Instance.GetService(typeof(FamiliasproductosModel), Context, _db);
            foreach (var item in model.TraspasosalmacenLin)
            {
                if (string.IsNullOrEmpty(item.fkarticulos))
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RTraspasosalmacen.Fkarticulos));

                if (!item.cantidad.HasValue)
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RTraspasosalmacen.Cantidad));

                if (!item.metros.HasValue)
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RTraspasosalmacen.Metros));

               

                var familiacodigo = ArticulosService.GetCodigoFamilia(item.fkarticulos);
                var familiaModel = familiasProductosService.get(familiacodigo) as FamiliasproductosModel;
                item.fkunidades = _db.Unidades.Single(f => f.id == familiaModel.Fkunidadesmedida).id;

                VerificarPertenenciaKit(model, item);
            }

            var vector = model.TraspasosalmacenLin.OrderBy(f => f.orden).ToList();
            for (var i = 0; i < vector.Count(); i++)
                vector[i].orden = (i + 1) * ApplicationHelper.EspacioOrdenLineas;

        }

      

      
        private void CalcularCostesadicionales(Traspasosalmacen model)
        {
            return;
        }

        private void VerificarPertenenciaKit(Traspasosalmacen model, TraspasosalmacenLin linea)
        {
            var loteid = linea.tabla?.ToString() ?? string.Empty;
            if (_db.KitLin.Any(f => f.empresa == model.empresa && f.lote == linea.lote && f.loteid == loteid))
            {
                var kitobj =
                    _db.Kit.Include("KitLin")
                        .Single(
                            f =>
                                f.empresa == model.empresa &&
                                f.KitLin.Any(j => j.lote == linea.lote && j.loteid == loteid));
                if (!kitobj.KitLin.All(f => model.TraspasosalmacenLin.Any(j => j.lote == f.lote && j.tabla.ToString() == f.loteid)))
                {
                    throw new ValidationException(string.Format("El Lote {0}{1} pertenece al Kit {2} que no está completo. Todas los registros del Kit deben añadirse al albarán.", linea.lote, Funciones.RellenaCod(loteid, 3), kitobj.referencia));
                }

                
                _db.Kit.AddOrUpdate(kitobj);
            }

            if (_db.BundleLin.Any(f => f.empresa == model.empresa && f.lote == linea.lote && f.loteid == loteid))
            {
                var bundleobj =
                    _db.Bundle.Include("BundleLin")
                        .Single(
                            f =>
                                f.empresa == model.empresa &&
                                f.BundleLin.Any(j => j.lote == linea.lote && j.loteid == loteid));
                if (!bundleobj.BundleLin.All(f => model.TraspasosalmacenLin.Any(j => j.lote == f.lote && j.tabla.ToString() == f.loteid)))
                {
                    throw new ValidationException(string.Format("El Lote {0}{1} pertenece al Bundle {0}{2} que no está completo. Todas los registros del Bundle deben añadirse al albarán.", linea.lote, Funciones.RellenaCod(loteid, 3), bundleobj.id));
                }

                
                _db.Bundle.AddOrUpdate(bundleobj);
            }
        }

        #endregion

        #region Eliminar

        public override bool ValidarBorrar(Traspasosalmacen model)
        {
            
            throw new ValidationException(RTraspasosalmacen.ErrorBorrarTraspaso);

           
        }

        #endregion

       
    }
}
