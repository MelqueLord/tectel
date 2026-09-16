using AssistenciaTecnica.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AssistenciaTecnica.Web.Controllers;

[Authorize(Roles = "Admin")]
public class UsuariosController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UsuariosController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public IActionResult Index()
    {
        ViewData["Title"] = "Usuários";
        var users = _userManager.Users.ToList();
        return View(users);
    }

    [HttpGet]
    public IActionResult Criar()
    {
        ViewData["Title"] = "Novo Usuário";
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(CriarUsuarioViewModel model)
    {
        ViewData["Title"] = "Novo Usuário";

        if (!ModelState.IsValid)
            return View(model);

        var user = new IdentityUser
        {
            UserName = model.Email,
            Email = model.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, model.Senha);

        if (result.Succeeded)
        {
            if (model.IsAdmin)
                await _userManager.AddToRoleAsync(user, "Admin");

            TempData["Sucesso"] = "Usuário criado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Editar(string id)
    {
        ViewData["Title"] = "Editar Usuário";

        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return NotFound();

        var roles = await _userManager.GetRolesAsync(user);

        var model = new EditarUsuarioViewModel
        {
            Id = user.Id,
            Email = user.Email!,
            IsAdmin = roles.Contains("Admin"),
            Ativo = user.LockoutEnd is null || user.LockoutEnd <= DateTimeOffset.UtcNow
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(EditarUsuarioViewModel model)
    {
        ViewData["Title"] = "Editar Usuário";

        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.FindByIdAsync(model.Id);
        if (user is null)
            return NotFound();

        user.Email = model.Email;
        user.UserName = model.Email;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            foreach (var error in updateResult.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return View(model);
        }

        // Atualizar role Admin
        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
        if (model.IsAdmin && !isAdmin)
            await _userManager.AddToRoleAsync(user, "Admin");
        else if (!model.IsAdmin && isAdmin)
            await _userManager.RemoveFromRoleAsync(user, "Admin");

        // Atualizar status ativo/inativo
        if (model.Ativo)
            user.LockoutEnd = null;
        else
            user.LockoutEnd = DateTimeOffset.MaxValue;

        await _userManager.UpdateAsync(user);

        // Atualizar senha se fornecida
        if (!string.IsNullOrWhiteSpace(model.NovaSenha))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var senhaResult = await _userManager.ResetPasswordAsync(user, token, model.NovaSenha);
            if (!senhaResult.Succeeded)
            {
                foreach (var error in senhaResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(model);
            }
        }

        TempData["Sucesso"] = "Usuário atualizado com sucesso!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Excluir(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return NotFound();

        // Não permitir excluir a si mesmo
        if (user.Email == User.Identity?.Name)
        {
            TempData["Erro"] = "Você não pode excluir seu próprio usuário!";
            return RedirectToAction(nameof(Index));
        }

        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
            TempData["Sucesso"] = "Usuário excluído com sucesso!";
        else
            TempData["Erro"] = "Erro ao excluir usuário.";

        return RedirectToAction(nameof(Index));
    }
}
