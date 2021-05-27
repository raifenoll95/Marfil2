using System;
using System.Collections.Generic;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using RArticulos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Articulos;
using Marfil.Dom.Persistencia.Model.Ficheros;

namespace Marfil.Dom.Persistencia.Model.FicherosGenerales
{

    public class ConsultaVisualFullModel
    {
        #region ctr
        public ConsultaVisualFullModel()
        {

        }

        public ConsultaVisualFullModel(IContextService context)
        {

        }

        #endregion

        #region properties

        public string Empresa { get; set; }
        public string DescEmpresa { get; set; }
        public string idAlmacen { get; set; }
        public string DescAlmacen { get; set; }
        public string idFamilia { get; set; }
        public string DescFamilia { get; set; }
        public string idGrupoMateriales { get; set; }
        public string DescGrupoMateriales { get; set; }
        public string idMaterial { get; set; }
        public string DescMaterial { get; set; }
        #endregion

    }


    public class ConsultaVisualModel
    {

        #region CTR

        public ConsultaVisualModel()
        {

        }

        public ConsultaVisualModel(IContextService context)
        {

        }

        #endregion

        #region Properties

        public String Empresa { get; set; }

        public string Descripcion { get; set; }

        public string DescripcionAbreviada { get; set; }

        public string Id { get; set; }

        public double MetrosCV { get; set; }

        public double LotesCV { get; set; }

        public String DescLote { get; set; }

        public double PiezasCV { get; set; }

        private List<FicheroGaleria> _ficheros = new List<FicheroGaleria>();
        public List<FicheroGaleria> Ficheros
        {
            get { return _ficheros; }
            set { _ficheros = value; }
        }
        #endregion


    }

    public class ConsultaVisualLinModel
    {

        #region CTR

        public ConsultaVisualLinModel()
        {

        }

        public ConsultaVisualLinModel(IContextService context)
        {

        }

        #endregion

        public string DescripcionArticulo { get; set; }

        public string IdArticulo { get; set; }

        public int Piezas { get; set; }

        public int Metros { get; set; }

        public int Cantidad { get; set; }

        public string Lote { get; set; }

        public int numLotes { get; set; }

        private List<FicheroGaleria> _ficheros = new List<FicheroGaleria>();
        public List<FicheroGaleria> Ficheros
        {
            get { return _ficheros; }
            set { _ficheros = value; }
        }
    }
}

