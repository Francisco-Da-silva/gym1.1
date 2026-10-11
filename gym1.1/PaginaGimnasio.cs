using System;

namespace gym1._1
{
    // Las operaciones de socios y cuotas pertenecen exclusivamente a un gimnasio.
    public class PaginaGimnasio : PaginaProtegida
    {
        protected override bool AutorizarRol(string rol)
        {
            return string.Equals(rol.Trim(), "Administrador", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(rol.Trim(), "Gimnasio", StringComparison.OrdinalIgnoreCase);
        }
    }
}
