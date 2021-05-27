using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using RFicheros = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Ficheros;
namespace Marfil.Dom.Persistencia.Model.Ficheros
{
    public class FicheroGaleria
    {
        private readonly IContextService _context;

        public string Id { get; private set; }
        public string Nombre { get; private set; }
        public string Descripcion { get; private set; }
        public string Ruta { get; private set; }
        public string Tipo { get; private set; }
        public string Fkcarpetas { get; private set; }
        public bool Esimagen { get; private set; }
        public string Imagenauxiliar { get; private set; }

        public FicheroGaleria(FicherosGaleriaModel m,IContextService context)
        {
            var vector = new [] {".gif",".jpg",".png",".bmp"};
            var vectorExcel = new [] {".xlsx",".xlsm",".xltx",".xltm",".csv"};
            var vectorWord = new [] { ".doc", ".docx" };
            var vectorPdf = new [] { ".pdf" };
            _context = context;
            Id = m.Id.ToString();
            Nombre = m.Nombre;
            Descripcion = m.Descripcion;
            Ruta = m.Ruta;
            Tipo = m.Tipo;
            Fkcarpetas = m.Fkcarpetas.ToString();
            Esimagen = vector.Contains(m.Tipo.ToLower());
            if (!Esimagen)
            {
                var urlhelper = _context.GetUrlHelper();
                if (vectorPdf.Contains(m.Tipo.ToLower()))
                    Imagenauxiliar = urlhelper.Content("~/Images/imagenesauxiliares/iconopdf.png");
                else if (vectorWord.Contains(m.Tipo.ToLower()))
                    Imagenauxiliar = urlhelper.Content("~/Images/imagenesauxiliares/iconoword.png");
                else if (vectorExcel.Contains(m.Tipo.ToLower()))
                    Imagenauxiliar = urlhelper.Content("~/Images/imagenesauxiliares/iconoexcel.png");
                else 
                    Imagenauxiliar = urlhelper.Content("~/Images/imagenesauxiliares/iconogenerico.png");
            }

        }
    }

    public class FicherosGaleriaModel : BaseModel<FicherosGaleriaModel, Persistencia.Ficheros>
    {
        #region Properties

       

        public string Empresa { get; set; }

        [Display(ResourceType = typeof(RFicheros), Name = "Id")]
        public Guid Id { get; set; }

        [Display(ResourceType = typeof(RFicheros), Name = "Nombre")]
        public string Nombre { get; set; }

        [Display(ResourceType = typeof(RFicheros), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(RFicheros), Name = "Ruta")]
        public string Ruta { get; set; }

        [Display(ResourceType = typeof(RFicheros), Name = "Tipo")]
        public string Tipo { get; set; }
        
        public Guid Fkcarpetas { get; set; }

        #endregion

        #region CTR

        public FicherosGaleriaModel()
        {

        }

        public FicherosGaleriaModel(IContextService context) : base(context)
        {

        }

        #endregion

        public override object generateId(string id)
        {
            return new Guid(id);
        }

        public override string DisplayName => RFicheros.TituloEntidad;
    }
}
