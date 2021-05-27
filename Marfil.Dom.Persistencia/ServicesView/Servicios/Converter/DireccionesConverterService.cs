using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class DireccionesConverterService : BaseConverterModel<DireccionesLinModel, Direcciones>
    {
        #region CTR

        public DireccionesConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
            
        }

        #endregion

        public override IEnumerable<IModelView> GetAll()
        {
            var list = _db.Set<Direcciones>().Where(f => f.empresa == Empresa).ToList();
            var result = new List<DireccionesLinModel>();
            foreach (var item in list)
            {
                var newItem = GetModelView(item) as DireccionesLinModel;
                
                result.Add(newItem);
            }

            return result;
        }

        public override bool Exists(string id)
        {

            var st = GetPrimaryKey(id);
            return _db.Set<Direcciones>().Any(f => f.id == st.Id && f.empresa == st.Empresa && f.tipotercero == st.Tipotercero && f.fkentidad == st.Fkentidad);
        }

        public override IModelView CreateView(string id)
        {
            var tablasservice = new TablasVariasService(Context,_db);
            var provincias =new ProvinciasService(Context,_db);
            var tipoviaservicio = tablasservice.GetTablasVariasByCode(2030);
            var tipodireccionservicio = tablasservice.GetTablasVariasByCode(2010);
            var st = GetPrimaryKey(id);
            var obj = _db.Set<Direcciones>().Single(f => f.id == st.Id && f.empresa == st.Empresa && f.fkentidad == st.Fkentidad);

            var result = GetModelView(obj) as DireccionesLinModel;
            result.Pais = tablasservice.GetListPaises().FirstOrDefault(f => f.Valor == obj.fkpais)?.Descripcion;
            result.Provincia = (provincias.get(string.Format("{0}-{1}", obj.fkpais, obj.fkprovincia)) as ProvinciasModel)?.Nombre;
            result.Tipovia = tipoviaservicio.Lineas.FirstOrDefault(f => f.Valor == obj.fkpais)?.Descripcion;
            result.Tipodireccion = tipoviaservicio.Lineas.FirstOrDefault(f => f.Valor == obj.fktipodireccion)?.Descripcion;
            return result;
        }

        public override Direcciones EditPersitance(IModelView obj)
        {
            var objext = obj as IModelViewExtension;
            var st = obj as DireccionesLinModel;
            var result = _db.Set<Direcciones>().Single(f => f.id == st.Id && f.empresa == st.Empresa && f.tipotercero == (int)st.Tipotercero && f.fkentidad == st.Fkentidad);

            foreach (var item in result.GetType().GetProperties())
            {
                item.SetValue(result, obj.get(item.Name));
            }

            
            return result;
        }

        public override IModelView GetModelView(Direcciones obj)
        {
            var tablasservice = new TablasVariasService(Context,_db);
            var provincias = new ProvinciasService(Context,_db);

            var resultado= base.GetModelView(obj) as DireccionesLinModel;
            resultado.Pais = tablasservice.GetListPaises().FirstOrDefault(f => f.Valor == obj.fkpais)?.Descripcion;
            if(!string.IsNullOrEmpty(obj.fkpais) && !string.IsNullOrEmpty(obj.fkprovincia))
                resultado.Provincia = (provincias.get(string.Format("{0}-{1}", obj.fkpais, obj.fkprovincia)) as ProvinciasModel)?.Nombre;

            return resultado;
        }

        #region Helpers

        private struct stPrimaryKey
        {
            public string Empresa { get; set; }
            public int Tipotercero { get; set; }
            public string Fkentidad { get; set; }
            public int Id { get; set; }
        }

        private stPrimaryKey GetPrimaryKey(string id)
        {
            var vector = id.Split(DireccionesLinModel.SeparatorPk);
            return new stPrimaryKey()
            {
                Empresa = Empresa,
                Tipotercero = int.Parse(vector[0]),
                Fkentidad = vector[1],
                Id = int.Parse(vector[2])
            };
        }

        #endregion


    }
}
