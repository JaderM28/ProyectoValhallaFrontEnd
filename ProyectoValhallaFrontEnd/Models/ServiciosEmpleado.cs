using System;
using System.Collections.Generic;

namespace ProyectoValhallaFrontEnd.Models;

public partial class ServiciosEmpleado
{
    public int IdServicioEmpleado { get; set; }

    public int? IdEmpleado { get; set; }

    public int? IdServicio { get; set; }

    public virtual Empleado? IdEmpleadoNavigation { get; set; }

    public virtual Servicio? IdServicioNavigation { get; set; }
}
