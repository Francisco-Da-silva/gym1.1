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
            using (SqlConnection con =
                   new SqlConnection(connectionString))
            using (SqlCommand cmd =
                   new SqlCommand("Marcar_Pago_Mes", con))
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

                cmd.Parameters.Add(
                    "@FechaDesde",
                    SqlDbType.Date
                ).Value = desde.Date;

                cmd.Parameters.Add(
                    "@FechaHasta",
                    SqlDbType.Date
                ).Value = hasta.Date;

                SqlParameter parametroMonto =
                    cmd.Parameters.Add(
                        "@Monto",
                        SqlDbType.Decimal
                    );

                parametroMonto.Precision = 10;
                parametroMonto.Scale = 2;
                parametroMonto.Value = monto;

                cmd.Parameters.Add(
                    "@Observacion",
                    SqlDbType.NVarChar,
                    200
                ).Value =
                    string.IsNullOrWhiteSpace(observacion)
                        ? (object)DBNull.Value
                        : observacion.Trim();

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}