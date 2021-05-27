using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.CRM;
using System.Runtime.Remoting.Contexts;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Mailing
{
    public struct StMensajeriaAdjuntos
    {
        public byte[] Adjunto { get; set; }
        public string Nombre { get; set; }
    }

    public class EmailsService 
    {

        public IContextService context2;

        public EmailsService(IContextService context, MarfilEntities db = null)
        {
            context2 = context;
        }

        #region Api

        public void EnviarEmail(string destino, string bcc, string nombre, string remitente,
            string usuario, string pass, string smtp, int port, string asunto, MailMessage mensaje, bool html,
            string[] ficheros, List<StMensajeriaAdjuntos> adjuntos, bool ssl,TipoCopiaRemitente? copia, ref string mierror, int id, int tipo)
        {

            EnviarEmail(mensaje,destino,bcc,nombre,remitente,usuario,pass,smtp,port,asunto,string.Empty,html,ficheros,adjuntos,ssl, copia,ref mierror, id, tipo);
        }

        public void EnviarEmail(string destino, string bcc, string nombre, string remitente,
            string usuario, string pass, string smtp, int port, string asunto, string cuerpo, bool html,
            string[] ficheros, List<StMensajeriaAdjuntos> adjuntos, bool ssl, TipoCopiaRemitente? copia, ref string mierror, int id, int tipo)
        {
            EnviarEmail(null, destino, bcc, nombre, remitente, usuario, pass, smtp, port, asunto, cuerpo, html, ficheros, adjuntos, ssl, copia, ref mierror, id, tipo);
        }

        #endregion

        #region helpers

        private void EnviarEmail(MailMessage mensaje, string destino, string bcc, string nombre, string remitente, string usuario, string pass, string smtp, int port, string asunto, string cuerpo, bool html, string[] ficheros, List<StMensajeriaAdjuntos> aduntos, bool ssl, TipoCopiaRemitente? copia, ref string mierror, int id, int tipo)
        {

            try
            {

                var email = new MailMessage();
                if (mensaje != null) email = mensaje;
                else email.Body = cuerpo;

                email.From = !nombre.Equals("") ? new MailAddress(remitente, nombre) : new MailAddress(remitente);

                email.IsBodyHtml = html;
                destino = destino.Replace(" ", "");
                var vectorTo = destino.Split(';');

                List<string> destinatarios = new List<string>();

                foreach (string item in vectorTo.Where(item => !item.Equals("")))
                {
                    email.To.Add(item);
                    destinatarios.Add(item);
                }

                email.SubjectEncoding = Encoding.UTF8;
                email.Subject = asunto;

                if (copia.HasValue)
                {
                    if (copia == TipoCopiaRemitente.Cc)
                    {
                        email.CC.Add(remitente);
                    }
                    else if (copia == TipoCopiaRemitente.Bcc)
                    {
                        email.Bcc.Add(remitente);
                    }
                }

                if (!string.IsNullOrEmpty(bcc))
                {
                    string[] vector = bcc.Split(';');
                    foreach (string item in vector)
                    {
                        if (!item.Equals(""))
                            email.Bcc.Add(item);
                    }
                }

                var cred = new System.Net.NetworkCredential(usuario, pass);
                
                var cliente = new SmtpClient(smtp, port)
                {
                    
                    Credentials = cred,
                    EnableSsl = ssl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                };
                //ficheros adjuntos                
                if (ficheros != null)
                {
                    foreach (var item in ficheros.Select(t => new Attachment(t)))
                    {
                        email.Attachments.Add(item);
                    }
                }
                if (aduntos != null)
                {
                    foreach (var item in aduntos.Where(f=>f.Adjunto!=null && f.Adjunto.Length>0).Select(st => new Attachment(new MemoryStream(st.Adjunto), st.Nombre)))
                    {
                        email.Attachments.Add(item);
                    }
                }
                cliente.Timeout = 500000;
                cliente.Send(email);

                //Seguimientos
                if(tipo == 0)
                {
                    //Aqui creo que ya se ha enviado.Las lineas de correos de seguimiento deben rellenarse
                    //SeguimientosCorreoModel correo = new SeguimientosCorreoModel();
                    //String destinatariosString = "";
                    //string last = destinatarios.Last();

                    //foreach (var des in destinatarios)
                    //{
                    //    if (des == last)
                    //    {
                    //        destinatariosString = destinatariosString + des;
                    //    }

                    //    else
                    //    {
                    //        destinatariosString = destinatariosString + des + ",";
                    //    }
                    //}

                    //correo.Empresa = context2.Empresa;
                    //correo.Fkorigen = fkorigen;
                    //correo.Fkseguimientos = fkseguimientos;
                    //correo.Correo = destinatariosString;
                    //correo.Asunto = asunto;
                    //correo.Fecha = DateTime.Now;

                    //var seguimientosService = new SeguimientosCorreoService(context2);
                    //seguimientosService.createLineasCorreos(correo);
                }
            }
            catch (Exception ex)
            {
                mierror = ex.Message;
            }
        }

        #endregion
    }
}
