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
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using RSeries = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Series;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface ISeriesService
    {

    }

    public class SeriesService : GestionService<SeriesModel, Series>, ISeriesService
    {
        #region CTR

        public SeriesService(IContextService context, MarfilEntities db = null) : base(context, db)
        {
            
        }

        #endregion

        public void Bloquear(string id, string motivo, string user, bool bloqueado)
        {
            var vector = id.Split('-');
            var tipodocumento = vector[0];
            var codigo = vector[1];
            var cuenta = _db.Series.Single(f => f.empresa == Empresa && f.id == codigo  && f.tipodocumento== tipodocumento);
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
            var st= base.GetListIndexModel(t, canEliminar, canModificar, controller);
            st.ExcludedColumns = new[] {"Bloqueo","Fkserieasociadatipodocumento", "CustomId", "Empresa","Fkmonedas","Fkregimeniva","Fkcontadores","Fkejercicios", "Fkseriesasociada", "Bloqueado", "Fkgruposusuarios", "Toolbar" };
            st.BloqueoColumn = "Bloqueado";
            st.PrimaryColumnns = new[] { "CustomId" };
            return st;
        }

        public override string GetSelectPrincipal()
        {
            return string.Format("select s.*,e.descripcioncorta as Fkejerciciosdescripcion from series as s " +
                                 " left join Ejercicios as e on e.empresa = s.empresa and s.fkejercicios = e.id " +
                   " where s.empresa='{0}'",Empresa);
        }

        #endregion

        #region Edit

        public override void edit(IModelView obj)
        {
            var model = obj as SeriesModel;
            if(!ValidarContador(model))
                throw new ValidationException(RSeries.ErrorSerieYaUtilizada);
            base.edit(obj);
        }

        private bool ValidarContador(SeriesModel model)
        {
            var modelBd = get(model.CustomId) as SeriesModel;
            if (model.Fkcontadores != modelBd.Fkcontadores)
            {
                switch (model.Tipodocumento)
                {
                    case "PRE":
                        return !_db.Presupuestos.Any(f => f.empresa==model.Empresa);
                    case "PED":
                        return !_db.Pedidos.Any(f => f.empresa == model.Empresa);
                    case "ALB":
                         return !_db.Albaranes.Any(f => f.empresa == model.Empresa);
                    case "FRA":
                        return !_db.Facturas.Any(f => f.empresa == model.Empresa);
                    case "PRC":
                        return !_db.PresupuestosCompras.Any(f => f.empresa == model.Empresa);
                    case "PEC":
                        return !_db.PedidosCompras.Any(f => f.empresa == model.Empresa);
                    case "ALC":
                        return !_db.AlbaranesCompras.Any(f => f.empresa == model.Empresa);
                    case "FRC":
                        return !_db.FacturasCompras.Any(f => f.empresa == model.Empresa);
                    case "RES":
                        return !_db.Reservasstock.Any(f => f.empresa == model.Empresa);
                    case "TRA":
                        return !_db.Traspasosalmacen.Any(f => f.empresa == model.Empresa);
                    case "INV":
                        return !_db.Inventarios.Any(f => f.empresa == model.Empresa);
                    case "TRF":
                        return !_db.Transformaciones.Any(f => f.empresa == model.Empresa);
                    case "TRL":
                        return !_db.Transformacioneslotes.Any(f => f.empresa == model.Empresa);
                    case "KIT":
                        return !_db.Kit.Any(f => f.empresa == model.Empresa);
                }
            }

            return true;
        }

       

        #endregion

        public IEnumerable<SeriesModel> GetSeriesTipoDocumento(TipoDocumento tipo)
        {
            var tipodocumento = GetSerieCodigo(tipo);
            return _db.Series.Where(f => f.empresa == Empresa && f.tipodocumento == tipodocumento).ToList().Select(f=>_converterModel.GetModelView(f) as SeriesModel);
        }

        public static string GetSerieCodigo(TipoDocumento tipo)
        {
            switch (tipo)
            {
                case TipoDocumento.PresupuestosVentas:
                    return "PRE";
                case TipoDocumento.PedidosVentas:
                    return "PED";
                case TipoDocumento.AlbaranesVentas:
                    return "ALB";
                case TipoDocumento.FacturasVentas:
                    return "FRA";
                case TipoDocumento.PresupuestosCompras:
                    return "PRC";
                case TipoDocumento.PedidosCompras:
                    return "PEC";
                case TipoDocumento.AlbaranesCompras:
                    return "ALC";
                case TipoDocumento.FacturasCompras:
                    return "FRC";
                case TipoDocumento.Reservas:
                    return "RES";
                case TipoDocumento.Traspasosalmacen:
                    return "TRA";
                case TipoDocumento.Inventarios:
                    return "INV";
                case TipoDocumento.Transformaciones:
                    return "TRF";
                case TipoDocumento.Transformacioneslotes:
                    return "TRL";
                case TipoDocumento.Kits:
                    return "KIT";
                default:
                    throw new ValidationException("No existe el tipo de documento");

            }
        }
    }
}
