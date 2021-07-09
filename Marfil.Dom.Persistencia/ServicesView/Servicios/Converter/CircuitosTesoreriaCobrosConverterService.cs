using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using System.Collections.Generic;
using System.Linq;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Documentos.CobrosYPagos;
using System;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class CircuitosTesoreriaConverterCobrosConverterService : BaseConverterModel<CircuitoTesoreriaCobrosModel, Persistencia.CircuitosTesoreriaCobros>
    {
        public CircuitosTesoreriaConverterCobrosConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override IEnumerable<IModelView> GetAll()
        {
            return _db.Set<Persistencia.CircuitosTesoreriaCobros>().Select(GetModelView);
        }

        public override IModelView CreateView(string id)
        {
            var idnum = Int32.Parse(id);
            var obj = _db.Set<Persistencia.CircuitosTesoreriaCobros>().Single(f => f.empresa == Empresa && f.id == idnum);
            var result = GetModelView(obj) as CircuitoTesoreriaCobrosModel;
            return result;
        }

        public override Persistencia.CircuitosTesoreriaCobros CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as CircuitoTesoreriaCobrosModel;
            var result = _db.Set<Persistencia.CircuitosTesoreriaCobros>().Create();
            result.empresa = viewmodel.Empresa;
            result.descripcion = viewmodel.Descripcion;          
            result.situacioninicial = viewmodel.Situacioninicial;
            result.situacionfinal = viewmodel.Situacionfinal;
            result.datos = viewmodel.Datos;
            result.fkmodopagopreferido = viewmodel.Fkmodopagopreferido;
            result.asientocontable = viewmodel.Asientocontable;
            result.documentocartera = viewmodel.Documentocartera;
            result.fecharemesa = viewmodel.Fecharemesa;
            result.fechapago = viewmodel.Fechapago;
            result.liquidariva = viewmodel.Liquidariva;
            result.conciliacion = viewmodel.Conciliacion;
            result.datosdocumento = viewmodel.Datosdocumento;
            result.cuentacargo1 = viewmodel.Cuentacargo1;
            result.cuentacargo2 = viewmodel.Cuentacargo2;
            result.cuentacargorel = viewmodel.Cuentacargorel;
            result.cuentaabono1 = viewmodel.Cuentaabono1;
            result.cuentaabono2 = viewmodel.Cuentaabono2;
            result.cuentaabonorel = viewmodel.Cuentaabonorel;
            result.importecuentacargo1 = (int)viewmodel.Importecuentacargo1;
            result.importecuentacargo2 = (int)viewmodel.Importecuentacargo2;
            result.importecuentacargorel = (int)viewmodel.Importecuentacargorel;
            result.importecuentaabono1 = (int)viewmodel.Importecuentaabono1;
            result.importecuentaabono2 = (int)viewmodel.Importecuentaabono2;
            result.importecuentaabonorel = (int)viewmodel.Importecuentaabonorel;
            result.desccuentacargo1 = viewmodel.Desccuentacargo1;
            result.desccuentacargo2 = viewmodel.Desccuentacargo2;
            result.desccuentacargorel = viewmodel.Desccuentacargorel;
            result.desccuentaabono1 = viewmodel.Desccuentaabono1;
            result.desccuentaabono2 = viewmodel.Desccuentaabono2;
            result.desccuentaabonorel = viewmodel.Desccuentaabonorel;
            result.tipocircuito = (int)viewmodel.Tipocircuito;
            result.codigodescripcionasiento = viewmodel.Codigodescripcionasiento;
            result.actualizarcobrador = viewmodel.Actualizarcobrador;
            return result;
        }

        public override Persistencia.CircuitosTesoreriaCobros EditPersitance(IModelView obj)
        {
            var viewmodel = obj as CircuitoTesoreriaCobrosModel;
            var result = _db.CircuitosTesoreriaCobros.Where(f => f.id == viewmodel.Id && f.empresa == Empresa).Single();
            result.empresa = viewmodel.Empresa;
            result.descripcion = viewmodel.Descripcion;
            result.situacioninicial = viewmodel.Situacioninicial;
            result.situacionfinal = viewmodel.Situacionfinal;
            result.datos = viewmodel.Datos;
            result.fkmodopagopreferido = viewmodel.Fkmodopagopreferido;
            result.documentocartera = viewmodel.Documentocartera;
            result.asientocontable = viewmodel.Asientocontable;
            result.fecharemesa = viewmodel.Fecharemesa;
            result.fechapago = viewmodel.Fechapago;
            result.liquidariva = viewmodel.Liquidariva;
            result.conciliacion = viewmodel.Conciliacion;
            result.datosdocumento = viewmodel.Datosdocumento;
            result.cuentacargo1 = viewmodel.Cuentacargo1;
            result.cuentacargo2 = viewmodel.Cuentacargo2;
            result.cuentacargorel = viewmodel.Cuentacargorel;
            result.cuentaabono1 = viewmodel.Cuentaabono1;
            result.cuentaabono2 = viewmodel.Cuentaabono2;
            result.cuentaabonorel = viewmodel.Cuentaabonorel;
            result.importecuentacargo1 = (int)viewmodel.Importecuentacargo1;
            result.importecuentacargo2 = (int)viewmodel.Importecuentacargo2;
            result.importecuentacargorel = (int)viewmodel.Importecuentacargorel;
            result.importecuentaabono1 = (int)viewmodel.Importecuentaabono1;
            result.importecuentaabono2 = (int)viewmodel.Importecuentaabono2;
            result.importecuentaabonorel = (int)viewmodel.Importecuentaabonorel;
            result.desccuentacargo1 = viewmodel.Desccuentacargo1;
            result.desccuentacargo2 = viewmodel.Desccuentacargo2;
            result.desccuentacargorel = viewmodel.Desccuentacargorel;
            result.desccuentaabono1 = viewmodel.Desccuentaabono1;
            result.desccuentaabono2 = viewmodel.Desccuentaabono2;
            result.desccuentaabonorel = viewmodel.Desccuentaabonorel;
            result.tipocircuito = (int)viewmodel.Tipocircuito;
            result.codigodescripcionasiento = viewmodel.Codigodescripcionasiento;
            result.actualizarcobrador = viewmodel.Actualizarcobrador;
            return result;
        }

        public override IModelView GetModelView(Persistencia.CircuitosTesoreriaCobros viewmodel)
        {
            var result = new CircuitoTesoreriaCobrosModel
            {
                Id = viewmodel.id,
                Empresa = viewmodel.empresa,
                Descripcion = viewmodel.descripcion,
                Situacioninicial = viewmodel.situacioninicial,
                Situacionfinal = viewmodel.situacionfinal,
                Datos = viewmodel.datos,
                Fkmodopagopreferido = viewmodel.fkmodopagopreferido,
                Documentocartera = viewmodel.documentocartera.Value,
                Asientocontable = viewmodel.asientocontable.Value,
                Fecharemesa = viewmodel.fecharemesa.Value,
                Fechapago = viewmodel.fechapago.Value,
                Liquidariva = viewmodel.liquidariva.Value,
                Conciliacion = viewmodel.conciliacion.Value,
                Datosdocumento = viewmodel.datosdocumento.Value,
                Cuentacargo1 = viewmodel.cuentacargo1,
                Cuentacargo2 = viewmodel.cuentacargo2,
                Cuentacargorel = viewmodel.cuentacargorel,
                Cuentaabono1 = viewmodel.cuentaabono1,
                Cuentaabono2 = viewmodel.cuentaabono2,
                Cuentaabonorel = viewmodel.cuentaabonorel,
                Importecuentacargo1 = (TipoImporte)viewmodel.importecuentacargo1,
                Importecuentacargo2 = (TipoImporte)viewmodel.importecuentacargo2,
                Importecuentacargorel = (TipoImporte)viewmodel.importecuentacargorel,
                Importecuentaabono1 = (TipoImporte)viewmodel.importecuentaabono1,
                Importecuentaabono2 = (TipoImporte)viewmodel.importecuentaabono2,
                Importecuentaabonorel = (TipoImporte)viewmodel.importecuentaabonorel,
                Desccuentacargo1 = viewmodel.desccuentacargo1,
                Desccuentacargo2 = viewmodel.desccuentacargo2,
                Desccuentacargorel = viewmodel.desccuentacargorel,
                Desccuentaabono1 = viewmodel.desccuentaabono1,
                Desccuentaabono2 = viewmodel.desccuentaabono2,
                Desccuentaabonorel = viewmodel.desccuentaabonorel,
                Tipocircuito = (TipoCircuito)viewmodel.tipocircuito,
                Codigodescripcionasiento = viewmodel.codigodescripcionasiento,
                Actualizarcobrador = viewmodel.actualizarcobrador.Value
            };
           
            return result;
        }
    }
}
