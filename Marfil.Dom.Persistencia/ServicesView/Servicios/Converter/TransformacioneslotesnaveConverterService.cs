using Marfil.Dom.Persistencia.Model.Documentos.Transformacioneslotesnave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos;
using Marfil.Inf.Genericos.Helper;


namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    class TransformacioneslotesnaveConverterService : BaseConverterModel<TransformacioneslotesnaveModel, Transformacioneslotesnave>
    {
        public TransformacioneslotesnaveConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }


        public override IEnumerable<IModelView> GetAll()
        {
            var result = _db.Set<Transformacioneslotesnave>().Where(f => f.empresa == Empresa).ToList().Select(f => GetModelView(f) as TransformacioneslotesnaveModel).ToList();

            return result;
        }

        public override bool Exists(string id)
        {
            var intid = Funciones.Qint(id);
            return _db.Transformacioneslotesnave.Any(f => f.empresa == Context.Empresa && f.id == intid);
        }

        public override IModelView CreateView(string id)
        {

            var identificador = Funciones.Qint(id);
            var obj = _db.Set<Transformacioneslotesnave>().Where(f => f.empresa == Empresa && f.id == identificador).Include(x => x.Transformacioneslotesnavelin).Single();

            var result = GetModelView(obj) as TransformacioneslotesnaveModel;




            result.Lineas = obj.Transformacioneslotesnavelin.ToList().Select(f => new TransformacioneslotesnaveLinModel()
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
                Terminado = (bool)f.terminado,
                Notas = f.notas,
                Canal = f.canal,
                Revision = f.revision,
                Decimalesmonedas = f.decimalesmonedas,
                Decimalesmedidas = f.decimalesmedidas,
                Orden = f.orden ?? f.id,
                Flagidentifier = f.flagidentifier,
                Precio = f.precio,
                Costeadicionalvariable = f.costeacicionalvariable,
                Costeadicionalmaterial = f.costeadicionalmaterial,
                Costeadicionalportes = f.costeadicionalportes,
                Costeadicionalotro = f.costeadicionalotro
            }).ToList();


            /*result.Costes = obj.Transformacioneslotescostesadicionales.ToList().Select(f => new TransformacioneslotesCostesadicionalesModel()
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
            }).ToList();*/
            return result;
        }

        public override Transformacioneslotesnave CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as TransformacioneslotesnaveModel;
            var result = _db.Set<Transformacioneslotesnave>().Create();

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



            foreach (var item in viewmodel.Lineas)
            {
                var newItem = _db.Set<Transformacioneslotesnavelin>().Create();
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
                newItem.terminado = item.Terminado;
                newItem.notas = item.Notas;
                newItem.canal = item.Canal;
                newItem.revision = item.Revision;
                newItem.decimalesmonedas = item.Decimalesmonedas;
                newItem.decimalesmedidas = item.Decimalesmedidas;
                newItem.orden = item.Orden ?? item.Id;
                newItem.precio = item.Precio;
                newItem.flagidentifier = Guid.NewGuid();
                newItem.costeacicionalvariable = item.Costeadicionalvariable;
                newItem.costeadicionalmaterial = item.Costeadicionalmaterial;
                newItem.costeadicionalotro = item.Costeadicionalotro;
                newItem.costeadicionalportes = item.Costeadicionalportes;

                result.Transformacioneslotesnavelin.Add(newItem);
            }

            /*foreach (var item in viewmodel.Costes)
            {
                var newItem = _db.Set<Transformacioneslotescostesadicionales>().Create();
                newItem.empresa = Empresa;
                newItem.fkTransformacioneslotes = result.id;
                newItem.id = item.Id;
                newItem.tipodocumento = (int)item.Tipodocumento;
                newItem.referenciadocumento = item.Referenciadocumento;
                newItem.importe = item.Importe;
                newItem.porcentaje = item.Porcentaje;
                newItem.total = item.Total;
                newItem.tipocoste = (int)item.Tipocoste;
                newItem.tiporeparto = (int)item.Tiporeparto;
                newItem.notas = item.Notas;
                result.Transformacioneslotescostesadicionales.Add(newItem);
            }*/

            return result;
        }

        public override Transformacioneslotesnave EditPersitance(IModelView obj)
        {
            var viewmodel = obj as TransformacioneslotesnaveModel;
            var result = _db.Transformacioneslotesnave.Where(f => f.empresa == viewmodel.Empresa && f.id == viewmodel.Id).Include(b => b.Transformacioneslotesnavelin).ToList().Single();
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


            result.Transformacioneslotesnavelin.Clear();
            foreach (var item in viewmodel.Lineas)
            {
                var newItem = _db.Set<Transformacioneslotesnavelin>().Create();
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
                newItem.terminado = item.Terminado;
                newItem.notas = item.Notas;
                newItem.canal = item.Canal;
                newItem.revision = item.Revision;
                newItem.decimalesmonedas = item.Decimalesmonedas;
                newItem.decimalesmedidas = item.Decimalesmedidas;
                newItem.orden = item.Orden ?? item.Id;
                newItem.precio = item.Precio;
                newItem.flagidentifier = item.Flagidentifier;
                newItem.costeacicionalvariable = item.Costeadicionalvariable;
                newItem.costeadicionalmaterial = item.Costeadicionalmaterial;
                newItem.costeadicionalotro = item.Costeadicionalotro;
                newItem.costeadicionalportes = item.Costeadicionalportes;
                result.Transformacioneslotesnavelin.Add(newItem);
            }
            /*result.Transformacioneslotescostesadicionales.Clear();
            foreach (var item in viewmodel.Costes)
            {
                var newItem = _db.Set<Transformacioneslotescostesadicionales>().Create();
                newItem.empresa = Empresa;
                newItem.fkTransformacioneslotes = result.id;
                newItem.id = item.Id;
                newItem.tipodocumento = (int)item.Tipodocumento;
                newItem.referenciadocumento = item.Referenciadocumento;
                newItem.importe = item.Importe;
                newItem.porcentaje = item.Porcentaje;
                newItem.total = item.Total;
                newItem.tipocoste = (int)item.Tipocoste;
                newItem.tiporeparto = (int)item.Tiporeparto;
                newItem.notas = item.Notas;
                result.Transformacioneslotescostesadicionales.Add(newItem);
            }*/
            return result;
        }

        public override IModelView GetModelView(Transformacioneslotesnave obj)
        {
            var result = base.GetModelView(obj) as TransformacioneslotesnaveModel;
            result.Integridadreferencialflag = obj.integridadreferencialflag;
            return result;
        }
    }
}
