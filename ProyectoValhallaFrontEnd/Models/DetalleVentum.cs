using System;
using System.Collections.Generic;

namespace ProyectoValhallaFrontEnd.Models;

public partial class DetalleVentum
{
    public int IdDetalleVenta { get; set; }

    public int? IdVenta { get; set; }

    public string? NombreProducto { get; set; }

    public int? Cantidad { get; set; }

    public decimal? Precio { get; set; }

    public decimal? Total { get; set; }

    public virtual Ventum? IdVentaNavigation { get; set; }
}
