using Marfil.Dom.Persistencia.Model.Stock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public class ActualizarCostesService 
    {
        private readonly IContextService _context;
        private readonly MarfilEntities _db;

        public ActualizarCostesService(IContextService context)
        {
            _context = context;
            _db = MarfilEntities.ConnectToSqlServer(context.BaseDatos);
        }

        public void ActualizarCostes(ActualizarCostesModel model)
        {
            var cabAlbaranes = new List<AlbaranesCompras>();
            var linAlbaranes = new List<AlbaranesComprasLin>();
            var articuloant = "";
            var articulosservice = new ArticulosService(_context);
            var series = model.Series;
            var metros = 0d;
            var precio = 0d;
            var elaboracion = 0d;
            var portes = 0d;
            var otros = 0d;

            cabAlbaranes = ObtenerCabeceras(model, series);
            CalcularLineas(model, cabAlbaranes, linAlbaranes, ref articuloant, articulosservice, ref metros, ref precio, ref elaboracion, ref portes, ref otros);

        }

        private void CalcularLineas(ActualizarCostesModel model, List<AlbaranesCompras> cabAlbaranes, List<AlbaranesComprasLin> linAlbaranes, ref string articuloant, ArticulosService articulosservice, ref double metros, ref double precio, ref double elaboracion, ref double portes, ref double otros)
        {
            //Obtenemos las líneas de las cabeceras 
            foreach (var item in cabAlbaranes)
            {
                linAlbaranes.AddRange(_db.AlbaranesComprasLin.Where(f => f.fkalbaranes == item.id));
            }

            //Si tenemos filtros por articulo
            if (model.ArticulosDesde == null && model.ArticulosHasta == null)
            {
                //Recorremos las líneas sumando los metros y costes
                foreach (var item in linAlbaranes.OrderBy(s => s.fkarticulos))
                {
                    if (articuloant == "")// primera vuelta
                    {
                        articuloant = item.fkarticulos;
                        metros += (double)item.metros;
                        precio += (double)item.importe;
                        elaboracion += (double)(item.costeadicionalmaterial != null ? item.costeadicionalmaterial : 0d);
                        portes += (double)(item.costeadicionalportes != null ? item.costeadicionalportes : 0d);
                        otros += (double)(item.costeadicionalotro != null ? item.costeadicionalotro : 0d);
                    }
                    else if (articuloant == item.fkarticulos && articuloant != "")//sumamos los costes del articulo
                    {
                        metros += (double)item.metros;
                        precio += (double)item.importe;
                        elaboracion += (double)(item.costeadicionalmaterial != null ? item.costeadicionalmaterial : 0d);
                        portes += (double)(item.costeadicionalportes != null ? item.costeadicionalportes : 0d);
                        otros += (double)(item.costeadicionalotro != null ? item.costeadicionalotro : 0d);
                    }
                    else if (articuloant != item.fkarticulos && articuloant != "")//cambio de articulo
                    {
                        EditarArticulo(articuloant, articulosservice, metros, precio, elaboracion, portes, otros);

                        //limpiamos las variables
                        metros = 0d;
                        precio = 0d;
                        elaboracion = 0d;
                        portes = 0d;
                        otros = 0d;

                        //comenzamos el nuevo articulo
                        articuloant = item.fkarticulos;
                        metros += (double)item.metros;
                        precio += (double)item.importe;
                        elaboracion += (double)(item.costeadicionalmaterial != null ? item.costeadicionalmaterial : 0d);
                        portes += (double)(item.costeadicionalportes != null ? item.costeadicionalportes : 0d);
                        otros += (double)(item.costeadicionalotro != null ? item.costeadicionalotro : 0d);
                    }
                }

                //El último articulo se edita
                EditarArticulo(articuloant, articulosservice, metros, precio, elaboracion, portes, otros);
            }
            else if (model.ArticulosDesde != null && model.ArticulosHasta == null)
            {
                //Recorremos las líneas sumando los metros y costes
                foreach (var item in linAlbaranes.Where(f => Convert.ToInt32(f.fkarticulos) >= Convert.ToInt32(model.ArticulosDesde)).OrderBy(s => s.fkarticulos))
                {
                    if (articuloant == "")// primera vuelta
                    {
                        articuloant = item.fkarticulos;
                        metros += (double)item.metros;
                        precio += (double)item.importe;
                        elaboracion += (double)(item.costeadicionalmaterial != null ? item.costeadicionalmaterial : 0d);
                        portes += (double)(item.costeadicionalportes != null ? item.costeadicionalportes : 0d);
                        otros += (double)(item.costeadicionalotro != null ? item.costeadicionalotro : 0d);
                    }
                    else if (articuloant == item.fkarticulos && articuloant != "")//sumamos los costes del articulo
                    {
                        metros += (double)item.metros;
                        precio += (double)item.importe;
                        elaboracion += (double)(item.costeadicionalmaterial != null ? item.costeadicionalmaterial : 0d);
                        portes += (double)(item.costeadicionalportes != null ? item.costeadicionalportes : 0d);
                        otros += (double)(item.costeadicionalotro != null ? item.costeadicionalotro : 0d);
                    }
                    else if (articuloant != item.fkarticulos && articuloant != "")//cambio de articulo
                    {
                        EditarArticulo(articuloant, articulosservice, metros, precio, elaboracion, portes, otros);

                        //limpiamos las variables
                        metros = 0d;
                        precio = 0d;
                        elaboracion = 0d;
                        portes = 0d;
                        otros = 0d;

                        //comenzamos el nuevo articulo
                        articuloant = item.fkarticulos;
                        metros += (double)item.metros;
                        precio += (double)item.importe;
                        elaboracion += (double)(item.costeadicionalmaterial != null ? item.costeadicionalmaterial : 0d);
                        portes += (double)(item.costeadicionalportes != null ? item.costeadicionalportes : 0d);
                        otros += (double)(item.costeadicionalotro != null ? item.costeadicionalotro : 0d);
                    }
                }

                //El último articulo se edita
                EditarArticulo(articuloant, articulosservice, metros, precio, elaboracion, portes, otros);
            }
            else if (model.ArticulosDesde == null && model.ArticulosHasta != null)
            {
                //Recorremos las líneas sumando los metros y costes
                foreach (var item in linAlbaranes.Where(f => Convert.ToInt32(f.fkarticulos) <= Convert.ToInt32(model.ArticulosHasta)).OrderBy(s => s.fkarticulos))
                {
                    if (articuloant == "")// primera vuelta
                    {
                        articuloant = item.fkarticulos;
                        metros += (double)item.metros;
                        precio += (double)item.importe;
                        elaboracion += (double)(item.costeadicionalmaterial != null ? item.costeadicionalmaterial : 0d);
                        portes += (double)(item.costeadicionalportes != null ? item.costeadicionalportes : 0d);
                        otros += (double)(item.costeadicionalotro != null ? item.costeadicionalotro : 0d);
                    }
                    else if (articuloant == item.fkarticulos && articuloant != "")//sumamos los costes del articulo
                    {
                        metros += (double)item.metros;
                        precio += (double)item.importe;
                        elaboracion += (double)(item.costeadicionalmaterial != null ? item.costeadicionalmaterial : 0d);
                        portes += (double)(item.costeadicionalportes != null ? item.costeadicionalportes : 0d);
                        otros += (double)(item.costeadicionalotro != null ? item.costeadicionalotro : 0d);
                    }
                    else if (articuloant != item.fkarticulos && articuloant != "")//cambio de articulo
                    {
                        EditarArticulo(articuloant, articulosservice, metros, precio, elaboracion, portes, otros);

                        //limpiamos las variables
                        metros = 0d;
                        precio = 0d;
                        elaboracion = 0d;
                        portes = 0d;
                        otros = 0d;

                        //comenzamos el nuevo articulo
                        articuloant = item.fkarticulos;
                        metros += (double)item.metros;
                        precio += (double)item.importe;
                        elaboracion += (double)(item.costeadicionalmaterial != null ? item.costeadicionalmaterial : 0d);
                        portes += (double)(item.costeadicionalportes != null ? item.costeadicionalportes : 0d);
                        otros += (double)(item.costeadicionalotro != null ? item.costeadicionalotro : 0d);
                    }
                }

                //El último articulo se edita
                EditarArticulo(articuloant, articulosservice, metros, precio, elaboracion, portes, otros);
            }
            else
            {
                //Recorremos las líneas sumando los metros y costes
                foreach (var item in linAlbaranes.Where(f => Convert.ToInt64(f.fkarticulos) >= Convert.ToInt64(model.ArticulosDesde) && Convert.ToInt64(f.fkarticulos) <= Convert.ToInt64(model.ArticulosHasta)).OrderBy(s => s.fkarticulos))
                {
                    if (articuloant == "")// primera vuelta
                    {
                        articuloant = item.fkarticulos;
                        metros += (double)item.metros;
                        precio += (double)item.importe;
                        elaboracion += (double)(item.costeadicionalmaterial != null ? item.costeadicionalmaterial : 0d);
                        portes += (double)(item.costeadicionalportes != null ? item.costeadicionalportes : 0d);
                        otros += (double)(item.costeadicionalotro != null ? item.costeadicionalotro : 0d);
                    }
                    else if (articuloant == item.fkarticulos && articuloant != "")//sumamos los costes del articulo
                    {
                        metros += (double)item.metros;
                        precio += (double)item.importe;
                        elaboracion += (double)(item.costeadicionalmaterial != null ? item.costeadicionalmaterial : 0d);
                        portes += (double)(item.costeadicionalportes != null ? item.costeadicionalportes : 0d);
                        otros += (double)(item.costeadicionalotro != null ? item.costeadicionalotro : 0d);
                    }
                    else if (articuloant != item.fkarticulos && articuloant != "")//cambio de articulo
                    {
                        EditarArticulo(articuloant, articulosservice, metros, precio, elaboracion, portes, otros);

                        //limpiamos las variables
                        metros = 0d;
                        precio = 0d;
                        elaboracion = 0d;
                        portes = 0d;
                        otros = 0d;

                        //comenzamos el nuevo articulo
                        articuloant = item.fkarticulos;
                        metros += (double)item.metros;
                        precio += (double)item.importe;
                        elaboracion += (double)(item.costeadicionalmaterial != null ? item.costeadicionalmaterial : 0d);
                        portes += (double)(item.costeadicionalportes != null ? item.costeadicionalportes : 0d);
                        otros += (double)(item.costeadicionalotro != null ? item.costeadicionalotro : 0d);
                    }
                }

                //El último articulo se edita
                EditarArticulo(articuloant, articulosservice, metros, precio, elaboracion, portes, otros);
            }
        }

        private static void EditarArticulo(string articuloant, ArticulosService articulosservice, double metros, double precio, double elaboracion, double portes, double otros)
        {
            //asignamos los valores y editamos el anterior
            var articulo = articulosservice.GetArticulo(articuloant);
            articulo.Preciomateriaprima = Math.Round(precio / metros,2);
            articulo.Costeelaboracionmateriaprima = Math.Round(elaboracion / metros, 2);
            articulo.Costeportes = Math.Round(portes / metros, 2);
            articulo.Otroscostes = Math.Round(otros / metros, 2);
            articulo.Coste = articulo.Preciomateriaprima + articulo.Costeelaboracionmateriaprima + articulo.Costeportes + articulo.Otroscostes;
            articulosservice.edit(articulo);
        }

        private List<AlbaranesCompras> ObtenerCabeceras(ActualizarCostesModel model, List<string> series)
        {
            List<AlbaranesCompras> cabAlbaranes;
            //Obtenemos las cabeceras de los albaranes de compra correspondientes
            if (series.Count == 0)
            {
                if (model.FechaDesde == null && model.FechaHasta == null)
                {
                    cabAlbaranes = _db.AlbaranesCompras.Where(f => f.empresa == _context.Empresa).ToList();
                }
                else if (model.FechaDesde != null && model.FechaHasta == null)
                {
                    cabAlbaranes = _db.AlbaranesCompras.Where(f => f.empresa == _context.Empresa && f.fechadocumento >= model.FechaDesde).ToList();
                }
                else if (model.FechaDesde == null && model.FechaHasta != null)
                {
                    cabAlbaranes = _db.AlbaranesCompras.Where(f => f.empresa == _context.Empresa && f.fechadocumento <= model.FechaHasta).ToList();
                }
                else
                {
                    cabAlbaranes = _db.AlbaranesCompras.Where(f => f.empresa == _context.Empresa && f.fechadocumento >= model.FechaDesde && f.fechadocumento <= model.FechaHasta).ToList();
                }
            }
            else
            {
                if (model.FechaDesde == null && model.FechaHasta == null)
                {
                    cabAlbaranes = _db.AlbaranesCompras.Where(f => f.empresa == _context.Empresa && series.Contains(f.fkseries)).ToList();
                }
                else if (model.FechaDesde != null && model.FechaHasta == null)
                {
                    cabAlbaranes = _db.AlbaranesCompras.Where(f => f.empresa == _context.Empresa && f.fechadocumento >= model.FechaDesde && series.Contains(f.fkseries)).ToList();
                }
                else if (model.FechaDesde == null && model.FechaHasta != null)
                {
                    cabAlbaranes = _db.AlbaranesCompras.Where(f => f.empresa == _context.Empresa && f.fechadocumento <= model.FechaHasta && series.Contains(f.fkseries)).ToList();
                }
                else
                {
                    cabAlbaranes = _db.AlbaranesCompras.Where(f => f.empresa == _context.Empresa && f.fechadocumento >= model.FechaDesde && f.fechadocumento <= model.FechaHasta && series.Contains(f.fkseries)).ToList();
                }
            }

            return cabAlbaranes;
        }

        public void Dispose()
        {
            _db?.Dispose();
        }
    }
}
