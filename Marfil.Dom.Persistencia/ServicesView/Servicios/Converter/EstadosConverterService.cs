using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    class EstadosConverterService : BaseConverterModel<EstadosModel, Estados>
    {
        public EstadosConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override IModelView CreateView(string id)
        {
            var vector = id.Split('-');
            var documento = int.Parse(vector[0]);
            var idestado = vector[1];
            var obj = _db.Set<Estados>().Single(f => f.id == idestado && f.documento == documento);
            return GetModelView(obj);
        }

        public override bool Exists(string id)
        {
            var vector = id.Split('-');
            var documento = int.Parse(vector[0]);
            var idestado = vector[1];
            return _db.Set<Estados>().Any(f => f.id == idestado && f.documento == documento);
        }

        public override Estados EditPersitance(IModelView obj)
        {
            var viewmodel = obj as EstadosModel;
            var result = _db.Set<Estados>().Single(f => f.id == viewmodel.Id && f.documento == (int)viewmodel.Documento);
            result.descripcion = viewmodel.Descripcion;
            result.notas = viewmodel.Notas;
            result.tipoestado = (int)viewmodel.Tipoestado;
            result.imputariesgo = viewmodel.Imputariesgo;
            result.tipomovimiento = (int) viewmodel.Tipomovimiento;
            return result;
        }

        public override IModelView GetModelView(Estados obj)
        {
            var result = base.GetModelView(obj) as EstadosModel;
            result.Documento = (DocumentoEstado)obj.documento;
            result.Tipoestado = (TipoEstado)obj.tipoestado;
            result.Tipomovimiento = (Model.Configuracion.TipoMovimiento)obj.tipomovimiento;
            return result;
        }
    }
}
