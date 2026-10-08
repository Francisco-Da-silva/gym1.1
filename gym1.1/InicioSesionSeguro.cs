using System;
using System.Web;
using System.Web.Caching;
using Conexcion;
using Conexxion;

namespace gym1._1
{
    internal static class InicioSesionSeguro
    {
        private const string Cookie = "GymLoginPending";
        private sealed class Datos
        {
            public int IdUsuario, IdGimnasio;
            public string Nombre, Gimnasio, Email;
        }
        public static void Preparar(HttpContext contexto, UsuarioSesion usuario)
        {
            string token = TokenHelper.GenerarToken();
            var datos = new Datos { IdUsuario = usuario.IdUsuario, IdGimnasio = usuario.IdGimnasio,
                Nombre = usuario.Nombre, Gimnasio = usuario.NombreGimnasio, Email = usuario.Email };
            contexto.Cache.Insert("GymLogin:" + token, datos, null, DateTime.UtcNow.AddMinutes(1), Cache.NoSlidingExpiration);
            contexto.Session.Clear();
            contexto.Session.Abandon();
            contexto.Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", "") {
                Expires = DateTime.UtcNow.AddDays(-1), HttpOnly = true,
                Secure = contexto.Request.IsSecureConnection, SameSite = SameSiteMode.Lax, Path = "/" });
            contexto.Response.Cookies.Add(new HttpCookie(Cookie, token) {
                HttpOnly = true, Secure = contexto.Request.IsSecureConnection,
                SameSite = SameSiteMode.Strict, Path = "/", Expires = DateTime.UtcNow.AddMinutes(1) });
        }
        public static bool Completar(HttpContext contexto)
        {
            HttpCookie cookie = contexto.Request.Cookies[Cookie];
            if (cookie == null) return false;
            contexto.Response.Cookies.Add(new HttpCookie(Cookie, "") {
                Expires = DateTime.UtcNow.AddDays(-1), HttpOnly = true,
                Secure = contexto.Request.IsSecureConnection, SameSite = SameSiteMode.Strict, Path = "/" });
            if (string.IsNullOrEmpty(cookie.Value) || cookie.Value.Length != 43) return false;
            // Remove es atómico: una entrega solo puede consumirse una vez.
            Datos datos = contexto.Cache.Remove("GymLogin:" + cookie.Value) as Datos;
            if (datos == null) return false;
            string rol = UsuarioDAL.ObtenerRolActivo(datos.IdUsuario, datos.IdGimnasio);
            if (rol == null) return false;
            contexto.Session["IdUsuario"] = datos.IdUsuario;
            contexto.Session["IdGimnasio"] = datos.IdGimnasio;
            contexto.Session["NombreUsuario"] = datos.Nombre;
            contexto.Session["NombreGimnasio"] = datos.Gimnasio;
            contexto.Session["EmailUsuario"] = datos.Email;
            contexto.Session["Rol"] = rol;
            return true;
        }
    }
}
