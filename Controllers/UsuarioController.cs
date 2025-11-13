using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrocaBateriaWebApp.Data;
using TrocaBateriaWebApp.Models;

namespace TrocaBateriaWebApp.Controllers
{

    [Authorize]
    public class UsuarioController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsuarioController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string? tipoBateria)
        {
            var user = await _userManager.GetUserAsync(User);
            
            if (user == null || user.TipoConta != "Usuario")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var empresasQuery = _context.Users
                .Where(u => u.TipoConta == "Empresa")
                .Include(u => u.Baterias)
                .AsQueryable();

            if (!string.IsNullOrEmpty(tipoBateria))
            {
                empresasQuery = empresasQuery
                    .Where(u => u.Baterias!.Any(b => b.Tipo == tipoBateria));
                ViewBag.TipoBateriaFiltro = tipoBateria;
            }

            var empresas = await empresasQuery.ToListAsync();

            ViewBag.TiposBateria = await _context.Baterias
                .Select(b => b.Tipo)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();

            return View(empresas);
        }


        [HttpGet]
        public async Task<IActionResult> DetalhesEmpresa(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            
            if (user == null || user.TipoConta != "Usuario")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var empresa = await _context.Users
                .Include(u => u.Baterias)
                .FirstOrDefaultAsync(u => u.Id == id && u.TipoConta == "Empresa");

            if (empresa == null)
            {
                return NotFound();
            }

            return View(empresa);
        }

        public async Task<IActionResult> MeusAgendamentos()
        {
            var user = await _userManager.GetUserAsync(User);
            
            if (user == null || user.TipoConta != "Usuario")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var agendamentos = await _context.Agendamentos
                .Include(a => a.Empresa)
                .Where(a => a.UsuarioId == user.Id)
                .OrderByDescending(a => a.DataAgendamento)
                .ToListAsync();

            return View(agendamentos);
        }


        [HttpGet]
        public async Task<IActionResult> Perfil()
        {
            var user = await _userManager.GetUserAsync(User);
            
            if (user == null || user.TipoConta != "Usuario")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Perfil(ApplicationUser model)
        {
            var user = await _userManager.GetUserAsync(User);
            
            if (user == null || user.TipoConta != "Usuario")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            user.NomeCompleto = model.NomeCompleto;
            user.Telefone = model.Telefone;
            user.Email = model.Email;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["Success"] = "Perfil atualizado com sucesso!";
                return RedirectToAction(nameof(Perfil));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(user);
        }
    }
}
