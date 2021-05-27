using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Documentos.Inventarios;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class InventariosConverterService : BaseConverterModel<InventariosModel, Inventarios>
    {

        #region CTR

        public InventariosConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
            
        }

        #endregion

        #region Api

        public override bool Exists(string id)
        {
            var intId = Funciones.Qint(id);
            return _db.Set<Inventarios>().Any(f => f.id == intId && f.empresa == Empresa);
        }

        public override IModelView CreateView(string id)
        {
            var intId = Funciones.Qint(id);
            var obj = _db.Set<Inventarios>().Single(f => f.id == intId && f.empresa == Empresa);
            var result = GetModelView(obj) as InventariosModel;
            result.Tipodealmacenlote = (TipoAlmacenlote?)obj.tipoalmacenlote;

            result.Lineas = obj.InventariosLin.Select(f => new InventariosLinModel()
            {
                
                
                Id = f.id,
                Fkarticulos = f.fkarticulos,
                Descripcion = f.descripcion,
                Lote = f.lote,
                Loteid = f.loteid,
                Referenciaproveedor = f.referenciaproveedor,
                Tag = f.tag,
                Fkunidadesmedida = f.fkunidadesmedida,
                Cantidad = f.cantidad,
                Largo = f.largo,
                Ancho = f.ancho,
                Grueso = f.grueso,
                Metros = f.metros,
                Fkcalificacioncomercial = f.fkcalificacioncomercial,
                Fktipograno = f.fktipograno,
                Fktonomaterial = f.fktonomaterial,
                Pesonetolote = f.pesonetolote,
                Fkincidenciasmaterial = f.fkincidenciasmaterial,
                Fkvariedades = f.fkvariedades,
                Estado = (EstadoLineaInventario)f.estado,
                Decimalesmedidas = f.decimalesmedidas
            }).ToList();

            return result;
        }

        public override Inventarios CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as InventariosModel;
            var result = _db.Inventarios.Create();

           result.tipoalmacenlote = (int?)viewmodel.Tipodealmacenlote;

            foreach (var item in result.GetType().GetProperties())
            {
                if (typeof(InventariosModel).GetProperties().Any(f => f.Name.ToLower() == item.Name.ToLower()))
                    item.SetValue(result, viewmodel.get(item.Name));
            }


            result.InventariosLin.Clear();


            var i = 1;
            foreach (var item in viewmodel.Lineas)
            {
                result.InventariosLin.Add(new InventariosLin()
                {
                    empresa = viewmodel.Empresa,
                    fkinventarios = viewmodel.Id,
                    id = i++,
                    fkarticulos = item.Fkarticulos,
                    descripcion = item.Descripcion,
                    lote = item.Lote,
                    loteid = item.Loteid,
                    referenciaproveedor = item.Referenciaproveedor,
                    tag = item.Tag,
                    fkunidadesmedida = item.Fkunidadesmedida,
                    cantidad = item.Cantidad??0,
                    largo = item.Largo,
                    ancho = item.Ancho,
                    grueso = item.Grueso,
                    metros = item.Metros,
                    fkcalificacioncomercial = item.Fkcalificacioncomercial,
                    fktipograno = item.Fktipograno,
                    fktonomaterial = item.Fktonomaterial,
                    pesonetolote = item.Pesonetolote,
                    fkincidenciasmaterial = item.Fkincidenciasmaterial,
                    fkvariedades = item.Fkvariedades,
                    estado = (int) item.Estado,
                    decimalesmedidas = item.Decimalesmedidas
                });
            }

            return result;
        }

        public override Inventarios EditPersitance(IModelView obj)
        {
            var viewmodel = obj as InventariosModel;

            var intId = Funciones.Qint(viewmodel.Id);

            var result = _db.Inventarios.Single(f => f.id == intId && f.empresa == viewmodel.Empresa);

            result.tipoalmacenlote = (int?)viewmodel.Tipodealmacenlote;

            foreach (var item in result.GetType().GetProperties())
            {
                if(typeof(InventariosModel).GetProperties().Any(f=>f.Name.ToLower()== item.Name.ToLower()))
                    item.SetValue(result, viewmodel.get(item.Name));
            }

            result.InventariosLin.Clear();
            var i = 1;
            foreach (var item in viewmodel.Lineas)
            {
                result.InventariosLin.Add(new InventariosLin()
                {
                    empresa = viewmodel.Empresa,
                    fkinventarios = viewmodel.Id,
                    id = i++,
                    fkarticulos = item.Fkarticulos,
                    descripcion = item.Descripcion,
                    lote = item.Lote,
                    loteid = item.Loteid,
                    referenciaproveedor = item.Referenciaproveedor,
                    tag=item.Tag,
                    fkunidadesmedida = item.Fkunidadesmedida,
                    cantidad= item.Cantidad??0,
                    largo = item.Largo,
                    ancho = item.Ancho,
                    grueso = item.Grueso,
                    metros= item.Metros,
                    fkcalificacioncomercial = item.Fkcalificacioncomercial,
                    fktipograno = item.Fktipograno,
                    fktonomaterial = item.Fktonomaterial,
                    pesonetolote = item.Pesonetolote,
                    fkincidenciasmaterial = item.Fkincidenciasmaterial,
                    fkvariedades = item.Fkvariedades,
                    estado=(int)item.Estado,
                    decimalesmedidas = item.Decimalesmedidas
                });
            }
            
            return result;
        }

        #endregion
    }
}
