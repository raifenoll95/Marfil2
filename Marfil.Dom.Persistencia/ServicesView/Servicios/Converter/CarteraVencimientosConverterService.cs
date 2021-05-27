using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using System.Collections.Generic;
using System.Linq;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Documentos.CobrosYPagos;
using System;
using Marfil.Inf.Genericos.Helper;
using System.Data.Entity;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class CarteraVencimientosConverterService : BaseConverterModel<CarteraVencimientosModel, Persistencia.CarteraVencimientos>
    {
        public string Ejercicio { get; set; }

        public CarteraVencimientosConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override IEnumerable<IModelView> GetAll()
        {
            return _db.Set<Persistencia.CarteraVencimientos>().Select(GetModelView);
        }

        public override IModelView CreateView(string id)
        {
            var identificador = Funciones.Qint(id);
            var obj = _db.Set<CarteraVencimientos>().Where(f => f.empresa == Empresa && f.id == identificador).Include(f => f.PrevisionesCartera).Single();
            var result = GetModelView(obj) as CarteraVencimientosModel;

            result.LineasCartera = obj.PrevisionesCartera.ToList().Select(f => new PrevisionesCarteraModel(Context)
            {
                Empresa = Empresa,
                Id = f.id,
                Codvencimiento = f.codvencimiento,
                Codcartera = f.codcartera,
                Imputado = f.imputado
            }).ToList();

            return result;
        }

        public override Persistencia.CarteraVencimientos CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as CarteraVencimientosModel;
            var result = _db.Set<Persistencia.CarteraVencimientos>().Create();
            result.id = viewmodel.Id.Value;
            result.empresa = viewmodel.Empresa;
            result.fkseriescontables = viewmodel.Fkseriescontables;
            result.referencia = viewmodel.Referencia;
            result.identificadorsegmento = viewmodel.Identificadorsegmento;
            result.traza = viewmodel.Traza;          
            result.tipovencimiento = (int)viewmodel.Tipovencimiento;
            result.fkformaspago = viewmodel.Fkformaspago;
            result.usuario = Context.Usuario;
            result.fkcuentas = viewmodel.Fkcuentas;
            result.fechacreacion = DateTime.Now;
            result.fechavencimiento = viewmodel.Fechavencimiento;
            result.fechadescuento = viewmodel.Fechadescuento;
            result.fecha = viewmodel.Fecha;
            result.fechapago = viewmodel.Fechapago;
            result.monedabase = viewmodel.Monedabase;
            result.monedagiro = viewmodel.Monedagiro;
            result.importegiro = viewmodel.Importegiro;
            result.cambioaplicado = viewmodel.Cambioaplicado;
            result.fkcuentastesoreria = viewmodel.Fkcuentastesoreria;
            result.mandato = viewmodel.Mandato;
            result.situacion = viewmodel.Situacion;
            result.comentario = viewmodel.Comentario;
            result.codigoremesa = viewmodel.Codigoremesa;
            result.tiponumerofactura = viewmodel.Tiponumerofactura;
            result.letra = viewmodel.Letra;
            result.banco = viewmodel.Banco;
            result.fkseriescontablesremesa = viewmodel.Fkseriescontablesremesa;
            result.fecharemesa = viewmodel.Fecharemesa;
            result.referenciaremesa = viewmodel.Referenciaremesa;
            result.identificadorsegmentoremesa = viewmodel.Identificadorsegmentoremesa;
            result.importeletra = viewmodel.Importeletra;

            foreach (var item in viewmodel.LineasCartera)
            {
                var newItem = _db.Set<PrevisionesCartera>().Create();
                newItem.empresa = Empresa;
                //newItem.id = item.Id.Value;
                newItem.codvencimiento = item.Codvencimiento.Value;
                newItem.codcartera = item.Codcartera.Value;
                newItem.imputado = item.Imputado;
                result.PrevisionesCartera.Add(newItem);
            }

            result.empresa = Empresa;
            return result;
        }

        public override Persistencia.CarteraVencimientos EditPersitance(IModelView obj)
        {
            var viewmodel = obj as CarteraVencimientosModel;
            var result = _db.CarteraVencimientos.Where(f => f.id == viewmodel.Id && f.empresa == Empresa).Single();
            result.empresa = viewmodel.Empresa;
            result.fkseriescontables = viewmodel.Fkseriescontables;
            result.referencia = viewmodel.Referencia;
            result.identificadorsegmento = viewmodel.Identificadorsegmento;
            result.traza = viewmodel.Traza;
            result.tipovencimiento = (int)viewmodel.Tipovencimiento;
            result.fkformaspago = viewmodel.Fkformaspago;
            result.usuario = viewmodel.Usuario;
            result.fkcuentas = viewmodel.Fkcuentas;
            result.fechacreacion = DateTime.Now;
            result.fechavencimiento = viewmodel.Fechavencimiento;
            result.fechadescuento = viewmodel.Fechadescuento;
            result.fechapago = viewmodel.Fechapago;
            result.fecha = viewmodel.Fecha;
            result.monedabase = viewmodel.Monedabase;
            result.monedagiro = viewmodel.Monedagiro;
            result.importegiro = viewmodel.Importegiro;
            result.cambioaplicado = viewmodel.Cambioaplicado;
            result.fkcuentastesoreria = viewmodel.Fkcuentastesoreria;
            result.mandato = viewmodel.Mandato;;
            result.situacion = viewmodel.Situacion;
            result.comentario = viewmodel.Comentario;
            result.codigoremesa = viewmodel.Codigoremesa;
            result.tiponumerofactura = viewmodel.Tiponumerofactura;
            result.letra = viewmodel.Letra;
            result.banco = viewmodel.Banco;
            result.fkseriescontablesremesa = viewmodel.Fkseriescontablesremesa;
            result.fecharemesa = viewmodel.Fecharemesa;
            result.referenciaremesa = viewmodel.Referenciaremesa;
            result.identificadorsegmentoremesa = viewmodel.Identificadorsegmentoremesa;
            result.importeletra = viewmodel.Importeletra;

            result.PrevisionesCartera.Clear();

            foreach (var item in viewmodel.LineasCartera)
            {
                var newItem = _db.Set<PrevisionesCartera>().Create();
                newItem.empresa = Empresa;
                newItem.id = item.Id.Value;
                newItem.codvencimiento = item.Codvencimiento.Value;
                newItem.codcartera = item.Codcartera.Value;
                newItem.imputado = item.Imputado;
                result.PrevisionesCartera.Add(newItem);
            }

            return result;
        }

        public override IModelView GetModelView(Persistencia.CarteraVencimientos viewmodel)
        {
            var result = new CarteraVencimientosModel
            {
                Empresa = viewmodel.empresa,
                Fkseriescontables = viewmodel.fkseriescontables,
                Referencia = viewmodel.referencia,
                Identificadorsegmento = viewmodel.identificadorsegmento,
                Id = viewmodel.id,
                Traza = viewmodel.traza,
                Tipovencimiento = (TipoVencimiento)viewmodel.tipovencimiento,
                Usuario = viewmodel.usuario,
                Fkformaspago = viewmodel.fkformaspago,
                Fkcuentas = viewmodel.fkcuentas,
                Fechacreacion = viewmodel.fechacreacion,
                Fechavencimiento = viewmodel.fechavencimiento,
                Fechadescuento = viewmodel.fechadescuento,
                Fecha = viewmodel.fecha,
                Fechapago = viewmodel.fechapago,
                Monedabase = viewmodel.monedabase.Value,
                Monedagiro = viewmodel.monedagiro.Value,
                Importegiro = viewmodel.importegiro,
                Cambioaplicado = viewmodel.cambioaplicado,
                Fkcuentastesoreria = viewmodel.fkcuentastesoreria,
                Mandato = viewmodel.mandato,
                Situacion = viewmodel.situacion,
                Comentario = viewmodel.comentario,
                Codigoremesa = viewmodel.codigoremesa,
                Tiponumerofactura = viewmodel.tiponumerofactura,
                Letra = viewmodel.letra,
                Banco = viewmodel.banco,
                Fkseriescontablesremesa = viewmodel.fkseriescontablesremesa,
                Fecharemesa = viewmodel.fecharemesa,
                Referenciaremesa = viewmodel.referenciaremesa,
                Identificadorsegmentoremesa = viewmodel.identificadorsegmentoremesa,
                Importeletra = viewmodel.importeletra
            };
           
            return result;
        }
    }
}
