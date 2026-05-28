using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace clientes;

public class Clientes
{
    public int IdCliente { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string DNI { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime FechaNacimiento { get; set; }

    public TipoPlan Planpago { get; set; }
}