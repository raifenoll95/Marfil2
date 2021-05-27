using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos.StateMachine;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public enum TipoMovimientos
    {
        Todos,
        Manuales
    }

    public interface IEstadosService
    {

    }

    public class EstadosService : GestionService<EstadosModel, Estados>, IEstadosService
    {
        #region CTR

        public EstadosService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        #region Index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var st= base.GetListIndexModel(t, canEliminar, canModificar, controller);

            var propiedadesVisibles = new[] { "Documento", "Id", "Descripcion" };
            var propiedades = Helpers.Helper.getProperties<EstadosModel>();
            st.PrimaryColumnns = new[] { "CampoId" };
            st.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            
            return st;
        }

        public override string GetSelectPrincipal()
        {
            return "select id,documento,(Concat(documento,'-', id)) as [CampoId],descripcion as Descripcion from Estados";
        }

        #endregion

        public IEnumerable<EstadosModel> GetStates(DocumentoEstado documento,TipoEstado? estadoactual)
        {
            var service=new MachineStateService();
            var estados = estadoactual.HasValue
                ? service.GetStatesFromState(estadoactual.Value)
                : new[] {TipoEstado.Diseño};

            return _db.Estados.Where(f => f.tipomovimiento == (int)Model.Configuracion.TipoMovimiento.Manual && (f.documento == (int)documento || f.documento==(int)DocumentoEstado.Todos) && estados.Any(j => j == (TipoEstado) f.tipoestado)).ToList().Select(h=>_converterModel.GetModelView(h) as EstadosModel);
        }

        
        public IEnumerable<EstadosModel> GetStates(DocumentoEstado documento, TipoMovimientos tiposmovimietos = TipoMovimientos.Manuales)
        {
            
            return tiposmovimietos ==TipoMovimientos.Todos ? _db.Estados.Where(f => (f.documento == (int)documento || f.documento == (int)DocumentoEstado.Todos)).ToList().Select(h => _converterModel.GetModelView(h) as EstadosModel):
                _db.Estados.Where(f => f.tipomovimiento == (int)Model.Configuracion.TipoMovimiento.Manual && (f.documento == (int)documento || f.documento == (int)DocumentoEstado.Todos)).ToList().Select(h => _converterModel.GetModelView(h) as EstadosModel);
        }


    }
}
