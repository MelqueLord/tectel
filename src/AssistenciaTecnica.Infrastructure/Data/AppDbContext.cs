using AssistenciaTecnica.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AssistenciaTecnica.Infrastructure.Data;

public class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Aparelho> Aparelhos => Set<Aparelho>();
    public DbSet<OrdemServico> OrdensServico => Set<OrdemServico>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<MovimentacaoEstoque> MovimentacoesEstoque => Set<MovimentacaoEstoque>();
    public DbSet<Pagamento> Pagamentos => Set<Pagamento>();
    public DbSet<ConfiguracaoEmpresa> ConfiguracaoEmpresa => Set<ConfiguracaoEmpresa>();
    public DbSet<Agendamento> Agendamentos => Set<Agendamento>();
    public DbSet<Tecnico> Tecnicos => Set<Tecnico>();
    public DbSet<Orcamento> Orcamentos => Set<Orcamento>();
    public DbSet<OrcamentoItem> OrcamentoItens => Set<OrcamentoItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrcamentoItem>()
            .Ignore(i => i.Subtotal);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
