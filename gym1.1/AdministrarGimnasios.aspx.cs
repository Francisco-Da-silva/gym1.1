using System;
using System.Data;
using System.Web.UI.WebControls;
using Conexcion;

namespace gym1._1
{
    public partial class AdministrarGimnasios :
          PaginaSuperAdministrador
    {

        protected void Page_Load(
         object sender,
         EventArgs e)
        {
            if (!IsPostBack)
            {
                OcultarMensaje();
                CargarGimnasios();
            }
        }
        //protected void Page_Load(
        //    object sender,
        //    EventArgs e)
        //{
        //    // ======================================
        //    // VALIDAR SESIÓN
        //    // ======================================

        //    if (Session["IdUsuario"] == null ||
        //        Session["Rol"] == null)
        //    {
        //        Response.Redirect(
        //            "~/Login.aspx",
        //            false
        //        );

        //        Context.ApplicationInstance
        //            .CompleteRequest();

        //        return;
        //    }


        //    // ======================================
        //    // SOLO SUPERADMINISTRADOR
        //    // ======================================

        //    string rol =
        //        Session["Rol"]
        //            .ToString()
        //            .Trim();


        //    if (!rol.Equals(
        //        "SuperAdministrador",
        //        StringComparison.OrdinalIgnoreCase))
        //    {
        //        Response.Redirect(
        //            "~/Default.aspx",
        //            false
        //        );

        //        Context.ApplicationInstance
        //            .CompleteRequest();

        //        return;
        //    }


        //    // ======================================
        //    // PRIMERA CARGA
        //    // ======================================

        //    if (!IsPostBack)
        //    {
        //        OcultarMensaje();

        //        CargarGimnasios();
        //    }
        //}


        // ==========================================
        // CARGAR GIMNASIOS
        // ==========================================

        //private void CargarGimnasios()
        //{
        //    try
        //    {
        //        DataTable dt =
        //            UsuarioDAL.ListarGimnasios();


        //        gvGimnasios.DataSource =
        //            dt;

        //        gvGimnasios.DataBind();
        //    }
        //    catch (Exception ex)
        //    {
        //        RedirigirAError(
        //            "Error al cargar los gimnasios.",
        //            ex
        //        );
        //    }
        //}


        private void CargarGimnasios()
        {
            try
            {
                DataTable dt =
                    UsuarioDAL.ListarGimnasios();

                gvGimnasios.DataSource = dt;
                gvGimnasios.DataBind();
            }
            catch (Exception ex)
            {
                string codigoError = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpperInvariant();
                Conexxion.AppLogger.RegistrarError(codigoError, "Cargar gimnasios", ex);
                MostrarMensaje("No se pudieron cargar los gimnasios. Código: " + codigoError, "danger");
            }
        }

        // ==========================================
        // CONFIGURAR FILAS
        // ==========================================

        protected void gvGimnasios_RowDataBound(
            object sender,
            GridViewRowEventArgs e)
        {
            if (e.Row.RowType !=
                DataControlRowType.DataRow)
            {
                return;
            }


            DataRowView fila =
                (DataRowView)e.Row.DataItem;


            bool activo =
                Convert.ToBoolean(
                    fila["Activo"]
                );


            Label lblEstado =
                (Label)e.Row.FindControl(
                    "lblEstado"
                );


            LinkButton btnCambiarEstado =
                (LinkButton)e.Row.FindControl(
                    "btnCambiarEstado"
                );


            if (activo)
            {
                // ==============================
                // ACTIVO
                // ==============================

                lblEstado.Text =
                    "ACTIVO";

                lblEstado.CssClass =
                    "badge bg-success";


                btnCambiarEstado.Text =
                    "Desactivar";

                btnCambiarEstado.CommandName =
                    "Desactivar";

                btnCambiarEstado.CssClass =
                    "btn btn-sm btn-outline-danger";


                btnCambiarEstado.OnClientClick =
                    "return confirm(" +
                    "'¿Seguro que querés desactivar este gimnasio? " +
                    "No podrá iniciar sesión hasta que vuelva a activarse.'" +
                    ");";
            }
            else
            {
                // ==============================
                // INACTIVO
                // ==============================

                lblEstado.Text =
                    "INACTIVO";

                lblEstado.CssClass =
                    "badge bg-secondary";


                btnCambiarEstado.Text =
                    "Activar";

                btnCambiarEstado.CommandName =
                    "Activar";

                btnCambiarEstado.CssClass =
                    "btn btn-sm btn-outline-success";


                btnCambiarEstado.OnClientClick =
                    "return confirm(" +
                    "'¿Querés volver a activar este gimnasio?'" +
                    ");";
            }
        }


        // ==========================================
        // ACTIVAR / DESACTIVAR
        // ==========================================

        protected void gvGimnasios_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Activar" &&
                e.CommandName != "Desactivar")
            {
                return;
            }


            if (!int.TryParse(
                e.CommandArgument.ToString(),
                out int idGimnasio))
            {
                MostrarMensaje(
                    "No se pudo identificar el gimnasio.",
                    "warning"
                );

                return;
            }


            try
            {
                bool actualizado;


                // ==================================
                // ACTIVAR
                // ==================================

                if (e.CommandName == "Activar")
                {
                    actualizado =
                        UsuarioDAL.ActivarGimnasio(
                            idGimnasio
                        );


                    if (actualizado)
                    {
                        MostrarMensaje(
                            "El gimnasio fue activado correctamente.",
                            "success"
                        );
                    }
                    else
                    {
                        MostrarMensaje(
                            "El gimnasio ya estaba activo o no existe.",
                            "warning"
                        );
                    }
                }


                // ==================================
                // DESACTIVAR
                // ==================================

                else
                {
                    actualizado =
                        UsuarioDAL.DesactivarGimnasio(
                            idGimnasio
                        );


                    if (actualizado)
                    {
                        MostrarMensaje(
                            "El gimnasio fue desactivado correctamente.",
                            "success"
                        );
                    }
                    else
                    {
                        MostrarMensaje(
                            "El gimnasio ya estaba inactivo o no existe.",
                            "warning"
                        );
                    }
                }


                // ==================================
                // ACTUALIZAR TABLA
                // ==================================

                CargarGimnasios();
            }
            catch (Exception ex)
            {
                RedirigirAError(
                    "Error al cambiar el estado del gimnasio.",
                    ex
                );
            }
        }


        // ==========================================
        // MENSAJES
        // ==========================================

        private void MostrarMensaje(
            string mensaje,
            string tipo)
        {
            pnlMensaje.Visible =
                true;

            pnlMensaje.CssClass =
                "alert alert-" + tipo;

            lblMensaje.Text =
                Server.HtmlEncode(mensaje);
        }


        private void OcultarMensaje()
        {
            pnlMensaje.Visible =
                false;

            lblMensaje.Text =
                string.Empty;
        }


        // ==========================================
        // ERROR GENERAL
        // ==========================================
        private void RedirigirAError(
    string descripcion,
    Exception ex)
        {
            string codigoError =
                Guid.NewGuid()
                    .ToString("N")
                    .Substring(0, 8)
                    .ToUpperInvariant();

            Conexxion.AppLogger.RegistrarError(codigoError, "AdministrarGimnasios.aspx", ex);

            Response.Redirect(
                "~/Error.aspx?codigo=" +
                Server.UrlEncode(codigoError),
                false
            );

            Context.ApplicationInstance
                .CompleteRequest();
        }
    }
}
//        private void RedirigirAError(
//            string descripcion,
//            Exception ex)
//        {
//            string codigoError =
//                Guid.NewGuid()
//                    .ToString("N")
//                    .Substring(0, 8)
//                    .ToUpperInvariant();


//            Conexxion.AppLogger.RegistrarError(codigoError, "AdministrarGimnasios.aspx", ex);


//            Response.Redirect(
//                "~/Error.aspx?codigo=" +
//                Server.UrlEncode(codigoError),
//                false
//            );


//            Context.ApplicationInstance
//                .CompleteRequest();
//        }
//    }
//}