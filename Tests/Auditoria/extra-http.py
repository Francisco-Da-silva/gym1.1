import urllib.request,urllib.parse,http.cookiejar,html,re,json
from datetime import date
from html.parser import HTMLParser
from pathlib import Path
R=Path(__file__).resolve().parent
class P(HTMLParser):
 def __init__(self):super().__init__();self.d={}
 def handle_starttag(self,tag,attrs):
  a=dict(attrs)
  if tag=='input' and a.get('type')=='hidden':self.d[a['name']]=a.get('value','')
jar=http.cookiejar.CookieJar();o=urllib.request.build_opener(urllib.request.HTTPCookieProcessor(jar))
def req(page,data=None):
 r=o.open('http://localhost:51877/'+page,None if data is None else urllib.parse.urlencode(data).encode(),timeout=20);return r,html.unescape(r.read().decode('utf8'))
r,t=req('Login.aspx');p=P();p.feed(t);p.d.update(txtEmail='qa-b@example.invalid',txtPassword='SoloFixtureQA!',btnIngresar='Ingresar');r,t=req('Login.aspx',p.d)
r,t=req('Deudores.aspx')
meses=['Enero','Febrero','Marzo','Abril','Mayo','Junio','Julio','Agosto','Septiembre','Octubre','Noviembre','Diciembre']
esperados=[meses[(date.today().month-1-i)%12] for i in range(4)]
results=[dict(test='D12 sin pagos desde FechaAlta',passed=all(m in t for m in esperados),evidence='Cliente de fixture sin pagos con alta tres meses antes; cuatro meses renderizados.')]
for page in ['Pagos.aspx','Deudores.aspx']:
 r,t=req(page);results.append(dict(test='E01 maxlength HTML '+page,passed=bool(re.search(r'(?:id="[^"]*txtObs"[^>]*maxlength="200"|maxlength="200"[^>]*id="[^"]*txtObs")',t,re.I)),evidence='Atributo maxlength200 presente en HTML real.'))
(R/'extra-http-results.json').write_text(json.dumps(results,ensure_ascii=False,indent=2),encoding='utf-8')
for x in results:print(('PASS ' if x['passed'] else 'FAIL ')+x['test'])
raise SystemExit(0 if all(x['passed'] for x in results) else 1)
