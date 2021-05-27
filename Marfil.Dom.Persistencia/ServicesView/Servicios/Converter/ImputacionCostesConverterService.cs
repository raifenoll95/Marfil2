using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Inf.Genericos;
using Marfil.Inf.Genericos.Helper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    class ImputacionCostesConverterService : BaseConverterModel<ImputacionCostesModel, ImputacionCostes>
    {
        #region ctr
        public ImputacionCostesConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }
        #endregion

        public override IEnumerable<IModelView> GetAll()
        {
            var result = _db.Set<ImputacionCostes>().Where(f => f.empresa == Empresa).ToList().Select(f => GetModelView(f) as ImputacionCostesModel).ToList();

            return result;
        }

        public override bool Exists(string id)
        {
            var intid = Funciones.Qint(id);
            return _db.ImputacionCostes.Any(f => f.empresa == Context.Empresa && f.id == intid);
        }

        public override IModelView CreateView(string id)
        {

            var identificador = Funciones.Qint(id);
            var obj = _db.Set<ImputacionCostes>().Where(f => f.empresa == Empresa && f.id == identificador).Include(f => f.ImputacionCostesLin).Include(f => f.ImputacionCostescostesadicionales).Single();

            var configuracion = _appService.GetConfiguracion(_db);
            var result = GetModelView(obj) as ImputacionCostesModel;

            result.LineasLotes = obj.ImputacionCostesLin.ToList().Select(f => new ImputacionCostesLinModel(Context)
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
                Costeadicionalvariable = f.costeadicionalvariable,
                Costeadicionalmaterial = f.costeadicionalmaterial,
                Costeadicionalotro = f.costeadicionalotro,
                Costeadicionalportes = f.costeadicionalportes,
                //Flagidentifier = f.flagidentifier,
                Precio = f.precio,
            }).ToList();

            result.LineasCostes = obj.ImputacionCostescostesadicionales.ToList().Select(f => new ImputacionCostesCostesadicionalesModel()
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


        public override ImputacionCostes CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as ImputacionCostesModel;
            var result = _db.Set<ImputacionCostes>().Create();

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

            foreach (var item in viewmodel.LineasLotes)
            {
                var newItem = _db.Set<ImputacionCostesLin>().Create();
                newItem.empresa = Empresa;
                newItem.fkimputacioncostes = result.id;
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
                newItem.costeadicionalvariable = item.Costeadicionalvariable;
                newItem.costeadicionalmaterial = item.Costeadicionalmaterial;
                newItem.costeadicionalotro = item.Costeadicionalotro;
                newItem.costeadicionalportes = item.Costeadicionalportes;
                newItem.flagidentifier = Guid.NewGuid();

                result.ImputacionCostesLin.Add(newItem);
            }

            foreach (var item in viewmodel.LineasCostes)
            {
                var newItem = _db.Set<ImputacionCostescostesadicionales>().Create();
                newItem.empresa = Empresa;
                newItem.fkimputacioncostes = result.id;
                newItem.id = item.Id;
                newItem.tipodocumento = (int)item.Tipodocumento;
                newItem.referenciadocumento = item.Referenciadocumento;
                newItem.importe = item.Importe;
                newItem.porcentaje = item.Porcentaje;
                newItem.total = item.Total;
                newItem.tipocoste = (int)item.Tipocoste;
                newItem.tiporeparto = (int)item.Tiporeparto;
                newItem.notas = item.Notas;

                result.ImputacionCostescostesadicionales.Add(newItem);
            }

            result.empresa = Empresa;

            return result;
        }

        public override ImputacionCostes EditPersitance(IModelView obj)
        {
            var viewmodel = obj as ImputacionCostesModel;
            var result = _db.ImputacionCostes.Where(f => f.empresa == viewmodel.Empresa && f.id == viewmodel.Id).Include(b => b.ImputacionCostesLin).Include(b => b.ImputacionCostescostesadicionales).ToList().Single();

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

            result.ImputacionCostesLin.Clear();

            foreach (var item in viewmodel.LineasLotes)
            {
                var newItem = _db.Set<ImputacionCostesLin>().Create();
                newItem.id = item.Id;
                newItem.fkimputacioncostes = result.id;
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
                newItem.costeadicionalvariable = item.Costeadicionalvariable;
                newItem.costeadicionalmaterial = item.Costeadicionalmaterial;
                newItem.costeadicionalotro = item.Costeadicionalotro;
                newItem.costeadicionalportes = item.Costeadicionalportes;
                newItem.precio = item.Precio;
                newItem.flagidentifier = item.Flagidentifier;
                result.ImputacionCostesLin.Add(newItem);
            }


            result.ImputacionCostescostesadicionales.Clear();
            foreach (var item in viewmodel.LineasCostes)
            {
                var newItem = _db.Set<ImputacionCostescostesadicionales>().Create();
                newItem.empresa = Empresa;
                newItem.fkimputacioncostes = result.id;
                newItem.id = item.Id;
                newItem.tipodocumento = (int)item.Tipodocumento;
                newItem.referenciadocumento = item.Referenciadocumento;
                newItem.importe = item.Importe;
                newItem.porcentaje = item.Porcentaje;
                newItem.total = item.Total;
                newItem.tipocoste = (int)item.Tipocoste;
                newItem.tiporeparto = (int)item.Tiporeparto;
                newItem.notas = item.Notas;
                result.ImputacionCostescostesadicionales.Add(newItem);
            }

            return result;
        }
    }
}
