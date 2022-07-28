using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Stock;
using RBundle = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Bundle;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Stock
{
    public class BundleService : GestionService<BundleModel, Bundle>
    {
        #region CTR

        public BundleService(IContextService context,MarfilEntities db) : base(context,db)
        {
            SeparatorPk = ';';
        }

        #endregion

        #region List Index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var st = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            st.List = st.List.OfType<BundleModel>().OrderByDescending(f => f.Fecha).ThenByDescending(f => f.Codigo);
            var propiedadesVisibles = new[] {"Lote","Codigo", "Fecha", "Fkalmacendescripcion", "Fkalmacendescripcion","Fkzonaalmacendescripcion", "Fkoperarios", "Estado", "Piezas" };
            var propiedades = Helpers.Helper.getProperties<BundleModel>();
            st.PrimaryColumnns = new[] { "CampoId" };
            st.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            return st;
        }

        public override string GetSelectPrincipal()
        {
            return string.Format("select k.*,a.descripcion as [Fkalmacendescripcion],c.descripcion as [Fkoperariosdescripcion],k.id as [Codigo],al.descripcion as [Fkzonaalmacendescripcion],(select count(*) from bundlelin as bl where bl.fkbundlelote=k.lote and bl.fkbundle=k.id and bl.empresa= k.empresa) as [Piezas] from Bundle as k " +
                               " inner join almacenes as a on a.empresa=k.empresa and a.id=k.fkalmacen " +
                               " left join almaceneszona as al on al.empresa=a.empresa and al.fkalmacenes= a.id and al.id=k.fkzonaalmacen " +
                               " left join cuentas as c on c.empresa=k.empresa and c.id= k.fkoperarios where k.empresa='{0}' ",Empresa);
        }

        #endregion

        public List<BundleLinModel> AgregarLineasBundle(List<BundleLinModel> lineasActuales, IEnumerable<StockActualVistaModel> listado)
        {
            if(lineasActuales==null)
                lineasActuales=new List<BundleLinModel>();

            var diccionario =new Hashtable();
            var articulosService = FService.Instance.GetService(typeof (ArticulosModel), _context);
            var max = lineasActuales.Any() ? lineasActuales.Max(f=>f.Id)+1 : 1;
            foreach(var item in listado)
            {
                if (lineasActuales.Any() && lineasActuales.Any(f=>f.Lote!=item.Lote ))
                {
                    throw new  Exception(string.Format(RBundle.ErrorLotesMismoBloque));
                }

                if (!lineasActuales.Any(f => f.Lote == item.Lote && f.Loteid == item.Loteid && f.Fkalmacenes == item.Fkalmacenes && f.Fkarticulos == item.Fkarticulos))
                {
                    ArticulosModel articulo;
                    if (!diccionario.ContainsKey(item.Fkarticulos))
                    {
                        diccionario.Add(item.Fkarticulos,articulosService.get(item.Fkarticulos) as ArticulosModel);
                    }

                    articulo = diccionario[item.Fkarticulos] as ArticulosModel;
                    

                    lineasActuales.Add(new BundleLinModel()
                    {
                        Id=max++,
                        Fkalmacenes = item.Fkalmacenes,
                        Lote =item.Lote,
                        Loteid = item.Loteid,
                        Fkarticulos = item.Fkarticulos,
                        Descripcion =item.Descripcion,
                        Cantidad = (int?)item.Cantidad,
                        Largo = item.Largo,
                        Ancho = item.Ancho,
                        Grueso = item.Grueso,
                        Metros = item.Metros,
                        Fkunidades = articulo.Fkunidades,
                        Decimalesunidades = articulo.Decimalestotales
                    });
                }
            }

            return lineasActuales;
        }

        public void Vendido(string id)
        {
            var model = get(id) as BundleModel;
            model.Estado = EstadoKit.Vendido;

            edit(model);
        }
    }
}
