using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexxion;
class ExtraTests
{
 static string Cs=ConfigurationManager.ConnectionStrings["GymDb"].ConnectionString;
 static int total,failed;
 static void Test(string name,bool ok){total++;if(!ok)failed++;Console.WriteLine((ok?"PASS ":"FAIL ")+name);}
 static void Main()
 {
  if(new SqlConnectionStringBuilder(Cs).InitialCatalog!="GYM_QA_20261007")throw new Exception("Fixture requerido");
  int success=0,rejected=0,other=0;var gate=new ManualResetEventSlim(false);
  var tasks=Enumerable.Range(0,8).Select(i=>Task.Run(()=>{gate.Wait();try{SP.AgregarCliente(1,"QA concurrencia","QA","33445566","","",new DateTime(2000,1,1),"Full");Interlocked.Increment(ref success);}catch(SqlException e){if(e.Number==50001)Interlocked.Increment(ref rejected);else Interlocked.Increment(ref other);}})).ToArray();
  gate.Set();Task.WaitAll(tasks);
  Test("F02 ocho altas concurrentes",success==1&&rejected==7&&other==0);
  using(var con=new SqlConnection(Cs))
  {
   con.Open();using(var tx=con.BeginTransaction())
   {
    foreach(int days in new [] {-1,0,5,6})
    {
     using(var q=new SqlCommand("DELETE dbo.Pagos WHERE IdCliente=2; INSERT dbo.Pagos(IdGimnasio,IdCliente,FechaPago,FechaDesde,FechaHasta,Monto) VALUES(2,2,GETDATE(),DATEADD(month,-1,CONVERT(date,GETDATE())),DATEADD(day,@Dias,CONVERT(date,GETDATE())),100);",con,tx)){q.Parameters.AddWithValue("@Dias",days);q.ExecuteNonQuery();}
     using(var q=new SqlCommand("dbo.Listar_Pagos_Por_Cliente",con,tx))
     {
      q.CommandType=CommandType.StoredProcedure;q.Parameters.AddWithValue("@IdGimnasio",2);q.Parameters.AddWithValue("@IdCliente",2);
      using(var r=q.ExecuteReader()){r.Read();string state=Convert.ToString(r["EstadoPago"]);Test("D11 limite "+days,days<0?state=="VENCIDO":days<=5?state=="POR VENCER":state.StartsWith("AL D"));}
     }
    }
    tx.Rollback();
   }
   using(var q=new SqlCommand("UPDATE dbo.Clientes SET FechaAlta=DATEADD(month,-3,CONVERT(date,GETDATE())) WHERE IdCliente=2",con))q.ExecuteNonQuery();
  }
  Console.WriteLine("TOTAL "+total+" FAILED "+failed);Environment.ExitCode=failed==0?0:1;
 }
}
