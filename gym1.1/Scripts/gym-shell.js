(function () {
  'use strict';
  var root = document.documentElement;
  var sidebar = document.getElementById('gym-sidebar');
  var toggle = document.getElementById('menu-toggle');
  var close = document.getElementById('menu-close');
  var backdrop = document.getElementById('menu-backdrop');
  if (!sidebar || !toggle || !close || !backdrop) return;
  var mobile = window.matchMedia('(max-width: 991.98px)');
  var key = 'gym.sidebar.collapsed';
  var collapsed = false;
  try { collapsed = localStorage.getItem(key) === 'true'; } catch (_) { /* Preferencias opcionales. */ }
  root.classList.add('gm-js');
  toggle.hidden = false;
  function sync() {
    var open = root.classList.contains('sidebar-open');
    root.classList.toggle('sidebar-collapsed', !mobile.matches && collapsed);
    sidebar.inert = mobile.matches && !open;
    backdrop.hidden = !mobile.matches || !open;
    toggle.setAttribute('aria-expanded', String(mobile.matches ? open : !collapsed));
    toggle.setAttribute('aria-label', mobile.matches ? (open ? 'Cerrar menú' : 'Abrir menú') : (collapsed ? 'Expandir menú' : 'Contraer menú'));
    toggle.title = toggle.getAttribute('aria-label');
  }
  function dismiss(restoreFocus) {
    root.classList.remove('sidebar-open');
    sync();
    if (restoreFocus) toggle.focus();
  }
  toggle.addEventListener('click', function () {
    if (mobile.matches) {
      root.classList.toggle('sidebar-open');
      sync();
      if (root.classList.contains('sidebar-open')) close.focus();
    } else {
      collapsed = !collapsed;
      try { localStorage.setItem(key, String(collapsed)); } catch (_) { }
      sync();
    }
  });
  close.addEventListener('click', function () { dismiss(true); });
  backdrop.addEventListener('click', function () { dismiss(true); });
  sidebar.addEventListener('click', function (event) { if (mobile.matches && event.target.closest('a')) dismiss(false); });
  document.addEventListener('keydown', function (event) {
    if (!mobile.matches || !root.classList.contains('sidebar-open')) return;
    if (event.key === 'Escape') { event.preventDefault(); dismiss(true); }
    if (event.key === 'Tab') {
      var controls = Array.prototype.filter.call(sidebar.querySelectorAll('a[href],button:not([disabled]),[tabindex="0"]'), function (el) { return el.getClientRects().length > 0; });
      var first = controls[0], last = controls[controls.length - 1];
      if (event.shiftKey && document.activeElement === first) { event.preventDefault(); last.focus(); }
      else if (!event.shiftKey && document.activeElement === last) { event.preventDefault(); first.focus(); }
    }
  });
  mobile.addEventListener('change', function () { dismiss(false); });
  var form = toggle.form;
  if (form) form.addEventListener('submit', function (event) {
    if (event.defaultPrevented) return;
    var submitter = event.submitter || document.activeElement;
    if (!submitter || submitter.getAttribute('data-submit-once') !== 'true') return;
    if (form.getAttribute('aria-busy') === 'true') { event.preventDefault(); return; }
    form.setAttribute('aria-busy','true');
    // Keep the submitter enabled until its name/value has been serialized by the browser.
    window.setTimeout(function () { submitter.disabled=true; submitter.value='Registrando…'; }, 0);
  });
  window.addEventListener('pageshow', function () { if (form) { form.removeAttribute('aria-busy'); form.querySelectorAll('[data-submit-once]').forEach(function (button) { button.disabled=false; button.value='Registrar pago'; }); } });
  sync();
})();
