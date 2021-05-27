using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Contabilidad.Movs;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    class MovsConverterService : BaseConverterModel<MovsModel, Movs>
    {
        public string Ejercicio { get; set; }

        public MovsConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override IEnumerable<IModelView> GetAll()
        {
            var result = _db.Set<Movs>().Where(f => f.empresa == Empresa).ToList().Select(f => GetModelView(f) as MovsModel).ToList();
            using (var serviceCriterios = FService.Instance.GetService(typeof(CriteriosagrupacionModel), Context))
            {
                var criterioslist = serviceCriterios.getAll().Select(f => (CriteriosagrupacionModel)f);
                foreach (var item in result)
                {
                    //item.Descripcioncriterioagrupacion = criterioslist.SingleOrDefault(f => f.Id == item.Fkcriteriosagrupacion)?.Nombre;

                }
            }

            return result;
        }

        public override bool Exists(string id)
        {
            var intid = Funciones.Qint(id);
            return _db.Movs.Any(f => f.empresa == Context.Empresa && f.id == intid);
        }

        public override IModelView CreateView(string id)
        {
            var identificador = Funciones.Qint(id);
            //var obj = _db.Set<Movs>().Where(f => f.empresa == Empresa && f.id == identificador).Include(f => f.MovsLin).Include(f => f.MovsTotales).Single();
            var obj = _db.Set<Movs>().Where(f => f.empresa == Empresa && f.id == identificador).Include(f => f.MovsLin).Single();

            var monedasObj = _db.Monedas.Single(f => f.id == obj.fkmonedas);
            var result = GetModelView(obj) as MovsModel;
            //var serieContableService = FService.Instance.GetService(typeof(SeriesContablesModel), Context, _db);
            //var serieContableObj = serieContableService.get(SeriesContablesService.GetSerieContableCodigo(TipoAsiento.Normal) + "-" + result.Fkseriescontables) as SeriesContablesModel;
            
            result.Decimalesmonedas = monedasObj.decimales ?? 2;

            //result.Codigodescripcionasiento =  obj.codigodescripcionasiento;
           

            return result;
        }

        public override Movs CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as MovsModel;
            var result = _db.Set<Movs>().Create();

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
                var newItem = _db.Set<MovsLin>().Create();
                newItem.empresa = Empresa;
                newItem.fkmovs = result.id;
                newItem.id = item.Id;
                newItem.importe = item.Importe;
                newItem.flagidentifier = Guid.NewGuid();
                newItem.fkcuentas = item.Fkcuentas;
                newItem.fkseccionesanaliticas = item.Fkseccionesanaliticas;
                newItem.flagidentifier = item.Flagidentifier;
                newItem.orden = item.Orden;
                newItem.codigocomentario = item.Codigocomentario;
                newItem.comentario = item.Comentario;
                newItem.esdebe = item.Esdebe;
                newItem.importemonedadocumento = item.Importemonedadocumento;
                newItem.conciliado = item.Conciliado;
                result.MovsLin.Add(newItem);
            }
            
            return result;
        }

        public override Movs EditPersitance(IModelView obj)
        {
            var viewmodel = obj as MovsModel;
           var result = _db.Movs.Where(f => f.empresa == viewmodel.Empresa && f.id == viewmodel.Id).Include(b => b.MovsLin).ToList().Single();
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

            result.MovsLin.Clear();

            foreach (var item in viewmodel.Lineas)
            {
                var newItem = _db.Set<MovsLin>().Create();
                newItem.flagidentifier = item.Flagidentifier;
                newItem.empresa = result.empresa;
                newItem.fkmovs = result.id;
                newItem.id = item.Id;
                newItem.orden = item.Orden;
                newItem.fkseccionesanaliticas = item.Fkseccionesanaliticas;
                newItem.fkcuentas = item.Fkcuentas;
                newItem.codigocomentario = item.Codigocomentario;
                newItem.comentario = item.Comentario;
                newItem.importe = item.Importe;
                newItem.esdebe = item.Esdebe;
                newItem.importemonedadocumento = item.Importemonedadocumento;
                newItem.conciliado = item.Conciliado;
               
                result.MovsLin.Add(newItem);
            }
            
            return result;
        }

        public override IModelView GetModelView(Movs obj)
        {
            var result = base.GetModelView(obj) as MovsModel;
            result.Integridadreferencial = obj.integridadreferencial;

            // Estaba antes en CreateView
            //Lineas
            result.Lineas = obj.MovsLin.ToList().Select(f => new MovsLinModel()
            {
                Id = f.id,
                Orden = f.orden ?? f.id,
                Fkseccionesanaliticas = f.fkseccionesanaliticas,
                Fkcuentas = f.fkcuentas,
                Flagidentifier = f.flagidentifier,
                Codigocomentario = f.codigocomentario,
                Comentario = f.comentario,

                Esdebe = f.esdebe,
                Importe = f.importe,

                Importemonedadocumento = f.importemonedadocumento,
                Conciliado = f.conciliado
            }).ToList();

            return result;
        }
    }
}
