using System;
using System.Configuration;
using System.Data.SqlClient;
using Conexxion;
class Seed
{
 static void Main()
 {
  using(var con=new SqlConnection(ConfigurationManager.ConnectionStrings["GymDb"].ConnectionString))
  {
   if(new SqlConnectionStringBuilder(con.ConnectionString).InitialCatalog!="GYM_QA_20261007")throw new Exception("Fixture requerido");
   con.Open();
   foreach(var data in new [] {new[]{"1","qa-a@example.invalid","Administrador"},new[]{"1","qa-super@example.invalid","SuperAdministrador"},new[]{"2","qa-b@example.invalid","Administrador"},new[]{"3","qa-off@example.invalid","Administrador"}})
   using(var cmd=new SqlCommand("INSERT dbo.Usuarios(IdGimnasio,Nombre,Email,PasswordHash,PasswordSalt,Rol) VALUES(@Gym,N'Usuario QA',@Email,@Hash,@Salt,@Rol)",con))
   {
    string salt=PasswordHelper.CrearSalt();
    cmd.Parameters.AddWithValue("@Gym",int.Parse(data[0]));cmd.Parameters.AddWithValue("@Email",data[1]);
    cmd.Parameters.AddWithValue("@Hash",PasswordHelper.CrearHash("SoloFixtureQA!",salt));cmd.Parameters.AddWithValue("@Salt",salt);cmd.Parameters.AddWithValue("@Rol",data[2]);
    cmd.ExecuteNonQuery();
   }
  }
  Console.WriteLine("Cuatro usuarios ficticios creados en base aislada.");
 }
}
