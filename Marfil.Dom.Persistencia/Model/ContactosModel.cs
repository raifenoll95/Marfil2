using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Interfaces;
using System.ComponentModel.DataAnnotations;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using RContactos= Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Contactos;
namespace Marfil.Dom.Persistencia.Model
{
    public class ContactosModel
    {
        public string Empresa { get; set; }
        public int Tipotercero { get; set; }
        public Guid Id { get; set; }
        public IEnumerable<ContactosLinModel> Contactos { get; set; }
        public bool ReadOnly { get; set; }
        public IEnumerable<TablasVariasCargosEmpresaModel> CargosEmpresa { get; set; }
        public DireccionesModel Direcciones { get; set; }
    }

    public class ContactosLinModel : BaseModel<ContactosLinModel, Contactos>
    {
        #region Relaciones

        public IEnumerable<TablasVariasGeneralModel> TiposContactoList { get; set; }
        public IEnumerable<TablasVariasCargosEmpresaModel> CargosEmpresaList { get; set; }
        public IEnumerable<TablasVariasIdiomasAplicacion> IdiomasList { get; set; }

        #endregion

        #region Properties

        public string IdView
        {
            get
            {
                return string.Format("{0}{1}{2}{1}{3}", (int)Tipotercero, SeparatorPk, Fkentidad, Id);
            }
        }

        public static string CreateId(int tipotercero, string entidad, int id)
        {
            return string.Format("{0}{1}{2}{1}{3}", (int)tipotercero, SeparatorPk, entidad, id);
        }

        public string Empresa { get; set; }
        
        public TiposCuentas Tipotercero { get; set; }

        public string Fkentidad { get; set; }

        public int Id { get; set; }

        [Required]
        [Display(ResourceType = typeof(RContactos), Name = "Nombre")]
        public string Nombre { get; set; }


        [Display(ResourceType = typeof(RContactos), Name = "Fktipocontacto")]
        public string Fktipocontacto { get; set; }

        public string TipoContacto { get; set; }


        [Display(ResourceType = typeof(RContactos), Name = "Fkcargoempresa")]
        public string Fkcargoempresa { get; set; }
        public string CargoEmpresa { get; set; }


        [Display(ResourceType = typeof(RContactos), Name = "Fkidioma")]
        public string Fkidioma { get; set; }
        public string Idioma { get; set; }

        [Display(ResourceType = typeof(RContactos), Name = "Telefono")]
        public string Telefono { get; set; }

        [Display(ResourceType = typeof(RContactos), Name = "Telefonomovil")]
        public string Telefonomovil { get; set; }

        [Display(ResourceType = typeof(RContactos), Name = "Fax")]
        public string Fax { get; set; }

        [Display(ResourceType = typeof(RContactos), Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(ResourceType = typeof(RContactos), Name = "Nifcif")]
        public string Nifcif { get; set; }

        [Display(ResourceType = typeof(RContactos), Name = "Observaciones")]
        public string Observaciones { get; set; }

        [Display(ResourceType = typeof(RContactos), Name = "Fkid_direccion")]
        public int? Fkid_direccion { get; set; } 

        public string Direccion { get; set; }

        #endregion

        #region CTR

        public ContactosLinModel()
        {

        }

        public ContactosLinModel(IContextService context) : base(context)
        {

        }

        #endregion

        public override string DisplayName => "Contactos";

        public const char SeparatorPk = ';';

        public override object generateId(string id)
        {
            return id;
        }
    }
}
