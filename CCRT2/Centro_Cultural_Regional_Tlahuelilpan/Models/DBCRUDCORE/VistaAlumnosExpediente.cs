using System;
using System.Collections.Generic;

namespace Centro_Cultural_Regional_Tlahuelilpan.Models.DBCRUDCORE;

public partial class VistaAlumnosExpediente
{
    public int AlumnoId { get; set; }

    public string Nombre { get; set; } = null!;

    public string ApellidoPaterno { get; set; } = null!;

    public string? ApellidoMaterno { get; set; }

    public bool? Becado { get; set; }

    public bool? DocumentosCompletos { get; set; }
}
