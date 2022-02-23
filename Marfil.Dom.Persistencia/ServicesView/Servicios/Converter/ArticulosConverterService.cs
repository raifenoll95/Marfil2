using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Inf.Genericos.Helper;
using Marfil.Inf.ResourcesGlobalization.Textos.Entidades;
using Marfil.Inf.Genericos;
using System.Data.Entity;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class ArticulosConverterService : BaseConverterModel<ArticulosModel, Articulos>
    {
        #region CTR 

        public ArticulosConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        #endregion

        #region API

        public override IEnumerable<IModelView> GetAll()
        {
            var result = new List<ArticulosModel>();
            var list = _db.Articulos.Where(f => f.empresa == Empresa).ToList();
            
            var familiasList = _db.Familiasproductos.Where(f => f.empresa == Empresa);
            foreach (var item in list)
            {
                var obj = GetModelView(item) as ArticulosModel;
                obj.Fkunidades = familiasList.Single(f => f.id == obj.Familia).fkunidadesmedida;
                result.Add(obj);
            }
            return result;
        }

        public override bool Exists(string id)
        {
            return _db.Set<Articulos>().Any(f => f.id == id && f.empresa == Empresa);
        }

        public override IModelView CreateView(string id)
        {
            var obj = _db.Set<Articulos>().Single(f => f.id == id && f.empresa == Empresa);
            var result = GetModelView(obj) as ArticulosModel;
            var familiasService = FService.Instance.GetService(typeof (FamiliasproductosModel), Context, _db);
            var materialesService = FService.Instance.GetService(typeof(MaterialesModel), Context, _db);
            var caracteristicasService = FService.Instance.GetService(typeof(CaracteristicasModel), Context, _db);
            var grosoresService = FService.Instance.GetService(typeof(GrosoresModel), Context, _db);
            var acabadosService = FService.Instance.GetService(typeof(AcabadosModel), Context, _db);
           

            result.Familia = ArticulosService.GetCodigoFamilia(result.Id);
            var familiaObj = familiasService.get(result.Familia) as FamiliasproductosModel;
            result.Validarmateriales = familiaObj.Validarmaterial;
            result.Validarcaracteristicas = familiaObj.Validarcaracteristica;
            result.Validargrosores = familiaObj.Validargrosor;
            result.Validaracabados = familiaObj.Validaracabado;
            result.Fkcontadores = familiaObj.Fkcontador;
            result.Materiales = familiaObj.Validarmaterial ? ArticulosService.GetCodigoMaterial(result.Id):string.Empty;
            result.Caracteristicas = familiaObj.Validarcaracteristica ? ArticulosService.GetCodigoCaracteristica(result.Id): string.Empty;
            result.Grosores = familiaObj.Validargrosor ? ArticulosService.GetCodigoGrosor(result.Id) :string.Empty;
            result.Acabados = familiaObj.Validaracabado ? ArticulosService.GetCodigoAcabado(result.Id) : string.Empty;
            result.Lotefraccionable = obj.lotefraccionable;
            result.Codigolibre = ArticulosService.GetCodigoLibre(result.Id, familiaObj.Validarmaterial,familiaObj.Validarcaracteristica);
            result.Tipoivavariable = obj.tipoivavariable;
            var materialesObj = familiaObj.Validarmaterial ? materialesService.get(result.Materiales) as MaterialesModel : null;
            result.FamiliaDescripcion = familiaObj.Descripcion;
            result.Tipofamilia = (int)familiaObj.Tipofamilia;
            result.Fkcontadores = familiaObj.Fkcontador;

            result.MaterialesDescripcion = familiaObj.Validarmaterial ? materialesObj.Descripcion : string.Empty;
            result.CaracteristicasDescripcion = familiaObj.Validarcaracteristica ? ((CaracteristicasModel)caracteristicasService.get(result.Familia)).Lineas.Single(f=>f.Id==result.Caracteristicas).Descripcion : string.Empty;
            result.GrosoresDescripcion = familiaObj.Validargrosor ? grosoresService.get(result.Grosores).get("Descripcion").ToString() : string.Empty;
            result.AcabadosDescripcion = familiaObj.Validaracabado ? acabadosService.get(result.Acabados).get("Descripcion").ToString() :string.Empty;
            result.Fkunidades = familiaObj.Fkunidadesmedida;
            result.Fkgruposmateriales = obj.fkgruposmateriales;

            result.Ean13 = obj.ean13;

            //ser tarifas
            result.TarifasSistemaVenta = GetTarifas(TipoFlujo.Venta, result.Id);
            result.TarifasSistemaCompra = GetTarifas(TipoFlujo.Compra, result.Id);
            result.TarifasEspecificasVentas = new TarifaEspecificaArticulo()
            {
                Tipo=TipoFlujo.Venta,
                Lineas = _appService.GetTarifasEspecificas(TipoFlujo.Venta, id,Empresa).ToList()
            };
            result.TarifasEspecificasCompras = new TarifaEspecificaArticulo()
            {
                Tipo = TipoFlujo.Compra,
                Lineas = _appService.GetTarifasEspecificas(TipoFlujo.Compra, id, Empresa).ToList()
            };

            //articulos documentos
            result.Largo = obj.largo ?? 0;
            result.Ancho = obj.ancho ?? 0;
            result.Grueso = obj.grueso ?? 0;
            
            var unidad = _db.Unidades.Single(f => f.id == familiaObj.Fkunidadesmedida);
            result.Decimalestotales = unidad.decimalestotales;
            result.Formulas=(TipoStockFormulas)Enum.Parse(typeof(TipoStockFormulas), unidad.formula.ToString());
            result.Fkunidades = familiaObj.Fkunidadesmedida;
            result.Permitemodificarmetros = unidad.tipototal == (int) TipoStockTotal.Editado;

            result.Fechaultimaentrada = obj.fechaultimaentrada;
            result.Ultimaentrada = obj.ultimaentrada;
            result.Fechaultimasalida = obj.fechaultimasalida;
            result.Ultimasalida = obj.ultimasalida;


            // Necesario para crear la url para el último albarán de salida/entrada
            result.idAlbaranEntrada = _db.AlbaranesCompras.Where(f => f.empresa == Empresa && f.referencia == result.Ultimaentrada).Select(f => f.id).SingleOrDefault();            
            result.modoAlbaranEntrada = _db.AlbaranesCompras.Where(f => f.empresa == Empresa && f.referencia == result.Ultimaentrada).Select(f => f.modo).SingleOrDefault();

            result.idAlbaranSalida = _db.Albaranes.Where(f => f.empresa == Empresa && f.referencia == result.Ultimasalida).Select(f => f.id).SingleOrDefault();            
            result.modoAlbaranSalida = _db.Albaranes.Where(f => f.empresa == Empresa && f.referencia == result.Ultimasalida).Select(f => f.modo).SingleOrDefault();

            return result;
        }

        private List<TarifasSistemaArticulosViewModel> GetTarifas(TipoFlujo tipo,string id)
        {

            //Recoge todas las lineas de tarifas de venta de sistema
            var tarifas = _db.Tarifas.Where(f => f.empresa == Empresa && f.tipotarifa == (int)TipoTarifa.Sistema && f.tipoflujo == (int)tipo).Include(f => f.TarifasLin).ToList();
            List<TarifasSistemaArticulosViewModel> tarifasreturn = new List<TarifasSistemaArticulosViewModel>();

            //Rai 22/03/2021
            //id --> fkarticulos

            //Recorremos todas las tarifas de sistema de venta
            foreach (var tarifa in tarifas)
            {
                //Recorremos todas las lineas de esa tarifa para ver si alguna tiene ese id
                foreach (var linea in tarifa.TarifasLin)
                {
                    if (linea.fkarticulos == id)
                    {
                        tarifasreturn.Add(new TarifasSistemaArticulosViewModel()
                        {
                            Id = tarifa.id, //Tarida.Id = "PC1"
                            Descripcion = tarifa.descripcion,
                            Precio = linea.precio.Value,
                            Obligatorio = tarifa.precioobligatorio.Value


                        });
                    }
                }
            }

            return tarifasreturn;


        //    var list = _appService.GetTarifasSistema(tipo, true, Empresa);
        //    //var existing = _db.TarifasLin.Where(f => f.empresa == Empresa && f.fkarticulos == id).ToList();
        //    var existing = _db.TarifasLin.Where(f => f.empresa == Empresa && f.fkarticulos == id && f.Tarifas.tipotarifa == (int)TipoTarifa.Sistema && f.Tarifas.tipoflujo == (int)tipo).ToList();
        //    return
        //        list.Select(
        //            f =>
        //                new TarifasSistemaArticulosViewModel()
        //                {
        //                    Id = f.Id,
        //                    Descripcion = f.Descripcion,
        //                    Obligatorio = f.Obligatorio,
        //                    Precio = existing.Where(x => x.empresa == Empresa && x.Tarifas.id == f.Id && x.fkarticulos == id).Select(x => x.precio).SingleOrDefault().Value
        //                    //Precio =
        //                    //    existing.SingleOrDefault(
        //                    //        j => j.Tarifas.id == f.Id && j.fkarticulos == id)?.precio ?? 0
        //                }).ToList();
        //
        }

        public override Articulos CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as ArticulosModel;
            var result = _db.Articulos.Create();

            /*
            foreach (var item in result.GetType().GetProperties())
            {
                if (typeof(ArticulosModel).GetProperties().Any(f => f.Name.ToLower() == item.Name.ToLower() && item.Name.ToLower() != "tipogestionlotes") )
                    item.SetValue(result, viewmodel.get(item.Name));
            }
            */

            foreach (var item in result.GetType().GetProperties())
            {
                if ((obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false) &&
                    (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.GetGenericTypeDefinition() !=
                    typeof(ICollection<>)) && item.Name.ToLower() != "tipogestionlotes" && item.Name.ToLower() != "articulostercero" && item.Name.ToLower() != "articuloscomponentes" && item.Name.ToLower() != "articulosstockseguridad")
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
            result.tipogestionlotes = (int)viewmodel.Tipogestionlotes;
            result.ArticulosTercero.Clear();

            foreach (var item in viewmodel.ArticulosTercero)
            {
                var newitem = _db.ArticulosTercero.Create();
                newitem.empresa = result.empresa;
                newitem.id = item.Id;
                newitem.codarticulo = item.CodArticulo;
                newitem.codtercero = item.CodTercero;
                newitem.descripciontercero = item.DescripcionTercero;
                newitem.codarticulotercero = item.CodArticuloTercero;
                newitem.descripcionarticulotercero = item.Descripcion;
                result.ArticulosTercero.Add(newitem);
            }

            result.ArticulosComponentes.Clear();
            foreach (var item in viewmodel.ArticulosComponentes)
            {
                var newitem = _db.ArticulosComponentes.Create();
                newitem.empresa = result.empresa;
                newitem.fkarticulo = item.FkArticulo;
                newitem.id = item.Id;
                newitem.idcomponente = item.IdComponente;
                newitem.descripcioncomponente = item.DescripcionComponente;
                newitem.piezas = item.Piezas;
                newitem.largo = item.Largo;
                newitem.ancho = item.Ancho;
                newitem.grueso = item.Grueso;
                newitem.merma = item.Merma;
                result.ArticulosComponentes.Add(newitem);
            }

            result.ArticulosStockSeguridad.Clear();
            foreach (var item in viewmodel.ArticulosStockSeguridad)
            {
                var newitem = _db.ArticulosStockSeguridad.Create();
                newitem.empresa = result.empresa;
                newitem.id = item.Id;
                newitem.codalmacen = item.Codalmacen;
                newitem.descripcionalmacen = item.Descripcionalmacen;
                newitem.codarticulo = result.id;
                newitem.stockseguridad = result.stockseguridad;
                newitem.stockminimo = item.Stockminimo;
                newitem.stockmaximo = item.Stockmaximo;
                
                result.ArticulosStockSeguridad.Add(newitem);
            }

            return result;
        }

        public override Articulos EditPersitance(IModelView obj)
        {
            var viewmodel = obj as ArticulosModel;
            //var result = _db.Articulos.Single(f => f.id == viewmodel.Id && f.empresa == viewmodel.Empresa);
            var result = _db.Articulos.Where(f => f.id == viewmodel.Id && f.empresa == viewmodel.Empresa).Include(b => b.ArticulosTercero).ToList().Single();

            /*
            foreach (var item in result.GetType().GetProperties())
            {
                if (typeof(ArticulosModel).GetProperties().Any(f => f.Name.ToLower() == item.Name.ToLower() && item.Name.ToLower()!= "tipogestionlotes"))
                    item.SetValue(result, viewmodel.get(item.Name));
            }
            */

            foreach (var item in result.GetType().GetProperties())
            {
                if ((obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false) &&
                    (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.GetGenericTypeDefinition() !=
                    typeof(ICollection<>)) && item.Name.ToLower() != "tipogestionlotes" && item.Name.ToLower() != "articulostercero" && item.Name.ToLower() != "articuloscomponentes" && item.Name.ToLower() != "articulosstockseguridad")
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
      
            result.tipogestionlotes = (int)viewmodel.Tipogestionlotes;
            result.ArticulosTercero.Clear();


            //Las claves ajenas se copian del result
            foreach (var item in viewmodel.ArticulosTercero)
            {
                var newitem = _db.Set<ArticulosTercero>().Create();
                newitem.empresa = result.empresa;
                newitem.id = item.Id;
                newitem.codarticulo = result.id;
                newitem.codtercero = item.CodTercero;
                newitem.descripciontercero = item.DescripcionTercero;
                newitem.codarticulotercero = item.CodArticuloTercero;
                newitem.descripcionarticulotercero = item.Descripcion;
                result.ArticulosTercero.Add(newitem);
            }

            result.ArticulosComponentes.Clear();
            foreach (var item in viewmodel.ArticulosComponentes)
            {
                var newitem = _db.Set<ArticulosComponentes>().Create();
                newitem.empresa = result.empresa;
                newitem.fkarticulo = item.FkArticulo;
                newitem.id = item.Id;
                newitem.idcomponente = item.IdComponente;
                newitem.descripcioncomponente = item.DescripcionComponente;
                newitem.piezas = item.Piezas;
                newitem.largo = item.Largo;
                newitem.ancho = item.Ancho;
                newitem.grueso = item.Grueso;
                newitem.merma = item.Merma;
                result.ArticulosComponentes.Add(newitem);
            }

            result.ArticulosStockSeguridad.Clear();
            foreach (var item in viewmodel.ArticulosStockSeguridad)
            {
                var newitem = _db.Set<ArticulosStockSeguridad>().Create();
                newitem.empresa = result.empresa;
                newitem.id = item.Id;
                newitem.codalmacen = item.Codalmacen;
                newitem.descripcionalmacen = item.Descripcionalmacen;
                newitem.codarticulo = result.id;
                newitem.stockseguridad = result.stockseguridad;
                newitem.stockminimo = item.Stockminimo;
                newitem.stockmaximo = item.Stockmaximo;

                result.ArticulosStockSeguridad.Add(newitem);
            }

            return result;
        }

        public override IModelView GetModelView(Articulos obj)
        {
            var result= base.GetModelView(obj) as ArticulosModel;
           
            result.Tipogestionlotes= obj.tipogestionlotes.HasValue ?  (Tipogestionlotes)obj.tipogestionlotes : Tipogestionlotes.Singestion;

            result.ArticulosTercero = obj.ArticulosTercero.ToList().Select(f => new ArticulosTerceroModel()
            {
                Id = f.id,
                CodArticulo = f.codarticulo,
                CodTercero = f.codtercero,
                DescripcionTercero = f.descripciontercero,
                CodArticuloTercero = f.codarticulotercero,
                Descripcion = f.descripcionarticulotercero
            }).ToList();

            result.ArticulosComponentes = obj.ArticulosComponentes.ToList().Select(f => new ArticulosComponentesModel()
            {
                Id = f.id,
                FkArticulo = f.fkarticulo,
                IdComponente = f.idcomponente,
                DescripcionComponente = f.descripcioncomponente,
                Piezas = f.piezas.Value,
                Largo = (float)f.largo,
                Ancho = (float)f.ancho,
                Grueso = (float)f.grueso,
                Merma = f.merma.Value
            }).ToList();

            result.ArticulosStockSeguridad = obj.ArticulosStockSeguridad.ToList().Select(f => new ArticulosStockSeguridadModel()
            {
                Id = f.id,
                Codalmacen = f.codalmacen,
                Descripcionalmacen = f.descripcionalmacen,
                Codarticulo = f.codarticulo,
                Stockseguridad = (TipoStockSeguridad)f.stockseguridad,
                Stockminimo = (double)f.stockminimo,
                Stockmaximo = (double)f.stockmaximo
            }).ToList();

            return result;
        }

        #endregion
    }
}
