using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
namespace Conexxion
{
    public static class DashboardDAL
    {
        public static DataSet Obtener(int idGimnasio)
        {
            if (idGimnasio<=0) throw new ArgumentOutOfRangeException("idGimnasio");
            // Reutiliza exactamente la definición de deuda/vencimiento del procedimiento vigente.
            const string sql=@"
EXEC dbo.Listar_Deudores @IdGimnasio=@Gym;
SELECT COUNT(*) AS Total FROM dbo.Clientes WHERE IdGimnasio=@Gym;
SELECT COALESCE(SUM(p.Monto),0) AS Total FROM dbo.Pagos p
JOIN dbo.Clientes c ON c.IdCliente=p.IdCliente AND c.IdGimnasio=p.IdGimnasio
WHERE p.IdGimnasio=@Gym AND c.IdGimnasio=@Gym
AND p.FechaPago>=DATEFROMPARTS(YEAR(GETDATE()),MONTH(GETDATE()),1)
AND p.FechaPago<DATEADD(month,1,DATEFROMPARTS(YEAR(GETDATE()),MONTH(GETDATE()),1));
SELECT TOP (5) p.IdCliente,c.Nombre+N' '+c.Apellido AS Cliente,p.FechaPago,p.FechaDesde,p.FechaHasta,p.Monto
FROM dbo.Pagos p JOIN dbo.Clientes c ON c.IdCliente=p.IdCliente AND c.IdGimnasio=p.IdGimnasio
WHERE p.IdGimnasio=@Gym AND c.IdGimnasio=@Gym ORDER BY p.FechaPago DESC,p.IdPago DESC;";
            using(var con=new SqlConnection(ConfigurationManager.ConnectionStrings["GymDb"].ConnectionString))
            using(var cmd=new SqlCommand(sql,con))
            using(var da=new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@Gym",SqlDbType.Int).Value=idGimnasio;
                var ds=new DataSet(); da.Fill(ds); return ds;
            }
        }
    }
}
