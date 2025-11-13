using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrocaBateriaWebApp.Data;
using TrocaBateriaWebApp.Models;

namespace TrocaBateriaWebApp.Controllers
{

    [Authorize]
    public class EmpresaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmpresaController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null || user.TipoConta != "Empresa")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            ViewBag.TotalBaterias = await _context.Baterias
                .Where(b => b.EmpresaId == user.Id)
                .CountAsync();

            ViewBag.AgendamentosPendentes = await _context.Agendamentos
                .Where(a => a.EmpresaId == user.Id && a.Status == "Pendente")
                .CountAsync();

            ViewBag.AgendamentosEmAndamento = await _context.Agendamentos
                .Where(a => a.EmpresaId == user.Id && a.Status == "EmAndamento")
                .CountAsync();

            ViewBag.AgendamentosConcluidos = await _context.Agendamentos
                .Where(a => a.EmpresaId == user.Id && a.Status == "Concluido")
                .CountAsync();

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Perfil()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null || user.TipoConta != "Empresa")
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

            if (user == null || user.TipoConta != "Empresa")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            user.NomeCompleto = model.NomeCompleto;
            user.Endereco = model.Endereco;
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


        public async Task<IActionResult> Agendamentos(string status = null)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null || user.TipoConta != "Empresa")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var query = _context.Agendamentos
                .Include(a => a.Usuario)
                .Where(a => a.EmpresaId == user.Id);

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(a => a.Status == status);
            }

            var agendamentos = await query
                .OrderByDescending(a => a.DataAgendamento)
                .ToListAsync();

            ViewBag.FiltroStatus = status;

            return View(agendamentos);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AtualizarStatusAgendamento(int id, string status)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null || user.TipoConta != "Empresa")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var agendamento = await _context.Agendamentos
                .Include(a => a.Empresa) 
                .ThenInclude(e => e.Baterias)
                .FirstOrDefaultAsync(a => a.Id == id && a.EmpresaId == user.Id);

            if (agendamento == null)
            {
                return NotFound();
            }

            if (status == "Concluido" && agendamento.Status != "Concluido")
            {
                var bateria = agendamento.Empresa.Baterias?
                    .FirstOrDefault(b => b.Nome == agendamento.TipoBateria);

                if (bateria != null)
                {
                    if (bateria.Quantidade > 0)
                    {
                        bateria.Quantidade -= 1; 
                    }
                    else
                    {
                        TempData["Error"] = "Erro: Estoque insuficiente para concluir este agendamento.";
                        return RedirectToAction(nameof(Agendamentos));
                    }
                }
                else
                {
                    TempData["Error"] = "Erro: Bateria não encontrada no estoque.";
                    return RedirectToAction(nameof(Agendamentos));
                }
            }

            if (status != "Concluido" && agendamento.Status == "Concluido")
            {
                var bateria = agendamento.Empresa.Baterias?
                    .FirstOrDefault(b => b.Nome == agendamento.TipoBateria);

                if (bateria != null)
                {
                    bateria.Quantidade += 1; 
                }
            }

            agendamento.Status = status;
            agendamento.DataAtualizacao = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Status do agendamento atualizado com sucesso!";
            return RedirectToAction(nameof(Agendamentos));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelarAgendamento(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null || user.TipoConta != "Empresa")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var agendamento = await _context.Agendamentos
                .Include(a => a.Empresa)
                .ThenInclude(e => e.Baterias)
                .FirstOrDefaultAsync(a => a.Id == id && a.EmpresaId == user.Id);

            if (agendamento == null)
            {
                return NotFound();
            }

            if (agendamento.Status == "Concluido")
            {
                var bateria = agendamento.Empresa.Baterias?
                    .FirstOrDefault(b => b.Nome == agendamento.TipoBateria);

                if (bateria != null)
                {
                    bateria.Quantidade += 1; 
                }
            }

            agendamento.Status = "Cancelado";
            agendamento.DataAtualizacao = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Agendamento cancelado com sucesso!";
            return RedirectToAction(nameof(Agendamentos));
        }
    }
}