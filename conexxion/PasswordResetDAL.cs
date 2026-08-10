using Conexxion;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Conexcion
{
    public static class PasswordResetDAL
    {
        private static readonly string connectionString =
            ConfigurationManager
                .ConnectionStrings["GymDb"]
                .ConnectionString;

        public static UsuarioSesion BuscarUsuarioPorEmail(
            string email)
        {
            const string consulta = @"
                SELECT
                    u.IdUsuario,
                    u.IdGimnasio,
                    u.Nombre,
                    u.Email,
                    u.PasswordHash,
                    u.PasswordSalt,
                    u.Rol,
                    g.Nombre AS NombreGimnasio
                FROM dbo.Usuarios u
                INNER JOIN dbo.Gimnasios g
                    ON g.IdGimnasio = u.IdGimnasio
                WHERE LOWER(u.Email) = LOWER(@Email)
                  AND u.Activo = 1
                  AND g.Activo = 1;";

            using (SqlConnection con =
                   new SqlConnection(connectionString))
            using (SqlCommand cmd =
                   new SqlCommand(consulta, con))
            {
                cmd.Parameters.Add(
                    "@Email",
                    SqlDbType.NVarChar,
                    100
                ).Value = email.Trim();

                con.Open();

                using (SqlDataReader reader =
                       cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    return new UsuarioSesion
                    {
                        IdUsuario =
                            Convert.ToInt32(
                                reader["IdUsuario"]
                            ),

                        IdGimnasio =
                            Convert.ToInt32(
                                reader["IdGimnasio"]
                            ),

                        Nombre =
                            reader["Nombre"].ToString(),

                        Email =
                            reader["Email"].ToString(),

                        Rol =
                            reader["Rol"].ToString(),

                        NombreGimnasio =
                            reader["NombreGimnasio"]
                                .ToString(),

                        PasswordHash =
                            reader["PasswordHash"]
                                .ToString(),

                        PasswordSalt =
                            reader["PasswordSalt"]
                                .ToString()
                    };
                }
            }
        }

        public static void GuardarToken(
            int idUsuario,
            string tokenHash,
            DateTime fechaVencimiento)
        {
            const string consulta = @"
                UPDATE dbo.PasswordResetTokens
                SET Utilizado = 1
                WHERE IdUsuario = @IdUsuario
                  AND Utilizado = 0;

                INSERT INTO dbo.PasswordResetTokens
                (
                    IdUsuario,
                    TokenHash,
                    FechaCreacion,
                    FechaVencimiento,
                    Utilizado
                )
                VALUES
                (
                    @IdUsuario,
                    @TokenHash,
                    GETDATE(),
                    @FechaVencimiento,
                    0
                );";

            using (SqlConnection con =
                   new SqlConnection(connectionString))
            using (SqlCommand cmd =
                   new SqlCommand(consulta, con))
            {
                cmd.Parameters.Add(
                    "@IdUsuario",
                    SqlDbType.Int
                ).Value = idUsuario;

                cmd.Parameters.Add(
                    "@TokenHash",
                    SqlDbType.NVarChar,
                    100
                ).Value = tokenHash;

                cmd.Parameters.Add(
                    "@FechaVencimiento",
                    SqlDbType.DateTime
                ).Value = fechaVencimiento;

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static int? ObtenerUsuarioPorToken(
            string tokenHash)
        {
            const string consulta = @"
                SELECT TOP 1 IdUsuario
                FROM dbo.PasswordResetTokens
                WHERE TokenHash = @TokenHash
                  AND Utilizado = 0
                  AND FechaVencimiento >= GETDATE()
                ORDER BY IdToken DESC;";

            using (SqlConnection con =
                   new SqlConnection(connectionString))
            using (SqlCommand cmd =
                   new SqlCommand(consulta, con))
            {
                cmd.Parameters.Add(
                    "@TokenHash",
                    SqlDbType.NVarChar,
                    100
                ).Value = tokenHash;

                con.Open();

                object resultado =
                    cmd.ExecuteScalar();

                if (resultado == null ||
                    resultado == DBNull.Value)
                {
                    return null;
                }

                return Convert.ToInt32(resultado);
            }
        }

        public static void ActualizarPassword(
            int idUsuario,
            string nuevoHash,
            string nuevoSalt,
            string tokenHash)
        {
            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                con.Open();

                using (SqlTransaction tx =
                       con.BeginTransaction())
                {
                    try
                    {
                        const string actualizar = @"
                            UPDATE dbo.Usuarios
                            SET PasswordHash = @PasswordHash,
                                PasswordSalt = @PasswordSalt
                            WHERE IdUsuario = @IdUsuario;";

                        using (SqlCommand cmd =
                               new SqlCommand(
                                   actualizar,
                                   con,
                                   tx))
                        {
                            cmd.Parameters.Add(
                                "@PasswordHash",
                                SqlDbType.NVarChar,
                                255
                            ).Value = nuevoHash;

                            cmd.Parameters.Add(
                                "@PasswordSalt",
                                SqlDbType.NVarChar,
                                255
                            ).Value = nuevoSalt;

                            cmd.Parameters.Add(
                                "@IdUsuario",
                                SqlDbType.Int
                            ).Value = idUsuario;

                            cmd.ExecuteNonQuery();
                        }

                        const string invalidar = @"
                            UPDATE dbo.PasswordResetTokens
                            SET Utilizado = 1
                            WHERE TokenHash = @TokenHash;";

                        using (SqlCommand cmd =
                               new SqlCommand(
                                   invalidar,
                                   con,
                                   tx))
                        {
                            cmd.Parameters.Add(
                                "@TokenHash",
                                SqlDbType.NVarChar,
                                100
                            ).Value = tokenHash;

                            cmd.ExecuteNonQuery();
                        }

                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}