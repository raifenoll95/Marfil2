using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Contabilidad;

using RSeriesContables = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.SeriesContables;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface ISeriesContablesService
    {

    }

    public class SeriesContablesService : GestionService<SeriesContablesModel, SeriesContables>, ISeriesContablesService
    {
        #region CTR

        public SeriesContablesService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        public void Bloquear(string id, string motivo, string user, bool bloqueado)
        {
            var vector = id.Split('-');
            var tipodocumento = vector[0];
            var codigo = vector[1];
            // var cuenta = _db.SeriesContables.Single(f => f.empresa == Empresa && f.id == codigo );
            var cuenta = _db.SeriesContables.Single(f => f.empresa == Empresa && f.id == codigo && f.tipodocumento == tipodocumento);
            cuenta.fkmotivosbloqueo = motivo;
            cuenta.fkusuariobloqueo = new Guid(user);
            cuenta.fechamodificacionbloqueo = DateTime.Now;
            cuenta.bloqueada = bloqueado;
            _db.Entry(cuenta).State = EntityState.Modified;
            _db.SaveChanges();
        }

        #region ListIndex

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var st = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            st.ExcludedColumns = new[] { "Bloqueo", "Fkserieasociadatipodocumento", "CustomId", "Empresa", "Fkmonedas", "Fkregimeniva", "Fkcontadores", "Fkejercicios", "Fkseriesasociada", "Bloqueado", "Toolbar" };
            st.BloqueoColumn = "Bloqueado";
            st.PrimaryColumnns = new[] { "CustomId" };
            return st;
        }

        public override string GetSelectPrincipal()
        {
            return string.Format("select s.*,e.descripcioncorta as Fkejerciciosdescripcion from SeriesContables as s " +
                                 " left join Ejercicios as e on e.empresa = s.empresa and s.fkejercicios = e.id " +
                   " where s.empresa='{0}'", Empresa);
        }

        #endregion

        #region Edit

        public override void edit(IModelView obj)
        {
            var model = obj as SeriesContablesModel;
            if (!ValidarContador(model))
                throw new ValidationException(RSeriesContables.ErrorSerieYaUtilizada);
            base.edit(obj);
        }

        private bool ValidarContador(SeriesContablesModel model)
        {
            var modelBd = get(model.CustomId) as SeriesContablesModel;


            if (model.Fkcontadores != modelBd.Fkcontadores)
            {
                        return !_db.Movs.Any(f => f.empresa == model.Empresa);
            }

            return true;
        }

        #endregion


        public IEnumerable<SeriesContablesModel> GetAll()
        {
            
            return _db.SeriesContables.Where(f => f.empresa == Empresa ).ToList().Select(f => _converterModel.GetModelView(f) as SeriesContablesModel);
        }

        public IEnumerable<SeriesContablesModel> Get(string id)
        {

            return _db.SeriesContables.Where(f => f.empresa == Empresa && f.id == id).ToList().Select(f => _converterModel.GetModelView(f) as SeriesContablesModel);
        }

        public List<SeriesContablesModel> getSerie(string tipo)
        {

            List<SeriesContablesModel> series = new List<SeriesContablesModel>();

            if (tipo == "0" || tipo == "CRC")
            {
                series.AddRange(_db.SeriesContables.Where(f => f.empresa == Empresa && f.tipodocumento == "CRC").
                    Select(f => new SeriesContablesModel() { Tipodocumento = f.tipodocumento, Id = f.id, Descripcion = f.descripcion }).ToList());
            }
            else if (tipo == "X")
            {
                series.AddRange(_db.SeriesContables.Where(f => f.empresa == Empresa && f.tipodocumento == "PRC").
                    Select(f => new SeriesContablesModel() { Tipodocumento = f.tipodocumento, Id = f.id, Descripcion = f.descripcion }).ToList());
            }
            else if (tipo == "1" || tipo == "CRP")
            {
                series.AddRange(_db.SeriesContables.Where(f => f.empresa == Empresa && f.tipodocumento == "CRP").
                    Select(f => new SeriesContablesModel() { Tipodocumento = f.tipodocumento, Id = f.id, Descripcion = f.descripcion }).ToList());
            }
            else if (tipo == "Y")
            {
                series.AddRange(_db.SeriesContables.Where(f => f.empresa == Empresa && f.tipodocumento == "PRP").
                    Select(f => new SeriesContablesModel() { Tipodocumento = f.tipodocumento, Id = f.id, Descripcion = f.descripcion }).ToList());
            }
            else
            {
                series.AddRange(_db.SeriesContables.Where(f => f.empresa == Empresa && f.tipodocumento == tipo).
                    Select(f => new SeriesContablesModel() { Tipodocumento = f.tipodocumento, Id = f.id, Descripcion = f.descripcion }).ToList());
            }

            return series;            
        }


        public string getSerieEjercicio(string tipodocumento)
        {
            string id = "";
            var ejercicio = Int32.Parse(_context.Ejercicio);

            if (tipodocumento == "PRC")
            {
                id = _db.Ejercicios.Where(f => f.empresa == Empresa && f.id == ejercicio).Select(f => f.fkseriescontablesPRC).SingleOrDefault();
            }

            if (tipodocumento == "PRP")
            {
                id = _db.Ejercicios.Where(f => f.empresa == Empresa && f.id == ejercicio).Select(f => f.fkseriescontablesPRP).SingleOrDefault();
            }

            return id;
        }

        //public static string GetSerieContableCodigo(TipoDocumento tipo)
        //{
        //    switch (tipo)
        //    {

        //        case TipoDocumento.Normal:
        //            return "F1";
        //        case TipoDocumento.Simulacion:
        //            return "F2";
        //        case TipoDocumento.AsientoVinculado:
        //            return "F3";
        //        case TipoDocumento.AperturaProvisional:
        //            return "R1";
        //        case TipoDocumento.Apertura:
        //            return "R2";
        //        case TipoDocumento.RegularizacionExistencias:
        //            return "R3";
        //        case TipoDocumento.RegularizacionGrupos6y7:
        //            return "R4";
        //        case TipoDocumento.Cierre:
        //            return "R5";
        //        default:
        //            throw new ValidationException("No existe el tipo de asiento");

        //    }
        //}
    }
}

