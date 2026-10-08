using System;
namespace gym1._1
{
    public class PaginaSuperAdministrador : PaginaProtegida
    {
        protected override bool AutorizarRol(string rol)
        {
            return string.Equals(rol.Trim(), "SuperAdministrador", StringComparison.OrdinalIgnoreCase);
        }
    }
}
