using System;
using System.Collections.Generic;

namespace ProyectoValhallaFrontEnd.Models;

public partial class Cliente
{
    public int IdCliente { get; set; }

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public int Telefono { get; set; }

    public string? TipoDocumento { get; set; }

    public string? NumeroDocumento { get; set; }

    public string? Direccion { get; set; }

    public string? Genero { get; set; }

    public DateTime? FechaNacimiento { get; set; }

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
