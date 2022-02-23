﻿using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    class LogStockSeguridadConverterService : BaseConverterModel<LogStockSeguridadModel, LogStockSeguridad>
    {
        public LogStockSeguridadConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }
    }
}
