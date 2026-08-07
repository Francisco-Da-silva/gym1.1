using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Conexxion
{
    public static class PagoDAL
    {
        private static readonly string connectionString =
            ConfigurationManager
                .ConnectionStrings["GymDb"]
                .ConnectionString;

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

        public static bool ExistePagoEnMes(
            int idGimnasio,
            int idCliente,
            DateTime fechaDesde)
        {
            const string consulta = @"
                SELECT COUNT(*)
                FROM dbo.Pagos p
                INNER JOIN dbo.Clientes c
                    ON c.IdCliente = p.IdCliente
                WHERE p.IdCliente = @IdCliente
                  AND p.IdGimnasio = @IdGimnasio
                  AND c.IdGimnasio = @IdGimnasio
                  AND YEAR(p.FechaDesde) = YEAR(@FechaDesde)
                  AND MONTH(p.FechaDesde) = MONTH(@FechaDesde);";

            using (SqlConnection con =
                   new SqlConnection(connectionString))
            using (SqlCommand cmd =
                   new SqlCommand(consulta, con))
            {
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
                ).Value = fechaDesde.Date;

                con.Open();

                int cantidad =
                    Convert.ToInt32(
                        cmd.ExecuteScalar()
                    );

                return cantidad > 0;
            }
        }

        public static void RegistrarPago(
            int idGimnasio,
            int idCliente,
            DateTime fechaPago,
            DateTime fechaDesde,
            DateTime fechaHasta,
            decimal monto,
            string observacion)
        {
            using (SqlConnection con =
                   new SqlConnection(connectionString))
            using (SqlCommand cmd =
                   new SqlCommand(
                       "Registrar_Pago",
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

                cmd.Parameters.Add(
                    "@FechaPago",
                    SqlDbType.DateTime
                ).Value = fechaPago;

                cmd.Parameters.Add(
                    "@FechaDesde",
                    SqlDbType.Date
                ).Value = fechaDesde.Date;

                cmd.Parameters.Add(
                    "@FechaHasta",
                    SqlDbType.Date
                ).Value = fechaHasta.Date;

                SqlParameter parametroMonto =
                    cmd.Parameters.Add(
                        "@Monto",
                        SqlDbType.Decimal
                    );

                parametroMonto.Precision = 12;
                parametroMonto.Scale = 2;
                parametroMonto.Value = monto;

                cmd.Parameters.Add(
                    "@Observacion",
                    SqlDbType.NVarChar,
                    200
                ).Value =
                    string.IsNullOrWhiteSpace(
                        observacion)
                        ? (object)DBNull.Value
                        : observacion.Trim();

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
