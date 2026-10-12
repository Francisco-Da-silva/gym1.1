from pathlib import Path
import subprocess,time,json,os
qa=Path(os.environ.get('GYM_QA_DIR',str(Path(__file__).parent/'redesign-qa')))
exec((qa/'superadmin-roles-http.py').read_text(encoding='utf-8').split('results=[]')[0])
results=[]
clients=[]
for email in ['qa-a@example.invalid','qa-super@example.invalid']:
 c=Client();r,t=c.login(email);assert r.url.endswith('/Default.aspx'),r.url
 clients.append((c,hidden(t)))
lock=subprocess.Popen(['sqlcmd','-S',r'(localdb)\gymredesign-20261011','-d','GYM_QA_20261007','-E','-b','-Q',"IF DB_NAME()<>'GYM_QA_20261007' THROW 51000,'Solo QA',1; BEGIN TRAN; SELECT COUNT(*) FROM dbo.Usuarios WITH(TABLOCKX,HOLDLOCK); RAISERROR('QA_LOCK_READY',10,1) WITH NOWAIT; WAITFOR DELAY '00:00:45'; ROLLBACK;"],stdout=subprocess.PIPE,stderr=subprocess.STDOUT,text=True)
try:
 time.sleep(1)
 probe=subprocess.run(['sqlcmd','-S',r'(localdb)\gymredesign-20261011','-d','GYM_QA_20261007','-E','-b','-t','1','-Q','SELECT COUNT(*) FROM dbo.Usuarios WITH(READCOMMITTEDLOCK);'],capture_output=True,text=True)
 assert 'timeout' in (probe.stdout+probe.stderr).lower() or 'tiempo' in (probe.stdout+probe.stderr).lower(),repr((probe.returncode,probe.stdout,probe.stderr))
 results.append({'test':'Consulta SQL de usuarios agota tiempo bajo bloqueo QA','passed':True})
 for i,(c,d) in enumerate(clients):
  if i==0:
   bad=dict(d);bad.update(__EVENTTARGET='ctl00$btnCerrarSesion',__GymCsrf='invalido')
   before=time.monotonic();r,t=c.req('Default.aspx',bad)
   results.append({'test':'CSRF inválido rechazado sin esperar SQL','passed':'Error' in r.url and time.monotonic()-before<5})
  d['__EVENTTARGET']='ctl00$btnCerrarSesion';before=time.monotonic();r,t=c.req('Default.aspx',d);elapsed=time.monotonic()-before
  results.append({'test':('Logout gimnasio' if i==0 else 'Logout superadmin')+' con Usuarios bloqueada','passed':r.url.endswith('/Login.aspx') and elapsed<5,'seconds':round(elapsed,3)})
  r,t=c.req('Clientes.aspx')
  results.append({'test':'Sesión invalidada después de logout '+str(i),'passed':r.url.endswith('/Login.aspx')})
finally:
 lock.terminate();lock.wait(timeout=10)
(qa/'logout-timeout-results.json').write_text(json.dumps(results,ensure_ascii=False,indent=2),encoding='utf-8')
print(json.dumps(results,ensure_ascii=False,indent=2))
assert all(x['passed'] for x in results)
