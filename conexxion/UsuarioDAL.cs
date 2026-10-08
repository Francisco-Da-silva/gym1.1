using Conexxion;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Conexcion
{
    public static class UsuarioDAL
    {
        // ==========================================
        // CONEXIÓN
        // ==========================================

        private static readonly string connectionString =
            ConfigurationManager
                .ConnectionStrings["GymDb"]
                .ConnectionString;


        // ==========================================
        // BUSCAR USUARIO POR EMAIL
        // LOGIN
        // ==========================================

        public static UsuarioSesion BuscarPorEmail(
            string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            const string consulta = @"
                SELECT
                    u.IdUsuario,
                    u.IdGimnasio,
                    u.Nombre,
                    u.Email,
                    u.PasswordHash,
                    u.PasswordSalt,
                    u.Rol,
                    CAST(CASE WHEN u.Activo = 1 AND g.Activo = 1
                        THEN 1 ELSE 0 END AS bit) AS Activo,
                    g.Nombre AS NombreGimnasio
                FROM dbo.Usuarios u
                INNER JOIN dbo.Gimnasios g
                    ON g.IdGimnasio = u.IdGimnasio
                WHERE LOWER(u.Email) = LOWER(@Email);";

            using (SqlConnection con =
                new SqlConnection(connectionString))
            using (SqlCommand cmd =
                new SqlCommand(consulta, con))
            {
                cmd.Parameters.Add(
                    "@Email",
                    SqlDbType.NVarChar,
                    100
                ).Value = email
                    .Trim()
                    .ToLowerInvariant();

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
                        IdUsuario =
                            Convert.ToInt32(
                                reader["IdUsuario"]
                            ),

                        IdGimnasio =
                            Convert.ToInt32(
                                reader["IdGimnasio"]
                            ),

                        Nombre =
                            reader["Nombre"]
                                .ToString(),

                        Email =
                            reader["Email"]
                                .ToString(),

                        Rol =
                            reader["Rol"]
                                .ToString(),

                        NombreGimnasio =
                            reader["NombreGimnasio"]
                                .ToString(),

                        PasswordHash =
                            reader["PasswordHash"]
                                .ToString(),

                        Activo = Convert.ToBoolean(reader["Activo"]),

                        PasswordSalt =
                            reader["PasswordSalt"]
                                .ToString()
                    };
                }
            }
        }


        // ==========================================
        // REGISTRAR NUEVO GIMNASIO
        // +
        // CREAR SU CUENTA DE ACCESO
        // ==========================================

        public static string ObtenerRolActivo(int idUsuario, int idGimnasio)
        {
            const string consulta = @"SELECT u.Rol FROM dbo.Usuarios u
                INNER JOIN dbo.Gimnasios g ON g.IdGimnasio=u.IdGimnasio
                WHERE u.IdUsuario=@IdUsuario AND u.IdGimnasio=@IdGimnasio
                  AND u.Activo=1 AND g.Activo=1;";
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(consulta, con))
            {
                cmd.Parameters.Add("@IdUsuario", SqlDbType.Int).Value=idUsuario;
                cmd.Parameters.Add("@IdGimnasio", SqlDbType.Int).Value=idGimnasio;
                con.Open();
                object resultado=cmd.ExecuteScalar();
                return resultado == null || resultado == DBNull.Value ? null : Convert.ToString(resultado);
            }
        }

        public static int RegistrarGimnasioConAdministrador(
            string nombreGimnasio,
            string emailGimnasio,
            string telefono,
            string nombreUsuario,
            string emailUsuario,
            string password)
        {
            // ======================================
            // VALIDACIONES BÁSICAS
            // ======================================

            if (string.IsNullOrWhiteSpace(nombreGimnasio))
            {
                throw new InvalidOperationException(
                    "El nombre del gimnasio es obligatorio."
                );
            }

            if (string.IsNullOrWhiteSpace(emailGimnasio))
            {
                throw new InvalidOperationException(
                    "El correo del gimnasio es obligatorio."
                );
            }

            if (string.IsNullOrWhiteSpace(nombreUsuario))
            {
                throw new InvalidOperationException(
                    "El nombre del usuario es obligatorio."
                );
            }

            if (string.IsNullOrWhiteSpace(emailUsuario))
            {
                throw new InvalidOperationException(
                    "El correo de acceso es obligatorio."
                );
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "La contraseña es obligatoria."
                );
            }


            // ======================================
            // NORMALIZAR DATOS
            // ======================================

            nombreGimnasio =
                nombreGimnasio.Trim();

            emailGimnasio =
                emailGimnasio
                    .Trim()
                    .ToLowerInvariant();

            nombreUsuario =
                nombreUsuario.Trim();

            emailUsuario =
                emailUsuario
                    .Trim()
                    .ToLowerInvariant();


            // ======================================
            // GENERAR CONTRASEÑA SEGURA
            // ======================================

            if (nombreGimnasio.Length > 100 || nombreUsuario.Length > 100 ||
                emailGimnasio.Length > 100 || emailUsuario.Length > 100 ||
                (telefono ?? "").Trim().Length > 30 || password.Length < 8 || password.Length > 100 ||
                !ValidacionDatos.EmailValido(emailGimnasio) || !ValidacionDatos.EmailValido(emailUsuario))
                throw new InvalidOperationException("Revisá los formatos y las longitudes de los datos ingresados.");

            string salt =
                PasswordHelper.CrearSalt();

            string hash =
                PasswordHelper.CrearHash(
                    password,
                    salt
                );


            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                con.Open();


                // ==================================
                // TRANSACCIÓN
                // ==================================

                using (SqlTransaction transaccion =
                    con.BeginTransaction())
                {
                    try
                    {
                        // ==============================
                        // VERIFICAR EMAIL DE ACCESO
                        // ==============================

                        const string verificarEmailUsuario = @"
                            SELECT COUNT(*)
                            FROM dbo.Usuarios
                            WHERE LOWER(Email) =
                                  LOWER(@Email);";


                        using (SqlCommand cmdVerificar =
                            new SqlCommand(
                                verificarEmailUsuario,
                                con,
                                transaccion))
                        {
                            cmdVerificar.Parameters.Add(
                                "@Email",
                                SqlDbType.NVarChar,
                                100
                            ).Value = emailUsuario;


                            int cantidad =
                                Convert.ToInt32(
                                    cmdVerificar
                                        .ExecuteScalar()
                                );


                            if (cantidad > 0)
                            {
                                throw new InvalidOperationException(
                                    "Ya existe una cuenta con ese correo electrónico."
                                );
                            }
                        }


                        // ==============================
                        // VERIFICAR EMAIL DEL GIMNASIO
                        // ==============================

                        const string verificarEmailGimnasio = @"
                            SELECT COUNT(*)
                            FROM dbo.Gimnasios
                            WHERE LOWER(Email) =
                                  LOWER(@Email);";


                        using (SqlCommand cmdVerificarGimnasio =
                            new SqlCommand(
                                verificarEmailGimnasio,
                                con,
                                transaccion))
                        {
                            cmdVerificarGimnasio.Parameters.Add(
                                "@Email",
                                SqlDbType.NVarChar,
                                100
                            ).Value = emailGimnasio;


                            int cantidad =
                                Convert.ToInt32(
                                    cmdVerificarGimnasio
                                        .ExecuteScalar()
                                );


                            if (cantidad > 0)
                            {
                                throw new InvalidOperationException(
                                    "Ya existe un gimnasio registrado con ese correo electrónico."
                                );
                            }
                        }


                        // ==============================
                        // INSERTAR GIMNASIO
                        // ==============================

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

                            SELECT CAST(
                                SCOPE_IDENTITY()
                                AS INT
                            );";


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
                            ).Value = nombreGimnasio;


                            cmdGimnasio.Parameters.Add(
                                "@Email",
                                SqlDbType.NVarChar,
                                100
                            ).Value = emailGimnasio;


                            cmdGimnasio.Parameters.Add(
                                "@Telefono",
                                SqlDbType.NVarChar,
                                30
                            ).Value =
                                string.IsNullOrWhiteSpace(
                                    telefono
                                )
                                    ? (object)DBNull.Value
                                    : telefono.Trim();


                            idGimnasio =
                                Convert.ToInt32(
                                    cmdGimnasio
                                        .ExecuteScalar()
                                );
                        }


                        // ==============================
                        // INSERTAR USUARIO DEL GIMNASIO
                        // ==============================

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
                            ).Value = nombreUsuario;


                            cmdUsuario.Parameters.Add(
                                "@Email",
                                SqlDbType.NVarChar,
                                100
                            ).Value = emailUsuario;


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


                            // ==========================
                            // ROL DEL GIMNASIO
                            // ==========================

                            cmdUsuario.Parameters.Add(
                                "@Rol",
                                SqlDbType.NVarChar,
                                30
                            ).Value = "Gimnasio";


                            cmdUsuario.ExecuteNonQuery();
                        }


                        // ==============================
                        // CONFIRMAR TRANSACCIÓN
                        // ==============================

                        transaccion.Commit();


                        return idGimnasio;
                    }
                    catch
                    {
                        // Si falla el gimnasio o el usuario,
                        // se cancela todo.

                        try
                        {
                            transaccion.Rollback();
                        }
                        catch
                        {
                            // Si el rollback falla,
                            // mantenemos la excepción original.
                        }

                        throw;
                    }
                }
            }

        }
        // ==========================================
        // LISTAR TODOS LOS GIMNASIOS
        // SUPERADMINISTRADOR
        // ==========================================

        public static DataTable ListarGimnasios()
        {
            const string consulta = @"
        SELECT
            g.IdGimnasio,
            g.Nombre,
            g.Email,
            g.Telefono,
            g.FechaAlta,
            g.Activo,

            u.Nombre AS NombreUsuario,
            u.Email AS EmailAcceso

        FROM dbo.Gimnasios g

        LEFT JOIN dbo.Usuarios u
            ON u.IdGimnasio = g.IdGimnasio
            AND u.Rol IN ('Gimnasio', 'Administrador')

        ORDER BY
            g.Activo DESC,
            g.Nombre ASC;";


            DataTable dt =
                new DataTable();


            using (SqlConnection con =
                new SqlConnection(connectionString))
            using (SqlCommand cmd =
                new SqlCommand(consulta, con))
            using (SqlDataAdapter da =
                new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }


            return dt;
        }


        // ==========================================
        // DESACTIVAR GIMNASIO
        // ==========================================

        public static bool DesactivarGimnasio(
            int idGimnasio)
        {
            const string consulta = @"
        UPDATE dbo.Gimnasios
        SET Activo = 0
        WHERE IdGimnasio = @IdGimnasio
          AND Activo = 1;";


            using (SqlConnection con =
                new SqlConnection(connectionString))
            using (SqlCommand cmd =
                new SqlCommand(consulta, con))
            {
                cmd.Parameters.Add(
                    "@IdGimnasio",
                    SqlDbType.Int
                ).Value = idGimnasio;


                con.Open();


                int filas =
                    cmd.ExecuteNonQuery();


                return filas > 0;
            }
        }


        // ==========================================
        // ACTIVAR GIMNASIO
        // ==========================================

        public static bool ActivarGimnasio(int idGimnasio)
        {
            const string consulta = @"
        UPDATE dbo.Gimnasios
        SET Activo = 1
        WHERE IdGimnasio = @IdGimnasio
          AND Activo = 0;";


            using (SqlConnection con =
                new SqlConnection(connectionString))
            using (SqlCommand cmd =
                new SqlCommand(consulta, con))
            {
                cmd.Parameters.Add(
                    "@IdGimnasio",
                    SqlDbType.Int
                ).Value = idGimnasio;


                con.Open();


                int filas =
                    cmd.ExecuteNonQuery();


                return filas > 0;
            }
        }
    }
}