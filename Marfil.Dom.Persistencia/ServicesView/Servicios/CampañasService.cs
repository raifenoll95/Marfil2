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
using Resources;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{

    public interface ICampañasService
    {

    }


    public class CampañasService : GestionService<CampañasModel, Campañas>, ICampañasService
    {

        #region CTR

        public CampañasService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion    

        #region List Index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var st = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var estadosService = new EstadosService(_context, _db);
            st.List = st.List.OfType<CampañasModel>().OrderByDescending(f => f.Fechadocumento).ThenByDescending(f => f.Referencia);
            var propiedadesVisibles = new[] { "Referencia", "Asunto", "Fketapa", "Cerrado" };
            var propiedades = Helpers.Helper.getProperties<CampañasModel>();
            st.PrimaryColumnns = new[] { "Id" };
            st.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            st.ColumnasCombo.Add("Fketapa", estadosService.GetStates(DocumentoEstado.Campañas, TipoMovimientos.Todos).Select(f => new Tuple<string, string>(f.CampoId, f.Descripcion)));
            return st;            
        }

        //public override string GetSelectPrincipal()
        //{
        //    var result = new StringBuilder();

        //    result.Append("SELECT o.*, c.descripcion AS [Descripciontercero]");
        //    result.Append(" FROM Oportunidades as o");
        //    result.Append(" LEFT JOIN Cuentas AS c ON c.empresa = o.empresa AND c.id = o.fkempresa");
        //    result.AppendFormat(" where o.empresa ='{0}' ", _context.Empresa);
        //    return result.ToString();
        //}

        #endregion

        public int NextId()
        {
            return _db.Campañas.Any() ? _db.Campañas.Max(f => f.id) + 1 : 1;
        }


        public override void create(IModelView obj)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as CampañasModel;
                var validation = _validationService as CampañasValidation;
                
                model.Id = NextId();
                var appService = new ApplicationHelper(_context);                                
                if (model.Fechadocumento == null)
                    model.Fechadocumento = DateTime.Now;
                var contador = ServiceHelper.GetNextId<Campañas>(_db, Empresa, model.Fkseries);
                var identificadorsegmento = "";
                model.Referencia = ServiceHelper.GetReference<Campañas>(_db, model.Empresa, model.Fkseries, contador, model.Fechadocumento.Value, out identificadorsegmento);
                model.Identificadorsegmento = identificadorsegmento;

                foreach (var linea in model.Campañas)
                {
                    linea.Empresa = model.Empresa;
                    linea.Context = model.Context;
                    linea.Fkcampañas = model.Id;
                }

                base.create(obj);

                _db.SaveChanges();
                tran.Complete();
            }
        }

        public override void edit(IModelView obj)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as CampañasModel;
                var currentValidationService = _validationService as CampañasValidation;

                var etapaAnterior = _db.Campañas.Where(f => f.empresa == model.Empresa && f.id == model.Id).Select(f => f.fketapa).SingleOrDefault();
                var s = etapaAnterior.Split('-');
                var documento = Funciones.Qint(s[0]);
                var id = s[1];
                var estadoAnterior = _db.Estados.Where(f => f.documento == documento && f.id == id).Select(f => f.tipoestado).SingleOrDefault();

                if (model.Cerrado && (estadoAnterior != (int)TipoEstado.Finalizado && estadoAnterior != (int)TipoEstado.Caducado && estadoAnterior != (int)TipoEstado.Anulado))
                {
                    var estadoFinalizado = _db.Estados.Where(f => f.documento == (int)DocumentoEstado.Campañas && f.tipoestado == (int)TipoEstado.Finalizado).SingleOrDefault()
                        ?? _db.Estados.Where(f => f.documento == (int)DocumentoEstado.Todos && f.tipoestado == (int)TipoEstado.Finalizado).SingleOrDefault();

                    model.Fketapa = estadoFinalizado.documento + "-" + estadoFinalizado.id;
                    currentValidationService.CambiarEstado = true;
                }

                foreach(var linea in model.Campañas)
                {
                    linea.Empresa = model.Empresa;
                    linea.Context = model.Context;
                    linea.Fkcampañas = model.Id;
                }            

                base.edit(model);

                _db.SaveChanges();
                tran.Complete();
            }
        }

        public override void delete(IModelView obj)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as CampañasModel;

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

    }

}
