using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    class MunicipiosConverterService : BaseConverterModel<MunicipiosModel, Municipios>
    {
        public MunicipiosConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        //public override IEnumerable<IModelView> GetAll()
        //{
        //    var result = base.GetAll();

        //    using (var service = new TablasVariasService(Context, _db))
        //    {
        //        var listPaises = service.GetListPaises();
        //        return result.Select(c =>
        //        {
        //            ((MunicipiosModel)c).DescripcionPais = listPaises.Single(f => f.Valor == ((ProvinciasModel)c).Codigopais).Descripcion;
        //            return c;
        //        });
        //    }
        //}

        public override IModelView CreateView(string id)
        {
            var fmodel = new FModel();
            var objExt = fmodel.GetModel<MunicipiosModel>(Context) as IModelViewExtension;            
            var vector = objExt.generateId(id) as string[];
            var codpais = vector[0];
            var codprovincia = vector[1];
            var codmunicipio = vector[2];
            var obj = _db.Municipios.Single(f => f.codigopais == codpais && f.codigoprovincia == codprovincia && f.cod == codmunicipio);

            var result = GetModelView(obj) as MunicipiosModel;

            var service = new TablasVariasService(Context, _db);
            result.DescripcionPais = service.GetListPaises().Single(f => f.Valor == codpais).Descripcion;

            var provService = FService.Instance.GetService(typeof(ProvinciasModel), Context, _db) as ProvinciasService;
            var listaProvincias = provService.GetProvinciasPais(codpais);
            result.DescripcionProvincia = listaProvincias.SingleOrDefault(f => f.Codigopais == codpais && f.Id == codprovincia).Nombre;

            result.Codigopais = codpais;
            result.Codigoprovincia = codprovincia;

            return result;
        }


        public override Municipios CreatePersitance(IModelView obj)
        {
            var objmodel = obj as MunicipiosModel;
            var result = _db.Set<Municipios>().Create();
            
            result.codigopais = objmodel.Codigopais;
            result.codigoprovincia = objmodel.Codigoprovincia;
            result.cod = objmodel.Cod;
            result.nombre = objmodel.Nombre;

            return result;
        }

        public override Municipios EditPersitance(IModelView obj)
        {
            var model = obj as MunicipiosModel;
            var result = _db.Set<Municipios>().Single(f => f.codigopais == model.Codigopais && f.codigoprovincia == model.Codigoprovincia && f.cod == model.Cod);
            foreach (var item in result.GetType().GetProperties())
            {
                item.SetValue(result, obj.get(item.Name));
            }

            return result;
        }


    }
}

