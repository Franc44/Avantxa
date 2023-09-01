using System;
using System.Net.Mail;

namespace Avantxa.Tools
{
    public class CorreosTemplates
    {
        public CorreosTemplates()
        {

        }

        public bool EnviarCorreo(string body, string correo, string filepath,int tipo)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.1and1.mx");

                mail.From = new MailAddress("contacto@avantxa.com.mx");
                mail.To.Add(correo);
                mail.Subject = "Avantxa";
                mail.Body = body;
                mail.IsBodyHtml = true;
                
                if(tipo == 0) mail.Attachments.Add(new Attachment(filepath, "application/pdf"));

                SmtpServer.Port = 587;
                SmtpServer.Host = "smtp.1and1.mx";
                SmtpServer.EnableSsl = true;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential("contacto@avantxa.com.mx", "CTOavantxa20.");

                SmtpServer.Send(mail);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public string CorreoNuevoUsuario(string UserName, string Contra)
        {
            return "<!DOCTYPE html><html>" +
                   "<head><meta http-equiv=\"Content-type\" content=\"text/html; charset=utf-8\" /><meta name=\"viewport\" content=\"width=device-width, initial-scale=1, maximum-scale=1\" /><meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\" /><meta name=\"format-detection\" content=\"date=no\" /><meta name=\"format-detection\" content=\"address=no\" /><meta name=\"format-detection\" content=\"telephone=no\" /><meta name=\"x-apple-disable-message-reformatting\" /><style type=\"text/css\" media=\"screen\">" +
                   "body { padding: 0 !important; margin: 0 !important; display: block !important; min-width: 100% !important; width: 100% !important; background: #ffffff; -webkit-text-size-adjust: none } a { color: #66c7ff; text-decoration: none } p { padding: 0 !important; margin: 0 !important } img { -ms-interpolation-mode: bicubic; } table { border-collapse: collapse; } #one td { border: 30px solid #FD0101; } .mcnPreviewText { display: none !important; } @media only screen and (max-device-width: 480px), only screen and (max-width: 480px) { .mobile-shell { width: 100% !important; min-width: 100% !important; } .bg { background-size: 100% auto !important; -webkit-background-size: 100% auto !important; } .text-header, .m-center { text-align: center !important; } .center { margin: 0 auto !important; } .container { padding: 20px 10px !important } .td { width: 100% !important; min-width: 100% !important; } .m-br-15 { height: 15px !important; } .p30-15 { padding: 30px 15px !important; } .m-td, .m-hide { display: none !important; width: 0 !important; height: 0 !important; font-size: 0 !important; line-height: 0 !important; min-height: 0 !important; } .m-block { display: block !important; } .fluid-img img { width: 100% !important; max-width: 100% !important; height: auto !important; } .column, .column-top, .column-empty, .column-empty2, .column-dir-top { float: left !important; width: 100% !important; display: block !important; } .column-empty { padding-bottom: 10px !important; } .column-empty2 { padding-bottom: 30px !important; } .content-spacing { width: 15px !important; } } </style> </head>" +
                   "<body class=\"body\" style=\"padding: 0!important; margin: 0!important; display: block!important; min - width:100 % !important; width: 100 % !important; background: #ffffff; -webkit-text-size-adjust:none;\">" +
                   "<table width=\"700 \" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" bgcolor=\"#ffffff\"> <tr> <td align=\"center\" valign=\"top\"> <table width=\"650\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"mobile-shell\"> <tr> <td class=\"td container\" style=\"width:650px; min-width:650px; font-size:0pt; line-height:0pt; margin:0; font-weight:normal; padding:55px 0px;\"> <table width=\"700\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"> <tr> <td class=\"p30-15\" style=\"padding: 0px 30px 30px 30px;\"> <table width=\"700\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"> <tr> <th class=\"column-top\" width=\"145\" style=\"font-size:0pt; line-height:0pt; padding:0; margin:0; font-weight:normal; vertical-align:top;\"> <table width=\"700\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"> <tr> <td class=\"img m-center\" style=\"font-size:0pt; line-height:0pt; text-align:left;\"><img src=\"https://www.avantxa.com.mx/img/logo.png\" width=\"600\" height=\"120\" border=\"0\" /></td> </tr> </table> </th> </tr> </table> </td> </tr></table>" +
                   "<table width=\"700 \" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"><tr><td style = \"padding-bottom: 10px;\"> <table width = \"700\" border = \"0\" cellspacing = \"0\" cellpadding = \"0\" ><tr> <td class=\"tbrr p30-15\" style=\"padding: 60px 30px; border-radius:26px 26px 0px 0px;\" bgcolor=\"#1c243e\"><table width = \"700\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"><tr>" +
                   "<td class=\"h1 pb25\" style=\"color:#ffffff; font-family:'Muli', Arial,sans-serif; font-size:40px; line-height:46px; text-align:center; padding-bottom:25px;\">Solicitud de factura</td></tr>" +
                   "<tr><td class=\"text-center pb25\" style=\"color:#c1cddc; font-family:'Muli', Arial,sans-serif; font-size:16px; line-height:30px; text-align:center; padding-bottom:25px;\">Usuario: " + UserName + " <span class=\"m-hide\"></span><br>Clave Temporal: " + Contra + "</td></tr></table></td></tr></table></td></tr></table>" +
                   "</td> </tr> </table> </td> </tr> </table> </body> </html>";
        }

        public string CorreoFactura(string Nombre, string RFC, string Correo, string CFDI, string Pago)
        {
            return "<!DOCTYPE html><html>" +
                   "<head><meta http-equiv=\"Content-type\" content=\"text/html; charset=utf-8\" /><meta name=\"viewport\" content=\"width=device-width, initial-scale=1, maximum-scale=1\" /><meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\" /><meta name=\"format-detection\" content=\"date=no\" /><meta name=\"format-detection\" content=\"address=no\" /><meta name=\"format-detection\" content=\"telephone=no\" /><meta name=\"x-apple-disable-message-reformatting\" /><style type=\"text/css\" media=\"screen\">" +
                   "body { padding: 0 !important; margin: 0 !important; display: block !important; min-width: 100% !important; width: 100% !important; background: #ffffff; -webkit-text-size-adjust: none } a { color: #66c7ff; text-decoration: none } p { padding: 0 !important; margin: 0 !important } img { -ms-interpolation-mode: bicubic; } table { border-collapse: collapse; } #one td { border: 30px solid #FD0101; } .mcnPreviewText { display: none !important; } @media only screen and (max-device-width: 480px), only screen and (max-width: 480px) { .mobile-shell { width: 100% !important; min-width: 100% !important; } .bg { background-size: 100% auto !important; -webkit-background-size: 100% auto !important; } .text-header, .m-center { text-align: center !important; } .center { margin: 0 auto !important; } .container { padding: 20px 10px !important } .td { width: 100% !important; min-width: 100% !important; } .m-br-15 { height: 15px !important; } .p30-15 { padding: 30px 15px !important; } .m-td, .m-hide { display: none !important; width: 0 !important; height: 0 !important; font-size: 0 !important; line-height: 0 !important; min-height: 0 !important; } .m-block { display: block !important; } .fluid-img img { width: 100% !important; max-width: 100% !important; height: auto !important; } .column, .column-top, .column-empty, .column-empty2, .column-dir-top { float: left !important; width: 100% !important; display: block !important; } .column-empty { padding-bottom: 10px !important; } .column-empty2 { padding-bottom: 30px !important; } .content-spacing { width: 15px !important; } } </style> </head>" +
                   "<body class=\"body\" style=\"padding: 0!important; margin: 0!important; display: block!important; min - width:100 % !important; width: 100 % !important; background: #ffffff; -webkit-text-size-adjust:none;\">" +
                   "<table width=\"700 \" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" bgcolor=\"#ffffff\"> <tr> <td align=\"center\" valign=\"top\"> <table width=\"650\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"mobile-shell\"> <tr> <td class=\"td container\" style=\"width:650px; min-width:650px; font-size:0pt; line-height:0pt; margin:0; font-weight:normal; padding:55px 0px;\"> <table width=\"700\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"> <tr> <td class=\"p30-15\" style=\"padding: 0px 30px 30px 30px;\"> <table width=\"700\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"> <tr> <th class=\"column-top\" width=\"145\" style=\"font-size:0pt; line-height:0pt; padding:0; margin:0; font-weight:normal; vertical-align:top;\"> <table width=\"700\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"> <tr> <td class=\"img m-center\" style=\"font-size:0pt; line-height:0pt; text-align:left;\"><img src=\"https://www.avantxa.com.mx/img/logo.png\" width=\"600\" height=\"120\" border=\"0\" /></td> </tr> </table> </th> </tr> </table> </td> </tr></table>" +
                   "<table width=\"700 \" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"><tr><td style = \"padding-bottom: 10px;\"> <table width = \"700\" border = \"0\" cellspacing = \"0\" cellpadding = \"0\" ><tr> <td class=\"tbrr p30-15\" style=\"padding: 60px 30px; border-radius:26px 26px 0px 0px;\" bgcolor=\"#1c243e\"><table width = \"700\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"><tr>" +
                   "<td class=\"h1 pb25\" style=\"color:#ffffff; font-family:'Muli', Arial,sans-serif; font-size:40px; line-height:46px; text-align:center; padding-bottom:25px;\">Solicitud de factura</td></tr>" +
                   "<tr><td class=\"text-center pb25\" style=\"color:#c1cddc; font-family:'Muli', Arial,sans-serif; font-size:16px; line-height:30px; text-align:center; padding-bottom:25px;\">Nombre: "+ Nombre + " <span class=\"m-hide\"></span><br>RFC: " + RFC + " <br>Correo electrónico: " + Correo + "<br> Uso del CFDI: " + CFDI +"<br> Método de pago: " + Pago + "</td></tr></table></td></tr></table></td></tr></table>" +
                   "</td> </tr> </table> </td> </tr> </table> </body> </html>";
        }

        public string CorreoMaterial(string Descripcion, string Material)
        {
            string[] result = Material.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            Material = String.Join("<br>-", result);

            return "<!DOCTYPE html><html>" +
                   "<head><meta http-equiv=\"Content-type\" content=\"text/html; charset=utf-8\" /><meta name=\"viewport\" content=\"width=device-width, initial-scale=1, maximum-scale=1\" /><meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\" /><meta name=\"format-detection\" content=\"date=no\" /><meta name=\"format-detection\" content=\"address=no\" /><meta name=\"format-detection\" content=\"telephone=no\" /><meta name=\"x-apple-disable-message-reformatting\" /><style type=\"text/css\" media=\"screen\">" +
                   "body { padding: 0 !important; margin: 0 !important; display: block !important; min-width: 100% !important; width: 100% !important; background: #ffffff; -webkit-text-size-adjust: none } a { color: #66c7ff; text-decoration: none } p { padding: 0 !important; margin: 0 !important } img { -ms-interpolation-mode: bicubic; } table { border-collapse: collapse; } #one td { border: 30px solid #FD0101; } .mcnPreviewText { display: none !important; } @media only screen and (max-device-width: 480px), only screen and (max-width: 480px) { .mobile-shell { width: 100% !important; min-width: 100% !important; } .bg { background-size: 100% auto !important; -webkit-background-size: 100% auto !important; } .text-header, .m-center { text-align: center !important; } .center { margin: 0 auto !important; } .container { padding: 20px 10px !important } .td { width: 100% !important; min-width: 100% !important; } .m-br-15 { height: 15px !important; } .p30-15 { padding: 30px 15px !important; } .m-td, .m-hide { display: none !important; width: 0 !important; height: 0 !important; font-size: 0 !important; line-height: 0 !important; min-height: 0 !important; } .m-block { display: block !important; } .fluid-img img { width: 100% !important; max-width: 100% !important; height: auto !important; } .column, .column-top, .column-empty, .column-empty2, .column-dir-top { float: left !important; width: 100% !important; display: block !important; } .column-empty { padding-bottom: 10px !important; } .column-empty2 { padding-bottom: 30px !important; } .content-spacing { width: 15px !important; } } </style> </head>" +
                   "<body class=\"body\" style=\"padding: 0!important; margin: 0!important; display: block!important; min - width:100 % !important; width: 100 % !important; background: #ffffff; -webkit-text-size-adjust:none;\">" +
                   "<table width=\"700 \" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" bgcolor=\"#ffffff\"> <tr> <td align=\"center\" valign=\"top\"> <table width=\"650\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"mobile-shell\"> <tr> <td class=\"td container\" style=\"width:650px; min-width:650px; font-size:0pt; line-height:0pt; margin:0; font-weight:normal; padding:55px 0px;\"> <table width=\"700\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"> <tr> <td class=\"p30-15\" style=\"padding: 0px 30px 30px 30px;\"> <table width=\"700\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"> <tr> <th class=\"column-top\" width=\"145\" style=\"font-size:0pt; line-height:0pt; padding:0; margin:0; font-weight:normal; vertical-align:top;\"> <table width=\"700\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"> <tr> <td class=\"img m-center\" style=\"font-size:0pt; line-height:0pt; text-align:left;\"><img src=\"https://www.avantxa.com.mx/img/logo.png\" width=\"600\" height=\"120\" border=\"0\" /></td> </tr> </table> </th> </tr> </table> </td> </tr></table>" +
                   "<table width=\"700 \" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"><tr><td style = \"padding-bottom: 10px;\"> <table width = \"700\" border = \"0\" cellspacing = \"0\" cellpadding = \"0\" ><tr> <td class=\"tbrr p30-15\" style=\"padding: 60px 30px; border-radius:26px 26px 0px 0px;\" bgcolor=\"#1c243e\"><table width = \"700\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"><tr>" +
                   "<td class=\"h1 pb25\" style=\"color:#ffffff; font-family:'Muli', Arial,sans-serif; font-size:40px; line-height:46px; text-align:center; padding-bottom:25px;\">Solicitud de material</td></tr>" +
                   "<tr><td class=\"text-center pb25\" style=\"color:#c1cddc; font-family:'Muli', Arial,sans-serif; font-size:16px; line-height:30px; text-align:center; padding-bottom:25px;\"> Se solicita la compra del siguiente material que será utilizado en el mantenimiento y/o reparación de la solicitud " + Descripcion + ": <span class=\"m-hide\"></span><br>-" + Material + "</td></tr></table></td></tr></table></td></tr></table>" +
                   "</td> </tr> </table> </td> </tr> </table> </body> </html>";
        }
    }
}
