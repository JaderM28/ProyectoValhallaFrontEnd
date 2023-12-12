using System;
using System.Collections.Generic;

namespace ProyectoValhallaFrontEnd.Models;

public partial class Venta
{
    public int IdVenta { get; set; }

    public DateTime Fecha { get; set; }

    public TimeSpan Hora { get; set; }

    public virtual ICollection<DetallesReservasVenta> DetallesReservasVenta { get; set; } = new List<DetallesReservasVenta>();
}
