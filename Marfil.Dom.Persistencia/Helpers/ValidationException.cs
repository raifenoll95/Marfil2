using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.Helpers
{
    public class UsuarioensuoException : Exception
    {
        public UsuarioensuoException(string message) : base(message)
        {

        }

        public UsuarioensuoException(Exception ex) : base(ex.Message, ex)
        {

        }

    }
    public class UsuarioactivoException : Exception
    {
        public UsuarioactivoException(string message) : base(message)
        {

        }

        public UsuarioactivoException(Exception ex):base(ex.Message,ex)
        {

        }

    }
    public class LicenciaException : Exception
    {
        public LicenciaException(string message) : base(message)
        {

        }

        public LicenciaException(Exception ex):base(ex.Message,ex)
        {

        }
    }

    public class CambiarEmpresaException : Exception
    {
        public CambiarEmpresaException(string message) : base(message)
        {

        }

        public CambiarEmpresaException(Exception ex):base(ex.Message,ex)
        {

        }
    }
    public class ValidationException:Exception
    {
        public ValidationException(string message) : base(message)
        {
            
        }

        public ValidationException(Exception ex):base(ex.Message,ex)
        {
            
        }
    }

    public class IntegridadReferencialException : Exception
    {
        public IntegridadReferencialException(string message) : base(message)
        {

        }

        public IntegridadReferencialException(Exception ex):base(ex.Message,ex)
        {

        }
    }
}
