using System;
using System.Diagnostics;
using System.Web;
using System.Web.Routing;


namespace gym1._1
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_Error(
            object sender,
            EventArgs e)
        {
            Exception exception =
                Server.GetLastError();

            if (exception == null)
            {
                return;
            }

            string codigoError =
                Guid.NewGuid()
                    .ToString("N")
                    .Substring(0, 8)
                    .ToUpper();

            // Mientras desarrollamos, el error aparece
            // en la ventana Output de Visual Studio.
            Conexxion.AppLogger.RegistrarError(codigoError, "Global.asax", exception);

            Server.ClearError();

            bool entradaInvalida = exception is HttpRequestValidationException ||
                exception.GetBaseException() is HttpRequestValidationException;
            string rutaError =
                "~/Error.aspx?codigo=" +
                HttpUtility.UrlEncode(codigoError) + (entradaInvalida ? "&entrada=invalida" : "");

            Response.Redirect(rutaError, false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
