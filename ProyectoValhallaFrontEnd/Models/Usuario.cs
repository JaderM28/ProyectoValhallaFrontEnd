using System;
using System.Collections.Generic;

namespace ProyectoValhallaFrontEnd.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string? Nombres { get; set; }

    public string? Apellidos { get; set; }

    public string? CorreoElectronico { get; set; }

    public string? Username { get; set; }

    public string? Telefono { get; set; }

    public int? IdRol { get; set; }

    public string? Password { get; set; }

    public virtual ICollection<HistorialRefreshToken> HistorialRefreshTokens { get; set; } = new List<HistorialRefreshToken>();

    public virtual Role? IdRolNavigation { get; set; }
}
