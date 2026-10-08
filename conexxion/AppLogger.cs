using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Web;
namespace Conexxion
{
    public static class AppLogger
    {
        private static readonly object Sync = new object();
        public static void RegistrarError(string codigo, string operacion, Exception error)
        {
            Exception causa = error;
            while (causa.InnerException != null) causa = causa.InnerException;
            SqlException sql = causa as SqlException;
            string linea = DateTime.UtcNow.ToString("O") + " | " + codigo + " | " + operacion +
                " | " + causa.GetType().FullName + (sql == null ? "" : " | SQL:" + sql.Number) +
                " | " + (causa.StackTrace ?? "").Replace("\r", " ").Replace("\n", " ");
            try
            {
                string raiz = HttpRuntime.AppDomainAppPath ?? AppDomain.CurrentDomain.BaseDirectory;
                string dir = Path.Combine(raiz, "App_Data", "Logs");
                lock (Sync)
                {
                    Directory.CreateDirectory(dir);
                    File.AppendAllText(Path.Combine(dir, "errores-" + DateTime.UtcNow.ToString("yyyyMMdd") + ".log"), linea + Environment.NewLine);
                }
            }
            catch (IOException) { Trace.TraceError(linea); }
            catch (UnauthorizedAccessException) { Trace.TraceError(linea); }
        }
    }
}
