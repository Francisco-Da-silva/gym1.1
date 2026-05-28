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
            ConfigurationManager.ConnectionStrings["GymDb"].ConnectionString;

        // 🔹 INSERTAR CLIENTE
        public static void AgregarCliente(
            string nombre,
            string apellido,
            string dni,
            string telefono,
            string email,
            DateTime fechaNacimiento,
            string plan)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("Agregar_Cliente", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Nombre", SqlDbType.VarChar).Value = nombre;
                cmd.Parameters.Add("@Apellido", SqlDbType.VarChar).Value = apellido;
                cmd.Parameters.Add("@DNI", SqlDbType.VarChar).Value = dni;
                cmd.Parameters.Add("@Telefono", SqlDbType.VarChar).Value = telefono;
                cmd.Parameters.Add("@Email", SqlDbType.VarChar).Value = email;
                cmd.Parameters.Add("@FechaNacimiento", SqlDbType.Date).Value = fechaNacimiento;
                cmd.Parameters.Add("@Planpago", SqlDbType.VarChar).Value = plan;

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // 🔹 LISTAR CLIENTES
        public static DataTable ListarClientes()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("Listar_Clientes", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            return dt;
        }



        public static class PagoDAL
        {
            private static string connectionString =
                ConfigurationManager.ConnectionStrings["GymDb"].ConnectionString;

            // ✅ LISTAR PAGOS POR CLIENTE
            public static DataTable ListarPagosPorCliente(int idCliente)
            {
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("Listar_Pagos_Por_Cliente", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdCliente", idCliente);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }

                return dt;
            }

            public static bool ExistePagoEnMes(int idCliente, DateTime fechaDesde)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT COUNT(1)
                    FROM dbo.Pagos
                    WHERE IdCliente = @IdCliente
                      AND YEAR(FechaDesde) = @Anio
                      AND MONTH(FechaDesde) = @Mes;", con))
                {
                    cmd.Parameters.AddWithValue("@IdCliente", idCliente);
                    cmd.Parameters.AddWithValue("@Anio", fechaDesde.Year);
                    cmd.Parameters.AddWithValue("@Mes", fechaDesde.Month);

                    con.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }

            // ✅ REGISTRAR UN NUEVO PAGO
            public static void RegistrarPago(
                int idCliente,
                DateTime fechaPago,
                DateTime fechaDesde,
                DateTime fechaHasta,
                decimal monto,
                string observacion
            )
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("Registrar_Pago", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IdCliente", idCliente);
                    cmd.Parameters.AddWithValue("@FechaPago", fechaPago);
                    cmd.Parameters.AddWithValue("@FechaDesde", fechaDesde);
                    cmd.Parameters.AddWithValue("@FechaHasta", fechaHasta);
                    cmd.Parameters.AddWithValue("@Monto", monto);
                    cmd.Parameters.AddWithValue("@Observacion",
                        string.IsNullOrWhiteSpace(observacion) ? (object)DBNull.Value : observacion);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }

}


