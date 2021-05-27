using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public enum TipoLicencia
    {
        Pyme,
        Profesional,
        Avanzado
    }

    public class LicenciasaplicacionService
    {
        private class ClientesModelView
        {
            public string Id { get; set; }

            public string Dominio { get; set; }

            public DateTime Fechaalta { get; set; }

            public DateTime Fechamodificacion { get; set; }

            public string Basedatos { get; set; }

            public TipoLicencia Tipolicencia { get; set; }

            public int Usuarios { get; set; }

            public bool Activado { get; set; }

            public string Azureblob { get; set; }
        }

        private readonly string _dominio;
        private readonly string _url;
        private readonly ClientesModelView _cliente;

        #region CTR

        public LicenciasaplicacionService(string dominio)
        {
            _dominio = dominio;
            _url = ConfigurationManager.AppSettings["UrlElefantWebApi"];
            _cliente= Cargarlicencia(dominio);

        }

        private ClientesModelView Cargarlicencia(string id)
        {
            var service = new HttpClient();
            service.BaseAddress = new Uri(_url);
            service.DefaultRequestHeaders.Clear();
            service.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            var response = service.GetAsync($"Api/Clientes?id=" + id);
            response.Wait();
            if (response.Result.IsSuccessStatusCode)
            {
                var resultadoCliente = response.Result.Content.ReadAsStringAsync();
                resultadoCliente.Wait();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<ClientesModelView>(resultadoCliente.Result);
            }
            return null;
        }

        #endregion

        #region Api

        public bool Activado
        {
            get { return _cliente?.Activado ?? false; } 
        }

        public int Usuarioslicencia
        {
            get { return _cliente?.Usuarios ?? 0; }
        }

        public TipoLicencia TipoLicencia
        {
            get { return _cliente?.Tipolicencia ?? TipoLicencia.Pyme; }
        }

        public string Basedatos
        {
            get { return _cliente?.Basedatos ?? string.Empty; }
        }

        public string Azureblob
        {
            get { return _cliente?.Azureblob ?? string.Empty; }
        }


        #endregion

    }
}
