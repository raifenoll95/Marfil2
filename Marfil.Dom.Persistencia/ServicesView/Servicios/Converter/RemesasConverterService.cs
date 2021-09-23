using Marfil.Dom.Persistencia.Model.Documentos.CobrosYPagos;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    class RemesasConverterService : BaseConverterModel<RemesasModel, Remesas>
    {
        public RemesasConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override IModelView CreateView(string id)
        {
            var identificador = Funciones.Qint(id);
            var obj = _db.Set<Remesas>().Where(f => f.empresa == Empresa && f.id == identificador).Single();
            var result = GetModelView(obj) as RemesasModel;


            return result;
        }

        public override Remesas CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as RemesasModel;
            var result = _db.Set<Persistencia.Remesas>().Create();
            result.id = viewmodel.Id;
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

            result.empresa = Empresa;
            return result;
        }
    }
}
