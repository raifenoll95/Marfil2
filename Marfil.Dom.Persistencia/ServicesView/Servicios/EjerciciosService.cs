﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Preferencias;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Validation;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IEjerciciosService
    {

    }

    public class EjerciciosService : GestionService<EjerciciosModel, Ejercicios>, IEjerciciosService
    {
        #region CTR

        public EjerciciosService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var propiedadesVisibles = new[] {  "Descripcion", "Descripcioncorta", "Desde", "Hasta", "Estado", "Contabilidadcerradahasta", "Registroivacerradohasta" };
            var propiedades = Helpers.Helper.getProperties<EjerciciosModel>();
            model.PrimaryColumnns = new[] { "Id" };       
            model.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();



            return model;
        }

        public string GetEjercicioDefecto(Guid tusuario, MarfilEntities db, string tempresa)
        {
            using (var preferencias = new PreferenciasUsuarioService(db))
            {
                var preferenciaItem = preferencias.GePreferencia(TiposPreferencias.EjercicioDefecto,
                                                                    tusuario, 
                                                                    PreferenciaEjercicioDefecto.Id, 
                                                                    PreferenciaEjercicioDefecto.Nombre) 
                                        as PreferenciaEjercicioDefecto;

                if (preferenciaItem != null)
                {
                    var ejercicios = preferenciaItem.ListEjercicios;
                    if (ejercicios.Any())
                    {
                        if (ejercicios.ContainsKey(tempresa))
                        {
                            return ejercicios[tempresa];
                        }
                    }
                }
            }

            return GetEjercicioActual(tempresa,db);
        }

        public string GetEjercicioActual(string empresa,MarfilEntities db=null)
        {
            return
                (db ?? _db).Ejercicios.FirstOrDefault(
                    f =>
                        f.empresa == empresa && f.estado == (int)EstadoEjercicio.Abierto && (DateTime.Now >= f.desde) &&
                        (DateTime.Now <= f.hasta))?.id.ToString() ?? GetEjercicioUltimoAbierto(empresa, db);
        }

        public string GetEjercicioUltimoAbierto(string empresa, MarfilEntities db = null)
        {
            return
                (db ?? _db).Ejercicios.OrderByDescending( f => f.hasta).FirstOrDefault(
                    f =>
                        f.empresa == empresa && f.estado == (int)EstadoEjercicio.Abierto)?.id.ToString() ?? string.Empty;
        }

        public IEnumerable<EjerciciosModel> GetEjercicios(int? id)
        {
            
                var result = getAll().Select(f => (EjerciciosModel) f);

                if (id.HasValue)
                {
                    result = result.Where(f => f.Id != id.Value);
                }

                return result;
            
        }

        public IEnumerable<EjerciciosModel> getEjercicios(string empresa)
        {

            var result = new List<EjerciciosModel>();
            var list = _db.Ejercicios.Where(f => f.empresa == empresa);

            foreach (var ejercicio in list)
            {
                var ejercicioModel = _converterModel.GetModelView(ejercicio) as EjerciciosModel;
                result.Add(ejercicioModel);
            }
            return result;
        }

        public override string GetSelectPrincipal() {

            return string.Format("select * from Ejercicios where empresa='{0}'", Empresa);
        }

        public string DescSerieContable(string id)
        {
            return _db.SeriesContables.Where(f => f.empresa == Empresa && f.id == id).FirstOrDefault().descripcion;
        }

        public IEnumerable<string> GetEjercicio(string id)
        {
            var idparse = int.Parse(id);
            return (IEnumerable<string>)_db.Ejercicios.Where(f => f.empresa == Empresa && f.id == idparse).Select(f => f.id).ToList();
        }

        public List<SelectListItem> GetEstadosEjerciciosRevertir(string id)
        {
            var idparse = int.Parse(id);
            var estado = _db.Ejercicios.Where(f => f.empresa == Empresa && f.id == idparse).Select(f => f.estado).FirstOrDefault();
            List<SelectListItem> listaestados = new List<SelectListItem>()
            {
                new SelectListItem() {Text="Abierto", Value="0"},
                new SelectListItem() { Text="Regularización de existencias", Value="1"},
                new SelectListItem() { Text="Regularización de grupos 6 y 7", Value="2"}
            };

            if (estado == 1)
            {                
                listaestados.RemoveAt(2);
                listaestados.RemoveAt(1);
            }
            else if (estado == 2)
            {
                listaestados.RemoveAt(2);
            }

            return listaestados;
        }

        public string GetGetEstadoEjercicioAct(string id)
        {
            var serviceEjercicios = new EjerciciosService(_context);
            var idejerc = int.Parse(_context.Ejercicio);
            var ejercicio = serviceEjercicios.getAll().Where(f => ((Model.Configuracion.EjerciciosModel)f).Id == idejerc).Select(f => f as Model.Configuracion.EjerciciosModel).FirstOrDefault();

            return ejercicio.Estado.ToString();
        }

        public string GetCuentaDesdeProvisional()
        {
            var cuentasbalances = _db.Empresas.Where(f => f.id == Empresa).FirstOrDefault().cuentasanuales;
            var primeraCuenta = cuentasbalances.Split(';')[0];

            return _db.Cuentas.Where(f => f.empresa == Empresa && f.nivel == 0 && f.id.StartsWith(primeraCuenta)).FirstOrDefault().id;
        }

        public string GetCuentaHastaProvisional()
        {
            var cuentasbalances = _db.Empresas.Where(f => f.id == Empresa).FirstOrDefault().cuentasanuales;
            var numCuentas = cuentasbalances.Split(';').Length;
            var ultimaCuenta = cuentasbalances.Split(';')[numCuentas-1];

            return _db.Cuentas.Where(f => f.empresa == Empresa && f.nivel == 0 && f.id.StartsWith(ultimaCuenta)).OrderByDescending(s => s.id).FirstOrDefault().id;
        }

        public string GetSerieRepercutido()
        {
            var idparse = int.Parse(_context.Ejercicio);
            var serie = _db.Ejercicios.Where(f => f.empresa == Empresa && f.id == idparse).FirstOrDefault().fkseriescontablesIVP;

            return serie;
        }
    }
}
