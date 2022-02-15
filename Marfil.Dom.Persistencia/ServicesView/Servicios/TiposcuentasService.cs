using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface ITiposcuentasService
    {

    }


    public class TiposcuentasService : GestionService<TiposCuentasModel, Tiposcuentas>, ITiposcuentasService
    {
        #region CTR

        public TiposcuentasService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
           
            var propiedadesVisibles = new[] { "Categoria", "Tipos", "Cuenta", "Descripcion","Nifobligatorio" };
            var propiedades = Helpers.Helper.getProperties<TiposCuentasModel>();
            model.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            model.PrimaryColumnns = new[] {"TipoInt"};
            return model;
        }

        

        #region Api

        public IEnumerable<TiposCuentas> GetTiposCuentasFromCuentaCliente(string cuenta)
        {
            var cuentaService=FService.Instance.GetService(typeof(CuentasModel),_context,_db) as CuentasService;
            var cuentaId = cuentaService.GetSuperCuenta(cuenta);
            return _db.Tiposcuentas.Where(f => f.empresa == Empresa && f.cuenta == cuentaId).ToList().Select(f => (TiposCuentas) f.tipos);
        }

        internal string GetMascara(TiposCuentas tipo)
        {
            return _db.Tiposcuentas.SingleOrDefault(f => f.tipos == (int)tipo && f.empresa == Empresa)?.cuenta;
        }

        public string GetMascaraFromTipoCuenta(TiposCuentas tipo)
        {
            using (var service = FService.Instance.GetService(typeof(TiposCuentasModel), _context) as TiposcuentasService)
            {
                return service.GetMascara(tipo);
            }
        }

        public IEnumerable<int> GetTipoTercero(string id)
        {
            return _db.Database.SqlQuery<int>(string.Format(
            "SELECT c.tipocuenta"
            + " FROM("
            + " SELECT empresa, fkcuentas"
            + " FROM Clientes"
            + " UNION"
            + " SELECT empresa, fkcuentas"
            + " FROM Proveedores"
            + " UNION"
            + " SELECT empresa, fkcuentas"
            + " FROM Acreedores"
            + " UNION"
            + " SELECT empresa, fkcuentas"
            + " FROM Cuentastesoreria"
            + " UNION"
            + " SELECT empresa, fkcuentas"
            + " FROM Transportistas"
            + " UNION"
            + " SELECT empresa, fkcuentas"
            + " FROM Agentes"
            + " UNION"
            + " SELECT empresa, fkcuentas"
            + " FROM Aseguradoras"
            + " UNION"
            + " SELECT empresa, fkcuentas"
            + " FROM Operarios"
            + " UNION"
            + " SELECT empresa, fkcuentas"
            + " FROM Comerciales"
            + " UNION"
            + " SELECT empresa, fkcuentas"
            + " FROM Prospectos"
            + " )t"
            + " LEFT JOIN Cuentas AS c ON c.empresa = t.empresa AND c.id = t.fkcuentas"
            + " WHERE t.empresa='" + Empresa + "' AND t.fkcuentas='" + id + "'"));       
        }

        public bool ExisteTipoCuenta(string digitoscuenta)
        {
            if(_db.Tiposcuentas.Where(f => f.empresa == Empresa && f.cuenta == digitoscuenta).FirstOrDefault() != null)
            {
                return true;
            }

            return false;
        }

        public int GetTipoCuenta(string digitoscuenta)
        {
            return _db.Tiposcuentas.Where(f => f.empresa == Empresa && f.cuenta == digitoscuenta).Select(f => f.tipos).FirstOrDefault();
        }

        #endregion
    }
}
