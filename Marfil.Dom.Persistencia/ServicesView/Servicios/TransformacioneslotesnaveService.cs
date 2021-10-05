using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Documentos.Transformacioneslotes;
using Marfil.Dom.Persistencia.Model.Documentos.Transformacioneslotesnave;
using Marfil.Inf.Genericos.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public class TransformacioneslotesnaveService : GestionService<TransformacioneslotesnaveModel, Transformacioneslotesnave>
    {
        #region CTR

        public TransformacioneslotesnaveService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        #region List index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var estadosService = new EstadosService(_context, _db);
            var st = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            st.List = st.List.OfType<TransformacioneslotesnaveModel>().OrderByDescending(f => f.Referencia);
            var propiedadesVisibles = new[] { "Referencia", "Fechadocumento", "Fkproveedores", "Nombreproveedor", "Trabajosdescripcion", "Fkestados" };
            var propiedades = Helper.getProperties<TransformacioneslotesnaveModel>();
            st.PrimaryColumnns = new[] { "Id" };
            st.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            st.ColumnasCombo.Add("Fkestados", estadosService.GetStates(DocumentoEstado.Transformacioneslotes, TipoMovimientos.Todos).Select(f => new Tuple<string, string>(f.CampoId, f.Descripcion)));
            return st;
        }

        public override string GetSelectPrincipal()
        {
            var operario = _db.Usuarios.Where(u => u.id == Usuarioid).FirstOrDefault().codigooperariousuario;
            return string.Format("select tl.*,t.descripcion as [Trabajosdescripcion] from Transformacioneslotesnave as tl inner join trabajos as t on t.empresa= tl.empresa and t.id=tl.fktrabajos " +
                "where tl.empresa='{0}' and tl.fkoperarios='{1}' and tl.fkestados = '99-002'", Empresa, operario);
        }

        #endregion

        //LotesNave
        public void TerminarBD(int lineaId, int transId, TransformacioneslotesnaveLinModel lineaLotesNave)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                //var transforacionId = lineaLotesNave.;
                var lineaLotes = _db.Transformacionesloteslin.Where(t => t.id == lineaId && t.empresa == Empresa && t.fkTransformacioneslotes == transId).FirstOrDefault();

                lineaLotes.largo = lineaLotesNave.Largo;
                lineaLotes.ancho = lineaLotesNave.Ancho;

                _db.SaveChanges();
                tran.Complete();
            }
        }
    }
}
