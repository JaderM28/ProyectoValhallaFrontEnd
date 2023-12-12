using System;
using System.Collections.Generic;

namespace ProyectoValhallaFrontEnd.Models;

public partial class DetallesReservasVenta
{
    public int IdDetallesReservasVentas { get; set; }

    public int? IdReserva { get; set; }

    public int? IdVenta { get; set; }

    public virtual Reserva? IdReservaNavigation { get; set; }

    public virtual Venta? IdVentaNavigation { get; set; }
}
