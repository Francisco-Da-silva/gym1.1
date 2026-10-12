using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
namespace Conexxion
{
    public static class ClientesDAL
    {
        public static DataSet BuscarPagina(int idGimnasio, string busqueda, int pagina)
        {
            if (idGimnasio <= 0 || pagina < 0 || pagina > 1000000) throw new ArgumentOutOfRangeException();
            string texto = (busqueda ?? "").Trim();
            if (texto.Length > 100) texto = texto.Substring(0, 100);
            string patron = "%" + texto.Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]") + "%";
            const string filtro = " IdGimnasio=@Gym AND (@Texto=N'' OR Nombre LIKE @Patron OR Apellido LIKE @Patron OR DNI LIKE @Patron OR Telefono LIKE @Patron OR Email LIKE @Patron OR PlanPago LIKE @Patron) ";
            string sql = "SELECT COUNT(*) AS Total FROM dbo.Clientes WHERE" + filtro +
                "; SELECT IdCliente,DNI,Nombre,Apellido,Telefono,Email,PlanPago FROM dbo.Clientes WHERE" + filtro +
                " ORDER BY Apellido,Nombre,IdCliente OFFSET @Saltar ROWS FETCH NEXT 20 ROWS ONLY;";
            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["GymDb"].ConnectionString))
            using (var cmd = new SqlCommand(sql, con))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@Gym", SqlDbType.Int).Value=idGimnasio;
                cmd.Parameters.Add("@Texto", SqlDbType.NVarChar,100).Value=texto;
                cmd.Parameters.Add("@Patron", SqlDbType.NVarChar,402).Value=patron;
                cmd.Parameters.Add("@Saltar", SqlDbType.Int).Value=pagina*20;
                var ds = new DataSet(); da.Fill(ds); return ds;
            }
        }
    }
}
