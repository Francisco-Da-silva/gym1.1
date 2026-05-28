using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Data;
using System.Data.SqlClient;


namespace Conexxion
{
    public static class DeudoresDAL
    {
        private static string connectionString =
            ConfigurationManager.ConnectionStrings["GymDb"].ConnectionString;

        public static DataTable ListarDeudores()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("Listar_Deudores", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            return dt;
        }

        public static void MarcarPagoMes(
            int idCliente,
            DateTime desde,
            DateTime hasta,
            decimal monto,
            string observacion
        )
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("Marcar_Pago_Mes", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdCliente", idCliente);
                cmd.Parameters.AddWithValue("@FechaDesde", desde.Date);
                cmd.Parameters.AddWithValue("@FechaHasta", hasta.Date);
                cmd.Parameters.AddWithValue("@Monto", monto);
                cmd.Parameters.AddWithValue("@Observacion",
                    string.IsNullOrWhiteSpace(observacion)
                        ? (object)DBNull.Value
                        : observacion);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
