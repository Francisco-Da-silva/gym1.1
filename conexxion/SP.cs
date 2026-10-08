using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;



namespace Conexxion


{
    public class SP
    {
        private static string connectionString =
            ConfigurationManager
                .ConnectionStrings["GymDb"]
                .ConnectionString;


        // INSERTAR CLIENTE
        public static void AgregarCliente(
            int idGimnasio,
            string nombre,
            string apellido,
            string dni,
            string telefono,
            string email,
            DateTime fechaNacimiento,
            string planPago)
        {
            string error = ValidacionDatos.ErrorCliente(nombre, apellido, dni, telefono, email, fechaNacimiento, planPago);
            if (idGimnasio <= 0 || error != null)
                throw new ArgumentException(error ?? "El gimnasio no es válido.");
            // Validación de fecha de nacimiento
            if (fechaNacimiento.Date > DateTime.Today)
            {
                throw new ArgumentException(
                    "La fecha de nacimiento no puede ser futura.",
                    nameof(fechaNacimiento)
                );
            }

            // Opcional: evitamos fechas absurdamente antiguas
            if (fechaNacimiento.Year < 1900)
            {
                throw new ArgumentException(
                    "La fecha de nacimiento ingresada no es válida.",
                    nameof(fechaNacimiento)
                );
            }

            using (SqlConnection con =
                new SqlConnection(connectionString))

            using (SqlCommand cmd =
                new SqlCommand("Agregar_Cliente", con))
            {
                cmd.CommandType =
                    CommandType.StoredProcedure;

                cmd.Parameters
                    .Add("@IdGimnasio", SqlDbType.Int)
                    .Value = idGimnasio;

                cmd.Parameters
                    .Add("@Nombre", SqlDbType.NVarChar, 50)
                    .Value = nombre;

                cmd.Parameters
                    .Add("@Apellido", SqlDbType.NVarChar, 50)
                    .Value = apellido;

                cmd.Parameters
                    .Add("@DNI", SqlDbType.NVarChar, 20)
                    .Value = dni;

                cmd.Parameters
                    .Add("@Telefono", SqlDbType.NVarChar, 30)
                    .Value =
                        string.IsNullOrWhiteSpace(telefono)
                            ? (object)DBNull.Value
                            : telefono;

                cmd.Parameters
                    .Add("@Email", SqlDbType.NVarChar, 100)
                    .Value =
                        string.IsNullOrWhiteSpace(email)
                            ? (object)DBNull.Value
                            : email;

                cmd.Parameters
                    .Add("@FechaNacimiento", SqlDbType.Date)
                    .Value = fechaNacimiento.Date;

                cmd.Parameters
                    .Add("@PlanPago", SqlDbType.NVarChar, 30)
                    .Value = planPago;

                con.Open();

                cmd.ExecuteNonQuery();
            }
        }
    

        // 🔹 LISTAR CLIENTES
        public static DataTable ListarClientes(int idGimnasio)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("Listar_Clientes", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@IdGimnasio", SqlDbType.Int).Value =
                    idGimnasio;

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            

            return dt;
        }


        public static bool ClientePertenece(int idGimnasio, int idCliente)
        {
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.Clientes WHERE IdCliente=@IdCliente AND IdGimnasio=@IdGimnasio;", con))
            {
                cmd.Parameters.Add("@IdCliente", SqlDbType.Int).Value=idCliente;
                cmd.Parameters.Add("@IdGimnasio", SqlDbType.Int).Value=idGimnasio;
                con.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
            }
        }
        public static bool EliminarCliente(int idGimnasio, int idCliente)
        {
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("DELETE FROM dbo.Clientes WHERE IdCliente=@IdCliente AND IdGimnasio=@IdGimnasio;", con))
            {
                cmd.Parameters.Add("@IdCliente", SqlDbType.Int).Value=idCliente;
                cmd.Parameters.Add("@IdGimnasio", SqlDbType.Int).Value=idGimnasio;
                con.Open();
                return cmd.ExecuteNonQuery() == 1;
            }
        }

        // OBTENER FECHA DE ALTA DEL CLIENTE
        public static DateTime ObtenerFechaAltaCliente(
            int idGimnasio,
            int idCliente)
        {
            const string query = @"
        SELECT FechaAlta
        FROM dbo.Clientes
        WHERE IdCliente = @IdCliente
          AND IdGimnasio = @IdGimnasio;";

            using (SqlConnection con =
                new SqlConnection(connectionString))

            using (SqlCommand cmd =
                new SqlCommand(query, con))
            {
                cmd.Parameters
                    .Add("@IdCliente", SqlDbType.Int)
                    .Value = idCliente;

                cmd.Parameters
                    .Add("@IdGimnasio", SqlDbType.Int)
                    .Value = idGimnasio;

                con.Open();

                object resultado =
                    cmd.ExecuteScalar();

                if (resultado == null ||
                    resultado == DBNull.Value)
                {
                    throw new Exception(
                        "No se encontró la fecha de alta del cliente."
                    );
                }

                return Convert.ToDateTime(resultado);
            }
        }


        public static class PagoDAL
        {
            private static string connectionString =
                ConfigurationManager.ConnectionStrings["GymDb"].ConnectionString;

            // ✅ LISTAR PAGOS POR CLIENTE
            public static DataTable ListarPagosPorCliente(
     int idGimnasio,
     int idCliente)
            {
                DataTable dt = new DataTable();

                using (SqlConnection con =
                    new SqlConnection(connectionString))
                using (SqlCommand cmd =
                    new SqlCommand(
                        "Listar_Pagos_Por_Cliente",
                        con))
                {
                    cmd.CommandType =
                        CommandType.StoredProcedure;

                    cmd.Parameters.Add(
                        "@IdGimnasio",
                        SqlDbType.Int
                    ).Value = idGimnasio;

                    cmd.Parameters.Add(
                        "@IdCliente",
                        SqlDbType.Int
                    ).Value = idCliente;

                    using (SqlDataAdapter da =
                        new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }

                return dt;
            }

            public static bool ExistePagoEnMes(int idCliente, DateTime fechaDesde)
            {
                return Conexxion.PagoDAL.ExistePagoEnMes(IdGimnasioSesion(), idCliente, fechaDesde);
            }
            public static void RegistrarPago(int idCliente, DateTime fechaPago, DateTime fechaDesde,
                DateTime fechaHasta, decimal monto, string observacion)
            {
                Conexxion.PagoDAL.RegistrarPago(IdGimnasioSesion(), idCliente, fechaPago, fechaDesde, fechaHasta, monto, observacion);
            }
            private static int IdGimnasioSesion()
            {
                var contexto = System.Web.HttpContext.Current;
                int id;
                if (contexto == null || contexto.Session == null ||
                    !int.TryParse(Convert.ToString(contexto.Session["IdGimnasio"]), out id) || id <= 0)
                    throw new InvalidOperationException("Se requiere una sesión de gimnasio.");
                return id;
            }
        }
    }
}
