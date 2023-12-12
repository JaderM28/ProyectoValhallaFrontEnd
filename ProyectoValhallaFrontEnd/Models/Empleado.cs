using System;
using System.Collections.Generic;

namespace ProyectoValhallaFrontEnd.Models;

public partial class Empleado
{
    public int IdEmpleado { get; set; }

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public int Telefono { get; set; }

    public string? TipoDocumento { get; set; }

    public string? NumeroDocumento { get; set; }

    public string? Direccion { get; set; }

    public string? Genero { get; set; }

    public DateTime? FechaNacimiento { get; set; }

    public int? IdServicio { get; set; }

    public virtual ICollection<Agenda> Agenda { get; set; } = new List<Agenda>();

    public virtual Servicio? IdServicioNavigation { get; set; }

    public virtual ICollection<ServiciosEmpleado> ServiciosEmpleados { get; set; } = new List<ServiciosEmpleado>();
}
