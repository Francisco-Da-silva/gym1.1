using System.Data;
using System;
using Microsoft.Data.SqlClient;

namespace Conexcion

{
    public class SP
    {
        private static string connectionString =
            "Server=.;Database=GYM_DB;Trusted_Connection=True";

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
                cmd.Parameters.Add("@Plan", SqlDbType.VarChar).Value = plan;

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
    }
}