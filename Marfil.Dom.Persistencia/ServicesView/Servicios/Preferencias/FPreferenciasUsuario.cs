using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Inf.Genericos;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Preferencias
{
    public class FPreferenciasUsuario
    {
        public static object GetPreferencia(TiposPreferencias tipopreferencia, string xml)
        {
            dynamic item = null;
            switch (tipopreferencia)
            {
                case TiposPreferencias.EmpresaDefecto:
                    item = new Serializer<PreferenciaEmpresaDefecto>();
                    break;
                case TiposPreferencias.ConfiguracionListado:
                    item = new Serializer<PreferenciaConfiguracionListado>();
                    break;

                case TiposPreferencias.DocumentoImpresionDefecto:
                    item = new Serializer<PreferenciaDocumentoImpresionDefecto>();
                    break;
                case TiposPreferencias.EjercicioDefecto:
                    item = new Serializer<PreferenciaEjercicioDefecto>();
                    break;
                case TiposPreferencias.AlmacenDefecto:
                    item = new Serializer<PreferenciaAlmacenDefecto>();
                    break;
                case TiposPreferencias.PanelControlDefecto:
                    item = new Serializer<PreferenciaPanelControlDefecto>();
                    break;
            }

            if (item != null)
            {
                return item.SetXml(xml);
            }
            return null;
        }

        public static string GetXmlPreferencia(TiposPreferencias tipopreferencia, object xml)
        {
            dynamic item = null;
            switch (tipopreferencia)
            {
                case TiposPreferencias.EmpresaDefecto:
                    item = new Serializer<PreferenciaEmpresaDefecto>();
                    return item.GetXml(xml as PreferenciaEmpresaDefecto);

                case TiposPreferencias.ConfiguracionListado:
                    item = new Serializer<PreferenciaConfiguracionListado>();
                    return item.GetXml(xml as PreferenciaConfiguracionListado);

                case TiposPreferencias.DocumentoImpresionDefecto:
                    item = new Serializer<PreferenciaDocumentoImpresionDefecto>();
                    return item.GetXml(xml as PreferenciaDocumentoImpresionDefecto);

                case TiposPreferencias.EjercicioDefecto:
                    item = new Serializer<PreferenciaEjercicioDefecto>();
                    return item.GetXml(xml as PreferenciaEjercicioDefecto);
                case TiposPreferencias.AlmacenDefecto:
                    item = new Serializer<PreferenciaAlmacenDefecto>();
                    return item.GetXml(xml as PreferenciaAlmacenDefecto);
                case TiposPreferencias.PanelControlDefecto:
                    item = new Serializer<PreferenciaPanelControlDefecto>();
                    return item.GetXml(xml as PreferenciaPanelControlDefecto);

            }

            return string.Empty;
        }
    }
}
