﻿using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public class LogStockSeguridadService : GestionService<LogStockSeguridadModel, LogStockSeguridad>
    {

        #region CTR

        public LogStockSeguridadService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        #region ListIndexModel

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var propiedadesVisibles = new[] { "Fecharotura", "Fecha", "Documento", "Codarticulo", "Descripcionarticulo", "Almacen", "Stockseguridad", "Stockactual", "Stockminimo", "Pedidooptimo"};
            var propiedades = Helpers.Helper.getProperties<LogStockSeguridadModel>();

            //model.PrimaryColumnns = new[] { "id" };
            model.ExcludedColumns = propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            model.OrdenColumnas.Add("Fecharotura", 0);
            model.OrdenColumnas.Add("Fecha", 1);
            model.OrdenColumnas.Add("Documento", 2);
            model.OrdenColumnas.Add("Codarticulo", 3);
            model.OrdenColumnas.Add("Descripcionarticulo", 4);
            model.OrdenColumnas.Add("Almacen", 5);
            model.OrdenColumnas.Add("Stockseguridad", 6);
            model.OrdenColumnas.Add("Stockactual", 7);
            model.OrdenColumnas.Add("Stockminimo", 8);
            model.OrdenColumnas.Add("Pedidooptimo", 9);

            return model;
        }

        public override string GetSelectPrincipal()
        {
            return string.Format("select * from LogStockSeguridad where empresa='{0}' order by fecha desc, documento desc", Empresa);
        }

        #endregion
    }
}
