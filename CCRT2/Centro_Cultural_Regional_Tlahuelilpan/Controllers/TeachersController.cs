using Microsoft.AspNetCore.Mvc;
using Centro_Cultural_Regional_Tlahuelilpan.Models.DBCRUDCORE;
using Centro_Cultural_Regional_Tlahuelilpan.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;
using Microsoft.Data.SqlClient;

namespace Centro_Cultural_Regional_Tlahuelilpan.Controllers
{
    public class TeachersController : Controller
    {
        private readonly CentroCulturalRegionalTlahuelilpanContext _DBContext;

        public TeachersController(CentroCulturalRegionalTlahuelilpanContext context)
        {
            _DBContext = context;
        }

        public IActionResult Teachers()
        {
            List<Usuario> list = _DBContext.Usuarios.Include(u => u.Rol).Include(u => u.Docente).ToList();
            return View(list);
        }

        [HttpGet]
        public IActionResult CreateEditTeacherUser(int UsuarioId)
        {
            TeachersUsersVM vm = new TeachersUsersVM()
            {
                objUsuario = new Usuario(),
                objDocente = new Docente(),
                lstRoles = _DBContext.Roles.Select(r => new SelectListItem
                {
                    Text = r.Nombre,
                    Value = r.RolId.ToString()
                }).ToList()
            };

            if (UsuarioId != 0)
            {
                vm.objUsuario = _DBContext.Usuarios.Include(u => u.Docente).FirstOrDefault(u => u.UsuarioId == UsuarioId);

                if (vm.objUsuario != null)
                {
                    vm.objDocente = vm.objUsuario.Docente;
                }
            }

            return View(vm);
        }

        [HttpPost]
        public IActionResult Confirm_CreateEditTeacherUser(TeachersUsersVM vm)
        {
            if (!string.IsNullOrEmpty(vm.objUsuario.Contraseña))
            {
                if (vm.objUsuario.Contraseña != vm.ConfirmarClave)
                {
                    Debug.WriteLine("\n\n Las contraseñas no coinciden \n\n");
                    vm.lstRoles = _DBContext.Roles.Select(r => new SelectListItem
                    {
                        Text = r.Nombre,
                        Value = r.RolId.ToString()
                    }).ToList();
                    return View("CreateEditTeacherUser", vm);
                }

                // Hash de la contraseña
                vm.objUsuario.Contraseña = Convert.ToHexString(
                    SHA256.HashData(Encoding.UTF8.GetBytes(vm.objUsuario.Contraseña)));
            }

            if (vm.objUsuario.UsuarioId == 0)
            {
                // Crear nuevo docente usando SP
                int docenteId = _DBContext.InsertarDocente(
                    vm.objDocente.Nombre,
                    vm.objDocente.ApellidoPaterno,
                    vm.objDocente.ApellidoMaterno,
                    vm.objDocente.Localidad,
                    vm.objDocente.NumeroContacto,
                    vm.objDocente.Email
                );

                // Crear usuario asociado usando SP
                _DBContext.InsertarUsuario(
                    docenteId,
                    vm.objUsuario.NombreUsuario,
                    vm.objUsuario.Contraseña,
                    vm.objUsuario.RolId
                );
            }
            else
            {
                // Actualizar docente usando SP
                _DBContext.ActualizarDocente(
                    vm.objDocente.DocenteId,
                    vm.objDocente.Nombre,
                    vm.objDocente.ApellidoPaterno,
                    vm.objDocente.ApellidoMaterno,
                    vm.objDocente.Localidad,
                    vm.objDocente.NumeroContacto,
                    vm.objDocente.Email
                );

                // Actualizar usuario usando SP
                _DBContext.ActualizarUsuario(
                    vm.objUsuario.UsuarioId,
                    vm.objDocente.DocenteId,
                    vm.objUsuario.NombreUsuario,
                    vm.objUsuario.Contraseña,
                    vm.objUsuario.RolId
                );
            }

            return RedirectToAction("Teachers");
        }

        [HttpGet]
        public IActionResult DeleteTeacher(int UsuarioId)
        {
            var usuario = _DBContext.Usuarios
                .Include(u => u.Docente)
                .FirstOrDefault(u => u.UsuarioId == UsuarioId);

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        [HttpPost]
        public IActionResult Confirm_DeleteTeacher(int UsuarioId)
        {
            var usuario = _DBContext.Usuarios
                .Include(u => u.Docente)
                .FirstOrDefault(u => u.UsuarioId == UsuarioId);

            if (usuario == null)
            {
                return NotFound();
            }

            // Verificar si el docente tiene grupos asignados
            bool tieneGrupos = _DBContext.Grupos.Any(g => g.DocenteId == usuario.DocenteId);

            if (tieneGrupos)
            {
                TempData["ErrorMessage"] = "No se puede eliminar el docente porque tiene grupos asignados";
                return RedirectToAction("Teachers");
            }

            // Eliminar usuario y docente usando SPs
            _DBContext.EliminarUsuario(usuario.UsuarioId);
            _DBContext.EliminarDocente(usuario.DocenteId);

            return RedirectToAction("Teachers");
        }
    }
}