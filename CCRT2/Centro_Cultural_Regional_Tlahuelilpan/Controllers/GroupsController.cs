﻿using Microsoft.AspNetCore.Mvc;
using Centro_Cultural_Regional_Tlahuelilpan.Models.DBCRUDCORE;
using Centro_Cultural_Regional_Tlahuelilpan.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Centro_Cultural_Regional_Tlahuelilpan.Controllers
{
    public class GroupsController : Controller
    {
        private readonly CentroCulturalRegionalTlahuelilpanContext _DBContext;

        public GroupsController(CentroCulturalRegionalTlahuelilpanContext context)
        {
            _DBContext = context;
        }

        public IActionResult Groups()
        {
            var grupos = _DBContext.Grupos.Include(g => g.Taller).Include(g => g.Docente).ToList();
            return View(grupos);
        }

        [HttpGet]
        public IActionResult CreateEditGroup(int grupoId = 0)
        {
            GrupoVM vm = new GrupoVM
            {
                objGrupo = new Grupo(),
                lstTalleres = _DBContext.Talleres.Select(t => new SelectListItem
                {
                    Text = t.NombreTaller,
                    Value = t.TallerId.ToString()
                }).ToList(),
                lstDocentes = _DBContext.Docentes.Select(d => new SelectListItem
                {
                    Text = $"{d.Nombre} {d.ApellidoPaterno}",
                    Value = d.DocenteId.ToString()
                }).ToList()
            };

            if (grupoId != 0)
            {
                vm.objGrupo = _DBContext.Grupos.Find(grupoId);
            }

            return View(vm);
        }

        [HttpPost]
        public IActionResult Confirm_CreateEditGroup(GrupoVM vm)
        {
            if (vm.objGrupo.GrupoId == 0)
            {
                // Insertar usando SP con conversión de DateOnly a DateTime
                _DBContext.InsertarGrupo(
                    vm.objGrupo.TallerId,
                    vm.objGrupo.DocenteId,
                    vm.objGrupo.NombreGrupo,
                    vm.objGrupo.Horario,
                    vm.objGrupo.Aula,
                    vm.objGrupo.FechaInicio.HasValue ?
                        vm.objGrupo.FechaInicio.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                    vm.objGrupo.FechaFin.HasValue ?
                        vm.objGrupo.FechaFin.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                    vm.objGrupo.Estado
                );
            }
            else
            {
                // Actualizar usando SP con conversión de DateOnly a DateTime
                _DBContext.ActualizarGrupo(
                    vm.objGrupo.GrupoId,
                    vm.objGrupo.TallerId,
                    vm.objGrupo.DocenteId,
                    vm.objGrupo.NombreGrupo,
                    vm.objGrupo.Horario,
                    vm.objGrupo.Aula,
                    vm.objGrupo.FechaInicio.HasValue ?
                        vm.objGrupo.FechaInicio.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                    vm.objGrupo.FechaFin.HasValue ?
                        vm.objGrupo.FechaFin.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                    vm.objGrupo.Estado
                );
            }

            return RedirectToAction("Groups");

            // Recargar listas si hay error
            vm.lstTalleres = _DBContext.Talleres.Select(t => new SelectListItem
            {
                Text = t.NombreTaller,
                Value = t.TallerId.ToString()
            }).ToList();

            vm.lstDocentes = _DBContext.Docentes.Select(d => new SelectListItem
            {
                Text = $"{d.Nombre} {d.ApellidoPaterno}",
                Value = d.DocenteId.ToString()
            }).ToList();

            return View("CreateEditGroup", vm);
        }

        [HttpGet]
        public IActionResult DeleteGroup(int grupoId)
        {
            var grupo = _DBContext.Grupos
                .Include(g => g.Taller)
                .Include(g => g.Docente)
                .FirstOrDefault(g => g.GrupoId == grupoId);

            if (grupo == null)
            {
                return NotFound();
            }

            return View(grupo);
        }

        [HttpPost]
        public IActionResult Confirm_DeleteGroup(int grupoId)
        {
            var grupo = _DBContext.Grupos.Find(grupoId);

            if (grupo == null)
            {
                return NotFound();
            }

            // Verificar si tiene progresos estudiantiles
            bool tieneProgresos = _DBContext.ProgresoEstudiantils.Any(p => p.GrupoId == grupoId);

            if (tieneProgresos)
            {
                TempData["ErrorMessage"] = "No se puede eliminar el grupo porque tiene progresos estudiantiles asociados";
                return RedirectToAction("Delete", new { grupoId = grupoId });
            }

            // Eliminar usando SP
            _DBContext.EliminarGrupo(grupoId);

            return RedirectToAction("Groups");
        }
    }
}