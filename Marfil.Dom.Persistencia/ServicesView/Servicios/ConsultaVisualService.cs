using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Documentos.GrupoMateriales;
using Marfil.Dom.Persistencia.Model.Ficheros;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Stock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public class ConsultaVisualService : GestionService<AlmacenesModel, Almacenes>, IAlmacenesService
    {
        public ConsultaVisualService(IContextService context, MarfilEntities db = null) : base(context, db)
        {
        }

        //Devuelve los ficheros por fkcarpeta
        public List<FicheroGaleria> obtenerFicherosGaleria(string fkcarpetas)
        {
            if (String.IsNullOrEmpty(fkcarpetas))
            {
                return new List<FicheroGaleria>();
            }

            else
            {
                var ficherosService = FService.Instance.GetService(typeof(FicherosGaleriaModel), _context) as FicherosService;
                var carpetaService = FService.Instance.GetService(typeof(CarpetasModel), _context) as CarpetasService;
                var carpetaModel = carpetaService.get(fkcarpetas) as CarpetasModel;
                var listado = ficherosService.GetFicherosDeCarpetaId(carpetaModel.Id).Select(f => new FicheroGaleria(f, _context)).ToList();
                return listado;
            }
        }

        #region consulta visual

        //Almacenes BD
        public List<AlmacenesModel> obtenerAlmacenes(ConsultaVisualFullModel modelo)
        {
            var almacenesBD = _db.Almacenes.Where(f => f.empresa == modelo.Empresa && f.privado == false).ToList(); //Los privados no
            List<AlmacenesModel> almacenes = new List<AlmacenesModel>();

            if(almacenesBD.Any())
            {
                foreach (var almacen in almacenesBD)
                {
                    almacenes.Add(new AlmacenesModel
                    {
                        Empresa = _context.Empresa,
                        Id = almacen.id ?? "",
                        Descripcion = almacen.descripcion ?? "",
                        Coordenadas = almacen.coordenadas ?? "",
                        numFamilias = _db.Stockactual.Where(f => f.empresa == modelo.Empresa && f.fkalmacenes == almacen.id).
                                        Select(f => f.fkarticulos.Substring(0, 2)).Distinct().ToList().Count(),
                        Ficheros = obtenerFicherosGaleria(almacen.fkcarpetas.ToString() ?? "")
                    });
                }
            }
            
            return almacenes;
        }

        //Familias que tienen ese Id de almacen en Stock
        public List<FamiliasproductosModel> getFamilias(ConsultaVisualFullModel modelo)
        {
            //Familias que tienen el campo visible en la web a true
            var familias = _db.Familiasproductos.Where(f => f.empresa == modelo.Empresa && f.web == true).ToList();
            List<FamiliasproductosModel> familiasDevolver = new List<FamiliasproductosModel>();

            if(familias.Any())
            {
                foreach (var familia in familias)
                {
                    if (_db.Stockactual.Any(f => f.empresa == familia.empresa && f.fkalmacenes == modelo.idAlmacen && f.fkarticulos.Substring(0, 2) == familia.id))
                    {
                        familiasDevolver.Add(new FamiliasproductosModel
                        {
                            Empresa = modelo.Empresa,
                            Id = familia.id ?? "",
                            Descripcion = familia.descripcion ?? "",
                            Descripcionabreviada = familia.descripcionabreviada ?? "",
                            Ficheros = obtenerFicherosGaleria(familia.fkcarpetas.ToString() ?? "")
                        });
                    }
                }
            }      

            return familiasDevolver.Distinct().ToList();
        }

        //Grupos de materiales a partir del id de familia
        public List<GrupoMaterialesModel> getGruposMateriales(ConsultaVisualFullModel modelo)
        {
            List<GrupoMaterialesModel> grupos = new List<GrupoMaterialesModel>();

            //Todos los [distintos] id de materiales que hay en el stock actual //001,002,004,009, etc
            var materialesStock =
                _db.Stockactual.Where(f => f.empresa == modelo.Empresa && f.fkalmacenes == modelo.idAlmacen && f.fkarticulos.Substring(0, 2) == modelo.idFamilia).Select(f => f.fkarticulos.Substring(2, 3)).ToList().Distinct();

            if(materialesStock.Any())
            {
                //Obtenemos el grupo de material a partir del id del material
                foreach (var material in materialesStock)
                {
                    var MaterialAsociado = _db.Materiales.Where(f => f.empresa == modelo.Empresa && f.id == material).SingleOrDefault();
                    if (MaterialAsociado != null)
                    {
                        var GrupoMaterialAsociado = _db.GrupoMateriales.Where(f => f.empresa == modelo.Empresa && f.cod == MaterialAsociado.fkgruposmateriales).SingleOrDefault();

                        if (GrupoMaterialAsociado != null)
                        {
                            GrupoMaterialesModel grupo = new GrupoMaterialesModel();
                            grupo.Empresa = modelo.Empresa;
                            grupo.Cod = GrupoMaterialAsociado.cod ?? "";
                            grupo.Descripcion = GrupoMaterialAsociado.descripcion ?? "";
                            grupo.Ficheros = obtenerFicherosGaleria(GrupoMaterialAsociado.fkcarpetas.ToString() ?? "");
                            grupos.Add(grupo);
                        }
                    }
                }
            }

            return grupos.Distinct(new GrupoMaterialesComparerModel()).ToList();
        }

        //Materiales
        public List<MaterialesModel> getMateriales(ConsultaVisualFullModel modelo)
        {
            //Todos los materiales con fkgrupos materiales(3) y empresa(0)
            var materialesDB = _db.Materiales.Where(f => f.empresa == modelo.Empresa && f.fkgruposmateriales == modelo.idGrupoMateriales).ToList();
            List<MaterialesModel> materiales = new List<MaterialesModel>();

            if(materialesDB.Any())
            {
                foreach (var material in materialesDB)
                {
                    //Sacamos todos los articulos en Stock que hay con EMPRESA, ALMACEN, FAMILIA, MATERIAL
                    var posibleStock = _db.Stockactual.Where(f => f.empresa == modelo.Empresa && f.fkalmacenes == modelo.idAlmacen && f.fkarticulos.Substring(0, 2) == modelo.idFamilia && f.fkarticulos.Substring(2, 3) == material.id).ToList();
                    if (posibleStock.Any())
                    {
                        var metros = Math.Round(posibleStock.Where(f => f.metros != null).Select(f => f.metros.Value).Sum(),3); //La suma de los metros de las filas
                        var lotes = posibleStock.GroupBy(f => new { f.lote }).Count(); //Lotes agrupados
                        var piezas = posibleStock.Count(); //Cantidad de filas de stock

                        materiales.Add(new MaterialesModel
                        {
                            Empresa = material.empresa,
                            Id = material.id,
                            Descripcion = material.descripcion ?? "",
                            Descripcionabreviada = material.descripcionabreviada ?? "",
                            Ficheros = obtenerFicherosGaleria(material.fkcarpetas.ToString() ?? ""),
                            MetrosCV = metros,
                            LotesCV = lotes,
                            PiezasCV = piezas
                        });
                    }
                }
            }         

            return materiales;
        }

        //Ultima pantalla
        public List<ConsultaVisualModel> getMaterialEspecifico(ConsultaVisualFullModel modelo)
        {
            List<ConsultaVisualModel> productos = new List<ConsultaVisualModel>();

            //Sacamos todos los articulos en Stock que hay con EMPRESA, ALMACEN, FAMILIA, MATERIAL
            var posibleStock = _db.Stockactual.Where(f => f.empresa == modelo.Empresa && f.fkalmacenes == modelo.idAlmacen &&
            f.fkarticulos.Substring(0, 2) == modelo.idFamilia && f.fkarticulos.Substring(2, 3) == modelo.idMaterial).GroupBy(f => new { f.lote, f.fkarticulos, f.fkalmacenes, f.empresa }).ToList();

            if(posibleStock.Any())
            {
                foreach (var loteproducto in posibleStock)
                {
                    ConsultaVisualModel c = new ConsultaVisualModel();

                    c.Empresa = loteproducto.Select(f => f.empresa).First();
                    var idArticulo = loteproducto.Select(x => x.fkarticulos).First();
                    c.Descripcion = _db.Articulos.Where(f => f.empresa == modelo.Empresa && f.id == idArticulo).Select(f => f.descripcion).SingleOrDefault() ?? "";
                    c.DescripcionAbreviada = _db.Articulos.Where(f => f.empresa == modelo.Empresa && f.id == idArticulo).Select(f => f.descripcionabreviada).SingleOrDefault() ?? "";
                    c.Id = loteproducto.Select(f => f.fkarticulos).First();
                    c.MetrosCV = Math.Round(loteproducto.Where(f => f.metros != null).Select(f => f.metros.Value).Sum(),3);
                    c.DescLote = loteproducto.Select(f => f.lote).First();
                    c.PiezasCV = loteproducto.Count();
                    var lote = loteproducto.Select(f => f.lote).First().ToString();
                    var lotes = _db.Stockhistorico.Where(f => f.empresa == modelo.Empresa && f.fkarticulos.Substring(0, 2) == modelo.idFamilia && f.fkarticulos.Substring(2, 3) == modelo.idMaterial && f.lote == lote).ToList();
                    c.Ficheros = obtenerFicherosGaleria(lotes.Select(f=> f.fkcarpetas).First().ToString() ?? "");
                    productos.Add(c);
                }
            }

            return productos;
        }
    }

    #endregion
}
