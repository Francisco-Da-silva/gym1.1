using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Conexxion
{
    public static class UsuarioDAL
    {
        private static readonly string connectionString =
            ConfigurationManager
                .ConnectionStrings["GymDb"]
                .ConnectionString;

        public static int RegistrarGimnasioConAdministrador(
            string nombreGimnasio,
            string emailGimnasio,
            string telefono,
            string nombreAdministrador,
            string emailAdministrador,
            string password)
        {
            string salt = PasswordHelper.CrearSalt();
            string hash = PasswordHelper.CrearHash(password, salt);

            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                con.Open();

                using (SqlTransaction transaccion =
                       con.BeginTransaction())
                {
                    try
                    {
                        const string verificarEmail = @"
                            SELECT COUNT(*)
                            FROM dbo.Usuarios
                            WHERE LOWER(Email) = LOWER(@Email);";

                        using (SqlCommand cmdVerificar =
                               new SqlCommand(
                                   verificarEmail,
                                   con,
                                   transaccion))
                        {
                            cmdVerificar.Parameters.Add(
                                "@Email",
                                SqlDbType.NVarChar,
                                100
                            ).Value = emailAdministrador
                                .Trim()
                                .ToLowerInvariant();

                            int cantidad = Convert.ToInt32(
                                cmdVerificar.ExecuteScalar()
                            );

                            if (cantidad > 0)
                            {
                                throw new InvalidOperationException(
                                    "Ya existe una cuenta con ese correo."
                                );
                            }
                        }

                        const string insertarGimnasio = @"
                            INSERT INTO dbo.Gimnasios
                            (
                                Nombre,
                                Email,
                                Telefono,
                                FechaAlta,
                                Activo
                            )
                            VALUES
                            (
                                @Nombre,
                                @Email,
                                @Telefono,
                                GETDATE(),
                                1
                            );

                            SELECT CAST(SCOPE_IDENTITY() AS INT);";

                        int idGimnasio;

                        using (SqlCommand cmdGimnasio =
                               new SqlCommand(
                                   insertarGimnasio,
                                   con,
                                   transaccion))
                        {
                            cmdGimnasio.Parameters.Add(
                                "@Nombre",
                                SqlDbType.NVarChar,
                                100
                            ).Value = nombreGimnasio.Trim();

                            cmdGimnasio.Parameters.Add(
                                "@Email",
                                SqlDbType.NVarChar,
                                100
                            ).Value = emailGimnasio
                                .Trim()
                                .ToLowerInvariant();

                            cmdGimnasio.Parameters.Add(
                                "@Telefono",
                                SqlDbType.NVarChar,
                                30
                            ).Value =
                                string.IsNullOrWhiteSpace(telefono)
                                    ? (object)DBNull.Value
                                    : telefono.Trim();

                            idGimnasio = Convert.ToInt32(
                                cmdGimnasio.ExecuteScalar()
                            );
                        }

                        const string insertarUsuario = @"
                            INSERT INTO dbo.Usuarios
                            (
                                IdGimnasio,
                                Nombre,
                                Email,
                                PasswordHash,
                                PasswordSalt,
                                Rol,
                                Activo
                            )
                            VALUES
                            (
                                @IdGimnasio,
                                @Nombre,
                                @Email,
                                @PasswordHash,
                                @PasswordSalt,
                                @Rol,
                                1
                            );";

                        using (SqlCommand cmdUsuario =
                               new SqlCommand(
                                   insertarUsuario,
                                   con,
                                   transaccion))
                        {
                            cmdUsuario.Parameters.Add(
                                "@IdGimnasio",
                                SqlDbType.Int
                            ).Value = idGimnasio;

                            cmdUsuario.Parameters.Add(
                                "@Nombre",
                                SqlDbType.NVarChar,
                                100
                            ).Value = nombreAdministrador.Trim();

                            cmdUsuario.Parameters.Add(
                                "@Email",
                                SqlDbType.NVarChar,
                                100
                            ).Value = emailAdministrador
                                .Trim()
                                .ToLowerInvariant();

                            cmdUsuario.Parameters.Add(
                                "@PasswordHash",
                                SqlDbType.NVarChar,
                                255
                            ).Value = hash;

                            cmdUsuario.Parameters.Add(
                                "@PasswordSalt",
                                SqlDbType.NVarChar,
                                255
                            ).Value = salt;

                            cmdUsuario.Parameters.Add(
                                "@Rol",
                                SqlDbType.NVarChar,
                                30
                            ).Value = "Administrador";

                            cmdUsuario.ExecuteNonQuery();
                        }

                        transaccion.Commit();

                        return idGimnasio;
                    }
                    catch
                    {
                        transaccion.Rollback();
                        throw;
                    }
                }
            }
        }

        public static bool CambiarPassword(
            string email,
            string nuevaPassword)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException(
                    "El correo electrónico es obligatorio."
                );
            }

            if (string.IsNullOrWhiteSpace(nuevaPassword) ||
                nuevaPassword.Length < 8)
            {
                throw new ArgumentException(
                    "La contraseña debe tener al menos 8 caracteres."
                );
            }

            string nuevoSalt = PasswordHelper.CrearSalt();

            string nuevoHash = PasswordHelper.CrearHash(
                nuevaPassword,
                nuevoSalt
            );

            const string consulta = @"
                UPDATE dbo.Usuarios
                SET PasswordHash = @PasswordHash,
                    PasswordSalt = @PasswordSalt
                WHERE LOWER(Email) = LOWER(@Email);";

            using (SqlConnection conexion =
                   new SqlConnection(connectionString))
            using (SqlCommand comando =
                   new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add(
                    "@PasswordHash",
                    SqlDbType.NVarChar,
                    255
                ).Value = nuevoHash;

                comando.Parameters.Add(
                    "@PasswordSalt",
                    SqlDbType.NVarChar,
                    255
                ).Value = nuevoSalt;

                comando.Parameters.Add(
                    "@Email",
                    SqlDbType.NVarChar,
                    100
                ).Value = email.Trim();

                conexion.Open();

                int filasAfectadas =
                    comando.ExecuteNonQuery();

                return filasAfectadas > 0;
            }
        }

        public static void CrearUsuario(
            int idGimnasio,
            string nombre,
            string email,
            string password,
            string rol)
        {
            string salt = PasswordHelper.CrearSalt();
            string hash = PasswordHelper.CrearHash(
                password,
                salt
            );

            const string consulta = @"
                INSERT INTO dbo.Usuarios
                (
                    IdGimnasio,
                    Nombre,
                    Email,
                    PasswordHash,
                    PasswordSalt,
                    Rol,
                    Activo
                )
                VALUES
                (
                    @IdGimnasio,
                    @Nombre,
                    @Email,
                    @PasswordHash,
                    @PasswordSalt,
                    @Rol,
                    1
                );";

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
                    "@Nombre",
                    SqlDbType.NVarChar,
                    100
                ).Value = nombre.Trim();

                cmd.Parameters.Add(
                    "@Email",
                    SqlDbType.NVarChar,
                    100
                ).Value = email
                    .Trim()
                    .ToLowerInvariant();

                cmd.Parameters.Add(
                    "@PasswordHash",
                    SqlDbType.NVarChar,
                    255
                ).Value = hash;

                cmd.Parameters.Add(
                    "@PasswordSalt",
                    SqlDbType.NVarChar,
                    255
                ).Value = salt;

                cmd.Parameters.Add(
                    "@Rol",
                    SqlDbType.NVarChar,
                    30
                ).Value = rol;

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static UsuarioSesion BuscarPorEmail(
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
                    {
                        return null;
                    }

                    return new UsuarioSesion
                    {
                        IdUsuario = Convert.ToInt32(
                            reader["IdUsuario"]
                        ),

                        IdGimnasio = Convert.ToInt32(
                            reader["IdGimnasio"]
                        ),

                        Nombre =
                            reader["Nombre"].ToString(),

                        Email =
                            reader["Email"].ToString(),

                        Rol =
                            reader["Rol"].ToString(),

                        NombreGimnasio =
                            reader["NombreGimnasio"].ToString(),

                        PasswordHash =
                            reader["PasswordHash"].ToString(),

                        PasswordSalt =
                            reader["PasswordSalt"].ToString()
                    };
                }
            }
        }
    }
}
