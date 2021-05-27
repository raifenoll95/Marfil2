using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using System.Collections.Generic;
using System.Linq;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Documentos.CobrosYPagos;
using System;
using Marfil.Dom.Persistencia.Helpers;
using System.Data.Entity;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class VencimientosConverterService : BaseConverterModel<VencimientosModel, Persistencia.Vencimientos>
    {
        public VencimientosConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override IEnumerable<IModelView> GetAll()
        {
            return _db.Set<Persistencia.Vencimientos>().Select(GetModelView);
        }

        public override IModelView CreateView(string id)
        {
            var idnum = Int32.Parse(id);
            var obj = _db.Set<Persistencia.Vencimientos>().Where(f => f.empresa == Empresa && f.id == idnum).Include(f => f.PrevisionesCartera).Single();
            var result = GetModelView(obj) as VencimientosModel;
            return result;
        }

        public override Persistencia.Vencimientos CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as VencimientosModel;
            var result = _db.Set<Persistencia.Vencimientos>().Create();
            result.id = viewmodel.Id.Value;
            result.empresa = viewmodel.Empresa;
            result.fkseriescontables = viewmodel.Fkseriescontables;
            result.referencia = viewmodel.Referencia;
            result.identificadorsegmento = viewmodel.Identificadorsegmento;
            result.fecha = viewmodel.Fecha;
            result.traza = viewmodel.Traza;          
            result.tipo = (int)viewmodel.Tipo;
            result.origen = (int)viewmodel.Origen;
            result.usuario = Context.Usuario;
            result.fkcuentas = viewmodel.Fkcuentas;
            result.fechacreacion = DateTime.Now;
            result.fechafactura = viewmodel.Fechafactura;
            result.fecharegistrofactura = viewmodel.Fecharegistrofactura;
            result.fechavencimiento = viewmodel.Fechavencimiento;
            result.fechadescuento = viewmodel.Fechadescuento;
            result.fechapago = viewmodel.Fechapago;
            result.monedabase = viewmodel.Monedabase;
            result.monedagiro = viewmodel.Monedagiro;
            result.importegiro = viewmodel.Importegiro;
            //result.importefactura = viewmodel.Importefactura;
            //result.cambioaplicado = viewmodel.Cambioaplicado;
            result.monedafactura = viewmodel.Monedafactura;
            result.fkformaspago = viewmodel.Fkformaspago;
            result.fkcuentatesoreria = viewmodel.Fkcuentatesoreria;
            result.mandato = viewmodel.Mandato;
            result.importeasignado = viewmodel.Importeasignado;
            result.importepagado = viewmodel.Importepagado;
            result.estado = (int)viewmodel.Estado;
            result.situacion = viewmodel.Situacion;
            result.comentario = viewmodel.Comentario;
            return result;
        }

        public override Persistencia.Vencimientos EditPersitance(IModelView obj)
        {
            var viewmodel = obj as VencimientosModel;
            var result = _db.Vencimientos.Where(f => f.id == viewmodel.Id && f.empresa == Empresa).Single();
            result.empresa = viewmodel.Empresa;
            result.fkseriescontables = viewmodel.Fkseriescontables;
            result.referencia = viewmodel.Referencia;
            result.identificadorsegmento = viewmodel.Identificadorsegmento;
            result.fecha = viewmodel.Fecha;
            result.traza = viewmodel.Traza;
            result.tipo = (int)viewmodel.Tipo;
            result.origen = (int)viewmodel.Origen;
            result.usuario = viewmodel.Usuario;
            result.fkcuentas = viewmodel.Fkcuentas;
            result.fechacreacion = viewmodel.Fechacreacion;
            result.fechafactura = viewmodel.Fechafactura;
            result.fecharegistrofactura = viewmodel.Fecharegistrofactura;
            result.fechavencimiento = viewmodel.Fechavencimiento;
            result.fechadescuento = viewmodel.Fechadescuento;
            result.fechapago = viewmodel.Fechapago;
            result.monedabase = viewmodel.Monedabase;
            result.monedagiro = viewmodel.Monedagiro;
            result.importegiro = viewmodel.Importegiro;
            //result.importefactura = viewmodel.Importefactura;
            //result.cambioaplicado = viewmodel.Cambioaplicado;
            result.monedafactura = viewmodel.Monedafactura;
            result.fkcuentatesoreria = viewmodel.Fkcuentatesoreria;
            result.fkformaspago = viewmodel.Fkformaspago;
            result.mandato = viewmodel.Mandato;
            result.importeasignado = viewmodel.Importeasignado;
            result.importepagado = viewmodel.Importepagado;
            result.estado = (int)viewmodel.Estado;
            result.situacion = viewmodel.Situacion;
            result.comentario = viewmodel.Comentario;
            return result;
        }

        public override IModelView GetModelView(Persistencia.Vencimientos viewmodel)
        {

            var result = Helper.fModel.GetModel<VencimientosModel>(Context);
            result.Empresa = viewmodel.empresa;
            result.Id = viewmodel.id;
            result.Traza = viewmodel.traza;
            result.Fkseriescontables = viewmodel.fkseriescontables;
            result.Referencia = viewmodel.referencia;
            result.Identificadorsegmento = viewmodel.identificadorsegmento;
            result.Fecha = viewmodel.fecha;
            result.Tipo = (TipoVencimiento)viewmodel.tipo;
            result.Origen = (TipoOrigen)viewmodel.origen;
            result.Usuario = viewmodel.usuario;
            result.Fkcuentas = viewmodel.fkcuentas;
            result.Fechacreacion = viewmodel.fechacreacion;
            result.Fechafactura = viewmodel.fechafactura;
            result.Fecharegistrofactura = viewmodel.fecharegistrofactura;
            result.Fechavencimiento = viewmodel.fechavencimiento;
            result.Fechadescuento = viewmodel.fechadescuento;
            result.Fechapago = viewmodel.fechapago;
            result.Monedabase = viewmodel.monedabase;
            result.Monedagiro = viewmodel.monedagiro;
            result.Importegiro = viewmodel.importegiro;
            result.Monedafactura = viewmodel.monedafactura;
            result.Fkcuentatesoreria = viewmodel.fkcuentatesoreria;
            result.Fkformaspago = viewmodel.fkformaspago;
            result.Mandato = viewmodel.mandato;
            result.Importeasignado = viewmodel.importeasignado;
            result.Importepagado = viewmodel.importepagado;
            result.Estado = (TipoEstado)viewmodel.estado;
            result.Situacion = viewmodel.situacion;
            result.Comentario = viewmodel.comentario;
           
            return result;
        }
    }
}
