using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Converter;
using Marfil.Inf.Genericos.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using DevExpress.Data.Helpers;
using DevExpress.DataAccess.Native.Data;
using DevExpress.DataAccess.Native.ObjectBinding;
using Marfil.Dom.Persistencia.Listados;
using Marfil.Dom.Persistencia.Listados.Base;
using Marfil.Dom.Persistencia.Model.Graficaslistados;
using Marfil.Inf.Genericos;
using DataColumn = System.Data.DataColumn;
using DataTable = System.Data.DataTable;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class ConfiguraciongraficasConverterService : BaseConverterModel<ConfiguraciongraficasModel, Configuraciongraficas>
    {
        private readonly Type[]  _numericTypes = new[] { typeof(Byte), typeof(Decimal), typeof(Double),
        typeof(Int16), typeof(Int32), typeof(Int64), typeof(SByte),
        typeof(Single), typeof(UInt16), typeof(UInt32), typeof(UInt64)};

        #region CTR

        public ConfiguraciongraficasConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
            
        }

        #endregion

        #region Api

        public override bool Exists(string id)
        {
            var valorint = int.Parse(id);
            return _db.Set<Configuraciongraficas>().Any(f => f.id == valorint && f.empresa == Empresa);
        }

        public ConfiguraciongraficasModel CrearNuevoModelo(ListadosModel listadoModel,string ejercicio)
        {
            var result = new ConfiguraciongraficasModel();
            result.Codigo = 1;
            
            //listadoModel.Ocultarlateral = true;
            result.Idlistado = listadoModel.WebIdListado;
            result.ListadoModel = listadoModel;
            result.Usuario = Context.Id;
            var estructura = CargarEstructuraConsulta(result.ListadoModel as ListadosModel);

            result.ColumnasAgruparPor = estructura.Columns.Cast<System.Data.DataColumn>().Where(f=> !f.ColumnName.StartsWith("_")).Select(f => f.ColumnName).ToList();
            result.ColumnasValores = estructura.Columns.Cast<System.Data.DataColumn>().Where(f => !f.ColumnName.StartsWith("_") && _numericTypes.Contains(f.DataType)).Select(f => f.ColumnName).ToList();
            return result;
        }

        public override IModelView CreateView(string id)
        {
            var valorint = int.Parse(id);
            var obj = _db.Set<Configuraciongraficas>().Single(f => f.id == valorint && f.empresa == Empresa);
            var result = GetModelView(obj) as ConfiguraciongraficasModel;
            //cargar estructura
            var estructura = CargarEstructuraConsulta(result.ListadoModel as ListadosModel);
            
            result.ColumnasAgruparPor = estructura.Columns.Cast<System.Data.DataColumn>().Where(f => !f.ColumnName.StartsWith("_")).Select(f=> f.ColumnName).ToList();
            result.ColumnasValores = estructura.Columns.Cast<System.Data.DataColumn>().Where(f => !f.ColumnName.StartsWith("_") && _numericTypes.Contains(f.DataType)).Select(f=>f.ColumnName).ToList();
            return result;
        }

        public override Configuraciongraficas CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as ConfiguraciongraficasModel;
            var result = _db.Configuraciongraficas.Create();
            foreach (var item in result.GetType().GetProperties())
            {
                if ((obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false) &&
                    obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.GetGenericTypeDefinition() !=
                    typeof(ICollection<>))
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

            var modeloListado = FListadosModel.Instance.GetModel(Context,viewmodel.Idlistado, Empresa, "");
            Type[] typerArgs = { modeloListado.GetType() };
            var makeme = typeof(Serializer<>).MakeGenericType(typerArgs);
            var method = makeme.GetMethod("GetXml");
            var instance = Activator.CreateInstance(makeme);
            result.filtros = method.Invoke(instance, new object[] { viewmodel.ListadoModel }) as string;

            //result.usuario = Context.RoleId;
            result.usuario = Context.Id;

            return result;
        }

        public override Configuraciongraficas EditPersitance(IModelView obj)
        {
            var viewmodel = obj as ConfiguraciongraficasModel;
            var result = _db.Configuraciongraficas.Single(f => f.id == viewmodel.Codigo && f.empresa == viewmodel.Empresa);


            foreach (var item in result.GetType().GetProperties())
            {
                if ((obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false) &&
                    obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.GetGenericTypeDefinition() !=
                    typeof(ICollection<>))
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
            var modeloListado = FListadosModel.Instance.GetModel(Context, viewmodel.Idlistado, Empresa, "");
            Type[] typerArgs = { modeloListado.GetType() };
            var makeme = typeof(Serializer<>).MakeGenericType(typerArgs);
            var method = makeme.GetMethod("GetXml");
            var instance = Activator.CreateInstance(makeme);
            result.filtros = method.Invoke(instance, new object[] { viewmodel.ListadoModel }) as string;
            result.usuario = Context.Id;
            return result;
        }

        public override IModelView GetModelView(Configuraciongraficas obj)
        {
            var result = base.GetModelView(obj) as ConfiguraciongraficasModel;
            result.set("Codigo",obj.id);
            var modeloListado = FListadosModel.Instance.GetModel(Context, obj.idlistado, Empresa, "");
            Type[] typerArgs = { modeloListado.GetType() };
            var makeme = typeof(Serializer<>).MakeGenericType(typerArgs);
            var method = makeme.GetMethod("SetXml");
            var instance = Activator.CreateInstance(makeme);
            result.ListadoModel = method.Invoke(instance, new object[] { obj.filtros }) as IListados;
            result.ListadoModel.Context = Context;
            return result;
        }


        public GraficasModel GetGraficaModel(string id)
        {
            var result =new GraficasModel(CreateView(id) as ConfiguraciongraficasModel);
            var obj = result.ListadoModel as ListadosModel;

            var cadenaselect = obj.Select;
            cadenaselect = Regex.Replace(cadenaselect, "order by.*", string.Empty);
            result.Datos = Query(cadenaselect, obj.ValoresParametros, result.Agruparpor, result.Valores);
            
            return result;
        }

        

        #endregion

        #region Helpers

        private DataTable CargarEstructuraConsulta(ListadosModel listado)
        {
            var select = listado.Select.Replace("select ", "select top 0 ");

            return Query(select, listado.ValoresParametros);
        }

        DataTable Query(string select, Dictionary<string, object> parametros,string agruparpor= "", string valores ="")
        {
            var result = new System.Data.DataTable();
            using (var con = new SqlConnection(_db.Database.Connection.ConnectionString))
            {
                var cadenaseelect = string.IsNullOrEmpty(agruparpor)
                    ? select
                    : string.Format("select a.[{0}], sum(a.[{1}]) as [{1}] from ({2}) as a group by a.[{0}]",agruparpor,valores, select);
                using (var cmd = new SqlCommand(cadenaseelect, con))
                {
                    foreach (var item in parametros)
                    {
                        cmd.Parameters.AddWithValue(item.Key, item.Value);
                    }

                    using (var ad = new SqlDataAdapter(cmd))
                    {
                        ad.Fill(result);
                    }
                }
            }


            return result;
        }

        private DataTable CrearDatosGrafico(DataTable data, GraficasModel model)
        {
            var groupdata = data.DefaultView.ToTable("group", true, new[] { model.Agruparpor});
            var result = new DataTable();
            result.Columns.Add(model.Agruparpor);
            result.Columns.Add(model.Valores,data.Columns[model.Valores].DataType);

            foreach (DataRow item in groupdata.Rows)
            {
                var row = result.NewRow();
                row[model.Agruparpor] = item[model.Agruparpor];
                row[model.Valores] = data.Compute("sum(["+ model.Valores +"])", "[" +model.Agruparpor + "] = "+ GetValorSql(data.Columns[model.Agruparpor], item[model.Agruparpor]));
                result.Rows.Add(row);
            }
            result.DefaultView.Sort = string.Format("{0} asc",model.Agruparpor);
            return result;

        }

        private string GetValorSql(DataColumn dataColumn, object o)
        {
            if (dataColumn.DataType == typeof (string))
            {
                return string.Format("'{0}'", Funciones.Qnull(o));
            }
            else if ( dataColumn.DataType == typeof(DateTime))
            {
                var date =(DateTime) o;
                return string.Format("#{0}#", date.ToString(CultureInfo.InvariantCulture));
            }

            return string.Format("{0}", Funciones.Qnull(o));
        }

        #endregion
    }
}

