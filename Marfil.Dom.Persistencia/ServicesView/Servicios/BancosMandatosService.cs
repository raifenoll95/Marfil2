using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IBancosMandatosService
    {

    }

    public class BancosMandatosService : GestionService<BancosMandatosLinModel, BancosMandatos>, IBancosMandatosService
    {
        #region CTR

        public BancosMandatosService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        #region API


        internal void CleanAllBancosMandatos(string fkcuentas)
        {
            var vector = _db.BancosMandatos.Where(f => f.empresa == Empresa && f.fkcuentas == fkcuentas);
            _db.BancosMandatos.RemoveRange(vector);
            _db.SaveChanges();
        }

        public List<BancosMandatosLinModel> GetBancosMandatos(string fkcuentas)
        {
            //return _db.Set<BancosMandatos>().Where(f => f.empresa == Empresa && f.fkcuentas == fkcuentas).ToList().Select(f => _converterModel.GetModelView(f) as BancosMandatosLinModel).ToList();
            //List<BancosMandatosLinModel> mandatos = new List<BancosMandatosLinModel>();
            //var mandatosDB = _db.BancosMandatos.Where(f => f.empresa == Empresa && f.fkcuentas == fkcuentas).ToList().ToList();

            //foreach(var mandato in mandatosDB)
            //{
            //    mandatos.Add(get(mandato.))
            //}

            var mandatos = _db.BancosMandatos.Where(f => f.empresa == Empresa && f.fkcuentas == fkcuentas).ToList().Select(f => new BancosMandatosLinModel()
            {
                Empresa = Empresa,
                Id = f.id,
                Bic = f.bic,
                Fkpaises = f.fkpaises,
                Descripcion = f.descripcion,
                Iban = f.iban,
                Defecto = f.defecto.Value,
                Sufijoacreedor = f.sufijoacreedor,
                Contratoconfirmig = f.contratoconfirmig,
                Contadorconfirming = f.contadorconfirming,
                Riesgonacional = f.riesgonacional,
                Riesgoextranjero = f.riesgoextranjero,
                Direccion = f.direccion,
                Fkprovincias = f.fkprovincias,
                Ciudad = f.ciudad,
                Cpostal = f.cpostal,
                Telefonobanco = f.telefonobanco,
                Personacontacto = f.personacontacto,
                Idmandato = f.idmandato,
                Tipoadeudo = (TipoAdeudo?)f.tipoadeudo,
                Esquema = (Esquema?)f.esquema
            }).ToList();

            return mandatos;
        }

        #endregion
    }
}
