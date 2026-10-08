using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexxion;
using Conexcion;
class IntegrationTests
{
 static string Cs=ConfigurationManager.ConnectionStrings["GymDb"].ConnectionString;
 static int total,failed;
 static void Test(string name,bool ok){total++;if(!ok)failed++;Console.WriteLine((ok?"PASS ":"FAIL ")+name);}
 static int Count(string sql){using(var c=new SqlConnection(Cs))using(var q=new SqlCommand(sql,c)){c.Open();return Convert.ToInt32(q.ExecuteScalar());}}
 static void Expect(string name,int number,Action action){try{action();Test(name,false);}catch(SqlException e){Test(name,e.Number==number);}}
 static void Main()
 {
  if(new SqlConnectionStringBuilder(Cs).InitialCatalog!="GYM_QA_20261007")throw new Exception("Fixture requerido");
  var u=UsuarioDAL.BuscarPorEmail("qa-a@example.invalid");
  Test("A02 inexistente",UsuarioDAL.BuscarPorEmail("noexiste@example.invalid")==null);
  Test("A04 SQL injection",UsuarioDAL.BuscarPorEmail("' OR 1=1--")==null);
  Test("A login valido",u!=null&&u.Activo&&PasswordHelper.VerificarPassword("SoloFixtureQA!",u.PasswordSalt,u.PasswordHash));
  var off=UsuarioDAL.BuscarPorEmail("qa-off@example.invalid");Test("Cuenta inactiva",off!=null&&!off.Activo);
  Test("C01 ListarClientes A",SP.ListarClientes(1).AsEnumerable().All(r=>Convert.ToInt32(r["IdGimnasio"])==1));
  Test("C01 ListarClientes B",SP.ListarClientes(2).AsEnumerable().All(r=>Convert.ToInt32(r["IdGimnasio"])==2));
  Test("C03 cliente ajeno sin pagos",PagoDAL.ListarPagosPorCliente(1,2).Rows.Count==0);
  Test("C05 pertenece",SP.ClientePertenece(1,1)&&!SP.ClientePertenece(1,2));
  bool noFecha=false;try{SP.ObtenerFechaAltaCliente(1,2);}catch(Exception){noFecha=true;}Test("C04 fecha ajena rechazada",noFecha);
  DateTime period=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1).AddMonths(1),hasta=period.AddMonths(1).AddDays(-1);
  Expect("C06 pago ajeno 50001",50001,()=>PagoDAL.RegistrarPago(1,2,DateTime.Now,period,hasta,15000.50m,null));
  PagoDAL.RegistrarPago(1,1,DateTime.Now,period,hasta,15000.50m,null);
  Test("D01/D09 INSERT futuro",Count("SELECT COUNT(*) FROM dbo.Pagos WHERE IdCliente=1")==1);
  Test("D07 decimal exacto",Count("SELECT COUNT(*) FROM dbo.Pagos WHERE IdCliente=1 AND Monto=15000.50")==1);
  Expect("D02 duplicado 50002",50002,()=>PagoDAL.RegistrarPago(1,1,DateTime.Now,period,hasta,15000m,null));
  Test("D13 Deudores actualizado",DeudoresDAL.ListarDeudores(1).AsEnumerable().All(r=>Convert.ToInt32(r["IdCliente"])!=1));
  DateTime next=period.AddMonths(1),end=next.AddMonths(1).AddDays(-1);
  int success=0,rejected=0,other=0;var gate=new ManualResetEventSlim(false);
  Task[] tasks=Enumerable.Range(0,8).Select(i=>Task.Run(()=>{gate.Wait();try{PagoDAL.RegistrarPago(1,1,DateTime.Now,next,end,100m,null);Interlocked.Increment(ref success);}catch(SqlException e){if(e.Number==50002)Interlocked.Increment(ref rejected);else Interlocked.Increment(ref other);}})).ToArray();
  gate.Set();Task.WaitAll(tasks);
  Test("D03 ocho envios concurrentes",success==1&&rejected==7&&other==0);
  string bad="</script><script>alert(1)</script>";
  SP.AgregarCliente(1,bad,"QA","87654321","","",new DateTime(2000,1,1),"Full");
  Test("F01 cliente INSERT",Count("SELECT COUNT(*) FROM dbo.Clientes WHERE DNI='87654321'")==1);
  Expect("F02 DNI duplicado",50001,()=>SP.AgregarCliente(1,"QA","QA","87654321","","",new DateTime(2000,1,1),"Full"));
  Test("G03 fixture sin huerfanos",Count("SELECT COUNT(*) FROM dbo.Pagos p LEFT JOIN dbo.Clientes c ON p.IdCliente=c.IdCliente WHERE c.IdCliente IS NULL")==0);
  Test("G04 fixture sin cruces",Count("SELECT COUNT(*) FROM dbo.Pagos p JOIN dbo.Clientes c ON p.IdCliente=c.IdCliente WHERE p.IdGimnasio<>c.IdGimnasio")==0);
  Expect("G01 FK compuesto",547,()=>{using(var c=new SqlConnection(Cs))using(var q=new SqlCommand("INSERT dbo.Pagos(IdGimnasio,IdCliente,FechaPago,FechaDesde,FechaHasta,Monto) VALUES(2,1,GETDATE(),'20990101','20990131',100)",c)){c.Open();q.ExecuteNonQuery();}});
  Test("G05 fixture sin duplicados",Count("SELECT COUNT(*) FROM (SELECT IdGimnasio,IdCliente,YEAR(FechaDesde)y,MONTH(FechaDesde)m FROM dbo.Pagos GROUP BY IdGimnasio,IdCliente,YEAR(FechaDesde),MONTH(FechaDesde) HAVING COUNT(*)>1)d")==0);
  string token=TokenHelper.ObtenerHash(TokenHelper.GenerarToken());PasswordResetDAL.GuardarToken(u.IdUsuario,token,DateTime.Now.AddMinutes(10));
  string salt=PasswordHelper.CrearSalt(),hash=PasswordHelper.CrearHash("SoloFixtureQA!",salt);
  PasswordResetDAL.ActualizarPassword(u.IdUsuario,hash,salt,token);
  bool reuse=false;try{PasswordResetDAL.ActualizarPassword(u.IdUsuario,hash,salt,token);}catch(InvalidOperationException){reuse=true;}
  Test("Reset token consumido una vez",reuse&&PasswordResetDAL.ObtenerUsuarioPorToken(token)==null);
  Test("Eliminar cliente ajeno",!SP.EliminarCliente(1,2)&&Count("SELECT COUNT(*) FROM dbo.Clientes WHERE IdCliente=2")==1);
  Console.WriteLine("TOTAL "+total+" FAILED "+failed);Environment.ExitCode=failed==0?0:1;
 }
}
