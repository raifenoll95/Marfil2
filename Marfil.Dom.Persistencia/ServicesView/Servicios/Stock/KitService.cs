using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Validation;
using Marfil.Inf.Genericos.Helper;
using RKit = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Kit;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Stock
{
    public class KitService : GestionService<KitModel, Kit>
    {
        #region Members

        private bool _flagDesmontar = false;

        #endregion

        #region CTR

        public KitService(IContextService  context,MarfilEntities db) : base(context,db)
        {
            
        }

        #endregion

        #region List Index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var st = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            st.List = st.List.OfType<KitModel>().OrderByDescending(f => f.Fechadocumento).ThenByDescending(f => f.Referencia);
            var propiedadesVisibles = new[] { "Referencia","Descripcion","Fecha", "Fkalmacendescripcion","Fkoperarios", "Fkzonaalmacendescripcion", "Estado","Piezas" };
            var propiedades = Helpers.Helper.getProperties<KitModel>();
            st.PrimaryColumnns = new[] { "Id" };
            st.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            
            return st;
        }

        public override string GetSelectPrincipal()
        {
            return string.Format("select k.*,a.descripcion as [Fkalmacendescripcion],c.descripcion as [Fkoperariosdescripcion],al.descripcion as [Fkzonaalmacendescripcion] ,(select count(*) from kitlin as bl where bl.fkkit=k.id and bl.empresa= k.empresa) as [Piezas] from kit as k " +
                               " inner join almacenes as a on a.empresa=k.empresa and a.id=k.fkalmacen " +
                               " left join almaceneszona as al on al.empresa=a.empresa and al.fkalmacenes= a.id and al.id=k.fkzonalamacen " +
                               " left join cuentas as c on c.empresa=k.empresa and c.id= k.fkoperarios where k.empresa='{0}' ",Empresa);

            
        }

        #endregion

        public List<KitLinModel> AgregarLineasKit(List<KitLinModel> lineasActuales, IEnumerable<StockActualVistaModel> listado)
        {
            if(lineasActuales==null)
                lineasActuales=new List<KitLinModel>();

            var diccionario =new Hashtable();
            var articulosService = FService.Instance.GetService(typeof (ArticulosModel), _context);
            var max = lineasActuales.Any() ? lineasActuales.Max(f=>f.Id)+1 : 1;
            foreach(var item in listado)
            {
                if (!lineasActuales.Any(f => f.Lote == item.Lote && f.Loteid == item.Loteid && f.Fkalmacenes == item.Fkalmacenes && f.Fkarticulos == item.Fkarticulos))
                {
                    ArticulosModel articulo;
                    if (!diccionario.ContainsKey(item.Fkarticulos))
                    {
                        diccionario.Add(item.Fkarticulos,articulosService.get(item.Fkarticulos) as ArticulosModel);
                    }

                    articulo = diccionario[item.Fkarticulos] as ArticulosModel;
                    

                    lineasActuales.Add(new KitLinModel()
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

        public override void create(IModelView obj)
        {
            var modelview = obj as KitModel;
            modelview.Estado = modelview.Lineas.Any() ? EstadoKit.Montado : EstadoKit.EnProceso;
            //Calculo ID
            var tipodocumento = "KIT"; //Kit
            var contador = ServiceHelper.GetNextId<Kit>(_db, Empresa, modelview.Fkseries, tipodocumento);
            var identificadorsegmento = "";
            modelview.Referencia = ServiceHelper.GetReference<Kit>(_db, modelview.Empresa, modelview.Fkseries, tipodocumento, contador, modelview.Fechadocumento, out identificadorsegmento);
            modelview.Identificadorsegmento = identificadorsegmento;

            base.create(obj);
        }

        public override void edit(IModelView obj)
        {
            var modelview = obj as KitModel;
            
            if(!_flagDesmontar && (modelview.Estado==EstadoKit.Desmontado || modelview.Estado==EstadoKit.Vendido))
                throw new ValidationException(string.Format(RKit.ErrorModificacionKitEstado,Funciones.GetEnumByStringValueAttribute(modelview.Estado)));

            if (!_flagDesmontar && modelview.Estado==EstadoKit.EnProceso && modelview.Lineas.Any())
            {
                modelview.Estado = EstadoKit.Montado;
            }

            base.edit(obj);
        }

        public void Desmontar(string id)
        {
            _flagDesmontar = true;
            var model = get(id) as KitModel;
            model.Estado=EstadoKit.Desmontado;
            edit(model);
        }
    }
}
