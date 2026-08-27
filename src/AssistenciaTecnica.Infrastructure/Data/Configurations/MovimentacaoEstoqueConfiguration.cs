using AssistenciaTecnica.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssistenciaTecnica.Infrastructure.Data.Configurations;

public class MovimentacaoEstoqueConfiguration : IEntityTypeConfiguration<MovimentacaoEstoque>
{
    public void Configure(EntityTypeBuilder<MovimentacaoEstoque> builder)
    {
        builder.ToTable("MovimentacoesEstoque");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .ValueGeneratedOnAdd();

        builder.Property(m => m.ProdutoId)
            .IsRequired();

        builder.Property(m => m.Tipo)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(m => m.Quantidade)
            .IsRequired();

        builder.Property(m => m.DataMovimentacao)
            .IsRequired();

        builder.Property(m => m.Motivo)
            .HasMaxLength(500);

        builder.Property(m => m.UsuarioResponsavel)
            .HasMaxLength(100);

        builder.HasOne(m => m.Produto)
            .WithMany(p => p.Movimentacoes)
            .HasForeignKey(m => m.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.OrdemServico)
            .WithMany(o => o.MovimentacoesEstoque)
            .HasForeignKey(m => m.OrdemServicoId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(m => m.ProdutoId);
        builder.HasIndex(m => m.DataMovimentacao);
    }
}
