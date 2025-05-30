﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Centro_Cultural_Regional_Tlahuelilpan.Models.DBCRUDCORE;
using Centro_Cultural_Regional_Tlahuelilpan.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Centro_Cultural_Regional_Tlahuelilpan.Models.Pdf;

namespace Centro_Cultural_Regional_Tlahuelilpan.Controllers
{
    public class StudentsController : Controller
    {
        private readonly CentroCulturalRegionalTlahuelilpanContext _DBContext;

        public StudentsController(CentroCulturalRegionalTlahuelilpanContext context)
        {
            _DBContext = context;
        }

        // Vista principal - Lista de todos los alumnos
        public IActionResult Students()
        {
            var alumnos = _DBContext.Alumnos.Include(a => a.Expediente).ToList();
            return View(alumnos);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var studentDetails = new StudentDetailsVM
            {
                Alumno = _DBContext.Alumnos.Find(id),
                Expediente = _DBContext.Expedientes.FirstOrDefault(e => e.AlumnoId == id),
                Progresos = _DBContext.ProgresoEstudiantils
                    .Where(p => p.AlumnoId == id)
                    .Include(p => p.Grupo)
                        .ThenInclude(g => g.Taller)
                    .Include(p => p.Grupo)
                        .ThenInclude(g => g.Docente)
                    .ToList(),
                Grupos = _DBContext.Grupos
                    .Where(g => g.ProgresoEstudiantils.Any(p => p.AlumnoId == id))
                    .Include(g => g.Taller)
                    .Include(g => g.Docente)
                    .ToList()
            };

            if (studentDetails.Alumno == null)
            {
                return NotFound();
            }

            return View(studentDetails);
        }

        // Vista Crear / Editar Alumno + Expediente
        [HttpGet]
        public IActionResult CreateEditStudent(int id = 0)
        {
            var vm = new FileStudentVM
            {
                Alumno = new Alumno(),
                Expediente = new Expediente()
            };

            if (id != 0)
            {
                vm.Alumno = _DBContext.Alumnos.Find(id);
                vm.Expediente = _DBContext.Expedientes.FirstOrDefault(e => e.AlumnoId == id) ?? new Expediente();

                // Cargar valores actuales del expediente
                vm.ActaNacimiento = vm.Expediente.ActaNacimiento ?? false;
                vm.Curp = vm.Expediente.Curp ?? false;
                vm.ComprobanteDomicilio = vm.Expediente.ComprobanteDomicilio ?? false;
                vm.Ine = vm.Expediente.Ine ?? false;
                vm.CertificadoMedico = vm.Expediente.CertificadoMedico ?? false;
                vm.ReciboPago = vm.Expediente.ReciboPago ?? false;
                vm.Fotografias = vm.Expediente.Fotografias ?? false;
                vm.DocumentosCompletos = vm.Expediente.DocumentosCompletos ?? false;
                vm.Becado = vm.Expediente.Becado ?? false;
            }

            return View(vm);
        }

        // Acción POST para guardar
        [HttpPost]
        public IActionResult Confirm_CreateEditStudent(FileStudentVM vm)
        {
            if (vm.Alumno.AlumnoId == 0)
            {
                int alumnoId = _DBContext.InsertarAlumno(
                    vm.Alumno.Nombre,
                    vm.Alumno.ApellidoPaterno,
                    vm.Alumno.ApellidoMaterno,
                    vm.Alumno.FechaNacimiento.HasValue ?
                        vm.Alumno.FechaNacimiento.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                    vm.Alumno.Localidad,
                    vm.Alumno.NumeroTelefono,
                    null,
                    vm.Alumno.AdultoResponsable,
                    vm.Alumno.TelefonoResponsable
                );

                _DBContext.InsertarExpediente(
                    alumnoId,
                    vm.ActaNacimiento,
                    vm.Curp,
                    vm.ComprobanteDomicilio,
                    vm.Ine,
                    vm.CertificadoMedico,
                    vm.ReciboPago,
                    vm.Fotografias,
                    vm.Becado
                );

                TempData["SuccessMessage"] = "✅ Alumno guardado exitosamente.";
                TempData["Direction"] = "Admin";

                return RedirectToAction("Details", new { id = alumnoId }); // Aquí usamos el ID real
            }
            else
            {
                // Actualizar alumno usando SP
                _DBContext.ActualizarAlumno(
                    vm.Alumno.AlumnoId,
                    vm.Alumno.Nombre,
                    vm.Alumno.ApellidoPaterno,
                    vm.Alumno.ApellidoMaterno,
                    vm.Alumno.FechaNacimiento.HasValue ?
                        vm.Alumno.FechaNacimiento.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                    vm.Alumno.Localidad,
                    vm.Alumno.NumeroTelefono,
                    null, // CorreoElectronico
                    vm.Alumno.AdultoResponsable,
                    vm.Alumno.TelefonoResponsable
                );

                // Verificar si existe expediente
                var existingExpediente = _DBContext.Expedientes
                    .FirstOrDefault(e => e.AlumnoId == vm.Alumno.AlumnoId);

                if (existingExpediente != null)
                {
                    // Actualizar expediente usando SP
                    _DBContext.ActualizarExpediente(
                        existingExpediente.ExpedienteId,
                        vm.Alumno.AlumnoId,
                        vm.ActaNacimiento,
                        vm.Curp,
                        vm.ComprobanteDomicilio,
                        vm.Ine,
                        vm.CertificadoMedico,
                        vm.ReciboPago,
                        vm.Fotografias,
                        vm.Becado
                    );
                }
                else
                {
                    // Crear nuevo expediente usando SP
                    _DBContext.InsertarExpediente(
                        vm.Alumno.AlumnoId,
                        vm.ActaNacimiento,
                        vm.Curp,
                        vm.ComprobanteDomicilio,
                        vm.Ine,
                        vm.CertificadoMedico,
                        vm.ReciboPago,
                        vm.Fotografias,
                        vm.Becado
                    );
                }
            }

            TempData["SuccessMessage"] = "✅ Alumno guardado exitosamente.";
            TempData["Direction"] = "Admin";
            return RedirectToAction("Details", new { id = vm.Alumno.AlumnoId });
        }

        // Eliminar alumno
        [HttpGet]
        public IActionResult DeleteStudent(int id)
        {
            var alumno = _DBContext.Alumnos
                .Include(a => a.Expediente)
                .Include(a => a.ProgresoEstudiantils)
                .FirstOrDefault(a => a.AlumnoId == id);

            if (alumno == null)
            {
                return NotFound();
            }

            return View(alumno);
        }

        [HttpPost]
        public IActionResult Confirm_DeleteStudent(int AlumnoId)
        {
            var alumno = _DBContext.Alumnos
                .Include(a => a.Expediente)
                .Include(a => a.ProgresoEstudiantils)
                .FirstOrDefault(a => a.AlumnoId == AlumnoId);

            if (alumno == null)
            {
                return NotFound();
            }

            // Eliminar progresos estudiantiles usando SP
            foreach (var progreso in alumno.ProgresoEstudiantils)
            {
                _DBContext.EliminarProgresoEstudiantil(progreso.ProgresoId);
            }

            // Eliminar expediente si existe
            if (alumno.Expediente != null)
            {
                _DBContext.EliminarExpediente(alumno.Expediente.ExpedienteId);
            }

            // Eliminar alumno usando SP
            _DBContext.EliminarAlumno(alumno.AlumnoId);

            return RedirectToAction("Students");
        }

        [HttpGet]
        public IActionResult AddToGroup(int id)
        {
            var alumno = _DBContext.Alumnos.Find(id);
            if (alumno == null)
            {
                return NotFound();
            }

            var vm = new StudentGroupVM
            {
                AlumnoId = alumno.AlumnoId,
                NombreAlumno = $"{alumno.Nombre} {alumno.ApellidoPaterno} {alumno.ApellidoMaterno}"
            };

            // Cargar solo los grupos "En curso"
            vm.GruposDisponibles = _DBContext.Grupos
                .Where(g => g.Estado == "En curso")
                .Select(g => new SelectListItem
                {
                    Value = g.GrupoId.ToString(),
                    Text = $"{g.NombreGrupo} - {g.Taller.NombreTaller} ({g.Horario})"
                }).ToList();

            return View(vm);
        }

        [HttpPost]
        public IActionResult Confirm_AddToGroup(StudentGroupVM vm)
        {
            if (!ModelState.IsValid)
            {
                // Recargar grupos si hay error
                vm.GruposDisponibles = _DBContext.Grupos
                    .Where(g => g.Estado == "En curso")
                    .Select(g => new SelectListItem
                    {
                        Value = g.GrupoId.ToString(),
                        Text = $"{g.NombreGrupo} - {g.Taller.NombreTaller} ({g.Horario})"
                    }).ToList();

                return View("AddToGroup", vm);
            }

            // Verificar si ya está inscrito en ese grupo
            bool existeProgreso = _DBContext.ProgresoEstudiantils
                .Any(p => p.AlumnoId == vm.AlumnoId && p.GrupoId == vm.GrupoId);

            if (existeProgreso)
            {
                TempData["ErrorMessage"] = "⚠️ El alumno ya está inscrito en este grupo.";
                return RedirectToAction("AddToGroup", new { id = vm.AlumnoId });
            }

            // Crear nuevo progreso estudiantil usando SP
            _DBContext.InsertarProgresoEstudiantil(
                vm.AlumnoId,
                vm.GrupoId,
                "Inscrito",
                null, // Calificacion
                null  // Asistencia
            );

            TempData["SuccessMessage"] = "✅ Alumno inscrito exitosamente en el grupo.";
            TempData["Direction"] = "Admin";
            return RedirectToAction("Details", new { id = vm.AlumnoId });
        }

        // Métodos para reportes (sin cambios)
        [HttpGet]
        public IActionResult GraduatesList()
        {
            var egresados = _DBContext.ProgresoEstudiantils
                .Include(p => p.Alumno)
                .Include(p => p.Grupo)
                    .ThenInclude(g => g.Taller)
                .Where(p => p.Estado == "Egresado" && p.Grupo.Estado == "En curso")
                .Select(p => new GraduatesVM
                {
                    Taller = p.Grupo.Taller.NombreTaller,
                    Grupo = p.Grupo.NombreGrupo,
                    NombreCompleto = $"{p.Alumno.Nombre} {p.Alumno.ApellidoPaterno} {p.Alumno.ApellidoMaterno}",
                    Calificacion = p.Calificacion,
                    Asistencia = p.Asistencia,
                    Telefono = p.Alumno.NumeroTelefono,
                    AdultoResponsable = p.Alumno.AdultoResponsable ?? "",
                    TelefonoResponsable = p.Alumno.TelefonoResponsable ?? ""
                })
                .ToList();

            return View(egresados);
        }

        [HttpGet]
        public IActionResult DownloadGraduatesPdf()
        {
            var egresados = _DBContext.ProgresoEstudiantils
                .Include(p => p.Alumno)
                .Include(p => p.Grupo)
                    .ThenInclude(g => g.Taller)
                .Where(p => p.Estado == "Egresado" && p.Grupo.Estado == "En curso")
                .Select(p => new GraduatesVM
                {
                    Taller = p.Grupo.Taller.NombreTaller,
                    Grupo = p.Grupo.NombreGrupo,
                    NombreCompleto = $"{p.Alumno.Nombre} {p.Alumno.ApellidoPaterno} {p.Alumno.ApellidoMaterno}",
                    Calificacion = p.Calificacion,
                    Asistencia = p.Asistencia
                })
                .ToList();

            var pdfGenerator = new GraduatePdfGenerator(egresados);
            byte[] pdfBytes = pdfGenerator.GeneratePdf();

            return File(pdfBytes, "application/pdf", $"Acuse_Egresados_{DateTime.Now:yyyy-MM-dd}.pdf");
        }


        /***************************************************** USO DE LAS VISTAS *****************************************************/
        [HttpGet]
        public IActionResult DownloadGroupsAndWorkshopsPdf()
        {
            var data = _DBContext.VistaGruposTalleres.ToList();
            var pdfGenerator = new GroupsAndWorkshopsPdfGenerator(data);
            byte[] pdfBytes = pdfGenerator.GeneratePdf();
            return File(pdfBytes, "application/pdf", $"Grupos_y_Talleres_{DateTime.Now:yyyy-MM-dd}.pdf");
        }

        [HttpGet]
        public IActionResult DownloadStudentDocumentsPdf()
        {
            var data = _DBContext.VistaAlumnosExpedientes.ToList();
            var pdfGenerator = new StudentDocumentsPdfGenerator(data);
            byte[] pdfBytes = pdfGenerator.GeneratePdf();
            return File(pdfBytes, "application/pdf", $"Expedientes_Alumnos_{DateTime.Now:yyyy-MM-dd}.pdf");
        }

        [HttpGet]
        public IActionResult DownloadProgressPdf()
        {
            var data = _DBContext.VistaProgresoAlumnos.ToList();
            var pdfGenerator = new ProgressPdfGenerator(data);
            byte[] pdfBytes = pdfGenerator.GeneratePdf();
            return File(pdfBytes, "application/pdf", $"Progreso_Alumnos_{DateTime.Now:yyyy-MM-dd}.pdf");
        }
    }
}