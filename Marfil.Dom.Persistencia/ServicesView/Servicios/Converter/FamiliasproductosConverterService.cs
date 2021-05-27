using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Inf.Genericos;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class FamiliasproductosConverterService : BaseConverterModel<FamiliasproductosModel, Familiasproductos>
    {
        public FamiliasproductosConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        #region API

        public override IEnumerable<IModelView> GetAll()
        {
            return _db.Familiasproductos.Where(f => f.empresa == Empresa).ToList().Select(f=>GetModelView(f));
        }

        public override bool Exists(string id)
        {
            return _db.Set<Familiasproductos>().Any(f => f.id == id && f.empresa == Empresa);
        }

        public override IModelView CreateView(string id)
        {
            var obj = _db.Set<Familiasproductos>().Single(f => f.id == id && f.empresa == Empresa);
            var result = GetModelView(obj) as FamiliasproductosModel;
            //Descripcion
            PartesDeLaDescripcion? parte1;
            PartesDeLaDescripcion? parte2;
            PartesDeLaDescripcion? parte3;
            PartesDeLaDescripcion? parte4;
            PartesDeLaDescripcion? parte5;
            PartesDeLaDescripcion? parte6;
            GetEnumFromDescripcion(result.Descripcion1generada,out parte1,out parte2,out parte3,out parte4,out parte5,out parte6);
            result.Descripcion1Generada_1 = parte1;
            result.Descripcion1Generada_2 = parte2;
            result.Descripcion1Generada_3 = parte3;
            result.Descripcion1Generada_4 = parte4;
            result.Descripcion1Generada_5 = parte5;
            result.Descripcion1Generada_6 = parte6;

            GetEnumFromDescripcion(result.Descripcion2generada, out parte1, out parte2, out parte3, out parte4, out parte5, out parte6);
            result.Descripcion2Generada_1 = parte1;
            result.Descripcion2Generada_2 = parte2;
            result.Descripcion2Generada_3 = parte3;
            result.Descripcion2Generada_4 = parte4;
            result.Descripcion2Generada_5 = parte5;
            result.Descripcion2Generada_6 = parte6;

            GetEnumFromDescripcion(result.Descripcionabreviadagenerada, out parte1, out parte2, out parte3, out parte4, out parte5, out parte6);
            result.Descripcionabreviadagenerada_1 = parte1;
            result.Descripcionabreviadagenerada_2 = parte2;
            result.Descripcionabreviadagenerada_3 = parte3;
            result.Descripcionabreviadagenerada_4 = parte4;
            result.Descripcionabreviadagenerada_5 = parte5;
            result.Descripcionabreviadagenerada_6 = parte6;
            //end descripcion
            return result;
        }

       public override Familiasproductos CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as FamiliasproductosModel;
            var result = _db.Familiasproductos.Create();
            var fkunidadesmedida = viewmodel.Fkunidadesmedida;
            var tipofamilia = viewmodel.Tipofamilia;
            foreach (var item in result.GetType().GetProperties())
            {
                if (obj.GetType().GetProperty(item.Name.FirstToUpper()).PropertyType.IsGenericType &&
                    obj.GetType().GetProperty(item.Name.FirstToUpper()).PropertyType.GetGenericTypeDefinition() !=
                    typeof(ICollection<>))
                {
                    item.SetValue(result, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
                else if (obj.GetType().GetProperty(item.Name.FirstToUpper()).PropertyType.IsEnum)
                {
                    item.SetValue(result, (int)obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
                else if (!obj.GetType().GetProperty(item.Name.FirstToUpper()).PropertyType.IsGenericType)
                {
                    item.SetValue(result, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
            }
            result.gestionarcaducidad = viewmodel.Gestionarcaducidad;
            result.tipogestionlotes = (int)viewmodel.Tipogestionlotes;
            result.fkunidadesmedida = fkunidadesmedida;
            result.tipofamilia = (int)tipofamilia;
            result.descripcion1generada = GenerarDescripcionGenerada(viewmodel.Descripcion1Generada_1,
               viewmodel.Descripcion1Generada_2, viewmodel.Descripcion1Generada_3, viewmodel.Descripcion1Generada_4,
               viewmodel.Descripcion1Generada_5, viewmodel.Descripcion1Generada_6);

            result.descripcion2generada = GenerarDescripcionGenerada(viewmodel.Descripcion2Generada_1,
               viewmodel.Descripcion2Generada_2, viewmodel.Descripcion2Generada_3, viewmodel.Descripcion2Generada_4,
               viewmodel.Descripcion2Generada_5, viewmodel.Descripcion2Generada_6);

            result.descripcionabreviadagenerada = GenerarDescripcionGenerada(viewmodel.Descripcionabreviadagenerada_1,
               viewmodel.Descripcionabreviadagenerada_2, viewmodel.Descripcionabreviadagenerada_3, viewmodel.Descripcionabreviadagenerada_4,
               viewmodel.Descripcionabreviadagenerada_5, viewmodel.Descripcionabreviadagenerada_6);
            return result;
        }

        public override Familiasproductos EditPersitance(IModelView obj)
        {
            var viewmodel = obj as FamiliasproductosModel;
            var result = _db.Familiasproductos.Single(f => f.id == viewmodel.Id && f.empresa == viewmodel.Empresa);

            var fkunidadesmedida = result.fkunidadesmedida;
            var tipofamilia = viewmodel.Tipofamilia;

            foreach (var item in result.GetType().GetProperties())
            {
                if (obj.GetType().GetProperty(item.Name.FirstToUpper()).PropertyType.IsGenericType &&
                    obj.GetType().GetProperty(item.Name.FirstToUpper()).PropertyType.GetGenericTypeDefinition() !=
                    typeof(ICollection<>))
                {
                    item.SetValue(result, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
                else if (obj.GetType().GetProperty(item.Name.FirstToUpper()).PropertyType.IsEnum)
                {
                    item.SetValue(result, (int)obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
                else if (!obj.GetType().GetProperty(item.Name.FirstToUpper()).PropertyType.IsGenericType)
                {
                    item.SetValue(result, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
            }
            result.gestionarcaducidad = viewmodel.Gestionarcaducidad;
            result.tipogestionlotes = (int)viewmodel.Tipogestionlotes;
            result.descripcion1generada = GenerarDescripcionGenerada(viewmodel.Descripcion1Generada_1,
               viewmodel.Descripcion1Generada_2, viewmodel.Descripcion1Generada_3, viewmodel.Descripcion1Generada_4,
               viewmodel.Descripcion1Generada_5, viewmodel.Descripcion1Generada_6);

            result.descripcion2generada = GenerarDescripcionGenerada(viewmodel.Descripcion2Generada_1,
               viewmodel.Descripcion2Generada_2, viewmodel.Descripcion2Generada_3, viewmodel.Descripcion2Generada_4,
               viewmodel.Descripcion2Generada_5, viewmodel.Descripcion2Generada_6);

            result.descripcionabreviadagenerada = GenerarDescripcionGenerada(viewmodel.Descripcionabreviadagenerada_1,
               viewmodel.Descripcionabreviadagenerada_2, viewmodel.Descripcionabreviadagenerada_3, viewmodel.Descripcionabreviadagenerada_4,
               viewmodel.Descripcionabreviadagenerada_5, viewmodel.Descripcionabreviadagenerada_6);


            result.fkunidadesmedida = fkunidadesmedida;
            result.tipofamilia = (int)tipofamilia;            
            return result;
        }

        public override IModelView GetModelView(Familiasproductos obj)
        {
            var result = base.GetModelView(obj) as FamiliasproductosModel;
            result.Tipogestionlotes = obj.tipogestionlotes.HasValue ? (Tipogestionlotes)obj.tipogestionlotes : Tipogestionlotes.Singestion;
            result.Gestionarcaducidad = obj.gestionarcaducidad ?? false;
            return result;
        }

        #endregion

        #region Helper

        private string GenerarDescripcionGenerada(PartesDeLaDescripcion? familia, PartesDeLaDescripcion? materiales,
            PartesDeLaDescripcion? caracteristica, PartesDeLaDescripcion? grosor, PartesDeLaDescripcion? acabado,
            PartesDeLaDescripcion? familiamaterial)
        {
            var sb=new StringBuilder();

            sb.AppendFormat("{0},{1},{2},{3},{4},{5}", familia.HasValue ? ((int) familia).ToString() : ""
                , materiales.HasValue ? ((int)materiales).ToString() : ""
                , caracteristica.HasValue ? ((int)caracteristica).ToString() : ""
                , grosor.HasValue ? ((int)grosor).ToString() : ""
                , acabado.HasValue ? ((int)acabado).ToString() : ""
                , familiamaterial.HasValue ? ((int)familiamaterial).ToString() : "");

            return sb.ToString();
        }

        private void GetEnumFromDescripcion(string descripcion, out PartesDeLaDescripcion? familia,
            out PartesDeLaDescripcion? materiales,
            out PartesDeLaDescripcion? caracteristica, out PartesDeLaDescripcion? grosor,
            out PartesDeLaDescripcion? acabado,
            out PartesDeLaDescripcion? familiamaterial)
        {
            familia = null;
            materiales = null;
            caracteristica = null;
            grosor = null;
            acabado = null;
            familiamaterial = null;

            var vector = descripcion.Split(',');
            for(var i=0;i< vector.Length;i++)
            {
                var j = i + 1;
                PartesDeLaDescripcion element;
                if (Enum.TryParse(vector[i], out element))
                {
                    if (j == 1)
                    {
                        familia = element;
                    }
                    else if (j == 2)
                    {
                        materiales = element;
                    }
                    else if (j == 3)
                    {
                        caracteristica = element;
                    }
                    else if (j == 4)
                    {
                        grosor = element;
                    }
                    else if (j == 5)
                    {
                        acabado = element;
                    }
                    else if (j == 6)
                    {
                        familiamaterial = element;
                    }
                }
                    
            }
        }

        #endregion
    }
}
