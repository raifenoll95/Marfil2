using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    class MonedasConverterService : BaseConverterModel<MonedasModel, Monedas>
    {
        public MonedasConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override IEnumerable<IModelView> GetAll()
        {
            return _db.Set<Monedas>().Include(f => f.MonedasLog).ToList().Select(GetModelView);
        }

        public override IModelView CreateView(string id)
        {
            var intId = int.Parse(id);
            var obj = _db.Set<Monedas>().Where(f => f.id == intId).Include(f => f.MonedasLog).Single();
            return GetModelView(obj);
        }

        public override Monedas CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as MonedasModel;
            var result = _db.Set<Monedas>().Create();
            result.id = viewmodel.Id;
            result.descripcion = viewmodel.Descripcion;
            result.abreviatura = viewmodel.Abreviatura;
            result.cambiomonedabase = viewmodel.CambioMonedaBase;
            result.cambiomonedaadicional = viewmodel.CambioMonedaAdicional;
            result.decimales = viewmodel.Decimales;
            result.fechamodificacion = viewmodel.FechaModificacion;
            result.fkUsuariosnombre = viewmodel.Usuario;
            result.fkUsuarios = viewmodel.UsuarioId;
            result.activado = viewmodel.Activado;
            foreach (var item in viewmodel.Log)
            {
                var newItem = _db.MonedasLog.Create();
                newItem.cambiomonedabase = item.CambioMonedaBase;
                newItem.cambiomonedaadicional = item.CambioMonedaAdicional;
                newItem.fechamodificacion = item.FechaModificacion;
                newItem.fkUsuarios = item.UsuarioId;
                newItem.fkUsuariosnombre = item.Usuario;

                result.MonedasLog.Add(newItem);
            }

            return result;
        }

        public override Monedas EditPersitance(IModelView obj)
        {
            var viewmodel = obj as MonedasModel;
            var result = _db.Monedas.Where(f => f.id == viewmodel.Id).Include(b => b.MonedasLog).ToList().Single();
            result.descripcion = viewmodel.Descripcion;
            result.abreviatura = viewmodel.Abreviatura;
            result.cambiomonedabase = viewmodel.CambioMonedaBase;
            result.cambiomonedaadicional = viewmodel.CambioMonedaAdicional;
            result.decimales = viewmodel.Decimales;
            result.fechamodificacion = viewmodel.FechaModificacion;
            result.fkUsuariosnombre = viewmodel.Usuario;
            result.fkUsuarios = viewmodel.UsuarioId;
            result.activado = viewmodel.Activado;
            foreach (var item in viewmodel.Log)
            {
                var newItem = _db.MonedasLog.Create();
                newItem.cambiomonedabase = item.CambioMonedaBase;
                newItem.cambiomonedaadicional = item.CambioMonedaAdicional;
                newItem.fechamodificacion = item.FechaModificacion;
                newItem.fkUsuarios = item.UsuarioId;
                newItem.fkUsuariosnombre = item.Usuario;

                result.MonedasLog.Add(newItem);
            }

            return result;
        }

        public override IModelView GetModelView(Monedas obj)
        {
            var result = new MonedasModel
            {
                Id= obj.id,
                Descripcion = obj.descripcion,
                Abreviatura = obj.abreviatura,
                CambioMonedaBase = obj.cambiomonedabase ?? 0,
                CambioMonedaAdicional = obj.cambiomonedaadicional?? 0,
                Decimales = obj.decimales?? 0,
                FechaModificacion = obj.fechamodificacion ?? DateTime.Now,
                Usuario = obj.fkUsuariosnombre,
                UsuarioId = obj.fkUsuarios?? Guid.Empty,
                Activado= obj.activado??false,
                Log = obj.MonedasLog.Select(
                            item => new CambioMonedasLogModel()
                            {
                                Id = item.id,
                                CambioMonedaBase = item.cambiomonedabase ?? 0,
                                CambioMonedaAdicional = item.cambiomonedaadicional ??  0,
                                FechaModificacion = item.fechamodificacion ?? DateTime.Now,
                                Usuario = item.fkUsuariosnombre,
                                UsuarioId = item.fkUsuarios.Value
                            })

            };

            return result;
        }
    }
}
