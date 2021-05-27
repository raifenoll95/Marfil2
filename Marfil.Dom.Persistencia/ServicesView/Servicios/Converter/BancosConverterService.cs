using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class BancosConverterService : BaseConverterModel<BancosModel, Bancos>
    {
        #region CTR

        public BancosConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
            
        }

        #endregion

        public override IEnumerable<IModelView> GetAll()
        {
            var list = _db.Set<Bancos>().ToList();

            var result = new List<BancosModel>();
            foreach (var item in list)
            {
                result.Add(GetModelView(item) as BancosModel);
            }

            return result;
        }

        //public override bool Exists(string id)
        //{
        //    var vector = id.Split('-');
        //    return _db.Set<BancosMandatos>().Any(f => f.id == vector[1] && f.empresa == Empresa && f.fkcuentas == vector[0]);
        //}

        public override IModelView CreateView(string id)
        {
            var obj = _db.Set<Bancos>().Single(f => f.id == id);

            var result = GetModelView(obj) as BancosModel;

            return result;
        }

        //public override BancosMandatos CreatePersitance(IModelView obj)
        //{
        //    var objext = obj as IModelViewExtension;
        //    var st = obj as BancosMandatosLinModel;
        //    var result = _db.Set<BancosMandatos>().Create();
        //    result.empresa = st.Empresa;
        //    result.fkcuentas = st.Fkcuentas;
        //    result.id = st.Id;
        //    result.descripcion = st.Descripcion;
        //    result.fkpaises = st.Fkpaises;
        //    result.iban = st.Iban;
        //    result.bic = st.Bic;
        //    result.sufijoacreedor = st.Sufijoacreedor;
        //    result.contratoconfirmig = st.Contratoconfirmig;
        //    result.contadorconfirming = st.Contadorconfirming;
        //    result.direccion = st.Direccion;
        //    result.cpostal = st.Cpostal;
        //    result.ciudad = st.Ciudad;
        //    result.fkprovincias = st.Fkprovincias;
        //    result.telefonobanco = st.Telefonobanco;
        //    result.personacontacto = st.Personacontacto;
        //    result.riesgoextranjero = st.Riesgoextranjero;
        //    result.riesgonacional = st.Riesgonacional;
        //    result.idmandato = st.Idmandato;
        //    result.idacreedor = st.Idacreedor;
        //    result.tiposecuenciasepa = (int?)st.Tiposecuenciasepa;
        //    result.tipoadeudo = (int?)st.Tipoadeudo;
        //    result.importemandato = st.Importemandato;
        //    result.recibosmandato = st.Recibosmandato;
        //    result.importelimiterecibo = st.Importelimiterecibo;
        //    result.fechafirma = st.Fechafirma;
        //    result.fechaexpiracion = st.Fechaexpiracion;
        //    result.fechaultimaremesa = st.Fechaultimaremesa;
        //    result.importeremesados = st.Importeremesados;
        //    result.recibosremesados = st.Recibosremesados;
        //    result.devolvera = st.Devolvera;
        //    result.notas = st.Notas;
        //    result.defecto = st.Defecto;
        //    result.finalizado = st.Finalizado;
        //    result.bloqueada = st.Bloqueado;
        //    result.esquema = (int?)st.Esquema;
            


        //    return result;

        //}

        //public override BancosMandatos EditPersitance(IModelView obj)
        //{
        //    var objext = obj as IModelViewExtension;
        //    var st = obj as BancosMandatosLinModel;
        //    var result = _db.Set<BancosMandatos>().Single(f => f.id == st.Id && f.empresa == st.Empresa && f.fkcuentas == st.Fkcuentas);

        //    result.empresa = st.Empresa;
        //    result.fkcuentas = st.Fkcuentas;
        //    result.id = st.Id;
        //    result.descripcion = st.Descripcion;
        //    result.fkpaises = st.Fkpaises;
        //    result.iban = st.Iban;
        //    result.bic = st.Bic;
        //    result.sufijoacreedor = st.Sufijoacreedor;
        //    result.contratoconfirmig = st.Contratoconfirmig;
        //    result.contadorconfirming = st.Contadorconfirming;
        //    result.direccion = st.Direccion;
        //    result.cpostal = st.Cpostal;
        //    result.ciudad = st.Ciudad;
        //    result.fkprovincias = st.Fkprovincias;
        //    result.telefonobanco = st.Telefonobanco;
        //    result.personacontacto = st.Personacontacto;
        //    result.riesgoextranjero = st.Riesgoextranjero;
        //    result.riesgonacional = st.Riesgonacional;
        //    result.idmandato = st.Idmandato;
        //    result.idacreedor = st.Idacreedor;
        //    result.tiposecuenciasepa = (int?)st.Tiposecuenciasepa;
        //    result.tipoadeudo = (int?)st.Tipoadeudo;
        //    result.importemandato = st.Importemandato;
        //    result.recibosmandato = st.Recibosmandato;
        //    result.importelimiterecibo = st.Importelimiterecibo;
        //    result.fechafirma = st.Fechafirma;
        //    result.fechaexpiracion = st.Fechaexpiracion;
        //    result.fechaultimaremesa = st.Fechaultimaremesa;
        //    result.importeremesados = st.Importeremesados;
        //    result.recibosremesados = st.Recibosremesados;
        //    result.devolvera = st.Devolvera;
        //    result.notas = st.Notas;
        //    result.defecto = st.Defecto;
        //    result.finalizado = st.Finalizado;
        //    result.esquema = (int?)st.Esquema;
        //    result.bloqueada = st.Bloqueado;

        //    return result;
        //}

        public override IModelView GetModelView(Bancos obj)
        {
            return new BancosModel()
            {
                Id = obj.id,
                Nombre = obj.nombre,
                Bic = obj.bic
            };
        }
    }
}
