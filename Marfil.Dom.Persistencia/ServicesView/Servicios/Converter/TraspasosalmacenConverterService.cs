using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Dom.Persistencia.Model.Documentos.Traspasosalmacen;
using Marfil.Dom.Persistencia.Model.Documentos.Presupuestos;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.GaleriaImagenes;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Inf.Genericos;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    class TraspasosalmacenConverterService : BaseConverterModel<TraspasosalmacenModel, Traspasosalmacen>
    {
        public string Ejercicio { get; set; }

        public TraspasosalmacenConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override IEnumerable<IModelView> GetAll()
        {
            var result = _db.Set<Traspasosalmacen>().Where(f => f.empresa == Empresa).ToList().Select(f => GetModelView(f) as TraspasosalmacenModel).ToList();

            return result;
        }

        public override IModelView CreateView(string id)
        {

            var identificador = Funciones.Qint(id);
            var obj = _db.Set<Traspasosalmacen>().Where(f => f.empresa == Empresa && f.id == identificador).Include(f => f.TraspasosalmacenLin).Single();
           

            var result = GetModelView(obj) as TraspasosalmacenModel;

           
            result.Tipodeportes = (Tipoportes?)obj.tipoportes;
           
            //Lineas
            result.Lineas = obj.TraspasosalmacenLin.ToList().Select(f => new TraspasosalmacenLinModel()
            {
                
                Id = f.id,
                Fkarticulos = f.fkarticulos,
                Descripcion = f.descripcion,
                Lote = f.lote,
                Tabla = f.tabla,
                Cantidad = f.cantidad,
                Cantidadpedida = f.cantidadpedida,
                Largo = f.largo,
                Ancho = f.ancho,
                Grueso = f.grueso,
                Fkunidades = f.fkunidades,
                Metros = f.metros,
                Precio = f.precio,
                Porcentajedescuento = f.porcentajedescuento,
                Importedescuento = f.importedescuento,
                Fkregimeniva = result.Fkregimeniva,
                Fktiposiva = f.fktiposiva,
                Porcentajeiva = f.porcentajeiva,
                Cuotaiva = f.cuotaiva,
                Porcentajerecargoequivalencia = f.porcentajerecargoequivalencia,
                Cuotarecargoequivalencia = f.cuotarecargoequivalencia,
                Importe = f.importe,
                Notas = f.notas,
                Canal = f.canal,
                Precioanterior = f.precioanterior,
                Revision = f.revision,
                Decimalesmonedas = f.decimalesmonedas,
                Decimalesmedidas = f.decimalesmedidas,
                Bundle = f.bundle,
                Tblnum = f.tblnum,
                Contenedor = f.contenedor,
                Sello = f.sello,
                Caja = f.caja,
                Pesoneto = f.pesoneto,
                Pesobruto = f.pesobruto,
                Seccion = f.seccion,
                Costeadicionalvariable = f.costeacicionalvariable,
                Costeadicionalportes = f.costeadicionalportes,
                Costeadicionalmaterial = f.costeadicionalmaterial,
                Costeadicionalotro = f.costeadicionalotro,
                Orden = f.orden ?? f.id,
                Flagidentifier = f.flagidentifier
            }).ToList();
            

            result.Costes =obj.TraspasosalmacenCostesadicionales.ToList().Select(f => new TraspasosalmacenCostesadicionalesModel()
                {
                    Id= f.id,
                    Tipodocumento = (TipoCosteAdicional)f.tipodocumento,
                    Referenciadocumento = f.referenciadocumento,
                    Importe=f.importe,
                    Porcentaje = f.porcentaje,
                    Total =f.total,
                    Tipocoste =(TipoCoste)f.tipocoste,
                    Tiporeparto = (TipoReparto)f.tiporeparto,
                    Notas = f.notas
                }).ToList();

           

            return result;
        }

        public override Traspasosalmacen CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as TraspasosalmacenModel;
            var result = _db.Set<Traspasosalmacen>().Create();

            foreach (var item in result.GetType().GetProperties())
            {
                if ((obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false) &&
                    (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.GetGenericTypeDefinition() !=
                    typeof(ICollection<>)))
                {
                    item.SetValue(result, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
                else if (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsEnum ?? false)
                {
                    item.SetValue(result, (int)obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
                else if (!obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false)
                {
                    item.SetValue(result, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
            }
            result.fechaalta = DateTime.Now;
            result.fechamodificacion = result.fechaalta;
            result.fkusuarioalta = Context.Id;
            result.fkusuariomodificacion = Context.Id;
            result.fkpuertosfkpaises = viewmodel.Fkpuertos.Fkpaises;
            result.fkpuertosid = viewmodel.Fkpuertos.Id;
            result.tipoportes = (int?)viewmodel.Tipodeportes;
            result.empresa = Empresa;
           
            foreach (var item in viewmodel.Lineas)
            {
                var newItem = _db.Set<TraspasosalmacenLin>().Create();
                newItem.empresa = Empresa;
                newItem.fktraspasosalmacen = result.id;
                newItem.id = item.Id;
                newItem.fkarticulos = item.Fkarticulos;
                newItem.descripcion = item.Descripcion;
                newItem.lote = item.Lote;
                newItem.tabla = item.Tabla;
                newItem.cantidad = item.Cantidad;
                newItem.cantidadpedida = item.Cantidadpedida;
                newItem.largo = item.Largo;
                newItem.ancho = item.Ancho;
                newItem.grueso = item.Grueso;
                newItem.fkunidades = item.Fkunidades;
                newItem.metros = item.Metros;
                newItem.precio = item.Precio;
                newItem.porcentajedescuento = item.Porcentajedescuento;
                newItem.importedescuento = item.Importedescuento;
                newItem.fktiposiva = item.Fktiposiva;
                newItem.porcentajeiva = item.Porcentajeiva;
                newItem.cuotaiva = item.Cuotaiva;
                newItem.porcentajerecargoequivalencia = item.Porcentajerecargoequivalencia ?? 0;
                newItem.cuotarecargoequivalencia = item.Cuotarecargoequivalencia;
                newItem.importe = item.Importe;
                newItem.notas = item.Notas;
                newItem.canal = item.Canal;
                newItem.precioanterior = item.Precioanterior;
                newItem.revision = item.Revision;
                newItem.decimalesmonedas = item.Decimalesmonedas;
                newItem.decimalesmedidas = item.Decimalesmedidas;
                newItem.bundle = item.Bundle?.ToUpper();
                newItem.tblnum = item.Tblnum;
                newItem.contenedor = item.Contenedor;
                newItem.sello = item.Sello;
                newItem.caja = item.Caja;
                newItem.pesoneto = item.Pesoneto;
                newItem.pesobruto = item.Pesobruto;
                newItem.seccion = item.Seccion;
                newItem.costeadicionalportes = item.Costeadicionalportes;
                newItem.costeacicionalvariable = item.Costeadicionalvariable;
                newItem.costeadicionalmaterial = item.Costeadicionalmaterial;
                newItem.costeadicionalotro = item.Costeadicionalotro;
                newItem.flagidentifier = Guid.NewGuid();
                result.TraspasosalmacenLin.Add(newItem);
            }


         
            foreach (var item in viewmodel.Costes)
            {
                var newItem = _db.Set<TraspasosalmacenCostesadicionales>().Create();
                newItem.empresa = Empresa;
                newItem.fktraspasosalmacen = result.id;
                newItem.id = item.Id;
                newItem.tipodocumento = (int) item.Tipodocumento;
                newItem.referenciadocumento = item.Referenciadocumento;
                newItem.importe = item.Importe;
                newItem.porcentaje = item.Porcentaje;
                newItem.total = item.Total;
                newItem.tipocoste = (int)item.Tipocoste;
                newItem.tiporeparto = (int) item.Tiporeparto;
                newItem.notas = item.Notas;
                result.TraspasosalmacenCostesadicionales.Add(newItem);
            }

            return result;
        }

        public override Traspasosalmacen EditPersitance(IModelView obj)
        {
            var viewmodel = obj as TraspasosalmacenModel;
            var result = _db.Traspasosalmacen.Where(f => f.empresa == viewmodel.Empresa && f.id == viewmodel.Id).Include(b => b.TraspasosalmacenLin).ToList().Single();
            //todo asignar
            foreach (var item in result.GetType().GetProperties())
            {
                if ((obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false) &&
                    (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.GetGenericTypeDefinition() !=
                    typeof(ICollection<>)))
                {
                    item.SetValue(result, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
                else if (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsEnum ?? false)
                {
                    item.SetValue(result, (int)obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
                else if (!obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false)
                {
                    item.SetValue(result, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
            }
            //todo asignar contador y referencia
            result.fechamodificacion = DateTime.Now;
            result.fkusuariomodificacion = Context.Id;
            result.fkpuertosfkpaises = viewmodel.Fkpuertos.Fkpaises;
            result.fkpuertosid = viewmodel.Fkpuertos.Id;
            result.tipoportes = (int?)viewmodel.Tipodeportes;
            result.TraspasosalmacenLin.Clear();
            foreach (var item in viewmodel.Lineas)
            {
                var newItem = _db.Set<TraspasosalmacenLin>().Create();
                newItem.empresa = result.empresa;
                newItem.fktraspasosalmacen = result.id;
                newItem.id = item.Id;
                newItem.fkarticulos = item.Fkarticulos;
                newItem.descripcion = item.Descripcion;
                newItem.lote = item.Lote;
                newItem.tabla = item.Tabla;
                newItem.cantidad = item.Cantidad;
                newItem.cantidadpedida = item.Cantidadpedida;
                newItem.largo = item.Largo;
                newItem.ancho = item.Ancho;
                newItem.grueso = item.Grueso;
                newItem.fkunidades = item.Fkunidades;
                newItem.metros = item.Metros;
                newItem.precio = item.Precio;
                newItem.porcentajedescuento = item.Porcentajedescuento;
                newItem.importedescuento = item.Importedescuento;
                newItem.fktiposiva = item.Fktiposiva;
                newItem.porcentajeiva = item.Porcentajeiva;
                newItem.cuotaiva = item.Cuotaiva;
                newItem.porcentajerecargoequivalencia = item.Porcentajerecargoequivalencia ?? 0;
                newItem.cuotarecargoequivalencia = item.Cuotarecargoequivalencia;
                newItem.importe = item.Importe;
                newItem.notas = item.Notas;
                newItem.canal = item.Canal;
                newItem.precioanterior = item.Precioanterior;
                newItem.revision = item.Revision;
                newItem.decimalesmonedas = item.Decimalesmonedas;
                newItem.decimalesmedidas = item.Decimalesmedidas;
                newItem.bundle = item.Bundle?.ToUpper();
                newItem.tblnum = item.Tblnum;
                newItem.contenedor = item.Contenedor;
                newItem.sello = item.Sello;
                newItem.caja = item.Caja;
                newItem.pesoneto = item.Pesoneto;
                newItem.pesobruto = item.Pesobruto;
                newItem.seccion = item.Seccion;
                newItem.costeadicionalportes = item.Costeadicionalportes;
                newItem.costeacicionalvariable = item.Costeadicionalvariable;
                newItem.costeadicionalmaterial = item.Costeadicionalmaterial;
                newItem.costeadicionalotro = item.Costeadicionalotro;
                newItem.orden = item.Orden;
                newItem.flagidentifier = item.Flagidentifier;
                result.TraspasosalmacenLin.Add(newItem);
            }

           

            result.TraspasosalmacenCostesadicionales.Clear();
            foreach (var item in viewmodel.Costes)
            {
                var newItem = _db.Set<TraspasosalmacenCostesadicionales>().Create();
                newItem.empresa = Empresa;
                newItem.fktraspasosalmacen = result.id;
                newItem.id = item.Id;
                newItem.tipodocumento = (int)item.Tipodocumento;
                newItem.referenciadocumento = item.Referenciadocumento;
                newItem.importe = item.Importe;
                newItem.porcentaje = item.Porcentaje;
                newItem.total = item.Total;
                newItem.tipocoste = (int)item.Tipocoste;
                newItem.tiporeparto = (int)item.Tiporeparto;
                newItem.notas = item.Notas;
                result.TraspasosalmacenCostesadicionales.Add(newItem);
            }
            return result;
        }

        public override IModelView GetModelView(Traspasosalmacen obj)
        {
            var result = base.GetModelView(obj) as TraspasosalmacenModel;

            result.Fkpuertos.Fkpaises = obj.fkpuertosfkpaises;
            result.Fkpuertos.Id = obj.fkpuertosid;
            result.Integridadreferenciaflag = obj.integridadreferenciaflag;
            return result;
        }
    }
}
