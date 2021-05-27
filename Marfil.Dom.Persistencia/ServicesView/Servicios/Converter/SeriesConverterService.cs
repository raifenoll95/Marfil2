using Marfil.Dom.Persistencia.Model.Configuracion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.ControlsUI.Bloqueo;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos.Helper;

using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class SeriesConverterService : BaseConverterModel<SeriesModel, Series>
    {
        public SeriesConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        #region API

        public override IEnumerable<IModelView> GetAll()
        {
            var list = _db.Series.Where(f => f.empresa == Empresa).ToList().Select(f => GetModelView(f) as SeriesModel).ToList();
            foreach (var item in list)
                if (!string.IsNullOrEmpty(item.Fkejercicios))
                {
                    var idejercicio = Funciones.Qint(item.Fkejercicios);
                    item.Fkejerciciosdescripcion =
                        _db.Ejercicios.Single(j => j.empresa == Empresa && j.id == idejercicio.Value).descripcion;
                }

            return list;

        }

        public override bool Exists(string id)
        {
            var vector = id.Split('-');
            var tipodocumento = vector[0];
            var codigo = vector[1];
            return _db.Set<Series>().Any(f => f.id == codigo && f.tipodocumento == tipodocumento && f.empresa == Empresa);
        }

        public override IModelView CreateView(string id)
        {
            var fmodel = new FModel();
            var objExt = fmodel.GetModel<SeriesModel>(Context) as IModelViewExtension;
            var vector = objExt.generateId(id) as string[];
            var tipodocumento = vector[0];
            var codigo = vector[1];

            var obj = _db.Set<Series>().Single(f => f.tipodocumento == tipodocumento && f.id == codigo && f.empresa == Empresa);
            var result = GetModelView(obj) as SeriesModel;

            var service = new TablasVariasService(Context, _db);
            var motivobloquedescripcion = string.Empty;
            if (!string.IsNullOrEmpty(obj.fkmotivosbloqueo))
            {
                var motivosbloqueo = service.GetTablasVariasByCode(12);
                motivobloquedescripcion = motivosbloqueo.Lineas.Single(f => f.Valor == obj.fkmotivosbloqueo).Descripcion;
            }
            result.Bloqueo = new BloqueoEntidadModel
            {
                Entidad = Inf.ResourcesGlobalization.Textos.Entidades.Series.TituloEntidadSingular,
                Campoclaveprimaria = "Id",
                Valorclaveprimaria = result.CustomId,
                Bloqueada = obj.bloqueada.HasValue && obj.bloqueada.Value,
                FkMotivobloqueo = obj.fkmotivosbloqueo,
                FkMotivobloqueoDescripcion = motivobloquedescripcion,
                Fecha = obj.fechamodificacionbloqueo ?? DateTime.MinValue,
                FkUsuario = obj.fkusuariobloqueo?.ToString() ?? string.Empty,
                FkUsuarioNombre =
                    obj.fkusuariobloqueo == Guid.Empty
                        ? ApplicationHelper.UsuariosAdministrador
                        : _db.Usuarios.SingleOrDefault(f => f.id == obj.fkusuariobloqueo.Value)?.usuario,
            };
            return result;
        }

        public override Series CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as SeriesModel;
            var result = _db.Series.Create();

            foreach (var item in result.GetType().GetProperties())
            {
                if (typeof(SeriesModel).GetProperties().Any(f => f.Name.ToLower() == item.Name.ToLower()) && item.Name.ToLower() != "tipoimpresion")
                    item.SetValue(result, viewmodel.get(item.Name));
            }

            result.tipoimpresion = (int)viewmodel.Tipoimpresion;

            result.bloqueada = viewmodel.Bloqueo?.Bloqueada;
            result.fkmotivosbloqueo = viewmodel.Bloqueo?.FkMotivobloqueo;
            if (!string.IsNullOrEmpty(viewmodel.Bloqueo?.FkUsuario))
            {
                result.fechamodificacionbloqueo = viewmodel.Bloqueo?.Fecha;
                result.fkusuariobloqueo = new Guid(viewmodel.Bloqueo?.FkUsuario);
            }


            result.tipoalmacenlote = (int?)viewmodel.Tipodealmacenlote;

            return result;
        }

        public override Series EditPersitance(IModelView obj)
        {
            var viewmodel = obj as SeriesModel;
            var result = _db.Series.Single(f => f.tipodocumento == viewmodel.Tipodocumento && f.id == viewmodel.Id && f.empresa == viewmodel.Empresa);

            foreach (var item in result.GetType().GetProperties())
            {
                if (typeof(SeriesModel).GetProperties().Any(f => f.Name.ToLower() == item.Name.ToLower()) && item.Name.ToLower() != "tipoimpresion")
                    item.SetValue(result, viewmodel.get(item.Name));
            }
            result.tipoimpresion = (int)viewmodel.Tipoimpresion;


            result.tipoalmacenlote = (int?)viewmodel.Tipodealmacenlote;

            return result;
        }

        public override IModelView GetModelView(Series obj)
        {
            var result = base.GetModelView(obj) as SeriesModel;
            result.Bloqueo = new BloqueoEntidadModel()
            {
                Entidad = Inf.ResourcesGlobalization.Textos.Entidades.Series.TituloEntidadSingular
            };
            result.Bloqueo.Bloqueada = obj.bloqueada ?? false;
            result.Bloqueo.FkMotivobloqueo = obj.fkmotivosbloqueo;
            result.Bloqueo.FkUsuario = obj.fkusuariobloqueo.ToString();
            result.Bloqueo.Fecha = obj.fechamodificacionbloqueo ?? DateTime.MinValue;
            result.Tipodealmacenlote = (TipoAlmacenlote?)obj.tipoalmacenlote;
            return result;
        }

        #endregion
    }
}
