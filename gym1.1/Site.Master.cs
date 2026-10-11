using System;
using System.Web;
using System.Web.UI;

namespace gym1._1
{
    public partial class SiteMaster : MasterPage
    {
        // ==========================================
        // PAGE LOAD
        // ==========================================

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            // Evitamos que páginas protegidas
            // queden almacenadas en caché.
            Response.Cache.SetCacheability(
                HttpCacheability.NoCache
            );

            Response.Cache.SetNoStore();

            Response.Cache.SetExpires(
                DateTime.UtcNow.AddYears(-1)
            );

            Response.Cache.SetRevalidation(
                HttpCacheRevalidation.AllCaches
            );


            // Configuramos las opciones del menú
            // según el rol del usuario.
            ConfigurarMenuSuperAdministrador();
            ConfigurarPaginaActiva();
        }


        // ==========================================
        // CONFIGURAR MENÚ SUPERADMINISTRADOR
        // ==========================================

        private void ConfigurarMenuSuperAdministrador()
        {
            // Por defecto nunca mostramos
            // las herramientas de SuperAdmin.
            pnlSuperAdministrador.Visible = false;
            menuGimnasio.Visible = false;


            // Si no existe una sesión válida,
            // dejamos el panel oculto.
            if (Session["IdUsuario"] == null ||
                Session["Rol"] == null)
            {
                return;
            }


            string rol =
                Convert.ToString(
                    Session["Rol"]
                ).Trim();


            menuGimnasio.Visible = rol.Equals("Administrador", StringComparison.OrdinalIgnoreCase) ||
                rol.Equals("Gimnasio", StringComparison.OrdinalIgnoreCase);

            // Solo mostramos las opciones especiales
            // al SuperAdministrador.
            if (rol.Equals(
                "SuperAdministrador",
                StringComparison.OrdinalIgnoreCase))
            {
                pnlSuperAdministrador.Visible = true;
            }
        }


        protected string TipoCuenta
        {
            get { return string.Equals(Convert.ToString(Session["Rol"]).Trim(), "SuperAdministrador", StringComparison.OrdinalIgnoreCase) ? "Superadministrador" : "Administrador de gimnasio"; }
        }

        protected string NombreContexto
        {
            get { return TipoCuenta == "Superadministrador" ? "Administración de la plataforma" : Convert.ToString(Session["NombreGimnasio"]); }
        }

        private void ConfigurarPaginaActiva()
        {
            string pagina = System.IO.Path.GetFileNameWithoutExtension(Request.AppRelativeCurrentExecutionFilePath);
            if (string.IsNullOrEmpty(pagina)) pagina = "Default";
            var enlaces = new[] { navInicio, navPagos, navClientes, navRegistro, navDeudores, navGimnasios, navCrearGimnasio };
            foreach (var enlace in enlaces)
            {
                string destino = System.IO.Path.GetFileNameWithoutExtension(enlace.HRef);
                if (string.Equals(pagina, destino, StringComparison.OrdinalIgnoreCase))
                {
                    enlace.Attributes["class"] = "nav-link active";
                    enlace.Attributes["aria-current"] = "page";
                }
            }
        }

        // ==========================================
        // CERRAR SESIÓN
        // ==========================================

        protected void btnCerrarSesion_Click(
            object sender,
            EventArgs e)
        {
            // Eliminamos todos los datos
            // almacenados en la sesión.
            Session.Clear();

            Session.Abandon();


            // ======================================
            // ELIMINAR COOKIE DE SESIÓN
            // ======================================

            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                HttpCookie cookieSesion =
                    new HttpCookie(
                        "ASP.NET_SessionId",
                        string.Empty
                    );

                cookieSesion.Expires =
                    DateTime.Now.AddDays(-1);

                cookieSesion.HttpOnly =
                    true;

                Response.Cookies.Add(
                    cookieSesion
                );
            }


            // ======================================
            // ======================================
            // EVITAR CACHÉ
            // ======================================

            Response.Cache.SetCacheability(
                HttpCacheability.NoCache
            );

            Response.Cache.SetNoStore();

            Response.Cache.SetExpires(
                DateTime.UtcNow.AddYears(-1)
            );


            // ======================================
            // VOLVER AL LOGIN
            // ======================================

            Response.Redirect(
                "~/Login.aspx",
                false
            );

            Context.ApplicationInstance
                .CompleteRequest();
        }
    }
}