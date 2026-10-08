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
        private static readonly string connectionString =
            ConfigurationManager
                .ConnectionStrings["GymDb"]
                .ConnectionString;

        public static DataTable ListarDeudores(int idGimnasio)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con =
                   new SqlConnection(connectionString))
            using (SqlCommand cmd =
                   new SqlCommand("Listar_Deudores", con))
            {
                cmd.CommandType =
                    CommandType.StoredProcedure;

                cmd.Parameters.Add(
                    "@IdGimnasio",
                    SqlDbType.Int
                ).Value = idGimnasio;

                using (SqlDataAdapter da =
                       new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            return dt;
        }

        public static void MarcarPagoMes(
            int idGimnasio,
            int idCliente,
            DateTime desde,
            DateTime hasta,
            decimal monto,
            string observacion)
        {
            PagoDAL.RegistrarPago(idGimnasio, idCliente, DateTime.Now, desde, hasta, monto, observacion);
        }
    }
}
