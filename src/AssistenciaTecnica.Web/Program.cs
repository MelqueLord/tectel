using System.Threading.RateLimiting;
using AssistenciaTecnica.Application.Interfaces;
using AssistenciaTecnica.Application.Services;
using AssistenciaTecnica.Domain.Entities;
using AssistenciaTecnica.Infrastructure.Data;
using AssistenciaTecnica.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("login", context =>
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5,
            Window = TimeSpan.FromMinutes(1),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0
        });
    });

    options.AddPolicy("register", context =>
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 3,
            Window = TimeSpan.FromMinutes(5),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0
        });
    });
});

// Database
var dbProvider = builder.Configuration.GetValue<string>("DatabaseProvider") ?? "Sqlite";

if (dbProvider.Equals("MySql", StringComparison.OrdinalIgnoreCase))
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(connectionString))
        throw new InvalidOperationException("Connection string 'DefaultConnection' not found for MySQL.");

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
}
else if (!dbProvider.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
{
    var sqlitePath = builder.Configuration.GetValue<string>("SqlitePath") ?? "AssistenciaTecnica.db";
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite($"Data Source={sqlitePath}"));
}

// Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

// Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>();

// Repositories
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IAparelhoRepository, AparelhoRepository>();
builder.Services.AddScoped<IOrdemServicoRepository, OrdemServicoRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IMovimentacaoEstoqueRepository, MovimentacaoEstoqueRepository>();
builder.Services.AddScoped<IPagamentoRepository, PagamentoRepository>();
builder.Services.AddScoped<IConfiguracaoEmpresaRepository, ConfiguracaoEmpresaRepository>();
builder.Services.AddScoped<IAgendamentoRepository, AgendamentoRepository>();
builder.Services.AddScoped<IOrcamentoRepository, OrcamentoRepository>();
builder.Services.AddScoped<ITecnicoRepository, TecnicoRepository>();

// Services
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IAparelhoService, AparelhoService>();
builder.Services.AddScoped<IOrdemServicoService, OrdemServicoService>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<IPagamentoService, PagamentoService>();
builder.Services.AddScoped<IConfiguracaoEmpresaService, ConfiguracaoEmpresaService>();
builder.Services.AddScoped<INotificacaoService, NotificacaoService>();
builder.Services.AddScoped<IAgendamentoService, AgendamentoService>();
builder.Services.AddScoped<IRelatorioService, RelatorioService>();
builder.Services.AddScoped<IOrcamentoService, OrcamentoService>();
builder.Services.AddScoped<ITecnicoService, TecnicoService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Auto-migrate on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (db.Database.IsMySql())
        db.Database.EnsureCreated();
    else
        db.Database.Migrate();

    // Seed admin user
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));

    var adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL") ?? "admin@assistencia.com";
    var adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD")
        ?? throw new InvalidOperationException(
            "ADMIN_PASSWORD environment variable is required. Set it before starting the application.");

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser is null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };
        await userManager.CreateAsync(adminUser, adminPassword);
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }

    // Seed demo data (only in Development environment)
    if (app.Environment.IsDevelopment() && !db.Clientes.Any())
    {
        // Configuração da empresa
        db.ConfiguracaoEmpresa.Add(new ConfiguracaoEmpresa
        {
            Nome = "TechCel Assistência Técnica",
            CpfCnpj = "12.345.678/0001-90",
            Telefone = "(11) 3456-7890",
            WhatsApp = "(11) 99876-5432",
            Endereco = "Rua das Tecnologias, 123 - Centro - São Paulo/SP",
            TextoPadraoOrdem = "Garantia de 90 dias sobre o serviço realizado. Peças com garantia do fabricante.",
            PrazoPadraoGarantiaDias = 90
        });

        // Clientes
        var clientes = new List<Cliente>
        {
            new() { Nome = "Maria Silva", CpfCnpj = "123.456.789-00", Telefone = "(11) 98765-4321", WhatsApp = "(11) 98765-4321", Email = "maria@email.com", Endereco = "Rua A, 100", Bairro = "Centro", Cidade = "São Paulo", Estado = "SP", Cep = "01000-000" },
            new() { Nome = "João Santos", CpfCnpj = "987.654.321-00", Telefone = "(11) 97654-3210", WhatsApp = "(11) 97654-3210", Email = "joao@email.com", Endereco = "Rua B, 200", Bairro = "Vila Nova", Cidade = "São Paulo", Estado = "SP", Cep = "02000-000" },
            new() { Nome = "Ana Oliveira", CpfCnpj = "456.789.123-00", Telefone = "(11) 96543-2109", WhatsApp = "(11) 96543-2109", Email = "ana@email.com", Endereco = "Av. C, 300", Bairro = "Jardim", Cidade = "São Paulo", Estado = "SP", Cep = "03000-000" },
            new() { Nome = "Pedro Costa", CpfCnpj = "789.123.456-00", Telefone = "(11) 95432-1098", WhatsApp = "(11) 95432-1098", Email = "pedro@email.com", Endereco = "Rua D, 400", Bairro = "Mooca", Cidade = "São Paulo", Estado = "SP", Cep = "04000-000" },
            new() { Nome = "Lucia Ferreira", CpfCnpj = "321.654.987-00", Telefone = "(11) 94321-0987", WhatsApp = "(11) 94321-0987", Email = "lucia@email.com", Endereco = "Rua E, 500", Bairro = "Liberdade", Cidade = "São Paulo", Estado = "SP", Cep = "05000-000" }
        };
        db.Clientes.AddRange(clientes);
        await db.SaveChangesAsync();

        // Aparelhos
        var aparelhos = new List<Aparelho>
        {
            new() { ClienteId = clientes[0].Id, Tipo = "Smartphone", Marca = "Samsung", Modelo = "Galaxy S23", Cor = "Preto", Imei = "111111111111111", EstadoAparelho = "Tela trincada" },
            new() { ClienteId = clientes[0].Id, Tipo = "Notebook", Marca = "Dell", Modelo = "Inspiron 15", Cor = "Prata", NumeroSerie = "DELL123456", EstadoAparelho = "Não liga" },
            new() { ClienteId = clientes[1].Id, Tipo = "Smartphone", Marca = "Apple", Modelo = "iPhone 14", Cor = "Branco", Imei = "222222222222222", EstadoAparelho = "Bateria viciada" },
            new() { ClienteId = clientes[2].Id, Tipo = "Tablet", Marca = "Samsung", Modelo = "Galaxy Tab A8", Cor = "Cinza", Imei = "333333333333333", EstadoAparelho = "Touch não funciona" },
            new() { ClienteId = clientes[3].Id, Tipo = "Smartphone", Marca = "Xiaomi", Modelo = "Redmi Note 12", Cor = "Azul", Imei = "444444444444444", EstadoAparelho = "Conector de carga com defeito" },
            new() { ClienteId = clientes[4].Id, Tipo = "Smartphone", Marca = "Motorola", Modelo = "Moto G73", Cor = "Preto", Imei = "555555555555555", EstadoAparelho = "Câmera embaçada" }
        };
        db.Aparelhos.AddRange(aparelhos);
        await db.SaveChangesAsync();

        // Produtos
        var produtos = new List<Produto>
        {
            new() { Codigo = "TEL001", Nome = "Tela Samsung Galaxy S23", Categoria = "Telas", Marca = "Samsung", PrecoCusto = 250.00m, PrecoVenda = 450.00m, QuantidadeEstoque = 5, EstoqueMinimo = 2 },
            new() { Codigo = "TEL002", Nome = "Tela iPhone 14", Categoria = "Telas", Marca = "Apple", PrecoCusto = 400.00m, PrecoVenda = 700.00m, QuantidadeEstoque = 3, EstoqueMinimo = 2 },
            new() { Codigo = "BAT001", Nome = "Bateria Samsung Galaxy S23", Categoria = "Baterias", Marca = "Samsung", PrecoCusto = 80.00m, PrecoVenda = 150.00m, QuantidadeEstoque = 8, EstoqueMinimo = 3 },
            new() { Codigo = "BAT002", Nome = "Bateria iPhone 14", Categoria = "Baterias", Marca = "Apple", PrecoCusto = 120.00m, PrecoVenda = 200.00m, QuantidadeEstoque = 4, EstoqueMinimo = 3 },
            new() { Codigo = "CAB001", Nome = "Conector USB-C", Categoria = "Conectores", Marca = "Genérico", PrecoCusto = 15.00m, PrecoVenda = 40.00m, QuantidadeEstoque = 20, EstoqueMinimo = 5 },
            new() { Codigo = "FLE001", Nome = "Flex de Carga Samsung", Categoria = "Flex", Marca = "Samsung", PrecoCusto = 30.00m, PrecoVenda = 80.00m, QuantidadeEstoque = 6, EstoqueMinimo = 3 },
            new() { Codigo = "CAM001", Nome = "Câmera Traseira Moto G73", Categoria = "Câmeras", Marca = "Motorola", PrecoCusto = 60.00m, PrecoVenda = 120.00m, QuantidadeEstoque = 2, EstoqueMinimo = 2 },
            new() { Codigo = "TOL001", Nome = "Kit Chaves Torx", Categoria = "Ferramentas", Marca = "Genérico", PrecoCusto = 25.00m, PrecoVenda = 50.00m, QuantidadeEstoque = 10, EstoqueMinimo = 3 }
        };
        db.Produtos.AddRange(produtos);
        await db.SaveChangesAsync();

        // Ordens de Serviço
        var hoje = DateTime.UtcNow;
        var ordens = new List<OrdemServico>
        {
            new() { Numero = "OS-2026-0001", ClienteId = clientes[0].Id, AparelhoId = aparelhos[0].Id, DefeitoRelatado = "Tela trincada após queda", Diagnostico = "Display LCD danificado, touch funcional", ServicoRealizado = "Troca de tela completa", DataEntrada = hoje.AddDays(-10), PrevisaoEntrega = hoje.AddDays(-3), DataConclusao = hoje.AddDays(-4), DataEntrega = hoje.AddDays(-3), Status = StatusOrdemServico.Entregue, ValorServico = 150.00m, ValorPecas = 450.00m, ValorTotal = 600.00m, ValorPago = 600.00m },
            new() { Numero = "OS-2026-0002", ClienteId = clientes[1].Id, AparelhoId = aparelhos[2].Id, DefeitoRelatado = "Bateria dura menos de 2 horas", Diagnostico = "Bateria com ciclo de carga acima de 800", DataEntrada = hoje.AddDays(-5), PrevisaoEntrega = hoje.AddDays(2), Status = StatusOrdemServico.EmManutencao, ValorServico = 80.00m, ValorPecas = 200.00m, ValorTotal = 280.00m, ValorPago = 140.00m },
            new() { Numero = "OS-2026-0003", ClienteId = clientes[2].Id, AparelhoId = aparelhos[3].Id, DefeitoRelatado = "Touch não responde em parte da tela", Diagnostico = "Digitizer com defeito", DataEntrada = hoje.AddDays(-3), PrevisaoEntrega = hoje.AddDays(5), Status = StatusOrdemServico.AguardandoAprovacao, ValorServico = 120.00m, ValorPecas = 350.00m, ValorTotal = 470.00m },
            new() { Numero = "OS-2026-0004", ClienteId = clientes[3].Id, AparelhoId = aparelhos[4].Id, DefeitoRelatado = "Não carrega", Diagnostico = "Conector de carga danificado", DataEntrada = hoje.AddDays(-2), PrevisaoEntrega = hoje.AddDays(3), Status = StatusOrdemServico.EmAnalise, ValorServico = 60.00m, ValorPecas = 40.00m, ValorTotal = 100.00m },
            new() { Numero = "OS-2026-0005", ClienteId = clientes[4].Id, AparelhoId = aparelhos[5].Id, DefeitoRelatado = "Câmera traseira embaçada", DataEntrada = hoje.AddDays(-1), PrevisaoEntrega = hoje.AddDays(7), Status = StatusOrdemServico.Aberta, ValorServico = 70.00m, ValorPecas = 120.00m, ValorTotal = 190.00m },
            new() { Numero = "OS-2026-0006", ClienteId = clientes[0].Id, AparelhoId = aparelhos[1].Id, DefeitoRelatado = "Notebook não liga", Diagnostico = "Placa com curto-circuito", DataEntrada = hoje.AddDays(-15), PrevisaoEntrega = hoje.AddDays(-8), DataConclusao = hoje.AddDays(-9), DataEntrega = hoje.AddDays(-8), Status = StatusOrdemServico.Entregue, ValorServico = 300.00m, ValorPecas = 0m, ValorTotal = 300.00m, ValorPago = 300.00m }
        };
        db.OrdensServico.AddRange(ordens);
        await db.SaveChangesAsync();

        // Pagamentos
        var pagamentos = new List<Pagamento>
        {
            new() { OrdemServicoId = ordens[0].Id, DataPagamento = hoje.AddDays(-3), Valor = 600.00m, FormaPagamento = FormaPagamento.Pix, Observacao = "Pagamento integral via PIX" },
            new() { OrdemServicoId = ordens[1].Id, DataPagamento = hoje.AddDays(-4), Valor = 140.00m, FormaPagamento = FormaPagamento.CartaoCredito, Observacao = "Entrada - 50%" },
            new() { OrdemServicoId = ordens[5].Id, DataPagamento = hoje.AddDays(-8), Valor = 300.00m, FormaPagamento = FormaPagamento.Dinheiro, Observacao = "Pagamento na entrega" }
        };
        db.Pagamentos.AddRange(pagamentos);

        // Movimentações de estoque
        var movimentacoes = new List<MovimentacaoEstoque>
        {
            new() { ProdutoId = produtos[0].Id, Tipo = TipoMovimentacao.Saida, Quantidade = 1, DataMovimentacao = hoje.AddDays(-8), Motivo = "Usado na OS-2026-0001", OrdemServicoId = ordens[0].Id, UsuarioResponsavel = "admin@assistencia.com" },
            new() { ProdutoId = produtos[3].Id, Tipo = TipoMovimentacao.Saida, Quantidade = 1, DataMovimentacao = hoje.AddDays(-3), Motivo = "Usado na OS-2026-0002", OrdemServicoId = ordens[1].Id, UsuarioResponsavel = "admin@assistencia.com" },
            new() { ProdutoId = produtos[4].Id, Tipo = TipoMovimentacao.Entrada, Quantidade = 10, DataMovimentacao = hoje.AddDays(-20), Motivo = "Compra de fornecedor", UsuarioResponsavel = "admin@assistencia.com" },
            new() { ProdutoId = produtos[1].Id, Tipo = TipoMovimentacao.Entrada, Quantidade = 5, DataMovimentacao = hoje.AddDays(-15), Motivo = "Reposição de estoque", UsuarioResponsavel = "admin@assistencia.com" }
        };
        db.MovimentacoesEstoque.AddRange(movimentacoes);

        // Agendamentos
        var agendamentos = new List<Agendamento>
        {
            new() { OrdemServicoId = ordens[1].Id, ClienteId = clientes[1].Id, Titulo = "Entrega iPhone 14 - João", Descricao = "Cliente confirmou horário", DataInicio = hoje.AddDays(2).AddHours(10), DataFim = hoje.AddDays(2).AddHours(11), Tipo = TipoAgendamento.Entrega, Status = StatusAgendamento.Agendado },
            new() { OrdemServicoId = ordens[2].Id, ClienteId = clientes[2].Id, Titulo = "Aprovação orçamento - Ana", Descricao = "Ligar para cliente sobre aprovação", DataInicio = hoje.AddDays(1).AddHours(14), DataFim = hoje.AddDays(1).AddHours(14).AddMinutes(30), Tipo = TipoAgendamento.Outro, Status = StatusAgendamento.Agendado },
            new() { ClienteId = clientes[3].Id, Titulo = "Revisão Xiaomi - Pedro", DataInicio = hoje.AddDays(5).AddHours(9), DataFim = hoje.AddDays(5).AddHours(10), Tipo = TipoAgendamento.Revisao, Status = StatusAgendamento.Agendado },
            new() { OrdemServicoId = ordens[0].Id, ClienteId = clientes[0].Id, Titulo = "Retirada Samsung S23", DataInicio = hoje.AddDays(-3).AddHours(16), DataFim = hoje.AddDays(-3).AddHours(17), Tipo = TipoAgendamento.Retirada, Status = StatusAgendamento.Concluido }
        };
        db.Agendamentos.AddRange(agendamentos);

        // Orçamentos
        var orcamentos = new List<Orcamento>
        {
            new() { Numero = "ORC-2026-0001", ClienteId = clientes[2].Id, AparelhoId = aparelhos[3].Id, DefeitoRelatado = "Touch não responde", Diagnostico = "Digitizer com defeito", DataCriacao = hoje.AddDays(-3), DataValidade = hoje.AddDays(10), Status = StatusOrcamento.Enviado, ValorServicos = 120.00m, ValorPecas = 350.00m, ValorTotal = 470.00m },
            new() { Numero = "ORC-2026-0002", ClienteId = clientes[4].Id, AparelhoId = aparelhos[5].Id, DefeitoRelatado = "Câmera embaçada", DataCriacao = hoje.AddDays(-1), DataValidade = hoje.AddDays(13), Status = StatusOrcamento.Rascunho, ValorServicos = 70.00m, ValorPecas = 120.00m, ValorTotal = 190.00m }
        };
        db.Orcamentos.AddRange(orcamentos);
        await db.SaveChangesAsync();

        // Itens do orçamento
        var itensOrcamento = new List<OrcamentoItem>
        {
            new() { OrcamentoId = orcamentos[0].Id, Tipo = TipoItemOrcamento.Servico, Descricao = "Mão de obra troca de digitizer", Quantidade = 1, ValorUnitario = 120.00m },
            new() { OrcamentoId = orcamentos[0].Id, Tipo = TipoItemOrcamento.Peca, Descricao = "Digitizer Galaxy Tab A8", Quantidade = 1, ValorUnitario = 350.00m },
            new() { OrcamentoId = orcamentos[1].Id, Tipo = TipoItemOrcamento.Servico, Descricao = "Mão de obra troca de câmera", Quantidade = 1, ValorUnitario = 70.00m },
            new() { OrcamentoId = orcamentos[1].Id, Tipo = TipoItemOrcamento.Peca, Descricao = "Câmera traseira Moto G73", Quantidade = 1, ValorUnitario = 120.00m }
        };
        db.OrcamentoItens.AddRange(itensOrcamento);

        await db.SaveChangesAsync();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRateLimiter();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();
