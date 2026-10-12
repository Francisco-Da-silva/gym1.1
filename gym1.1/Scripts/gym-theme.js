(function () {
  'use strict';
  var root = document.documentElement;
  var storageKey = 'gym.theme';
  var theme = 'light';
  try { if (localStorage.getItem(storageKey) === 'dark') theme = 'dark'; } catch (_) { }
  root.setAttribute('data-bs-theme', theme);
  document.addEventListener('DOMContentLoaded', function () {
    var button = document.getElementById('theme-toggle');
    if (!button) return;
    function sync() {
      button.hidden = false;
      button.setAttribute('aria-pressed', String(theme === 'dark'));
      button.setAttribute('aria-label', theme === 'dark' ? 'Activar modo claro' : 'Activar modo oscuro');
      button.title = button.getAttribute('aria-label');
    }
    button.addEventListener('click', function () {
      theme = theme === 'dark' ? 'light' : 'dark';
      root.setAttribute('data-bs-theme', theme);
      try { localStorage.setItem(storageKey, theme); } catch (_) { }
      sync();
    });
    sync();
  });
})();
