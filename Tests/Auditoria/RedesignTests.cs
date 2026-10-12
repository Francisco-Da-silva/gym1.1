using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Conexxion;
class RedesignTests
{
 static int count,fail;
 static void Check(string name,bool ok) { count++; if(!ok)fail++; Console.WriteLine((ok?"PASS ":"FAIL ")+name); }
 static void Main()
 {
  string cs=ConfigurationManager.ConnectionStrings["GymDb"].ConnectionString;
  if(new SqlConnectionStringBuilder(cs).InitialCatalog!="GYM_QA_20261007" || !new SqlConnectionStringBuilder(cs).DataSource.Contains("gymredesign-20261011")) throw new Exception("Fixture exclusiva requerida");
  for(int i=0;i<25;i++) SP.AgregarCliente(1,"QA Paginado","Prueba",(40000000+i).ToString(),"","",new DateTime(2000,1,1),"Full");
  var page=ClientesDAL.BuscarPagina(1,"QA Paginado",0);
  Check("Paginación total25",Convert.ToInt32(page.Tables[0].Rows[0]["Total"])==25);
  Check("Primera página20",page.Tables[1].Rows.Count==20);
  Check("Segunda página5",ClientesDAL.BuscarPagina(1,"QA Paginado",1).Tables[1].Rows.Count==5);
  Check("Búsqueda DNI",ClientesDAL.BuscarPagina(1,"40000024",0).Tables[1].Rows.Count==1);
  Check("Búsqueda apellido",Convert.ToInt32(ClientesDAL.BuscarPagina(1,"Prueba",0).Tables[0].Rows[0]["Total"])==25);
  Check("Búsqueda SQL injection literal",ClientesDAL.BuscarPagina(1,"' OR 1=1--",0).Tables[1].Rows.Count==0);
  Check("Búsqueda comodín literal",ClientesDAL.BuscarPagina(1,"%",0).Tables[1].Rows.Count==0);
  Check("Búsqueda aislamientoB",ClientesDAL.BuscarPagina(2,"QA Paginado",0).Tables[1].Rows.Count==0);
  Check("Búsqueda sin resultados",ClientesDAL.BuscarPagina(1,"InexistenteXYZ",0).Tables[1].Rows.Count==0);
  var a=DashboardDAL.Obtener(1); var b=DashboardDAL.Obtener(2);
  Check("Dashboard datasets",a.Tables.Count==4&&b.Tables.Count==4);
  Check("Dashboard clientes aislados",Convert.ToInt32(a.Tables[1].Rows[0]["Total"])>25 && Convert.ToInt32(b.Tables[1].Rows[0]["Total"])==1);
  Check("Dashboard pagosB vacío",b.Tables[3].Rows.Count==0&&Convert.ToDecimal(b.Tables[2].Rows[0]["Total"])==0);
  Check("Dashboard pagos recientesA",a.Tables[3].Rows.Count>0&&a.Tables[3].Rows.Count<=5&&a.Tables[3].AsEnumerable().All(r=>Convert.ToInt32(r["IdCliente"])==1));
  Check("Dashboard deudaB aislada",b.Tables[0].AsEnumerable().All(r=>Convert.ToInt32(r["IdGimnasio"])==2));
  using(var con=new SqlConnection(cs)) using(var cmd=new SqlCommand("SELECT COALESCE(SUM(Monto),0) FROM dbo.Pagos WHERE IdGimnasio=1 AND FechaPago>=DATEFROMPARTS(YEAR(GETDATE()),MONTH(GETDATE()),1) AND FechaPago<DATEADD(month,1,DATEFROMPARTS(YEAR(GETDATE()),MONTH(GETDATE()),1))",con))
  {con.Open();Check("Dashboard suma mensual exacta",Convert.ToDecimal(cmd.ExecuteScalar())==Convert.ToDecimal(a.Tables[2].Rows[0]["Total"]));}
  Console.WriteLine("TOTAL "+count+" FAILED "+fail); Environment.ExitCode=fail==0?0:1;
 }
}
