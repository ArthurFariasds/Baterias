using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrocaBateriaWebApp.Data;
using TrocaBateriaWebApp.Models;

namespace TrocaBateriaWebApp.Controllers
{
    [Authorize]
    public class BateriaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BateriaController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.TipoConta != "Empresa")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,Tipo,Descricao,Quantidade")] Bateria bateria)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.TipoConta != "Empresa")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            bateria.EmpresaId = user.Id;
            bateria.DataCadastro = DateTime.UtcNow;

            ModelState.Remove("EmpresaId");
            ModelState.Remove("bateria.EmpresaId");

            TryValidateModel(bateria);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Baterias.Add(bateria);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Bateria cadastrada com sucesso!";
                    return RedirectToAction("MinhasBaterias", "Empresa");
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Ocorreu um erro ao salvar a bateria. Tente novamente mais tarde.");
                }
            }
            else
            {
                var errors = ModelState
                    .Where(kvp => kvp.Value.Errors.Count > 0)
                    .Select(kvp => new { Field = kvp.Key, Errors = kvp.Value.Errors.Select(e => e.ErrorMessage) })
                    .ToList();

                TempData["ModelErrors"] = System.Text.Json.JsonSerializer.Serialize(errors);
            }

            return View(bateria);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.TipoConta != "Empresa")
                return RedirectToAction("AccessDenied", "Account");

            var bateria = await _context.Baterias
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id && b.EmpresaId == user.Id);

            if (bateria == null) return NotFound();

            return View(bateria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Tipo,Descricao,Quantidade")] Bateria bateria)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.TipoConta != "Empresa")
                return RedirectToAction("AccessDenied", "Account");

            if (id != bateria.Id) return BadRequest();

            var existente = await _context.Baterias.AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id && b.EmpresaId == user.Id);
            if (existente == null) return NotFound();

            bateria.EmpresaId = user.Id;
            bateria.DataCadastro = existente.DataCadastro;

            ModelState.Remove("EmpresaId");
            TryValidateModel(bateria);

            if (!ModelState.IsValid)
            {
                TempData["ModelErrors"] = System.Text.Json.JsonSerializer.Serialize(
                    ModelState.Where(kvp => kvp.Value.Errors.Count > 0)
                              .Select(kvp => new { Field = kvp.Key, Errors = kvp.Value.Errors.Select(e => e.ErrorMessage) })
                              .ToList());
                return View(bateria);
            }

            try
            {
                _context.Update(bateria);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Bateria atualizada com sucesso!";
                return RedirectToAction("MinhasBaterias", "Empresa");
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError(string.Empty, "Conflito ao salvar. Tente novamente.");
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Erro ao atualizar. Tente novamente mais tarde.");
            }

            return View(bateria);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.TipoConta != "Empresa")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var bateria = await _context.Baterias
                .FirstOrDefaultAsync(b => b.Id == id && b.EmpresaId == user.Id);

            if (bateria == null)
            {
                return NotFound();
            }

            return View(bateria);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.TipoConta != "Empresa")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var bateria = await _context.Baterias
                .FirstOrDefaultAsync(b => b.Id == id && b.EmpresaId == user.Id);

            if (bateria == null)
            {
                return NotFound();
            }

            try
            {
                _context.Baterias.Remove(bateria);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Bateria removida com sucesso!";
                return RedirectToAction("MinhasBaterias", "Empresa");
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Erro ao remover a bateria. Tente novamente mais tarde.");
                return View(bateria);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var bateria = await _context.Baterias
                .Include(b => b.Empresa)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bateria == null)
            {
                return NotFound();
            }

            return View(bateria);
        }

        [HttpGet]
        public async Task<IActionResult> MinhasBaterias()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.TipoConta != "Empresa")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var baterias = await _context.Baterias
                .Where(b => b.EmpresaId == user.Id)
                .OrderByDescending(b => b.DataCadastro)
                .ToListAsync();

            return View(baterias);
        }
    }
}
