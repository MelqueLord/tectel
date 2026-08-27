using AssistenciaTecnica.Application.DTOs;
using AssistenciaTecnica.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssistenciaTecnica.Web.Controllers;

[Authorize]
public class ConfiguracoesController : Controller
{
    private readonly IConfiguracaoEmpresaService _configService;
    private readonly IWebHostEnvironment _env;

    public ConfiguracoesController(IConfiguracaoEmpresaService configService, IWebHostEnvironment env)
    {
        _configService = configService;
        _env = env;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Configurações";

        var config = await _configService.ObterAsync(cancellationToken);

        var form = config is not null
            ? MapearParaForm(config)
            : new ConfiguracaoEmpresaFormDto();

        return View(form);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Salvar(ConfiguracaoEmpresaFormDto form, CancellationToken cancellationToken)
    {
        try
        {
            var config = await _configService.ObterAsync(cancellationToken);
            var logoCaminho = config?.LogoCaminho;

            if (form.LogoArquivo is { Length: > 0 })
            {
                var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsDir);

                var ext = Path.GetExtension(form.LogoArquivo.FileName).ToLowerInvariant();
                var fileName = $"logo{ext}";
                var filePath = Path.Combine(uploadsDir, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await form.LogoArquivo.CopyToAsync(stream, cancellationToken);

                logoCaminho = $"/uploads/{fileName}";
            }

            var dto = new ConfiguracaoEmpresaDto
            {
                Id = form.Id,
                Nome = form.Nome,
                CpfCnpj = form.CpfCnpj,
                Telefone = form.Telefone,
                WhatsApp = form.WhatsApp,
                Endereco = form.Endereco,
                LogoCaminho = logoCaminho,
                TextoPadraoOrdem = form.TextoPadraoOrdem,
                PrazoPadraoGarantiaDias = form.PrazoPadraoGarantiaDias
            };

            await _configService.SalvarAsync(dto, cancellationToken);
            TempData["Sucesso"] = "Configurações salvas com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        ViewData["Title"] = "Configurações";
        return View("Index", form);
    }

    private static ConfiguracaoEmpresaFormDto MapearParaForm(ConfiguracaoEmpresaDto dto)
    {
        return new ConfiguracaoEmpresaFormDto
        {
            Id = dto.Id,
            Nome = dto.Nome,
            CpfCnpj = dto.CpfCnpj,
            Telefone = dto.Telefone,
            WhatsApp = dto.WhatsApp,
            Endereco = dto.Endereco,
            LogoCaminho = dto.LogoCaminho,
            TextoPadraoOrdem = dto.TextoPadraoOrdem,
            PrazoPadraoGarantiaDias = dto.PrazoPadraoGarantiaDias
        };
    }
}
