using System;
using System.Collections.Generic;

namespace Centro_Cultural_Regional_Tlahuelilpan.Models.DBCRUDCORE;

public partial class VistaGruposTallere
{
    public int GrupoId { get; set; }

    public string NombreGrupo { get; set; } = null!;

    public string Horario { get; set; } = null!;

    public string? Aula { get; set; }

    public string? Estado { get; set; }

    public string NombreTaller { get; set; } = null!;

    public string? Duracion { get; set; }

    public decimal? Precio { get; set; }
}
