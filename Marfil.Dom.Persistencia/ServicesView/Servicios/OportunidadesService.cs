using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Inf.ResourcesGlobalization.Textos.Entidades;
using Marfil.Dom.Persistencia.Model.CRM;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Validation;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Inf.Genericos.Helper;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using Resources;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{

    public interface IOportunidadesService
    {

    }


    public class OportunidadesService : GestionService<OportunidadesModel, Oportunidades>, IOportunidadesService
    {
        #region CTR

        public OportunidadesService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion


        #region List Index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var st = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var estadosService = new EstadosService(_context, _db);
            st.List = st.List.OfType<OportunidadesModel>().OrderByDescending(f => f.Fechadocumento).ThenByDescending(f => f.Referencia);
            var propiedadesVisibles = new[] { "Referencia", "Fechadocumento", "Fechaultimoseguimiento", "Fechaproximoseguimiento", "Descripciontercero", "Asunto", "Fketapa", "Cerrado" };
            var propiedades = Helpers.Helper.getProperties<OportunidadesModel>();
            st.PrimaryColumnns = new[] { "Id" };
            st.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            st.ColumnasCombo.Add("Fketapa", estadosService.GetStates(DocumentoEstado.Oportunidades, TipoMovimientos.Todos).Select(f => new Tuple<string, string>(f.CampoId, f.Descripcion)));
            return st;
        }

        public override string GetSelectPrincipal()
        {
            var result = new StringBuilder();

            result.Append("SELECT o.*, c.descripcion AS [Descripciontercero]");
            result.Append(" FROM Oportunidades as o");
            result.Append(" LEFT JOIN Cuentas AS c ON c.empresa = o.empresa AND c.id = o.fkempresa");
            result.AppendFormat(" where o.empresa ='{0}' ", _context.Empresa);
            return result.ToString();
        }

        #endregion

        public int NextId()
        {
            return _db.Oportunidades.Any() ? _db.Oportunidades.Max(f => f.id) + 1 : 1;
        }


        public override void create(IModelView obj)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as OportunidadesModel;
                var validation = _validationService as OportunidadesValidation;

                model.Id = NextId();
                var appService = new ApplicationHelper(_context);
                if (model.Fechadocumento == null)
                    model.Fechadocumento = DateTime.Now;
                var tipodocumento = "OPO"; //Oportunidades 
                var contador = ServiceHelper.GetNextId<Oportunidades>(_db, Empresa, model.Fkseries, tipodocumento);
                var identificadorsegmento = "";
                model.Referencia = ServiceHelper.GetReference<Oportunidades>(_db, model.Empresa, model.Fkseries, tipodocumento, contador, model.Fechadocumento.Value, out identificadorsegmento);
                model.Identificadorsegmento = identificadorsegmento;

                base.create(obj);

                _db.SaveChanges();
                tran.Complete();
            }
        }

        public override void edit(IModelView obj)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as OportunidadesModel;
                var currentValidationService = _validationService as OportunidadesValidation;

                var etapaAnterior = _db.Oportunidades.Where(f => f.empresa == model.Empresa && f.id == model.Id).Select(f => f.fketapa).SingleOrDefault();
                var s = etapaAnterior.Split('-');
                var documento = Funciones.Qint(s[0]);
                var id = s[1];
                var estadoAnterior = _db.Estados.Where(f => f.documento == documento && f.id == id).Select(f => f.tipoestado).SingleOrDefault();

                if (model.Cerrado && (estadoAnterior != (int)TipoEstado.Finalizado && estadoAnterior != (int)TipoEstado.Caducado && estadoAnterior != (int)TipoEstado.Anulado))
                {
                    var estadoFinalizado = _db.Estados.Where(f => f.documento == (int)DocumentoEstado.Oportunidades && f.tipoestado == (int)TipoEstado.Finalizado).SingleOrDefault()
                        ?? _db.Estados.Where(f => f.documento == (int)DocumentoEstado.Todos && f.tipoestado == (int)TipoEstado.Finalizado).SingleOrDefault();

                    model.Fketapa = estadoFinalizado.documento + "-" + estadoFinalizado.id;
                    currentValidationService.CambiarEstado = true;
                }

                base.edit(obj);

                _db.SaveChanges();
                tran.Complete();
            }
        }

        public override void delete(IModelView obj)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as OportunidadesModel;

                var s = model.Fketapa.Split('-');
                var documento = Funciones.Qint(s[0]);
                var id = s[1];
                var tipoEstado = _db.Estados.Where(f => f.documento == documento && f.id == id).Select(f => f.tipoestado).SingleOrDefault();

                if (tipoEstado != (int)TipoEstado.Finalizado && tipoEstado != (int)TipoEstado.Caducado && tipoEstado != (int)TipoEstado.Anulado)
                {
                    base.delete(obj);
                    _db.Seguimientos.RemoveRange(_db.Seguimientos.Where(f => f.empresa == Empresa && f.origen == model.Referencia));
                    _db.SaveChanges();
                    tran.Complete();
                }
                else
                {
                    throw new ValidationException(General.ErrorDocumentoFinalizado);
                }
            }
        }

        public List<OportunidadesModel> getOportunidaesCliente(string cliente)
        {
            var oportunidadesService = new OportunidadesService(_context, _db);
            List<OportunidadesModel> oportunidades = new List<OportunidadesModel>();

            var list = _db.Oportunidades.Where(f => f.empresa == Empresa && f.fkempresa == cliente).ToList();

            foreach (var f in list)
            {
                if ((Int32.Parse(f.fketapa.Split('-')[0]) == (int)DocumentoEstado.Oportunidades || Int32.Parse(f.fketapa.Split('-')[0]) == (int)DocumentoEstado.Todos) && f.cerrado == false)
                {
                    OportunidadesModel oportunidad = new OportunidadesModel();
                    oportunidad.Empresa = f.empresa;
                    oportunidad.Id = f.id;
                    oportunidad.Fkseries = f.fkseries;
                    oportunidad.Identificadorsegmento = f.identificadorsegmento;
                    oportunidad.Referencia = f.referencia;
                    oportunidad.Fechadocumento = f.fechadocumento;
                    oportunidad.Asunto = f.asunto;
                    if(f.fechaproximoseguimiento != null)
                    {
                        oportunidad.fechaproximo = f.fechaproximoseguimiento.ToString().Split(' ')[0];
                    }

                    if(f.fechaultimoseguimiento != null)
                    {
                        oportunidad.fechaultimo = f.fechaultimoseguimiento.ToString().Split(' ')[0];
                    }
                    if (f.fechadocumento != null)
                    {
                        oportunidad.FechaAperturaStr = f.fechadocumento.ToString().Split(' ')[0];
                    }
                    oportunidad.Fkempresa = f.fkempresa;
                    oportunidad.Fkcontacto = f.fkcontacto;
                    oportunidad.Fkorigen = f.fkorigen;
                    var documento = Int32.Parse(f.fketapa.Split('-')[0]);
                    var id = f.fketapa.Split('-')[1];
                    oportunidad.Fketapa = _db.Estados.Where(a => a.documento == documento && a.id == id).Select(a => a.descripcion).FirstOrDefault();
                    oportunidad.Fkcomercial = f.fkcomercial;
                    oportunidad.Fkagente = f.fkagente;
                    oportunidad.Fkmargen = f.fkmargen;
                    oportunidad.Fkoperario = f.fkoperario;
                    oportunidad.Fechacierre = f.fechacierre;

                    oportunidades.Add(oportunidad);
                }
            }

            return oportunidades;
        }
    }
}
