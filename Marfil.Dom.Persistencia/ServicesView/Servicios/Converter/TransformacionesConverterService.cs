using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Dom.Persistencia.Model.Documentos.Transformaciones;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    class TransformacionesConverterService : BaseConverterModel<TransformacionesModel, Transformaciones>
    {
        public string Ejercicio { get; set; }

        public TransformacionesConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override IEnumerable<IModelView> GetAll()
        {
            var result = _db.Set<Transformaciones>().Where(f => f.empresa == Empresa).ToList().Select(f => GetModelView(f) as TransformacionesModel).ToList();

            return result;
        }

        public override bool Exists(string id)
        {
            var intid = Funciones.Qint(id);
            return _db.Transformaciones.Any(f => f.empresa == Context.Empresa && f.id == intid);
        }

        public override IModelView CreateView(string id)
        {

            var identificador = Funciones.Qint(id);
            var obj = _db.Set<Transformaciones>().Where(f => f.empresa == Empresa && f.id == identificador).Include(f => f.Transformacionesentradalin).Include(f => f.Transformacionessalidalin).Include(f => f.Transformacionescostesadicionales).Single();

            var result = GetModelView(obj) as TransformacionesModel;

            result.Tipodealmacenlote = (TipoAlmacenlote?)obj.tipoalmacenlote;
             var configuracion =_appService.GetConfiguracion(_db);
            result.Materialsalidaigualentrada = configuracion.Materialentradaigualsalida;
            result.Lineasentrada = obj.Transformacionesentradalin.ToList().Select(f => new TransformacionesentradaLinModel()
            {

                Id = f.id,
                Fkarticulos = f.fkarticulos,
                Descripcion = f.descripcion,
                Lote = f.lote,
                Tabla = f.tabla,
                Cantidad = f.cantidad,
                Largo = f.largo,
                Ancho = f.ancho,
                Grueso = f.grueso,
                Fkunidades = f.fkunidades,
                Metros = f.metros,
                Notas = f.notas,
                Canal = f.canal,
                Revision = f.revision,
                Decimalesmonedas = f.decimalesmonedas,
                Decimalesmedidas = f.decimalesmedidas,
                Orden = f.orden ?? f.id,
                Fkcontadoreslotes = f.fkcontadoreslotes,
                Flagidentifier = f.flagidentifier,
                Costeadicionalvariable = f.costeacicionalvariable,
                Costeadicionalmaterial = f.costeadicionalmaterial,
                Costeadicionalotro = f.costeadicionalotro,
                Costeadicionalportes = f.costeadicionalportes,
                Loteautomaticoid = f.loteautomaticoid,
                Lotenuevocontador = f.lotenuevocontador??0,
                Nueva= f.nuevo??false,
                Precio = f.precio
            }).ToList();

            

            result.Lineassalida = obj.Transformacionessalidalin.ToList().Select(f => new TransformacionessalidaLinModel()
            {
                Id = f.id,
                Fkarticulos = f.fkarticulos,
                Descripcion = f.descripcion,
                Lote = f.lote,
                Tabla = f.tabla,
                Cantidad = f.cantidad,
                Largo = f.largo,
                Ancho = f.ancho,
                Grueso = f.grueso,
                Fkunidades = f.fkunidades,
                Metros = f.metros,
                Notas = f.notas,
                Canal = f.canal,
                Revision = f.revision,
                Decimalesmonedas = f.decimalesmonedas,
                Decimalesmedidas = f.decimalesmedidas,
                Orden = f.orden ?? f.id,
                Flagidentifier = f.flagidentifier,
                Precio = f.precio,
        }).ToList();

            var primeritem = result.Lineassalida.FirstOrDefault();
            if (primeritem != null)
            {
                var serviceFamilia = FService.Instance.GetService(typeof(FamiliasproductosModel), Context, _db) as FamiliasproductosService;
                var familia = serviceFamilia.get(ArticulosService.GetCodigoFamilia(primeritem.Fkarticulos)) as FamiliasproductosModel;
                if (familia.Tipofamilia == TipoFamilia.Bloque)
                {
                    result.Lotesalida = primeritem.Lote;
                }
            }
            result.Costes = obj.Transformacionescostesadicionales.ToList().Select(f => new TransformacionesCostesadicionalesModel()
            {
                Id = f.id,
                Tipodocumento = (TipoCosteAdicional)f.tipodocumento,
                Referenciadocumento = f.referenciadocumento,
                Importe = f.importe,
                Porcentaje = f.porcentaje,
                Total = f.total,
                Tipocoste = (TipoCoste)f.tipocoste,
                Tiporeparto = (TipoReparto)f.tiporeparto,
                Notas = f.notas
            }).ToList();
            return result;
        }

        public override Transformaciones CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as TransformacionesModel;
            var result = _db.Set<Transformaciones>().Create();

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
            result.empresa = Empresa;
            result.tipoalmacenlote = (int?)viewmodel.Tipodealmacenlote;

            foreach (var item in viewmodel.Lineasentrada)
            {
                var newItem = _db.Set<Transformacionesentradalin>().Create();
                newItem.empresa = Empresa;
                newItem.id = item.Id;
                newItem.fkarticulos = item.Fkarticulos;
                newItem.descripcion = item.Descripcion;
                newItem.lote = item.Lote;
                newItem.tabla = item.Tabla;
                newItem.cantidad = item.Cantidad;
                newItem.largo = item.Largo;
                newItem.ancho = item.Ancho;
                newItem.grueso = item.Grueso;
                newItem.fkunidades = item.Fkunidades;
                newItem.metros = item.Metros;
                newItem.notas = item.Notas;
                newItem.canal = item.Canal;
                newItem.revision = item.Revision;
                newItem.decimalesmonedas = item.Decimalesmonedas;
                newItem.decimalesmedidas = item.Decimalesmedidas;
                newItem.orden = item.Orden ?? item.Id;
                newItem.costeacicionalvariable = item.Costeadicionalvariable;
                newItem.costeadicionalmaterial = item.Costeadicionalmaterial;
                newItem.costeadicionalotro = item.Costeadicionalotro;
                newItem.costeadicionalportes = item.Costeadicionalportes;
                newItem.precio = item.Precio;
                newItem.loteautomaticoid = item.Loteautomaticoid;
                newItem.lotenuevocontador = item.Lotenuevocontador;
                newItem.nuevo = item.Nueva;
                newItem.flagidentifier = Guid.NewGuid();
          
                result.Transformacionesentradalin.Add(newItem);
            }

            foreach (var item in viewmodel.Lineassalida)
            {
                var newItem = _db.Set<Transformacionessalidalin>().Create();
                newItem.empresa = Empresa;
                newItem.id = item.Id;
                newItem.fkarticulos = item.Fkarticulos;
                newItem.descripcion = item.Descripcion;
                newItem.lote = item.Lote;
                newItem.tabla = item.Tabla;
                newItem.cantidad = item.Cantidad;
                newItem.largo = item.Largo;
                newItem.ancho = item.Ancho;
                newItem.grueso = item.Grueso;
                newItem.fkunidades = item.Fkunidades;
                newItem.metros = item.Metros;
                newItem.notas = item.Notas;
                newItem.canal = item.Canal;
                newItem.revision = item.Revision;
                newItem.decimalesmonedas = item.Decimalesmonedas;
                newItem.decimalesmedidas = item.Decimalesmedidas;
                newItem.orden = item.Orden ?? item.Id;
                newItem.precio = item.Precio;
                newItem.flagidentifier = Guid.NewGuid();
                result.Transformacionessalidalin.Add(newItem);
            }

            foreach (var item in viewmodel.Costes)
            {
                var newItem = _db.Set<Transformacionescostesadicionales>().Create();
                newItem.empresa = Empresa;
                newItem.fktransformaciones = result.id;
                newItem.id = item.Id;
                newItem.tipodocumento = (int)item.Tipodocumento;
                newItem.referenciadocumento = item.Referenciadocumento;
                newItem.importe = item.Importe;
                newItem.porcentaje = item.Porcentaje;
                newItem.total = item.Total;
                newItem.tipocoste = (int)item.Tipocoste;
                newItem.tiporeparto = (int)item.Tiporeparto;
                newItem.notas = item.Notas;
                result.Transformacionescostesadicionales.Add(newItem);
            }

            return result;
        }

        public override Transformaciones EditPersitance(IModelView obj)
        {
            var viewmodel = obj as TransformacionesModel;
            var result = _db.Transformaciones.Where(f => f.empresa == viewmodel.Empresa && f.id == viewmodel.Id).Include(b => b.Transformacionesentradalin).Include(b => b.Transformacionessalidalin).Include(b => b.Transformacionescostesadicionales).ToList().Single();
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
            result.tipoalmacenlote = (int?)viewmodel.Tipodealmacenlote;

            result.Transformacionesentradalin.Clear();

            foreach (var item in viewmodel.Lineasentrada)
            {
                var newItem = _db.Set<Transformacionesentradalin>().Create();
                newItem.id = item.Id;
                newItem.fkarticulos = item.Fkarticulos;
                newItem.descripcion = item.Descripcion;
                newItem.lote = item.Lote;
                newItem.tabla = item.Tabla;
                newItem.cantidad = item.Cantidad;
                newItem.largo = item.Largo;
                newItem.ancho = item.Ancho;
                newItem.grueso = item.Grueso;
                newItem.fkunidades = item.Fkunidades;
                newItem.metros = item.Metros;
                newItem.notas = item.Notas;
                newItem.canal = item.Canal;
                newItem.revision = item.Revision;
                newItem.decimalesmonedas = item.Decimalesmonedas;
                newItem.decimalesmedidas = item.Decimalesmedidas;
                newItem.orden = item.Orden ?? item.Id;
                newItem.costeacicionalvariable = item.Costeadicionalvariable;
                newItem.costeadicionalmaterial = item.Costeadicionalmaterial;
                newItem.costeadicionalotro = item.Costeadicionalotro;
                newItem.costeadicionalportes = item.Costeadicionalportes;
                newItem.precio = item.Precio;
                newItem.flagidentifier = item.Flagidentifier;
                newItem.loteautomaticoid = item.Loteautomaticoid;
                newItem.lotenuevocontador = item.Lotenuevocontador;
                newItem.nuevo = item.Nueva;
                result.Transformacionesentradalin.Add(newItem);
            }

            result.Transformacionessalidalin.Clear();
            foreach (var item in viewmodel.Lineassalida)
            {
                var newItem = _db.Set<Transformacionessalidalin>().Create();
                newItem.id = item.Id;
                newItem.fkarticulos = item.Fkarticulos;
                newItem.descripcion = item.Descripcion;
                newItem.lote = item.Lote;
                newItem.tabla = item.Tabla;
                newItem.cantidad = item.Cantidad;
                newItem.largo = item.Largo;
                newItem.ancho = item.Ancho;
                newItem.grueso = item.Grueso;
                newItem.fkunidades = item.Fkunidades;
                newItem.metros = item.Metros;
                newItem.notas = item.Notas;
                newItem.canal = item.Canal;
                newItem.revision = item.Revision;
                newItem.decimalesmonedas = item.Decimalesmonedas;
                newItem.decimalesmedidas = item.Decimalesmedidas;
                newItem.orden = item.Orden ?? item.Id;
                newItem.precio = item.Precio;
                newItem.flagidentifier = item.Flagidentifier;
                result.Transformacionessalidalin.Add(newItem);
            }
            result.Transformacionescostesadicionales.Clear();
            foreach (var item in viewmodel.Costes)
            {
                var newItem = _db.Set<Transformacionescostesadicionales>().Create();
                newItem.empresa = Empresa;
                newItem.fktransformaciones = result.id;
                newItem.id = item.Id;
                newItem.tipodocumento = (int)item.Tipodocumento;
                newItem.referenciadocumento = item.Referenciadocumento;
                newItem.importe = item.Importe;
                newItem.porcentaje = item.Porcentaje;
                newItem.total = item.Total;
                newItem.tipocoste = (int)item.Tipocoste;
                newItem.tiporeparto = (int)item.Tiporeparto;
                newItem.notas = item.Notas;
                result.Transformacionescostesadicionales.Add(newItem);
            }
            return result;
        }

        public override IModelView GetModelView(Transformaciones obj)
        {
            var result = base.GetModelView(obj) as TransformacionesModel;
            result.Integridadreferencialflag = obj.integridadreferencialflag;
            return result;
        }
    }
}
