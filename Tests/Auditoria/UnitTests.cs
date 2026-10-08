using System;
using System.Globalization;
using System.Linq;
using Conexxion;
class UnitTests
{
    static int count, failed;
    static void Test(string name, bool ok) { count++; if (!ok) failed++; Console.WriteLine((ok ? "PASS " : "FAIL ") + name); }
    static string Cliente(string nombre="José",string apellido="D'Angelo",string dni="12345678",string tel="+54 9 11 1234-5678",string email="",string plan="Full",DateTime? nac=null)
    { return ValidacionDatos.ErrorCliente(nombre,apellido,dni,tel,email,nac ?? new DateTime(2000,1,1),plan); }
    static void Main()
    {
        foreach (string culture in new [] {"es-AR","en-US"})
        {
            CultureInfo.CurrentCulture=new CultureInfo(culture);
            decimal n;
            Test("D07 "+culture+" entero",ValidacionDatos.TryMonto("15000",out n)&&n==15000m);
            Test("D07 "+culture+" coma",ValidacionDatos.TryMonto("15000,50",out n)&&n==15000.50m);
            Test("D07 "+culture+" punto legado",ValidacionDatos.TryMonto("15000.50",out n)&&n==15000.50m);
            foreach (string text in new [] {"0","-1000","abc","--","10000000000","1,001","1.000,50","9999999999,999"})
                Test("D04-D08 "+culture+" rechaza "+text,!ValidacionDatos.TryMonto(text,out n));
            Test("D08 máximo exacto "+culture,ValidacionDatos.TryMonto("9999999999,99",out n)&&n==9999999999.99m);
        }
        Test("E01 200",ValidacionDatos.ErrorPago(100m,new string('a',200))==null);
        Test("E01 201",ValidacionDatos.ErrorPago(100m,new string('a',201))!=null);
        Test("Observación opcional",ValidacionDatos.ErrorPago(100m,null)==null);
        foreach (string mail in new [] {"hola@","francisco.com","@gmail.com","francisco@gmail",new string('a',101)})
            Test("F04 email inválido",!ValidacionDatos.EmailValido(mail));
        Test("F04 válido",ValidacionDatos.EmailValido("qa@example.com"));
        Test("F04 opcional",ValidacionDatos.EmailValido(""));
        foreach(string name in new [] {"José","Muñoz","María José","D'Angelo","Ana-María"})
            Test("F07 "+name,Cliente(nombre:name)==null);
        Test("F08 50",Cliente(nombre:new string('a',50))==null);
        Test("F08 51 nombre",Cliente(nombre:new string('a',51))!=null);
        Test("F08 51 apellido",Cliente(apellido:new string('a',51))!=null);
        foreach(string tel in new [] {"+54 9 11 1234-5678","(03489) 15-123456","3489 123456"})
            Test("F09 flexible",Cliente(tel:tel)==null);
        Test("F09 21",Cliente(tel:new string('1',21))!=null);
        foreach(string dni in new [] {"","123","123456789","abcdefgh","１２３４５６７８"})
            Test("F03 DNI inválido",Cliente(dni:dni)!=null);
        Test("F05 fecha futura",Cliente(nac:DateTime.Today.AddDays(1))!=null);
        Test("F06 1899",Cliente(nac:new DateTime(1899,1,1))!=null);
        Test("F10 Full",Cliente(plan:"Full")==null);
        Test("F10 3xSemana",Cliente(plan:"3xSemana")==null);
        Test("F10 manipulado",Cliente(plan:"PLAN_FALSO")!=null);
        string salt=PasswordHelper.CrearSalt(),salt2=PasswordHelper.CrearSalt();
        string hash=PasswordHelper.CrearHash("SoloFixtureQA!",salt);
        Test("A05 salt aleatorio",salt!=salt2&&Convert.FromBase64String(salt).Length==16);
        Test("A05 password válida",PasswordHelper.VerificarPassword("SoloFixtureQA!",salt,hash));
        Test("A03 password incorrecta",!PasswordHelper.VerificarPassword("incorrecta",salt,hash));
        Test("A05 hash32",Convert.FromBase64String(hash).Length==32);
        Test("Reset token aleatorio",TokenHelper.GenerarToken()!=TokenHelper.GenerarToken());
        Console.WriteLine("TOTAL "+count+" FAILED "+failed);
        Environment.ExitCode=failed==0?0:1;
    }
}
