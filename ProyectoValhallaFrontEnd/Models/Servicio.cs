using System;
using System.Collections.Generic;

namespace ProyectoValhallaFrontEnd.Models;

public partial class Servicio
{
    public int IdServicio { get; set; }

    public string Nombre { get; set; } = null!;

    public int Precio { get; set; }

    public TimeSpan DuracionAproximada { get; set; }

    public string Descripcion { get; set; } = null!;

    public int? IdCategoria { get; set; }

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();

    public virtual Categoria? IdCategoriaNavigation { get; set; }

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();

    public virtual ICollection<ServiciosEmpleado> ServiciosEmpleados { get; set; } = new List<ServiciosEmpleado>();
}
