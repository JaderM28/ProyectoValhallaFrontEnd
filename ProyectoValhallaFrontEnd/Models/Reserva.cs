using System;
using System.Collections.Generic;

namespace ProyectoValhallaFrontEnd.Models;

public partial class Reserva
{
    public int IdReserva { get; set; }

    public bool EstadoReserva { get; set; }

    public DateTime FechaReserva { get; set; }

    public TimeSpan HoraReserva { get; set; }

    public int ValorServicio { get; set; }

    public int? IdCliente { get; set; }

    public int? IdAgenda { get; set; }

    public int? IdServicio { get; set; }

    public virtual ICollection<DetallesReservasVenta> DetallesReservasVenta { get; set; } = new List<DetallesReservasVenta>();

    public virtual Agenda? IdAgendaNavigation { get; set; }

    public virtual Cliente? IdClienteNavigation { get; set; }

    public virtual Servicio? IdServicioNavigation { get; set; }
}
