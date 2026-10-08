from pathlib import Path
import urllib.request,urllib.parse,http.cookiejar,json,re,html
from html.parser import HTMLParser
R=Path(__file__).resolve().parent
base='http://localhost:51877/'
results=[]
class Form(HTMLParser):
 def __init__(self):super().__init__();self.hidden={}
 def handle_starttag(self,tag,attrs):
  a=dict(attrs)
  if tag=='input' and a.get('type')=='hidden':self.hidden[a['name']]=a.get('value','')
def hidden(text):
 p=Form();p.feed(text);return p.hidden
class Client:
 def __init__(self):
  self.jar=http.cookiejar.CookieJar();self.opener=urllib.request.build_opener(urllib.request.HTTPCookieProcessor(self.jar))
 def req(self,page,data=None):
  r=self.opener.open(base+page,None if data is None else urllib.parse.urlencode(data).encode(),timeout=25)
  text=r.read().decode('utf8',errors='replace');return r,html.unescape(text)
 def sid(self):return next((c.value for c in self.jar if c.name=='ASP.NET_SessionId'),None)
 def login(self,email,password='SoloFixtureQA!'):
  r,t=self.req('Login.aspx');old=self.sid();d=hidden(t);d.update(txtEmail=email,txtPassword=password,btnIngresar='Ingresar')
  r,t=self.req('Login.aspx',d);return r,t,old,self.sid()
def test(name,ok,evidence):
 results.append(dict(test=name,passed=bool(ok),evidence=evidence));print(('PASS ' if ok else 'FAIL ')+name)
anon=Client()
for email,pw,name in [('noexiste@example.invalid','anything','A02 usuario inexistente'),('qa-a@example.invalid','incorrecta','A03 contraseña incorrecta'),("' OR 1=1--",'anything','A04 SQL Injection')]:
 r,t,_,_=anon.login(email,pw);test(name,'Correo electrónico o contraseña incorrectos.' in t and r.url.endswith('/Login.aspx'),'POST real, mensaje genérico')
r,t,_,_=anon.login('qa-off@example.invalid');test('Cuenta inactiva','actualmente se encuentra inactiva' in t and r.url.endswith('/Login.aspx'),'POST real de cuenta ficticia inactiva')
a=Client();r,t,old,new=a.login('qa-a@example.invalid');test('J01 login correcto',r.url.endswith('/Default.aspx'),'POST real y entrega de un solo uso')
test('B05 Session Fixation',old is not None and new is not None and old!=new,'SID previo y posterior distintos; valores omitidos')
oldjar=Client();oldjar.jar.set_cookie(http.cookiejar.Cookie(version=0,name='ASP.NET_SessionId',value=old,port=None,port_specified=False,domain='localhost.local',domain_specified=False,domain_initial_dot=False,path='/',path_specified=True,secure=False,expires=None,discard=True,comment=None,comment_url=None,rest={}))
r,t=oldjar.req('Clientes.aspx');test('B05 SID anterior sin acceso',r.url.endswith('/Login.aspx'),'Reutilización del SID previo no autentica')
for page in ['CrearAdministrador.aspx','AdministrarGimnasios.aspx']:
 r,t=a.req(page);test('B06 rol Administrador '+page,r.url.endswith('/Default.aspx'),'GET real bloqueado por rol de base')
r,t=a.req('CrearAdministrador.aspx',{'ctl00$MainContent$btnCrear':'Crear'});test('B06 POST sin rol',r.url.endswith('/Default.aspx'),'Evento de alta no ejecutado')
r,t=a.req('Clientes.aspx');test('J05 listado aislado','Cliente QA A' in t and 'Cliente QA B' not in t,'HTML real solo tenant A')
b=Client();r,t,_,_=b.login('qa-b@example.invalid');r,t=b.req('Clientes.aspx');test('J15 tenant B','Cliente QA B' in t and 'Cliente QA A' not in t,'Segundo usuario y sesión distintos')
r,t=a.req('Pagos.aspx');test('E02 cierre script escapado','\\u003c/script\\u003e' in t and '</script><script>alert(1)</script>' not in t,'Datos ficticios maliciosos desde SQL serializados en la página real')
d=hidden(t);d.update({'ctl00$MainContent$hdnIdClienteSeleccionado':'2','ctl00$MainContent$hdnTextoClienteSeleccionado':'Cliente ajeno','__EVENTTARGET':'ctl00$MainContent$btnSeleccionarCliente'})
r,t=a.req('Pagos.aspx',d);test('C05 hidden manipulado','No se pudo validar el cliente seleccionado.' in t and 'Cliente QA B' not in t,'IdCliente del tenant B rechazado sin excepción')
r,t=a.req('Pagos.aspx');d=hidden(t);d.update({'ctl00$MainContent$hdnIdClienteSeleccionado':'1','ctl00$MainContent$hdnTextoClienteSeleccionado':'QA A','__EVENTTARGET':'ctl00$MainContent$btnSeleccionarCliente'})
r,t=a.req('Pagos.aspx',d);test('J09 selección cliente','QA A' in t and 'SIN PAGO' not in t,'Postback y carga del historial del fixture')
from datetime import date
today=date.today()
def pay_data(t,monto,obs=''):
 d=hidden(t);d.update({'ctl00$MainContent$ddlMesPago':str(today.month),'ctl00$MainContent$ddlAnioPago':str(today.year),'ctl00$MainContent$txtMonto':monto,'ctl00$MainContent$txtObs':obs,'ctl00$MainContent$btnRegistrar':'Registrar pago'})
 return d
for monto,name in [('0','D04 cero'),('-1000','D05 negativo'),('abc','D06 texto'),('10000000000','D08 exceso')]:
 r,t=a.req('Pagos.aspx',pay_data(t,monto));test(name,'El monto es inválido.' in t and '/Error.aspx' not in r.url,'POST real rechazo amigable')
r,t=a.req('Pagos.aspx',pay_data(t,'100','a'*201));test('E01 201 caracteres','observación no puede superar los 200' in t and '/Error.aspx' not in r.url,'POST real longitud excedida')
# El token debe impedir cualquier operación sin aceptar una solicitud de otro contexto.
d=pay_data(t,'100');d.pop('__GymCsrf',None);r,csrf_t=a.req('Pagos.aspx',d)
test('CSRF token obligatorio','/Error.aspx' in r.url,'POST sin token no alcanza el evento protegido')
# Recargar y seleccionar de nuevo luego del error; mantiene sesión autenticada.
r,t=a.req('Pagos.aspx');d=hidden(t);d.update({'ctl00$MainContent$hdnIdClienteSeleccionado':'1','ctl00$MainContent$hdnTextoClienteSeleccionado':'QA A','__EVENTTARGET':'ctl00$MainContent$btnSeleccionarCliente'});r,t=a.req('Pagos.aspx',d)
r,t=a.req('Pagos.aspx',pay_data(t,'15000,50'));test('J11 pago normal UI','Pago registrado correctamente.' in t,'POST real, recarga y mensaje de éxito')
r,t=a.req('Pagos.aspx',pay_data(t,'15000,50'));test('J12 pago duplicado UI','ya tiene registrado un pago' in t and '/Error.aspx' not in r.url,'Segundo envío rechazado con mensaje amigable')
r,t=a.req('Deudores.aspx');test('J14 deudores','QA A' not in re.sub(r'<script.*?</script>','',t,flags=re.S),'Cliente A cubierto por pago futuro no figura como deudor')
r,t=a.req('Registro.aspx');d=hidden(t);d.update({'ctl00$MainContent$txtNombre':'José','ctl00$MainContent$txtApellido':"D'Angelo",'ctl00$MainContent$txtDni':'22334455','ctl00$MainContent$txtTelefono':'+54 9 11 1234-5678','ctl00$MainContent$txtEmail':'','ctl00$MainContent$txtFechaNac':'2000-01-01','ctl00$MainContent$Planpago':'Full','ctl00$MainContent$btnAgregar':'Agregar'})
r,t=a.req('Registro.aspx',d);test('J06 alta cliente UI','Cliente registrado correctamente.' in t,'POST real con nombre y teléfono flexibles')
d.update(hidden(t));r,t=a.req('Registro.aspx',d);test('J07 DNI duplicado UI','Ya existe un cliente con ese DNI' in t,'Segundo alta mismo tenant rechazada')
d.update(hidden(t));d['ctl00$MainContent$txtDni']='22334456';d['ctl00$MainContent$txtFechaNac']='2099-01-01';r,t=a.req('Registro.aspx',d);test('J08 fecha futura UI','fecha de nacimiento no puede ser futura' in t,'POST real sin alta')
r,t=a.req('Pagos.aspx');d=hidden(t);d.update({'ctl00$MainContent$txtObs':'<script>alert(1)</script>','ctl00$MainContent$btnRegistrar':'Registrar'});r,t=a.req('Pagos.aspx',d);test('E03 RequestValidation','contenido no permitido' in t and '/Error.aspx' in r.url,'ASP.NET mantiene validación y usa aviso fijo')
superadmin=Client();r,t,_,_=superadmin.login('qa-super@example.invalid');r,t=superadmin.req('AdministrarGimnasios.aspx');test('SuperAdmin listado','QA Gimnasio A' in t and 'QA Gimnasio B' in t,'Listado real con datos de ambos gimnasios y rol correcto')
r,t=a.req('Default.aspx');d=hidden(t);d['__EVENTTARGET']='ctl00$btnCerrarSesion';r,t=a.req('Default.aspx',d);test('B02/J03 logout',r.url.endswith('/Login.aspx'),'Click real; Clear, Abandon y expiración de cookie')
for page in ['Default.aspx','Clientes.aspx','Pagos.aspx']:
 r,t=a.req(page);test('B04 tras logout '+page,r.url.endswith('/Login.aspx'),'GET real después de logout')
(R/'http-integration-results.json').write_text(json.dumps(results,ensure_ascii=False,indent=2),encoding='utf8')
print('TOTAL',len(results),'FAILED',sum(not x['passed'] for x in results))
raise SystemExit(0 if all(x['passed'] for x in results) else 1)
