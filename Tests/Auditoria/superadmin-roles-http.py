import urllib.request,urllib.parse,http.cookiejar,html,re,json
from html.parser import HTMLParser
from pathlib import Path
base='http://localhost:51878/'
class Form(HTMLParser):
 def __init__(self):super().__init__();self.hidden={}
 def handle_starttag(self,tag,attrs):
  a=dict(attrs)
  if tag=='input' and a.get('type')=='hidden':self.hidden[a['name']]=a.get('value','')
def hidden(t):
 f=Form();f.feed(t);return f.hidden
class Client:
 def __init__(self):self.o=urllib.request.build_opener(urllib.request.HTTPCookieProcessor(http.cookiejar.CookieJar()))
 def req(self,page,data=None):
  r=self.o.open(base+page,None if data is None else urllib.parse.urlencode(data).encode(),timeout=45)
  return r,html.unescape(r.read().decode('utf-8'))
 def login(self,email):
  r,t=self.req('Login.aspx');d=hidden(t);d.update(txtEmail=email,txtPassword='SoloFixtureQA!',btnIngresar='Ingresar');return self.req('Login.aspx',d)
results=[]
def check(name,ok):results.append({'test':name,'passed':bool(ok)});print(('PASS ' if ok else 'FAIL ')+name,flush=True)
superadmin=Client();r,t=superadmin.login('qa-super@example.invalid')
check('Superadmin inicio administrativo',r.url.endswith('/Default.aspx') and 'Administración de la plataforma' in t)
check('Superadmin sin enlaces operativos',all(not re.search(r'href="[^"]*(?:'+p+r')(?:\.aspx)?(?:["/?#])',t,re.I) for p in ['Clientes','Registro','Pagos','Deudores']))
check('Superadmin enlaces plataforma y cuenta',all(x in t for x in ['AdministrarGimnasios.aspx','CrearAdministrador.aspx','#mi-cuenta']))
check('Resumen de gimnasio activo e inactivo',bool(re.search(r'Gimnasios activos.*?<p[^>]*>\s*2\s*</p>',t,re.S)) and bool(re.search(r'Gimnasios inactivos.*?<p[^>]*>\s*1\s*</p>',t,re.S)))
check('Resumen sin duplicar cuentas ni contar plataforma',bool(re.search(r'Gimnasios registrados.*?<p[^>]*>\s*3\s*</p>',t,re.S)))
for page in ['Clientes','Registro','Pagos','Deudores']:
 for suffix in ['.aspx','']:
  r,t=superadmin.req(page+suffix);check('Superadmin GET bloqueado '+page+suffix,r.url.endswith('/Default.aspx'))
 r,t=superadmin.req(page+'.aspx',{'__EVENTTARGET':'','ctl00$MainContent$btnGuardar':'Guardar','ctl00$MainContent$btnRegistrar':'Registrar'})
 check('Superadmin POST bloqueado antes de eventos '+page,r.url.endswith('/Default.aspx'))
for page in ['AdministrarGimnasios.aspx','CrearAdministrador.aspx']:
 r,t=superadmin.req(page);check('Superadmin autorizado '+page,r.url.endswith('/'+page) and 'Ocurrió un error' not in t)
 if page=='AdministrarGimnasios.aspx':check('Listado excluye cuenta administrativa de plataforma','QA Plataforma administrativa' not in t)
gym=Client();r,t=gym.login('qa-a@example.invalid')
check('Gimnasio conserva inicio',r.url.endswith('/Default.aspx') and 'Resumen del gimnasio' in t and 'Administración de la plataforma' not in t)
check('Gimnasio menú operativo',all(p in t for p in ['Pagos','Clientes','Registro','Deudores']) and 'AdministrarGimnasios.aspx' not in t and 'CrearAdministrador.aspx' not in t)
for page in ['Clientes.aspx','Registro.aspx','Pagos.aspx','Deudores.aspx']:
 r,t=gym.req(page);check('Gimnasio autorizado '+page,r.url.endswith('/'+page) and 'Ocurrió un error' not in t)
for page in ['AdministrarGimnasios.aspx','CrearAdministrador.aspx']:
 r,t=gym.req(page);check('Gimnasio administración denegada '+page,r.url.endswith('/Default.aspx'))
anon=Client();r,t=anon.req('Default.aspx');check('Anónimo sin acceso',r.url.endswith('/Login.aspx'))
r,t=superadmin.req('Default.aspx');d=hidden(t);d['__EVENTTARGET']='ctl00$btnCerrarSesion';r,t=superadmin.req('Default.aspx',d);check('Logout superadmin',r.url.endswith('/Login.aspx'))
Path(__file__).with_name('superadmin-roles-results.json').write_text(json.dumps(results,ensure_ascii=False,indent=2),encoding='utf-8')
print('TOTAL',len(results),'FAIL',sum(not x['passed'] for x in results))
raise SystemExit(0 if all(x['passed'] for x in results) else 1)
