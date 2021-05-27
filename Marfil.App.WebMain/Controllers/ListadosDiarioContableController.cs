﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Marfil.Dom.Persistencia.Listados;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.ControlsUI.Toolbar;
using Resources;
using Newtonsoft.Json;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Listados.Base;

namespace Marfil.App.WebMain.Controllers
{
    public class ListadosDiarioContableController : ListadosController<ListadosDiarioContable>
    {

        public ListadosDiarioContableController(IContextService context) : base(context)
        {           
        }

        //protected override IEnumerable<IToolbaritem> HazTuToolbar()
        //{
        //    var result = new List<IToolbaritem>();
        //    result.Add(new ToolbarSeparatorModel());
        //    result.Add(CreateComboImprimir());
        //    return result;
        //}


    }
}