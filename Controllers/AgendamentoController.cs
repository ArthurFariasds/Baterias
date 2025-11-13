using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrocaBateriaWebApp.Data;
using TrocaBateriaWebApp.Models;

namespace TrocaBateriaWebApp.Controllers
{

    [Authorize]
    public class AgendamentoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AgendamentoController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Create(string empresaId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null || user.TipoConta != "Usuario")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var empresa = await _context.Users
                .Include(u => u.Baterias)
                .FirstOrDefaultAsync(u => u.Id == empresaId && u.TipoConta == "Empresa");

            if (empresa == null)
            {
                return NotFound();
            }

            ViewBag.Empresa = empresa;
            return View(new Agendamento { EmpresaId = empresaId });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Agendamento agendamento)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null || user.TipoConta != "Usuario")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            // Verifica se a empresa existe
            var empresa = await _context.Users
                .Include(u => u.Baterias)
                .FirstOrDefaultAsync(u => u.Id == agendamento.EmpresaId && u.TipoConta == "Empresa");

            if (empresa == null)
            {
                ModelState.AddModelError(string.Empty, "Empresa não encontrada.");
                var empresaView = await _context.Users
                    .Include(u => u.Baterias)
                    .FirstOrDefaultAsync(u => u.Id == agendamento.EmpresaId);
                ViewBag.Empresa = empresaView;
                return View(agendamento);
            }

            ViewBag.Empresa = empresa;

            //Verificar se a bateria selecionada existe e tem estoque
            var bateriaSelecionada = empresa.Baterias?
                .FirstOrDefault(b => b.Nome == agendamento.TipoBateria && b.Quantidade > 0);

            if (bateriaSelecionada == null)
            {
                ModelState.AddModelError("TipoBateria", "A bateria selecionada não está mais disponível ou está sem estoque.");
                return View(agendamento);
            }

            agendamento.UsuarioId = user.Id;
            ModelState.Remove("UsuarioId");
            ModelState.Remove("EmpresaId");
            TryValidateModel(agendamento);

            if (ModelState.IsValid)
            {
                try
                {
                    var novoAgendamento = new Agendamento
                    {
                        UsuarioId = user.Id,
                        EmpresaId = empresa.Id,
                        TipoBateria = agendamento.TipoBateria,
                        Observacoes = agendamento.Observacoes,
                        Status = "Pendente",
                        DataAgendamento = DateTime.Now,
                        DataAtualizacao = DateTime.Now
                    };

                    _context.Agendamentos.Add(novoAgendamento);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Agendamento realizado com sucesso!";
                    return RedirectToAction("MeusAgendamentos", "Usuario");
                }
                catch (DbUpdateException dbEx)
                {
                    ModelState.AddModelError(string.Empty, $"Erro ao salvar: {dbEx.InnerException?.Message ?? dbEx.Message}");
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Erro inesperado. Tente novamente.");
                }
            }
            else
            {
                var errors = ModelState.Where(kvp => kvp.Value.Errors.Count > 0)
                    .Select(kvp => new { Field = kvp.Key, Errors = kvp.Value.Errors.Select(e => e.ErrorMessage) })
                    .ToList();
                TempData["ModelErrors"] = System.Text.Json.JsonSerializer.Serialize(errors);
            }

            return View(agendamento);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var agendamento = await _context.Agendamentos
                .Include(a => a.Usuario)
                .Include(a => a.Empresa)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (agendamento == null)
            {
                return NotFound();
            }

            if (agendamento.UsuarioId != user.Id && agendamento.EmpresaId != user.Id)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            return View(agendamento);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null || user.TipoConta != "Usuario")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var agendamento = await _context.Agendamentos
                .FirstOrDefaultAsync(a => a.Id == id && a.UsuarioId == user.Id);

            if (agendamento == null)
            {
                return NotFound();
            }

            agendamento.Status = "Cancelado";
            agendamento.DataAtualizacao = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Agendamento cancelado com sucesso!";
            return RedirectToAction("MeusAgendamentos", "Usuario");
        }
    }
}