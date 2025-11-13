using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TrocaBateriaWebApp.Models;

namespace TrocaBateriaWebApp.Controllers
{

    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                // Tenta encontrar o usuário pelo nome de usuário ou e-mail
                var user = await _userManager.FindByNameAsync(model.UserNameOrEmail) 
                    ?? await _userManager.FindByEmailAsync(model.UserNameOrEmail);

                if (user != null)
                {
                    // Tenta fazer o login
                    var result = await _signInManager.PasswordSignInAsync(
                        user.UserName!, 
                        model.Password, 
                        model.RememberMe, 
                        lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        // Redireciona baseado no tipo de conta
                        if (user.TipoConta == "Empresa")
                        {
                            return RedirectToAction("Index", "Empresa");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Usuario");
                        }
                    }
                }

                ModelState.AddModelError(string.Empty, "Usuário ou senha inválidos.");
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Validação específica para empresas
                if (model.TipoConta == "Empresa" && string.IsNullOrWhiteSpace(model.Cnpj))
                {
                    ModelState.AddModelError("Cnpj", "O CNPJ é obrigatório para empresas");
                    return View(model);
                }

                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    NomeCompleto = model.NomeCompleto,
                    TipoConta = model.TipoConta,
                    Cnpj = model.TipoConta == "Empresa" ? model.Cnpj : null,
                    Endereco = model.Endereco,
                    Telefone = model.Telefone,
                    DataCriacao = DateTime.Now
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Faz login automático após registro
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    if (user.TipoConta == "Empresa")
                    {
                        return RedirectToAction("Index", "Empresa");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Usuario");
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
