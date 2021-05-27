using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RCostesVariablesPeriodo = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.CostesVariablesPeriodo;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System.Linq;

namespace Marfil.Dom.Persistencia.Model.CRM {

    public class CostesVariablesPeriodoModel: BaseModel<CostesVariablesPeriodoModel, CostesVariablesPeriodo> {

        public List<CostesVariablesPeriodoLinModel> _costes = new List<CostesVariablesPeriodoLinModel>();


        public CostesVariablesPeriodoModel()
        {

        }
        public CostesVariablesPeriodoModel(IContextService context) : base(context) {

        }
        

        #region propiedades

        [Required]
        public string Empresa { get; set; }

        [Key]
        [Required]
        [Display(ResourceType = typeof(RCostesVariablesPeriodo), Name = "Fkejercicio")]
        public int Fkejercicio { get; set; }

        [Display(ResourceType = typeof(RCostesVariablesPeriodo), Name = "Descripcion_ejercicio")]
        public string Descripcion_ejercicio { get; set; }

        #endregion

        #region implement base class
        public override string DisplayName => RCostesVariablesPeriodo.TituloEntidad;

        public override object generateId(string id)
        {

            return id.Split('-');
        }

        /*
        public override string GetPrimaryKey()
        {
            return this.Id.ToString();
        }
        */
        
        public override void createNewPrimaryKey()
        {
            primaryKey = new[] { GetType().GetProperty("Fkejercicio") };
        }
        #endregion
    }

    public class CostesVariablesPeriodoLinModel
    {

        #region propiedades

        [Required]
        public string Empresa { get; set; }

        [Required]
        [Display(ResourceType = typeof(RCostesVariablesPeriodo), Name = "Fkejercicio")]
        public int Fkejercicio { get; set; }

        [Key]
        [Required]
        [Display(ResourceType = typeof(RCostesVariablesPeriodo), Name = "Id")]
        public int Id { get; set; }

        //[Required]
        [Display(ResourceType = typeof(RCostesVariablesPeriodo), Name = "TablaVaria")]
        public string Tablavaria { get; set; }

        [Required]
        [Display(ResourceType = typeof(RCostesVariablesPeriodo), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Required]
        [Display(ResourceType = typeof(RCostesVariablesPeriodo), Name = "Precio")]
        public float Precio { get; set; }

        #endregion
    }
}