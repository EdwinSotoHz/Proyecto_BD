using System;
using System.Collections.Generic;

namespace Centro_Cultural_Regional_Tlahuelilpan.Models.DBCRUDCORE;

public partial class VistaProgresoAlumno
{
    public int ProgresoId { get; set; }

    public string NombreCompleto { get; set; } = null!;

    public string? Estado { get; set; }

    public decimal? Calificacion { get; set; }

    public decimal? Asistencia { get; set; }
}
