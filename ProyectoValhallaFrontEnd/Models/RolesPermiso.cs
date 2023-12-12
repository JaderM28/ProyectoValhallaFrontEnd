﻿using System;
using System.Collections.Generic;

namespace ProyectoValhallaFrontEnd.Models;

public partial class RolesPermiso
{
    public int IdRolPermiso { get; set; }

    public int? IdPermiso { get; set; }

    public int? IdRol { get; set; }

    public virtual Permiso? IdPermisoNavigation { get; set; }

    public virtual Role? IdRolNavigation { get; set; }
}
