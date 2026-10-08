import urllib.request, urllib.parse, urllib.error, http.cookiejar, json, re
from pathlib import Path
R=Path(__file__).resolve().parent
base='http://localhost:51877/'
jar=http.cookiejar.CookieJar()
opener=urllib.request.build_opener(urllib.request.HTTPCookieProcessor(jar))
results=[]
def req(path,data=None):
 r=opener.open(base+path, None if data is None else urllib.parse.urlencode(data).encode(),timeout=20)
 return r,r.read().decode('utf-8',errors='replace')
def result(name,ok,evidence):results.append(dict(test=name,passed=bool(ok),evidence=evidence))
for path in ['Default.aspx','Clientes.aspx','Pagos.aspx','Deudores.aspx','Registro.aspx','CrearAdministrador.aspx','AdministrarGimnasios.aspx']:
 r,html=req(path)
 result('B01/B04 '+path,r.url.endswith('/Login.aspx') and 'Ingresar' in html,'GET sin sesión -> '+r.url)
def hidden(html):
 from html.parser import HTMLParser
 class Parser(HTMLParser):
  def __init__(self):super().__init__();self.inputs={}
  def handle_starttag(self,tag,attrs):
   a=dict(attrs)
   if tag=='input' and a.get('type')=='hidden':self.inputs[a['name']]=a.get('value','')
 p=Parser();p.feed(html);return p.inputs
r,html=req('Login.aspx');data=hidden(html)
data.update({'txtEmail':'','txtPassword':'','btnIngresar':'Ingresar'})
r,html=req('Login.aspx',data)
result('A01 login vacío','Ingresá el correo electrónico y la contraseña.' in html and '/Login.aspx' in r.url,'POST real sin excepción ni sesión autenticada')
r,html=req('Error.aspx?codigo=D13BEEC9')
result('I01 referencia','D13BEEC9' in html and 'StackTrace' not in html,'GET real, referencia y mensaje genérico')
r,html=req('Error.aspx?codigo=%26lt%3Bscript%26gt%3B')
result('I01 referencia inválida','Código de referencia:' not in html and '<script>' not in html,'Query manipulada no se refleja como HTML')
r,html=req('Error.aspx?entrada=invalida')
result('E03 mensaje entrada','contenido no permitido' in html,'Mensaje amigable fijo sin desactivar RequestValidation')
r,html=req('Registro.aspx',{'__EVENTTARGET':'ctl00$MainContent$btnAgregar','ctl00$MainContent$txtNombre':'QA'})
result('B06 POST anónimo no ejecuta evento',r.url.endswith('/Login.aspx'),'POST directo bloqueado antes de Click')
(R/'http-results.json').write_text(json.dumps(results,ensure_ascii=False,indent=2),encoding='utf-8')
for t in results: print(('PASS' if t['passed'] else 'FAIL')+' '+t['test'])
if not all(t['passed'] for t in results):raise SystemExit(1)
