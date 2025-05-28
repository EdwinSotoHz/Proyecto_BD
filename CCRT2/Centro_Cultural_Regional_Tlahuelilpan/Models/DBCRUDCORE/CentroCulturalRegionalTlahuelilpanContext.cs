using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Centro_Cultural_Regional_Tlahuelilpan.Models.DBCRUDCORE;

public partial class CentroCulturalRegionalTlahuelilpanContext : DbContext
{
    /************************************* PROCEDIMIENTOS ALMACENADOS PARA TALLERES *************************************/
    public int InsertarTaller(string nombreTaller, string descripcion = null, decimal precio = 200.00m,
        string duracion = null, string urlImagen = null)
    {
        return Database.ExecuteSqlRaw("EXEC sp_I_Talleres @Nombre_Taller, @Descripcion, @Precio, @Duracion, @URL_Imagen",
            new SqlParameter("@Nombre_Taller", nombreTaller),
            new SqlParameter("@Descripcion", descripcion ?? (object)DBNull.Value),
            new SqlParameter("@Precio", precio),
            new SqlParameter("@Duracion", duracion ?? (object)DBNull.Value),
            new SqlParameter("@URL_Imagen", urlImagen ?? (object)DBNull.Value));
    }

    public int ActualizarTaller(int tallerId, string nombreTaller, string descripcion = null, decimal precio = 200.00m,
        string duracion = null, string urlImagen = null)
    {
        return Database.ExecuteSqlRaw("EXEC sp_U_Talleres @Taller_ID, @Nombre_Taller, @Descripcion, @Precio, @Duracion, @URL_Imagen",
            new SqlParameter("@Taller_ID", tallerId),
            new SqlParameter("@Nombre_Taller", nombreTaller),
            new SqlParameter("@Descripcion", descripcion ?? (object)DBNull.Value),
            new SqlParameter("@Precio", precio),
            new SqlParameter("@Duracion", duracion ?? (object)DBNull.Value),
            new SqlParameter("@URL_Imagen", urlImagen ?? (object)DBNull.Value));
    }

    public int EliminarTaller(int tallerId)
    {
        return Database.ExecuteSqlRaw("EXEC sp_D_Talleres @Taller_ID",
            new SqlParameter("@Taller_ID", tallerId));
    }

    /************************************* PROCEDIMIENTOS ALMACENADOS PARA DOCENTES *************************************/
    public int InsertarDocente(string nombre, string apellidoPaterno, string apellidoMaterno = null,
        string localidad = null, string numeroContacto = null, string email = null)
    {
        var docenteIdParam = new SqlParameter("@DocenteID", SqlDbType.Int) { Direction = ParameterDirection.Output };

        Database.ExecuteSqlRaw(
            "EXEC @DocenteID = sp_I_Docentes @Nombre, @Apellido_Paterno, @Apellido_Materno, @Localidad, @Numero_Contacto, @Email",
            docenteIdParam,
            new SqlParameter("@Nombre", nombre),
            new SqlParameter("@Apellido_Paterno", apellidoPaterno),
            new SqlParameter("@Apellido_Materno", apellidoMaterno ?? (object)DBNull.Value),
            new SqlParameter("@Localidad", localidad ?? (object)DBNull.Value),
            new SqlParameter("@Numero_Contacto", numeroContacto ?? (object)DBNull.Value),
            new SqlParameter("@Email", email ?? (object)DBNull.Value));

        return (int)docenteIdParam.Value;
    }

    public int ActualizarDocente(int docenteId, string nombre, string apellidoPaterno, string apellidoMaterno = null,
        string localidad = null, string numeroContacto = null, string email = null)
    {
        return Database.ExecuteSqlRaw(
            "EXEC sp_U_Docentes @Docente_ID, @Nombre, @Apellido_Paterno, @Apellido_Materno, @Localidad, @Numero_Contacto, @Email",
            new SqlParameter("@Docente_ID", docenteId),
            new SqlParameter("@Nombre", nombre),
            new SqlParameter("@Apellido_Paterno", apellidoPaterno),
            new SqlParameter("@Apellido_Materno", apellidoMaterno ?? (object)DBNull.Value),
            new SqlParameter("@Localidad", localidad ?? (object)DBNull.Value),
            new SqlParameter("@Numero_Contacto", numeroContacto ?? (object)DBNull.Value),
            new SqlParameter("@Email", email ?? (object)DBNull.Value));
    }

    public int EliminarDocente(int docenteId)
    {
        return Database.ExecuteSqlRaw("EXEC sp_D_Docentes @Docente_ID",
            new SqlParameter("@Docente_ID", docenteId));
    }

    /************************************* PROCEDIMIENTOS ALMACENADOS PARA USUARIOS *************************************/
    public int InsertarUsuario(int docenteId, string nombreUsuario, string contraseña, int rolId)
    {
        var usuarioIdParam = new SqlParameter("@UsuarioID", SqlDbType.Int) { Direction = ParameterDirection.Output };

        Database.ExecuteSqlRaw(
            "EXEC @UsuarioID = sp_I_Usuarios @Docente_ID, @Nombre_Usuario, @Contraseña, @Rol_ID",
            usuarioIdParam,
            new SqlParameter("@Docente_ID", docenteId),
            new SqlParameter("@Nombre_Usuario", nombreUsuario),
            new SqlParameter("@Contraseña", contraseña),
            new SqlParameter("@Rol_ID", rolId));

        return (int)usuarioIdParam.Value;
    }

    public int ActualizarUsuario(int usuarioId, int docenteId, string nombreUsuario, string contraseña, int rolId)
    {
        return Database.ExecuteSqlRaw(
            "EXEC sp_U_Usuarios @Usuario_ID, @Docente_ID, @Nombre_Usuario, @Contraseña, @Rol_ID",
            new SqlParameter("@Usuario_ID", usuarioId),
            new SqlParameter("@Docente_ID", docenteId),
            new SqlParameter("@Nombre_Usuario", nombreUsuario),
            new SqlParameter("@Contraseña", contraseña),
            new SqlParameter("@Rol_ID", rolId));
    }

    public int EliminarUsuario(int usuarioId)
    {
        return Database.ExecuteSqlRaw("EXEC sp_D_Usuarios @Usuario_ID",
            new SqlParameter("@Usuario_ID", usuarioId));
    }

    /************************************* PROCEDIMIENTOS ALMACENADOS PARA GRUPOS *************************************/
    public int InsertarGrupo(int tallerId, int docenteId, string nombreGrupo, string horario,
        string aula = null, DateTime? fechaInicio = null, DateTime? fechaFin = null, string estado = "En curso")
    {
        var grupoIdParam = new SqlParameter("@GrupoID", SqlDbType.Int) { Direction = ParameterDirection.Output };

        Database.ExecuteSqlRaw(
            "EXEC @GrupoID = sp_I_Grupos @Taller_ID, @Docente_ID, @Nombre_Grupo, @Horario, @Aula, @Fecha_Inicio, @Fecha_Fin, @Estado",
            grupoIdParam,
            new SqlParameter("@Taller_ID", tallerId),
            new SqlParameter("@Docente_ID", docenteId),
            new SqlParameter("@Nombre_Grupo", nombreGrupo),
            new SqlParameter("@Horario", horario),
            new SqlParameter("@Aula", aula ?? (object)DBNull.Value),
            new SqlParameter("@Fecha_Inicio", fechaInicio ?? (object)DBNull.Value),
            new SqlParameter("@Fecha_Fin", fechaFin ?? (object)DBNull.Value),
            new SqlParameter("@Estado", estado));

        return (int)grupoIdParam.Value;
    }

    public int ActualizarGrupo(int grupoId, int tallerId, int docenteId, string nombreGrupo, string horario,
        string aula = null, DateTime? fechaInicio = null, DateTime? fechaFin = null, string estado = "En curso")
    {
        return Database.ExecuteSqlRaw(
            "EXEC sp_U_Grupos @Grupo_ID, @Taller_ID, @Docente_ID, @Nombre_Grupo, @Horario, @Aula, @Fecha_Inicio, @Fecha_Fin, @Estado",
            new SqlParameter("@Grupo_ID", grupoId),
            new SqlParameter("@Taller_ID", tallerId),
            new SqlParameter("@Docente_ID", docenteId),
            new SqlParameter("@Nombre_Grupo", nombreGrupo),
            new SqlParameter("@Horario", horario),
            new SqlParameter("@Aula", aula ?? (object)DBNull.Value),
            new SqlParameter("@Fecha_Inicio", fechaInicio ?? (object)DBNull.Value),
            new SqlParameter("@Fecha_Fin", fechaFin ?? (object)DBNull.Value),
            new SqlParameter("@Estado", estado));
    }

    public int EliminarGrupo(int grupoId)
    {
        return Database.ExecuteSqlRaw("EXEC sp_D_Grupos @Grupo_ID",
            new SqlParameter("@Grupo_ID", grupoId));
    }

    /************************************* PROCEDIMIENTOS ALMACENADOS PARA ALUMNOS *************************************/
    public int InsertarAlumno(string nombre, string apellidoPaterno, string apellidoMaterno = null,
        DateTime? fechaNacimiento = null, string localidad = null, string numeroTelefono = null,
        string correoElectronico = null, string adultoResponsable = null, string telefonoResponsable = null)
    {
        var alumnoIdParam = new SqlParameter("@AlumnoID", SqlDbType.Int) { Direction = ParameterDirection.Output };

        Database.ExecuteSqlRaw(
            "EXEC @AlumnoID = sp_I_Alumnos @Nombre, @Apellido_Paterno, @Apellido_Materno, @Fecha_Nacimiento, @Localidad, @Numero_Telefono, @Correo_Electronico, @Adulto_Responsable, @Telefono_Responsable",
            alumnoIdParam,
            new SqlParameter("@Nombre", nombre),
            new SqlParameter("@Apellido_Paterno", apellidoPaterno),
            new SqlParameter("@Apellido_Materno", apellidoMaterno ?? (object)DBNull.Value),
            new SqlParameter("@Fecha_Nacimiento", fechaNacimiento ?? (object)DBNull.Value),
            new SqlParameter("@Localidad", localidad ?? (object)DBNull.Value),
            new SqlParameter("@Numero_Telefono", numeroTelefono ?? (object)DBNull.Value),
            new SqlParameter("@Correo_Electronico", correoElectronico ?? (object)DBNull.Value),
            new SqlParameter("@Adulto_Responsable", adultoResponsable ?? (object)DBNull.Value),
            new SqlParameter("@Telefono_Responsable", telefonoResponsable ?? (object)DBNull.Value));

        return (int)alumnoIdParam.Value;
    }

    public int ActualizarAlumno(int alumnoId, string nombre, string apellidoPaterno, string apellidoMaterno = null,
        DateTime? fechaNacimiento = null, string localidad = null, string numeroTelefono = null,
        string correoElectronico = null, string adultoResponsable = null, string telefonoResponsable = null)
    {
        return Database.ExecuteSqlRaw(
            "EXEC sp_U_Alumnos @Alumno_ID, @Nombre, @Apellido_Paterno, @Apellido_Materno, @Fecha_Nacimiento, @Localidad, @Numero_Telefono, @Correo_Electronico, @Adulto_Responsable, @Telefono_Responsable",
            new SqlParameter("@Alumno_ID", alumnoId),
            new SqlParameter("@Nombre", nombre),
            new SqlParameter("@Apellido_Paterno", apellidoPaterno),
            new SqlParameter("@Apellido_Materno", apellidoMaterno ?? (object)DBNull.Value),
            new SqlParameter("@Fecha_Nacimiento", fechaNacimiento ?? (object)DBNull.Value),
            new SqlParameter("@Localidad", localidad ?? (object)DBNull.Value),
            new SqlParameter("@Numero_Telefono", numeroTelefono ?? (object)DBNull.Value),
            new SqlParameter("@Correo_Electronico", correoElectronico ?? (object)DBNull.Value),
            new SqlParameter("@Adulto_Responsable", adultoResponsable ?? (object)DBNull.Value),
            new SqlParameter("@Telefono_Responsable", telefonoResponsable ?? (object)DBNull.Value));
    }

    public int EliminarAlumno(int alumnoId)
    {
        return Database.ExecuteSqlRaw("EXEC sp_D_Alumnos @Alumno_ID",
            new SqlParameter("@Alumno_ID", alumnoId));
    }

    /************************************* PROCEDIMIENTOS ALMACENADOS PARA EXPEDIENTES *************************************/
    public int InsertarExpediente(int alumnoId, bool actaNacimiento = false, bool curp = false,
        bool comprobanteDomicilio = false, bool ine = false, bool certificadoMedico = false,
        bool reciboPago = false, bool fotografias = false, bool becado = false)
    {
        var expedienteIdParam = new SqlParameter("@ExpedienteID", SqlDbType.Int) { Direction = ParameterDirection.Output };

        Database.ExecuteSqlRaw(
            "EXEC @ExpedienteID = sp_I_Expedientes @Alumno_ID, @Acta_Nacimiento, @CURP, @Comprobante_Domicilio, @INE, @Certificado_Medico, @Recibo_Pago, @Fotografias, @Becado",
            expedienteIdParam,
            new SqlParameter("@Alumno_ID", alumnoId),
            new SqlParameter("@Acta_Nacimiento", actaNacimiento),
            new SqlParameter("@CURP", curp),
            new SqlParameter("@Comprobante_Domicilio", comprobanteDomicilio),
            new SqlParameter("@INE", ine),
            new SqlParameter("@Certificado_Medico", certificadoMedico),
            new SqlParameter("@Recibo_Pago", reciboPago),
            new SqlParameter("@Fotografias", fotografias),
            new SqlParameter("@Becado", becado));

        return (int)expedienteIdParam.Value;
    }

    public int ActualizarExpediente(int expedienteId, int alumnoId, bool actaNacimiento = false, bool curp = false,
        bool comprobanteDomicilio = false, bool ine = false, bool certificadoMedico = false,
        bool reciboPago = false, bool fotografias = false, bool becado = false)
    {
        return Database.ExecuteSqlRaw(
            "EXEC sp_U_Expedientes @Expediente_ID, @Alumno_ID, @Acta_Nacimiento, @CURP, @Comprobante_Domicilio, @INE, @Certificado_Medico, @Recibo_Pago, @Fotografias, @Becado",
            new SqlParameter("@Expediente_ID", expedienteId),
            new SqlParameter("@Alumno_ID", alumnoId),
            new SqlParameter("@Acta_Nacimiento", actaNacimiento),
            new SqlParameter("@CURP", curp),
            new SqlParameter("@Comprobante_Domicilio", comprobanteDomicilio),
            new SqlParameter("@INE", ine),
            new SqlParameter("@Certificado_Medico", certificadoMedico),
            new SqlParameter("@Recibo_Pago", reciboPago),
            new SqlParameter("@Fotografias", fotografias),
            new SqlParameter("@Becado", becado));
    }

    public int EliminarExpediente(int expedienteId)
    {
        return Database.ExecuteSqlRaw("EXEC sp_D_Expedientes @Expediente_ID",
            new SqlParameter("@Expediente_ID", expedienteId));
    }

    /************************************* PROCEDIMIENTOS ALMACENADOS PARA PROGRESO ESTUDIANTIL *************************************/
    public int InsertarProgresoEstudiantil(int alumnoId, int grupoId, string estado = "Inscrito",
        decimal? calificacion = null, decimal? asistencia = null)
    {
        var progresoIdParam = new SqlParameter("@ProgresoID", SqlDbType.Int) { Direction = ParameterDirection.Output };

        Database.ExecuteSqlRaw(
            "EXEC @ProgresoID = sp_I_Progreso_Estudiantil @Alumno_ID, @Grupo_ID, @Estado, @Calificacion, @Asistencia",
            progresoIdParam,
            new SqlParameter("@Alumno_ID", alumnoId),
            new SqlParameter("@Grupo_ID", grupoId),
            new SqlParameter("@Estado", estado),
            new SqlParameter("@Calificacion", calificacion ?? (object)DBNull.Value),
            new SqlParameter("@Asistencia", asistencia ?? (object)DBNull.Value));

        return (int)progresoIdParam.Value;
    }

    public int ActualizarProgresoEstudiantil(int progresoId, int alumnoId, int grupoId, string estado = "Inscrito",
        decimal? calificacion = null, decimal? asistencia = null)
    {
        return Database.ExecuteSqlRaw(
            "EXEC sp_U_Progreso_Estudiantil @Progreso_ID, @Alumno_ID, @Grupo_ID, @Estado, @Calificacion, @Asistencia",
            new SqlParameter("@Progreso_ID", progresoId),
            new SqlParameter("@Alumno_ID", alumnoId),
            new SqlParameter("@Grupo_ID", grupoId),
            new SqlParameter("@Estado", estado),
            new SqlParameter("@Calificacion", calificacion ?? (object)DBNull.Value),
            new SqlParameter("@Asistencia", asistencia ?? (object)DBNull.Value));
    }

    public int EliminarProgresoEstudiantil(int progresoId)
    {
        return Database.ExecuteSqlRaw("EXEC sp_D_Progreso_Estudiantil @Progreso_ID",
            new SqlParameter("@Progreso_ID", progresoId));
    } 

    /************************************* PROCEDIMIENTOS ALMACENADOS PARA ROLES *************************************/
    public int InsertarRol(string nombre, string descripcion = null)
    {
        var rolIdParam = new SqlParameter("@RolID", SqlDbType.Int) { Direction = ParameterDirection.Output };

        Database.ExecuteSqlRaw(
            "EXEC @RolID = sp_I_Roles @Nombre, @Descripcion",
            rolIdParam,
            new SqlParameter("@Nombre", nombre),
            new SqlParameter("@Descripcion", descripcion ?? (object)DBNull.Value));

        return (int)rolIdParam.Value;
    }

    public int ActualizarRol(int rolId, string nombre, string descripcion = null)
    {
        return Database.ExecuteSqlRaw(
            "EXEC sp_U_Roles @Rol_ID, @Nombre, @Descripcion",
            new SqlParameter("@Rol_ID", rolId),
            new SqlParameter("@Nombre", nombre),
            new SqlParameter("@Descripcion", descripcion ?? (object)DBNull.Value));
    }

    public int EliminarRol(int rolId)
    {
        return Database.ExecuteSqlRaw("EXEC sp_D_Roles @Rol_ID",
            new SqlParameter("@Rol_ID", rolId));
    }



    public CentroCulturalRegionalTlahuelilpanContext()
    {
    }

    public CentroCulturalRegionalTlahuelilpanContext(DbContextOptions<CentroCulturalRegionalTlahuelilpanContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Alumno> Alumnos { get; set; }

    public virtual DbSet<Docente> Docentes { get; set; }

    public virtual DbSet<Expediente> Expedientes { get; set; }

    public virtual DbSet<Grupo> Grupos { get; set; }

    public virtual DbSet<ProgresoEstudiantil> ProgresoEstudiantils { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Tallere> Talleres { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<VistaAlumnosExpediente> VistaAlumnosExpedientes { get; set; }

    public virtual DbSet<VistaGruposTallere> VistaGruposTalleres { get; set; }

    public virtual DbSet<VistaProgresoAlumno> VistaProgresoAlumnos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // La configuración ahora se hace desde Program.cs
        if (!optionsBuilder.IsConfigured)
        {
            // Esto es opcional, solo para casos donde el contexto se crea sin inyección de dependencias
            // Pero con tu configuración actual en Program.cs, no debería ser necesario
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Alumno>(entity =>
        {
            entity.HasIndex(e => new { e.ApellidoPaterno, e.ApellidoMaterno, e.Nombre }, "IDX_Alumnos_Nombre");
            entity.Property(e => e.AlumnoId).HasColumnName("Alumno_ID");
            entity.Property(e => e.AdultoResponsable)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Adulto_Responsable");
            entity.Property(e => e.ApellidoMaterno)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Apellido_Materno");
            entity.Property(e => e.ApellidoPaterno)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Apellido_Paterno");
            entity.Property(e => e.CorreoElectronico)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Correo_Electronico");
            entity.Property(e => e.Edad).HasComputedColumnSql("(datediff(year,[Fecha_Nacimiento],getdate())-case when dateadd(year,datediff(year,[Fecha_Nacimiento],getdate()),[Fecha_Nacimiento])>getdate() then (1) else (0) end)", false);
            entity.Property(e => e.FechaNacimiento).HasColumnName("Fecha_Nacimiento");
            entity.Property(e => e.Localidad)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NumeroTelefono)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("Numero_Telefono");
            entity.Property(e => e.TelefonoResponsable)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("Telefono_Responsable");
        });

        modelBuilder.Entity<Docente>(entity =>
        {
            entity.HasIndex(e => new { e.ApellidoPaterno, e.ApellidoMaterno, e.Nombre }, "IDX_Docentes_Nombre");

            entity.Property(e => e.DocenteId).HasColumnName("Docente_ID");
            entity.Property(e => e.ApellidoMaterno)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Apellido_Materno");
            entity.Property(e => e.ApellidoPaterno)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Apellido_Paterno");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Localidad)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NumeroContacto)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("Numero_Contacto");
        });

        modelBuilder.Entity<Expediente>(entity =>
        {
            entity.ToTable(tb => tb.HasTrigger("TR_Actualizar_Documentos_Completos"));
            entity.HasIndex(e => e.DocumentosCompletos, "IDX_Expedientes_Completos").HasFilter("([Documentos_Completos]=(1))");

            entity.HasIndex(e => e.AlumnoId, "UQ_Alumno_Expediente").IsUnique();

            entity.Property(e => e.ExpedienteId).HasColumnName("Expediente_ID");
            entity.Property(e => e.ActaNacimiento)
                .HasDefaultValue(false)
                .HasColumnName("Acta_Nacimiento");
            entity.Property(e => e.AlumnoId).HasColumnName("Alumno_ID");
            entity.Property(e => e.CertificadoMedico)
                .HasDefaultValue(false)
                .HasColumnName("Certificado_Medico");
            entity.Property(e => e.ComprobanteDomicilio)
                .HasDefaultValue(false)
                .HasColumnName("Comprobante_Domicilio");
            entity.Property(e => e.Curp)
                .HasDefaultValue(false)
                .HasColumnName("CURP");
            entity.Property(e => e.DocumentosCompletos)
                .HasDefaultValue(false)
                .HasColumnName("Documentos_Completos");
            entity.Property(e => e.Fotografias).HasDefaultValue(false);
            entity.Property(e => e.Ine)
                .HasDefaultValue(false)
                .HasColumnName("INE");
            entity.Property(e => e.ReciboPago)
                .HasDefaultValue(false)
                .HasColumnName("Recibo_Pago");

            entity.HasOne(d => d.Alumno).WithOne(p => p.Expediente)
                .HasForeignKey<Expediente>(d => d.AlumnoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Expedientes_Alumnos");
        });

        modelBuilder.Entity<Grupo>(entity =>
        {
            entity.ToTable(tb => tb.HasTrigger("TR_Actualizar_Estado_Grupo"));

            entity.HasIndex(e => e.Horario, "IDX_Grupos_Horario");

            entity.HasIndex(e => new { e.TallerId, e.NombreGrupo }, "UQ_Taller_Grupo").IsUnique();

            entity.Property(e => e.GrupoId).HasColumnName("Grupo_ID");
            entity.Property(e => e.Aula)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.DocenteId).HasColumnName("Docente_ID");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("En curso");
            entity.Property(e => e.FechaFin).HasColumnName("Fecha_Fin");
            entity.Property(e => e.FechaInicio).HasColumnName("Fecha_Inicio");
            entity.Property(e => e.Horario)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.NombreGrupo)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Nombre_Grupo");
            entity.Property(e => e.TallerId).HasColumnName("Taller_ID");

            entity.HasOne(d => d.Docente).WithMany(p => p.Grupos)
                .HasForeignKey(d => d.DocenteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Grupos_Docentes");

            entity.HasOne(d => d.Taller).WithMany(p => p.Grupos)
                .HasForeignKey(d => d.TallerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Grupos_Talleres");
        });

        modelBuilder.Entity<ProgresoEstudiantil>(entity =>
        {
            entity.HasKey(e => e.ProgresoId);

            entity.ToTable("Progreso_Estudiantil", tb => tb.HasTrigger("TR_Actualizar_Egresado"));

            entity.HasIndex(e => e.AlumnoId, "IDX_Progreso_Alumno");

            entity.HasIndex(e => e.Estado, "IDX_Progreso_Egresados").HasFilter("([Estado]='Egresado')");

            entity.HasIndex(e => e.GrupoId, "IDX_Progreso_Grupo");

            entity.HasIndex(e => new { e.AlumnoId, e.GrupoId }, "UQ_Alumno_Grupo").IsUnique();

            entity.Property(e => e.ProgresoId).HasColumnName("Progreso_ID");
            entity.Property(e => e.AlumnoId).HasColumnName("Alumno_ID");
            entity.Property(e => e.Asistencia).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Calificacion).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Inscrito");
            entity.Property(e => e.GrupoId).HasColumnName("Grupo_ID");

            entity.HasOne(d => d.Alumno).WithMany(p => p.ProgresoEstudiantils)
                .HasForeignKey(d => d.AlumnoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Progreso_Alumnos");

            entity.HasOne(d => d.Grupo).WithMany(p => p.ProgresoEstudiantils)
                .HasForeignKey(d => d.GrupoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Progreso_Grupos");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RolId);

            entity.HasIndex(e => e.Nombre, "UQ_Nombre_Rol").IsUnique();

            entity.Property(e => e.RolId).HasColumnName("Rol_ID");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Tallere>(entity =>
        {
            entity.HasKey(e => e.TallerId);

            entity.HasIndex(e => e.NombreTaller, "IDX_Talleres_Nombre");

            entity.HasIndex(e => e.NombreTaller, "UQ_Nombre_Taller").IsUnique();

            entity.Property(e => e.TallerId).HasColumnName("Taller_ID");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Duracion)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NombreTaller)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Nombre_Taller");
            entity.Property(e => e.Precio)
                .HasDefaultValue(200.00m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UrlImagen)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("URL_Imagen");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.UsuarioId);

            entity.HasIndex(e => e.NombreUsuario, "UQ_Nombre_Usuario").IsUnique();

            entity.Property(e => e.Contraseña)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.DocenteId).HasColumnName("Docente_ID");
            entity.Property(e => e.NombreUsuario)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Nombre_Usuario");
            entity.Property(e => e.RolId).HasColumnName("Rol_ID");
            entity.Property(e => e.UsuarioId)
                .ValueGeneratedOnAdd()
                .HasColumnName("Usuario_ID");

            entity.HasOne(d => d.Docente).WithMany()
                .HasForeignKey(d => d.DocenteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuarios_Docentes");

            entity.HasOne(d => d.Rol).WithMany()
                .HasForeignKey(d => d.RolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuarios_Roles");
        });


        /******************************************* VISTAS *******************************************/

        modelBuilder.Entity<VistaAlumnosExpediente>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vista_Alumnos_Expedientes");

            entity.Property(e => e.AlumnoId).HasColumnName("Alumno_ID");
            entity.Property(e => e.ApellidoMaterno)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Apellido_Materno");
            entity.Property(e => e.ApellidoPaterno)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Apellido_Paterno");
            entity.Property(e => e.DocumentosCompletos).HasColumnName("Documentos_Completos");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });
        modelBuilder.Entity<VistaGruposTallere>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vista_Grupos_Talleres");

            entity.Property(e => e.Aula)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Duracion)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.GrupoId).HasColumnName("Grupo_ID");
            entity.Property(e => e.Horario)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.NombreGrupo)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Nombre_Grupo");
            entity.Property(e => e.NombreTaller)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Nombre_Taller");
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<VistaProgresoAlumno>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vista_Progreso_Alumnos");

            entity.Property(e => e.Asistencia).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Calificacion).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.NombreCompleto)
                .HasMaxLength(101)
                .IsUnicode(false)
                .HasColumnName("Nombre_Completo");
            entity.Property(e => e.ProgresoId).HasColumnName("Progreso_ID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
