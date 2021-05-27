using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Converter;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IContactosService
    {

    }

    public class ContactosService : GestionService<ContactosLinModel, Contactos>, IContactosService
    {
        public const char Separator = ';';

        #region CTR

        public ContactosService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        #region Override

        public override bool exists(string Id)
        {
            var vector = Id.Split(Separator);
            var empresa = Empresa;
            var tipotercero = Funciones.Qint(vector[0]).Value;
            var fkentidad = vector[1];
            var id = Funciones.Qint(vector[2]).Value;

            return
                _db.Set<Contactos>()
                    .Any(
                        f =>
                            f.empresa == empresa && f.fkentidad == fkentidad && f.tipotercero == tipotercero && f.id == id);
        }

        #endregion

        #region Api

        public IEnumerable<ContactosLinModel> GetContactos( string fkCuentas)
        {
            return _db.Set<Contactos>().Where(f => f.empresa == Empresa  && f.fkentidad == fkCuentas).ToList().Select(f => _converterModel.GetModelView(f) as ContactosLinModel).ToList();
        }
        public IEnumerable<ContactosLinModel> GetContactos(TiposCuentas tipo, string fkCuentas)
        {
            var tipoInt = (int)tipo;
            return _db.Set<Contactos>().Where(f => f.empresa == Empresa && f.tipotercero == tipoInt && f.fkentidad == fkCuentas).ToList().Select(f => _converterModel.GetModelView(f) as ContactosLinModel).ToList();
        }

        public void CleanAllContactos(TiposCuentas tipo, string fkCuentas)
        {
            var tipoInt = (int)tipo;
            var list =
                _db.Set<Contactos>()
                    .Where(f => f.empresa == Empresa && f.tipotercero == tipoInt && f.fkentidad == fkCuentas)
                    .ToList();


            foreach (var item in list)
            {
                _db.Set<Contactos>().Remove(item);
            }

        }

        #endregion
    }
}
