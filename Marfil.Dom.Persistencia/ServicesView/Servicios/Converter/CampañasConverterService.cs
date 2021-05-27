using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Inf.Genericos;
using System.Data.Entity;
using System.Collections;
using Marfil.Dom.Persistencia.Model.CRM;
using Marfil.Inf.Genericos.Helper;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Helpers;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class CampañasConverterService : BaseConverterModel<CampañasModel, Campañas>
    {

        #region CTR

        public CampañasConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {

        }

        #endregion

        public override IEnumerable<IModelView> GetAll()
        {
            var result = _db.Set<Campañas>().Where(f => f.empresa == Empresa).ToList().Select(f => GetModelView(f) as CampañasModel).ToList();
            return result;
        }

        //Exist
        public override bool Exists(string id)
        {
            var intid = Funciones.Qint(id);
            return _db.Campañas.Any(f => f.empresa == Context.Empresa && f.id == intid);
        }

        //Create view
        public override IModelView CreateView(string id)
        {
            var identificador = Funciones.Qint(id);
            var obj = _db.Set<Campañas>().Include(f => f.CampañasTercero).Where(f => f.empresa == Empresa && f.id == identificador).Single();
            var result = GetModelView(obj) as CampañasModel;
            return result;
        }

        //get model view
        public override IModelView GetModelView(Campañas obj)
        {
            var result = new CampañasModel(Context)
            {
                Empresa = obj.empresa,
                Id = obj.id,
                Fkseries = obj.fkseries,
                Identificadorsegmento = obj.identificadorsegmento,
                Referencia = obj.referencia,
                Fechadocumento = obj.fechadocumento,
                Asunto = obj.asunto,
                Fechaultimoseguimiento = obj.fechaultimoseguimiento,
                Fechaproximoseguimiento = obj.fechaproximoseguimiento,
                Prioridad = (TipoPrioridad)obj.prioridad,
                Fkmodocontacto = obj.fkmodocontacto,
                Fkoperario = obj.fkoperario,
                Fkcomercial = obj.fkcomercial,
                Fkagente = obj.fkagente,
                Notas = obj.notas,
                Fechacierre = obj.fechacierre,
                Coste = obj.coste.Value,
                Cerrado = obj.cerrado.Value,
                Fkreaccion = obj.fkreaccion,
                Fketapa = obj.fketapa,
                Campañas = obj.CampañasTercero.Select(f => 
                    new CampañasTerceroModel() {
                        Empresa = f.empresa, Fkcampañas = f.fkcampañas, Id = f.id, Codtercero = f.codtercero,
                        Descripciontercero = f.descripciontercero, Poblacion = f.poblacion, Fkprovincia = f.fkprovincia,
                        Fkpais = f.fkpais, Email = f.email, Telefono = f.telefono}).ToList()
            };

            var seguimientos = _db.Seguimientos.Where(f => f.empresa == result.Empresa && f.origen == result.Referencia);
            result.Seguimientos = seguimientos.Select(f => new SeguimientosModel()
            {
                Id = f.id,
                Fechadocumento = f.fechadocumento,
                Usuario = f.usuario,
                Fketapa = f.fketapa,
                Fkaccion = f.fkaccion,
                Notas = f.notas,
                Fkdocumentorelacionado = f.fkdocumentorelacionado,
                Fkreferenciadocumentorelacionado = f.fkreferenciadocumentorelacionado
            }).ToList();

            var appService = new ApplicationHelper(Context);
            var serviceSeguimientos = FService.Instance.GetService(typeof(SeguimientosModel), Context) as SeguimientosService;

            foreach (var item in result.Seguimientos)
            {
                item.Fketapa = _db.Estados.Where(f => (f.documento + "-" + f.id) == item.Fketapa).Select(f => f.descripcion).SingleOrDefault();
                item.Fkaccion = appService.GetListAcciones().Where(f => f.Valor == item.Fkaccion).Select(f => f.Descripcion).SingleOrDefault();
                if (!string.IsNullOrEmpty(item.Fkreferenciadocumentorelacionado))
                    item.Idrelacionado = serviceSeguimientos.BuscarIdDocumentoRelaciondo(item.Fkreferenciadocumentorelacionado).SingleOrDefault();
            }

            return result;
        }

        //Create p
        public override Campañas CreatePersitance(IModelView obj)
        {
            var objmodel = obj as CampañasModel;
            var result = _db.Set<Campañas>().Create();

            foreach (var item in result.GetType().GetProperties())
            {
                if ((obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false) &&
                    (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.GetGenericTypeDefinition() !=
                    typeof(ICollection<>)))
                {
                    item.SetValue(result, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
                else if (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsEnum ?? false)
                {
                    item.SetValue(result, (int)obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
                else if (!obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false)
                {
                    item.SetValue(result, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
            }

            result.empresa = objmodel.Empresa;
            result.id = objmodel.Id;
            result.fkseries = objmodel.Fkseries;
            result.identificadorsegmento = objmodel.Identificadorsegmento;
            result.referencia = objmodel.Referencia;
            result.fechadocumento = objmodel.Fechadocumento;
            result.asunto = objmodel.Asunto;
            result.fechaultimoseguimiento = objmodel.Fechaultimoseguimiento;
            result.fechaproximoseguimiento = objmodel.Fechaproximoseguimiento;
            result.prioridad = (int)objmodel.Prioridad;
            result.fkmodocontacto = objmodel.Fkmodocontacto;
            result.fkoperario = objmodel.Fkoperario;
            result.fkcomercial = objmodel.Fkcomercial;
            result.fkagente = objmodel.Fkagente;
            result.notas = objmodel.Notas;
            result.fechacierre = objmodel.Fechacierre;
            result.coste = objmodel.Coste;
            result.cerrado = objmodel.Cerrado;
            result.fkreaccion = objmodel.Fkreaccion;
            result.fketapa = objmodel.Fketapa;


            // Añadir las líneas
            result.CampañasTercero.Clear();
            foreach (var item in objmodel.Campañas)
            {
                var newItem = _db.Set<CampañasTercero>().Create();
                newItem.empresa = item.Empresa;
                newItem.fkcampañas = item.Fkcampañas;
                newItem.id = item.Id;
                newItem.codtercero = item.Codtercero;
                newItem.descripciontercero = item.Descripciontercero;
                newItem.poblacion = item.Poblacion;
                newItem.fkprovincia = item.Fkprovincia;
                newItem.fkpais = item.Fkpais;
                newItem.email = item.Email;
                newItem.telefono = item.Telefono;

                result.CampañasTercero.Add(newItem);
            }

            return result;
        }

        public override Campañas EditPersitance(IModelView obj)
        {
            var objmodel = obj as CampañasModel;
            var result = _db.Set<Campañas>().Single(f => f.empresa == Context.Empresa && f.id == objmodel.Id);

            foreach (var item in result.GetType().GetProperties())
            {
                if ((obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false) &&
                    (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.GetGenericTypeDefinition() !=
                    typeof(ICollection<>)))
                {
                    item.SetValue(result, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
                else if (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsEnum ?? false)
                {
                    item.SetValue(result, (int)obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
                else if (!obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false)
                {
                    item.SetValue(result, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
            }

            result.empresa = objmodel.Empresa;
            result.id = objmodel.Id;
            result.fkseries = objmodel.Fkseries;
            result.identificadorsegmento = objmodel.Identificadorsegmento;
            result.referencia = objmodel.Referencia;
            result.fechadocumento = objmodel.Fechadocumento;
            result.asunto = objmodel.Asunto;
            result.fechaultimoseguimiento = objmodel.Fechaultimoseguimiento;
            result.fechaproximoseguimiento = objmodel.Fechaproximoseguimiento;
            result.prioridad = (int)objmodel.Prioridad;
            result.fkmodocontacto = objmodel.Fkmodocontacto;
            result.fkoperario = objmodel.Fkoperario;
            result.fkcomercial = objmodel.Fkcomercial;
            result.fkagente = objmodel.Fkagente;
            result.notas = objmodel.Notas;
            result.fechacierre = objmodel.Fechacierre;
            result.coste = objmodel.Coste;
            result.cerrado = objmodel.Cerrado;
            result.fkreaccion = objmodel.Fkreaccion;
            result.fketapa = objmodel.Fketapa;


            // Añadir las líneas
            result.CampañasTercero.Clear();
            foreach (var item in objmodel.Campañas)
            {
                var newItem = _db.Set<CampañasTercero>().Create();
                newItem.empresa = item.Empresa;
                newItem.fkcampañas = item.Fkcampañas;
                newItem.id = item.Id;
                newItem.codtercero = item.Codtercero;
                newItem.descripciontercero = item.Descripciontercero;
                newItem.poblacion = item.Poblacion;
                newItem.fkprovincia = item.Fkprovincia;
                newItem.fkpais = item.Fkpais;
                newItem.email = item.Email;
                newItem.telefono = item.Telefono;

                result.CampañasTercero.Add(newItem);
            }

            return result;
        }
    }
}
