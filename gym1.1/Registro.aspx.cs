using Conexxion;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;




namespace gym1._1
{
    public partial class Registro : PaginaProtegida
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarClientes();
            }
        }

        private void MostrarMensaje(
    string mensaje,
    string tipoBootstrap)
        {
            pnlMensaje.Visible = true;

            pnlMensaje.CssClass =
                "alert alert-" +
                tipoBootstrap +
                " mb-4";

            lblMensaje.Text =
                Server.HtmlEncode(mensaje);
        }

        private void OcultarMensaje()

        {
            pnlMensaje.Visible = false;
            lblMensaje.Text = string.Empty;
        }


        private void LimpiarFormulario()
        {
            txtNombre.Text = string.Empty;
            txtApellido.Text = string.Empty;
            txtDni.Text = string.Empty;
            txtTelefono.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtFechaNac.Text = string.Empty;

            if (Planpago.Items.Count > 0)
            {
                Planpago.SelectedIndex = 0;
            }
        }


        protected void btnAgregar_Click(object sender, EventArgs e)
        {
            OcultarMensaje();

            try
            {
                if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                    string.IsNullOrWhiteSpace(txtApellido.Text) ||
                    string.IsNullOrWhiteSpace(txtDni.Text) ||
                    string.IsNullOrWhiteSpace(txtFechaNac.Text))

                {
                    MostrarMensaje(
                        "Completá nombre, apellido, DNI y fecha de nacimiento.",
                        "warning"
                    );

                    return;
                }

                if (!DateTime.TryParse(
                    txtFechaNac.Text,
                    out DateTime fechaNacimiento))
                {
                    MostrarMensaje(
                        "La fecha de nacimiento no es válida.",
                        "warning"
                    );

                    txtFechaNac.Focus();
                    return;
                }


                string telefono = txtTelefono.Text.Trim();

                if (telefono.Length > 20)
                {
                    MostrarMensaje(
                        "El teléfono no puede superar los 20 caracteres.",
                        "warning"
                    );
                    return;
                }
                // NO PERMITIR FECHA FUTURA
                if (fechaNacimiento.Date > DateTime.Today)
                {
                    MostrarMensaje(
                        "La fecha de nacimiento no puede ser futura.",
                        "warning"
                    );

                    return;
                }

                // NO PERMITIR FECHAS ABSURDAMENTE ANTIGUAS
                if (fechaNacimiento.Year < 1900)
                {
                    MostrarMensaje(
                        "La fecha de nacimiento ingresada no es válida.",
                        "warning"
                    );

                    return;
                }

                //validacion para los dni 
                string dni = txtDni.Text.Trim();

                if (string.IsNullOrWhiteSpace(dni))
                {
                    MostrarMensaje(
                        "Ingresá el DNI del cliente.",
                        "warning"
                    );

                    txtDni.Focus();
                    return;
                }

                if (!dni.All(char.IsDigit))
                {
                    MostrarMensaje(
                        "El DNI solo puede contener números.",
                        "warning"
                    );

                    txtDni.Focus();
                    return;
                }

                if (dni.Length < 7 || dni.Length > 8)
                {
                    MostrarMensaje(
                        "El DNI debe tener entre 7 y 8 dígitos.",
                        "warning"
                    );

                    txtDni.Focus();
                    return;
                }
                string error = ValidacionDatos.ErrorCliente(txtNombre.Text.Trim(), txtApellido.Text.Trim(),
                    dni, telefono, txtEmail.Text.Trim(), fechaNacimiento, Planpago.SelectedValue);
                if (error != null)
                {
                    MostrarMensaje(error, "warning");
                    return;
                }
                SP.AgregarCliente(
                    IdGimnasioActual,
                    txtNombre.Text.Trim(),
                    txtApellido.Text.Trim(),
                    dni,
                    txtTelefono.Text.Trim(),
                    txtEmail.Text.Trim(),
                    fechaNacimiento,
                    Planpago.SelectedValue
                );

                CargarClientes();
                LimpiarFormulario();

                MostrarMensaje(
                    "Cliente registrado correctamente.",
                    "success"
                );
            }
            catch (SqlException ex)
            {
                if (ex.Number == 50001 || ex.Number == 2601 || ex.Number == 2627)
                {
                    MostrarMensaje(
                        "Ya existe un cliente con ese DNI en este gimnasio.",
                        "warning"
                    );

                    txtDni.Focus();
                    return;
                }

                if (ex.Number == 50002)
                {
                    MostrarMensaje(
                        "El gimnasio no existe o se encuentra inactivo.",
                        "danger"
                    );

                    return;
                }


                if (ex.Number == 50003)
                {
                    MostrarMensaje(
                        "El DNI solo puede contener números.",
                        "warning"
                    );

                    txtDni.Focus();
                    return;
                }

                if (ex.Number == 50004)
                {
                    MostrarMensaje(
                        "El DNI debe tener entre 7 y 8 dígitos.",
                        "warning"
                    );

                    txtDni.Focus();
                    return;
                }
                if (ex.Number == 50005)
                {
                    MostrarMensaje("Revisá los datos del cliente ingresados.", "warning");
                    return;
                }
                string codigoError =
                    Guid.NewGuid()
                        .ToString("N")
                        .Substring(0, 8)
                        .ToUpperInvariant();

                Conexxion.AppLogger.RegistrarError(codigoError, "Registro.aspx", ex);

                MostrarMensaje(
                    "No se pudo registrar el cliente. Código: " + codigoError,
                    "danger"
                );
            }
            catch (Exception ex)
            {
                string codigoError =
                    Guid.NewGuid()
                        .ToString("N")
                        .Substring(0, 8)
                        .ToUpperInvariant();

                Conexxion.AppLogger.RegistrarError(codigoError, "Registro.aspx", ex);

                MostrarMensaje(
                    "Ocurrió un error inesperado. Código: " + codigoError,
                    "danger"
                );
            }
        }
        private void CargarClientes()
        {
            gvClientes.DataSource =
                SP.ListarClientes(IdGimnasioActual);

            gvClientes.DataBind();
        }

        protected void gvClientes_RowDeleting(object sender, System.Web.UI.WebControls.GridViewDeleteEventArgs e)
        {
            int id = Convert.ToInt32(gvClientes.DataKeys[e.RowIndex].Value);

            try
            {
                if (!SP.EliminarCliente(IdGimnasioActual, id))
                    MostrarMensaje("No se pudo validar el cliente seleccionado.", "warning");
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547)
                    MostrarMensaje("No se puede eliminar un cliente que tiene pagos registrados.", "warning");
                else
                {
                    string codigo = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpperInvariant();
                    AppLogger.RegistrarError(codigo, "Eliminar cliente", ex);
                    MostrarMensaje("No se pudo eliminar el cliente. Código: " + codigo, "danger");
                }
            }
            CargarClientes();
        }
    }
}
