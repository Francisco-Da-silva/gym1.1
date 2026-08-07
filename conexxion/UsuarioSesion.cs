namespace Conexxion
{
    public class UsuarioSesion
    {
        public int IdUsuario { get; set; }
        public int IdGimnasio { get; set; }

        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;

        public string NombreGimnasio { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;
        public string PasswordSalt { get; set; } = string.Empty;
    }
}
