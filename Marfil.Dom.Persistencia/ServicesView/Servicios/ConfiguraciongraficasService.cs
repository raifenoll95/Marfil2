using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DevExpress.XtraCharts;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Listados;
using Marfil.Dom.Persistencia.Listados.Base;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Graficaslistados;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Converter;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Preferencias;
using Marfil.Inf.Genericos.Helper;
using Resources;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IConfiguraciongraficasService
    {

    }

    public class ConfiguraciongraficasService : GestionService<ConfiguraciongraficasModel, Configuraciongraficas>, IConfiguraciongraficasService
    {


        #region CTR

        public ConfiguraciongraficasService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        #region Api

        public IEnumerable<ConfiguraciongraficasModel> GetConfiguracionesListado(string idlistado,Guid usuario)
        {
            if (idlistado == null)
                return _db.Configuraciongraficas.Where(f => f.empresa == Empresa && f.usuario == usuario).ToList().Select(f => _converterModel.GetModelView(f) as ConfiguraciongraficasModel);
            else
                return _db.Configuraciongraficas.Where(f => f.empresa == Empresa && f.idlistado == idlistado && f.usuario == usuario).ToList().Select(f=> _converterModel.GetModelView(f) as ConfiguraciongraficasModel);
        }

        public ConfiguraciongraficasModel CrearNuevoModel(ListadosModel listadoModel,string ejercicio)
        {
            var serviceConverter = _converterModel as ConfiguraciongraficasConverterService;
            return serviceConverter.CrearNuevoModelo(listadoModel, ejercicio);
        }

        public GraficasModel CargarGrafica(string id)
        {
            var converterService = _converterModel as ConfiguraciongraficasConverterService;


            return converterService.GetGraficaModel(id);




        }

        public static  bool GraficaXy(ViewType tipo)
        {

            return (tipo == ViewType.Area || tipo == ViewType.Bubble || tipo == ViewType.FullStackedArea ||
                    tipo == ViewType.FullStackedLine || tipo == ViewType.FullStackedArea
                    || tipo == ViewType.Line || tipo == ViewType.RangeBar || tipo == ViewType.Point ||
                    tipo == ViewType.SideBySideFullStackedBar || tipo == ViewType.SideBySideRangeBar ||
                    tipo == ViewType.SideBySideStackedBar
                    || tipo == ViewType.Spline || tipo == ViewType.SplineArea || tipo == ViewType.StackedArea ||
                    tipo == ViewType.StackedArea || tipo == ViewType.StackedBar
                    || tipo == ViewType.StackedSplineArea || tipo == ViewType.StepLine || tipo == ViewType.Stock);


        }

        public override void create(IModelView obj)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                var newItem = _converterModel.CreatePersitance(obj);
                if (_validationService.ValidarGrabar(newItem))
                {
                    _db.Set<Configuraciongraficas>().Add(newItem);
                    try
                    {
                        _db.SaveChanges();
                        obj.set("Codigo",newItem.id);
                    }
                    catch (DbUpdateException ex)
                    {
                        if (ex.InnerException != null
                            && ex.InnerException.InnerException != null)
                        {
                            var inner = ex.InnerException.InnerException as SqlException;
                            if (inner != null)
                            {
                                if (inner.Number == 2627 || inner.Number == 2601)
                                {
                                    throw new ValidationException(General.ErrorRegistroExistente);
                                }
                            }
                        }


                        throw;
                    }

                }

                ActualizarPreferencias(obj as ConfiguraciongraficasModel);
                tran.Complete();
            }
        }

        public override void edit(IModelView obj)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                base.edit(obj);
                ActualizarPreferencias(obj as ConfiguraciongraficasModel);
                tran.Complete();
            }
        }

        public override void delete(IModelView obj)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {

                base.delete(obj);
                EliminarPreferencias(obj as ConfiguraciongraficasModel);
                tran.Complete();
            }
        }

        public void ActualizarOrdenPanelControl(IEnumerable<StOrdenPanelControl> modelo)
        {
            var service = new PreferenciasUsuarioService(_db);
            var user = _context;
            var preferencia = service.GePreferencia(TiposPreferencias.PanelControlDefecto, user.Id, "1", "Defecto") as PreferenciaPanelControlDefecto ?? new PreferenciaPanelControlDefecto();
            preferencia.SetPanelControl(Empresa, modelo);
            service.SetPreferencia(TiposPreferencias.PanelControlDefecto, user.Id, "1", "Defecto", preferencia,true);
        }

        #endregion

        #region Helper

        private void ActualizarPreferencias(ConfiguraciongraficasModel configuraciongraficasModel)
        {
            var service = new PreferenciasUsuarioService(_db);
            var user =_context;
            var preferencia = service.GePreferencia(TiposPreferencias.PanelControlDefecto, user.Id, "1", "Defecto") as PreferenciaPanelControlDefecto ?? new PreferenciaPanelControlDefecto();
            if (configuraciongraficasModel.Apareceinicioview)
            {
                preferencia.SetPanelControl(Empresa, new StOrdenPanelControl() { Grafica = configuraciongraficasModel.Codigo.ToString() , Indice = -1});
            }
            else
            {
                preferencia.DeletePanelControl(Empresa, configuraciongraficasModel.Codigo.ToString());
            }
            
            service.SetPreferencia(TiposPreferencias.PanelControlDefecto, user.Id, "1", "Defecto", preferencia, true);
        }

        private void EliminarPreferencias(ConfiguraciongraficasModel configuraciongraficasModel)
        {
            var service = new PreferenciasUsuarioService(_db);
            var user = _context;
            var preferencia = service.GePreferencia(TiposPreferencias.PanelControlDefecto, user.Id, "1", "Defecto") as PreferenciaPanelControlDefecto;
            if (preferencia != null)
            {
                preferencia.DeletePanelControl(Empresa,configuraciongraficasModel.Codigo.ToString());
                service.SetPreferencia(TiposPreferencias.PanelControlDefecto, user.Id, "1", "Defecto", preferencia,true);
            }
        }

        #endregion

    }
}
