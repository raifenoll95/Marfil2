using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal abstract class BaseValidation<T>: IValidationService<T>
    {
        #region members

        protected  MarfilEntities _db;
        private List<string> _warningList;
        protected ApplicationHelper _appService;
        #endregion

        #region Properties

        public List<string> WarningList
        {
            get { return _warningList; }
            set { _warningList = value; }
        } 

        public IContextService Context { get; set; }

        #endregion

        #region CTR

        public BaseValidation(IContextService context,MarfilEntities db)
        {
            _db = db;
            Context = context;
            _warningList = new List<string>();

            _appService=new ApplicationHelper(context);
        }

        #endregion

        #region API

        public MarfilEntities Db
        {
            get { return _db; }
            set { _db = value; }
        }

        public virtual bool ValidarGrabar(T model)
        {
            return true;
        }

        public virtual bool ValidarBorrar(T model)
        {
            return true;
        }

        #endregion
    }
}
