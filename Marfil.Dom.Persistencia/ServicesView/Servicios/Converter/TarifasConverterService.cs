using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.ControlsUI.Bloqueo;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using RTarifas =Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Tarifas;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class TarifasConverterService : BaseConverterModel<TarifasModel, Tarifas>
    {
        public TarifasConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override bool Exists(string id)
        {
            return _db.Set<Tarifas>().Any(f => f.id == id && f.empresa == Empresa);
        }

        public override IEnumerable<IModelView> GetAll()
        {
            return _db.Tarifas.Where(f => f.empresa == Empresa).ToList().Select(f => GetModelView(f));
        }

        public override IModelView CreateView(string id)
        {
            var obj = _db.Set<Tarifas>().Include("TarifasLin").Single(f => f.id == id && f.empresa == Empresa);
            var result=  GetModelView(obj) as TarifasModel;
            
            result.Lineas = obj.TarifasLin.ToList().Select(f => new TarifasLinModel() { Descuento = f.descuento??0, Fkarticulos = f.fkarticulos,Precio = f.precio??0, Unidades = _db.Unidades.FirstOrDefault(j=>j.id==f.Unidades)?.codigounidad}).ToList();
            var motivobloquedescripcion = string.Empty;
            if (!string.IsNullOrEmpty(obj.fkMotivosbloqueo))
            {
                var service = new TablasVariasService(Context, _db);
                var motivosbloqueo = service.GetTablasVariasByCode(12);
                motivobloquedescripcion = motivosbloqueo.Lineas.Single(f => f.Valor == obj.fkMotivosbloqueo).Descripcion;
            }
            result.BloqueoModel = new BloqueoEntidadModel
            {
                Entidad = RTarifas.TituloEntidadSingular,
                Campoclaveprimaria = "Id",
                Valorclaveprimaria = result.Id,
                Bloqueada = obj.bloqueada.HasValue && obj.bloqueada.Value,
                FkMotivobloqueo = obj.fkMotivosbloqueo,
                FkMotivobloqueoDescripcion = motivobloquedescripcion,
                Fecha = obj.fechamodificacionbloqueo ?? DateTime.MinValue,
                FkUsuario = obj.fkUsuariobloqueo?.ToString() ?? string.Empty,
                FkUsuarioNombre =
                    obj.fkUsuariobloqueo == Guid.Empty
                        ? ApplicationHelper.UsuariosAdministrador
                        : _db.Usuarios.SingleOrDefault(f => f.id == obj.fkUsuariobloqueo.Value)?.usuario,
            };
            return result;
        }

        public override Tarifas EditPersitance(IModelView obj)
        {
            var viewmodel = obj as TarifasModel;
            var result = _db.Tarifas.Include("TarifasLin").Single(f => f.id == viewmodel.Id && f.empresa == viewmodel.Empresa);

            foreach (var item in result.GetType().GetProperties())
            {
                if (typeof(TarifasModel).GetProperties().Any(f => f.Name.ToLower() == item.Name.ToLower()) && !item.Name.ToLower().Equals("tipoflujo") && !item.Name.ToLower().Equals("tipotarifa"))
                    item.SetValue(result, viewmodel.get(item.Name));
            }

            result.bloqueada = viewmodel.BloqueoModel?.Bloqueada;
            result.fkMotivosbloqueo = viewmodel.BloqueoModel?.FkMotivobloqueo;
            if (!string.IsNullOrEmpty(viewmodel.BloqueoModel?.FkUsuario))
            {
                result.fechamodificacionbloqueo = viewmodel.BloqueoModel?.Fecha;
                result.fkUsuariobloqueo = new Guid(viewmodel.BloqueoModel?.FkUsuario);
            }

            result.TarifasLin.Clear();
            foreach (var item in viewmodel.Lineas)
            {
                var newItem = _db.TarifasLin.Create();

                newItem.empresa = viewmodel.Empresa;
                newItem.fktarifas = viewmodel.Id;
                newItem.fkarticulos = item.Fkarticulos;
                newItem.precio = item.Precio;
                newItem.descuento = item.Descuento;

                result.TarifasLin.Add(newItem);
            }
            return result;
        }

        public override IModelView GetModelView(Tarifas obj)
        {
            var result = base.GetModelView(obj) as TarifasModel;

            var motivobloquedescripcion = string.Empty;
            result.BloqueoModel = new BloqueoEntidadModel
            {
                Entidad = RTarifas.TituloEntidadSingular,
                Campoclaveprimaria = "Id",
                Valorclaveprimaria = result.Id,
                Bloqueada = obj.bloqueada.HasValue && obj.bloqueada.Value,
                FkMotivobloqueo = obj.fkMotivosbloqueo,
                FkMotivobloqueoDescripcion = motivobloquedescripcion,
                Fecha = obj.fechamodificacionbloqueo ?? DateTime.MinValue,
                FkUsuario = obj.fkUsuariobloqueo?.ToString() ?? string.Empty
            };

            return result;
        }
    }
}
